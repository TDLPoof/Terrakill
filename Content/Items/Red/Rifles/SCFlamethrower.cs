using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Red.Rifles;

public class SCFlamethrower : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.alpha = 255;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 0;
        Projectile.ArmorPenetration = 9999;

        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 2;
    }

    public float ScaleFunction(float x)
    {
        return 2 * ((x * x / 700f) + 1f);
    }

    public override void AI()
    {

        if (Projectile.wet) Projectile.velocity = Vector2.Zero;

        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 1, 1, DustID.Torch, Scale: ScaleFunction(Projectile.ai[0]));
            d.noGravity = true;
        }

        Projectile.ai[0]++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 240);
        target.AddBuff(BuffID.Oiled, 240);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        modifiers.FinalDamage /= 2;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
        // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
        int size = 30;
        hitbox.X -= size;
        hitbox.Y -= size;
        hitbox.Width += size * 2;
        hitbox.Height += size * 2;
    }
}
