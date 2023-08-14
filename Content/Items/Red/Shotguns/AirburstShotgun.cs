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

namespace Terrakill.Content.Items.Red.Shotguns;

public class AirburstShotgun : ModItem
{
    float airbursts = 2.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/Shotgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 65;
        Item.useAnimation = 65;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AirburstShotgunPellet>();
        Item.shootSpeed = 32;


        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        Item.UseSound = Shotgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return airbursts > 1f;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width;
        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(SoundID.Item61, position);
            damage *= 2;
            velocity /= 2;
            type = ModContent.ProjectileType<AirburstBomb>();
            airbursts--;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            for (int i = 0; i < 11; i++)
            {
                Vector2 altVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15f, 15f)));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, altVelocity, type, damage, knockback, player.whoAmI);
            }
        }
    }

    public override void UpdateInventory(Player player)
    {
        airbursts += 0.005f;
        if (airbursts > 2) airbursts = 2;
        if (airbursts < 0) airbursts = 0;

        Item.SetNameOverride("Shotgun (Airburst) - " + MathF.Round(airbursts, 2));
        base.UpdateInventory(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Shotguns.CEShotgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Shotguns.PCShotgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltRed.Shotguns.AltAirburstShotgun>())
            .Register();
    }
}

