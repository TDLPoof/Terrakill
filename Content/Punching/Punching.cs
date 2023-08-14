using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace Terrakill.Content.Punching;

public class Punching : ModPlayer
{
    int fistState = 0;
    int timeSinceLastPunch = 0;

    public bool knuckleblasterUnlocked = false;
    public bool revsaberUnlocked = true;
    public bool whiplashUnlocked = false;

    SoundStyle HookStart = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookStart")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };

    public override void PreUpdate()
    {
        if (Keybinds.ChangeHand.JustPressed)
        {
            fistState++;
            if (!knuckleblasterUnlocked && fistState == 1)
            {
                if (revsaberUnlocked) fistState = 2;
                else fistState = 0;
            }
            if (fistState > 1) fistState = 0;
            if (fistState == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(Player.TopLeft, Player.width, Player.height, DustID.BlueTorch, Scale: 2f).noGravity = true;
                }
            }
            else if (fistState == 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(Player.TopLeft, Player.width, Player.height, DustID.RedTorch, Scale: 2f).noGravity = true;
                }
            }
            else if (fistState == 2)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(Player.TopLeft, Player.width, Player.height, DustID.YellowTorch, Scale: 2f).noGravity = true;
                }
            }
        }

        if (Keybinds.Punch.JustPressed && timeSinceLastPunch > 10 && fistState == 0)
        {
            timeSinceLastPunch = 0;
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(),
                                           Player.Center,
                                           Player.Center.DirectionTo(Main.MouseWorld) * 7,
                                           ModContent.ProjectileType<Feedbacker>(),
                                           30, 2f, Player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item1, Player.Center);

            if (Main.MouseWorld.X > Player.position.X) Player.direction = 1;
            if (Main.MouseWorld.X < Player.position.X) Player.direction = -1;
        }
        if (Keybinds.Punch.JustPressed && timeSinceLastPunch > 25 && fistState == 1)
        {
            timeSinceLastPunch = 0;
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(),
                                           Player.Center,
                                           Player.Center.DirectionTo(Main.MouseWorld) * 7,
                                           ModContent.ProjectileType<Knuckleblaster>(),
                                           120, 12f, Player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item1, Player.Center);

            if (Main.MouseWorld.X > Player.position.X) Player.direction = 1;
            if (Main.MouseWorld.X < Player.position.X) Player.direction = -1;
        }
        timeSinceLastPunch++;

        if (Keybinds.Whiplash.JustPressed && whiplashUnlocked && Player.ownedProjectileCounts[ModContent.ProjectileType<WhiplashProjectile>()] < 1)
        {
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(),
                                           Player.Center,
                                           Player.Center.DirectionTo(Main.MouseWorld) * 7,
                                           ModContent.ProjectileType<Whiplash>(),
                                           120, 12f, Player.whoAmI);
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(),
                               Player.Center,
                               Player.Center.DirectionTo(Main.MouseWorld) * 20,
                               ModContent.ProjectileType<WhiplashProjectile>(),
                               20, 0f, Player.whoAmI);
            SoundEngine.PlaySound(HookStart, Player.Center);

            if (Main.MouseWorld.X > Player.position.X) Player.direction = 1;
            if (Main.MouseWorld.X < Player.position.X) Player.direction = -1;
        }
    }

    public override void Load()
    {
        knuckleblasterUnlocked = false;
        revsaberUnlocked = true;
        whiplashUnlocked = false;
    }

    public override void Unload()
    {
        knuckleblasterUnlocked = false;
        revsaberUnlocked = true;
        whiplashUnlocked = false;
    }

    public override void SaveData(TagCompound tag)
    {
        if (knuckleblasterUnlocked) tag["KnuckleblasterUnlocked"] = knuckleblasterUnlocked;
        if (whiplashUnlocked) tag["WhiplashUnlocked"] = whiplashUnlocked;
    }

    public override void LoadData(TagCompound tag)
    {
        knuckleblasterUnlocked = tag.ContainsKey("KnuckleblasterUnlocked");
        whiplashUnlocked = tag.ContainsKey("WhiplashUnlocked");
    }
}

