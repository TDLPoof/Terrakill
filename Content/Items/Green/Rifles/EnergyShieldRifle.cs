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

namespace Terrakill.Content.Items.Green.Rifles;

public class EnergyShieldRifle : ModItem
{
    float shields = 3;

    SoundStyle ElectricShot = new SoundStyle($"{nameof(Terrakill)}/Sounds/Rifle/ElectricShot")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 30
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 31;
        Item.height = 12;

        Item.damage = 30;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<ESElectricBullet>();
        Item.shootSpeed = 6;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;

        Item.UseSound = ElectricShot;
    }

    public override bool AltFunctionUse(Player player)
    {
        return shields > 1;
    }

    int fireTimer = 0;
    public override void UpdateInventory(Player player)
    {
        shields += 1 / 600f;
        if (shields > 3) shields = 3;
        if (shields < 0) shields = 0;
        Item.SetNameOverride("Rifle (Energy Shield) - " + MathF.Round(shields, 2));
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(SoundID.NPCHit53, position);
            type = ModContent.ProjectileType<EnergyShield>();
            shields--;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            velocity /= velocity.Length();
        }

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-6, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ZeMaterials.ExperimentalCircuitry>(), 3)
            .Register();
    }
}

