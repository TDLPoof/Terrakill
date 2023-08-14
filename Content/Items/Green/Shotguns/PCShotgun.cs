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
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Terrakill.Content.Items.Green.Shotguns;

public class PCShotgun : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/Shotgun")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };
    SoundStyle ShotgunPump = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/ShotgunPump")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<G>();

        Item.width = 27;
        Item.height = 17;

        Item.damage = 10;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 55;
        Item.useAnimation = 55;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<PCShotgunPellet>();
        Item.shootSpeed = 32;


        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        Item.UseSound = Shotgun;
    }

    bool altFiringLastFrame = false;

    int timesCharged = 0;

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(ShotgunPump, position);
            type = ProjectileID.None;
            timesCharged++;
            if (timesCharged > 3) timesCharged = 3;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            type = ModContent.ProjectileType<PCShotgunPellet>();
            if (timesCharged < 3)
            {
                for (int i = 0; i < 9 + (3 * timesCharged); i++)
                {
                    Vector2 altVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-12f - (3 * timesCharged), 12f + (3 * timesCharged))));
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, altVelocity, type, damage, knockback, player.whoAmI);
                }
            }
            else
            {
                Explode(position, 100, 50, DustID.Torch);
                type = ProjectileID.None;
            }
            timesCharged = 0;
        }
    }

    public override void UpdateInventory(Player player)
    {
        string wuhOh = (timesCharged > 2) ? "!" : "";
        Item.SetNameOverride("Shotgun (Pump Charge) - " + timesCharged + wuhOh);
        base.UpdateInventory(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 2);
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
                    ModContent.ProjectileType<PCSelfDamage>(), 35, 0, Item.playerIndexTheItemIsReservedFor);
            }
            else
            {
                Projectile.NewProjectileDirect(Item.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<PCDamage>(), (int)MathF.Round(damage * distFactor), 0, Item.playerIndexTheItemIsReservedFor);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(position) > size) continue;
            Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<PCSelfDamage>(), 35, 0, Item.playerIndexTheItemIsReservedFor);
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<AltGreen.Shotguns.AltPCShotgun>())
            .Register();
    }
}

