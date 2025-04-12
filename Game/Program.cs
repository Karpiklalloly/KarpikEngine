using KarpikEngineMonoGame;

namespace Game;

class Program
{
    static void Main(string[] args)
    {
        using var main = new Main();
        main.Window.AllowUserResizing = true;
        main.Add(new DemoModule());
        main.Run();
    }
}