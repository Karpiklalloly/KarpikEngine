using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules;

public static class UI
{
    public static VisualElement Root { get; internal set; }
    internal static SpriteBatch UISpriteBatch { get; set; }
    public static GameWindow Window { get; internal set; }
    
    public static SpriteFont DefaultFont { get; internal set; }

    internal static void Update()
    {
        Root.OffsetRect = Window.ClientBounds with {X = 0, Y = 0};
        Root.Update(Time.DeltaTime);
    }

    internal static void Draw()
    {
        var elements = Root.AllChildren.Concat([Root]).OrderBy(x => x.Order);
        
        UISpriteBatch.Begin();
        foreach (var element in elements)
        {
            element.Draw(Time.DeltaTime, UISpriteBatch);
        }
        Root.Draw(Time.DeltaTime, UISpriteBatch);
        UISpriteBatch.End();
    }
}