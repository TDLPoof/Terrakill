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

namespace Terrakill.Content.Items.Green.Nailguns;

public class OverheatNailgun : ModItem
{
    int heat = 0;
    float heatsinks = 2;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Nailgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 60
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 44;
        Item.height = 13;

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<NormNail>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
    }

    public override bool AltFunctionUse(Player player)
    {
        return heat > 0 && heatsinks > 0;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        
        Item.SetNameOverride("Nailgun (Overheat) - " + heat + "% / " + MathF.Round(heatsinks, 2));

        if (timer++ % 4 == 0 && timeSinceLastFired > 5) heat--;
        if (timer % 4 == 0) heatsinks += 1f / 60f;
        if (heat > 100) heat = 100;
        if (heat < 0) heat = 0;

        if (heatsinks > 2) heatsinks = 2;
        if (heatsinks < 0) heatsinks = 0;

        if (Keybinds.AltFire.JustPressed && heat > 0 && heatsinks > 0) heatsinks--;
        if (Keybinds.AltFire.Current)
        {
            Item.useTime = 1;
            Item.useAnimation = heat;
        }
        else
        {
            Item.useTime = 2 + (heat / 20) + (int)MathF.Round(4 * (2 - heatsinks));
            Item.useAnimation = 2 + (heat / 20) + (int)MathF.Round(4 * (2 - heatsinks));
        }

        timeSinceLastFired++;
    }

    int timeSinceLastFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 1.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            Item.shoot = ModContent.ProjectileType<FireNail>();
            damage *= 2;
            heatsinks = (int)heatsinks;
            heat -= 2;
            if (heat < 0) heat = 0;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)));
        }
        else
        {
            timeSinceLastFired = 0;
            Item.shoot = ModContent.ProjectileType<NormNail>();
            heat++;
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
            .AddIngredient(ModContent.ItemType<AltGreen.Sawlaunchers.OverheatSawlauncher>())
            .Register();
    }
}

