using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;

namespace Terrakill.Content.Punching;

public class GetFucked : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.extraUpdates = 499;
        Projectile.timeLeft = 0;

        Projectile.penetrate = 1;
    }

    public void MakeADust(int type, float scale = 1f, float velocityMult = 1f, bool velocityChange = false)
    {
        Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, type, Scale: scale);
        d.noGravity = true;
        d.velocity = Projectile.velocity * velocityMult;
        if (velocityChange) d.velocity += Main.rand.NextVector2CircularEdge(3, 3);
    }

    public override void AI()
    {
        Explode(150, 300, DustID.Torch);
        Projectile.Kill();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // modifiers.FinalDamage += 3;
        Explode(150, 100, DustID.Torch);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Explode(150, 100, DustID.Torch);
        return true;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        int size = 20;
        hitbox.X -= size / 2;
        hitbox.Y -= size / 2;
        hitbox.Width += size;
        hitbox.Height += size;
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
                    ModContent.ProjectileType<ForYouToo>(), damage, 0, Projectile.owner);
            }
            else
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(damage * distFactor), 0, Projectile.owner);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(Projectile.Center) > size) continue;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<ForYouToo>(), 35, 0, Projectile.owner);
        }
    }
}

