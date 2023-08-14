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

namespace Terrakill.Content.Items.Red.RocketLaunchers;

public class BarrageRocketLauncher : ModItem
{

    SoundStyle Rocket = new SoundStyle($"{nameof(Terrakill)}/Sounds/RocketLauncher/Rocket")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle BarrageRocket = new SoundStyle($"{nameof(Terrakill)}/Sounds/RocketLauncher/BarrageRocket")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 13
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 53;
        Item.height = 21;

        Item.damage = 100;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;

        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        Item.shoot = ModContent.ProjectileType<BRocket>();
        Item.shootSpeed = 20;

        Item.UseSound = Rocket;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return Item.GetGlobalItem<BarrageRocketManager>().barrageRockets > 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item.GetGlobalItem<BarrageRocketManager>().barrageRockets > 20) Item.GetGlobalItem<BarrageRocketManager>().barrageRockets = 20;
        if (Item.GetGlobalItem<BarrageRocketManager>().barrageRockets < 0) Item.GetGlobalItem<BarrageRocketManager>().barrageRockets = 0;
        Item.SetNameOverride("Rocket Launcher (Barrage) - " + Item.GetGlobalItem<BarrageRocketManager>().barrageRockets);
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 7;
            Item.useAnimation = 7;
        }
        else
        {
            Item.useTime = 60;
            Item.useAnimation = 60;
        }
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<BarrageRocket>();
            Item.GetGlobalItem<BarrageRocketManager>().barrageRockets--;
            SoundEngine.PlaySound(BarrageRocket, position);
            velocity *= 1.2f;
            float amt = MathHelper.ToRadians(Main.rand.NextFloat(-5f, 5f));
            velocity = velocity.RotatedBy(amt);
            Vector2 vCorr = velocity.RotatedBy(-amt);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, vCorr.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5f, 5f))), type, damage, knockback, player.whoAmI);
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltRed.GrenadeLaunchers.BarrageGrenadeLauncher>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.RocketLaunchers.FFRocketLauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.RocketLaunchers.SRSRocketLauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
    }
}

