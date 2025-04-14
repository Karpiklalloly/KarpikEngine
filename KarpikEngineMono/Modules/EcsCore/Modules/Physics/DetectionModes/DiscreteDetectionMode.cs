using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules.EcsCore;

public class DiscreteDetectionMode : IDetectionMode
{
    private class BoxAspect : EcsAspect
    {
        public EcsPool<ColliderBox> box = Inc;
        public EcsPool<Transform> transform = Inc;
        public EcsPool<RigidBody> rigidBody = Inc;
    }
    
    private class CircleAspect : EcsAspect
    {
        public EcsPool<ColliderCircle> circle = Inc;
        public EcsPool<Transform> transform = Inc;
        public EcsPool<RigidBody> rigidBody = Inc;
    }
    
    private struct ColliderWorldData : IEcsComponent
    {
        public int Entity;
        public Transform Transform;
        public Velocity Velocity;
        public Vector2 WorldPosition;
        public double WorldRotation;
        public BoundingBox AABB;
        public bool IsTrigger;
        public RigidBody RigidBody;
        public Type Shape;

        public ColliderBox Box;
        public ColliderCircle Circle;
        
        public enum Type
        {
            Box,
            Circle
        }
    }
    
    public struct CollisionResult
    {
        public bool Intersects;
        public Vector2 Normal; // Нормаль указывает ОТ другого объекта К текущему
        public double PenetrationDepth;

        public static readonly CollisionResult NoCollision = new CollisionResult { Intersects = false };
    }
    
    private const double CellSize = 32.0;
    private readonly Dictionary<(int, int), List<int>> _grid = new();
    private readonly HashSet<(int, int)> _testedPairs = new();
    private List<ColliderWorldData> _colliderDataCache = new();
    
    private EcsDefaultWorld _world;
    private EcsPool<Collisions> _collisionsPool;
    private EcsPool<Velocity> _velocityPool;

    public DiscreteDetectionMode()
    {
        _world = Worlds.Instance.World;
        _collisionsPool = _world.GetPool<Collisions>();
        _velocityPool = _world.GetPool<Velocity>();
    }
    
    public void Collect()
    {
        _grid.Clear();
        _testedPairs.Clear();

        _colliderDataCache.Clear();
        
        Box();
        Circle();
    }

    public void Detect()
    {
        foreach (var cellEntities in _grid.Values)
        {
            if (cellEntities.Count < 2) continue; // Не с чем сталкиваться в этой ячейке

            // Проверяем все уникальные пары внутри ячейки
            for (int i = 0; i < cellEntities.Count; i++)
            {
                for (int j = i + 1; j < cellEntities.Count; j++)
                {
                    int entityAId = cellEntities[i];
                    int entityBId = cellEntities[j];

                    // Упорядочиваем ID для ключа в _checkedPairs
                    int id1 = Math.Min(entityAId, entityBId);
                    int id2 = Math.Max(entityAId, entityBId);

                    // Проверяем, не обрабатывали ли уже эту пару
                    if (_testedPairs.Add((id1, id2)))
                    {
                        // Получаем закэшированные данные коллайдеров
                        // (Используем FindIndex или словарь для быстрого поиска по ID, если entities много)
                        var dataA = _colliderDataCache.Find(d => d.Entity == entityAId);
                        var dataB = _colliderDataCache.Find(d => d.Entity == entityBId);

                        // Оптимизация: Дополнительная проверка AABB перед детальной проверкой
                        // (Технически, если они в одной ячейке, их AABB *могут* пересекаться,
                        // но эта проверка быстра и может отсеять ложные срабатывания сетки)
                        if (!DoAABBsOverlap(dataA.AABB, dataB.AABB))
                        {
                            continue;
                        }

                        // --- Narrowphase ---
                        CollisionResult result = CheckCollision(ref dataA, ref dataB);

                        if (result.Intersects)
                        {
                            ProcessCollision(entityAId, entityBId, ref dataA, ref dataB, result);
                        }
                    }
                }
            }
        }
    }
    
