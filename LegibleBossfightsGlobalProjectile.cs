using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Tile_Entities;
using Terraria.ModLoader;

namespace LegibleBossfights
{
    public class LegibleBossfightsGlobalProjectile : GlobalProjectile
    {
        
        public override bool InstancePerEntity => true;
        /// <summary>
        /// Should this projectile be hidden?
        /// </summary>
        public bool lowRender;

        public bool GetLowCondition(Projectile p)
        {
            bool pass = false;
            if (p.friendly) pass = true;
            if (LegibleBossfightsConfig.Instance.ExcludePets && Main.projPet[p.type]) pass = false;
            return pass;
        }
        public void SetRenderLevel(Projectile projectile)
        {
            if (GetLowCondition(projectile))
                lowRender = true;
        }
        public override void SetDefaults(Projectile projectile)
        {
            SetRenderLevel(projectile);
            base.SetDefaults(projectile);
        }

        private float formeropacity;
        private float formerlight;
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (LegibleBossfights.FadeProjectiles)
            {
                //check for only simplify other players projectiles option
                if (LegibleBossfightsConfig.Instance.OnlySimplifyOtherPlayersProjectiles && projectile.owner == Main.myPlayer)
                    return base.PreDraw(projectile, ref lightColor);
                if (lowRender)
                {
                    //cache former values for mod compatibility (calamity depends on projectile opacity to time some weapons)
                    formeropacity = projectile.Opacity;
                    formerlight = projectile.light;
                    float mult = LegibleBossfights.ProjectileTransparency;
                    projectile.Opacity = Math.Min(mult, projectile.Opacity);
                    projectile.light = Math.Min(mult, projectile.light);

                    //Override draw with simplified version
                    if (LegibleBossfightsConfig.Instance.SimplifyFriendlyProjectileDrawing)
                    {
                        Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
                        Rectangle frame = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
                        Vector2 origin = frame.Size() / 2f;


                        Color modColor = new Color(lightColor.R, lightColor.G, lightColor.B, Math.Min(LegibleBossfights.ProjectileTransparency, projectile.Opacity));
                        SpriteEffects effects = projectile.spriteDirection == 1
                            ? SpriteEffects.None
                            : SpriteEffects.FlipHorizontally;

                        Main.spriteBatch.Draw(
                            tex,
                            projectile.Center - Main.screenPosition,
                            frame,
                            modColor * projectile.Opacity,
                            projectile.rotation,
                            origin,
                            projectile.scale,
                            effects,
                            0f
                            );
                        return false;
                    }
                }
            }
            return base.PreDraw(projectile, ref lightColor);
            
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            base.PostDraw(projectile, lightColor);
            
            if (LegibleBossfights.FadeProjectiles)
            {
                //check for only simplify other players projectiles option
                if (LegibleBossfightsConfig.Instance.OnlySimplifyOtherPlayersProjectiles && projectile.owner == Main.myPlayer)
                    return;
                if (lowRender)
                {
                    //Set these back to former values for compatibility
                    projectile.Opacity = formeropacity;
                    projectile.light = formerlight;
                    if (projectile.sentry)
                    {
                        projectile.Opacity = LegibleBossfights.ProjectileTransparency;
                    }
                }
            }
        }
        
        
    }
}
