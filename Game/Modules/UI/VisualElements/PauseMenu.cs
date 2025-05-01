using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public class PauseMenu : VisualElement
{
    public PauseMenu(Rectangle bounds) : base(bounds)
    {
        var resumeButton = new Button(new Rectangle(0, 0, 600, 200), "Resume", UI.DefaultFont);
        resumeButton.Clicked += OnResumeClicked;
        resumeButton.Anchor = Anchor.Center;
        
        var inputField = new InputField(new Rectangle(0, 60, 600, 100), UI.DefaultFont, UI.Window);
        inputField.Anchor = Anchor.TopCenter;
        
        Anchor = Anchor.Center;
        Add(resumeButton);
        Add(inputField);
    }

    private void OnResumeClicked()
    {
        Time.IsPaused = false;
        Console.WriteLine(this.Q<InputField>().Text);
    }
}