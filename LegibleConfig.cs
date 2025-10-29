using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI.ModBrowser;

namespace LegibleBossfights
{
    public sealed class LegibleBossfightsConfig : ModConfig
    {
        public static LegibleBossfightsConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Transparent Friendly Projectiles")]
        [Tooltip("Set alpha of all projectiles not marked as evil")]
        [DefaultValue(1f)]
        [Slider]
        public float TransparentFriendlyProjectiles;

        [Label("Line Thickness")]
        [Tooltip("How thick the line from the player to the cursor appears.")]
        [Range(1, 10)]
        [DefaultValue(4)]
        public int LineThickness { get; set; }
        [Label("Line Border Thickness")]
        [Tooltip("How thick the line border should be.")]
        [Range(1, 10)]
        [DefaultValue(4)]
        public int LineBorder { get; set; }

        [Label("Line Transparency")]
        [Tooltip("How visible the line is. 1 = fully visible, 0 = fully transparent.")]
        [Range(0f, 1f)]
        [DefaultValue(0.9f)]
        public float LineAlpha { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            TransparentFriendlyProjectiles = Utils.Clamp(TransparentFriendlyProjectiles, 0f, 1f);
        }
        public override void OnChanged()
        {
            SetLineThickness();
                
        }
        public void SetLineThickness()
        {
            if (LegibleBossfights.LineTexture is null)
                return;
            LegibleBossfights.LineAlpha = LineAlpha;
            LegibleBossfights.LineThickness = LineThickness;
            LegibleBossfights.LineBorderThickeness = LineBorder;
            LegibleBossfights.TotalLineThickness = LineBorder * 2 + LineThickness + 1;
            Color linecolor = Color.Red;
            Color bordercolor = Color.Black;
            Color[] data = new Color[LegibleBossfights.MaxTextureSize];
            for (int i = 0; i < LegibleBossfights.MaxTextureSize; i++)
            {
                data[i] = i < LineBorder || i > LineBorder + LineThickness ? bordercolor: linecolor ;
            }
                //
            Main.QueueMainThreadAction(() =>
            {
                LegibleBossfights.LineTexture.SetData(data);
                });

        }
    }
}
