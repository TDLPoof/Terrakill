using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Red.Nailguns;

public class NerveMarker : ModProjectile
{
    SoundStyle Impaled = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/Parry")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 5
    };

    public Vector2 offset;
    public Projectile attached = null;

    float deltaR;

    public bool grounded = false;
    bool slabbed = false;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 450;

        deltaR = Main.rand.NextFloat(5f, 7.5f);
        if (Main.rand.NextBool()) deltaR *= -1;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0 && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<AltRed.Sawlaunchers.NervenetSawlauncher>())
        {
            slabbed = true;
        }

        Projectile.rotation += MathHelper.ToRadians(deltaR);

        if (slabbed)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.friendly) continue;
                if (!npc.active) continue;
                if (npc.life <= 0) continue;
                if (npc.dontTakeDamage) continue;
                if (npc.type == NPCID.TargetDummy) continue;
                if (npc.Distance(Projectile.position) > 500) continue;
                Vector2 toTarget = Projectile.DirectionTo(npc.Center + new Vector2(npc.width + 20, 0).RotatedBy(Projectile.rotation));
                toTarget *= 30;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget, 0.04f);
                break;
            }
        }
        else
        {
            Projectile.velocity *= 0.98f;
            deltaR *= 0.97f;
        }

        if (attached == null)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.type == ModContent.ProjectileType<Blue.Nailguns.Magnet>() && p.active && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    SoundEngine.PlaySound(Impaled, Projectile.position);
                    Projectile.timeLeft = 600;
                    attached = p;
                    break;
                }
            }
        }
        if (attached != null)
        {
            Projectile.position = attached.position;
            Projectile.tileCollide = false;
        }

        Projectile.ai[0]++;
    }

    public override void Kill(int timeLeft)
    {
        for (int i = 0; i < 4; i++)
        {
            Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), 1, 1, DustID.RedTorch).noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

        // If the projectile hits the left or right side of the tile, reverse the X velocity
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }

        return false;
    }
}

