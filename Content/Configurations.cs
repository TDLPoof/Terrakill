using System;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.Graphics.CameraModifiers;

namespace Terrakill.Content;

public class ClientConfigurations : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("Hitstop Duration")]
    [Tooltip("Controls the length of time that hitstops are applied.")]
    [DefaultValue(1f)]
    public float hitstopMultiplier;

    [Label("Screenshake Strength")]
    [Tooltip("Controls the strength of weapon screenshakes.")]
    [DefaultValue(1f)]
    public float screenshakeMultiplier;

    [Label("Vanilla Screenshakes")]
    [Tooltip("Allows vanilla weapons to use screenshake. Undoubtedly buggy.")]
    [DefaultValue(false)]
    public bool vanillaScreenshakes;

    [Label("Vanilla Screenshake Strength")]
    [Tooltip("Controls the strength of weapon screenshakes for vanilla weapons.")]
    [DefaultValue(1f)]
    public float vanillaScreenshakeMultiplier;
}

public class ServerConfigurations : ModConfig
{
    public enum MovementType
    {
        None,
        Dashing_Only,
        Sliding_Only,
        All
    };

    public enum PDBType
    {
        None,
        Modded_Only,
        All
    };

    public enum FireType
    {
        None,
        Player_Only,
        Enemies_Only,
        All
    };

    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Label("Revolver Progression Change")]
    [Tooltip("If enabled, revolvers will sometimes drop from any boss.\n" +
        "If not, they will always drop from King Slime.")]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool randomRevolvers;

    [Label("Health Mechanics")]
    [Tooltip("If enabled, the health system will behave similarly to ULTRAKILL's.\n" +
        "If not, it will be unchanged.")]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ultrahealth;

    [Label("Movement Mechanics")]
    [Tooltip("If enabled, the movement system will behave similarly to ULTRAKILL's.\n" +
        "If not, it will be unchanged.")]
    [DefaultValue(MovementType.All)]
    [ReloadRequired]
    public MovementType ultramovement;

    [Label("Damage Mechanics")]
    [Tooltip("Changes which projectiles are affected by the mod's damage boost system.")]
    [DefaultValue(PDBType.Modded_Only)]
    [ReloadRequired]
    public PDBType ultraPDB;

    [Label("Prehardmode Boss Buff")]
    [Tooltip("If enabled, Pre-Hardmode bosses will receive extra damage reduction.")]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ultraBossBuff;

    [Label("Fire Mechanics")]
    [Tooltip("Changes how fire affects the player and enemies.")]
    [DefaultValue(FireType.All)]
    [ReloadRequired]
    public FireType ultrafire;

    [Label("Developer Key")]
    [Tooltip("Allows for a variety of silly effects.")]
    [DefaultValue("")]
    [ReloadRequired]
    public string developerKey;
}