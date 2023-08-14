using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Red.Rifles;

public class RifleLore : ModItem
{
    /*
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Lore [Rifle]");
        Tooltip.SetDefault("A weapon created as a prototype before the Final War designed applicable in\n" +
            "a variety of situations. The rifle features several attachment options and integrations,\n" +
            "leading to many different variations, employing electric, heat, and in rare\n" +
            "cases, even plasma-based projectiles.\n \n" +
            "A common variation includes systems to fire both electric and heat projectiles, but the\n" +
            "Size constraints imposed by the dual nature of the weapon means that the cooling and\n" +
            "battery recharges take longer.");
    }
    */

    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 14;

        Item.rare = ItemRarityID.Quest;

        Item.maxStack = 20;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        gravity = 0;
        maxFallSpeed = 0.5f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}