    private void Box()
    {
        foreach (var e in _world.Where(out BoxAspect a))
        {
            ref var box = ref a.box.Get(e);
            ref var transform = ref a.transform.Get(e);
            ref var rigidBody = ref a.rigidBody.Get(e);
            ref var velocity = ref _velocityPool.TryAddOrGet(e);

            var worldData = new ColliderWorldData
            {
                Entity = e,
                Transform = transform,
                Velocity = velocity,
                WorldPosition = transform.Position,
                WorldRotation = transform.Rotation,
                Shape = ColliderWorldData.Type.Box,
                RigidBody = rigidBody,
                Box = box,
                IsTrigger = box.IsTrigger
            };

            worldData.AABB = CalculateWorldAABB(ref worldData);
            
            AddToGrid(worldData.AABB, e);
            _colliderDataCache.Add(worldData);
        }
    }

    private void Circle()
    {
        foreach (var e in _world.Where(out CircleAspect a))
        {
            ref var circle = ref a.circle.Get(e);
            ref var transform = ref a.transform.Get(e);
            ref var rigidBody = ref a.rigidBody.Get(e);
            ref var velocity = ref _velocityPool.TryAddOrGet(e);

            var worldData = new ColliderWorldData
            {
                Entity = e,
                Transform = transform,
                Velocity = velocity,
                WorldPosition = transform.Position,
                WorldRotation = transform.Rotation,
                Shape = ColliderWorldData.Type.Circle,
                RigidBody = rigidBody,
                Circle = circle,
                IsTrigger = circle.IsTrigger
            };

            worldData.AABB = CalculateWorldAABB(ref worldData);
            
            AddToGrid(worldData.AABB, e);
        }
    }
    
    private BoundingBox CalculateWorldAABB(ref ColliderWorldData data)
    {
        // Важно учитывать и позицию, и вращение!
        switch (data.Shape)
        {
            case ColliderWorldData.Type.Circle:
                double radius = data.Circle.Radius; // Пример приведения
                return new BoundingBox(
                    new Vector3(data.WorldPosition - new Vector2((float)radius), 0),
                    new Vector3(data.WorldPosition + new Vector2((float)radius), 0)
                );
            case ColliderWorldData.Type.Box:
                var boxCollider = data.Box; // Пример
                var halfExtents = boxCollider.Size * 0.5f;
                // Для AABB повернутого прямоугольника нужно найти min/max координат его вершин
                var transform = Matrix.CreateRotationZ((float)data.WorldRotation) * Matrix.CreateTranslation(data.WorldPosition.X, data.WorldPosition.Y, 0);
                var p1 = Vector2.Transform(new Vector2(-halfExtents.X, -halfExtents.Y), transform);
                var p2 = Vector2.Transform(new Vector2( halfExtents.X, -halfExtents.Y), transform);
                var p3 = Vector2.Transform(new Vector2( halfExtents.X,  halfExtents.Y), transform);
                var p4 = Vector2.Transform(new Vector2(-halfExtents.X,  halfExtents.Y), transform);
                var min = Vector2.Min(Vector2.Min(p1, p2), Vector2.Min(p3, p4));
                var max = Vector2.Max(Vector2.Max(p1, p2), Vector2.Max(p3, p4));
                return new BoundingBox(new Vector3(min, 0), new Vector3(max, 0));
            default:
                return new BoundingBox(); // Или бросить исключение
        }
    }
    
