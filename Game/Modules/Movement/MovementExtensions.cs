using System.Runtime.CompilerServices;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public static class MovementExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveBySpeed(this entlong entity, Vector2 direction)
    {
        var speed = entity.Get<Speed>().ModifiedValue;
        var body = entity.Get<RigidBody>();
        entity.Move((float)(speed * body.Mass) * direction.Normalized());
    }
}