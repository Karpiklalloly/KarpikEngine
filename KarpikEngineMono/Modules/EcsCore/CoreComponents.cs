using Karpik.DragonECS;
using KarpikEngineMono.Modules.SaveLoad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.EcsCore;

[Serializable]
public struct Player : IEcsTagComponent {}

[Serializable]
public struct Transform : IEcsComponent
{
    public Vector2 Position;
    public double Rotation;
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