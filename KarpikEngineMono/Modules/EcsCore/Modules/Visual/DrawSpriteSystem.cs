using System;
using System.Numerics;
using DCFApixels.DragonECS;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace KarpikEngineMono.Modules.EcsCore;

public class DrawSpriteSystem : IEcsRun
{
    public class Aspect : EcsAspect
    {
        public EcsPool<SpriteRenderer> sprite = Inc;
        public EcsPool<Transform> transform = Inc;
    }
    
    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void Run()
    {
        var span = _world.Where(out Aspect a);
        foreach (var e in span)
        {
            var sprite = a.sprite.Get(e);
            var transform = a.transform.Get(e);
            Drawer.Sprite(sprite, transform);
        }
    }
}