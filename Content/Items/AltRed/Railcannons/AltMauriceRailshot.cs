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

namespace Terrakill.Content.Items.AltRed.Railcannons;

public class AltMauriceRailshot : ModProjectile
{
    SoundStyle CoreNuke = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/CoreNuke")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    SoundStyle CoinHit = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/CoinThrow")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 5
    };

    List<NPC> hit = new List<NPC>();

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.extraUpdates = 499;

        Projectile.penetrate = 999;
    }

    public void MakeADust(int type, float scale = 1f, float velocityMult = 1f, bool velocityChange = false)
    {
        Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, type, Scale: scale);
        d.noGravity = true;
        d.velocity = Vector2.Zero;
        if (velocityChange) d.velocity += Main.rand.NextVector2CircularEdge(3, 3);
    }

    int timer = 0;
    public override void AI()
    {
        if (hit.Count > 0)
        {
            if (timer++ > 3) Projectile.Kill();
        }

        if (Main.rand.NextBool() && !Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.75f)
        {
            MakeADust(DustID.RedTorch, 1f, 0);
            MakeADust(DustID.Torch, 1f, 0);
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.type != ModContent.ProjectileType<Blue.Shotguns.CoreBomb>()) continue;
            if (p.Distance(Projectile.position) > 50) continue;
            SoundEngine.PlaySound(CoreNuke, p.position);
            Explode(200, 2000 / 3, DustID.RedTorch, DustID.Torch);
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
        if (hit.Count <= 0) Explode(125, 100, DustID.YellowTorch, DustID.OrangeTorch);
        hit.Add(target);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Explode(125, 100, DustID.YellowTorch, DustID.OrangeTorch);
        return true;
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

    public void Explode(int size, int damage, int dustID = DustID.Torch, int altDustID = -1)
    {
        ModContent.GetInstance<Hitstop>().hitstopping = 100;
        SoundEngine.PlaySound(CoreNuke, Projectile.Center);
        size += 50 * coinBounces;

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

