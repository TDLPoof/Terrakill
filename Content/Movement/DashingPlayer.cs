using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Terrakill.Content.Movement;

public class DashingPlayer : ModPlayer
{
    public bool dashActive = false;
    int dashTimer = 0;
    bool jumpedThisDash = false;
    SoundStyle dash = new SoundStyle($"{nameof(Terrakill)}/Sounds/Movement/Dash")
    {
        Volume = 0.9f,
        PitchVariance = 0.2f,
        MaxInstances = 3,
    };

    public override void PreUpdateMovement()
    {
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.Dashing_Only ||
            ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.All)
        {

            Player.wallSpeed = 2f;
            Player.equippedAnyWallSpeedAcc = true;
            if (!dashActive
             && Player.GetModPlayer<StaminaPlayer>().statStamina > 1f
             && Player.controlLeft && Keybinds.DashKeybind.JustPressed)
            {
                SoundEngine.PlaySound(dash, Player.position);
                Player.velocity.X = -14;
                Player.AddImmuneTime(ImmunityCooldownID.General, 60);
                Player.GetModPlayer<StaminaPlayer>().statStamina -= 1f;
                dashActive = true;
            }

            if (!dashActive
             && Player.GetModPlayer<StaminaPlayer>().statStamina > 1f
             && Player.controlRight && Keybinds.DashKeybind.JustPressed)
            {
                SoundEngine.PlaySound(dash, Player.position);
                Player.velocity.X = 14;
                Player.AddImmuneTime(ImmunityCooldownID.General, 60);
                Player.GetModPlayer<StaminaPlayer>().statStamina -= 1f;
                dashActive = true;
            }

            if (dashActive && Player.controlJump && !jumpedThisDash && Player.TouchedTiles.Count > 0)
            {
                jumpedThisDash = true;
                Player.velocity.X *= 2f;
            }
        }
    }

    public override void PostUpdate()
    {
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.Dashing_Only ||
            ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.All)
        {
            if (dashActive)
            {
                if (!Main.dedServ)
                {
                    Dust.NewDustDirect(Player.Top, 1, 1, DustID.SilverFlame, 0, 0, Scale: 2f).noGravity = true;
                    Dust.NewDustDirect(Player.Center, 1, 1, DustID.SilverFlame, 0, 0, Scale: 2f).noGravity = true;
                    Dust.NewDustDirect(Player.Bottom, 1, 1, DustID.SilverFlame, 0, 0, Scale: 2f).noGravity = true;
                }
                if (dashTimer++ > 40)
                {
                    dashActive = false;
                    jumpedThisDash = false;
                    dashTimer = 0;
                }
            }
        }
    }
}

