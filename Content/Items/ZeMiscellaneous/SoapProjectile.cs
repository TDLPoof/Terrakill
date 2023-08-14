using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.ZeMiscellaneous;

public class SoapProjectile : ModProjectile
{
    Projectile attached = null;

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 10;

        Projectile.scale = 0.5f;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.penetrate = 1;

        Projectile.damage = 1;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public override void AI()
    {

        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, Main.rand.Next(176, 180));
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }

        Projectile.rotation += MathHelper.ToRadians(3);
        if (attached == null)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.type == ModContent.ProjectileType<Content.Punching.WhiplashProjectile>() && p.Hitbox.Intersects(Projectile.Hitbox) && p.active)
                {
                    attached = p;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            Projectile.velocity.Y += 0.1f;
            Projectile.velocity *= 0.98f;
        }
        else
        {
            Projectile.Center = attached.Center;
            Projectile.rotation = attached.rotation;
            Projectile.velocity = Vector2.Zero;
        }

        if (!attached.active) attached = null;

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= 42069;
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.FinalDamage *= 42069;
    }
}

