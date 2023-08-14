using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Punching.Swordsmasword;

public class SwordsmaswordChargeManager : ModPlayer
{
    public int charge = 0;

    public override void ResetEffects()
    {
        if (charge > 100) charge = 100;
        if (charge < 0) charge = 0;
    }
}

