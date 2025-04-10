using System;
using System.Numerics;
using DCFApixels.DragonECS;
using KarpikEngine.Modules.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace KarpikEngine.Modules.EcsCore;

public class DrawSpriteSystem : IEcsRun, IEcsInject<SpriteBatch>
{
    public class Aspect : EcsAspect
    {
        public EcsPool<SpriteRenderer> sprite = Inc;
        public EcsPool<Transform> transform = Inc;
    }
    
    private SpriteBatch _batch;
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

    public void Inject(SpriteBatch batch)
    {
        _batch = batch;
    }
}