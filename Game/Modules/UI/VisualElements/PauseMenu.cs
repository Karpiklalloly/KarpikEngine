using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public class PauseMenu : VisualElement
{
    private readonly Button _resumeButton;
    private readonly InputField _inputField;

    public PauseMenu(Vector2 size) : base(size)
    {
        _resumeButton = new Button(new Vector2(600, 200), "Resume", UI.DefaultFont);
        _resumeButton.Clicked += OnResumeClicked;
        _resumeButton.Anchor = Anchor.Center;
        
        _inputField = new InputField(new Vector2(600, 100), UI.DefaultFont, UI.Window);
        _inputField.OffsetPosition = new Vector2(0, 60);
        _inputField.Anchor = Anchor.TopCenter;
        _inputField.Stretch = StretchMode.Horizontal;
        
        Add(_resumeButton);
        Add(_inputField);
    }

    private void OnResumeClicked()
    {
        Time.IsPaused = false;
        Close();
    }

    public void Close()
    {
        _inputField.Text = string.Empty;
    }
}