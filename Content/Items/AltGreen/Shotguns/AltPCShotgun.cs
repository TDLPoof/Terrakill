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

namespace Terrakill.Content.Items.AltGreen.Shotguns;

public class AltPCShotgun : ModItem
{
    bool charging = false;
    float chargeLastFrame = 1.00f;

    SoundStyle Shotgun = new SoundStyle($"{nameof(Terrakill)}/Sounds/Shotgun/AltShotgun")
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

        Item.damage = 7;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 23;
        Item.useAnimation = 23;
        Item.autoReuse = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<AltPCShotgunPellet>();
        Item.shootSpeed = 24;


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
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * 0.5f;

        position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(ShotgunPump, position);
            type = ProjectileID.None;
            timesCharged++;
            if (timesCharged > 12) timesCharged = 12;
        }
        else
        {
            SoundEngine.PlaySound(Item.UseSound, position);
            type = ModContent.ProjectileType<AltPCShotgunPellet>();
            if (timesCharged < 12)
            {
                for (int i = 0; i < 3 + (3 * timesCharged); i++)
                {
                    Vector2 altVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-12f - (timesCharged), 12f + (timesCharged))));
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
        string wuhOh = (timesCharged > 11) ? "!" : "";
        Item.SetNameOverride("Alternate Shotgun (Pump Charge) - " + timesCharged + wuhOh);
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
            .AddIngredient(ModContent.ItemType<Green.Shotguns.PCShotgun>())
            .Register();
    }
}

