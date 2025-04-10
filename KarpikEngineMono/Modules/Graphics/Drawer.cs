using System;
using System.Collections.Generic;
using KarpikEngine.Modules.EcsCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngine.Modules.Graphics;

public static class Drawer
{
    internal static SpriteBatch SpriteBatch;

    private static Queue<Action> _draws = new Queue<Action>();
    
    public static void Sprite(SpriteRenderer spriteRenderer, Transform transform)
    {
        _draws.Enqueue(() => SpriteBatch.Draw(
            spriteRenderer.Texture,
            transform.Position,
            null,
            spriteRenderer.Color,
            transform.Rotation,
            Vector2.Zero,
            transform.Scale.X,
            spriteRenderer.Effect,
            spriteRenderer.Layer));
    }

    internal static void Draw()
    {
        SpriteBatch.Begin();
        while (_draws.Count > 0)
        {
            _draws.Dequeue().Invoke();
        }
        SpriteBatch.End();
    }
}