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

namespace Terrakill.Content.Items.Red.Rifles;

public class StreetcleanerRifle : ModItem
{
    SoundStyle HeatShot = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/HeatShot")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };
    SoundStyle HeatFullCharge = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/HeatFullCharge")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle Fire = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/Fire")
    {
        PitchVariance = 0.1f,
        Volume = 0.3f,
        MaxInstances = 30
    };
    SoundStyle HeatCharge = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/HeatCharge")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    float charge = 0.00f, chargeLastFrame = 0.00f;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 31;
        Item.height = 12;

        Item.damage = 30;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SCHeatBullet>();
        Item.shootSpeed = 6;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;

        Item.UseSound = HeatShot;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    int fireTimer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem)
        {
            if (!Keybinds.AltFire.Current) charge -= 0.01f;
            if (Keybinds.AltFire.Current && fireTimer == 0)
            {
                charge += 1f / 200f;
            }
            if (Keybinds.AltFire.Current & charge >= 1f)
            {
                fireTimer = (int)MathF.Round(charge * 100);
                Item.useTime = 2;
                Item.useAnimation = fireTimer * 2;
                Item.shootSpeed = 20;
                SoundEngine.PlaySound(Fire, player.position);
                Item.shoot = ModContent.ProjectileType<SCFlamethrower>();
            }
            fireTimer--;
            if (charge > 1.00f) charge = 1.00f;
            if (charge < 0.00f) charge = 0.00f;
            if (fireTimer < 0)
            {
                fireTimer = 0;
                Item.useTime = 15;
                Item.useAnimation = 15;
                Item.shootSpeed = 9;
                Item.shoot = ModContent.ProjectileType<SCHeatBullet>();
            }

            if (charge == 1.00f && chargeLastFrame != 1.00f) SoundEngine.PlaySound(HeatFullCharge, player.position);
            if ((int)(charge * 100) % 20 == 0 && charge < 1.00f && charge > 0f)
            {
                SoundEngine.PlaySound(HeatCharge, player.position);
            }
        }

        Item.SetNameOverride("Rifle (Streetcleaner) - " + MathF.Round(charge * 100) + "%");

        chargeLastFrame = charge;
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            if (fireTimer > 0)
            {
                type = ModContent.ProjectileType<SCFlamethrower>();
                charge -= 0.02f;
            }
            else
            {
                type = ProjectileID.None;
            }
        }
        else
        {
            SoundEngine.PlaySound(HeatShot, position);
            velocity /= velocity.Length();
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

