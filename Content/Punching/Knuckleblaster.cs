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

public class Knuckleblaster : ModProjectile
{
    SoundStyle Parry = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/Parry")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 2
    };

    Vector2 localVelocity, localPosition;
    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 9;

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

    bool knuckleblasted = false;
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

        if (Keybinds.Punch.Current && !knuckleblasted) Projectile.timeLeft++;
        if (Projectile.ai[0] > 39 && !knuckleblasted)
        {
            knuckleblasted = true;
            Explode(100, DustID.SteampunkSteam, DustID.Torch);
            PunchCameraModifier shake = new PunchCameraModifier(Projectile.position, Main.rand.NextVector2CircularEdge(1, 1), 60 * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
            Main.instance.CameraModifiers.Add(shake);
            Projectile.timeLeft = 10;
        }

        Projectile.ai[0]++;

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // modifiers.FinalDamage += 3;
        ModContent.GetInstance<Hitstop>().hitstopping = 60;
    }

    public void Explode(int size, int dustID = DustID.Torch, int altDustID = -1)
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
            if (npc.netID == NPCID.TargetDummy) continue;
            float distFactor = 1.00f - (npc.Distance(Projectile.Center) / size);
            npc.velocity += Projectile.Center.DirectionTo(npc.Center) * 30 * distFactor;
            if (npc.friendly)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<ForYouToo>(), 35, 0, Projectile.owner);
            }
            else
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(50 * distFactor), 0, Projectile.owner);
            }
        }
        foreach (Item item in Main.item)
        {
            if (item.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (item.Distance(Projectile.Center) / size);
            item.velocity += Projectile.Center.DirectionTo(item.Center) * 30 * distFactor;
        }
        foreach (Projectile proj in Main.projectile)
        {
            if (proj == Projectile) continue;
            if (proj.Distance(Projectile.Center) > size) continue;
            float distFactor = 1.00f - (proj.Distance(Projectile.Center) / size);
            Vector2 addVel = Projectile.Center.DirectionTo(proj.Center) * 30 * distFactor;
            if (proj.type == ModContent.ProjectileType<Items.Green.Revolvers.EndMeCoin>()) addVel *= 3;
            proj.velocity += addVel;
        }
    }
}

