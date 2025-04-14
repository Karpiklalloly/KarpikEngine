using System.Numerics;
using System.Runtime.CompilerServices;
using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    public static class CoreComponentExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Get<T>(this entlong entity) where T : struct, IEcsComponent
        {
            return ref Worlds.Instance.World.GetPool<T>().Get(entity.ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this entlong entity) where T : struct, IEcsComponent
        {
            return ref Worlds.Instance.World.GetPool<T>().Add(entity.ID);
        }
    }

    public static class CoreComponentExtensionsTag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(this entlong entity) where T : struct, IEcsTagComponent
        {
            Worlds.Instance.World.GetPool<T>().Add(entity.ID);
        }
    }
}
