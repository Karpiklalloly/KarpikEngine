using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules.EcsCore;

public struct Velocity : IEcsComponent
{
    public Vector2 Linear;
    public double Angular;
}

public struct Force : IEcsComponent
{
    public Vector2 Direction; // направление силы
    public double Torque; // момент силы
}

[Serializable]
public struct RigidBody : IEcsComponent
{
    public double Mass;
    [JsonIgnore]
    public double InverseMass;
    public double MomentOfInertia;
    [JsonIgnore]
    public double InverseMomentOfInertia;
    public double Restitution;
    public double StaticFriction;
    public double DynamicFriction;
    public BodyType Type;

    [JsonConstructor]
    public RigidBody(double mass, double momentOfInertia, double restitution, double staticFriction, double dynamicFriction, BodyType type)
    {
        Mass = mass;
        InverseMass = mass > 0 ? 1 / mass : 0;
        MomentOfInertia = momentOfInertia;
        InverseMomentOfInertia = momentOfInertia > 0 ? 1 / momentOfInertia : 0;
        Restitution = restitution;
        StaticFriction = staticFriction;
        DynamicFriction = dynamicFriction;
        Type = type;
    }
    
    public enum BodyType
    {
        Static,
        Dynamic,
        Kinematic
    }
}

[Serializable]
public struct ColliderBox : IEcsComponent
{
    public Vector2 Size;
    public Vector2 Offset;
    public double RotationOffset;
    public bool IsTrigger;
}

[Serializable]
public struct ColliderCircle : IEcsComponent
{
    public double Radius;
    public Vector2 Offset;
    public double RotationOffset;
    public bool IsTrigger;
}

public struct Collisions : IEcsComponent
{
    public List<CollisionInfo> Infos;

    public Collisions()
    {
        Infos = new List<CollisionInfo>();
    }
}

public struct CollisionInfo
{
    public entlong Other;
    public Vector2 Normal;
    public double PenetrationDepth;
    public Vector2[] ContactPoints;
}