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

namespace Terrakill.Content.Items.AltZeGold.Railcannons;

public class AltCorrosiveRailcannon : ModItem
{
    bool charging = false;

    SoundStyle Sniper = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/Sniper")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G2>();

        Item.width = 40;
        Item.height = 11;

        Item.damage = 90;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 12;

        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.reuseDelay = 60;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltCorrosiveRailshot>();
        Item.shootSpeed = 1;

        Item.UseSound = Sniper;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<RailcannonCharge>().charge >= 33;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
        }
        else
        {
            damage /= 2;
            player.GetModPlayer<RailcannonCharge>().charge -= 33;
            SoundEngine.PlaySound(Item.UseSound, position);
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem) player.scope = true;
        Item.SetNameOverride("Alternate Railcannon (Corrosive) - " + player.GetModPlayer<RailcannonCharge>().charge + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltRed.Railcannons.AltMauriceRailcannon>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ZeGold.Railcannons.CorrosiveRailcannon>())
            .Register();
    }
}

