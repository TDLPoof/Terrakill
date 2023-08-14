using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Punching;

public class WhiplashItem : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 9;

        Item.rare = ModContent.RarityType<Items.G>();

        Item.maxStack = 1;

        Item.useStyle = ItemUseStyleID.RaiseLamp;

        Item.consumable = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.GetModPlayer<Punching>().whiplashUnlocked) return null;
        player.GetModPlayer<Punching>().whiplashUnlocked = true;
        return true;
    }
}

