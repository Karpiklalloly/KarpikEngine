namespace KarpikEngineMono.Modules.EcsCore;

public class InputUpdateSystem : IEcsRun
{
    public void Run()
    {
        Input.Update();
    }
}