using DCFApixels.DragonECS.RunnersCore;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;

namespace KarpikEngineMono.Modules.EcsRunners;

public interface IEcsDebugRun : IEcsProcess
{
    public void DebugRun();
}

public class DebugRunner : EcsRunner<IEcsDebugRun>, IEcsDebugRun, IEcsInject<ImGuiRenderer>, IEcsInject<GameTime>
{
    private ImGuiRenderer _imGuiRenderer;
    private GameTime _gameTime = new GameTime();
    
    public void DebugRun()
    {
        _imGuiRenderer.BeginLayout(_gameTime);
        foreach (var process in Process)
        {
            process.DebugRun();
        }
        _imGuiRenderer.EndLayout();
    }

    public void Inject(ImGuiRenderer obj)
    {
        _imGuiRenderer = obj;
    }

    public void Inject(GameTime obj)
    {
        _gameTime = obj;
    }
}