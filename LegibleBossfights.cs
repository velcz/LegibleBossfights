using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public static Texture2D LineTexture { get; private set; }
        public static BlendState OverwriteBlend { get; private set; }
        public static int TotalLineThickness = 0;
        public static int LineThickness = 0;
        public static int LineBorderThickeness = 0;
        public static float LineAlpha = 0.9f;
        public const int MaxTextureSize = 64;
        public override void Load()
        {
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
                LineTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 64);
                //LineTexture.SetData(new[] { Color.White });
            });
            ToggleLineKey = KeybindLoader.RegisterKeybind(this, "Toggle Cursor Line", "L");
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
                Main.QueueMainThreadAction(() => { LineTexture?.Dispose(); LineTexture = null; });
        }
        }
}
