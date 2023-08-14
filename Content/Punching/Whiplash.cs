using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terrakill.Content.Items.Blue.Shotguns;

namespace Terrakill.Content.Punching;

public class Whiplash : ModProjectile
{
    SoundStyle HookLoop = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookLoop")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };

    Vector2 localVelocity, localPosition;
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 5;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.timeLeft = 20;
        Projectile.DamageType = DamageClass.Melee;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;

        Projectile.alpha = 255;
    }

    bool knuckleblasted = false;
    int timer = 0;
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            localVelocity = Projectile.velocity;
            localPosition = Main.player[Projectile.owner].position.DirectionTo(Projectile.position) *
                Main.player[Projectile.owner].position.Distance(Projectile.position);
            Projectile.velocity = Vector2.Zero;
        }

        if (Projectile.ai[0] < 6) Projectile.alpha -= 50;

        localVelocity *= 0.75f;

        localPosition += localVelocity;

        Projectile.rotation = localVelocity.ToRotation() + (Projectile.direction == 1 ? 0 : MathF.PI);

        Projectile.position = Main.player[Projectile.owner].position + localPosition;

        if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<WhiplashProjectile>()] > 0)
        {
            Projectile.timeLeft++;
        }
        Projectile.ai[0]++;
    }
}

