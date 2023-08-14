using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Microsoft.Xna.Framework;
using Terrakill.Content.Items.Blue.Shotguns;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Terrakill.Content.Punching;

public class Feedbacker : ModProjectile
{
    SoundStyle Parry = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/Parry")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 2
    };

    Vector2 localVelocity, localPosition;
    int recentPBoosts = 0;
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 5;

        Projectile.friendly = true;
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

    bool parriedAProjectile = false;

    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            localVelocity = Projectile.velocity;
            localPosition = Main.player[Projectile.owner].position.DirectionTo(Projectile.position) *
                Main.player[Projectile.owner].position.Distance(Projectile.position);
            Projectile.velocity = Vector2.Zero;
        }

        Projectile.alpha -= 50;

        localVelocity *= 0.75f;

        localPosition += localVelocity;

        Projectile.rotation = localVelocity.ToRotation() + (Projectile.direction == 1 ? 0 : MathF.PI);

        Projectile.position = Main.player[Projectile.owner].position + localPosition;

        Projectile.ai[0]++;

        Vector2 shotgunParryPos = Vector2.Zero;
        if (!parriedAProjectile && Projectile.ai[0] < 10)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.hostile && p.Distance(Projectile.position) < 30)
                {
                    Ultrastyle.PushNewStyleBonus(new StyleBonus("+Parry", 100, StyleBonus.StyleLevel.Green));
                    SoundEngine.PlaySound(Parry, Projectile.position);
                    ModContent.GetInstance<Hitstop>().hitstopping = 100;
                    PunchCameraModifier shake = new PunchCameraModifier(p.position, Main.rand.NextVector2CircularEdge(1, 1), p.damage * 0.6f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    p.hostile = false;
                    p.friendly = true;
                    p.velocity = (localVelocity / localVelocity.Length()) * p.velocity.Length() * 2;
                    p.damage *= 5;
                    p.ArmorPenetration += 9999;
                    p.extraUpdates++;
                }

                
                if (p.active && p.ai[0] > 1 && p.ai[0] < 5
                 && (p.type == ModContent.ProjectileType<Items.Blue.Shotguns.ShotgunPellet>()
                 || p.type == ModContent.ProjectileType<Items.Green.Shotguns.PCShotgunPellet>()
                 || p.type == ModContent.ProjectileType<Items.Red.Shotguns.AirburstShotgunPellet>()))
                {
                    SoundEngine.PlaySound(Parry, Projectile.position);
                    PunchCameraModifier shake = new PunchCameraModifier(p.position, Main.rand.NextVector2CircularEdge(1, 1), p.damage * 0.6f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    shotgunParryPos = p.position;
                    p.velocity = (localVelocity / localVelocity.Length()) * p.velocity.Length();
                    p.velocity = p.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5f, 5f)));
                    p.extraUpdates++;
                    p.penetrate += 2;
                    if (Main.rand.NextBool()) p.Kill();
                }

                if (p.Distance(Projectile.position) < 10 && p.active && p.type == ModContent.ProjectileType<Items.Green.Revolvers.EndMeCoin>())
                {
                    SoundEngine.PlaySound(Parry, Projectile.position);
                    ModContent.GetInstance<Hitstop>().hitstopping = 100;
                    PunchCameraModifier shake = new PunchCameraModifier(p.position, Main.rand.NextVector2CircularEdge(1, 1), p.damage * 0.6f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    p.velocity = (localVelocity / localVelocity.Length());
                    p.extraUpdates = 199;
                    p.friendly = true;
                }

                if (p.Distance(Projectile.position) < 20 && p.active && p.type == ModContent.ProjectileType<Items.Red.Nailguns.NerveMarker>())
                {
                    SoundEngine.PlaySound(Parry, Projectile.position);
                    ModContent.GetInstance<Hitstop>().hitstopping = 100;
                    PunchCameraModifier shake = new PunchCameraModifier(p.position, Main.rand.NextVector2CircularEdge(1, 1), p.damage * 0.6f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    p.velocity = (localVelocity / localVelocity.Length()) * 20f / 3f;
                    p.timeLeft = 450;
                }
                if (p.Distance(Projectile.position) < 30 && p.active && p.type == ModContent.ProjectileType<Items.Green.RocketLaunchers.SRSBall>())
                {
                    SoundEngine.PlaySound(Parry, Projectile.position);
                    ModContent.GetInstance<Hitstop>().hitstopping = 100;
                    PunchCameraModifier shake = new PunchCameraModifier(p.position, Main.rand.NextVector2CircularEdge(1, 1), p.damage * 0.6f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    p.velocity = (localVelocity / localVelocity.Length()) * 20f * 0.7f;
                    p.ai[1] = 0.001f;
                }
            }
        }
        if (shotgunParryPos != Vector2.Zero)
        {
            if (recentPBoosts < 5)
            {
                Ultrastyle.PushNewStyleBonus(new StyleBonus("+Projectile Boost", 150, StyleBonus.StyleLevel.Green));
                ModContent.GetInstance<Hitstop>().hitstopping = 100;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                    shotgunParryPos, localVelocity / localVelocity.Length(),
                    ModContent.ProjectileType<ParryBomb>(), 10, 0, Projectile.owner);
                recentPBoosts += 2;
            }
            else
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                    shotgunParryPos, localVelocity / localVelocity.Length(),
                    ModContent.ProjectileType<GetFucked>(), 75, 0, Projectile.owner);
            }
        }

        if (Projectile.ai[0] % 120 == 0)
        {
            recentPBoosts--;
        }
        if (recentPBoosts < 0) recentPBoosts = 0;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // modifiers.FinalDamage += 3;
        ModContent.GetInstance<Hitstop>().hitstopping = 30;
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
            npc.velocity += Projectile.DirectionTo(npc.Center) * 20 * distFactor;
        }
    }
}

