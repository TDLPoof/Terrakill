using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrakill.Content.Items;

namespace Terrakill;

public class PHMBossBuff : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int[] shotgunPellets = new int[]
    {
        ModContent.ProjectileType<Content.Items.Blue.Shotguns.ShotgunPellet>(),
        ModContent.ProjectileType<Content.Items.Green.Shotguns.PCShotgunPellet>(),
        ModContent.ProjectileType<Content.Items.Red.Shotguns.AirburstShotgunPellet>(),
        ModContent.ProjectileType<Content.Items.AltBlue.Shotguns.AltShotgunPellet>(),
        ModContent.ProjectileType<Content.Items.AltGreen.Shotguns.AltPCShotgunPellet>(),
        ModContent.ProjectileType<Content.Items.AltRed.Shotguns.AltAirburstShotgunPellet>(),
    };

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (npc.boss || npc.knockBackResist == 0 && projectile.ModProjectile != null && projectile.ModProjectile.Mod == Mod)
        {
            if (new List<int>(shotgunPellets).Contains(projectile.type)) modifiers.FinalDamage *= 0.5f;
        }
    }
}

