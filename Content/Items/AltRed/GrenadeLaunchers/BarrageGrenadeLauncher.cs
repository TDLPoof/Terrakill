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
using Terrakill.Content.Items.AltGreen.Shotguns;

namespace Terrakill.Content.Items.AltRed.GrenadeLaunchers;

public class BarrageGrenadeLauncher : ModItem
{
    float fired = 0.00f;

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<R>();

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
        Item.shoot = ModContent.ProjectileType<BGrenade>();
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
        Item.SetNameOverride("Grenade Launcher (Barrage) - " + MathF.Round(100f * fired / 15f) + "%");

        fired -= 0.05f;
        if (fired < 0) fired = 0;
        if (fired > 15) fired = 15;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.reuseDelay = 9;
        }
        else
        {
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.reuseDelay = 0;
        }
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
        position += muzzleOffset;
        if (player.altFunctionUse == 2)
        {
            fired++;
            if (Main.rand.NextFloat(0.6f, 1f) < fired / 15f)
            {
                Explode(position, 75, 75, DustID.Torch, DustID.OrangeTorch);
                type = ProjectileID.None;
            }
        }
        SoundEngine.PlaySound(Item.UseSound, position);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-6, 0);
    }

    public void Explode(Vector2 position, int size, int damage, int dustID = DustID.Torch, int altDustID = -1)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, position);

        if (!Main.dedServ)
        {
            for (int i = 0; i < size; i++)
            {
                Dust d = Dust.NewDustDirect(position + Main.rand.NextVector2Circular(size, size), 1, 1, dustID, Scale: 2f);
                d.noGravity = true;
            }
            if (altDustID != -1)
            {
                for (int i = 0; i < size; i++)
                {
                    Dust d = Dust.NewDustDirect(position + Main.rand.NextVector2Circular(size, size), 1, 1, altDustID, Scale: 2f);
                    d.noGravity = true;
                }
            }
        }

        foreach (NPC npc in Main.npc)
        {
            if (npc.Distance(position) > size) continue;
            float distFactor = 1.00f - (npc.Distance(position) / size);
            if (npc.friendly)
            {
                Projectile.NewProjectileDirect(Item.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<AltPCSelfDamage>(), 10, 0, Item.playerIndexTheItemIsReservedFor);
            }
            else
            {
                Projectile.NewProjectileDirect(Item.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<AltPCDamage>(), (int)MathF.Round(damage * distFactor), 0, Item.playerIndexTheItemIsReservedFor);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(position) > size) continue;
            Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<AltPCSelfDamage>(), 10, 0, Item.playerIndexTheItemIsReservedFor);
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.RocketLaunchers.SRSRocketLauncher>())
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltBlue.GrenadeLaunchers.FFGrenadeLauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.GrenadeLaunchers.SRSGrenadeLauncher>())
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>())
            .Register();
    }
}

