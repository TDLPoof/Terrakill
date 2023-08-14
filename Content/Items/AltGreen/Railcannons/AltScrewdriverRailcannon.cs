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

namespace Terrakill.Content.Items.AltGreen.Railcannons;

public class AltScrewdriverRailcannon : ModItem
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
        Item.rare = ModContent.RarityType<G>();

        Item.width = 40;
        Item.height = 11;

        Item.damage = 150;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 0;

        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltScrewdriver>();
        Item.shootSpeed = 5;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.UseSound = Sniper;
    }

    int scopeScore = 0;

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<RailcannonCharge>().charge >= 20;
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
            Item.useTime = 30;
            Item.useAnimation = 30;
        }
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustDirect(position, 1, 1, DustID.Clentaminator_Green, Scale: 1.5f);
                d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f));
                d.noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust d = Dust.NewDustDirect(position, 1, 1, DustID.Clentaminator_Green, Scale: 1.5f);
                d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f)) * Main.rand.NextFloat(0.1f, 0.6f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(Item.UseSound, position);
            player.GetModPlayer<RailcannonCharge>().charge -= 20;
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
        }
        Item.SetNameOverride("Alternate Railcannon (Screwdriver) - " + player.GetModPlayer<RailcannonCharge>().charge + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Railcannons.ScrewdriverRailcannon>())
            .Register();
    }
}

