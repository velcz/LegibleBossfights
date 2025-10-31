using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegibleBossfights
{
    public class CheckBossAliveSystem : ModSystem
    {
        private bool _wasBossAlive;

        private int checktime = 15;

        public override void PostUpdateEverything()
        {
            if (checktime-- > 0)
                return;

            bool isBossAlive = IsAnyBossAlive();

            if (!LegibleBossfights.AutoFriendlyProjectileHide)
                LegibleBossfights.FadeProjectiles = true;
            else if (LegibleBossfights.FadeProjectiles && !isBossAlive)
                LegibleBossfights.FadeProjectiles = false;
            checktime = 15;
            

            if (isBossAlive && !_wasBossAlive)// Boss JUST spawned/engaged
            {
                if (LegibleBossfights.AutoCircles) LegibleBossfights.ShowCircles = true;
                if (LegibleBossfights.AutoLine) LegibleBossfights.ShowLine = true;
                if (LegibleBossfights.AutoFriendlyProjectileHide) LegibleBossfights.FadeProjectiles = true;
            }
            else if (!isBossAlive && _wasBossAlive)// Boss JUST despawned/died
            {
                if (LegibleBossfights.AutoCircles) LegibleBossfights.ShowCircles = false;
                if (LegibleBossfights.AutoLine) LegibleBossfights.ShowLine = false;
                if (LegibleBossfights.AutoFriendlyProjectileHide) LegibleBossfights.FadeProjectiles = false;
            }

            _wasBossAlive = isBossAlive;
        }

        public static bool IsAnyBossAlive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (!n.active) continue;
                if (n.boss) return true;
            }
            return false;
        }
    }
}
