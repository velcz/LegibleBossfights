using Iced.Intel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace LegibleBossfights
{
    public class LineDrawSystem : ModSystem
    {
        public float visiblefade = 1f;

        public static float BaseRadiusPadding = 2f;
        public static float Thickness = 3f;
        public static Color RingColor = new Color(255, 60, 60, 255);
        public static int Segments = 56;


        public static float ProjCircleFinalDistance = 120;
        public static float ProjCircleGrowDistance = 80f;
        public static float ProjCircleMaxRadiusBoost = 24f;
        public static float ProjAlphaDistance = 512f;

        public static float ProjCircleRadius = 100;
        public static float ProjCircleMaxAlpha = 1f;
        public static float ProjCircleMinAlpha = .9f;


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int invIdx = layers.FindIndex(l => l.Name.Equals("Vanilla: Inventory", StringComparison.Ordinal));
            int mapIdx = layers.FindIndex(l => l.Name.Equals("Vanilla: Map / Minimap", StringComparison.Ordinal));

            int insertAt = int.MaxValue;
            if (invIdx >= 0) insertAt = Math.Min(insertAt, invIdx);
            if (mapIdx >= 0) insertAt = Math.Min(insertAt, mapIdx);
            if (insertAt == int.MaxValue) insertAt = layers.Count;

            layers.Insert(insertAt, new LegacyGameInterfaceLayer(
                "MyMod: UIUnderInvAndMinimap",
                delegate {
                    drawinterface();
                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
        private static List<(Projectile,byte)> passedprojlist = new List<(Projectile, byte)>(10000);
        public void drawinterface()
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            float fadesize = LegibleBossfights.FadeSize;
            var lp = Main.LocalPlayer?.GetModPlayer<LegiblePlayer>();
            Vector2 playerworldpos = Main.LocalPlayer.MountedCenter + new Vector2(0f, Main.LocalPlayer.gfxOffY);
            Vector2 playerposition = GetScreenCoords(playerworldpos);
            Vector2 mouseposition = Main.MouseScreen;
           
            #region ProjectileHighlight
            if (LegibleBossfights.ShowCircles)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                passedprojlist.Clear();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    ref Projectile p = ref Main.projectile[i];
                    if (!p.active || !p.hostile) continue;
                    if (!IsOnScreen(p.Hitbox)) continue;
                    float alphamaxminusmin = ProjCircleMaxAlpha - ProjCircleMinAlpha;

                    if (!IsLaserLike(p))
                    {
                        float radiusx = p.width + BaseRadiusPadding;
                        float radiusy = p.height + BaseRadiusPadding;
                        float disttoplayer = (p.Center - playerworldpos).Length();
                        float boostalpha = 0;
                        if (disttoplayer < ProjCircleFinalDistance + ProjAlphaDistance && disttoplayer > ProjCircleFinalDistance + ProjCircleGrowDistance)
                        {
                            float t = (ProjCircleFinalDistance + ProjAlphaDistance - disttoplayer) / ProjAlphaDistance;
                            boostalpha = 1 * t;
                        }
                        else if (disttoplayer < ProjCircleFinalDistance + ProjCircleGrowDistance && disttoplayer > ProjCircleFinalDistance)
                        {
                            float t = (ProjCircleFinalDistance + ProjCircleGrowDistance - disttoplayer) / ProjCircleGrowDistance;
                            float scale = ProjCircleMaxRadiusBoost * t;
                            boostalpha = 1;
                            radiusx += scale;
                            radiusy += scale;
                        }
                        else if (disttoplayer < ProjCircleFinalDistance)
                        {
                            float t = disttoplayer / ProjCircleFinalDistance;
                            boostalpha = 1;
                            float scale = ProjCircleMaxRadiusBoost * t;
                            radiusx += scale;
                            radiusy += scale;
                        }
                        float drawnRadX = radiusx / ProjCircleRadius;
                        float drawnRadY = radiusy / ProjCircleRadius;

                        float checkboundx = TextureAssets.Projectile[p.type].Value.Width * p.scale;
                        float checkboundy = TextureAssets.Projectile[p.type].Value.Height * p.scale;

                        if (checkboundx < LegibleBossfights.MaxHighlightSize || checkboundy < LegibleBossfights.MaxHighlightSize)
                        {
                            byte alphaamnt = (byte)(255 * (ProjCircleMinAlpha + alphamaxminusmin * boostalpha));
                            passedprojlist.Add((p, alphaamnt));
                            Vector2 partpos = GetScreenCoords(p.Center);
                            if (LegibleBossfights.DrawCircleHighlights)
                            {
                                Color ringcoloradapted = new Color(RingColor.R, RingColor.G, RingColor.B, alphaamnt);
                                spriteBatch.Draw(
                                        ((Texture2D)LegibleBossfights.RingTexture),
                                        partpos,
                                        null,
                                        ringcoloradapted,
                                        0f,
                                        new Vector2(64, 64),
                                        new Vector2(drawnRadX, drawnRadY),
                                        SpriteEffects.None,
                                        0f
                                        );
                            }
                            if (LegibleBossfights.ProjectileHighlightMode == ProjectileHighlightMode.Simple)
                            {
                                Texture2D tex = TextureAssets.Projectile[p.type].Value;
                                Rectangle frame = tex.Frame(1, Main.projFrames[p.type], 0, p.frame);
                                Vector2 origin = frame.Size() / 2f;
                                SpriteEffects effects = p.spriteDirection == 1
                                    ? SpriteEffects.None
                                    : SpriteEffects.FlipHorizontally;
                                spriteBatch.Draw(
                                    tex,
                                    partpos,
                                    frame,
                                    new Color(RingColor.R, RingColor.G, RingColor.B, alphaamnt),
                                    p.rotation,
                                    origin,
                                    p.scale,
                                    effects,
                                    0f
                                    );
                            }
                        }
                    }

                }
                
                //DO FANCY HIGHLIGHT DRAWING
                if (LegibleBossfights.ProjectileHighlightMode == ProjectileHighlightMode.Fancy)
                {
                    Effect effect = LegibleBossfights.HighlightShader;
                    EffectParameter effectParameter = effect.Parameters["uImageSize0"];
                    EffectParameter effectParameter2 = effect.Parameters["OutlineThickness"];
                    EffectParameter effectParameter3 = effect.Parameters["OutlineColor"];
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect, Main.UIScaleMatrix);
                    for (int i = 0; i < passedprojlist.Count; i++)
                    {
                        var result = passedprojlist[i];
                        Projectile p = result.Item1;
                        byte alphaamnt = result.Item2;
                        Texture2D tex = TextureAssets.Projectile[p.type].Value;
                        Rectangle frame = tex.Frame(1, Main.projFrames[p.type], 0, p.frame);
                        Vector2 origin = frame.Size() / 2f;
                        SpriteEffects effects = p.spriteDirection == 1
                            ? SpriteEffects.None
                            : SpriteEffects.FlipHorizontally;

                        
                        if (effectParameter != null)
                        {
                            effectParameter.SetValue(new Vector2((float)p.width, (float)p.height));
                        }
                        
                        if (effectParameter2 != null)
                        {
                            effectParameter2.SetValue(2); // outline thickeness
                        }
                        Color outlineColor = RingColor; // config this
                        
                        if (effectParameter3 != null)
                        {
                            effectParameter3.SetValue(outlineColor.ToVector4());
                        }
                        
                        spriteBatch.Draw(
                            tex,
                            GetScreenCoords(p.Center),
                            frame,
                            new Color(255, 255, 255, alphaamnt),
                            p.rotation,
                            origin,
                            p.scale,
                            effects,
                            0f
                            );
                        

                    }
                    
                }
            }
            #endregion
            #region Cursor Line
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            if (LegibleBossfights.ShowLine)
            {
                float killfade = fadesize * 2;
                Vector2 diff = mouseposition - playerposition;
                float len = diff.Length();
                len = MathF.Max(-fadesize / 2, len - fadesize * 2);
                float rot = diff.ToRotation();
                float cutsprite = 0;

                if (len < 0)
                {
                    cutsprite = Math.Abs(len);
                    visiblefade = (fadesize / 2 - Math.Abs(len)) / (fadesize / 2);
                }
                else visiblefade = 1f;

                if (len < 3000)
                {
                    if (len > 0)
                        spriteBatch.Draw(
                            LegibleBossfights.LineTexture,
                            playerposition + new Vector2(MathF.Cos(rot) * fadesize, MathF.Sin(rot) * fadesize),
                            new Rectangle(0, 0, 1, LegibleBossfights.TotalLineThickness),
                            new Color(255, 255, 255, LegibleBossfights.LineAlpha * visiblefade),
                            rot,
                            new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                            new Vector2(len, 1),
                            SpriteEffects.None,
                            0f
                        );
                    spriteBatch.Draw(
                        LegibleBossfights.LineFadeTexture,
                        mouseposition,
                        new Rectangle((int)cutsprite * 2 + 2, 0, (int)fadesize, LegibleBossfights.TotalLineThickness),
                        new Color(255, 255, 255, LegibleBossfights.LineAlpha * visiblefade),
                        rot - MathF.PI,
                        new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                        Vector2.One,
                        SpriteEffects.None,
                        0f
                        );
                    spriteBatch.Draw(
                        LegibleBossfights.LineFadeTexture,
                        playerposition,
                        new Rectangle(0, 0, (int)fadesize - (int)cutsprite, LegibleBossfights.TotalLineThickness),
                        new Color(255, 255, 255, LegibleBossfights.LineAlpha * visiblefade),
                        rot,
                        new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                        Vector2.One,
                        SpriteEffects.None,
                        0f
                        );
                }
            }
            #endregion

        }

        private Vector2 GetScreenCoords(Vector2 pos)
        {
            return Vector2.Transform(pos - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix) / Main.UIScale;
        }
        private static bool IsLaserLike(Projectile p)
    => p.aiStyle == ProjAIStyleID.ThickLaser
       || p.type == ProjectileID.SaucerDeathray
       || p.type == ProjectileID.EyeLaser;

        private static bool IsOnScreen(Rectangle worldRect)
        {
            Rectangle screenWorldRect = new Rectangle((int)Main.screenPosition.X-100, (int)Main.screenPosition.Y-100, Main.screenWidth+200, Main.screenHeight+200);
            return worldRect.Intersects(screenWorldRect);
        }
    }
}
