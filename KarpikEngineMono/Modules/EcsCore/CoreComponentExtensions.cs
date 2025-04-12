using System.Numerics;
using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    public static class CoreComponentExtensions
    {
        public static void AddMove(this entlong entity, Vector2 direction)
        {
            var world = entity.World;
            ref var velocity = ref world.GetPool<Force>().TryAddOrGet(entity.ID);
            velocity.Direction += direction;
        }

        // public static void Move(this entlong entity, ref MoveDirection direction)
        // {
        //     entity.Get<Transform>().Position += direction.Direction * (float)entity.Get<Speed>().Value * (float)Time.DeltaTime;
        // }

        public static ref T Get<T>(this entlong entity) where T : struct, IEcsComponent
        {
            return ref Worlds.Instance.World.GetPool<T>().Get(entity.ID);
        }

        public static ref T Add<T>(this entlong entity) where T : struct, IEcsComponent
        {
            return ref Worlds.Instance.World.GetPool<T>().Add(entity.ID);
        }
    }

    public static class CoreComponentExtensionsTag
    {
        public static void Add<T>(this entlong entity) where T : struct, IEcsTagComponent
        {
            Worlds.Instance.World.GetPool<T>().Add(entity.ID);
        }
    }
}
