using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Movement;

public class StaminaPlayer : ModPlayer
{
    public float statStamina = 3.00f;
    public float statStaminaMax = 3.00f;
    public float staminaRegenTime = 1.00f;

    int timeSinceLastDashed = 0;

    public override void Load()
    {
        statStamina = 3.00f;
    }

    public override void ResetEffects()
    {
        if (statStamina < 0f) statStamina = 0f;

        if (timeSinceLastDashed > 20) statStamina += staminaRegenTime / 60f;

        if (statStamina > statStaminaMax) statStamina = statStaminaMax;
    }

    public override void PostUpdate()
    {
        timeSinceLastDashed++;
        if (Player.GetModPlayer<DashingPlayer>().dashActive) timeSinceLastDashed = 0;
    }
}

