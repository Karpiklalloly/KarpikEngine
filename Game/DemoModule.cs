using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.Graphics;
using KarpikEngineMono.Modules.SaveLoad;
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
        
        // var world = Worlds.Instance.World;
        // var e = world.NewEntityLong();
        // var transform = new Transform();
        // transform.Position = new Vector2(100, 100);
        // transform.Rotation = 0;
        // transform.Scale = new Vector2(1, 1);
        // var sprite = new SpriteRenderer();
        // sprite.Texture = Loader.LoadTexture("Player");
        // sprite.Color = Color.White;
        // sprite.Effect = SpriteEffects.None;
        // sprite.Layer = 0;
        // sprite.TexturePath = "Player";
        // var rigidBody = new RigidBody(10, 11, 12, 13, 14, RigidBody.BodyType.Dynamic);
        // var box = new ColliderBox();
        // box.Size = new Vector2(1, 1);
        // box.Offset = new Vector2(0, 0);
        // box.IsTrigger = false;
        //
        // e.Add<Transform>() = transform;
        // e.Add<SpriteRenderer>() = sprite;
        // e.Add<RigidBody>() = rigidBody;
        // e.Add<HandleInputMovement>();
        // e.Add<ColliderBox>() = box;
        //
        // ComponentsTemplate template = new ComponentsTemplate(transform, sprite, rigidBody, box, new HandleInputMovement());
        // Loader.Serialize(template, "test physics");
        //
        // var t = Loader.LoadTemplate("test physics");
        // var e2 = world.NewEntityLong();
        // Loader.Serialize(t, "test physics2");
        // t.ApplyTo(e2.ID, world);
    }

    public void Run()
    {
        DebugGraphics.Begin("DemoWindow");
        DebugGraphics.Text("Hello World!");
        DebugGraphics.Text($"{typeof(DemoModule).FullName}");
        DebugGraphics.Text($"Delta time: {Time.DeltaTime}");
        DebugGraphics.Text($"Total time: {Time.TotalTime:F2}");
        DebugGraphics.Text($"FPS: {1 / Time.DeltaTime:F2}");
        DebugGraphics.Text($"Entities: {Worlds.Instance.World.Entities.Count}");
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
    }
}