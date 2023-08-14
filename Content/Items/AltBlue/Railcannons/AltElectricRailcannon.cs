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

namespace Terrakill.Content.Items.AltBlue.Railcannons;

public class AltElectricRailcannon : ModItem
{
    bool charging = false;
    int chargeLastFrame = 100;
    int charge = 100;

    SoundStyle Sniper = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/Sniper")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle RailcannonReady = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/RailcannonReady")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle BeginScope = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/BeginScope")
    {
        PitchVariance = 0f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle ScopeLoop = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/ScopeLoop")
    {
        PitchVariance = 0f,
        Volume = 0.5f,
        MaxInstances = 5
    };
    SoundStyle MaxPower = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/MaxPower")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 1
    };
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<B>();

        Item.width = 40;
        Item.height = 11;

        Item.damage = 150;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;
        Item.knockBack = 12;

        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltElectricRailshot>();
        Item.shootSpeed = 1;

        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.UseSound = Sniper;
    }

    int scopeScore = 0, rawTimeScoping = 0;

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<RailcannonCharge>().charge > 10;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            type = ProjectileID.None;
            Item.useTime = 1;
            Item.useAnimation = 1;
        }
        else
        {
            Item.useTime = 30 + scopeScore;
            Item.useAnimation = 30 + scopeScore;
            damage += 2 * scopeScore;
            scopeScore = 0;
            rawTimeScoping = 0;
            SoundEngine.PlaySound(Item.UseSound, position);
            player.GetModPlayer<RailcannonCharge>().charge -= 10;
        }

        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 2;

        position += muzzleOffset;
    }

    int timer = 0;
    bool hitMaxScope = false;
    public override void UpdateInventory(Player player)
    {
        if (Item == player.HeldItem)
        {
            player.scope = true;
            if (Keybinds.AltFire.Current && timer++ % 3 == 0)
            {
                if (rawTimeScoping < 1)
                {
                    SoundEngine.PlaySound(BeginScope, player.position);
                }
                else if (rawTimeScoping % 4 == 0)
                {
                    SoundEngine.PlaySound(ScopeLoop, player.position);
                }
                scopeScore++;
                rawTimeScoping++;

                if (scopeScore > 60)
                {
                    if (!hitMaxScope)
                    {
                        SoundEngine.PlaySound(MaxPower, player.position);
                    }
                    hitMaxScope = true;
                }
            }
            if (scopeScore > 60) scopeScore = 60;
        }
        if (!Keybinds.AltFire.Current)
        {
            scopeScore = 0;
            rawTimeScoping = 0;
            hitMaxScope = false;
        }
        float val = scopeScore / 60f;
        val *= 100f;
        Item.SetNameOverride("Alternate Railcannon (Electric) - " + player.GetModPlayer<RailcannonCharge>().charge + "% / " + MathF.Round(val) + "%");
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Blue.Railcannons.ElectricRailcannon>())
            .Register();
    }
}

