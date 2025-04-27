using Karpik.DragonECS;
using KarpikEngineMono;
using KarpikEngineMono.Modules.EcsCore;

namespace Game.Modules;

public class DealDamageOnContactSystem : IEcsFixedRunOnEvent<CollisionsEvent>
{
    private EcsEventWorld _eventWorld = Worlds.Instance.EventWorld;
    
    private void Process(entlong source, entlong target)
    {
        if (!source.Has<DealDamageOnContact>()) return;
        if (!source.Has<Damage>()) return;
        if (!target.Has<Health>()) return;
        
        var damage = source.Get<Damage>().ModifiedValue;
        _eventWorld.SendEvent(new DealDamageEvent()
        {
            Target = target,
            Damage = damage,
            Source = source,
        });
    }

    public void RunOnEvent(ref CollisionsEvent evt)
    {
        Console.WriteLine("DealDamageOnContactSystem");
        Process(evt.Source, evt.Target);
        Process(evt.Target, evt.Source);
    }
}