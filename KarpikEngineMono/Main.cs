using System.Diagnostics;
using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.SaveLoad;

namespace KarpikEngineMonoGame;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ImGuiRenderer _imGuiRenderer;

    private EcsPipeline.Builder _builder;
    private EcsPipeline _pipeline;
    private double _fixedTimer = 0;
    private Stopwatch _stopWatch = new();

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
        _stopWatch.Start();
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
        Time.Update(_stopWatch.Elapsed.TotalSeconds);
        _stopWatch.Restart();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _fixedTimer += Time.DeltaTime;
        
        _pipeline.Run();
        if (_fixedTimer >= Time.FixedDeltaTime)
        {
            _pipeline.GetRunner<EcsFixedRunRunner>().FixedRun();
            _fixedTimer -= Time.FixedDeltaTime;
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
        _builder
            .AddRunner<PausableRunner>()
            .AddRunner<PausableLateRunner>()
            .AddRunner<EcsFixedRunRunner>()
            .AddRunner<GamePreInit>()
            .AddRunner<GameInit>()
            .AddModule(new VisualModule())
            .AddModule(new InputModule())
            .AddModule(new PhysicsModule())
            .AddModule(new PhysicsModule());
    }
}