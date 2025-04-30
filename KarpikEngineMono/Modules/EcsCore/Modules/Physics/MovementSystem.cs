using KarpikEngineMono.Modules.EcsRunners;
using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules.EcsCore;

public class MovementSystem : IEcsFixedRun
{
    private const double LinearDampingFactor = 0.01;
    private const double AngularDampingFactor = 0.98;
    private const bool EnableDamping = true;
    
    private class Aspect : EcsAspect
    {
        public EcsPool<Transform> transform = Inc;
        public EcsPool<Velocity> velocity = Opt;
        public EcsPool<Force> force = Inc;
        public EcsPool<RigidBody> rigidBody = Inc;
    }
    
    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void FixedRun()
    {
        if (Time.IsPaused) return;
        
        foreach (var e in _world.Where(out Aspect aspect))
        {
            ref var transform = ref aspect.transform.Get(e);
            ref var velocity = ref aspect.velocity.TryAddOrGet(e);
            ref var force = ref aspect.force.Get(e);
            ref var rigidBody = ref aspect.rigidBody.Get(e);

            if (rigidBody.Type == RigidBody.BodyType.Static)
            {
                force.Direction = Vector2.Zero;
                force.Torque = 0;
                continue;
            }

            if (rigidBody.Type == RigidBody.BodyType.Dynamic)
            {
                if (rigidBody.InverseMass > 0)
                {
                    //a = F / m  =>  Δv = a * Δt = (F * InverseMass) * Δt
                    var linearAcceleration = force.Direction * (float)rigidBody.InverseMass;
                    velocity.Linear += linearAcceleration * (float)Time.FixedDeltaTime;
                }

                if (rigidBody.InverseMomentOfInertia > 0)
                {
                    // Угловое движение: α = τ / I  =>  Δω = α * Δt = (τ * InverseMomentOfInertia) * Δt
                    var angularAcceleration = force.Torque * rigidBody.InverseMomentOfInertia;
                    velocity.Angular += angularAcceleration * Time.FixedDeltaTime;
                }


                if (EnableDamping)
                {
                    //Затухание
                    velocity.Linear *= (float)Math.Pow(LinearDampingFactor, Time.FixedDeltaTime);
                    velocity.Angular *= (float)Math.Pow(AngularDampingFactor, Time.FixedDeltaTime);
                }
            }
            
            //Пропускаем движение для кинематических тел
            
            transform.Position += velocity.Linear * (float)Time.FixedDeltaTime;
            transform.Rotation += NormalizeAngle(velocity.Angular * Time.FixedDeltaTime);
            
            force.Direction = Vector2.Zero;
            force.Torque = 0;
        }
    }
    
    private static double NormalizeAngle(double angle)
    {
        const double _2pi = Math.PI * 2.0f;
        angle %= _2pi;
        if (angle >= MathF.PI) angle -= _2pi;
        else if (angle < -MathF.PI) angle += _2pi;
        return angle;
    }
}