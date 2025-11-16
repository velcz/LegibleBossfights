using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
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

        #region Automatic
        [JsonProperty(Order = 100)]
        [Header("headerAutomatic")]

        [BackgroundColor(0, 0, 0, 255)]
        [DefaultValue(true)]
        public bool TransparentAutoBoss { get; set; }

        [BackgroundColor(0, 0, 0, 255)]
        [DefaultValue(true)]
        public bool LineAutoBoss { get; set; }

        [BackgroundColor(0, 0, 0, 255)]
        [DefaultValue(true)]
        public bool ProjectileCircleAutoBoss { get; set; }

        [BackgroundColor(0, 0, 0, 255)]
        [DefaultValue(true)]
        public bool ParticleAutoBoss { get; set; }
        #endregion
        #region Declutter

        [JsonProperty(Order = 125)]
        [Header("headerDeclutter")]

        [DefaultValue(false)]
        [BackgroundColor(200, 80, 40, 180)]
        public bool OnlySimplifyOtherPlayersProjectiles { get; set; }

        [BackgroundColor(200, 120, 140, 180)]
        [Range(0f, 1f)]
        [DefaultValue(.75f)]
        [Slider]
        public float TransparentFriendlyProjectiles { get; set; }

        [BackgroundColor(200, 120, 140, 180)]
        [DefaultValue(false)]
        public bool SimplifyFriendlyProjectileDrawing { get; set; }

        

        [BackgroundColor(200, 120, 140, 180)]
        [DefaultValue(true)]
        public bool ExcludePets { get; set; }

        [Range(0f, 1f)]
        [DefaultValue(0f)]
        [Slider]
        public float DustReducerChance { get; set; }

        #endregion
        #region Line

        [JsonProperty(Order = 150)]
        [Header("headerLine")]

        [DefaultValue(typeof(Color), "255, 0, 0, 255")] // R, G, B, A
        [ColorNoAlpha]
        public Color LineColor { get; set; }

        [DefaultValue(typeof(Color), "255, 255, 255, 255")]
        [ColorNoAlpha]
        public Color OutlineColor { get; set; }

        [Range(0f, 1f)]
        [DefaultValue(0.8f)]
        public float LineAlpha { get; set; }

        [Range(1, 10)]
        [DefaultValue(4)]
        [Slider]

        public int LineThickness { get; set; }

        [Range(0, 10)]
        [DefaultValue(2)]
        [Slider]
        public int LineBorder { get; set; }

        
        #endregion
        #region Projectile Highlighters

        
        [JsonProperty(Order = 115)]
        [Header("headerProjectiles")]

        [DefaultValue(typeof(Color), "0, 255, 0, 255")]
        [ColorNoAlpha]
        public Color ProjRingColor { get; set; }

        [BackgroundColor(255, 139, 139, 255)]
        [Range(100f, 1500f)]
        [DefaultValue(525f)]
        public float ProjectileCircleDistance { get; set; }

        [BackgroundColor(255, 139, 139, 255)]
        [Range(1f, 8f)]
        [DefaultValue(1.8)]
        public float ProjectileCircleSize { get; set; }

        [BackgroundColor(255, 100, 100, 255)]
        [Range(0f, 500)]
        [DefaultValue(210)]
        public float ProjectileCircleWarningDistance { get; set; }

        [BackgroundColor(255, 100, 100, 255)]
        [Range(0f, 3f)]
        [DefaultValue(1f)]
        public float ProjectileCircleGrow { get; set; }

        [BackgroundColor(255, 200, 200, 255)]
        [Range(0f, 1f)]
        [DefaultValue(0f)]
        public float ProjectileCircleMinAlpha{ get; set; }

        [BackgroundColor(255, 200, 200, 255)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float ProjectileCircleMaxAlpha { get; set; }

        [DefaultValue(true)]
        public bool ProjectileRedrawSprite { get; set; }

        [Range(25, 500)]
        [DefaultValue(150)]
        [Slider]
        public int MaxHighlightSize { get; set; }



        #endregion



        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            TransparentFriendlyProjectiles = Utils.Clamp(TransparentFriendlyProjectiles, 0f, 1f);
        }
        public override void OnChanged()
        {
            SetLineThickness();
            //set projectile variables
            LineDrawSystem.ProjCircleFinalDistance = ProjectileCircleWarningDistance * 0.3f;
            LineDrawSystem.ProjCircleGrowDistance = ProjectileCircleWarningDistance * 0.7f;
            LineDrawSystem.ProjCircleMaxRadiusBoost = ProjectileCircleGrow * 24f;
            LineDrawSystem.ProjAlphaDistance = ProjectileCircleDistance;
            LineDrawSystem.ProjCircleRadius = 128f / ProjectileCircleSize;//when 8, =16, when 1, = 128
            LineDrawSystem.ProjCircleMaxAlpha = ProjectileCircleMaxAlpha;
            LineDrawSystem.ProjCircleMinAlpha = MathF.Min(ProjectileCircleMinAlpha, ProjectileCircleMaxAlpha);
            LegibleBossfights.RedrawProjectileSprites = ProjectileRedrawSprite;
            LegibleBossfights.MaxHighlightSize = MaxHighlightSize;
            //set auto toggles
            LegibleBossfights.AutoLine = LineAutoBoss;
            LegibleBossfights.AutoFriendlyProjectileHide = TransparentAutoBoss;
            LegibleBossfights.AutoCircles = ProjectileCircleAutoBoss;
            LegibleBossfights.AutoParticle = ParticleAutoBoss;

            LegibleBossfights.ProjectileTransparency = 1f - TransparentFriendlyProjectiles;
            LegibleBossfights.ParticleRate = 1f - DustReducerChance;

            LineDrawSystem.RingColor = ProjRingColor;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active) continue;
                LegibleBossfightsGlobalProjectile lp = p.GetGlobalProjectile<LegibleBossfightsGlobalProjectile>();
                if (!Main.projPet[p.type]) continue;

                lp.lowRender = !ExcludePets;
                if (ExcludePets)
                    p.Opacity = 1;
            }
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
