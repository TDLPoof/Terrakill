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

namespace Terrakill.Content.Items.ZeGold.Railcannons;

public class CorrosiveRailcannon : ModItem
{
    bool charging = false;

    SoundStyle Railcannon = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/Railcannon")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle RailcannonReady = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/RailcannonReady")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle RailCharge = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/RailCharge")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G2>();

        Item.width = 50;
        Item.height = 16;

        Item.damage = 300;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 12;

        Item.useTime = 1;
        Item.useAnimation = 121;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<CorrosiveBeam>();
        Item.shootSpeed = 1;

        Item.UseSound = Railcannon;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<RailcannonCharge>().charge >= 100;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
        }
        else
        {
            if (player.GetModPlayer<RailcannonCharge>().charge > 40)
            {
                type = ModContent.ProjectileType<CorrosiveBeam>();
                knockback = 0;
                damage = 10;
                if (timesCharged == 0 && player.GetModPlayer<RailcannonCharge>().charge >= 100) SoundEngine.PlaySound(RailCharge, position);
                player.GetModPlayer<RailcannonCharge>().charge--;
                timesCharged++;
            }
            else if (player.GetModPlayer<RailcannonCharge>().charge > 0)
            {
                type = ProjectileID.None;
                player.GetModPlayer<RailcannonCharge>().charge--;
            }
            else
            {
                damage = (3 * damage) / 4;
                type = ModContent.ProjectileType<CorrosiveRailshot>();
                SoundEngine.PlaySound(Item.UseSound, position);
                player.GetModPlayer<RailcannonCharge>().charge = 0;
                timesCharged = 0;
            }
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem) player.scope = true;
        Item.SetNameOverride("Railcannon (Corrosive) - " + player.GetModPlayer<RailcannonCharge>().charge + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Railcannons.MauriceRailcannon>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Railcannons.ScrewdriverRailcannon>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Railcannons.ElectricRailcannon>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.VeryExperimentalCircuitry>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltZeGold.Railcannons.AltCorrosiveRailcannon>())
            .Register();
    }
}

