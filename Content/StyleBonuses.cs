using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content;

public class StyleBonusProjectiles : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (damageDone > target.life || target.life <= 0)
        {
            if (target.lifeMax > 1000)
            {
                if (projectile.DamageType == DamageClass.Melee)
                {
                    Ultrastyle.PushNewStyleBonus(new StyleBonus("+Big Fistkill", 150, StyleBonus.StyleLevel.White));
                }
                else
                {
                    Ultrastyle.PushNewStyleBonus(new StyleBonus("+Big Kill", 125, StyleBonus.StyleLevel.White));
                }
            }
            else
            {
                if (projectile.owner == Main.LocalPlayer.whoAmI)
                {
                    if (projectile.DamageType == DamageClass.Melee)
                    {
                        Ultrastyle.PushNewStyleBonus(new StyleBonus("+Fistkill", 30, StyleBonus.StyleLevel.White));
                    }
                    else
                    {
                        Ultrastyle.PushNewStyleBonus(new StyleBonus("+Kill", 45, StyleBonus.StyleLevel.White));
                    }
                }
            }
            if (projectile.owner != Main.LocalPlayer.whoAmI)
            {
                Ultrastyle.PushNewStyleBonus(new StyleBonus("+Friendly Fire", 250, StyleBonus.StyleLevel.White));
            }
        }
    }
}

