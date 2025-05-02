using System.Diagnostics;
using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.VisualElements;

namespace KarpikEngineMonoGame;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
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
        UI.Window = Window;
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

        UI.Root = new VisualElement(Window.ClientBounds.Size.ToVector2())
        {
            OffsetPosition = Vector2.Zero,
            Anchor = Anchor.TopLeft,
            Stretch = StretchMode.Both,
            Pivot = Vector2.Zero
        };
        UI.DefaultFont = Loader.Load<SpriteFont>("DefaultFont");
        base.Initialize();
    }

    protected override void BeginRun()
    {
        _pipeline = _builder.BuildAndInit();
        _pipeline.GetRunner<GameInit>().InitGame();
        _stopWatch.Start();
        base.BeginRun();
    }

    protected override void LoadContent()
    {
        Drawer.SpriteBatch = new SpriteBatch(GraphicsDevice);
        Drawer.Window = Window;
        UI.UISpriteBatch = new SpriteBatch(GraphicsDevice);
        _builder.Inject(Drawer.SpriteBatch);
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(_stopWatch.Elapsed.TotalSeconds);
        _stopWatch.Restart();

        _fixedTimer += Time.DeltaTime;
        
        _pipeline.Run();
        if (_fixedTimer >= Time.FixedDeltaTime)
        {
            _pipeline.GetRunner<EcsFixedRunRunner>().FixedRun();
            _fixedTimer -= Time.FixedDeltaTime;
        }
        _pipeline.GetRunner<PausableRunner>().PausableRun();
        _pipeline.GetRunner<PausableLateRunner>().PausableLateRun();
        
        UI.Update();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Drawer.Draw();
        UI.Draw();
        
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
            .AddModule(new PhysicsModule());
    }
}