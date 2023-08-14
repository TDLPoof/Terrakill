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

namespace Terrakill.Content.Items.AltRed.Railcannons;

public class AltMauriceRailcannon : ModItem
{
    bool charging = false;
    int chargeLastFrame = 100;
    int charge = 100;

    SoundStyle Sniper = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/Sniper")
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

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 40;
        Item.height = 11;

        Item.damage = 150;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 12;

        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltMauriceRailshot>();
        Item.shootSpeed = 1;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.UseSound = Sniper;
    }

    int scopeScore = 0;

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<RailcannonCharge>().charge > 15;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 1;
            Item.useAnimation = 1;
        }
        else
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
        }
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2 || charge <= 10)
        {
            type = ProjectileID.None;
        }
        else
        {
            damage /= 2;
            SoundEngine.PlaySound(Item.UseSound, position);
            player.GetModPlayer<RailcannonCharge>().charge -= 15;
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem)
        {
            player.scope = true;
            if (Keybinds.AltFire.Current && timer++ % 3 == 0) scopeScore++;
            if (scopeScore > 60) scopeScore = 60;
        }
        if (!Keybinds.AltFire.Current) scopeScore = 0;
        float val = scopeScore / 60f;
        val *= 100f;
        Item.SetNameOverride("Alternate Railcannon (Malicious) - " + player.GetModPlayer<RailcannonCharge>().charge + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Railcannons.MauriceRailcannon>())
            .Register();
    }
}

