using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;

namespace Terrakill.Content;

public class DefenseDamageBoost : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public int[] p = new int[]
    {
        ModContent.ProjectileType<Items.Green.Nailguns.FireNail>(),
        ModContent.ProjectileType<Items.ZeGold.Railcannons.CorrosiveBeam>(),
        ModContent.ProjectileType<Items.ZeGold.Railcannons.CorrosiveRailshot>(),
        ModContent.ProjectileType<Items.Blue.Revolvers.PiercerSuper>(),
        ModContent.ProjectileType<Items.Red.Revolvers.SharpshooterSuper>(),
        ModContent.ProjectileType<Items.AltBlue.Revolvers.SlabPiercerSuper>(),
        ModContent.ProjectileType<Items.AltRed.Revolvers.SlabSharpshooterSuper>(),
        ModContent.ProjectileType<Items.AltBlue.Railcannons.AltElectricRailshot>(),
        ModContent.ProjectileType<Items.AltGreen.Railcannons.AltScrewdriver>(),
        ModContent.ProjectileType<Items.AltGreen.Railcannons.AltScrewDamage>(),
        ModContent.ProjectileType<Punching.Feedbacker>(),
        ModContent.ProjectileType<Punching.Knuckleblaster>(),
        ModContent.ProjectileType<Punching.WhiplashProjectile>(),
        ModContent.ProjectileType<Punching.Swordsmasword.SwordsmaswordParry>(),

        ModContent.ProjectileType<Items.Blue.Shotguns.HaveSomeDamage>(),
        ModContent.ProjectileType<Items.Blue.Shotguns.ForYouToo>(),
    };

    public int[] noDDB = new int[]
    {
        ModContent.ProjectileType<Items.Blue.Shotguns.ShotgunPellet>(),
        ModContent.ProjectileType<Items.Green.Shotguns.PCShotgunPellet>(),
        ModContent.ProjectileType<Items.Red.Shotguns.AirburstShotgunPellet>(),
        ModContent.ProjectileType<Items.AltBlue.Shotguns.AltShotgunPellet>(),
        ModContent.ProjectileType<Items.AltGreen.Shotguns.AltPCShotgunPellet>(),
        ModContent.ProjectileType<Items.AltRed.Shotguns.AltAirburstShotgunPellet>(),
    };

    public bool[] bossConditions = new bool[]
    {
        NPC.downedSlimeKing,
        NPC.downedBoss1,
        NPC.downedBoss2,
        NPC.downedBoss3,
        NPC.downedQueenBee,
        NPC.downedDeerclops,
        Main.hardMode,
        NPC.downedQueenSlime,
        NPC.downedMechBoss1,
        NPC.downedMechBoss2,
        NPC.downedMechBoss3,
        NPC.downedPlantBoss,
        NPC.downedEmpressOfLight,
        NPC.downedFishron,
        NPC.downedGolemBoss,
        NPC.downedAncientCultist,
        NPC.downedMoonlord
    };

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        int downedBosses = 0;
        foreach (bool cond in bossConditions)
        {
            if (cond) downedBosses++;
        }
        if (ModContent.GetInstance<ServerConfigurations>().ultraPDB == ServerConfigurations.PDBType.All)
        {
            modifiers.FlatBonusDamage += 10 * downedBosses;
            modifiers.ArmorPenetration += 5 * downedBosses;
        }
        else if (ModContent.GetInstance<ServerConfigurations>().ultraPDB == ServerConfigurations.PDBType.Modded_Only)
        {
            if (projectile.ModProjectile != null && projectile.ModProjectile.Mod == Mod)
            {
                modifiers.FlatBonusDamage += 10 * downedBosses;
                modifiers.ArmorPenetration += 5 * downedBosses;
            }
        }
    }
}

public class Screenshake : GlobalItem
{
    public override bool InstancePerEntity => true;

