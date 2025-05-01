using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public class OpenMenuOnPauseSystem : IEcsGameInit, IEcsRun
{
    private PauseMenu _menu;
    
    public void InitGame()
    {
        _menu = new PauseMenu(new Rectangle(0, 0, 0, 0));
        _menu.Anchor = Anchor.StretchAll;
        UI.Root.Add(_menu);
    }
    
    public void Run()
    {
        _menu.IsVisible = Time.IsPaused;
        _menu.IsEnabled = Time.IsPaused;
    }
}