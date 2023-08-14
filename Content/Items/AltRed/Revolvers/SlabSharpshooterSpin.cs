using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.AltRed.Revolvers;

public class SlabSharpshooterSpin : ModProjectile
{
    public float bounces = 5;

    Vector2 localPosition;
    public override void SetDefaults()
    {
        Projectile.width = 17;
        Projectile.height = 27;

        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 3;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0) localPosition = Main.player[Projectile.owner].Center.DirectionTo(Projectile.Center) * Main.player[Projectile.owner].Center.Distance(Projectile.Center);
        Projectile.rotation = Projectile.knockBack;
        Projectile.Center = Main.player[Projectile.owner].Center + localPosition;
        Projectile.ai[0]++;
    }
}

