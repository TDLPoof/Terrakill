using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.PlayerDrawLayer;
using Terrakill.Content.Items.Green.Shotguns;

namespace Terrakill.Content.Items.AltRed.Revolvers;

public class SlabSharpshooter : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;
    float charges = 3;

    SoundStyle Revolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SlabRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 6
    };
    SoundStyle SuperRevolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SuperRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

        Item.width = 30;
        Item.height = 16;

        Item.damage = 45;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 37;
        Item.useAnimation = 37;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SlabSharpshooterBullet>();
        Item.shootSpeed = 1;

        Item.UseSound = Revolver;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    bool altFiringLastFrame = false;

    public override bool AltFunctionUse(Player player)
    {
        return charges >= 3;
    }

    float timeSpinning = 0;
    public override void UpdateInventory(Player player)
    {
        if (player.HeldItem == Item)
        {
            if (Keybinds.AltFire.Current && charges >= 3) timeSpinning += 1f / 180f;
            else
            {
                if (timeSpinning > 1.00f)
                {
                    Vector2 velocity = player.Center.DirectionTo(Main.MouseWorld) * Item.shootSpeed;
                    Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width;
                    Vector2 position = player.Center + muzzleOffset;
                    SoundEngine.PlaySound(SuperRevolver, position);
                    PunchCameraModifier shake = new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1, 1), Item.damage * 3.5f * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(shake);
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity,
                                             ModContent.ProjectileType<SlabSharpshooterSuper>(),
                                             (int)MathF.Round(Item.damage * 3.5f), 0f, player.whoAmI);
                    charges = 0;
                }
                timeSpinning = 0;
            }
        }
        if (timeSpinning < 0) timeSpinning = 0;
        charges += 3f / 384f;
        if (charges > 3) charges = 3;
        if (charges < 0) charges = 0;

        int visual = (int)MathF.Round(timeSpinning * 100);
        if (ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "helicopter")
        {
            if (visual > 150) player.velocity.Y -= 0.5f + ((visual - 150f) / 1000f);
        }
        else if (visual > 100) visual = 100;

        Item.SetNameOverride("Alternate Revolver (Sharpshooter) - " + visual + "% / " + MathF.Round(charges / 3f, 2));
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
            Item.useTime = 37;
            Item.useAnimation = 37;
        }
        return base.UseItem(player);
    }

    public float RotationCalc(float x)
    {
        if (x < 1.5f) return MathF.Pow(4, x) - 1;
        return (11.09f * x) - 9.635f;
    }

    public int timesSpun = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;

        if (Keybinds.AltFire.Current && charges > 1f)
        {
            if (player.direction == -1) position.X -= 45;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.noUseGraphic = true;
            position -= muzzleOffset;
            if (timesSpun++ % 10 == 0) SoundEngine.PlaySound(SoundID.Item1, position);
            type = ModContent.ProjectileType<SlabSharpshooterSpin>();
            knockback = 2.71f * RotationCalc(2 * timeSpinning) * -player.direction;
        }
        else
        {
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.noUseGraphic = false;
            SoundEngine.PlaySound(Item.UseSound, position);
            type = ModContent.ProjectileType<SlabSharpshooterBullet>();
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, -2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Red.Revolvers.SharpshooterRevolver>())
            .Register();
    }
}

