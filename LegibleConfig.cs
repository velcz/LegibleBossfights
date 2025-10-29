using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
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
        [Header("headerDeclutter")]
        [DefaultValue(1f)]
        [Slider]
        public float TransparentFriendlyProjectiles;

        [Header("headerLine")]
        [DefaultValue(typeof(Color), "255, 56, 56, 255")] // R, G, B, A
        [ColorNoAlpha]
        public Color LineColor { get; set; }

        [DefaultValue(typeof(Color), "255, 255, 255, 255")]
        [ColorNoAlpha]
        public Color OutlineColor { get; set; }

        [Range(1, 10)]
        [DefaultValue(2)]
        [Slider]
        
        public int LineThickness { get; set; }

        [Range(0, 10)]
        [DefaultValue(1)]
        [Slider]
        public int LineBorder { get; set; }


        [Range(0f, 1f)]
        [DefaultValue(0.67f)]
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
            Color[] datafade = new Color[LegibleBossfights.FadeSize * LegibleBossfights.MaxTextureSize];
            for (int i = 0; i < LegibleBossfights.MaxTextureSize; i++)
            {
                Color col = i < LineBorder || i > LineBorder + LineThickness ? OutlineColor : LineColor;
                data[i] = col;
                for (int j = 0; j < LegibleBossfights.FadeSize; j++)
                {
                    Color col2 = col;
                    col.A = (byte)(255 * ((float)j / LegibleBossfights.FadeSize));
                    datafade[i * LegibleBossfights.FadeSize + j] = col;
                }
            }
            Main.QueueMainThreadAction(() =>
            {
                LegibleBossfights.LineTexture.SetData(data);
                LegibleBossfights.LineFadeTexture.SetData(datafade);
            });

        }
    }
}
