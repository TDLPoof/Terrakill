using System;
using Terrakill.Content.Items.Blue.Shotguns;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terrakill.Content.Items.ZeGold.Revolvers;

public class TrapManager : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public Projectile trap = null;
    public bool slabTrapped = false;

    public override void ResetEffects(NPC npc)
    {
        trap = null;
        slabTrapped = false;
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (slabTrapped && projectile.type != ModContent.ProjectileType<HaveSomeDamage>()
                        && projectile.type != ModContent.ProjectileType<ForYouToo>()
                        && projectile.extraUpdates > 100)
            Explode(npc, 100, 75, projectile.owner, DustID.YellowTorch, DustID.GoldFlame);
    }

    public void Explode(NPC NPC, int size, int damage, int playerID, int dustID = DustID.Torch, int altDustID = -1)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

        if (!Main.dedServ)
        {
            for (int i = 0; i < size; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.Center + Main.rand.NextVector2Circular(size, size), 1, 1, dustID, Scale: 2f);
                d.noGravity = true;
            }
            if (altDustID != -1)
            {
                for (int i = 0; i < size; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.Center + Main.rand.NextVector2Circular(size, size), 1, 1, altDustID, Scale: 2f);
                    d.noGravity = true;
                }
            }
        }

        foreach (NPC npc in Main.npc)
        {
            if (npc.Distance(NPC.Center) > size) continue;
            float distFactor = 1.00f - (npc.Distance(NPC.Center) / size);
            if (npc.friendly)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<ForYouToo>(), 35, 0, playerID);
            }
            else
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<HaveSomeDamage>(), (int)MathF.Round(damage * distFactor), 0, playerID);
            }
        }

        foreach (Player player in Main.player)
        {
            if (player.Distance(NPC.Center) > size) continue;
            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<ForYouToo>(), 35, 0, playerID);
        }
    }
}

