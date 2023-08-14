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

namespace Terrakill.Content.Items.Blue.RocketLaunchers;

public class FFRocketLauncher : ModItem
{
    float freezeTime = 100f, freezeTimeLastFrame = 100f;
    bool freezing = false;

    SoundStyle Rocket = new SoundStyle($"{nameof(Terrakill)}/Sounds/RocketLauncher/Rocket")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle ClockTick = new SoundStyle($"{nameof(Terrakill)}/Sounds/RocketLauncher/ClockTick")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 3
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 53;
        Item.height = 21;

        Item.damage = 100;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<Rocket>();
        Item.shootSpeed = 20;

        Item.UseSound = Rocket;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        if (freezing)
        {
            if (timer++ % 30 == 0)
                SoundEngine.PlaySound(ClockTick, player.position);
            freezeTime -= 25f / 75f;
            if (freezeTime < 0)
            {
                freezing = false;
                freezeTime = 0;
            }
        }
        else
        {
            timer = 0;
            freezeTime += 25f / 150f;
            if (freezeTime > 100f) freezeTime = 100f;
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.owner == player.whoAmI && p.active && p.type == Item.shoot)
            {
                if (freezing) p.velocity = Vector2.Zero;
                else p.velocity = new Vector2(Item.shootSpeed, 0).RotatedBy(p.rotation);
            }
        }

        Item.SetNameOverride("Rocket Launcher (Freezeframe) - " + (int)MathF.Round(freezeTime) + "%");
        freezeTimeLastFrame = freezeTime;
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
            Item.useTime = 60;
            Item.useAnimation = 60;
        }
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
            freezing = !freezing;
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
        position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltBlue.GrenadeLaunchers.FFGrenadeLauncher>())
            .Register();
    }
}

