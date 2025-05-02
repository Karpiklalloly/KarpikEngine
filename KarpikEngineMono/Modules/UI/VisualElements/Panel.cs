using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class Panel : VisualElement
{
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public Texture2D BackgroundTexture { get; set; }
    
    public Panel(Vector2 size) : base(size) { }
    public Panel(Vector2 size, Color backgroundColor) : base(size)
    {
        BackgroundColor = backgroundColor;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (BackgroundColor.A > 0)
        {
            Texture2D pixel = Button.GetPixelTexture(spriteBatch.GraphicsDevice);
            spriteBatch.Draw(pixel, Bounds, BackgroundColor);
        }
        if (BackgroundTexture != null)
        {
            spriteBatch.Draw(BackgroundTexture, Bounds, Color.White);
        }
    }
}