using KarpikEngineMonoGame;

namespace Game;

class Program
{
    static void Main(string[] args)
    {
        using var main = new Main();
        main.Add(new DemoModule());
        main.Run();
    }
}