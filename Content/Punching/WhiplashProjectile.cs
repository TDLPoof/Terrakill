using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Terrakill.Content.Punching;

public class WhiplashProjectile : ModProjectile
{
    SoundStyle HookPullStart = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookPullStart")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };
    SoundStyle HookPullLoop = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookPullLoop")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };
    SoundStyle HookLoop = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookLoop")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };
    SoundStyle HookHit = new SoundStyle($"{nameof(Terrakill)}/Sounds/Melee/HookHit")
    {
        PitchVariance = 0.1f,
        Volume = 5f,
        MaxInstances = 3
    };

    bool returning = false;
    public NPC attached;
    public Projectile attachedProjectile;

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 9;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Melee;

        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;

        Projectile.penetrate = 999;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        List<Vector2> list = new List<Vector2>();
        list.Add(Projectile.Center + (Projectile.Center - Projectile.position));
        list.Add(player.Center);
        Texture2D texture = TextureAssets.FishingLine.Value;
        Rectangle frame = texture.Frame();
        Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

        Vector2 pos = Vector2.Lerp(list[0], list[1], 0.5f);

        Vector2 element = list[0];
        Vector2 diff = list[1] - element;

        float rotation = diff.ToRotation() - MathHelper.PiOver2;
        Color color = lightColor.MultiplyRGBA(new Color(30, 20, 20));
        Vector2 scale = new Vector2(2, diff.Length() / 12);
        Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);
        return true;
    }

    int returningTimer = 1, normTimer = 0;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (player.dead) Projectile.Kill();

        if (!Keybinds.Whiplash.Current || Projectile.Distance(player.position) > 1000)
        {
            if (!returning) SoundEngine.PlaySound(HookPullStart, player.position);
            returning = true;
        }

        if (returning)
        {
            if (returningTimer++ % 25 == 0) SoundEngine.PlaySound(HookPullLoop, player.position);
            if (Projectile.Center.Distance(player.Center) < 30 || player.controlJump) Projectile.Kill();
            if (attached != null)
            {
                if (attached.boss || attached.knockBackResist < 0.2f || attached.type == NPCID.TargetDummy)
                {
                    player.velocity = player.Center.DirectionTo(attached.Center) * 15;
                    if (!player.wet)
                    {
                        if (player.GetModPlayer<Health.Ultrahealth>().hardDamage < 50)
                        {
                            player.GetModPlayer<Health.Ultrahealth>().hardDamage++;
                        }
                        player.GetModPlayer<Health.Ultrahealth>().hardDamageTime = 0;
                    }
                }
                else
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 20;
                    attached.velocity = attached.Center.DirectionTo(player.Center) * 20;
                }
            }
            else
            {
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 30;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI;
            }
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (normTimer++ % 29 == 0) SoundEngine.PlaySound(HookLoop, player.position);
        }

        if (attached != null)
        {
            if (attached.boss || attached.knockBackResist < 0.2f || attached.type == NPCID.TargetDummy)
            {
                Projectile.Center = attached.Center;
            }
            else
            {
                attached.Center = Projectile.Center;
            }
        }

        if (attachedProjectile == null)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (Projectile.friendly && Projectile.Hitbox.Intersects(p.Hitbox) && p.type == ModContent.ProjectileType<Items.Green.RocketLaunchers.SRSBall>())
                {
                    attachedProjectile = p;
                    Projectile.friendly = false;
                    break;
                }
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(HookHit, Main.player[Projectile.owner].position);
        SoundEngine.PlaySound(HookPullStart, Main.player[Projectile.owner].position);
        returning = true;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(HookHit, Main.player[Projectile.owner].position);
        SoundEngine.PlaySound(HookPullStart, Main.player[Projectile.owner].position);
        attached = target;
        Projectile.friendly = false;
        returning = true;
    }
}

