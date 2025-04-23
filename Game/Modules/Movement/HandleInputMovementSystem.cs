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
            // if (entities.Count > 0)
            // {
            //     var en = entities[0];
            //     var e = _world.GetEntityLong(en);
            //     var speed = a.speed.Get(en).Value;
            //     if (Input.IsDown(Keys.W))
            //     {
            //         e.MoveBySpeed(new Vector2(0, -1));
            //     }
            //     if (Input.IsDown(Keys.S))
            //     {
            //         e.MoveBySpeed(new Vector2(0, 1));
            //     }
            //     if (Input.IsDown(Keys.A))
            //     {
            //         e.MoveBySpeed(new Vector2(-1, 0));
            //     }
            //     if (Input.IsDown(Keys.D))
            //     {
            //         e.MoveBySpeed(new Vector2(1, 0));
            //     }
            // }
            //
            // if (entities.Count > 1)
            // {
            //     var en = entities[1];
            //     var e = _world.GetEntityLong(en);
            //     var speed = a.speed.Get(en).Value;
            //     if (Input.IsDown(Keys.Up))
            //     {
            //         e.MoveBySpeed(new Vector2(0, -1));
            //     }
            //     if (Input.IsDown(Keys.Down))
            //     {
            //         e.MoveBySpeed(new Vector2(0, 1));
            //     }
            //     if (Input.IsDown(Keys.Left))
            //     {
            //         e.MoveBySpeed(new Vector2(-1, 0));
            //     }
            //     if (Input.IsDown(Keys.Right))
            //     {
            //         e.MoveBySpeed(new Vector2(1, 0));
            //     }
            // }
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
