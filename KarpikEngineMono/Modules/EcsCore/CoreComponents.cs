using Karpik.DragonECS;
using KarpikEngineMono.Modules.SaveLoad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.EcsCore;

public struct TimeComponent : IEcsComponent
{
    public bool Paused;
}

[Serializable]
public struct Player : IEcsTagComponent {}

[Serializable]
public struct Transform : IEcsComponent
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale; 
}

[Serializable]
public struct SpriteRenderer : IEcsComponent
{
    [JsonIgnore] public Texture2D Texture;
    public Color Color;
    public SpriteEffects Effect;
    public float Layer;
    public string TexturePath
    {
        readonly get => _path;
        set
        {
            if (value == _path) return;
            _path = value;
            Texture = Loader.LoadTexture(value);
        }
    }

    private string _path;
}

public struct MoveTo : IEcsComponentRequest
{
    public Vector2 Position;

    public entlong Source { get; set; }
    public entlong Target { get; set; }
}

public struct MoveDirection : IEcsComponentEvent
{
    public Vector2 Direction;

    public entlong Source { get; set; }
    public entlong Target { get; set; }
}

[Serializable]
public struct Speed : IEcsComponent
{
    public double Value;
}