using DCFApixels.DragonECS;
using KarpikEngineMono;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using KarpikEngineMono.Modules;

namespace Game
{
    public class HandleInputMovementSystem : IEcsRun
    {
        private class Aspect : EcsAspect
        {
            public EcsPool<Transform> Transform = Inc;
            public EcsTagPool<HandleInputMovement> handleInputMovements = Inc;
        }

        private EcsDefaultWorld _world = Worlds.Instance.World;

        public void Run()
        {
            foreach (var en in _world.Where(out Aspect _))
            {
                var e = _world.GetEntityLong(en);
                if (Input.IsDown(Keys.W) || Input.IsDown(Keys.Up))
                {
                    e.AddMove(new Vector2(0, -1));
                }
                if (Input.IsDown(Keys.S) || Input.IsDown(Keys.Down))
                {
                    e.AddMove(new Vector2(0, 1));
                }
                if (Input.IsDown(Keys.A) || Input.IsDown(Keys.Left))
                {
                    e.AddMove(new Vector2(-1, 0));
                }
                if (Input.IsDown(Keys.D) || Input.IsDown(Keys.Right))
                {
                    e.AddMove(new Vector2(1, 0));
                }
            }
        }
    }
}
