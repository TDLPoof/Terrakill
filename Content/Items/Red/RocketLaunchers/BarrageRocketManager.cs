using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.Red.RocketLaunchers;

public class BarrageRocketManager : GlobalItem
{
    public override bool InstancePerEntity => true;
    public int barrageRockets = 20;
}

