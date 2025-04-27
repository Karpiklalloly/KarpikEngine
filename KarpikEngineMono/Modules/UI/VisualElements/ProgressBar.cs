using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class ProgressBar : VisualElement
{
    private float _value;
    private float _maxValue = 100f;

    // --- Свойства для управления прогрессом ---
    public float MaxValue
    {
        get => _maxValue;
        set
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "MaxValue must be positive.");
            _maxValue = value;
            // Пересчитываем значение, чтобы оно осталось в пределах нового максимума
            Value = _value; // Используем сеттер Value для проверки
        }
    }

    public float Value
    {
        get => _value;
        set
        {
            // Ограничиваем значение диапазоном [0, MaxValue]
            _value = Math.Clamp(value, 0f, _maxValue);
        }
    }

    /// <summary>
    /// Прогресс в диапазоне от 0.0 до 1.0
    /// </summary>
    public float NormalizedValue => (_maxValue > 0) ? (_value / _maxValue) : 0f;

    // --- Свойства для внешнего вида ---
    public Color BackgroundColor { get; set; } = Color.Gray;
    public Color ForegroundColor { get; set; } = Color.Green; // Цвет заполнения
    public Texture2D BackgroundTexture { get; set; } // Опциональная текстура фона
    public Texture2D ForegroundTexture { get; set; } // Опциональная текстура заполнения

    // --- Свойства для текста (Опционально) ---
    public bool ShowText { get; set; } = true;
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; } = Color.White;
    public string TextFormat { get; set; } = "{0:0}%"; // Формат для string.Format (0 - значение 0-100)

    // --- Конструктор ---
    public ProgressBar(Rectangle bounds) : base(bounds)
    {
        // ProgressBar обычно не интерактивен
        IsEnabled = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Texture2D pixel = Button.GetPixelTexture(spriteBatch.GraphicsDevice); // Берем пиксель из Button

        // 1. Рисуем фон
        if (BackgroundTexture != null)
        {
            spriteBatch.Draw(BackgroundTexture, Bounds, Color.White); // Текстура как есть
        }
        else
        {
            spriteBatch.Draw(pixel, Bounds, BackgroundColor); // Заливка цветом
        }

        // 2. Рисуем заполнение (Foreground)
        if (_value > 0) // Рисуем только если есть прогресс
        {
            // Рассчитываем ширину заполненной части
            int foregroundWidth = (int)(Bounds.Width * NormalizedValue);

            if (foregroundWidth > 0)
            {
                Rectangle foregroundRect = new Rectangle(Bounds.X, Bounds.Y, foregroundWidth, Bounds.Height);

                if (ForegroundTexture != null)
                {
                    // Рисуем часть текстуры заполнения
                    // Рассчитываем SourceRectangle для тайлинга или обрезки текстуры
                    Rectangle sourceRect = new Rectangle(0, 0,
                        (int)(ForegroundTexture.Width * NormalizedValue), // Берем часть ширины текстуры
                        ForegroundTexture.Height);
                    spriteBatch.Draw(ForegroundTexture, foregroundRect, sourceRect, Color.White);
                    // Примечание: Этот способ растянет/сожмет текстуру. Для тайлинга нужен другой подход.
                }
                else
                {
                    // Заливка цветом
                    spriteBatch.Draw(pixel, foregroundRect, ForegroundColor);
                }
            }
        }

        // 3. Рисуем текст (Опционально)
        if (ShowText && Font != null)
        {
            // Формируем текст
            string textToShow = string.Format(TextFormat, NormalizedValue * 100f);

            // Центрируем текст
            Vector2 textSize = Font.MeasureString(textToShow);
            Vector2 textPosition = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            // Рисуем текст (можно добавить тень для читаемости)
            // Простой вариант:
             spriteBatch.DrawString(Font, textToShow, textPosition, TextColor);

            // Вариант с простой тенью:
            // Vector2 shadowOffset = new Vector2(1, 1);
            // spriteBatch.DrawString(Font, textToShow, textPosition + shadowOffset, Color.Black * 0.5f); // Полупрозрачная черная тень
            // spriteBatch.DrawString(Font, textToShow, textPosition, TextColor); // Основной текст
        }
    }
}