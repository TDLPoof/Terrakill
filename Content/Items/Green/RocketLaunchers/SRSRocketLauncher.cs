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

namespace Terrakill.Content.Items.Green.RocketLaunchers;

public class SRSRocketLauncher : ModItem
{

    SoundStyle Rocket = new SoundStyle($"{nameof(Terrakill)}/Sounds/RocketLauncher/Rocket")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    float charge = 1.00f;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 53;
        Item.height = 21;

        Item.damage = 100;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        Item.shoot = ModContent.ProjectileType<SRocket>();
        Item.shootSpeed = 20;

        Item.UseSound = Rocket;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return charge >= 1.00f;
    }

    public override void UpdateInventory(Player player)
    {
        string val = "" + (int)MathF.Round(100 * charge) + "%";
        Item.SetNameOverride("Rocket Launcher (SRS) - " + val);

        charge += 0.005f;
        if (charge > 1.00f) charge = 1.00f;
        if (charge < 0f) charge = 0f;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<SRSBall>();
            SoundEngine.PlaySound(SoundID.Item61, position);
            velocity *= 0.7f;
            charge -= 1f;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.GrenadeLaunchers.SRSGrenadeLauncher>())
            .Register();
    }
}
