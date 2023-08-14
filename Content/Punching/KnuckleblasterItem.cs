using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Punching;

public class KnuckleblasterItem : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 9;

        Item.rare = ModContent.RarityType<Items.R>();

        Item.maxStack = 1;

        Item.useStyle = ItemUseStyleID.RaiseLamp;

        Item.consumable = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.GetModPlayer<Punching>().knuckleblasterUnlocked) return null;
        player.GetModPlayer<Punching>().knuckleblasterUnlocked = true;
        return true;
    }
}

