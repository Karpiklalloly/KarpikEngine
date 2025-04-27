using KarpikEngineMono;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using KarpikEngineMono.Modules;

namespace Game.Modules
{
    public class HandleInputMovementSystem : IEcsRun
    {
        private class Aspect : EcsAspect
        {
            public EcsPool<Transform> Transform = Inc;
            public EcsTagPool<HandleInputMovement> handleInputMovements = Inc;
            public EcsPool<Speed> speed = Inc;
        }

        private EcsDefaultWorld _world = Worlds.Instance.World;

        public void Run()
        {
            var entities = _world.Where(out Aspect a);
            foreach (var en in entities)
            {
                var e = _world.GetEntityLong(en);
                var speed = a.speed.Get(en).ModifiedValue;
                if (Input.IsDown(Keys.W) || Input.IsDown(Keys.Up))
                {
                    e.MoveBySpeed(new Vector2(0, 1));
                }
                if (Input.IsDown(Keys.S) || Input.IsDown(Keys.Down))
                {
                    e.MoveBySpeed(new Vector2(0, -1));
                }
                if (Input.IsDown(Keys.A) || Input.IsDown(Keys.Left))
                {
                    e.MoveBySpeed(new Vector2(-1, 0));
                }
                if (Input.IsDown(Keys.D) || Input.IsDown(Keys.Right))
                {
                    e.MoveBySpeed(new Vector2(1, 0));
                }
            }
        }
    }
}
