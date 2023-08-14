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
using Terrakill.Content.Items.AltBlue.Revolvers;
using Terrakill.Content.Items.Green.Shotguns;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Terrakill.Content.Items.AltGreen.GrenadeLaunchers;

public class SRSGrenadeLauncher : ModItem
{
    float charge = 0.00f;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 53;
        Item.height = 21;

        Item.damage = 65;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.shoot = ModContent.ProjectileType<SGrenade>();
        Item.shootSpeed = 20;

        Item.UseSound = SoundID.Item61;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Grenade Launcher (SRS) - " + MathF.Round(charge, 2));

        if (player.HeldItem == Item)
        {
            if (Keybinds.AltFire.Current)
            {
                charge += 0.1f;
                if (charge > 12) charge = 12;
            }
            if (Keybinds.AltFire.JustReleased)
            {
                Vector2 velocity = player.Center.DirectionTo(Main.MouseWorld) * Item.shootSpeed * 0.6f;
                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
                if (ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "cannonballtech"
                 || ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "cbt") player.velocity = -velocity * charge / 12f / 0.6f;
                SoundEngine.PlaySound(Item.UseSound, player.position);
                PunchCameraModifier shake = new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1, 1), charge / 12f * Item.damage * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                Main.instance.CameraModifiers.Add(shake);
                for (int i = 0; i < 5; i++)
                {
                    Dust d = Dust.NewDustDirect(player.Center + muzzleOffset, 1, 1, DustID.Stone, Scale: 2f);
                    d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f));
                    d.noGravity = true;
                }
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustDirect(player.Center + muzzleOffset, 1, 1, DustID.SteampunkSteam, Scale: 2f);
                    d.velocity = velocity.RotatedByRandom(Main.rand.NextFloat(MathF.PI / -4f, MathF.PI / 4f)) * Main.rand.NextFloat(0.1f, 0.6f);
                    d.noGravity = true;
                }
                for (int i = 0; i < (int)charge; i++)
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center + muzzleOffset, Main.rand.NextFloat(0.7f, 1.3f) * velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20, 20))), ModContent.ProjectileType<SRShard>(), Item.damage / 3, 0, player.whoAmI);
                }
                charge = 0.00f;
            }
        }

        if (charge > 12) charge = 12;
        if (charge < 0) charge = 0;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-6, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.RocketLaunchers.SRSRocketLauncher>())
            .Register();
    }
}

