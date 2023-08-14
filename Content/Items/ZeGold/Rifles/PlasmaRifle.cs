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

namespace Terrakill.Content.Items.ZeGold.Rifles;

public class PlasmaRifle : ModItem
{
    SoundStyle PlasmaShot = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/PlasmaShot")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    float charge = 1.00f, chargeLastFrame = 1.00f;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G2>();

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


        Item.shoot = ModContent.ProjectileType<PlasmaBullet>();
        Item.shootSpeed = 1;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;

        Item.UseSound = PlasmaShot;
    }

    public override bool AltFunctionUse(Player player)
    {
        return charge > 0.5f;
    }

    int fireTimer = 0;
    public override void UpdateInventory(Player player)
    {
        charge += 0.001f;
        if (charge > 1.00f) charge = 1;
        if (charge < 0) charge = 0;
        Item.SetNameOverride("Rifle (Plasma) - " + ((int)(1000 * charge) / 10) + "%");

        chargeLastFrame = charge;
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(SoundID.Item117, position);
            velocity *= 16;
            type = ModContent.ProjectileType<PlasmaVortex>();
            charge -= 0.5f;
        }
        else
        {
            SoundEngine.PlaySound(PlasmaShot, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-6, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Rifles.StreetcleanerRifle>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Rifles.EnergyShieldRifle>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Rifles.DoubledRifle>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
    }
}

