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

        public static float ProjCircleRadius = 100; //128 = 1x, 64 = 2x, 32 = 4x, 16 = 8x, 8 = 16x, 4 = 32x, 2 = 64x, 1 = 128x;
        public static float ProjCircleMaxAlpha = 1f;
        public static float ProjCircleMinAlpha = .9f;


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Find vanilla layer indices we want to be UNDER
            int invIdx = layers.FindIndex(l => l.Name.Equals("Vanilla: Inventory", StringComparison.Ordinal));
            int mapIdx = layers.FindIndex(l => l.Name.Equals("Vanilla: Map / Minimap", StringComparison.Ordinal));

            // Choose the earliest one that exists. If neither exists, we’ll just add at the end.
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
                InterfaceScaleType.UI // use UI scaling, not world scaling
            ));
        }
        public void drawinterface()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            float fadesize = LegibleBossfights.FadeSize;
            // if (Main.mapFullscreen) return;
            var lp = Main.LocalPlayer?.GetModPlayer<LegiblePlayer>();
            Vector2 playerworldpos = Main.LocalPlayer.MountedCenter + new Vector2(0f, Main.LocalPlayer.gfxOffY);
            // Convert world -> screen (accounts for zoom)
            Vector2 playerposition = GetScreenCoords(playerworldpos);
            Vector2 mouseposition = Main.MouseScreen;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            #region ProjectileHighlight
            if (LegibleBossfights.ShowCircles)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    ref Projectile p = ref Main.projectile[i];
                    if (!p.active || !p.hostile) continue;
                    if (!IsOnScreen(p.Hitbox)) continue;
                    //adjustables
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
                            // Distance above the threshold, normalized 0→1
                            float t = (ProjCircleFinalDistance + ProjCircleGrowDistance - disttoplayer) / ProjCircleGrowDistance;
                            // Grow from 0 to 32 as t goes 0→1
                            float scale = ProjCircleMaxRadiusBoost * t;
                            boostalpha = 1;
                            radiusx += scale;
                            radiusy += scale;
                        }
                        else if (disttoplayer < ProjCircleFinalDistance)
                        {
                            // Distance below the threshold, normalized 0→1 (1 at far edge, 0 when touching)
                            float t = disttoplayer / ProjCircleFinalDistance;
                            boostalpha = 1;
                            // Shrink from 32→0
                            float scale = ProjCircleMaxRadiusBoost * t;
                            radiusx += scale;
                            radiusy += scale;
                        }
                        byte alphaamnt = (byte)(255 * (ProjCircleMinAlpha + alphamaxminusmin * boostalpha));
                        Vector2 partpos = GetScreenCoords(p.Center);
                        Color ringcoloradapted = new Color(RingColor.R, RingColor.G, RingColor.B, alphaamnt);
                        //DrawCircle(spriteBatch, partpos, radiusx, radiusy, ringcoloradapted, Thickness, Segments);
                        spriteBatch.Draw(
                                ((Texture2D)LegibleBossfights.RingTexture),
                                partpos,
                                null,
                                ringcoloradapted,
                                0f,
                                new Vector2(64, 64),
                                new Vector2(radiusx / ProjCircleRadius, radiusy / ProjCircleRadius),
                                SpriteEffects.None,
                                0f
                                );
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
            #endregion
            #region Cursor Line
            if (LegibleBossfights.ShowLine)
            {
                float killfade = fadesize * 2;
                Vector2 diff = mouseposition - playerposition;
                float len = diff.Length();
                len = MathF.Max(-fadesize / 2, len - fadesize * 2);
                //if (len <= 0) imvisible = false;
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
           
            //spriteBatch.End();
            //spriteBatch.Begin();
        }

        private Vector2 GetScreenCoords(Vector2 pos)
        {
            return Vector2.Transform(pos - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix) / Main.UIScale;
        }
        private static bool IsLaserLike(Projectile p)
    => p.aiStyle == ProjAIStyleID.ThickLaser
       || p.type == ProjectileID.SaucerDeathray
       || p.type == ProjectileID.EyeLaser;
        private static void DrawCircle(SpriteBatch sb, Vector2 centerWorld, float radiusx, float radiusy, Color color, float thickness, int segments)
        {
            float step = MathHelper.TwoPi / segments;
            Vector2 prev = centerWorld + new Vector2(radiusx, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float ang = step * i;
                Vector2 next = centerWorld + new Vector2(MathF.Cos(ang) * radiusx, MathF.Sin(ang) * radiusy);
                DrawLine(sb, prev, next, color, thickness);
                prev = next;
            }
        }

        private static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            float angle = MathF.Atan2(edge.Y, edge.X);
            sb.Draw(LegibleBossfights.PixelTexture, start, null, color, angle, Vector2.Zero, new Vector2(edge.Length(), thickness), SpriteEffects.None, 0f);
        }

        private static bool IsOnScreen(Rectangle worldRect)
        {
            Rectangle screenWorldRect = new Rectangle((int)Main.screenPosition.X-100, (int)Main.screenPosition.Y-100, Main.screenWidth+200, Main.screenHeight+200);
            return worldRect.Intersects(screenWorldRect);
        }
    }
}
