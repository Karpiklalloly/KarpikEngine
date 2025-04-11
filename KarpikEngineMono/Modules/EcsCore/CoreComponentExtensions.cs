using System.Numerics;
using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    public static class CoreComponentExtensions
    {
        public static void AddMove(this entlong entity, Vector2 direction)
        {
            Worlds.Instance.EventWorld.SendEvent(new MoveDirection()
            {
                Target = entity,
                Direction = direction,
            });
        }

        public static void Move(this entlong entity, ref MoveDirection direction)
        {
            entity.Get<Transform>().Position += direction.Direction * (float)entity.Get<Speed>().Value * (float)Time.DeltaTime;
        }

        public static ref T Get<T>(this entlong entity) where T : struct, IEcsComponent
        {
            return ref Worlds.Instance.World.GetPool<T>().Get(entity.ID);
        }
    }
}
