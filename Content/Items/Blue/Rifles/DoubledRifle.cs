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

namespace Terrakill.Content.Items.Blue.Rifles;

public class DoubledRifle : ModItem
{
    public int electricAmmo = 30, heatAmmo = 30;

    SoundStyle ElectricShot = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/ElectricShot")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };
    SoundStyle HeatShot = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/HeatShot")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 31;
        Item.height = 12;

        Item.damage = 30;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<ElectricBullet>();
        Item.shootSpeed = 3;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;

        Item.UseSound = ElectricShot;
    }

    public override bool AltFunctionUse(Player player)
    {
        return heatAmmo > 0;
    }

    public override bool CanUseItem(Player player)
    {
        return (Keybinds.AltFire.Current && heatAmmo > 0) || electricAmmo > 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Rifle (Doubled) - " + electricAmmo + " / " + heatAmmo);

        if (timeSinceLastFired > 11 && timer % 6 == 0) electricAmmo++;
        if (timeSinceLastAltFired > 11 && timer % 6 == 3) heatAmmo++;

        timer++;

        if (electricAmmo > 30) electricAmmo = 30;
        if (heatAmmo > 30) heatAmmo = 30;

        timeSinceLastFired++;
        timeSinceLastAltFired++;
    }

    int timeSinceLastFired = 0, timeSinceLastAltFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            damage = (int)MathF.Round(damage * 1.5f);
            SoundEngine.PlaySound(HeatShot, position);
            type = ModContent.ProjectileType<HeatBullet>();
            heatAmmo--;
            timeSinceLastAltFired = 0;
        }
        else
        {
            SoundEngine.PlaySound(ElectricShot, position);
            velocity /= velocity.Length();
            electricAmmo--;
            timeSinceLastFired = 0;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-6, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>(), 3)
            .Register();
    }
}

