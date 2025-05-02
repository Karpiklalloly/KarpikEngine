using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class Window : Panel
{
    public string Title { get; private set; }
    public SpriteFont TitleFont { get; set; }
    
    public Window(Vector2 size) : base(size)
    {
        
    }
}