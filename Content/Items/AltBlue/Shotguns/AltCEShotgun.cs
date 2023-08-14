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

namespace Terrakill.Content.Items.AltBlue.Shotguns;

public class AltCEShotgun : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/AltShotgun")
    {
        PitchVariance = 0.1f,
        Volume = 0.8f,
        MaxInstances = 12
    };

    int hotCores = 0, maxHotCores = 12;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 19;
        Item.useAnimation = 19;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltShotgunPellet>();
        Item.shootSpeed = 32;

        Item.UseSound = Shotgun;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return hotCores > 0;
    }

    public override bool CanUseItem(Player player)
    {
        return (hotCores < 12) || (player.altFunctionUse == 2 && hotCores > 0);
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 0.5f;

        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<AltCoreBomb>();
            damage *= 2;
            velocity *= 0.75f;
            for (int i = 0; i < hotCores - 1; i++)
            {
                Vector2 randVel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-hotCores * 2, hotCores * 2)));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, randVel, type, damage, knockback, player.whoAmI);
            }
            hotCores = 0;
        }
        else
        {
            timeSinceLastFired = 0;
            type = ModContent.ProjectileType<AltShotgunPellet>();
            for (int i = 0; i < 8; i++)
            {
                Vector2 altVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, altVelocity, type, damage, knockback, player.whoAmI);
            }
            hotCores++;
        }
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Alternate Shotgun (Core Eject) - " + hotCores + " / " + maxHotCores);
        if (timer++ % 15 == 0 && timeSinceLastFired > 15) hotCores--;
        if (hotCores > maxHotCores) hotCores = maxHotCores;
        if (hotCores < 0) hotCores = 0;
        timeSinceLastFired++;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Shotguns.CEShotgun>())
            .Register();
    }
}

