using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace LegibleBossfights
{
    public class LegibleBossfightsGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool lowRender;
        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.friendly && !Main.projPet[projectile.type])
                lowRender = true;
        }
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            float transparency = LegibleBossfightsConfig.Instance.TransparentFriendlyProjectiles;
            if (lowRender && !projectile.hostile && transparency < 1)
            {
                Color? color = projectile.ModProjectile?.GetAlpha(lightColor);
                if (color != null)
                {
                    return color.Value * LegibleBossfightsConfig.Instance.TransparentFriendlyProjectiles;
                }
                lightColor *= projectile.Opacity * transparency;
                return lightColor;

            }

            return base.GetAlpha(projectile, lightColor);
        }
    }
}
