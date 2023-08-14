using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Terrakill.Content.Punching.Swordsmasword;

public class Swordsmasword : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<Items.G2>();

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 1;
        Item.useTime = 1;
        Item.UseSound = SoundID.Item22;
        ItemID.Sets.SkipsInitialUseSound[Type] = true;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.autoReuse = true;

        Item.damage = 100;
        Item.knockBack = 5.6f;
        Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

        Item.width = 26;
        Item.height = 26;
        Item.scale = 2f;

        // Projectile Properties
        Item.shootSpeed = 3.7f; // The speed of the projectile measured in pixels per frame.
        Item.shoot = ModContent.ProjectileType<SwordsmaswordParry>(); // The projectile that is fired from this weapon
    }

    int uses = 0;

    public override bool? UseItem(Player player)
    {
        if (uses % 20 == 0) SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item22 : SoundID.Item23, player.position);
        uses++;
        return null;
    }
}