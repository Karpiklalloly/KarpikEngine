using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
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

    public static event Action<Keys> KeyPressed;
    public static event Action<Keys> KeyUnPressed;
    public static event Action<Keys> KeyPressing;
    
    private static ConcurrentDictionary<Keys, State> _states = new();
    
    private static MouseState _currentMouseState;
    private static MouseState _previousMouseState;
    
    public static Vector2 MousePosition => _currentMouseState.Position.ToVector2();
    
    public static IEnumerable<Keys> PressedKeys => _states.Keys.Where(IsPressed);
    
    public static bool IsMouseLeftButtonDown => _currentMouseState.LeftButton == ButtonState.Pressed
                                                && _previousMouseState.LeftButton == ButtonState.Released;
    
    public static bool IsMouseLeftButtonUp => _currentMouseState.LeftButton == ButtonState.Released
                                              && _previousMouseState.LeftButton == ButtonState.Pressed;
    
    public static bool IsMouseLeftButtonHold => _currentMouseState.LeftButton == ButtonState.Pressed
                                                && _previousMouseState.LeftButton == ButtonState.Pressed;
    
    public static bool IsMouseRightButtonDown => _currentMouseState.RightButton == ButtonState.Pressed 
                                                 && _previousMouseState.RightButton == ButtonState.Released;
    
    public static bool IsMouseRightButtonUp => _currentMouseState.RightButton == ButtonState.Released
                                               && _previousMouseState.RightButton == ButtonState.Pressed;
    
    public static bool IsMouseRightButtonHold => _currentMouseState.RightButton == ButtonState.Pressed
                                                 && _previousMouseState.RightButton == ButtonState.Pressed;
    
    public static bool IsMouseMiddleButtonDown => _currentMouseState.MiddleButton == ButtonState.Pressed
                                                  && _previousMouseState.MiddleButton == ButtonState.Released;
    
    public static bool IsMouseMiddleButtonUp => _currentMouseState.MiddleButton == ButtonState.Released
                                                && _previousMouseState.MiddleButton == ButtonState.Pressed;
    
    public static bool IsMouseMiddleButtonHold => _currentMouseState.MiddleButton == ButtonState.Pressed
                                                  && _previousMouseState.MiddleButton == ButtonState.Pressed;
    
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
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
        
        var keys = Keyboard.GetState().GetPressedKeys();
        
        foreach (var key in keys)
        {
            if (_states.ContainsKey(key))
            {
                if (_states[key] == State.DownEvent)
                {
                    _states[key] = State.DownHold;
                    KeyPressing?.Invoke(key);
                }
                if (_states[key] == State.UpEvent || _states[key] == State.UpHold)
                {
                    _states[key] = State.DownEvent;
                    KeyPressed?.Invoke(key);
                    KeyPressing?.Invoke(key);
                }
            }
            
            if (!_states.ContainsKey(key))
            {
                _states.TryAdd(key, State.DownEvent);
                KeyPressed?.Invoke(key);
                KeyPressing?.Invoke(key);
            }
        }

        keys = _states.Keys.Except(keys).ToArray();
        foreach (var key in keys)
        {
            if (_states[key] == State.DownEvent || _states[key] == State.DownHold)
            {
                _states[key] = State.UpEvent;
                KeyUnPressed?.Invoke(key);
            }

            if (_states[key] == State.UpEvent)
            {
                _states[key] = State.UpHold;
            }
        }
    }
}