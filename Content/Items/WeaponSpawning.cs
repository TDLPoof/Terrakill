using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Terrakill.Content.Items;

public class WeaponSpawning : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.EyeofCthulhu)
        {
            int[] shotguns = new int[]
            {
                ModContent.ItemType<Blue.Shotguns.CEShotgun>(),
                ModContent.ItemType<Green.Shotguns.PCShotgun>()
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, shotguns));
        }
        if (npc.type == NPCID.SkeletronHead)
        {
            int[] nailguns = new int[]
            {
                ModContent.ItemType<Blue.Nailguns.AttractorNailgun>(),
                ModContent.ItemType<Green.Nailguns.OverheatNailgun>()
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, nailguns));
        }
        if (npc.type == NPCID.QueenBee)
        {
            int[] rocketLaunchers = new int[]
            {
                ModContent.ItemType<Blue.RocketLaunchers.FFRocketLauncher>(),
                ModContent.ItemType<Green.RocketLaunchers.SRSRocketLauncher>(),
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, rocketLaunchers));
        }
        if (npc.type == NPCID.Deerclops)
        {
            int[] rifles = new int[]
            {
                ModContent.ItemType<Blue.Rifles.DoubledRifle>(),
                ModContent.ItemType<Green.Rifles.EnergyShieldRifle>(),
                ModContent.ItemType<Red.Rifles.StreetcleanerRifle>(),
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, rifles));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Red.Rifles.RifleLore>(), 10));
        }
        if (npc.type == NPCID.WallofFlesh)
        {
            int[] railcannons = new int[]
            {
                ModContent.ItemType<Blue.Railcannons.ElectricRailcannon>(),
                ModContent.ItemType<Green.Railcannons.ScrewdriverRailcannon>(),
                ModContent.ItemType<Red.Railcannons.MauriceRailcannon>(),
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, railcannons));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Punching.KnuckleblasterItem>(), 1));
        }

        if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism || npc.type == NPCID.SkeletronPrime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>(), 3));
        }

        if (npc.type == NPCID.Plantera)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Punching.WhiplashItem>(), 1));
        }

        if (ModContent.GetInstance<ServerConfigurations>().randomRevolvers)
        {
            int[] revolvers = new int[]
            {
                ModContent.ItemType<Blue.Revolvers.PiercerRevolver>(),
                ModContent.ItemType<Green.Revolvers.EndMeRevolver>(),
                ModContent.ItemType<Red.Revolvers.SharpshooterRevolver>(),
            };
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.OneFromOptions(6, revolvers));
            }
        }
        else if (npc.type == NPCID.KingSlime)
        {
            int[] revolvers = new int[]
            {
                ModContent.ItemType<Blue.Revolvers.PiercerRevolver>(),
                ModContent.ItemType<Green.Revolvers.EndMeRevolver>(),
                ModContent.ItemType<Red.Revolvers.SharpshooterRevolver>(),
            };
            npcLoot.Add(ItemDropRule.OneFromOptions(1, revolvers));
        }
    }
}

public class StartingWeapon : ModPlayer
{
    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        List<Item> starting = new List<Item>();
        starting.Add(new Item(ModContent.ItemType<Blue.Revolvers.PiercerRevolver>()));
        return starting;
    }
}

