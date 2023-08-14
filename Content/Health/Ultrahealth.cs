using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Health;

public class Ultrahealth : ModPlayer
{
    public bool ultrahealthActive = true;
    public bool ultraRegainingLife = false;
    public int lifeOld = int.MaxValue;

    int lifeTarget;
    public int hardDamage;
    int timer = 0;
    public int hardDamageTime = 0;
    bool dobodobo = false;

    public override void PreUpdate()
    {
        ultrahealthActive = ModContent.GetInstance<ServerConfigurations>().ultrahealth;
    }

    public override void ResetEffects()
    {
        if (ultrahealthActive)
        {
            if (!dobodobo) Player.statLife = 200;
            dobodobo = true;
            Player.statLifeMax = 400;
            Player.statLifeMax2 = 100;
            Player.endurance = 0.5f;
            Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;

            if (lifeTarget > Player.statLifeMax2 - hardDamage) lifeTarget = Player.statLifeMax2 - hardDamage;

            if (ultraRegainingLife)
            {
                Player.statLife = (int)MathF.Round(MathHelper.Lerp(1f * Player.statLife, 1f * lifeTarget, 0.6f));
                if (Player.statLife == Player.statLifeMax2 - hardDamage) ultraRegainingLife = false;
                if (Player.statLife == lifeTarget) ultraRegainingLife = false;
            }
            else if (Player.statLife > lifeOld)
            {
                Player.statLife = lifeOld;
            }
            lifeOld = Player.statLife;
            if (hardDamage > Player.statLifeMax2 - Player.statLife) hardDamage = Player.statLifeMax2 - Player.statLife;
            if (hardDamageTime > 120 && timer % 2 == 0) hardDamage--;
            if (hardDamage < 0) hardDamage = 0;
        }

        timer++;
        hardDamageTime++;
    }

    public override void UpdateDead()
    {
        hardDamage = 0;
        lifeTarget = 100;
        ultraRegainingLife = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (ultrahealthActive)
        {
            float maxDist = 400f;
            float invDist = maxDist - target.Center.Distance(Player.Center);
            if (invDist < 0) invDist = 0;
            int maxHealthRegain = (int)MathHelper.Clamp(MathF.Round(2 * MathF.Sqrt(damageDone * 1f)), 1, 20);
            lifeTarget = Player.statLife + (int)MathF.Round(maxHealthRegain * (invDist / maxDist));
            ultraRegainingLife = true;

            for (int i = 0; i < (int)MathF.Round(MathF.Sqrt(damageDone * 1f)); i++)
            {
                Dust.NewDustDirect(target.TopLeft, target.width, target.height, DustID.Blood);
            }
        }
    }

    public int[] moreHealthProjectiles = new int[]
    {
        ModContent.ProjectileType<Items.Blue.Shotguns.ShotgunPellet>(),
        ModContent.ProjectileType<Items.Green.Shotguns.PCShotgunPellet>(),
        ModContent.ProjectileType<Items.Red.Shotguns.AirburstShotgunPellet>(),
        ModContent.ProjectileType<Items.Red.Shotguns.AirburstBomb>(),
    };
    public int[] lessHealthProjectiles = new int[]
    {
        ModContent.ProjectileType<Items.Blue.Nailguns.Nail>(),
        ModContent.ProjectileType<Items.Green.Nailguns.NormNail>(),
        ModContent.ProjectileType<Items.Green.Nailguns.FireNail>(),
        ModContent.ProjectileType<Items.Red.Nailguns.NerveNail>(),
        ModContent.ProjectileType<Items.Red.Rifles.SCFlamethrower>()
    };

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        List<int> mhp = new List<int>(moreHealthProjectiles);
        List<int> lhp = new List<int>(lessHealthProjectiles);
        if (ultrahealthActive)
        {
            float maxDist = 400f;
            if (proj.type == ModContent.ProjectileType<Items.Green.Railcannons.Screwdriver>()) maxDist = 600f;
            float invDist = maxDist - target.Center.Distance(Player.Center);
            if (invDist < 0) invDist = 0;
            int maxHealthRegain = (int)MathHelper.Clamp(MathF.Round(2 * MathF.Sqrt(damageDone * 1f)), 1, 20);
            int regain = (int)MathF.Round(maxHealthRegain * (invDist / maxDist));
            if (mhp.Contains(proj.type)) regain *= 2;
            if (lhp.Contains(proj.type)) regain /= 2;
            lifeTarget = Player.statLife + regain;
            ultraRegainingLife = true;
        }
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (ultrahealthActive)
        {
            int damageDiv = 4;
            if (Main.masterMode) damageDiv = 1;
            if (Main.expertMode) damageDiv = 2;
            hardDamage += hurtInfo.Damage / damageDiv;
            hardDamageTime = 0;
        }
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        if (ultrahealthActive)
        {
            int damageDiv = 4;
            if (Main.masterMode) damageDiv = 1;
            if (Main.expertMode) damageDiv = 2;
            hardDamage += hurtInfo.Damage / damageDiv;
            hardDamageTime = 0;
        }
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        if (ultrahealthActive) modifiers.SetMaxDamage(80);
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (ultrahealthActive) modifiers.SetMaxDamage(80);
    }
}

