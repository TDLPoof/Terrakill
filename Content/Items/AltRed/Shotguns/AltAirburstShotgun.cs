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

namespace Terrakill.Content.Items.AltRed.Shotguns;

public class AltAirburstShotgun : ModItem
{
    float airbursts = 6.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/AltShotgun")
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

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 21;
        Item.useAnimation = 21;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltAirburstShotgunPellet>();
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
            damage = (int)MathF.Round(damage * 1.5f);
            velocity /= 2;
            type = ModContent.ProjectileType<AltAirburstBomb>();
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
        airbursts += 0.015f;
        if (airbursts > 6) airbursts = 6;
        if (airbursts < 0) airbursts = 0;

        Item.SetNameOverride("Alternate Shotgun (Airburst) - " + MathF.Round(airbursts, 2));
        base.UpdateInventory(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltBlue.Shotguns.AltCEShotgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.Shotguns.AltPCShotgun>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Shotguns.AirburstShotgun>())
            .Register();
    }
}

