﻿using System.Diagnostics;
using Karpik.Vampire.Scripts.DragonECS.CustomRunners;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsCore.Modules.Modding;
using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.Modding;
using KarpikEngineMono.Modules.VisualElements;
using MoonSharp.Interpreter;

namespace KarpikEngineMonoGame;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private ImGuiRenderer _imGuiRenderer;

    private EcsPipeline.Builder _builder;
    private EcsPipeline _pipeline;
    private double _fixedTimer = 0;
    private Stopwatch _stopWatch = new();
    private ModManager _modManager = ModManager.Instance;

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
        IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.ApplyChanges();
        SuppressDraw();
        _modManager.LoadMods("Mods");
        _builder = _builder.Inject(_modManager);
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
        Camera.Main = new Camera(GraphicsDevice.Viewport);

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
        _pipeline = _builder
            .Inject(_imGuiRenderer)
            .BuildAndInit();
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
        Camera.Main.UpdateViewport(GraphicsDevice.Viewport);
        
        _pipeline.Run();
        if (_fixedTimer >= Time.FixedDeltaTime)
        {
            _pipeline.GetRunner<EcsFixedRunRunner>().FixedRun();
            _fixedTimer -= Time.FixedDeltaTime;
        }
        _pipeline.GetRunner<EcsPausableRunner>().PausableRun();
        _pipeline.GetRunner<PausableLateRunner>().PausableLateRun();
        
        UI.Update();
        
        base.Update(gameTime);
        
        SuppressDraw();
        if (BeginDraw())
        {
            Draw(gameTime);
            EndDraw();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Drawer.Draw();
        UI.Draw();
        
        base.Draw(gameTime);

        _pipeline.Injector.Inject(gameTime);
        _pipeline.GetRunner<DebugRunner>().DebugRun();
    }

    private void InitEcs()
    {
        _builder
            .AddRunner<EcsPausableRunner>()
            .AddRunner<PausableLateRunner>()
            .AddRunner<EcsFixedRunRunner>()
            .AddRunner<GamePreInit>()
            .AddRunner<GameInit>()
            .AddRunner<DebugRunner>()
            .AddModule(new VisualModule())
            .AddModule(new InputModule())
            .AddModule(new PhysicsModule())
            .AddModule(new TimeModule())
            .AddModule(new ModdingModule());
    }
}