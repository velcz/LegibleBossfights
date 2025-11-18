using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.WorldBuilding;

namespace LegibleBossfights
{
    public class MiscHooks : ModSystem
    {
        public override void Load()
        {
            On_WallDrawing.DrawWalls += TileDrawing_DrawWalls_Skip;
            
        }
        public override void PostSetupContent()
        {
            On_CameraModifierStack.Add += AddHook;
            On_PunchCameraModifier.Update += updateskip;
            base.PostSetupContent();
           
        }
        public override void Unload()
        {
            On_WallDrawing.DrawWalls -= TileDrawing_DrawWalls_Skip;
            On_PunchCameraModifier.Update -= updateskip;
            On_CameraModifierStack.Add -= AddHook;
        }
        /// <summary>
        /// For some unknown reason, this hook stops calling when calamity mod is present. I have absolutely no idea why.
        /// </summary>
        public void AddHook(On_CameraModifierStack.orig_Add orig, CameraModifierStack self, ICameraModifier modifier)
        {
            if (LegibleBossfights.NoShaking && modifier is PunchCameraModifier)
            {
                return;
            }
            orig(self, modifier);
        }

        public void updateskip(On_PunchCameraModifier.orig_Update orig, PunchCameraModifier self, ref CameraInfo cameraInfo)
        {
            if (LegibleBossfights.NoShaking)
                return;
            orig(self, ref cameraInfo);
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
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
