using Terraria.ModLoader;

namespace Terrakill;
public class Keybinds : ModSystem
{
    public static ModKeybind DashKeybind { get; private set; }
    public static ModKeybind SlideKeybind { get; private set; }
    public static ModKeybind AltFire { get; private set; }
    public static ModKeybind Punch { get; private set; }
    public static ModKeybind ChangeHand { get; private set; }
    public static ModKeybind Whiplash { get; private set; }

    public override void Load()
    {
        // Registers a new keybind
        // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to english users is in en-US.hjson
        DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", "LeftShift");
        SlideKeybind = KeybindLoader.RegisterKeybind(Mod, "Slide", "Z");
        AltFire = KeybindLoader.RegisterKeybind(Mod, "Alternate Fire", "Mouse2");
        Punch = KeybindLoader.RegisterKeybind(Mod, "Punch", "F");
        ChangeHand = KeybindLoader.RegisterKeybind(Mod, "Change Hand", "G");
        Whiplash = KeybindLoader.RegisterKeybind(Mod, "Whiplash", "V");
    }

    // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
    public override void Unload()
    {
        // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
        DashKeybind = null;
        SlideKeybind = null;
        AltFire = null;
        Punch = null;
        ChangeHand = null;
        Whiplash = null;
    }
}