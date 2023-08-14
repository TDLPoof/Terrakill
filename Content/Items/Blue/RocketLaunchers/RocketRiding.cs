using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Blue.RocketLaunchers;

public class RocketRiding : ModPlayer
{
    public Projectile rocketRide;
    public override void PreUpdateMovement()
    {
        if (rocketRide == null)
        {
            if (!Player.controlJump)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<Rocket>()
                     && p.active
                     && p.velocity.Length() < float.Epsilon
                     && p.Distance(Player.position) < 30
                     && p.position.Y > Player.position.Y)
                    {
                        rocketRide = p;
                        break;
                    }
                }
            }
        }
        else
        {
            Player.position = rocketRide.position - new Vector2(0, 60);
            if (Player.controlJump)
            {
                rocketRide = null;
                Player.velocity.Y = -4;
            }

            if (Player.controlLeft)
            {
                if (rocketRide.velocity.Length() < float.Epsilon)
                {
                    rocketRide.rotation -= MathHelper.ToRadians(0.75f);
                }
                else
                {
                    rocketRide.velocity = rocketRide.velocity.RotatedBy(MathHelper.ToRadians(-0.75f));
                }
            }
            if (Player.controlRight)
            {
                if (rocketRide.velocity.Length() < float.Epsilon)
                {
                    rocketRide.rotation += MathHelper.ToRadians(0.75f);
                }
                else
                {
                    rocketRide.velocity = rocketRide.velocity.RotatedBy(MathHelper.ToRadians(0.75f));
                }
            }
        }
    }
}

