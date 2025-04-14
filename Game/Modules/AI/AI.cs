using KarpikEngineMono;
using KarpikEngineMono.Modules.EcsCore;

namespace Game.Modules;

public static class AI
{
    public static void Follow(this entlong entity, int target)
    {
        entity.Add<FollowTarget>() = new FollowTarget() 
        {
            Target = target
        };
    }

    public static void FollowPlayer(this entlong entity)
    {
        ref var player = ref Worlds.Instance.MetaWorld.GetPlayer();
        
        entity.Add<FollowTarget>() = new FollowTarget() 
        {
            Target = player.Player.ID
        };
    }

    public static ref PlayerRef GetPlayer(this MetaWorld metaWorld)
    {
        ref var player = ref Worlds.Instance.MetaWorld.Get<PlayerRef>();
        if (player.Player.IsNull)
        {
            player = new PlayerRef()
            {
                Player = Worlds.Instance.World.Where(EcsStaticMask.Inc<Player>().Build()).Longs[0]
            };
        }

        return ref player;
    }
}