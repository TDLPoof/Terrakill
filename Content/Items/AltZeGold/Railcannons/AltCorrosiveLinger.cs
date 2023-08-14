using System;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terrakill.Content.Items.Green.Revolvers;
using Terrakill.Content.Items.Red.Shotguns;

namespace Terrakill.Content.Items.AltZeGold.Railcannons;

public class AltCorrosiveLinger : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 30;

        Projectile.penetrate = 5;
        Projectile.ArmorPenetration = 9999;

        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 5;
    }

    public List<Dust> dusts = new List<Dust>();

    public void MakeADust(int type, float scale = 1f, float velocityMult = 1f, bool velocityChange = false)
    {
        Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, type, Scale: scale);
        d.noGravity = true;
        d.velocity = Vector2.Zero;
        if (velocityChange) d.velocity += Main.rand.NextVector2CircularEdge(3, 3);
    }

    public override void AI()
    {
        if (Main.rand.NextBool(10) && !Main.dedServ && Projectile.Distance(Main.player[Projectile.owner].position) < Main.screenWidth * 0.75f)
        {
            MakeADust(DustID.GoldFlame, 1f, 0);
            MakeADust(DustID.YellowTorch, 0.75f, 0);
        }

        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        modifiers.FinalDamage /= 6;
        target.AddBuff(BuffID.OnFire3, 60);
    }

    public override void Kill(int timeLeft)
    {
        foreach (Dust d in dusts)
        {
            d.scale = 0;
            d.active = false;
        } 
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        int size = 10;
        hitbox.X -= size / 2;
        hitbox.Y -= size / 2;
        hitbox.Width += size;
        hitbox.Height += size;
    }
}

