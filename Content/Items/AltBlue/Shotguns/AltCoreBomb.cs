using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.AltBlue.Shotguns;

public class AltCoreBomb : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 6;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.OrangeTorch, Scale: 1.5f);
            d.noGravity = true;
            d.velocity = Projectile.velocity;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity.Y += 0.3f;
        Projectile.velocity /= 1.01f;

        Projectile.ai[0]++;

        foreach (Projectile p in Main.projectile)
        {
            if (p.extraUpdates > 100 && p.active && p.Hitbox.Intersects(Projectile.Hitbox))
            {
                Explode(150, 40);
                Projectile.Kill();
            }
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        Explode(150, 30);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Explode(100, 30);
        return true;
    }

    public void Explode(int size, int damage, int dustID = DustID.Torch, int altDustID = -1)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        if (!Main.dedServ)
        {
            for (int i = 0; i < size; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(size, size), 1, 1, dustID, Scale: 2f);
                d.noGravity = true;
            }
            if (altDustID != -1)
            {
                for (int i = 0; i < size; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(size, size), 1, 1, altDustID, Scale: 2f);
                    d.noGravity = true;
                }
            }
        }

        foreach (NPC npc in Main.npc)
        {
            if (npc.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (npc.Distance(Projectile.Center) / size);
            if (npc.friendly)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<Blue.Shotguns.ForYouToo>(), 35, 0, Projectile.owner);
            }
            else
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<Blue.Shotguns.HaveSomeDamage>(), (int)MathF.Round(damage * distFactor), 0, Projectile.owner);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(Projectile.Center) > size) continue;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<Blue.Shotguns.ForYouToo>(), 35, 0, Projectile.owner);
        }
    }
}

