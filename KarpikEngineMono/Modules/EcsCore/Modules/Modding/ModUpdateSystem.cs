using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.Modding;

namespace KarpikEngineMono.Modules.EcsCore.Modules.Modding;

public class ModUpdateSystem : IEcsRun, IEcsInit, IEcsFixedRun, IEcsInject<ModManager>
{
    private ModManager _modManager;

    public void Run()
    {
        _modManager.UpdateMods();
    }
    
    public void FixedRun()
    {
        _modManager.FixedUpdateMods();
    }

    public void Init()
    {
        _modManager.StartMods();
    }

    public void Inject(ModManager obj)
    {
        _modManager = obj;
    }
}