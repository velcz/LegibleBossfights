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
        }
        public string getboolname(bool b) => b ? "ON" : "OFF";
    }
}
