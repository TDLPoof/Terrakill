using System;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content;

public class Hitstop : ModSystem
{
    public int hitstopping = 0;
    public override void PostUpdateEverything()
    {
        if (hitstopping > 0) Thread.Sleep((int)MathF.Round(hitstopping * ModContent.GetInstance<ClientConfigurations>().hitstopMultiplier));

        hitstopping = 0;
    }
}

