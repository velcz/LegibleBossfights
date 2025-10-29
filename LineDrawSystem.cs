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
        public override void PostDrawTiles()
        {
            if (Main.gameMenu || Main.mapFullscreen) return;
            var lp = Main.LocalPlayer?.GetModPlayer<LegiblePlayer>();
            if (lp is null || !lp.ShowLine) return;

            Vector2 start = (Main.LocalPlayer.MountedCenter + new Vector2(0f, Main.LocalPlayer.gfxOffY));
            Vector2 end = GetMouseWorldUnclamped(); // mouse in world -> screen

            Vector2 diff = end - start;
            float len = diff.Length();
            if (len <= 1f) return;

            float rot = diff.ToRotation();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(
                LegibleBossfights.LineTexture,
                start - Main.screenPosition,
                new Rectangle(0,0,1,LegibleBossfights.TotalLineThickness),
                Color.White * LegibleBossfights.LineAlpha,
                rot,
                new Vector2(0, LegibleBossfights.TotalLineThickness * 0.5f),
                new Vector2(len, 1),
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
