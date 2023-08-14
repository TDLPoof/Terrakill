using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Green.RocketLaunchers;

public class SRSBall : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.ArmorPenetration = 20;

        Projectile.extraUpdates = 3;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    float fallingAmt;
    int fallingTicks;
    public override void AI()
    {
        if (ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "cannonballtech"
         || ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "cbt")
        {
            Vector2 toTarget = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center + Main.rand.NextVector2CircularEdge(8, 8));
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * 14, 0.01f);
        }
        else
        {
            Projectile.velocity.Y += Projectile.ai[1] / (Projectile.extraUpdates + 1);
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        Shockwave(100, DustID.SteampunkSteam, DustID.Stone);
        float bounceTowardPlayer = (Main.player[Projectile.owner].position.X - Projectile.position.X);
        float corr;
        if (bounceTowardPlayer > 0) corr = MathF.Sqrt(MathF.Sqrt(bounceTowardPlayer));
        else corr = -1 * MathF.Sqrt(MathF.Sqrt(MathF.Abs(bounceTowardPlayer)));
        Projectile.velocity = new Vector2(corr / 5, -5);
        Projectile.ai[1] = 0.1f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Shockwave(100, DustID.SteampunkSteam, DustID.Stone);
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
            if (player.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (player.Distance(Projectile.Center) / size);
            player.velocity += Projectile.Center.DirectionTo(player.Center) * 40 * distFactor;
        }
    }

}

