using System;
using System.Xml.Serialization;
using DCFApixels.DragonECS;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.Graphics;
using KarpikEngineMono.Modules.SaveLoad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class DemoModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new MySystem())
            .Add(new HandleInputMovementSystem(), EcsConsts.BEGIN_LAYER);
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
        DebugGraphics.Text("Hello World!");
        DebugGraphics.Text($"{typeof(DemoModule).FullName}");
        DebugGraphics.Text($"{Time.DeltaTime}");
        DebugGraphics.Text($"{Time.TotalTime:F2}");
        foreach (var entity in Worlds.Instance.EventWorld.Entities)
        {
            DebugGraphics.Text(entity.ToString());
        }
        DebugGraphics.End();
        
        if (Input.IsDown(Keys.F1))
        {
            var template = Loader.LoadTemplate("Player");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
                    
            template.ApplyTo(e.ID, world);
        }
    }
}

[Serializable]
public struct HandleInputMovement : IEcsTagComponent
{

}