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

        private static bool setback = false;

        public override void PostUpdatePlayers()
        {
            if (checktime-- > 0)
                return;

            bool isBossAlive = IsAnyBossAlive();


            if (!LegibleBossfights.AutoParticle)
                LegibleBossfights.ReduceParticles = true;
            else if (LegibleBossfights.ReduceParticles && !isBossAlive)
                LegibleBossfights.ReduceParticles = false;
            checktime = 15;
            

            if (isBossAlive && !_wasBossAlive)// Boss JUST spawned
            {
                if (LegibleBossfights.AutoCircles) LegibleBossfights.ShowCircles = true;
                if (LegibleBossfights.AutoLine) LegibleBossfights.ShowLine = true;
                if (LegibleBossfights.AutoFriendlyProjectileHide) LegibleBossfights.FadeProjectiles = true;
                if (LegibleBossfights.AutoParticle) LegibleBossfights.ReduceParticles = true;
                if (LegibleBossfights.AutoHideWalls) LegibleBossfights.HideWalls = true;


            }
            else if (!isBossAlive && _wasBossAlive)// Boss JUST despawned
            {
                if (LegibleBossfights.AutoCircles) LegibleBossfights.ShowCircles = false;
                if (LegibleBossfights.AutoLine) LegibleBossfights.ShowLine = false;
                if (LegibleBossfights.AutoFriendlyProjectileHide) LegibleBossfights.FadeProjectiles = false;
                if (LegibleBossfights.AutoParticle) LegibleBossfights.ReduceParticles = false;
                if (LegibleBossfights.AutoHideWalls) LegibleBossfights.HideWalls = false;
                if (setback)
                {
                    LegibleBossfights.ProjectileHighlightMode = LegibleBossfightsConfig.Instance.ProjectileHighlightMode;
                    setback = false;
                }
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
