using Game.Modules;
using GTweens.Easings;
using GTweens.Enums;
using GTweens.Extensions;
using Karpik.DragonECS;
using Karpik.StatAndAbilities;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class DemoModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new MySystem());
    }
}

public class MySystem : IEcsInit, IEcsRun
{
    public void Init()
    {
        Console.WriteLine("init");
    }

    public void Run()
    {
        DebugGraphics.Begin("DemoWindow");
        DebugGraphics.Text($"Total time: {Time.TotalTime:F2}");
        DebugGraphics.Text($"Delta time: {Time.DeltaTime}");
        DebugGraphics.Text($"FPS: {1 / Time.DeltaTime:F2}");
        DebugGraphics.Text($"Entities: {Worlds.Instance.World.Entities.Count}");
        DebugGraphics.Text($"Event Entities: {Worlds.Instance.EventWorld.Entities.Count}");
        if (!Worlds.Instance.MetaWorld.GetPlayer().Player.IsNull)
        {
            var health = Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Health>();
            var pos = Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Transform>().Position;
            DebugGraphics.Text($"Player Health: {health.Value} of {health.Max.ModifiedValue}");
            DebugGraphics.Text($"Player Position: {pos.X:F2}, {pos.Y:F2}");
        }
        DebugGraphics.End();

        if (Input.IsPressed(Keys.Escape))
        {
            Time.IsPaused = !Time.IsPaused;
        }
        
        if (Input.IsPressed(Keys.F1))
        {
            var template = Loader.LoadTemplate("Player");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
        }
        
        if (Input.IsPressed(Keys.F2))
        {
            var template = Loader.LoadTemplate("Enemy");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
        }

        if (Input.IsPressed(Keys.F3))
        {
            var template = new ComponentsTemplate(
                new SpriteRenderer(),
                new Transform(),
                new HandleInputMovement(),
                new RigidBody(10, 11, 12, 13, 14, RigidBody.BodyType.Dynamic, RigidBody.CollisionMode.ContinuousSpeculative),
                new ColliderBox(),
                new Speed(),
                new Player(),
                new Health());
            Loader.Serialize(template, "test template");
        }

        if (Input.IsPressed(Keys.F4))
        {
            Worlds.Instance.EventWorld.SendEvent(new KillEvent());
        }

        if (Input.IsPressed(Keys.F5))
        {
            ref var health = ref Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Health>();
            health.Value += health.Max.ModifiedValue;
        }
        
        if (Input.IsPressed(Keys.F6))
        {
            var template = Loader.LoadTemplate("Enemy");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
            e.Del<FollowPlayer>();
            
            var transform = e.Get<Transform>();
            var position = transform.Position;

            var tween = GTweenExtensions.Tween(
                () => e.Get<Transform>().Position,
                pos => e.Get<Transform>().Position = pos,
                position + new Vector2(0, 200),
                2f)
                .SetEasing(Easing.InOutCirc)
                .SetLoops(-1, ResetMode.PingPong);
            Tween.Add(tween, false);
        }
        
        if (Input.IsPressed(Keys.F7))
        {
            var template = Loader.LoadTemplate("Enemy");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
            e.Del<FollowPlayer>();
            
            var transform = e.Get<Transform>();
            var position = transform.Position;

            var tween = GTweenExtensions.Tween(
                    () => e.Get<Transform>().Position,
                    pos => e.Get<Transform>().Position = pos,
                    position + new Vector2(200, 200),
                    2f)
                .SetEasing(Easing.InOutCirc)
                .SetLoops(-1, ResetMode.PingPong);
            Tween.Add(tween, true);
        }
    }
}