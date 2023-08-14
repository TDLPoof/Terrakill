using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.ZeMaterials;

public class VeryExperimentalCircuitry : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 14;

        Item.rare = ModContent.RarityType<Items.G2>();

        Item.maxStack = 20;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ExperimentalCircuitry>(), 2);
    }
}

