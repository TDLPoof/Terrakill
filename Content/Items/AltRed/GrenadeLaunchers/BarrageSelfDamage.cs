using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.AltRed.GrenadeLaunchers;

public class BarrageSelfDamage : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = false;
        Projectile.hostile = true;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.penetrate = 1;
        Projectile.timeLeft = 5;

        Projectile.ArmorPenetration = 9999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
}

