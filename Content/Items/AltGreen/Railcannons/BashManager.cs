using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.AltGreen.Railcannons;

public class BashManager : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public bool harpooned;

    public override void ResetEffects(NPC npc)
    {
        harpooned = false;
    }

    public override void AI(NPC npc)
    {
        if (harpooned)
        {
            foreach (NPC p in Main.npc)
            {
                if (p.Distance(npc.position) < 70) NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[p.type] = true;
            }
        }
    }

    public override void PostAI(NPC npc)
    {
        if (harpooned)
        {
            foreach (NPC p in Main.npc)
            {
                if (p.Distance(npc.position) < 70) NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[p.type] = false;
            }
        }
    }
}

