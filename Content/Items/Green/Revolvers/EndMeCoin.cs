using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.Green.Revolvers;

public class EndMeCoin : ModProjectile
{
    bool slabbed = false, coinGodded = false;
    
    public override void SetDefaults()
    {
        Projectile.width = 6;
        Projectile.height = 6;

        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.penetrate = 999;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0 && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<AltGreen.Revolvers.SlabMarksman>())
        {
            slabbed = true;
            Projectile.scale = 1.1f;
        }
        if (Projectile.ai[0] == 0 && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<ZeMiscellaneous.CoinGod>())
        {
            coinGodded = true;
            Projectile.scale = 1.2f;
        }

        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.YellowTorch);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 2f;
        }

        if (!Projectile.friendly)
        {
            if (slabbed)
            {
                Projectile.velocity.Y += 0.04f;
                Projectile.velocity /= 1.015f;
            }
            else if (coinGodded)
            {
                Projectile.velocity /= 1.03f;
            }
            else
            {
                Projectile.velocity.Y += 0.06f;
                Projectile.velocity /= 1.01f;
            }
        }

        Projectile.ai[0]++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.extraUpdates = 0;
        Projectile.velocity = new Vector2(0, -2);
        Projectile.friendly = false;
    }
}

