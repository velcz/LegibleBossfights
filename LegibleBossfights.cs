using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace LegibleBossfights
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class LegibleBossfights : Mod
	{
        public static ModKeybind ToggleLineKey { get; private set; }
        public static ModKeybind ToggleProjKey { get; private set; }
        public static Texture2D LineTexture { get; private set; }
        public static Texture2D LineFadeTexture { get; private set; }
        public static Texture2D PixelTexture { get; private set; }
        public static Asset<Texture2D> RingTexture;
        public static BlendState OverwriteBlend { get; private set; }
        public static int TotalLineThickness = 0;
        public static int LineThickness = 0;
        public static int LineBorderThickeness = 0;
        public static float LineAlpha = 0.9f;
        public const int MaxTextureSize = 64;
        public const int FadeSize = 32;

        public static bool AutoLine, AutoFriendlyProjectileHide, AutoCircles;

        public static float ProjectileTransparency;
        public static float ParticleRate;

        public static bool ShowLine;
        public static bool ShowCircles;
        public static bool FadeProjectiles;

        public override void Load()
        {
            RingTexture = ModContent.Request<Texture2D>("LegibleBossfights/assets/ring", AssetRequestMode.ImmediateLoad);
            OverwriteBlend = new BlendState
            {
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero
            };
            // Defer pixel creation to the main thread
            Main.QueueMainThreadAction(() =>
            {
                LineTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, MaxTextureSize);
                LineFadeTexture = new Texture2D(Main.graphics.GraphicsDevice, FadeSize, MaxTextureSize);
                PixelTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
                PixelTexture.SetData(new[] { Color.White });
            });
            ToggleLineKey = KeybindLoader.RegisterKeybind(this, "Toggle Cursor Line", "L");
            ToggleProjKey = KeybindLoader.RegisterKeybind(this, "Toggle Projectile Highlights", "P");
            base.Load();
        }
        public override void PostSetupContent()
        {
            ModContent.GetInstance<LegibleBossfightsConfig>().SetLineThickness();
            base.PostSetupContent();
        }
        public override void Unload()
        {
            if (!Main.dedServ)
                Main.QueueMainThreadAction(() => { 
                    LineTexture?.Dispose(); LineTexture = null;
                    LineFadeTexture?.Dispose(); LineFadeTexture = null;
                    PixelTexture?.Dispose(); PixelTexture = null;
                });
        }
        }
}
