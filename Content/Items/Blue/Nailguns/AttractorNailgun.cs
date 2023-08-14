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

namespace Terrakill.Content.Items.Blue.Nailguns;

public class AttractorNailgun : ModItem
{
    int ammo = 100;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Nailgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 44;
        Item.height = 13;

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<Nail>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return player.ownedProjectileCounts[ModContent.ProjectileType<Magnet>()] < 3;
    }

    public override bool CanUseItem(Player player)
    {
        return player.altFunctionUse == 2 || ammo > 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        string fixedAmmo = (ammo > 100) ? "100" : "" + ammo;
        Item.SetNameOverride("Nailgun (Attractor) - " + fixedAmmo + " / " + (3 - player.ownedProjectileCounts[ModContent.ProjectileType<Magnet>()]));

        if (timer++ % 6 == 0 && timeSinceLastFired > 5) ammo++;
        if (ammo > 100) ammo = 100;
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
            Item.useTime = 2;
            Item.useAnimation = 2;
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
            type = ModContent.ProjectileType<Magnet>();
            velocity *= 2f / 3f;
        }
        else
        {
            timeSinceLastFired = 0;
            type = ModContent.ProjectileType<Nail>();
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
            .AddIngredient(ModContent.ItemType<AltBlue.Sawlaunchers.AttractorSawlauncher>())
            .Register();
    }
}

