using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Modules;

public class OpenMenuOnPauseSystem : IEcsRun
{
    private bool _show = false;
    private PauseMenu _menu;
    
    public OpenMenuOnPauseSystem()
    {
        Input.KeyPressed += OnKeyPressed;

        _menu = new PauseMenu(new Rectangle(0, 0, 1600, 900));
    }
    
    public void Run()
    {
        
    }
    
    private void OnKeyPressed(Keys key)
    {
        if (key == Keys.Escape)
        {
            _show = !_show;
        }
    }
}