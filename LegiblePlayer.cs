using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace LegibleBossfights
{

    public class LegiblePlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !Main.dedServ;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (LegibleBossfights.ToggleLineKey.JustPressed)
            {
                LegibleBossfights.ShowLine = !LegibleBossfights.ShowLine;
                Main.NewText("Cursor Line: " + getboolname(LegibleBossfights.ShowLine), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleProjKey.JustPressed)
            {
                LegibleBossfights.ShowCircles = !LegibleBossfights.ShowCircles;
                Main.NewText("Enemy Projectile Indicators: " + getboolname(LegibleBossfights.ShowCircles), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleTransparentKey.JustPressed)
            {
                LegibleBossfights.FadeProjectiles = !LegibleBossfights.FadeProjectiles;
                Main.NewText("Transparent Friendly Projectiles: " + getboolname(LegibleBossfights.FadeProjectiles), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleHideWallsKey.JustPressed)
            {
                LegibleBossfights.HideWalls = !LegibleBossfights.HideWalls;

                if (Main.netMode != Terraria.ID.NetmodeID.Server)
                    Main.NewText($"Hide Walls: {(LegibleBossfights.HideWalls ? "ON" : "OFF")}", 200, 200, 255);
            }
        }
        public string getboolname(bool b) => b ? "ON" : "OFF";
    }
}