    // Добавляет Entity во все ячейки сетки, которые пересекает его AABB
    private void AddToGrid(BoundingBox aabb, int entityId)
    {
        int minX = (int)Math.Floor(aabb.Min.X / CellSize);
        int minY = (int)Math.Floor(aabb.Min.Y / CellSize);
        int maxX = (int)Math.Floor(aabb.Max.X / CellSize);
        int maxY = (int)Math.Floor(aabb.Max.Y / CellSize);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var cellKey = (x, y);
                if (!_grid.TryGetValue(cellKey, out var entityList))
                {
                    entityList = new List<int>(4); // Начнем с малого размера
                    _grid[cellKey] = entityList;
                }
                entityList.Add(entityId);
            }
        }
    }
    
    // Простая проверка пересечения AABB
    private bool DoAABBsOverlap(BoundingBox a, BoundingBox b)
    {
        return a.Max.X > b.Min.X && a.Min.X < b.Max.X &&
               a.Max.Y > b.Min.Y && a.Min.Y < b.Max.Y;
    }
    
    // --- Narrowphase: Главный метод выбора ---
    private CollisionResult CheckCollision(ref ColliderWorldData dataA, ref ColliderWorldData dataB)
    {
        var modeA = dataA.RigidBody.Mode;
        var modeB = dataB.RigidBody.Mode;
        
        var discreteResult = StandardNarrowPhaseCheck(ref dataA, ref dataB);
        if (discreteResult.Intersects)
        {
            return discreteResult;
        }
        
        // --- Шаг 2: Если нет текущего пересечения, выполняем спекулятивную проверку, ---
        // --- если ХОТЯ БЫ один объект не Discrete (и не оба Static).       ---
        bool needsSpeculativeCheck = (modeA != RigidBody.CollisionMode.Discrete || modeB != RigidBody.CollisionMode.Discrete)
                                     && !(dataA.RigidBody.Type == RigidBody.BodyType.Static && dataB.RigidBody.Type == RigidBody.BodyType.Static);
        
        if (!needsSpeculativeCheck)
        {
            return CollisionResult.NoCollision; // Возвращаем результат дискретной проверки (нет пересечения)
        }
        
        // --- УПРОЩЕННАЯ СПЕКУЛЯТИВНАЯ ЛОГИКА: Проверка пересечения в *будущих* позициях ---
        // ВНИМАНИЕ: Это сильное упрощение! Настоящий спекулятивный режим использует
        // более сложные методы (GJK/EPA с раздутием/суммой Минковского).

        // 1. Вычисляем предполагаемые будущие позиции
        Vector2 futurePosA = dataA.WorldPosition + dataA.Velocity.Linear * (float)Time.FixedDeltaTime;
        Vector2 futurePosB = dataB.WorldPosition + dataB.Velocity.Linear * (float)Time.FixedDeltaTime;
        // (Вращение для простоты здесь не учитываем для будущей позиции,
        // настоящая реализация должна учитывать и его)

        // 2. Создаем временные данные для будущих позиций
        // (Это неэффективно, но для демонстрации идеи)
        ColliderWorldData futureDataA = dataA; // Копируем текущие данные
        futureDataA.WorldPosition = futurePosA;
        futureDataA.AABB = CalculateWorldAABB(ref futureDataA); // Пересчитываем AABB для новой позиции

        ColliderWorldData futureDataB = dataB;
        futureDataB.WorldPosition = futurePosB;
        futureDataB.AABB = CalculateWorldAABB(ref futureDataB);

        // 3. Выполняем стандартную проверку пересечения для *будущих* данных
        // Сначала быстрая проверка AABB в будущем
        if (!DoAABBsOverlap(futureDataA.AABB, futureDataB.AABB))
        {
            return CollisionResult.NoCollision; // Даже в будущем AABB не пересекаются
        }

        // Выполняем детальную проверку для будущих позиций/ориентаций
        CollisionResult futureResult = StandardNarrowPhaseCheck(ref futureDataA, ref futureDataB);

        // 4. Если в БУДУЩЕМ есть пересечение, возвращаем этот результат КАК ТЕКУЩИЙ КОНТАКТ
        if (futureResult.Intersects)
        {
            // Мы нашли спекулятивный контакт. Возвращаем информацию о нем.
            // Нормаль и глубина взяты из расчета для будущих позиций.
            // Это не идеально точно, но дает сигнал резолверу растолкнуть объекты.
            // Console.WriteLine($"Speculative contact between {dataA.EntityId} and {dataB.EntityId}");
            return futureResult;
        }

        // Если и в будущем нет пересечения
        return CollisionResult.NoCollision;
    }

    private CollisionResult StandardNarrowPhaseCheck(ref ColliderWorldData dataA, ref ColliderWorldData dataB)
    {
        var typeA = dataA.Shape;
        var typeB = dataB.Shape;

        // Выбираем нужную функцию проверки на основе типов коллайдеров
        // Важно обрабатывать оба порядка (A vs B и B vs A) или нормализовать пару
        if (typeA == ColliderWorldData.Type.Circle && typeB == ColliderWorldData.Type.Circle)
        {
            return CheckCircleCircle(ref dataA, ref dataB);
        }
        if (typeA == ColliderWorldData.Type.Box && typeB == ColliderWorldData.Type.Box)
        {
            // TODO: Для повернутых ящиков нужна проверка SAT или OBB vs OBB
            // Пока можно сделать только AABB vs AABB (быстро, но неточно для повернутых)
            return CheckAABB_AABB(ref dataA, ref dataB); // Использует только AABB
        }
        if (typeA == ColliderWorldData.Type.Circle && typeB == ColliderWorldData.Type.Box)
        {
            return CheckCircleBox(ref dataA, ref dataB);
        }
        if (typeA == ColliderWorldData.Type.Box && typeB == ColliderWorldData.Type.Circle)
        {
            // Переворачиваем результат Circle vs Box
            var result = CheckCircleBox(ref dataB, ref dataA); // Вызываем с B как кругом, A как коробкой
            result.Normal = -result.Normal; // Инвертируем нормаль
            return result;
        }
        // TODO: Добавить проверки для полигонов (Polygon vs Polygon, Polygon vs Circle, Polygon vs Box) с использованием SAT

        // Если пара типов не поддерживается
        return CollisionResult.NoCollision;
    }
    
    private CollisionResult CheckCircleCircle(ref ColliderWorldData dataA, ref ColliderWorldData dataB)
    {
        var circleA = dataA.Circle;
        var circleB = dataB.Circle;
        double radiusA = circleA.Radius;
        double radiusB = circleB.Radius;
        Vector2 distanceVec = dataB.WorldPosition - dataA.WorldPosition;
        float distanceSq = distanceVec.LengthSquared();
        double radiiSum = radiusA + radiusB;
        double radiiSumSq = radiiSum * radiiSum;

        if (distanceSq >= radiiSumSq)
        {
            return CollisionResult.NoCollision;
        }

        float distance = (float)Math.Sqrt(distanceSq);
        Vector2 normal = distance > 0.0001f ? distanceVec / distance : new Vector2(0, 1); // Нормаль от B к A
        double penetration = radiiSum - distance;

        return new CollisionResult { Intersects = true, Normal = normal, PenetrationDepth = penetration };
    }
    
    // Проверка AABB vs AABB (неточна для повернутых прямоугольников!)
    private CollisionResult CheckAABB_AABB(ref ColliderWorldData dataA, ref ColliderWorldData dataB)
    {
        BoundingBox aabbA = dataA.AABB;
        BoundingBox aabbB = dataB.AABB;

        Vector2 distance = dataB.WorldPosition - dataA.WorldPosition; // Вектор между центрами (примерно)
        Vector2 halfExtents1 = new Vector2(aabbA.Max.X - aabbA.Min.X, aabbA.Max.Y - aabbA.Min.Y) * 0.5f;
        Vector2 halfExtents2 = new Vector2(aabbB.Max.X - aabbB.Min.X, aabbB.Max.Y - aabbB.Min.Y) * 0.5f;
        float overlapX = (halfExtents1.X + halfExtents2.X) - Math.Abs(distance.X);
        float overlapY = (halfExtents1.Y + halfExtents2.Y) - Math.Abs(distance.Y);

        if (overlapX <= 0 || overlapY <= 0)
        {
            return CollisionResult.NoCollision;
        }

        // Определяем нормаль и глубину по наименьшему перекрытию
        Vector2 normal;
        float penetration;
        if (overlapX < overlapY)
        {
            penetration = overlapX;
            normal = new Vector2(Math.Sign(-distance.X), 0); // Нормаль от B к A
            if (normal.X == 0) normal.X = 1; // Избегаем нулевой нормали, если центры точно друг над другом по X
        }
        else
        {
            penetration = overlapY;
            normal = new Vector2(0, Math.Sign(-distance.Y)); // Нормаль от B к A
            if (normal.Y == 0) normal.Y = 1; // Избегаем нулевой нормали, если центры точно друг над другом по Y
        }

        return new CollisionResult { Intersects = true, Normal = normal, PenetrationDepth = penetration };
    }
    
    private CollisionResult CheckCircleBox(ref ColliderWorldData dataCircle, ref ColliderWorldData dataBox)
    {
        var circleCollider = dataCircle.Circle;
        var boxCollider = dataBox.Box; // Предполагаем BoxColliderComponent
        double radius = circleCollider.Radius;
        Vector2 circlePos = dataCircle.WorldPosition;
        Vector2 boxPos = dataBox.WorldPosition;
        float boxRotation = (float)dataBox.WorldRotation;
        Vector2 boxHalfExtents = boxCollider.Size * 0.5f;

        // --- Алгоритм Circle vs OBB (Oriented Bounding Box) ---
        // 1. Переводим центр круга в локальную систему координат прямоугольника
        Matrix worldToBox = Matrix.Identity;
        var m = Matrix.CreateRotationZ(boxRotation) * Matrix.CreateTranslation(new Vector3(boxPos, 0));
        Matrix.Invert(ref m, out worldToBox);
        Vector2 circlePosLocal = Vector2.Transform(circlePos, worldToBox);

        // 2. Находим ближайшую точку на прямоугольнике к центру круга (в локальных координатах)
        Vector2 closestPointLocal;
        closestPointLocal.X = Math.Clamp(circlePosLocal.X, -boxHalfExtents.X, boxHalfExtents.X);
        closestPointLocal.Y = Math.Clamp(circlePosLocal.Y, -boxHalfExtents.Y, boxHalfExtents.Y);

        // 3. Проверяем расстояние от центра круга до этой ближайшей точки
        Vector2 diff = circlePosLocal - closestPointLocal;
        float distanceSq = diff.LengthSquared();

        if (distanceSq >= radius * radius)
        {
            return CollisionResult.NoCollision; // Круг слишком далеко
        }

        // 4. Рассчитываем нормаль и глубину
        // Переводим ближайшую точку обратно в мировые координаты
        Matrix boxToWorld = Matrix.CreateRotationZ(boxRotation) * Matrix.CreateTranslation(new Vector3(boxPos, 0));
        Vector2 closestPointWorld = Vector2.Transform(closestPointLocal, boxToWorld);

        Vector2 normal = circlePos - closestPointWorld; // Вектор от ближайшей точки на коробке к центру круга
        float distance = normal.Length();
        double penetration = radius - distance;

        // Нормализуем нормаль (указывает от коробки к кругу)
        if (distance > 0.0001f)
        {
            normal /= distance;
        }
        else
        {
            // Круг точно в центре ближайшей точки (вероятно, на ребре или внутри)
            // Используем разницу центров в локальных координатах для определения нормали
            if (Math.Abs(circlePosLocal.X) > Math.Abs(circlePosLocal.Y))
                normal = new Vector2(Math.Sign(circlePosLocal.X), 0); // Нормаль по X
            else
                normal = new Vector2(0, Math.Sign(circlePosLocal.Y)); // Нормаль по Y
            // Переводим нормаль в мировые координаты
            normal = Vector2.TransformNormal(normal, Matrix.CreateRotationZ(boxRotation));
        }

        return new CollisionResult { Intersects = true, Normal = normal, PenetrationDepth = penetration };
    }
    
    // --- Обработка результата столкновения ---
    private void ProcessCollision(int entityAId, int entityBId, ref ColliderWorldData dataA, ref ColliderWorldData dataB, CollisionResult result)
    {
        // Проверяем флаги триггеров
        bool isTriggerA = dataA.IsTrigger;
        bool isTriggerB = dataB.IsTrigger;

        if (isTriggerA || isTriggerB)
        {
            // --- Обработка триггера ---
            // TODO: Реализовать систему событий или добавить компоненты OnTriggerEnter/Stay/Exit
            Console.WriteLine($"Trigger between {entityAId} and {entityBId}");
        }
        else
        {
            // --- Обработка физического столкновения ---
            // Проверяем, не являются ли оба объекта статическими
            bool isStaticA = dataA.RigidBody.Type == RigidBody.BodyType.Static;
            bool isStaticB = dataB.RigidBody.Type == RigidBody.BodyType.Static;
            if (isStaticA && isStaticB)
            {
                return; // Игнорируем столкновения Static vs Static
            }

            // Нормаль в CollisionInfo указывает ОТ ДРУГОГО объекта К ТЕКУЩЕМУ
            ref var c1 = ref _collisionsPool.TryAddOrGet(entityAId);
            c1.Infos ??= [];
            c1.Infos.Add(new CollisionInfo()
            {
                Other = _world.GetEntityLong(entityBId),
                Normal = result.Normal, // Нормаль от B к A
                PenetrationDepth = result.PenetrationDepth
            });
            
            ref var c2 = ref _collisionsPool.TryAddOrGet(entityBId);
            c2.Infos ??= [];
            c2.Infos.Add(new CollisionInfo()
            {
                Other = _world.GetEntityLong(entityAId),
                Normal = -result.Normal, // Нормаль от A к B
                PenetrationDepth = result.PenetrationDepth
            });
            
            //Console.WriteLine($"Collision between {entityAId} and {entityBId}, Depth: {result.PenetrationDepth}");
        }
    }
}