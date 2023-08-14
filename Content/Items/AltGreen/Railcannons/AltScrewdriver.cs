using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terrakill.Content.Items.Blue.Shotguns;

namespace Terrakill.Content.Items.AltGreen.Railcannons;

public class AltScrewdriver : ModProjectile
{
    public Vector2 offset;
    public NPC attached = null;

    float attachedRot = 0;
    float thisRot = 0;

    public bool grounded = false;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 3;
        Projectile.scale = 2;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Ranged;

        Projectile.timeLeft = 450;

        Projectile.ArmorPenetration = 999;

        Projectile.extraUpdates = 22;

        Projectile.penetrate = 999;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 3;
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

    int timer = 0;
    Vector2 oldVel;
    public override void AI()
    {
        if (attached == null && !grounded) Projectile.rotation = Projectile.velocity.ToRotation();
        else
        {
            if (attached != null)
            {
                attached.GetGlobalNPC<BashManager>().harpooned = true;
                if (timer % 10 == 0) Main.player[Projectile.owner].GetModPlayer<RailcannonCharge>().charge--;
                if (Main.player[Projectile.owner].GetModPlayer<RailcannonCharge>().charge <= 0 || Main.player[Projectile.owner].controlJump) Projectile.Kill();
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = (attached.Center + offset).RotatedBy(attached.rotation - attachedRot, attached.Center) + Main.rand.NextVector2Circular(4, 4);
                Projectile.rotation = thisRot + attached.rotation;
                if (timer++ % 30 == 0) for (int i = 0; i < 3; i++) { SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item22 : SoundID.Item23, Projectile.Center); }

                float aKBScore = attached.knockBackResist;
                if (attached.boss) aKBScore = 0;

                if (Main.player[Projectile.owner].controlDown)
                {
                    playerDist -= 1f / 20f;
                    enemyDist -= 1f / 20f;
                }
                if (Main.player[Projectile.owner].controlUp)
                {
                    playerDist += 1f / 20f;
                    enemyDist += 1f / 20f;
                }

                Vector2 normalised = Projectile.position + (Projectile.position.DirectionTo(Main.MouseWorld) * playerDist);
                Main.player[Projectile.owner].velocity = Vector2.Lerp(Main.player[Projectile.owner].velocity, Main.player[Projectile.owner].DirectionTo(normalised) * 16, 0.11f * (1 - aKBScore));

                Projectile.rotation = Main.player[Projectile.owner].AngleTo(Projectile.position);
                Vector2 normalisedEnemy = Main.player[Projectile.owner].position + (Main.player[Projectile.owner].position.DirectionTo(Main.MouseWorld) * enemyDist);
                attached.velocity = Vector2.Lerp(attached.velocity, attached.DirectionTo(normalisedEnemy) * 24, 0.11f * aKBScore);

                if (MathF.Abs(attached.velocity.Length() - oldVel.Length()) > 2)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), attached.Center, Vector2.Zero,
                        ModContent.ProjectileType<AltScrewDamage>(), (int)MathF.Round(6 * attached.velocity.Length()), 0, Projectile.owner);
                    oldVel = attached.velocity;
                }
            }
        }

        if (attached != null && (!attached.active || attached.life <= 0)) Projectile.Kill();

        Projectile.ai[0]++;
    }

    float playerDist;
    float enemyDist;
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (attached == null)
        {
            playerDist = Projectile.Center.Distance(Main.player[Projectile.owner].Center);
            enemyDist = target.Center.Distance(Main.player[Projectile.owner].Center);
            offset = target.Center.DirectionTo(Projectile.Center) * target.Center.Distance(Projectile.Center);
            attached = target;
            attachedRot = target.rotation;
            thisRot = Projectile.rotation;
            Projectile.velocity = Vector2.Zero;
            Projectile.friendly = false;
            oldVel = attached.velocity;
            
        }
        Projectile.extraUpdates = 0;
    }

    public override void Kill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(3, 3), 1, 1, DustID.Torch, Scale: 2f).noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        return true;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        if (attached != null)
        {
            int size = 20;
            hitbox.X -= size / 2;
            hitbox.Y -= size / 2;
            hitbox.Width += size;
            hitbox.Height += size;
        }
    }
}

