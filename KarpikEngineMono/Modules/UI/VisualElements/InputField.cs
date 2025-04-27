using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KarpikEngineMono.Modules.VisualElements;

public class InputField : VisualElement
{
    public string Text { get; private set; } = "";
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public Color BackgroundColor { get; set; } = Color.White;
    public Color FocusedBackgroundColor { get; set; } = Color.LightYellow;
    public Color CaretColor { get; set; } = Color.Black;
    public int MaxLength { get; set; } = 128; // Ограничение длины
    
    public event Action<string> TextChanged;
    public event Action EnterPressed;
    
    public bool IsFocused { get; private set; } = false;

    private StringBuilder _textBuilder;
    private int _caretPosition = 0; // Позиция курсора
    private double _caretTimer = 0f; // Таймер для мигания
    private const float CaretBlinkRate = 0.5f; // Секунд на мигание
    
    // Статическое поле для отслеживания фокуса (только одно поле может быть в фокусе)
    private static InputField _currentlyFocusedField = null;
    
    public InputField(Rectangle bounds, SpriteFont font, GameWindow window) : base(bounds)
    {
        Font = font;
        _textBuilder = new StringBuilder();

        // --- Подписка на событие ввода текста ---
        window.TextInput += HandleTextInput;
    }
    
    // Важно отписаться при удалении объекта, чтобы избежать утечек!
    public void Unsubscribe(GameWindow window)
    {
         window.TextInput -= HandleTextInput;
    }

    private void HandleTextInput(object sender, TextInputEventArgs args)
    {
        if (!IsFocused || !IsEnabled) return; // Обрабатываем только если в фокусе

        char character = args.Character;

        // Проверяем, можно ли добавить символ
        if (Font.Characters.Contains(character) && _textBuilder.Length < MaxLength)
        {
            // Вставляем символ в позицию курсора
             _textBuilder.Insert(_caretPosition, character);
             _caretPosition++;
             OnTextChanged();
        }
    }


    protected override void HandleInput()
    {
        if (!IsEnabled)
        {
            if (IsFocused)
            {
                LoseFocus(); // Теряем фокус, если отключили
            }
            return;
        }

        if (Input.IsMouseLeftButtonDown) // Проверяем клик
        {
            if (IsHovered)
            {
                GainFocus();
                // TODO: Установить позицию курсора по клику мыши (требует расчета по символам)
                _caretPosition = _textBuilder.Length; // Пока ставим в конец
            }
            else if (IsFocused)
            {
                LoseFocus(); // Кликнули вне поля - теряем фокус
            }
        }

        if (IsFocused)
        {
            // Обработка управляющих клавиш
            if (Input.IsPressed(Keys.Back) && _caretPosition > 0)
            {
                _textBuilder.Remove(_caretPosition - 1, 1);
                _caretPosition--;
                OnTextChanged();
            }
            if (Input.IsPressed(Keys.Delete) && _caretPosition < _textBuilder.Length)
            {
                 _textBuilder.Remove(_caretPosition, 1);
                 OnTextChanged();
                 // Позиция курсора не меняется
            }
            if (Input.IsPressed(Keys.Left) && _caretPosition > 0)
            {
                _caretPosition--;
                ResetCaretBlink();
            }
            if (Input.IsPressed(Keys.Right) && _caretPosition < _textBuilder.Length)
            {
                _caretPosition++;
                ResetCaretBlink();
            }
            if (Input.IsPressed(Keys.Home))
            {
                 _caretPosition = 0;
                 ResetCaretBlink();
            }
            if (Input.IsPressed(Keys.End))
            {
                 _caretPosition = _textBuilder.Length;
                 ResetCaretBlink();
            }
            if (Input.IsPressed(Keys.Enter))
            {
                 EnterPressed?.Invoke();
                 Invalidate();
                 // LoseFocus(); // Опционально: терять фокус по Enter
            }
             // TODO: Выделение текста, копирование, вставка
        }
    }

    private void GainFocus()
    {
        if (IsFocused) return;
        // Убираем фокус с предыдущего поля, если оно было
        _currentlyFocusedField?.LoseFocus();

        IsFocused = true;
        _currentlyFocusedField = this;
        ResetCaretBlink();
        Invalidate();
    }

    private void LoseFocus()
    {
        if (!IsFocused) return;
        IsFocused = false;
        if (_currentlyFocusedField == this)
        {
            _currentlyFocusedField = null;
        }
        Invalidate();
    }

    private void OnTextChanged()
    {
        Text = _textBuilder.ToString(); // Обновляем публичное свойство
        TextChanged?.Invoke(Text);
        ResetCaretBlink();
        Invalidate();
    }

    private void ResetCaretBlink()
    {
         _caretTimer = 0f; // Сбрасываем таймер, чтобы курсор сразу стал видимым
         Invalidate();
    }

    protected override void OnUpdate(double deltaTime)
    {
        // Обновляем таймер курсора
        if (IsFocused)
        {
            _caretTimer += deltaTime;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var uiBounds = UIBounds;
        Color currentBgColor = IsFocused ? FocusedBackgroundColor : BackgroundColor;
        Texture2D pixel = Button.GetPixelTexture(spriteBatch.GraphicsDevice); // Используем тот же пиксель
        spriteBatch.Draw(pixel, uiBounds, currentBgColor);

        // Рисуем текст
        if (Font == null) return;
        
        // Простая отрисовка без скроллинга/клиппинга
        // TODO: Добавить клиппинг текста, если он выходит за Bounds
        var scale = (float)90 / Font.LineSpacing;
        Vector2 textPosition = new Vector2(uiBounds.X + 5, uiBounds.Y + (uiBounds.Height - Font.MeasureString(" ").Y * scale) / 2); // Небольшой отступ слева
        spriteBatch.DrawString(Font, _textBuilder.ToString(), textPosition, TextColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

        // Рисуем курсор (мигающий)
        if (IsFocused && (_caretTimer % (CaretBlinkRate * 2)) < CaretBlinkRate)
        {
            // Вычисляем позицию курсора
            string textBeforeCaret = _textBuilder.ToString(0, _caretPosition);
            Vector2 caretOffset = Font.MeasureString(textBeforeCaret) * scale;
            int caretX = (int)(textPosition.X + caretOffset.X);
            Rectangle caretRect = new Rectangle(caretX, uiBounds.Y + 4, 1, uiBounds.Height - 8); // Тонкий курсор
            spriteBatch.Draw(pixel, caretRect, CaretColor);
        }
    }
}