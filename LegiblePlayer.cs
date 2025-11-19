using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
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
                Main.NewText(Language.GetTextValue("Mods.LegibleBossfights.Messages.ShowLine", getboolname(LegibleBossfights.ShowLine)), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleProjKey.JustPressed)
            {
                LegibleBossfights.ShowCircles = !LegibleBossfights.ShowCircles;
                Main.NewText(Language.GetTextValue("Mods.LegibleBossfights.Messages.ShowCircles", getboolname(LegibleBossfights.ShowCircles)), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleTransparentKey.JustPressed)
            {
                LegibleBossfights.FadeProjectiles = !LegibleBossfights.FadeProjectiles;
                Main.NewText(Language.GetTextValue("Mods.LegibleBossfights.Messages.FadeProjectiles", getboolname(LegibleBossfights.FadeProjectiles)), 100, 240, 100);
            }
            if (LegibleBossfights.ToggleHideWallsKey.JustPressed)
            {
                LegibleBossfights.HideWalls = !LegibleBossfights.HideWalls;
                    Main.NewText(Language.GetTextValue("Mods.LegibleBossfights.Messages.HideWalls", getboolname(LegibleBossfights.HideWalls)), 100, 240, 100);
            }
        }
        public string getboolname(bool b) => b ? Language.GetTextValue("Mods.LegibleBossfights.Messages.On") : Language.GetTextValue("Mods.LegibleBossfights.Messages.Off");
    }
}
