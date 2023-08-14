using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using Terrakill.Content.Health;
using static System.Net.Mime.MediaTypeNames;

namespace Terrakill.Content;

public class StyleBonus
{
    public enum StyleLevel
    {
        Gray,
        White,
        Red,
        Orange,
        Green,
        Blue,
        Custom
    };

    public string name;
    public int style;
    public StyleLevel styleLevel;
    public readonly Color styleColor;

    Color GetStyleColor(StyleLevel level)
    {
        if (styleLevel == StyleLevel.Gray) return new Color(192, 192, 192);
        if (styleLevel == StyleLevel.White) return new Color(255, 255, 255);
        if (styleLevel == StyleLevel.Red) return new Color(255, 0, 0);
        if (styleLevel == StyleLevel.Orange) return new Color(255, 140, 0);
        if (styleLevel == StyleLevel.Green) return new Color(0, 255, 0);
        if (styleLevel == StyleLevel.Blue) return new Color(0, 255, 255);
        return new Color(255, 0, 255);
    }

    public StyleBonus(string n, int s, StyleLevel sl)
    {
        name = n;
        style = s;
        styleLevel = sl;
        styleColor = GetStyleColor(styleLevel);
    }

    public StyleBonus(string n, int s, Color c)
    {
        name = n;
        style = s;
        styleLevel = StyleLevel.Custom;
        styleColor = c;
    }
}

public class Ultrastyle : UIState
{
    // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
    // Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
    private UIElement area;
    private UIImage barFrame;
    private UIText stylePlaceholder;
    private UIText[] texts = new UIText[5];
    private Color color;

    public static List<StyleBonus> styleEvents;
    public static float style;

    public static void PushNewStyleBonus(StyleBonus bonus)
    {
        styleEvents.Insert(0, bonus);
        style += bonus.style;
        /*while (styleEvents.Count > 5)
        {
            styleEvents.RemoveAt(5);
        }*/
    }

    public override void OnInitialize()
    {
        StyleBonus[] starting = new StyleBonus[]
        {
            new StyleBonus("", 0, StyleBonus.StyleLevel.White),
            new StyleBonus("", 0, StyleBonus.StyleLevel.Red),
            new StyleBonus("", 0, StyleBonus.StyleLevel.Orange),
            new StyleBonus("", 0, StyleBonus.StyleLevel.Green),
            new StyleBonus("", 0, new Color(255, 255, 0)),
        };
        styleEvents = new List<StyleBonus>(starting);
        // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
        // UIElement is invisible and has no padding.
        area = new UIElement();
        area.Left.Set(-850, 1f); // Place the resource bar to the left of the hearts.
        area.Top.Set(17, 0f); // Placing it just a bit below the top of the screen.
        area.Width.Set(241, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
        area.Height.Set(60, 0f);

        barFrame = new UIImage(ModContent.Request<Texture2D>("Terrakill/Content/Movement/StaminaBarFrame")); // Frame of our resource bar
        barFrame.Left.Set(22, 0f);
        barFrame.Top.Set(35, 0f);
        barFrame.Width.Set(56, 0f);
        barFrame.Height.Set(12, 0f);
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement == ServerConfigurations.MovementType.None)
        {
            barFrame.Deactivate();
        }

        float height = 60;

        stylePlaceholder = new UIText("style_value", 1.25f);
        stylePlaceholder.Width.Set(138, 0f);
        stylePlaceholder.Height.Set(34, 0f);
        stylePlaceholder.Top.Set(height - 20, 0f);
        stylePlaceholder.Left.Set(-10, 0f);

        area.Append(stylePlaceholder);

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i] = new UIText("style_event_" + i, 1.25f);
            texts[i].Width.Set(138, 0f);
            texts[i].Height.Set(34, 0f);
            texts[i].Top.Set(60 + (20 * i), 0f);
            texts[i].Left.Set(-10, 0f);

            area.Append(texts[i]);
        }
        /*area.Append(barFrame); REMOVE ME LATER DUMBASS*/
        Append(area);
    }

    // Here we draw our UI
    float quotient = 0f;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        return; // REMOVE ME IF ITS NOT RENDERING BAR.

        // HEY. FUTURE ME. THERE'S A RETURN UP HERE.

        if (ModContent.GetInstance<ServerConfigurations>().ultramovement != ServerConfigurations.MovementType.None)
        {
            base.DrawSelf(spriteBatch);
            var modPlayer = Main.LocalPlayer.GetModPlayer<Movement.StaminaPlayer>();
            // Calculate quotient
            quotient = (float)MathHelper.Lerp(quotient, modPlayer.statStamina / modPlayer.statStaminaMax, 0.1f);
            quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i++)
            {
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), color);
            }
        }
    }

    int timer = 0;
    public override void Update(GameTime gameTime)
    {
        stylePlaceholder.SetText("Style: " + style);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].SetText(styleEvents[i].name);
            texts[i].TextColor = styleEvents[i].styleColor;
        }
        base.Update(gameTime);
    }
}

class StyleUISystem : ModSystem
{
    private UserInterface StyleUI;

    internal Ultrastyle StyleBar;

    public override void Load()
    {
        // All code below runs only if we're not loading on a server
        if (!Main.dedServ)
        {
            StyleBar = new();
            StyleUI = new();
            StyleUI.SetState(StyleBar);
        }
    }

    int timer = 0;
    public override void UpdateUI(GameTime gameTime)
    {
        if (++timer % 4 == 0)
        {   
            if (Ultrastyle.style <= 0) Ultrastyle.style = 0;
            else Ultrastyle.style--;
            if (timer % 120 == 0)
            {
                Ultrastyle.PushNewStyleBonus(new StyleBonus("", 0, new Color(255, 0, 255)));
            }
        }
        StyleUI?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (resourceBarIndex != -1)
        {
            layers.Insert(resourceBarIndex + 1, new LegacyGameInterfaceLayer(
                "Terrakill: Style",
                delegate {
                    StyleUI.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
