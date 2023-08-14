using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Green.Rifles;

public class EnergyShield : ModProjectile
{
    Vector2 ogDir;
    int health;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.timeLeft = 300;

        Projectile.DamageType = DamageClass.Ranged;
        health = 10;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            ogDir = Projectile.velocity / Projectile.velocity.Length();
        }
        Projectile.velocity *= 0.96f;

        Dust d1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), 1, 1, DustID.Clentaminator_Green);
        d1.noGravity = true;
        d1.velocity = Vector2.Zero;

        for (int i = 0; i < health * 2; i++)
        {
            Vector2 positionOnShield = Projectile.Center + (ogDir.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-80f, 80f))) * 100);

            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.hostile && p.Distance(positionOnShield) < 20)
                {
                    health--;
                    p.Kill();
                }
            }

            if (!Main.dedServ && Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(positionOnShield, 1, 1, DustID.Clentaminator_Green);
                d.noGravity = true;
                d.velocity = Vector2.Zero;
            }
        }

        if (health <= 0) Projectile.Kill();

        Projectile.ai[0]++;
    }
}

