using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.ZeGold.Revolvers;

public class SignalTrap : ModProjectile
{
    public Vector2 offset;
    public NPC attached = null;

    float attachedRot = 0;
    float thisRot = 0;

    public bool grounded = false;
    float trapRot = 0;
    float coeff = 0;

    bool slabbed = false;

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 12000;

        Projectile.ArmorPenetration = 999;

        Projectile.extraUpdates = 19;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;

        trapRot = Main.rand.NextFloat(MathF.PI * 2);
        coeff = Main.rand.NextFloat(0.5f, 1f) * (Main.rand.NextBool() ? 1 : -1);
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0 && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<AltZeGold.Revolvers.SlabSignal>())
        {
            slabbed = true;
        }

        if (!Main.dedServ && Main.rand.NextBool(5))
        {
            Vector2 rand = Vector2.Zero;
            if (attached != null)
            {
                if (slabbed)
                {
                    if (Main.rand.NextBool(5)) rand = new Vector2(20, 0).RotatedBy(trapRot);
                    else if (Main.rand.NextBool(4)) rand = new Vector2(20, 0).RotatedBy(trapRot + (MathF.PI / 2));
                    else if (Main.rand.NextBool(3)) rand = new Vector2(20, 0).RotatedBy(trapRot + MathF.PI);
                    else if (Main.rand.NextBool()) rand = new Vector2(20, 0).RotatedBy(trapRot + (3 * MathF.PI / 2));
                }
                else
                {
                    if (Main.rand.NextBool(3)) rand = new Vector2(10, 0).RotatedBy(trapRot);
                    else if (Main.rand.NextBool()) rand = new Vector2(10, 0).RotatedBy(trapRot + MathF.PI);
                }
            }
            Dust d = Dust.NewDustDirect(Projectile.Center + rand, 1, 1, DustID.GoldFlame, Scale: 2f);
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }

        trapRot += (MathHelper.ToRadians(7 * coeff) / 20);
        if (trapRot > MathF.PI * 2) trapRot -= MathF.PI * 2;

        if (attached != null)
        {
            Projectile.Center = (attached.Center + offset).RotatedBy(attached.rotation - attachedRot, attached.Center);
            Projectile.rotation = thisRot + attached.rotation;
            attached.GetGlobalNPC<TrapManager>().trap = Projectile;
            if (slabbed) attached.GetGlobalNPC<TrapManager>().slabTrapped = true;
        }

        if (attached != null && (!attached.active || attached.life <= 0)) Projectile.Kill();

        Projectile.ai[0]++;
    }

    public override bool PreKill(int timeLeft)
    {
        return base.PreKill(timeLeft);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Vector2.Zero;
        return false;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return target.GetGlobalNPC<TrapManager>().trap == null;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetMaxDamage(1);
        offset = target.Center.DirectionTo(Projectile.Center) * target.Center.Distance(Projectile.Center);
        attached = target;
        attachedRot = target.rotation;
        thisRot = Projectile.rotation;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
}

