using System;
using System.Collections.Generic;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.Graphics;

public static class Drawer
{
    internal static SpriteBatch SpriteBatch;
    internal static GameWindow Window;

    private static DrawAction[] _actions = new DrawAction[128];
    private static int _actionsCount = 0;
    
    public static void Sprite(SpriteRenderer spriteRenderer, Transform transform)
    {
        ResizeIfNeed();
        _actions[_actionsCount++] = new DrawAction()
        {
            Texture = spriteRenderer.Texture,
            Position = new Vector2(transform.Position.X, -transform.Position.Y),
            Color = spriteRenderer.Color,
            Rotation = transform.Rotation,
            Scale = transform.Scale,
            Effect = spriteRenderer.Effect,
            Layer = spriteRenderer.Layer
        };
    }

    internal static void Draw()
    {
        SpriteBatch.Begin(transformMatrix: Camera.Main.GetViewMatrix());
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
        public double Rotation;
        public Vector2 Scale;
        public SpriteEffects Effect;
        public float Layer;
        
        public void Draw() => SpriteBatch
            .Draw(
                Texture,
                Position,
                null,
                Color,
                (float)Rotation,
                new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                Scale,
                Effect,
                Layer);
    }
}