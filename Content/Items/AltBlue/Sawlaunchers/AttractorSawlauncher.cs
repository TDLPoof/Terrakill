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

namespace Terrakill.Content.Items.AltBlue.Sawlaunchers;

public class AttractorSawlauncher : ModItem
{
    int ammo = 10;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Sawlauncher")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 38;
        Item.height = 9;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SilverSaw>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return player.ownedProjectileCounts[ModContent.ProjectileType<Blue.Nailguns.Magnet>()] < 3;
    }

    public override bool CanUseItem(Player player)
    {
        return player.altFunctionUse == 2 || ammo > 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        string fixedAmmo = (ammo > 10) ? "10" : "" + ammo;
        Item.SetNameOverride("Sawblade Launcher (Attractor) - " + fixedAmmo + " / " + (3 - player.ownedProjectileCounts[ModContent.ProjectileType<Blue.Nailguns.Magnet>()]));

        if (timer++ % 40 == 0 && timeSinceLastFired > 41) ammo++;
        if (ammo > 10) ammo = 10;
        if (ammo < 0) ammo = 0;

        timeSinceLastFired++;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
        }
        else
        {
            Item.useTime = 7;
            Item.useAnimation = 7;
        }
        return base.UseItem(player);
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<Blue.Nailguns.Magnet>();
            velocity *= 2f / 3f;
        }
        else
        {
            timeSinceLastFired = 0;
            type = ModContent.ProjectileType<SilverSaw>();
            ammo--;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)));
        }

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-12, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Nailguns.AttractorNailgun>())
            .Register();
    }
}

