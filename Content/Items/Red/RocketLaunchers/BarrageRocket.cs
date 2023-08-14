using System;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Red.RocketLaunchers;

public class BarrageRocket : ModProjectile
{
    float rotDirection;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 0.5f;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.ArmorPenetration = 999;

        rotDirection = Main.rand.NextFloat(-1.5f, 1.5f);
    }

    float fallingAmt;
    int fallingTicks;
    NPC homing;
    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.OrangeTorch, Scale: 3f);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 3;
            d.velocity = d.velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5)));
        }

        if (Projectile.velocity.Length() < 20) Projectile.velocity *= 1.02f;

        if (Projectile.ai[0] == 0) Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.ai[0] < 15)
        {
            Projectile.rotation += MathHelper.ToRadians(1) * rotDirection;
            Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Projectile.rotation);
        }
        else if (homing == null)
        {
            Projectile.rotation += Projectile.velocity.ToRotation();
            List<NPC> closeNPCs = new List<NPC>();
            foreach (NPC npc in Main.npc)
            {
                if (npc.life > 0 && npc.active && !npc.friendly && !npc.dontTakeDamage && npc.type != NPCID.TargetDummy && npc.Distance(Projectile.position) < 600)
                {
                    closeNPCs.Add(npc);
                }
            }
            if (closeNPCs.Count > 0)
            {
                homing = closeNPCs[0];
                for (int i = 1; i < closeNPCs.Count; i++)
                {
                    if (closeNPCs[i].Distance(Projectile.position) < homing.Distance(Projectile.position))
                    {
                        homing = closeNPCs[i];
                    }
                }
            }
        }
        else
        {
            Vector2 toTarget = Projectile.Center.DirectionTo(homing.Center) * 24;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget, 0.08f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        if (target.velocity.Y > 1 && !target.noGravity)
        {
            modifiers.FinalDamage *= 2;
            Explode(75, 26, DustID.Torch, DustID.RedTorch);
        }
        else Explode(50, 13);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Shockwave(30, DustID.SteampunkSteam, DustID.Cloud);
        return true;
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

