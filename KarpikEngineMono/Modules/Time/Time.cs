namespace KarpikEngineMono.Modules;

public static class Time
{
    public static double DeltaTime { get; private set; }
    public static double FixedDeltaTime => 1.0 / 50;
    public static double TotalTime { get; private set; }
    public static bool IsPaused { get; set; }

    internal static void Update(double deltaTime)
    {
        DeltaTime = deltaTime;
        TotalTime += deltaTime;
    }
}