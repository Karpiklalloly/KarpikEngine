using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Input;

namespace KarpikEngineMono.Modules;

public static class Input
{
    private enum State
    {
        DownEvent,
        DownHold,
        UpEvent,
        UpHold
    }
    private static ConcurrentDictionary<Keys, State> _states = new();

    public static bool IsPressed(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state == State.DownEvent;
        return false;
    }
    
    public static bool IsUnPressed(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state == State.UpEvent;
        return false;
    }

    public static bool IsPressing(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state == State.DownHold;
        return false;
    }
    
    public static bool IsUnPressing(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state == State.UpHold;
        return false;
    }
    
    public static bool IsDown(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state is State.DownEvent or State.DownHold;
        return false;
    }
    
    public static bool IsUp(Keys key)
    {
        if (_states.TryGetValue(key, out var state)) return state is State.UpEvent or State.UpHold;
        return false;
    }

    internal static void Update()
    {
        var keys = Keyboard.GetState().GetPressedKeys();
        
        foreach (var key in keys)
        {
            if (_states.ContainsKey(key))
            {
                if (_states[key] == State.DownEvent)
                {
                    _states[key] = State.DownHold;
                }
                if (_states[key] == State.UpEvent || _states[key] == State.UpHold)
                {
                    _states[key] = State.DownEvent;
                }
            }
            
            if (!_states.ContainsKey(key))
            {
                _states.TryAdd(key, State.DownEvent);
            }
        }

        keys = _states.Keys.Except(keys).ToArray();
        foreach (var key in keys)
        {
            if (_states[key] == State.DownEvent || _states[key] == State.DownHold)
            {
                _states[key] = State.UpEvent;
            }

            if (_states[key] == State.UpEvent)
            {
                _states[key] = State.UpHold;
            }
        }
    }
}