using ImGuiNET;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.Graphics;

namespace Game.Modules;

public class MoveCameraToPlayerSystem : IEcsFixedRun, IEcsDebugRun
{
    private class Aspect : EcsAspect
    {
        public EcsTagPool<Player> player = Inc;
        public EcsPool<Transform> transform = Inc;
    }

    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void FixedRun()
    {
        if (Time.IsPaused) return;
        
        var span = _world.Where(out Aspect a);
        if (span.Count == 0) return;
        
        var transform = a.transform.Get(span[0]);
        Camera.Main.Position = transform.Position;
    }

    public void DebugRun()
    {
        ImGui.Begin("Camera");
        
        ImGui.Text($"Camera Position: {Camera.Main.Position}");
        
        ImGui.End();
    }
}