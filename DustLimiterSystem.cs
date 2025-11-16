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

        // reset the per-frame counter
        public override void PostUpdateEverything()
        {
        }

        private static bool ShouldBlockSpawn()
        {
            return (Main.rand.NextFloat() > LegibleBossfights.ParticleRate);
        }

        // --- Hook: NewDust(int return) ---
        private static int Hook_NewDust(On_Dust.orig_NewDust orig, Vector2 pos, int width, int height, int type,
            float spx = 0f, float spy = 0f, int alpha = 0, Color newColor=default, float scale = 1f)
        {
            int idx = orig(pos, width, height, type, spx, spy, alpha, newColor, scale);
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.FadeProjectiles && ShouldBlockSpawn())
                Main.dust[idx].active = false;//idx = orig(Vector2.Zero, width, height, type, spx, spy, 0, new Color(0, 0, 0, 0), 1f);
            return idx;
        }

        // --- Hook: NewDustDirect(Dust return) ---
        private static Terraria.Dust Hook_NewDustDirect(On_Dust.orig_NewDustDirect orig, Vector2 pos, int width, int height, int type,
            float spx = 0f, float spy = 0f, int alpha = 0, Color newColor = default, float scale = 1f)
        {
            Dust dust = orig(pos, width, height, type, spx, spy, alpha, newColor, scale); ;
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.FadeProjectiles && ShouldBlockSpawn())
                dust.active = false;

            return dust;
        }

        // --- Hook: NewDustPerfect(Dust return) ---
        private static Terraria.Dust Hook_NewDustPerfect(On_Dust.orig_NewDustPerfect orig, Vector2 pos, int type,
            Vector2? spd = default, int alpha = 0, Color newColor = default, float scale = 1f)
        {
            Dust dust = orig(pos, type, spd, alpha, newColor, scale);
            if (LegibleBossfights.ParticleRate <= 1 && LegibleBossfights.FadeProjectiles && ShouldBlockSpawn())
                dust.active = false;
            return dust;
        }
    }
}
