using System.Runtime.CompilerServices;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules;

public static class Physics
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Move(this entlong entity, Vector2 direction)
    {
        var world = entity.World;
        ref var velocity = ref world.GetPool<Force>().TryAddOrGet(entity.ID);
        velocity.Direction += direction;
    }
}