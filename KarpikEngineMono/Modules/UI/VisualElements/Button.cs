using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class Button : VisualElement
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = Color.DarkGray;
    public Color HoverColor { get; set; } = Color.Gray;
    public Color PressedColor { get; set; } = Color.LightGray;
    public Color DisabledColor { get; set; } = Color.DarkGray;
    public Texture2D BackgroundTexture { get; set; }

    public event Action Clicked;

    private bool _isPressed;
    
    public Button(Vector2 size, string text, SpriteFont font) : base(size)
    {
        Text = text;
        Font = font;
    }

    protected override void HandleInput()
    {
        if (!IsEnabled)
        {
            _isPressed = false;
            Invalidate();
        }

        if (!IsHovered)
        {
            _isPressed = false;
            Invalidate();
            return;
        }

        if (Input.IsMouseLeftButtonDown)
        {
            Clicked?.Invoke();
            _isPressed = true;
            Invalidate();
        }
        else if (Input.IsMouseLeftButtonHold)
        {
            _isPressed = true;
            Invalidate();
        }
        else
        {
            _isPressed = false;
            Invalidate();
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Color currentBgColor = BackgroundColor;
        if (!IsEnabled)
        {
            currentBgColor = DisabledColor;
        }
        else if (_isPressed)
        {
            currentBgColor = PressedColor;
        }
        else if (IsHovered)
        {
            currentBgColor = HoverColor;
        }

        var uiBounds = Bounds;
        // Рисуем фон (текстурой или цветом)
        if (BackgroundTexture != null)
        {
            spriteBatch.Draw(BackgroundTexture, uiBounds, currentBgColor); // Можно использовать Color.White, если текстура цветная
        }
        else
        {
            // Простой хак для рисования прямоугольника цветом (нужна текстура 1x1 пикселя белого цвета)
            Texture2D pixel = GetPixelTexture(spriteBatch.GraphicsDevice); // Получаем пиксель (нужна реализация)
            spriteBatch.Draw(pixel, uiBounds, currentBgColor);
        }
        
        // Рисуем текст
        if (Font != null && !string.IsNullOrEmpty(Text))
        {
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 textPosition = new Vector2(
                uiBounds.X + (uiBounds.Width - textSize.X) / 2,
                uiBounds.Y + (uiBounds.Height - textSize.Y) / 2
            );
            spriteBatch.DrawString(Font, Text, textPosition, TextColor);
        }
    }
    
    // --- Вспомогательное поле и метод для получения текстуры 1x1 ---
    public static Texture2D _pixelTexture;
    public static Texture2D GetPixelTexture(GraphicsDevice graphicsDevice)
    {
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        return _pixelTexture;
    }
}