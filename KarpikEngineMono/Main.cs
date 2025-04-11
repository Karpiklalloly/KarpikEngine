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
        Time.Update(gameTime.ElapsedGameTime.TotalSeconds);
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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
        _builder.AddModule(new MovementModule());
        _builder.AddModule(new VisualModule());
        _builder.AddModule(new InputModule());
    }
}