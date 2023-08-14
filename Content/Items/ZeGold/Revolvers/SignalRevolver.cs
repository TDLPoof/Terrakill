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

namespace Terrakill.Content.Items.ZeGold.Revolvers;

public class SignalRevolver : ModItem
{
    float traps = 6, trapsLastFrame = 6;

    SoundStyle Revolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/Revolver")
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

        Item.damage = 20;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SignalBullet>();
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
        traps += 1f / 160f;
        if (traps > 6) traps = 6;
        if (traps < 0) traps = 0;

        Item.SetNameOverride("Revolver (Signal) - " + MathF.Round(traps, 2));

        trapsLastFrame = traps;
    }

    public int timesSpun = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            Item.noUseGraphic = true;
            SoundEngine.PlaySound(SoundID.Item61, position);
            type = ModContent.ProjectileType<SignalTrap>();
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
            .AddIngredient(ModContent.ItemType<Red.Revolvers.SharpshooterRevolver>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Revolvers.EndMeRevolver>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Revolvers.PiercerRevolver>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltZeGold.Revolvers.SlabSignal>())
            .Register();
    }
}

