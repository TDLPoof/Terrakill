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

public class CoinGod : ModItem
{
    SoundStyle CoinThrow = new SoundStyle($"{nameof(Terrakill)}/Sounds/Revolver/CoinThrow")
    {
        PitchVariance = 0.1f,
        Volume = 1.0f,
        MaxInstances = 5
    };

    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<MISC>();

        Item.width = 18;
        Item.height = 10;

        Item.noMelee = true;
        Item.noUseGraphic = true;

        Item.damage = 20;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 3;
        Item.useAnimation = 3;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;


        Item.shoot = ModContent.ProjectileType<Green.Revolvers.EndMeCoin>();
        Item.shootSpeed = 10;

        Item.UseSound = CoinThrow;
    }
}

