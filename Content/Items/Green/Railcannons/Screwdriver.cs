using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Green.Railcannons;

public class Screwdriver : ModProjectile
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

        Projectile.timeLeft = 450;

        Projectile.ArmorPenetration = 999;

        Projectile.extraUpdates = 22;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 3;
    }

    int timer = 0;
    public override void AI()
    {
        if (attached == null && !grounded) Projectile.rotation = Projectile.velocity.ToRotation();
        else
        {
            if (attached != null)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = (attached.Center + offset).RotatedBy(attached.rotation - attachedRot, attached.Center) + Main.rand.NextVector2Circular(4, 4);
                Projectile.rotation = thisRot + attached.rotation;
                if (timer++ % 30 == 0) for (int i = 0; i < 3; i++) { SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item22 : SoundID.Item23, Projectile.Center); }
            }
        }

        if (attached != null && (!attached.active || attached.life <= 0)) Projectile.Kill();

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (attached == null)
        {
            offset = target.Center.DirectionTo(Projectile.Center) * target.Center.Distance(Projectile.Center);
            attached = target;
            attachedRot = target.rotation;
            thisRot = Projectile.rotation;
            Projectile.velocity = Vector2.Zero;
        }
        else modifiers.FinalDamage /= 14f;
        Projectile.extraUpdates = 0;
    }

    public override void Kill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(3, 3), 1, 1, DustID.Torch, Scale: 2f).noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        return true;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        if (attached != null)
        {
            int size = 20;
            hitbox.X -= size / 2;
            hitbox.Y -= size / 2;
            hitbox.Width += size;
            hitbox.Height += size;
        }
    }
}

