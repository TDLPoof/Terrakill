using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.ZeGold.Rifles;

public class PlasmaBullet : ModProjectile
{
    float timeInWall = 0;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.tileCollide = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.extraUpdates = 29;

        Projectile.penetrate = 999;

        Projectile.ArmorPenetration = 999;

        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.707f)
        {
            int dustType = Main.rand.NextBool() ? DustID.GoldFlame : DustID.YellowTorch;
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, dustType, Scale: 1.5f);
            d.noGravity = true;
            d.velocity = Projectile.velocity;
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= 0.85f;
        // modifiers.FinalDamage *= 3;
    }
}

