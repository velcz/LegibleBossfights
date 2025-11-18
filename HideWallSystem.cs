using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace LegibleBossfights
{
    public class HideWallsSystem : ModSystem
    {
        public override void Load()
        {
            On_WallDrawing.DrawWalls += TileDrawing_DrawWalls_Skip;
        }
        public override void Unload()
        {
            On_WallDrawing.DrawWalls -= TileDrawing_DrawWalls_Skip;
        }

        private void TileDrawing_DrawWalls_Skip(
            On_WallDrawing.orig_DrawWalls orig,
            WallDrawing self)
        {
            if (LegibleBossfights.HideWalls)
                return; // skip wall draw pass
            orig(self);
        }
    }
}
