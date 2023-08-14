using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Blue.Nailguns;

public class Magnet : ModProjectile
{
    public Vector2 offset;
    public NPC attached = null;

    float attachedRot = 0;
    float thisRot = 0;

    public bool grounded = false;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 3;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 1200;

        Projectile.ArmorPenetration = 999;

        Projectile.extraUpdates = 2;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        int nailsNearby = 0;
        foreach (Projectile p in Main.projectile)
        {
            if (p.type == ModContent.ProjectileType<Nail>() && p.Distance(Projectile.position) < 150) nailsNearby++;
        }

        int beepScore = 25 - nailsNearby;
        if (beepScore < 3) beepScore = 3;

        if (!Main.dedServ && Projectile.ai[0] % beepScore == 0)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.GreenTorch, Scale: 6f);
            d.noGravity = true;
        }

        if (attached == null && !grounded) Projectile.rotation = Projectile.velocity.ToRotation();
        else
        {
            if (attached != null)
            {
                Projectile.Center = (attached.Center + offset).RotatedBy(attached.rotation - attachedRot, attached.Center);
                Projectile.rotation = thisRot + attached.rotation;
            }
        }

        if (attached != null && (!attached.active || attached.life <= 0)) Projectile.Kill();

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetMaxDamage(1);
        offset = target.Center.DirectionTo(Projectile.Center) * target.Center.Distance(Projectile.Center);
        attached = target;
        attachedRot = target.rotation;
        thisRot = Projectile.rotation;
        Projectile.friendly = false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        grounded = true;
        Projectile.position += oldVelocity / 2;
        Projectile.velocity = Vector2.Zero;
        Projectile.friendly = false;
        return false;
    }
}

