using Iced.Intel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace LegibleBossfights
{
    public class LineDrawSystem : ModSystem
    {
        public bool imvisible = false;
        public float visiblefade = 1f;
        public override void PostDrawTiles()
        {
            float fadesize = LegibleBossfights.FadeSize;
            if (Main.gameMenu || Main.mapFullscreen) return;
            var lp = Main.LocalPlayer?.GetModPlayer<LegiblePlayer>();
            if (lp is not null) imvisible = lp.ShowLine;
            Vector2 start = (Main.LocalPlayer.MountedCenter + new Vector2(0f, Main.LocalPlayer.gfxOffY));
            Vector2 end = GetMouseWorldUnclamped(); // mouse in world -> screen

            float killfade = fadesize * 2;
            Vector2 diff = end - start;
            float len = diff.Length();
            len = MathF.Max(-fadesize/2, len - fadesize*2);
            //if (len <= 0) imvisible = false;
            float rot = diff.ToRotation();
            float cutsprite = 0;

            if (len < 0)
            {
                cutsprite = Math.Abs(len);
                visiblefade = (fadesize / 2 - Math.Abs(len)) / (fadesize / 2);
            }
            else visiblefade = 1f;

                //fade line on/off
                /*
                float fadeSpeed = 8; // seconds to fully fade
                float dt = 60f / Main.frameRate / fadeSpeed;
                if (imvisible && visiblefade < 1)
                    visiblefade = Math.Min(visiblefade + dt, 1);
                else if (!imvisible && visiblefade > 0)
                    visiblefade = Math.Max(visiblefade - dt, 0);
                */


                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(
                LegibleBossfights.LineTexture,
                start + new Vector2(MathF.Cos(rot)* fadesize, MathF.Sin(rot)* fadesize) - Main.screenPosition,
                new Rectangle(0,0,1,LegibleBossfights.TotalLineThickness),
                Color.White * LegibleBossfights.LineAlpha * visiblefade,
                rot,
                new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                new Vector2(len, 1),
                SpriteEffects.None,
                0f
            );
            Main.spriteBatch.Draw(
                LegibleBossfights.LineFadeTexture,
                end - Main.screenPosition,
                new Rectangle(0, 0, (int)fadesize - (int)cutsprite, LegibleBossfights.TotalLineThickness),
                Color.White * LegibleBossfights.LineAlpha * visiblefade,
                rot-MathF.PI,
                new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                Vector2.One,
                SpriteEffects.None,
                0f
                );
            Main.spriteBatch.Draw(
                LegibleBossfights.LineFadeTexture,
                start - Main.screenPosition,
                new Rectangle(0, 0, (int)fadesize- (int)cutsprite, LegibleBossfights.TotalLineThickness),
                Color.White * LegibleBossfights.LineAlpha * visiblefade,
                rot,
                new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                Vector2.One,
                SpriteEffects.None,
                0f
                );

            Main.spriteBatch.End();
        }
        private static Vector2 GetMouseWorldUnclamped()
        {
            // Start with screen-space mouse
            Vector2 ms = Main.MouseScreen;

            // Gravity flips Y in Terraria — undo that before unprojecting
            if (Main.LocalPlayer?.gravDir == -1f)
                ms.Y = Main.screenHeight - ms.Y;

            // Unproject by removing zoom, then add world camera position
            Matrix invZoom = Matrix.Invert(Main.GameViewMatrix.ZoomMatrix);
            return Vector2.Transform(ms, invZoom) + Main.screenPosition;
        }
    }
}
