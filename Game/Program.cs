using Game.Modules;
using GTweens.Builders;
using GTweens.Extensions;
using GTweens.Tweens;
using Karpik.StatAndAbilities;
using KarpikEngineMonoGame;
using Microsoft.Xna.Framework;

namespace Game;

class Program
{
    static void Main(string[] args)
    {
        GTweenSequenceBuilder.New()
            .Build();
        
        
        using var main = new Main();
        main.Window.AllowUserResizing = true;
        main.Add(new DemoModule())
            .Add(new AIModule())
            .Add(new HealthModule())
            .Add(new MovementModule())
            .Add(new UIModule())
            ;
        main.Run();
    }
}