using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.AltRed.Shotguns;

public class AltAirburstShotgunPellet : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = Main.rand.Next(45, 90);

        Projectile.ArmorPenetration = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.OrangeTorch, Scale: 1.5f);
            d.noGravity = true;
            d.velocity = Projectile.velocity;
        }

        Projectile.velocity *= 0.95f;

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
    }
}

