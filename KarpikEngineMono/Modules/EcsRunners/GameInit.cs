using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using KarpikEngine;

namespace Karpik.Vampire.Scripts.DragonECS.CustomRunners
{
    public interface IEcsGamePreInit : IEcsProcess
    {
        public void PreInitGame();
    }
    
    public class GamePreInit : EcsRunner<IEcsGamePreInit>, IEcsGamePreInit
    {
        private EcsDefaultWorld _world;

        public GamePreInit()
        {
            _world = Worlds.Instance.World;
        }

        public void PreInitGame()
        {
            foreach (var gameInit in Process)
            {
                gameInit.PreInitGame();
            }
        }
    }
    
    public interface IEcsGameInit : IEcsProcess
    {
        public void InitGame();
    }
    
    public class GameInit : EcsRunner<IEcsGameInit>, IEcsGameInit
    {
        private EcsDefaultWorld _world;

        public GameInit()
        {
            _world = Worlds.Instance.World;
        }

        public void InitGame()
        {
            foreach (var gameInit in Process)
            {
                gameInit.InitGame();
            }
        }
    }
}