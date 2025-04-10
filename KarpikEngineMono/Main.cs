using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngine.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using KarpikEngine;
using KarpikEngine.Modules.EcsCore;
using KarpikEngine.Modules.EcsRunners;
using KarpikEngine.Modules.SaveLoad;

namespace KarpikEngineMonoGame;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ImGuiRenderer _imGuiRenderer;

    private EcsPipeline.Builder _builder;
    private EcsPipeline _pipeline;
    private double _timer = 0;
    private double _fixedTime = 1.0 / 50;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _builder = EcsPipeline.New()
            .Inject(Worlds.Instance.World)
            .Inject(Worlds.Instance.EventWorld)
            .Inject(Worlds.Instance.MetaWorld);

        Loader.Manager = Content;
        EcsDebug.OnPrint = Console.WriteLine;
    }

    public Main Add(IEcsModule module)
    {
        _builder.AddModule(module);
        return this;
    }

    protected override void Initialize()
    {
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();

        InitEcs();
        Worlds.Instance.Pipeline = _pipeline;

        base.Initialize();
    }

    protected override void BeginRun()
    {
        _pipeline = _builder.BuildAndInit();
        base.BeginRun();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Drawer.SpriteBatch = _spriteBatch;
        _builder.Inject(_spriteBatch);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if (Keyboard.GetState().IsKeyDown(Keys.F1))
        {
            var world = Worlds.Instance.World;
            var e = world.NewEntity();
            world.GetPool<SpriteRenderer>().Add(e) = new SpriteRenderer()
            {
                Texture = Content.Load<Texture2D>("player"),
                Color = Color.White,
                Effect = SpriteEffects.None,
                Layer = 0
            };
            world.GetPool<Transform>().Add(e) = new Transform()
            {
                Position = new Vector2(0, 0),
                Rotation = 0,
                Scale = Vector2.One
            };
        }

        _timer += gameTime.ElapsedGameTime.TotalSeconds;

        _pipeline.Run();
        if (_timer >= _fixedTime)
        {
            _pipeline.GetRunner<EcsFixedRunRunner>().FixedRun();
            _timer -= _fixedTime;
        }
        _pipeline.GetRunner<PausableRunner>().PausableRun();
        _pipeline.GetRunner<PausableLateRunner>().PausableLateRun();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Drawer.Draw();

        base.Draw(gameTime);

        _imGuiRenderer.BeginLayout(gameTime);
        DebugGraphics.Draw();
        _imGuiRenderer.EndLayout();
    }

    private void InitEcs()
    {
        Worlds.Instance.MetaWorld.Get<TimeComponent>() = new TimeComponent()
        {
            Paused = false
        };

        
        _builder.AddRunner<PausableRunner>();
        _builder.AddRunner<PausableLateRunner>();
        _builder.AddRunner<EcsFixedRunRunner>();
        _builder.AddRunner<GamePreInit>();
        _builder.AddRunner<GameInit>();
    }
}