using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
	public class LegibleBossfights : Mod
	{
        public static ModKeybind ToggleLineKey { get; private set; }
        public static ModKeybind ToggleProjKey { get; private set; }
        public static ModKeybind ToggleTransparentKey { get; private set; }
        public static ModKeybind ToggleHideWallsKey { get; private set; }
        public static Texture2D LineTexture { get; private set; }
        public static Texture2D LineFadeTexture { get; private set; }

        public static Asset<Texture2D> RingTexture;
        public static BlendState OverwriteBlend { get; private set; }
        public static int TotalLineThickness = 0;
        public static int LineThickness = 0;
        public static int LineBorderThickeness = 0;
        public static float LineAlpha = 0.9f;
        public const int MaxTextureSize = 64;
        public const int FadeSize = 32;

        public static bool AutoLine, AutoFriendlyProjectileHide, AutoCircles, AutoParticle, AutoHideWalls;

        public static float ProjectileTransparency;
        public static float ParticleRate;

        public static bool ShowLine;
        public static bool ShowCircles;
        public static bool FadeProjectiles;
        //public static bool RedrawProjectileSprites;
        public static bool ReduceParticles;
        //public static bool FancyProjectileHighlights;
        public static ProjectileHighlightMode ProjectileHighlightMode;
        public static bool DrawCircleHighlights;

        public static int MaxHighlightSize = 200;

        public static Effect HighlightShader;

        public static bool HideWalls = false;

        public static bool NoShaking = false;


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
            Main.QueueMainThreadAction(() =>
            {
                LineTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, MaxTextureSize);
                LineFadeTexture = new Texture2D(Main.graphics.GraphicsDevice, FadeSize, MaxTextureSize);
            });
            ToggleLineKey = KeybindLoader.RegisterKeybind(this, "Toggle Cursor Line", "L");
            ToggleProjKey = KeybindLoader.RegisterKeybind(this, "Toggle Projectile Highlights", "P");
            ToggleTransparentKey = KeybindLoader.RegisterKeybind(this, "Toggle Transparent Friendly Projectiles", "O");
            ToggleHideWallsKey = KeybindLoader.RegisterKeybind(this, "Toggle Hide Walls", Keys.OemSemicolon);

            HighlightShader = ModContent.Request<Effect>("LegibleBossfights/Effects/glowshader", AssetRequestMode.ImmediateLoad).Value;

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
                });
        }
        }
}
