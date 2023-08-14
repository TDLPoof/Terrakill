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

namespace Terrakill.Content.Items.AltBlue.Revolvers;

public class SlabPiercer : ModItem
{
    bool charging = false;
    float charge = 1.00f;
    float chargeLastFrame = 1.00f;

    SoundStyle Revolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SlabRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 6
    };
    SoundStyle BatteryFull = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/BatteryFull")
    {
        PitchVariance = 0.1f,
        Volume = 0.8f,
        MaxInstances = 2
    };
    SoundStyle PierceCharge = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/PierceCharge")
    {
        PitchVariance = 0.1f,
        Volume = 0.8f,
        MaxInstances = 2
    };
    SoundStyle SuperRevolver = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/SuperRevolver")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 30;
        Item.height = 16;

        Item.damage = 45;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<SlabPiercerBullet>();
        Item.shootSpeed = 1;

        Item.UseSound = Revolver;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return charge >= 1.00f || charging;
    }

    public override bool CanUseItem(Player player)
    {
        if (Keybinds.AltFire.Current) return charge >= 1.00f || charging;
        return charge > 1f / 3f;
    }

    bool altFiringLastFrame = false;

    public override void UpdateInventory(Player player)
    {
        string value = "" + MathF.Round(charge, 2);
        if (value == "1.01") value = "1";
        if (value == "0") value = "0.00";
        else if (value == "1") value = "1.00";
        else if (value.ToCharArray()[value.Length - 1] == '0') value = value + "0";
        if (charge < 0.05) value = "0.00";
        Item.SetNameOverride("Alternate Revolver (Piercer) - " + value);

        if (!charging)
        {
            charge += 0.0066f;
        }
        if (charge > 1.00f) charge = 1.00f;
        if (charge < 0.00f) charge = 0.00f;

        if (charge == 1.00f && chargeLastFrame != 1.00f)
            SoundEngine.PlaySound(BatteryFull, player.Center);

        if (player.HeldItem == Item)
        {
            if (charge < 0 && Keybinds.AltFire.Current) charge = 0.00f;

            if (charging && charge < 0.05f && !Keybinds.AltFire.Current)
            {
                Vector2 velocity = player.Center.DirectionTo(Main.MouseWorld);
                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;
                int damage = (int)MathF.Round(20f + (30f * (2.00f - charge)));
                SoundEngine.PlaySound(SuperRevolver, player.position);
                PunchCameraModifier shake = new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1, 1), damage * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
                Main.instance.CameraModifiers.Add(shake);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center + muzzleOffset, velocity, ModContent.ProjectileType<SlabPiercerSuper>(), (int)MathF.Round(damage * 3.5f), 0, player.whoAmI);
                }
                charge = 0.00f;
                charging = false;
            }
        }

        if (!Keybinds.AltFire.Current) charging = false;

        chargeLastFrame = charge;
        altFiringLastFrame = Keybinds.AltFire.Current;
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
            Item.useTime = 18;
            Item.useAnimation = 18;
        }
        return base.UseItem(player);
    }

    int timesCharged = 0;

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;

        if (Keybinds.AltFire.Current)
        {
            if (timesCharged++ % 10 == 0) SoundEngine.PlaySound(PierceCharge, position);
            charging = true;
            charge -= 0.03f;
            PunchCameraModifier shake = new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1, 1), (1 - charge) * 3 * ModContent.GetInstance<ClientConfigurations>().screenshakeMultiplier, 12f, 10, 1000f);
            Main.instance.CameraModifiers.Add(shake);

            int size = (int)MathF.Round(100 - (charge * 100));
            if (!Main.dedServ)
            {
                for (int i = 0; i < size; i++)
                {
                    Dust d = Dust.NewDustDirect(position + Main.rand.NextVector2Circular(size / 2, size / 2), 1, 1, DustID.Clentaminator_Cyan);
                    d.noGravity = true;
                    d.velocity = player.velocity;
                }
            }
            type = ProjectileID.None;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            type = ModContent.ProjectileType<SlabPiercerBullet>();
            charge -= 1f / 3f;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, -2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Revolvers.PiercerRevolver>())
            .Register();
    }
}

