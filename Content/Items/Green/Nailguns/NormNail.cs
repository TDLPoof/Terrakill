using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Green.Nailguns;

public class NormNail : ModProjectile
{
    int bounces;
    bool nailbombed, hasBeenNailbombed;

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 3;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 200;
        bounces = 20;

        Projectile.ArmorPenetration = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.SilverFlame, Scale: 1.2f);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 2;
        }

        Projectile.velocity.Y += 0.25f;

        foreach (Projectile p in Main.projectile)
        {
            if (p.type == ModContent.ProjectileType<Blue.Nailguns.Magnet>() && p.Distance(Projectile.position) < 150)
            {
                if (p.active) Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(p.Center) * 60, 0.04f);
                else
                {
                    if (!hasBeenNailbombed) nailbombed = true;
                    hasBeenNailbombed = true;
                }
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (nailbombed)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.velocity *= 1.5f;
        }

        nailbombed = false;
        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        if (target.lifeMax > 200) Projectile.Kill();
        if (hasBeenNailbombed) modifiers.FinalDamage *= 2;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (bounces-- < 0) Projectile.Kill();
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

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

