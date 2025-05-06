using DCFApixels.DragonECS;
using Game.Modules;
using Karpik.StatAndAbilities;
using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;
using Microsoft.Xna.Framework;

namespace EngineTest;

public class TestCollision
{
    private EcsPipeline.Builder _builder;
    private EcsPipeline _pipeline;
    
    [SetUp]
    public void Setup()
    {
        
    }
    
    [Test]
    public void When3Entities_AndCollision_Thenbubububebebe()
    {
        //Action
        _builder = EcsPipeline.New()
            .Inject(Worlds.Instance.World)
            .Inject(Worlds.Instance.EventWorld)
            .Inject(Worlds.Instance.MetaWorld);
        
        _builder
            .AddRunner<EcsPausableRunner>()
            .AddRunner<PausableLateRunner>()
            .AddRunner<EcsFixedRunRunner>()
            .AddRunner<GamePreInit>()
            .AddRunner<GameInit>()
            .AddModule(new PhysicsModule())
            ;
        
        _pipeline = _builder.BuildAndInit();

        var player = SpawnPlayer();
        player.Get<Transform>().Position = new Vector2(100, 100);
        var enemy1 = SpawnEnemy();
        enemy1.Get<Transform>().Position = new Vector2(101, 100);
        var enemy2 = SpawnEnemy();
        enemy2.Get<Transform>().Position = new Vector2(102, 100);
        
        Time.Update(1 / 60f);
        Update();
        Update();
        Update();
        Update();
        Update();

        //Condition

        //Result
    }

    private void Update()
    {
        _pipeline.GetRunner<EcsFixedRunRunner>().FixedRun();
    }

    private entlong SpawnPlayer()
    {
        var world = Worlds.Instance.World;
        var e = world.NewEntityLong();
        e.Add<Transform>() = new Transform()
        {
            Position = new Vector2(100, 100),
            Scale = Vector2.One
        };
        e.Add<RigidBody>() = new RigidBody(10, 0, 0, 0, 0, RigidBody.BodyType.Dynamic, RigidBody.CollisionMode.ContinuousSpeculative);
        e.Add<ColliderBox>() = new ColliderBox()
        {
            Size = new Vector2(32, 32),
            IsTrigger = false
        };
        e.Add<Speed>() = new Speed()
        {
            BaseValue = 500
        };
        e.Add<Player>();
        e.Add<Health>() = new Health()
        {
            Value = 100,
            Max = new DefaultStat()
            {
                BaseValue = 100
            },
            Min = new DefaultStat()
            {
                BaseValue = 0
            }
        };
        return e;
    }
    
    private entlong SpawnEnemy()
    {
        var world = Worlds.Instance.World;
        var e = world.NewEntityLong();
        e.Add<Transform>() = new Transform()
        {
            Position = new Vector2(100, 100),
            Scale = Vector2.One
        };
        e.Add<RigidBody>() = new RigidBody(10, 0, 0, 0, 0, RigidBody.BodyType.Dynamic, RigidBody.CollisionMode.ContinuousSpeculative);
        e.Add<ColliderBox>() = new ColliderBox()
        {
            Size = new Vector2(32, 32),
            IsTrigger = false
        };
        e.Add<Speed>() = new Speed()
        {
            BaseValue = 200
        };
        e.Add<Enemy>();
        e.Add<FollowPlayer>();
        e.Add<Health>() = new Health()
        {
            Value = 100,
            Max = new DefaultStat()
            {
                BaseValue = 100
            },
            Min = new DefaultStat()
            {
                BaseValue = 0
            }
        };
        e.Add<DealDamageOnContact>();
        return e;
    }
}