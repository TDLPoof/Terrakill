using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Punching;

public class Dummy : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.timeLeft = 1;

        Projectile.alpha = 255;
    }
}

