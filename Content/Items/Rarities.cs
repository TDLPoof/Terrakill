using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items;

public class R : ModRarity
{
    public override Color RarityColor => new Color(255, 0, 0);
}

public class G : ModRarity
{
    public override Color RarityColor => new Color(0, 255, 0);
}

public class B : ModRarity
{
    public override Color RarityColor => new Color(0, 235, 255);
}

public class G2 : ModRarity
{
    public override Color RarityColor => new Color(254, 212, 69);
}

public class MISC : ModRarity
{
    public override Color RarityColor => new Color(255, 0, 255);
}