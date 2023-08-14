using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.AltGreen.GrenadeLaunchers;

public class SGrenade : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 6;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.ArmorPenetration = 999;

        Projectile.timeLeft = 120;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 0;
    }

    float fallingAmt;
    int fallingTicks;

    public NPC attached = null;

    float attachedRot = 0;
    float thisRot = 0;

    Vector2 offset;

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.OrangeTorch, Scale: 1.5f);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 3;
            d.velocity = d.velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5)));
        }

        Projectile.velocity.Y += 0.15f;

        if (Projectile.velocity.Length() > float.Epsilon) Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.timeLeft < 2)
        {
            Shockwave(100, DustID.SteampunkSteam, DustID.Cloud);
            Projectile.Kill();
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        if (target.velocity.Y > 1 && !target.noGravity)
        {
            modifiers.FinalDamage *= 2;
            Explode(150, 200, DustID.Torch, DustID.RedTorch);
            Projectile.Kill();
        }
        else
        {
            Explode(100, 100);
            Projectile.Kill();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

        // If the projectile hits the left or right side of the tile, reverse the X velocity
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X * 0.3f;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
        }
        return false;
    }

    public void Shockwave(int size, int dustID = DustID.Torch, int altDustID = -1)
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
            if (npc.netID == NPCID.TargetDummy) continue;
            float distFactor = 1.00f - (npc.Distance(Projectile.Center) / size);
            npc.velocity.Y -= 20 * distFactor * npc.knockBackResist;
        }
        foreach (Item item in Main.item)
        {
            if (item.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (item.Distance(Projectile.Center) / size);
            item.velocity += Projectile.Center.DirectionTo(item.Center) * 20 * distFactor;
        }
        foreach (Projectile proj in Main.projectile)
        {
            if (proj.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (proj.Distance(Projectile.Center) / size);
            proj.velocity += Projectile.Center.DirectionTo(proj.Center) * 20 * distFactor;
        }
        foreach (Player player in Main.player)
        {
            float distFactor = 1.00f - (player.Distance(Projectile.Center) / size);
            if (distFactor < 0) distFactor = 0;
            player.velocity += Projectile.Center.DirectionTo(player.Center) * 30 * distFactor;
        }
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
                    ModContent.ProjectileType<ForYouToo>(), 35, 0, Projectile.owner);
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

