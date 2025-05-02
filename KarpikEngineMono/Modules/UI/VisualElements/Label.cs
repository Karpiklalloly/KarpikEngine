using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class Label : VisualElement
{
    public string Text { get; private set; }
    public SpriteFont Font { get; set; } = UI.DefaultFont;
    public Color TextColor { get; set; } = Color.White;
    public Vector2 TextAlignment { get; set; } = new Vector2(0.0f, 0.5f); // Выравнивание текста внутри Bounds 
    public float FontSize { get; set; } = 64f;
    
    public Label(string text) : base(Vector2.Zero)
    {
        SetText(text);
    }
    
    // Обновляем размер, если текст изменился
    public void SetText(string text)
    {
        if (Text == text) return;
        
        Text = text;
        if (Font == null || string.IsNullOrEmpty(Text)) return;
        
        Size = Font.MeasureString(Text);
        UpdateLayout();
    }
    
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Font == null || string.IsNullOrEmpty(Text)) return;
        
        var scale = FontSize / Font.LineSpacing;
        var textSize = Font.MeasureString(Text) * scale;
        var textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) * TextAlignment.X,
            Bounds.Y + (Bounds.Height - textSize.Y) * TextAlignment.Y
        );
        spriteBatch.DrawString(Font, Text, textPos, TextColor);
    }
}