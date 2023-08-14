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
using Terrakill.Content.Items.ZeGold.Revolvers;

namespace Terrakill.Content.Items.Red.Revolvers;

public class SharpshooterSuper : ModProjectile
{
    List<Projectile> bouncedCoins = new List<Projectile>();
    List<NPC> hit = new List<NPC>();
    List<NPC> intersecting = new List<NPC>();

    int bounces = 7;

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

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 0;
    }

    int spentFrames = 0;
    public override void AI()
    {
        for (int i = 0; i < intersecting.Count; i++)
        {
            NPC npc = intersecting[i];
            if (!Projectile.Hitbox.Intersects(npc.Hitbox))
            {
                intersecting.RemoveAt(i);
                i--;
            }
        }

        if (Main.rand.NextBool(3) && !Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.75f)
        {
            Dust d;
            if (Main.rand.NextBool()) d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.RedTorch, Scale: 2f);
            else d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.WhiteTorch, Scale: 2f);
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.type != ModContent.ProjectileType<CoreBomb>()) continue;
            if (p.Distance(Projectile.position) > 40) continue;
            Explode(200, 200, DustID.Torch, DustID.RedTorch);
            p.Kill();
            ModContent.GetInstance<Hitstop>().hitstopping = 100;
            Projectile.penetrate--;
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p == Projectile) continue;
            if (p.type == ModContent.ProjectileType<SharpshooterSpin>()) continue;
            if (!p.active) continue;
            if (p.Center.Distance(Projectile.Center) < 10)
            {
                Explode(50, 100, DustID.WhiteTorch, DustID.RedTorch);
                p.Kill();
                Projectile.penetrate--;
            }
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

        if (spentFrames > 2) Projectile.extraUpdates = 499;
        if (Projectile.extraUpdates == 0)
        {
            spentFrames++;
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        hit.Add(target);
        intersecting.Add(target);
        //PolaritiesPort/
        if (target.GetGlobalNPC<TrapManager>().trap != null)
        {
            TrapBounce(target, ref modifiers);
        }
        ModContent.GetInstance<Hitstop>().hitstopping = 80;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return !intersecting.Contains(target);
    }

    public void TrapBounce(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.GetGlobalNPC<TrapManager>().trap != null)
        {
            modifiers.FinalDamage *= 1.2f;
            Projectile.penetrate++;
            bounces++;
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

    public void CoinBounce(Projectile originalCoin)
    {
        Projectile.penetrate++;
        foreach (Projectile p in Main.projectile)
        {
            if (!p.active) continue;
            if (bouncedCoins.Contains(p)) continue;
            if (p == originalCoin) continue;
            if (p.type == ModContent.ProjectileType<EndMeCoin>()
             && p.position.Distance(originalCoin.position) < 600)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(p.Center) * Projectile.velocity.Length();
                Projectile.damage = (int)MathF.Round(Projectile.damage * 1.1f);
                Projectile.penetrate++;
                bouncedCoins.Add(p);
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
            if (!npc.friendly)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(damage * distFactor), 0, Projectile.owner);
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (bounces-- < 0) Projectile.Kill();
        ModContent.GetInstance<Hitstop>().hitstopping = 30;
        intersecting = new List<NPC>();
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

        Projectile.extraUpdates = 0;
        spentFrames = 0;

        return false;
    }
}

