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

namespace Terrakill.Content.Items.ZeMiscellaneous;

public class SoapBar : ModItem
{

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<MISC>();

        Item.width = 18;
        Item.height = 10;

        Item.noMelee = true;
        Item.noUseGraphic = true;

        Item.damage = 20;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Swing;


        Item.shoot = ModContent.ProjectileType<SoapProjectile>();
        Item.shootSpeed = 10;

        Item.UseSound = SoundID.Item1;
    }
}

