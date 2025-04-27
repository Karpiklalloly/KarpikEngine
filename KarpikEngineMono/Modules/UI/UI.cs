using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules;

public static class UI
{
    public static VisualElement Root { get; internal set; }
    internal static SpriteBatch UISpriteBatch { get; set; }
    private static Queue<VisualElement> _queuedElements = new();
    
    public static SpriteFont DefaultFont { get; internal set; }

    internal static void Update()
    {
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