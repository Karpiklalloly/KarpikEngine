using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public class PauseMenu : VisualElement
{
    private readonly Button _resumeButton;
    private readonly Label _title;

    public PauseMenu(Vector2 size) : base(size)
    {
        _resumeButton = new Button(new Vector2(600, 200), "Resume", UI.DefaultFont);
        _resumeButton.Clicked += OnResumeClicked;
        _resumeButton.Anchor = Anchor.Center;
        
        _title = new Label("My game");
        _title.OffsetPosition = new Vector2(0, 60);
        _title.Anchor = Anchor.TopCenter;
        _title.Stretch = StretchMode.Horizontal;
        
        Add(_resumeButton);
        Add(_title);
    }

    private void OnResumeClicked()
    {
        Time.IsPaused = false;
        Close();
    }
    
    public void Open()
    {
        IsVisible = IsEnabled = true;
    }

    public void Close()
    {
        IsVisible = IsEnabled = false;
    }
}