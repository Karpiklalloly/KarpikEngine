﻿using Karpik.DragonECS;
using KarpikEngineMono.Modules.EcsRunners;
using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules.EcsCore;

public class CollisionResolutionSystem : IEcsFixedRunOnEvent<CollisionsEvent>
{
    private const float PenetrationCorrectionFactor = 0.6f;
    // Минимальная глубина проникновения, которую стоит исправлять, чтобы избежать лишних вычислений
    private const float PenetrationSlop = 0.01f;
    
    private EcsDefaultWorld _world;
    private EcsEventWorld _eventWorld;
    private EcsPool<CollisionsEvent> _collisions;
    private EcsPool<Transform> _transformPool;
    private EcsPool<Velocity> _velocityPool;
    private EcsPool<RigidBody> _rigidBodyPool;

    public CollisionResolutionSystem()
    {
        _world = Worlds.Instance.World;
        _eventWorld = Worlds.Instance.EventWorld;
        _collisions = _eventWorld.GetPool<CollisionsEvent>();
        _transformPool = _world.GetPool<Transform>();
        _velocityPool = _world.GetPool<Velocity>();
        _rigidBodyPool = _world.GetPool<RigidBody>();
    }
    
    public void RunOnEvent(ref CollisionsEvent evt)
    {
        if (evt.Source.IsNull)
        {
            
        }
        
        ref var collisions1 = ref evt;
        ref var transform1 = ref _transformPool.Get(collisions1.Source.ID);
        ref var velocity1 = ref _velocityPool.TryAddOrGet(collisions1.Source.ID);

        var entityA = collisions1.Source.ID;

        foreach (var collision in collisions1.Infos)
        {
            int entityB = collision.Other.ID;
            
            bool hasAllComponentsA = _transformPool.Has(entityA)
                                     && _velocityPool.Has(entityA) // Velocity не нужна для Static, но нужна для B
                                     && _rigidBodyPool.Has(entityA);

            bool hasAllComponentsB = _transformPool.Has(entityB)
                                     && _velocityPool.Has(entityB)
                                     && _rigidBodyPool.Has(entityB);

            // Если у одного из участников нет нужных компонентов, не можем разрешить столкновение
            // (Это может случиться, если сущность была удалена между детекцией и разрешением)
            if (!hasAllComponentsA || !hasAllComponentsB)
            {
                // Можно залогировать предупреждение
                Console.WriteLine($"[CollisionResolution] Warning: Missing components for collision pair ({entityA}, {entityB}). Skipping.");
                continue;
            }
                
            _rigidBodyPool.TryAddOrGet(entityB);
            _transformPool.TryAddOrGet(entityB);
            _velocityPool.TryAddOrGet(entityB);

            ref var transformA = ref _transformPool.Get(entityA);
            ref var velocityA = ref _velocityPool.Get(entityA);
            ref var rigidBodyA = ref _rigidBodyPool.Get(entityA);

            ref var transformB = ref _transformPool.Get(entityB);
            ref var velocityB = ref _velocityPool.Get(entityB);
            ref var rigidBodyB = ref _rigidBodyPool.Get(entityB);

            // Пропускаем разрешение для двух статических/кинематических объектов
            // (InverseMass будет 0 для них)
            double totalInverseMass = rigidBodyA.InverseMass + rigidBodyB.InverseMass;
            if (Math.Abs(totalInverseMass) < 0.0001f) // Сравнение с плавающей точкой
            {
                continue;
            }

            // --- Разрешение Столкновения ---
            ResolvePenetration(ref transformA, ref rigidBodyA, ref transformB, ref rigidBodyB, collision.Normal, collision.PenetrationDepth, totalInverseMass);
            ResolveVelocity(ref velocityA, ref rigidBodyA, ref velocityB, ref rigidBodyB, collision.Normal, totalInverseMass);

            // Компонент CollisionInfoComponent НЕ удаляется здесь.
            // Он будет удален позже системой CleanupSystem.
        }
    }
    
    // --- Разрешение Проникновения (Positional Correction) ---
    private void ResolvePenetration(
        ref Transform transformA, ref RigidBody rigidBodyA,
        ref Transform transformB, ref RigidBody rigidBodyB,
        Vector2 normal, double penetrationDepth, double totalInverseMass)
    {
        // Вычисляем величину коррекции, немного уменьшенную для стабильности
        // И вычитаем "slop", чтобы не исправлять очень маленькие перекрытия
        double correctionMagnitude = Math.Max(penetrationDepth - PenetrationSlop, 0.0f)
                                     / totalInverseMass // Распределяем по массе
                                     * PenetrationCorrectionFactor; // Уменьшаем для стабильности

        Vector2 correctionVector = normal * (float)correctionMagnitude;

        // Применяем коррекцию к позициям
        // Статические/кинематические тела (InverseMass = 0) не сдвинутся
        transformA.Position += correctionVector * (float)rigidBodyA.InverseMass;
        transformB.Position -= correctionVector * (float)rigidBodyB.InverseMass;
    }

    // --- Разрешение Скоростей (Impulse Response) ---
    // УПРОЩЕННАЯ ВЕРСИЯ: без вращения и трения
    private void ResolveVelocity(
        ref Velocity velocityA, ref RigidBody rigidBodyA,
        ref Velocity velocityB, ref RigidBody rigidBodyB,
        Vector2 normal, double totalInverseMass)
    {
        // 1. Вычисляем относительную скорость
        Vector2 relativeVelocity = velocityB.Linear - velocityA.Linear;

        // 2. Вычисляем скорость вдоль нормали столкновения
        float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

        // 3. Если скорости вдоль нормали > 0, объекты уже расходятся, импульс не нужен
        if (velocityAlongNormal > 0)
        {
            return;
        }

        // 4. Вычисляем коэффициент восстановления (упругости)
        // Используем минимальный из двух объектов
        double e = Math.Min(rigidBodyA.Restitution, rigidBodyB.Restitution);

        // 5. Вычисляем величину импульса (скаляр)
        // Формула: j = -(1 + e) * Vrn / (1/mA + 1/mB)
        double j = -(1.0f + e) * velocityAlongNormal;
        j /= totalInverseMass; // Делим на суммарную обратную массу

        // 6. Вычисляем вектор импульса
        Vector2 impulse = normal * (float)j;

        // 7. Применяем импульс к скоростям
        // Импульс делится пропорционально обратной массе
        // Статические/кинематические тела (InverseMass = 0) не изменят скорость
        velocityA.Linear -= impulse * (float)rigidBodyA.InverseMass;
        velocityB.Linear += impulse * (float)rigidBodyB.InverseMass;

        // --- TODO: Реализовать расчет и применение импульса трения ---
        // Это потребует:
        // - Расчета тангенциальной скорости (перпендикулярно нормали)
        // - Вычисления максимального импульса трения (зависит от нормального импульса 'j' и коэффициентов трения)
        // - Вычисления тангенциального импульса (противоположно тангенциальной скорости, но не более макс. трения)
        // - Применения тангенциального импульса к скоростям

        // --- TODO: Реализовать учет вращения ---
        // Это потребует:
        // - Информации о точке контакта (ContactPoint из CollisionInfoComponent)
        // - Расчета относительной скорости в точке контакта (учитывая AngularVelocity)
        // - Расчета влияния импульса (нормального и трения) на AngularVelocity через момент импульса (зависит от вектора от центра масс до точки контакта и InverseMomentOfInertia)
    }
}