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

namespace Terrakill.Content.Items.Green.Railcannons;

public class ScrewdriverRailcannon : ModItem
{
    bool charging = false;
    int chargeLastFrame = 100;
    int charge = 100;

    SoundStyle Railcannon = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/Railcannon")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 50;
        Item.height = 16;

        Item.damage = 300;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 12;

        Item.useTime = 60;
        Item.useAnimation = 40;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<Screwdriver>();
        Item.shootSpeed = 5;

        Item.UseSound = Railcannon;
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
            Item.useTime = 1;
            Item.useAnimation = 1;
        }
        else
        {
            Item.useTime = 60;
            Item.useAnimation = 40;
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;

        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(position, 1, 1, DustID.Clentaminator_Green, Scale: 2f);
            d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f));
            d.noGravity = true;
        }
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(position, 1, 1, DustID.Clentaminator_Green, Scale: 2f);
            d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f)) * Main.rand.NextFloat(0.1f, 0.6f);
            d.noGravity = true;
        }
        player.GetModPlayer<RailcannonCharge>().charge = 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem) player.scope = true;
        Item.SetNameOverride("Railcannon (Screwdriver) - " + player.GetModPlayer<RailcannonCharge>().charge + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.Railcannons.AltScrewdriverRailcannon>())
            .Register();
    }
}

