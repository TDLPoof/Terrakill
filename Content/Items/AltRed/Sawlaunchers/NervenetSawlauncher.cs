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

namespace Terrakill.Content.Items.AltRed.Sawlaunchers;

public class NervenetSawlauncher : ModItem
{
    float markers = 5.00f;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Sawlauncher")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 38;
        Item.height = 9;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<NerveSaw>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return player.ownedProjectileCounts[ModContent.ProjectileType<Red.Nailguns.NerveMarker>()] == 0;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Sawblade Launcher (Nervenet) - " + (3 - player.ownedProjectileCounts[ModContent.ProjectileType<Red.Nailguns.NerveMarker>()]));
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
            Item.useTime = 14;
            Item.useAnimation = 14;
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
            type = ModContent.ProjectileType<Red.Nailguns.NerveMarker>();
            velocity /= 3;
            velocity = velocity.RotatedBy(MathHelper.ToRadians(-20));

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<Red.Nailguns.NerveMarker>(), 0, 0, player.whoAmI);
                velocity = velocity.RotatedBy(MathHelper.ToRadians(20));
            }
        }
        else
        {
            type = ModContent.ProjectileType<NerveSaw>();
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
            .AddIngredient(ModContent.ItemType<AltBlue.Sawlaunchers.AttractorSawlauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.Sawlaunchers.OverheatSawlauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Nailguns.NervenetNailgun>())
            .Register();
    }
}

