using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using System.Collections.Generic;
using Terrakill.Content.Health;
using static System.Net.Mime.MediaTypeNames;

namespace Terrakill.Content.Movement;

internal class StaminaBar : UIState
{
    // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
    // Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
    private UIElement area;
    private UIImage barFrame;
    private UIText text;
    private Color color;

    public override void OnInitialize()
    {
        // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
        // UIElement is invisible and has no padding.
        area = new UIElement();
        area.Left.Set(/*-area.Width.Pixels */- 322, 1f); // Place the resource bar to the left of the hearts.
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

        text = new UIText("0/0", 0.8f); // text to show stat
        text.TextColor = new Color(192, 192, 192);
        text.Width.Set(138, 0f);
        text.Height.Set(34, 0f);
        text.Top.Set(20, 0f);
        text.Left.Set(0, 0f); 

        color = new Color(74, 204, 199);

        area.Append(text);
        area.Append(barFrame);
        Append(area);
    }

    // Here we draw our UI
    float quotient = 0f;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (ModContent.GetInstance<ServerConfigurations>().ultramovement != ServerConfigurations.MovementType.None)
        {
            base.DrawSelf(spriteBatch);
            var modPlayer = Main.LocalPlayer.GetModPlayer<StaminaPlayer>();
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

    public override void Update(GameTime gameTime)
    {
        Ultrahealth healthPlayer = Main.LocalPlayer.GetModPlayer<Ultrahealth>();
        if (ModContent.GetInstance<ServerConfigurations>().ultrahealth)
        {
            text.SetText($"{healthPlayer.hardDamage} / {Main.LocalPlayer.statLifeMax2}");
        }
        else
        {
            text.SetText("");
        }
        base.Update(gameTime);
    }
}

class StaminaUISystem : ModSystem
{
    private UserInterface StaminaUI;

    internal StaminaBar StaminaBar;

    public override void Load()
    {
        // All code below runs only if we're not loading on a server
        if (!Main.dedServ)
        {
            StaminaBar = new();
            StaminaUI = new();
            StaminaUI.SetState(StaminaBar);
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        StaminaUI?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (resourceBarIndex != -1)
        {
            layers.Insert(resourceBarIndex + 1, new LegacyGameInterfaceLayer(
                "Terrakill: Stamina",
                delegate {
                    StaminaUI.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
