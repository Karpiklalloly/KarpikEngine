using System;
using System.Collections.Generic;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.Graphics;

public static class Drawer
{
    internal static SpriteBatch SpriteBatch;

    private static DrawAction[] _actions = new DrawAction[128];
    private static int _actionsCount = 0;
    
    public static void Sprite(SpriteRenderer spriteRenderer, Transform transform)
    {
        ResizeIfNeed();
        _actions[_actionsCount++] = new DrawAction()
        {
            Texture = spriteRenderer.Texture,
            Position = transform.Position,
            Color = spriteRenderer.Color,
            Rotation = transform.Rotation,
            Scale = transform.Scale,
            Effect = spriteRenderer.Effect,
            Layer = spriteRenderer.Layer
        };
    }

    internal static void Draw()
    {
        SpriteBatch.Begin();
        while (_actionsCount > 0)
        {
            _actions[--_actionsCount].Draw();
        }
        SpriteBatch.End();
    }

    private static void ResizeIfNeed()
    {
        if (_actionsCount >= _actions.Length)
        {
            Array.Resize(ref _actions, _actions.Length * 2);
        }
    }

    private struct DrawAction
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Scale;
        public SpriteEffects Effect;
        public float Layer;
        
        public void Draw() => SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Vector2.Zero, Scale, Effect, Layer);
    }
}