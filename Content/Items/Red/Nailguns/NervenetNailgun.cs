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

namespace Terrakill.Content.Items.Red.Nailguns;

public class NervenetNailgun : ModItem
{
    float markers = 5.00f;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Nailgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 44;
        Item.height = 13;

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 4;
        Item.useAnimation = 4;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<NerveNail>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return player.ownedProjectileCounts[ModContent.ProjectileType<NerveMarker>()] < 5;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Nailgun (Nervenet) - " + (5 - player.ownedProjectileCounts[ModContent.ProjectileType<NerveMarker>()]));
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
        }
        else
        {
            Item.useTime = 4;
            Item.useAnimation = 4;
        }
        return base.UseItem(player);
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<NerveMarker>();
            velocity /= 3;
        }
        else
        {
            type = ModContent.ProjectileType<NerveNail>();
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)));
        }

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-12, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Nailguns.AttractorNailgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Nailguns.OverheatNailgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltRed.Sawlaunchers.NervenetSawlauncher>())
            .Register();
    }
}

