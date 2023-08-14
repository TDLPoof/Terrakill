using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Terrakill.Content.Items.AltRed.Sawlaunchers;

public class NerveSaw : ModProjectile
{
    bool nailbombed, hasBeenNailbombed;
    float rotation, deltaR;

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 200;
        Projectile.penetrate = 20;

        Projectile.ArmorPenetration = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;

        deltaR = Main.rand.NextFloat(8f, 16f);
        if (Main.rand.NextBool()) deltaR *= -1;
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Clentaminator_Red, Scale: 2f);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 2;
        }

        rotation += MathHelper.ToRadians(deltaR);

        Projectile.velocity.Y += 0.25f;

        foreach (Projectile p in Main.projectile)
        {
            if (p.type == ModContent.ProjectileType<Blue.Nailguns.Magnet>() && p.Distance(Projectile.position) < 400)
            {
                Vector2 target = p.Center + new Vector2(60, 0).RotatedBy(rotation);
                if (p.active) Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target) * 30, 0.12f);
                else
                {
                    if (!hasBeenNailbombed) nailbombed = true;
                    hasBeenNailbombed = true;
                }
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (nailbombed)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.velocity *= 1.5f;
        }

        foreach (Projectile p in Main.projectile)
        {
            if (p.active && p.type == ModContent.ProjectileType<Red.Nailguns.NerveMarker>() && p.Distance(Projectile.position) < 250d)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 positionOnLine = Vector2.Lerp(Projectile.Center, p.Center, Main.rand.NextFloat());
                    Dust d = Dust.NewDustDirect(positionOnLine, 1, 1, DustID.RedTorch, Scale: 0.8f);
                    d.noGravity = true;
                    d.velocity = Vector2.Zero;
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Hitbox.Contains(positionOnLine.ToPoint()))
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<Red.Nailguns.NerveSlabDamage>(), 15, 0, Projectile.owner);
                        }
                    }
                }
            }
        }

        nailbombed = false;
        Projectile.ai[0]++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        //PolaritiesPort/
        if (hasBeenNailbombed) modifiers.FinalDamage *= 2;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.penetrate-- < 0) Projectile.Kill();
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

        // If the projectile hits the left or right side of the tile, reverse the X velocity
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }

        return false;
    }
}

