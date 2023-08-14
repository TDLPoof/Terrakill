using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Terrakill.Content.Items;

public class RailcannonCharge : ModPlayer
{
    public int charge = 100;
    int chargeLastFrame = 100;

    SoundStyle RailcannonReady = new SoundStyle($"{nameof(Terrakill)}/Sounds/Railcannon/RailcannonReady")
    {
        PitchVariance = 0.1f,
        Volume = 1f,
        MaxInstances = 2
    };

    int timer = 0;
    public override void PostUpdate()
    {
        if (timer++ % 9 == 0 || ModContent.GetInstance<ServerConfigurations>().developerKey.ToLower() == "fastcannon") charge++;
        if (charge > 100) charge = 100;
        if (charge < 0) charge = 0;

        if (charge == 100 && chargeLastFrame != 100)
        {
            SoundEngine.PlaySound(RailcannonReady, Player.position);
        }

        chargeLastFrame = charge;
    }
}

