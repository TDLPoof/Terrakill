using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using System;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;
using Newtonsoft.Json.Linq;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Terrakill.Content.Punching.Swordsmasword;

public class SwordsmaswordParry : ModProjectile
{

    // Define the range of the Spear Projectile. These are overrideable properties, in case you'll want to make a class inheriting from this one.
    protected virtual float HoldoutRangeMin => 150f;
    float HoldoutRangeMax = 150f;

    float rotCoeff = 0;
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
        Projectile.width = 26;
        Projectile.height = 26;
        Projectile.scale = 2f;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
    }

    float angularDisplacement, oldRot;

    public override bool PreAI()
    {
        Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
        int duration = player.itemAnimationMax + 1; // Define the duration the projectile will exist in frames

        player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

        // Reset projectile time left if necessary
        if (Projectile.timeLeft > duration)
        {
            Projectile.timeLeft = duration;
        }

        Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

        float halfDuration = duration * 0.5f;
        float progress;

        // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
        if (Projectile.timeLeft < halfDuration)
        {
            progress = Projectile.timeLeft / halfDuration;
        }
        else
        {
            progress = (duration - Projectile.timeLeft) / halfDuration;
        }

        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(progress * rotCoeff * Projectile.spriteDirection));

        foreach (Projectile p in Main.projectile)
        {
            if (p != Projectile && p.hostile && p.active && p.Distance(Projectile.position) < 30)
            {
                float deflectAngle = Main.rand.NextBool() ? -MathF.PI / 2 : MathF.PI / 2;
                p.velocity = (Projectile.rotation + deflectAngle).ToRotationVector2() * p.velocity.Length() * 2;
                ModContent.GetInstance<Hitstop>().hitstopping = 100;
                PunchCameraModifier shake = new PunchCameraModifier(Projectile.position, Main.rand.NextVector2CircularEdge(1, 1), 30, 12f, 10, 1000f);
                Main.instance.CameraModifiers.Add(shake);
                Explode(75, DustID.YellowTorch, DustID.GoldFlame);
            }
        }

        // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
        Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress) + Main.rand.NextVector2Circular(5, 5);

        // Apply proper rotation to the sprite.
        if (Projectile.spriteDirection == -1)
        {
            // If sprite is facing left, rotate 45 degrees
            Projectile.rotation += MathHelper.ToRadians(45f);
        }
        else
        {
            // If sprite is facing right, rotate 135 degrees
            Projectile.rotation += MathHelper.ToRadians(135f);
        }


        // Avoid spawning dusts on dedicated servers
        if (!Main.dedServ)
        {
            // These dusts are added later, for the 'ExampleMod' effect
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, Alpha: 128, Scale: 0.75f).noGravity = true;
            }
        }

        angularDisplacement = Projectile.velocity.ToRotation() - oldRot;
        oldRot = Projectile.velocity.ToRotation();

        return false; // Don't execute vanilla AI.
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // modifiers.FinalDamage += 3;
        PunchCameraModifier shake = new PunchCameraModifier(Projectile.position, Main.rand.NextVector2CircularEdge(1, 1), 10 * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
        Main.instance.CameraModifiers.Add(shake);
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
        // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
        int size = -2;
        hitbox.X += size - (int)hitbox.Size().X;
        hitbox.Y -= size;
        hitbox.Width += size * 2;
        hitbox.Height += size * 2;
    }

    public void Explode(int size, int dustID = DustID.Torch, int altDustID = -1)
    {
        SoundEngine.PlaySound(
            new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/Parry")
            {
                PitchVariance = 0.1f,
                Volume = 5f,
                MaxInstances = 2
            },
            Projectile.Center);

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
            if (!npc.friendly)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(50 * distFactor), 0, Projectile.owner);
            }
        }
    }
}