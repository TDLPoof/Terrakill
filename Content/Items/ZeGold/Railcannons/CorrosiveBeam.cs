using System;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terrakill.Content.Items.Green.Revolvers;
using Terrakill.Content.Items.Red.Shotguns;

namespace Terrakill.Content.Items.ZeGold.Railcannons;

public class CorrosiveBeam : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.extraUpdates = 499;

        Projectile.penetrate = 5;
        Projectile.ArmorPenetration = 9999;

        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 5;
    }

    public List<Dust> dusts = new List<Dust>();

    public void MakeADust(int type, float scale = 1f, float velocityMult = 1f, bool velocityChange = false)
    {
        Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, type, Scale: scale);
        d.noGravity = true;
        d.velocity = Vector2.Zero;
        if (velocityChange) d.velocity += Main.rand.NextVector2CircularEdge(3, 3);
    }

    public override void AI()
    {
        if (Main.rand.NextBool(10) && !Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.75f)
        {
            MakeADust(DustID.GoldFlame, 1f, 0);
            MakeADust(DustID.YellowTorch, 0.75f, 0);
        }

        foreach (Projectile p in Main.projectile)
        {
            if (!p.active) continue;
            if (p.type != ModContent.ProjectileType<AirburstBomb>()) continue;
            if (p.Distance(Projectile.position) > 20) continue;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, p.Center);
            ShotBomb(30, 180);
            p.Kill();
            break;
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        target.AddBuff(BuffID.OnFire3, 60);
    }

    public override void Kill(int timeLeft)
    {
        foreach (Dust d in dusts)
        {
            d.scale = 0;
            d.active = false;
        } 
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        int size = 40;
        hitbox.X -= size / 2;
        hitbox.Y -= size / 2;
        hitbox.Width += size;
        hitbox.Height += size;
    }

    public void ShotBomb(int pellets, float spreadPosNeg)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
        for (int i = 0; i < pellets; i++)
        {
            Vector2 altVelocity = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-spreadPosNeg, spreadPosNeg)));
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, altVelocity,
                ModContent.ProjectileType<AirburstShotgunPellet>(),
                10, 0, Projectile.owner);
        }
    }

    int coinBounces = 0;
    public void CoinBounce(Projectile originalCoin)
    {
        foreach (Projectile p in Main.projectile)
        {
            if (!p.active) continue;
            if (p == originalCoin) continue;
            if (p.type == ModContent.ProjectileType<EndMeCoin>()
             && p.position.Distance(originalCoin.position) < 600)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(p.Center) * Projectile.velocity.Length();
                Projectile.damage = (int)MathF.Round(Projectile.damage * 1.1f);
                coinBounces++;
                return;
            }
        }

        foreach (NPC npc in Main.npc)
        {
            if (!npc.active) continue;
            if (!npc.friendly && npc.position.Distance(originalCoin.position) < 600)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(npc.Center) * Projectile.velocity.Length();
                Projectile.damage = (int)MathF.Round(Projectile.damage * 1.2f);
                return;
            }
        }
        Projectile.velocity = Projectile.velocity.RotatedByRandom(2 * MathF.PI);
    }
}

