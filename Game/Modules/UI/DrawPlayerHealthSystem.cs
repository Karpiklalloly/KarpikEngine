using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.VisualElements;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public class DrawPlayerHealthSystem : IEcsRun
{
    private class Aspect : EcsAspect
    {
        public EcsPool<Health> health = Inc;
        public EcsTagPool<Player> player = Inc;
    }

    private EcsDefaultWorld _world = Worlds.Instance.World;
    private ProgressBar _bar;
    
    public void Run()
    {
        var entities = _world.Where(out Aspect a);
        if (entities.Count == 0) return;
        
        var health = a.health.Get(entities[0]);
        if (_bar == null)
        {
            _bar = new ProgressBar(new Rectangle(new Point(10, 10), new Point(300, 20)));
            _bar.Font = UI.DefaultFont;
            _bar.ForegroundColor = Color.Red;
            _bar.BackgroundColor = Color.Black;
            UI.Root.Add(_bar);
        }
        
        _bar.Value = health.Value;
        _bar.MaxValue = health.Max.ModifiedValue;
    }
}