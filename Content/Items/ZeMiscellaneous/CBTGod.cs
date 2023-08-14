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

public class CBTGod : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<MISC>();

        Item.width = 18;
        Item.height = 10;

        Item.noMelee = true;
        Item.noUseGraphic = true;

        Item.damage = 100;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 6;
        Item.useAnimation = 6;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<Green.RocketLaunchers.SRSBall>();
        Item.shootSpeed = 20 * 0.7f;

        Item.UseSound = SoundID.Item61;
    }
}

