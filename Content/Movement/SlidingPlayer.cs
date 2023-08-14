using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Terrakill.Content.Movement;

public class SlidingPlayer : ModPlayer
{
    public int onGround = 0;
    bool shockwaved = false;
    int slideDir = 1;
    float velocity = 12;

    int timer = 0;

    bool startedInAir = false;
    bool jumpedThisSlide = false;

    Vector2 pos;

    float targetRotation;

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (slideDir == 1 && Keybinds.SlideKeybind.Current && !startedInAir)
        {
            drawInfo.Position.Y += 40;
        }
        if (slideDir == -1 && Keybinds.SlideKeybind.Current && !startedInAir)
        {
            drawInfo.Position.Y += 25;
            drawInfo.Position.X += 10;
        }
    }

    public override void PreUpdateMovement()
    {
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.Sliding_Only ||
            ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.All)
        {
            Player.gills = true;
            Player.UpdateTouchingTiles();
            if (Keybinds.SlideKeybind.JustPressed)
            {
                startedInAir = Player.TouchedTiles.Count <= 0;
                jumpedThisSlide = false;
                shockwaved = false;
            }
            if (Keybinds.SlideKeybind.Current)
            {
                if (startedInAir && !shockwaved) // slam
                {
                    Player.velocity.X = 0f;
                    Player.velocity.Y = 16f;

                    if (timer % 5 == 0)
                    {
                        float ring = Main.rand.NextFloat(10f, 16f);
                        pos = Main.rand.NextVector2CircularEdge(ring, ring);
                    }

                    Dust d = Dust.NewDustDirect(Player.Center + pos, 1, 8, DustID.GoldFlame);
                    d.noGravity = true;
                    d.velocity.Y = 4f;
                    if (Player.TouchedTiles.Count > 0)
                    {
                        if (!shockwaved) Shockwave(Player, 20);
                        shockwaved = true;
                    }
                }
                else if (!shockwaved)// slide
                {
                    if (Keybinds.SlideKeybind.JustPressed)
                    {
                        slideDir = (int)(Player.velocity.X / MathF.Abs(Player.velocity.X));
                        velocity = 12;
                    }
                    if (Player.TouchedTiles.Count > 0 && !jumpedThisSlide && Player.controlJump)
                    {
                        jumpedThisSlide = true;
                        velocity *= 1.5f;
                    }
                    targetRotation = MathF.PI / -2 * slideDir;
                    Player.direction = slideDir;
                    Player.velocity.X = velocity * slideDir;
                    velocity *= 0.99f;
                    if (!Main.dedServ)
                    {
                        Dust.NewDustDirect(Player.BottomLeft, Player.width, 1,
                                        DustID.SilverFlame,
                                        0f, 0.5f,
                                        Scale: 2f).noGravity = true;
                        Dust.NewDustDirect(Player.BottomLeft - new Vector2(0, 4), Player.width, 1,
                                        DustID.GoldFlame,
                                        0f, 0.5f).noGravity = true;
                    }

                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, targetRotation, 0.9f);
                    Player.headRotation = MathHelper.Lerp(Player.headRotation, -targetRotation * 0.9f, 0.6f);
                }
            }
        }
        timer++;
    }

    public void Shockwave(Player player, float force)
    {
        if (!Main.dedServ)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(player.BottomLeft, player.width, 1,
                                   DustID.SilverFlame,
                                   Main.rand.NextFloat(-10f, 10f), 0.5f,
                                   Scale: 3f).noGravity = true;
            }
        }
        foreach (NPC npc in Main.npc)
        {
            if (npc.type == NPCID.TargetDummy) continue;
            if (npc.Distance(player.Bottom) > 600) continue;
            float amount = 100f * force / npc.Distance(player.Bottom);
            npc.velocity += player.Bottom.DirectionTo(npc.Center) * amount * npc.knockBackResist;
        }
    }

    public override void ResetEffects()
    {
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.Sliding_Only ||
            ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.All)
        {
            if (!Keybinds.SlideKeybind.Current)
            {
                Player.fullRotation = MathHelper.Lerp(Player.fullRotation, 0, 0.8f);
                Player.headRotation = MathHelper.Lerp(Player.headRotation, 0, 0.5f);
            }
        }
    }
}

