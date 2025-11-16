using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace LegibleBossfights
{
    public class DustLimiterSystem : ModSystem
    {
        public override void Load()
        {
            On_Dust.NewDust += Hook_NewDust;
            On_Dust.NewDustDirect += Hook_NewDustDirect;
            On_Dust.NewDustPerfect += Hook_NewDustPerfect;
        }

        public override void Unload()
        {
            On_Dust.NewDust -= Hook_NewDust;
            On_Dust.NewDustDirect -= Hook_NewDustDirect;
            On_Dust.NewDustPerfect -= Hook_NewDustPerfect;
        }

        public override void PostUpdateEverything()
        {
        }

        private static bool ShouldBlockSpawn()
        {
            return (Main.rand.NextFloat() > LegibleBossfights.ParticleRate);
        }

        private static int Hook_NewDust(On_Dust.orig_NewDust orig, Vector2 pos, int width, int height, int type,
            float spx = 0f, float spy = 0f, int alpha = 0, Color newColor=default, float scale = 1f)
        {
            int idx = orig(pos, width, height, type, spx, spy, alpha, newColor, scale);
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.ReduceParticles && ShouldBlockSpawn())
                Main.dust[idx].active = false;
            return idx;
        }
        private static Dust Hook_NewDustDirect(On_Dust.orig_NewDustDirect orig, Vector2 pos, int width, int height, int type,
            float spx = 0f, float spy = 0f, int alpha = 0, Color newColor = default, float scale = 1f)
        {
            Dust dust = orig(pos, width, height, type, spx, spy, alpha, newColor, scale); ;
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.ReduceParticles && ShouldBlockSpawn())
                dust.active = false;

            return dust;
        }
        private static Dust Hook_NewDustPerfect(On_Dust.orig_NewDustPerfect orig, Vector2 pos, int type,
            Vector2? spd = default, int alpha = 0, Color newColor = default, float scale = 1f)
        {
            Dust dust = orig(pos, type, spd, alpha, newColor, scale);
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.ReduceParticles && ShouldBlockSpawn())
                dust.active = false;
            return dust;
        }
    }
}
