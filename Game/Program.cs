using Game.Modules;
using Karpik.StatAndAbilities;
using KarpikEngineMonoGame;

namespace Game;

class Program
{
    static void Main(string[] args)
    {
        using var main = new Main();
        main.Window.AllowUserResizing = true;
        main.Add(new DemoModule())
            .Add(new AIModule())
            .Add(new MovementModule());
        main.Run();
    }
}