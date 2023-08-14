using System;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terrakill.Content.Items.Red.Shotguns;
using Terrakill.Content.Items.ZeGold.Revolvers;
using Terrakill.Content.Items.Green.Revolvers;

namespace Terrakill.Content.Items.AltGreen.Revolvers;

public class SlabMarksmanBullet : ModProjectile
{
    List<NPC> hit = new List<NPC>();
    List<Projectile> bouncedCoins = new List<Projectile>();
    bool bouncingTowardCoin = false;

    SoundStyle CoinHit = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/CoinThrow")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 5
    };

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.extraUpdates = 499;

        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void AI()
    {
        if (bouncingTowardCoin) Projectile.penetrate = 999;
        else Projectile.penetrate = 1;
        if (Main.rand.NextBool(3) && !Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.75f)
        {
            Dust d;
            if (Main.rand.NextBool(3)) d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.GreenTorch);
            else d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.WhiteTorch);
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.type != ModContent.ProjectileType<CoreBomb>()) continue;
            if (p.Distance(Projectile.position) > 40) continue;
            Explode(200, 200, DustID.Torch, DustID.RedTorch);
            p.Kill();
            Projectile.Kill();
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.type != ModContent.ProjectileType<Blue.Nailguns.Magnet>()) continue;
            if (p.Distance(Projectile.position) < 5) continue;
            SoundEngine.PlaySound(SoundID.Item14, p.Center);
            p.Kill();
            Projectile.Kill();
        }

        foreach (Projectile p in Main.projectile)
        {
            if (!p.active) continue;
            if (p.type != ModContent.ProjectileType<Green.Revolvers.EndMeCoin>()) continue;
            if (p.Distance(Projectile.position) > 20) continue;
            SoundEngine.PlaySound(CoinHit, p.Center);
            CoinBounce(p);
            p.Kill();
            break;
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
        hit.Add(target);
        if (bouncedCoins.Count > 0) modifiers.SetCrit();
        if (target.GetGlobalNPC<TrapManager>().trap != null)
        {
            TrapBounce(target, ref modifiers);
        }
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

    public void TrapBounce(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.GetGlobalNPC<TrapManager>().trap != null)
        {
            modifiers.FinalDamage *= 2;
            Projectile.penetrate++;
            foreach (NPC npc in Main.npc)
            {
                if (!hit.Contains(npc) && npc.life > 0 && npc.active && !npc.friendly && !npc.dontTakeDamage && /*npc.type != NPCID.TargetDummy &&*/ npc.Distance(Projectile.position) < 800)
                {
                    Vector2 toTarget = Projectile.Center.DirectionTo(npc.Center) * Projectile.velocity.Length();
                    Projectile.velocity = toTarget;
                    break;
                }
            }
            target.GetGlobalNPC<TrapManager>().trap.Kill();
        }
    }

    public void CoinBounce(Projectile originalCoin)
    {
        bouncedCoins.Add(originalCoin);
        ModContent.GetInstance<Hitstop>().hitstopping = 10;
        List<Projectile> coins = new List<Projectile>();
        foreach (Projectile p in Main.projectile)
        {
            if (bouncedCoins.Contains(p)) continue;
            if (!p.active) continue;
            if (p == originalCoin) continue;
            if (p.type == ModContent.ProjectileType<EndMeCoin>()
             && p.position.Distance(originalCoin.position) < 600)
            {
                coins.Add(p);
            }
        }

        if (coins.Count > 0 && !Main.rand.NextBool(5))
        {
            Projectile coin = coins[0];
            for (int i = 1; i < coins.Count; i++)
            {
                if (coins[i].Distance(Projectile.position) > coin.Distance(Projectile.position)) coin = coins[i];
            }
            Projectile.velocity = Projectile.Center.DirectionTo(coin.Center) * Projectile.velocity.Length();
            Projectile.damage = (int)MathF.Round(Projectile.damage * 1.1f);
            bouncingTowardCoin = true;
            return;
        }
        else if (coins.Count > 0)
        {
            Projectile.velocity = Projectile.Center.DirectionTo(coins[Main.rand.Next(coins.Count)].Center) * Projectile.velocity.Length();
            Projectile.damage = (int)MathF.Round(Projectile.damage * 1.1f);
            bouncingTowardCoin = true;
            return;
        }

        foreach (NPC npc in Main.npc)
        {
            if (!npc.active) continue;
            if (!npc.friendly && npc.position.Distance(originalCoin.position) < 600)
            {
                Projectile.velocity = Projectile.DirectionTo(npc.Center) * Projectile.velocity.Length();
                Projectile.damage = (int)MathF.Round(Projectile.damage * 1.2f);
                bouncingTowardCoin = false;
                return;
            }
        }
        bouncingTowardCoin = false;
        Projectile.velocity = Projectile.velocity.RotatedByRandom(2 * MathF.PI);
    }

    public void Explode(int size, int damage, int dustID = DustID.Torch, int altDustID = -1)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        if (!Main.dedServ)
        {
            for (int i = 0; i < size; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(size, size), 1, 1, dustID, Scale: 2f);
                d.noGravity = true;
            }
            if (altDustID != -1)
            {
                for (int i = 0; i < size; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(size, size), 1, 1, altDustID, Scale: 2f);
                    d.noGravity = true;
                }
            }
        }

        foreach (NPC npc in Main.npc)
        {
            if (npc.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (npc.Distance(Projectile.Center) / size);
            if (npc.friendly)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<ForYouToo>(), 35, 0, Projectile.owner);
            }
            else
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(damage * distFactor), 0, Projectile.owner);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(Projectile.Center) > size) continue;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<ForYouToo>(), 35, 0, Projectile.owner);
        }
    }
}

