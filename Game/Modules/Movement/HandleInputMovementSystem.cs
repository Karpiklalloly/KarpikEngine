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
            //         e.AddMove(new Vector2(0, -1) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.S))
            //     {
            //         e.AddMove(new Vector2(0, 1) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.A))
            //     {
            //         e.AddMove(new Vector2(-1, 0) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.D))
            //     {
            //         e.AddMove(new Vector2(1, 0) * (float)speed);
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
            //         e.AddMove(new Vector2(0, -1) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.Down))
            //     {
            //         e.AddMove(new Vector2(0, 1) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.Left))
            //     {
            //         e.AddMove(new Vector2(-1, 0) * (float)speed);
            //     }
            //     if (Input.IsDown(Keys.Right))
            //     {
            //         e.AddMove(new Vector2(1, 0) * (float)speed);
            //     }
            // }
            foreach (var en in entities)
            {
                var e = _world.GetEntityLong(en);
                var speed = a.speed.Get(en).Value;
                if (Input.IsDown(Keys.W) || Input.IsDown(Keys.Up))
                {
                    e.Move(new Vector2(0, -1) * (float)speed);
                }
                if (Input.IsDown(Keys.S) || Input.IsDown(Keys.Down))
                {
                    e.Move(new Vector2(0, 1) * (float)speed);
                }
                if (Input.IsDown(Keys.A) || Input.IsDown(Keys.Left))
                {
                    e.Move(new Vector2(-1, 0) * (float)speed);
                }
                if (Input.IsDown(Keys.D) || Input.IsDown(Keys.Right))
                {
                    e.Move(new Vector2(1, 0) * (float)speed);
                }
            }
        }
    }
}
