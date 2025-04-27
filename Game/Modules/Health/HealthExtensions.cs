using System.Runtime.CompilerServices;
using Karpik.DragonECS;
using KarpikEngineMono;

namespace Game.Modules;

public partial class HealthExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DealDamageTo(this entlong source, entlong target, float damage)
    {
        Worlds.Instance.EventWorld.SendEvent(new DealDamageEvent()
        {
            Damage = damage,
            Target = target,
            Source = source,
        });
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TakeDamageFrom(this entlong target, entlong source, float damage)
    {
        source.DealDamageTo(target, damage);
    }
}