using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.ZeMaterials;

public class ExperimentalCircuitry : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 14;

        Item.rare = ModContent.RarityType<Items.R>();

        Item.maxStack = 20;
    }
}