    public int[] skipScreenshake = new int[]
    {
        ModContent.ProjectileType<Items.Green.Revolvers.EndMeCoin>(),
        ModContent.ProjectileType<Content.Punching.Swordsmasword.SwordsmaswordParry>(),
        ModContent.ProjectileType<Items.Red.Revolvers.SharpshooterSpin>(),
        ModContent.ProjectileType<Items.AltRed.Revolvers.SlabSharpshooterSpin>(),
    };

    public int[] halfScreenshake = new int[]
    {
        ModContent.ProjectileType<Items.AltBlue.Revolvers.SlabPiercerBullet>(),
        ModContent.ProjectileType<Items.AltGreen.Revolvers.SlabMarksmanBullet>(),
        ModContent.ProjectileType<Items.AltRed.Revolvers.SlabSharpshooterBullet>(),
        ModContent.ProjectileType<Items.AltZeGold.Revolvers.SlabSignalBullet>(),
        ModContent.ProjectileType<Items.AltBlue.Sawlaunchers.SilverSaw>(),
        ModContent.ProjectileType<Items.AltGreen.Sawlaunchers.NormSaw>(),
        ModContent.ProjectileType<Items.AltRed.Sawlaunchers.NerveSaw>(),
        ModContent.ProjectileType<Items.Blue.Nailguns.Nail>(),
        ModContent.ProjectileType<Items.Green.Nailguns.NormNail>(),
        ModContent.ProjectileType<Items.Green.Nailguns.FireNail>(),
        ModContent.ProjectileType<Items.Red.Nailguns.NerveNail>(),
        ModContent.ProjectileType<Items.Blue.Rifles.ElectricBullet>(),
        ModContent.ProjectileType<Items.Blue.Rifles.HeatBullet>(),
        ModContent.ProjectileType<Items.Green.Rifles.ESElectricBullet>(),
        ModContent.ProjectileType<Items.Red.Rifles.SCHeatBullet>(),
        ModContent.ProjectileType<Items.Red.Rifles.SCFlamethrower>(),
        ModContent.ProjectileType<Items.ZeGold.Rifles.PlasmaBullet>(),
        ModContent.ProjectileType<Items.Blue.RocketLaunchers.Rocket>(),
        ModContent.ProjectileType<Items.Green.RocketLaunchers.SRocket>(),
        ModContent.ProjectileType<Items.Red.RocketLaunchers.BRocket>(),
        ModContent.ProjectileType<Items.Red.RocketLaunchers.BarrageRocket>(),
        ModContent.ProjectileType<Items.ZeGold.Railcannons.CorrosiveBeam>(),
    };

    public int[] longScreenshake = new int[]
    {
        ModContent.ProjectileType<Items.Blue.Railcannons.ElectricRailshot>(),
        ModContent.ProjectileType<Items.Green.Railcannons.Screwdriver>(),
        ModContent.ProjectileType<Items.Red.Railcannons.MauriceRailshot>(),
    };


    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        List<int> lSkip = new List<int>(skipScreenshake);
        List<int> lHalf = new List<int>(halfScreenshake);
        List<int> lLong = new List<int>(longScreenshake);
        bool condition = item.ModItem != null && item.ModItem.Mod == Mod && type != ProjectileID.None && !lSkip.Contains(type);
        if (ModContent.GetInstance<ClientConfigurations>().vanillaScreenshakes) condition = type != ProjectileID.None && !lSkip.Contains(type);
        if (condition)
        {
            float mult = 0.4f;
            if (lHalf.Contains(type)) mult /= 2;
            if (type == ModContent.ProjectileType<Items.Red.Rifles.SCFlamethrower>()) mult /= 2;
            float decay = 1f;
            if (lLong.Contains(type))
            {
                decay *= 2;
                mult *= 2f;
            }

            if (item.ModItem == null) mult *= ModContent.GetInstance<ClientConfigurations>().vanillaScreenshakeMultiplier;

            PunchCameraModifier shake = new PunchCameraModifier(position, Main.rand.NextVector2CircularEdge(1, 1), damage * mult * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f * decay);
            Main.instance.CameraModifiers.Add(shake);
        }
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}