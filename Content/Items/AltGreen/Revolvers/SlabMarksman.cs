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

namespace Terrakill.Content.Items.AltGreen.Revolvers;

public class SlabMarksman : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;
    float coins = 4, coinsLastFrame = 4;

    SoundStyle Revolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SlabRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 6
    };
    SoundStyle CoinThrow = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/CoinThrow")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 5
    };
    SoundStyle CoinReady = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/CoinReady")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 45;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 37;
        Item.useAnimation = 37;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SlabMarksmanBullet>();
        Item.shootSpeed = 1;

        Item.UseSound = Revolver;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    bool altFiringLastFrame = false;

    public override bool AltFunctionUse(Player player)
    {
        return coins >= 1f;
    }

    float timeSpinning = 0;
    public override void UpdateInventory(Player player)
    {
        coins += 1f / 290f;
        if (coins > 4) coins = 4;
        if (coins < 0) coins = 0;

        if (coins == (int)coins && coins != coinsLastFrame)
        {
            SoundEngine.PlaySound(CoinReady, player.position);
        }

        int visual = (int)MathF.Round(timeSpinning * 100);
        if (visual > 100) visual = 100;

        Item.SetNameOverride("Alternate Revolver (Marksman) - " + MathF.Round(coins, 2));

        coinsLastFrame = coins;
    }

    public int timesSpun = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            Item.noUseGraphic = true;
            SoundEngine.PlaySound(CoinThrow, position);
            type = ModContent.ProjectileType<Green.Revolvers.EndMeCoin>();
            velocity = new Vector2(MathF.Pow(24, 0.5f) * player.direction, -MathF.Pow(48, 0.5f));
            velocity += player.velocity;
            damage = 100;
            coins--;
        }
        else
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
            position += muzzleOffset;
            Item.noUseGraphic = false; 
            SoundEngine.PlaySound(Item.UseSound, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, -2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Revolvers.EndMeRevolver>())
            .Register();
    }
}

