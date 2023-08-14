﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using log4net.Core;
using Microsoft.CodeAnalysis;

namespace Terrakill.Content.Items.AltGreen.Sawlaunchers;

public class FireSaw : ModProjectile
{
    float rotation, deltaR;
    float ogVel;

    bool nailbombed, hasBeenNailbombed;

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 200;
        Projectile.penetrate = 40;

        Projectile.ArmorPenetration = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;

        deltaR = Main.rand.NextFloat(8f, 16f);
        if (Main.rand.NextBool()) deltaR *= -1;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0) ogVel = Projectile.velocity.Length();

        rotation += MathHelper.ToRadians(deltaR);

        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Torch, Scale: 2f);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 2;
        }

        Projectile.velocity.Y += 0.1f;

        foreach (Projectile p in Main.projectile)
        {
            if (p.type == ModContent.ProjectileType<Blue.Nailguns.Magnet>() && p.Distance(Projectile.position) < 600)
            {
                Vector2 target = p.Center + new Vector2(120, 0).RotatedBy(rotation);
                if (p.active) Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target) * 30, 0.24f);
                else
                {
                    if (!hasBeenNailbombed) nailbombed = true;
                    hasBeenNailbombed = true;
                }
            }
        }

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
        if (hasBeenNailbombed) modifiers.FinalDamage *= 2;

        target.AddBuff(BuffID.OnFire3, 360);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.penetrate-- < 0) Projectile.Kill();
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

