using System;
using DCFApixels.DragonECS;
using KarpikEngine.Modules.SaveLoad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KarpikEngine.Modules.EcsCore;

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
}