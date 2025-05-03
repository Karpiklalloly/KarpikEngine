using GTweens.Contexts;
using GTweens.Tweens;

namespace KarpikEngineMono.Modules;

public static class Tween
{
    private static GTweensContext _context = new GTweensContext();
    private static GTweensContext _pausableContext = new GTweensContext();
    
    public static void Add(GTween tween, bool pausable)
    {
        if (pausable)
        {
            _pausableContext.Play(tween);
        }
        else
        {
            _context.Play(tween);
        }
    }

    public static void Update(double deltaTime)
    {
        _context.Tick((float)deltaTime);
    }

    public static void UpdatePausable(double deltaTime)
    {
        _pausableContext.Tick((float)deltaTime);
    }
}