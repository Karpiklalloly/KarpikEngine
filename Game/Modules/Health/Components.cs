using Karpik.DragonECS;
using Karpik.StatAndAbilities;

namespace Game.Modules;

[EzRangeStat]
public partial struct Health { }

[Stat]
public partial struct Damage { }

public struct DealDamageRequest : IEcsComponentRequest
{
    public entlong Target { get; set; }
    public double Damage { get; set; }
}

public struct DealDamageEvent : IEcsComponentEvent
{
    public entlong Source { get; set; }
    public entlong Target { get; set; }
    public double Damage { get; set; }
}

public struct KillEvent : IEcsComponentEvent
{
    public entlong Source { get; set; }
    public entlong Target { get; set; }
}

public struct KillRequest : IEcsComponentRequest
{
    public entlong Target { get; set; }
}

[Serializable]
public struct DealDamageOnContact : IEcsTagComponent;