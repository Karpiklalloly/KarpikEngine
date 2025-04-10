using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;

namespace KarpikEngine.Modules.EcsRunners;

public interface IEcsFixedRunProcess : IEcsProcess
{
    public void FixedRun();
}

public class EcsFixedRunRunner : EcsRunner<IEcsFixedRunProcess>, IEcsFixedRunProcess
{
    private RunHelper _helper;
    protected override void OnSetup()
    {
        _helper = new RunHelper(this);
    }
    public void FixedRun()
    {
        _helper.Run(p => p.FixedRun());
    }
}
