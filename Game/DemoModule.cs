using System;
using System.Xml.Serialization;
using DCFApixels.DragonECS;
using KarpikEngine;
using KarpikEngine.Modules.EcsCore;
using KarpikEngine.Modules.Graphics;
using KarpikEngine.Modules.SaveLoad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class DemoModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new MySystem())
            .Add(new DrawSpriteSystem());
    }
}

public class MySystem : IEcsInit, IEcsRun
{
    public void Init()
    {
        Console.WriteLine("init");

        ComponentsTemplate template = new ComponentsTemplate(new SpriteRenderer(), new Transform());
        Loader.Serialize(template, "test2");
    }

    public void Run()
    {
        DebugGraphics.Begin("DemoWindow");
        DebugGraphics.Text("Hello World!");
        DebugGraphics.Text($"{typeof(DemoModule).FullName}");
        DebugGraphics.End();
        
        if (Keyboard.GetState().IsKeyDown(Keys.F1))
        {
            
            //var template = Loader.LoadTemplate("test.xml");
            var world = Worlds.Instance.World;
            var e = world.NewEntity();
            world.GetPool<SpriteRenderer>().Add(e) = new SpriteRenderer()
            {
                Texture = Loader.Load<Texture2D>("player"),
                Color = Color.White,
                Effect = SpriteEffects.None,
                Layer = 0
            };
            world.GetPool<Transform>().Add(e) = new Transform()
            {
                Position = new Vector2(0, 0),
                Rotation = 0,
                Scale = Vector2.One
            };
        }
    }
}