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
        public bool ShowLine;
        public override bool IsLoadingEnabled(Mod mod)
        {
            // Only load this mod on clients, not on dedicated servers
            return !Main.dedServ;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (LegibleBossfights.ToggleLineKey.JustPressed)
                ShowLine = !ShowLine;
        }
    }
}
