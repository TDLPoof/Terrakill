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

namespace Terrakill.Content.Items.Blue.Shotguns;

public class CEShotgun : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/Shotgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 75;
        Item.useAnimation = 75;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<ShotgunPellet>();
        Item.shootSpeed = 32;

        Item.UseSound = Shotgun;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<CoreBomb>();
            damage *= 2;
            velocity *= 0.75f;
        }
        else
        {
            type = ModContent.ProjectileType<ShotgunPellet>();
            for (int i = 0; i < 11; i++)
            {
                Vector2 altVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15)));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, altVelocity, type, damage, knockback, player.whoAmI);
            }
        }
    }

    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Shotgun (Core Eject)");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltBlue.Shotguns.AltCEShotgun>())
            .Register();
    }
}

