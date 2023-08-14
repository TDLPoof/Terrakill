using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill;

public class FireBuff : GlobalBuff
{
    public override void Update(int type, NPC npc, ref int buffIndex)
    {
        if ((ModContent.GetInstance<Content.ServerConfigurations>().ultrafire == Content.ServerConfigurations.FireType.Enemies_Only
         || ModContent.GetInstance<Content.ServerConfigurations>().ultrafire == Content.ServerConfigurations.FireType.All)
         && type == BuffID.OnFire3)
        {
            npc.lifeRegen -= (int)MathF.Round(195 * (1 - npc.knockBackResist)) - 5;
            if (npc.boss) npc.lifeRegen -= 200;
            if (npc.HasBuff(BuffID.Oiled)) npc.lifeRegen -= 200;
        }
    }
}
public class CursedInfernoRework : ModPlayer
{
    public override void UpdateBadLifeRegen()
    {
        if (ModContent.GetInstance<Content.ServerConfigurations>().ultrafire == Content.ServerConfigurations.FireType.Player_Only
         || ModContent.GetInstance<Content.ServerConfigurations>().ultrafire == Content.ServerConfigurations.FireType.All)
        {
            if (Player.HasBuff(BuffID.OnFire))
            {
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 25) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
            if (Player.HasBuff(BuffID.OnFire3))
            {
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 50) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
            if (Player.HasBuff(BuffID.Frostburn))
            {
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 35) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
            if (Player.HasBuff(BuffID.Frostburn2))
            {
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 60) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
            if (Player.HasBuff(BuffID.ShadowFlame))
            {
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 65) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
            if (Player.HasBuff(BuffID.CursedInferno))
            {
                Player.lifeRegen += 10;
                if (Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage < 70) Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamage++;
                Player.GetModPlayer<Content.Health.Ultrahealth>().hardDamageTime = 0;
            }
        }
    }
}