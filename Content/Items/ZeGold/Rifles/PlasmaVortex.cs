using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.ZeGold.Rifles;

public class PlasmaVortex : ModProjectile
{
    float timeInWall = 0;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.tileCollide = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 400;

        Projectile.ArmorPenetration = 999;

        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            int dustType = Main.rand.NextBool() ? DustID.GoldFlame : DustID.YellowTorch;
            Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 1, 1, dustType, Scale: 3f);
            d.noGravity = true;
            d.velocity = Projectile.velocity;

            for (int i = 0; i < 8; i++)
            {
                dustType = Main.rand.NextBool() ? DustID.GoldFlame : DustID.YellowTorch;
                Dust ring = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2CircularEdge(24 , 24), 1, 1, dustType, Scale: 2f);
                ring.noGravity = true;
                ring.velocity = ring.position.DirectionTo(Projectile.position).RotatedBy(MathF.PI / 2) * 4;
            }
        }

        Projectile.velocity *= 0.97f;
        bool attractingPlayer = false;

        foreach (NPC npc in Main.npc)
        {
            if (!npc.boss && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && npc.Distance(Projectile.position) < 400)
            {
                Vector2 toTarget = npc.Center.DirectionTo(Projectile.Center) * 18 * MathHelper.Clamp(npc.knockBackResist, 0.25f, 1f);
                npc.velocity = Vector2.Lerp(npc.velocity, toTarget, 0.06f);
                if (Projectile.timeLeft == 1) npc.velocity *= 3;
            }
        }

        foreach (Projectile projectile in Main.projectile)
        {
            if (projectile.Distance(Projectile.position) < 400 &&
                (projectile.type == ModContent.ProjectileType<Green.Revolvers.EndMeCoin>()
              || projectile.type == ModContent.ProjectileType<Blue.Nailguns.Magnet>())
              || projectile.type == ModContent.ProjectileType<Blue.RocketLaunchers.Rocket>()
              || projectile.type == ModContent.ProjectileType<Green.RocketLaunchers.SRocket>()
              || projectile.type == ModContent.ProjectileType<Green.RocketLaunchers.SRSBall>()
              || projectile.type == ModContent.ProjectileType<Red.RocketLaunchers.BRocket>()
              || projectile.type == ModContent.ProjectileType<Red.RocketLaunchers.BarrageRocket>()
              || projectile.type == ModContent.ProjectileType<Blue.Shotguns.CoreBomb>())
            {
                Vector2 toTarget = projectile.Center.DirectionTo(Projectile.Center) * 18;
                projectile.velocity = Vector2.Lerp(projectile.velocity, toTarget, 0.06f);
                if (Projectile.timeLeft == 1) projectile.velocity *= 3;
            }

            if (projectile.type == Projectile.type && projectile.Distance(Projectile.position) < 50) attractingPlayer = true;
        }

        if (attractingPlayer)
        {
            foreach (Player player in Main.player)
            {
                if (player.dead) continue;
                Vector2 toTarget = player.Center.DirectionTo(Projectile.Center) * 18;
                player.velocity = Vector2.Lerp(player.velocity, toTarget, 0.06f);
                if (Projectile.timeLeft == 1) player.velocity *= 3;
            }
        }

        Projectile.ai[0]++;
    }
}

