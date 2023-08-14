using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Terrakill.Content.Items.AltZeGold.Revolvers;

public class SlabSignal : ModItem
{
    float traps = 3, trapsLastFrame = 3;

    SoundStyle Revolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SlabRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 6
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G2>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 45;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 37;
        Item.useAnimation = 37;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SlabSignalBullet>();
        Item.shootSpeed = 1;

        Item.UseSound = Revolver;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    bool altFiringLastFrame = false;

    public override bool AltFunctionUse(Player player)
    {
        return traps >= 1f;
    }

    float timeSpinning = 0;
    public override void UpdateInventory(Player player)
    {
        traps += 1f / 200f;
        if (traps > 3) traps = 3;
        if (traps < 0) traps = 0;

        Item.SetNameOverride("Alternate Revolver (Signal) - " + MathF.Round(traps, 2));

        trapsLastFrame = traps;
    }

    public int timesSpun = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            Item.noUseGraphic = true;
            SoundEngine.PlaySound(SoundID.Item61, position);
            type = ModContent.ProjectileType<ZeGold.Revolvers.SignalTrap>();
            velocity *= 2;
            damage = 1;
            traps--;
        }
        else
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
            position += muzzleOffset;
            Item.noUseGraphic = false; 
            SoundEngine.PlaySound(Item.UseSound, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, -2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltBlue.Revolvers.SlabPiercer>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.Revolvers.SlabMarksman>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltRed.Revolvers.SlabSharpshooter>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ZeGold.Revolvers.SignalRevolver>())
            .Register();
    }
}

