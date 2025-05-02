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
        _menu = new PauseMenu(Vector2.Zero);
        _menu.Pivot = Vector2.Zero;
        _menu.Anchor = Anchor.StretchAll;
        _menu.Stretch = StretchMode.Both;
        UI.Root.Add(_menu);
    }
    
    public void Run()
    {
        if (Time.IsPaused)
        {
            _menu.Open();
        }
        else
        {
            _menu.Close();
        }
    }
}