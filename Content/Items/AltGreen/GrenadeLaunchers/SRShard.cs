using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.AltGreen.GrenadeLaunchers;

public class SRShard : ModProjectile
{
    int hits = 0;
    public override void SetDefaults()
    {
        Projectile.width = 6;
        Projectile.height = 6;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.ArmorPenetration = 20;

        Projectile.extraUpdates = 3;

        Projectile.penetrate = 999;


        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    float fallingAmt;
    int fallingTicks;
    public override void AI()
    {
        if (Projectile.ai[0] == 0) Projectile.rotation = Main.rand.NextFloat(MathF.PI * 2);
        Projectile.velocity /= Main.rand.NextFloat(1.01f, 1.03f);

        if (Projectile.velocity.Length() < 1) Projectile.Kill();
        
        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        modifiers.FinalDamage *= MathF.Pow(0.75f, hits);
        hits++;
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
            if (player.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (player.Distance(Projectile.Center) / size);
            player.velocity += Projectile.Center.DirectionTo(player.Center) * 40 * distFactor;
        }
    }

}

