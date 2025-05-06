using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.Graphics;

public class Camera(Viewport viewport)
{
    public static Camera Main { get; internal set; }
    
    public float Zoom
    {
        get => _zoom;
        set
        {
            if (_zoom == value) return;
            _zoom = MathF.Max(value, 0.01f);
            _isDirty = true;
        }
    }
    
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            _isDirty = true;
        }
    }
    
    public float Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation == value) return;
            _rotation = value;
            _isDirty = true;
        }
    }

    public Vector2 ViewportCenter => new(_viewport.Width * 0.5f, _viewport.Height * 0.5f);
    
    private float _zoom = 1;
    private Vector2 _position = Vector2.Zero;
    private float _rotation = 0;
    private Matrix _transform = Matrix.Identity;
    private Viewport _viewport = viewport;
    private bool _isDirty = true;

    public void UpdateViewport(Viewport viewport)
    {
        if (_viewport.Width == viewport.Width && _viewport.Height == viewport.Height) return;
        
        _viewport = viewport;
        _isDirty = true;
    }

    public Matrix GetViewMatrix()
    {
        if (!_isDirty) return _transform;

        _transform =
            Matrix.CreateTranslation(-_position.X, _position.Y, 0) *
            Matrix.CreateRotationZ(_rotation) *
            Matrix.CreateScale(_zoom, _zoom, 1) *
            Matrix.CreateTranslation(ViewportCenter.X, ViewportCenter.Y, 0);
        _isDirty = false;

        return _transform;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix()));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, GetViewMatrix());
    }
    
    public void AdjustZoom(float zoomAmount, Vector2? focusPointScreen = null)
    {
        Zoom += zoomAmount;

        // Если указана точка фокуса, корректируем позицию, чтобы она осталась на месте
        if (!focusPointScreen.HasValue) return;
        var focusWorldBefore = ScreenToWorld(focusPointScreen.Value);
        GetViewMatrix();
        var focusWorldAfter = ScreenToWorld(focusPointScreen.Value);
        Position += focusWorldBefore - focusWorldAfter;
        _isDirty = true;
    }
}