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
using Terrakill.Content.Items.AltBlue.Revolvers;

namespace Terrakill.Content.Items.AltGreen.Sawlaunchers;

public class OverheatSawlauncher : ModItem
{
    int heat = 0;
    float heatsinks = 1;

    SoundStyle Nailgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Nailgun/Sawlauncher")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 60
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 38;
        Item.height = 9;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 7;
        Item.useAnimation = 7;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.useTime = 7;
        Item.useAnimation = 7;

        Item.shoot = ModContent.ProjectileType<NormSaw>();
        Item.shootSpeed = 20;

        Item.UseSound = Nailgun;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return heat > 0 && heatsinks >= 1;
    }

    int timer = 0;
    public override void UpdateInventory(Player player)
    {
        
        Item.SetNameOverride("Sawblade Launcher (Overheat) - " + heat + "% / " + MathF.Round(heatsinks, 2));

        if (timer++ % 4 == 0 && timeSinceLastFired > 17) heat--;
        if (timer % 4 == 0) heatsinks += 1f / 60f;
        if (heat > 100) heat = 100;
        if (heat < 0) heat = 0;

        if (heatsinks > 1) heatsinks = 1;
        if (heatsinks < 0) heatsinks = 0;

        if (Keybinds.AltFire.JustPressed && heat > 0 && heatsinks >= 1)
        {
            heatsinks = 0;
            SoundEngine.PlaySound(Item.UseSound, player.position);
            Vector2 velocity = player.Center.DirectionTo(Main.MouseWorld) * Item.shootSpeed * 1.1f;
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
            int damage = Item.damage * 2 + (heat / 5);
            velocity *= heat / 75f;
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center + muzzleOffset, velocity, ModContent.ProjectileType<FireSaw>(), (int)MathF.Round(damage * 3.5f), 0, player.whoAmI);
            heat = 0;
        }
        else
        {
            Item.useTime = 7 + (heat / 7) + (int)MathF.Round(6 * (1 - heatsinks));
            Item.useAnimation = 7 + (heat / 7) + (int)MathF.Round(6 * (1 - heatsinks));
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
            type = ProjectileID.None;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, player.position);
            timeSinceLastFired = 0;
            type = Item.shoot;
            heat += 6;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-heat / 8, heat / 8)));
        }

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-12, 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Green.Nailguns.OverheatNailgun>())
            .Register();
    }
}

