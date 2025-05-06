using Game.Modules;
using GTweens.Easings;
using GTweens.Enums;
using GTweens.Extensions;
using ImGuiNET;
using Karpik.DragonECS;
using Karpik.StatAndAbilities;
using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;
using KarpikEngineMono.Modules.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class DemoModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new MySystem());
    }
}

public class MySystem : IEcsDebugRun, IEcsFixedRun
{
    private bool[] _bools = new bool[1];
    
    public void DebugRun()
    {
        ImGui.Begin("DemoWindow");
        ShowButtons();
        ShowStats();
        ImGui.End();


        if (Input.IsPressed(Keys.Escape))
        {
            Time.IsPaused = !Time.IsPaused;
        }
    }

    private void ShowButtons()
    {
        ImGui.Columns(5);
        if (ImGui.Button("Spawn Player"))
        {
            var template = Loader.LoadTemplate("Player");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
        }

        ImGui.NextColumn();
        if (ImGui.Button("Spawn Enemy"))
        {
            var template = Loader.LoadTemplate("Enemy");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
        }

        ImGui.NextColumn();
        if (ImGui.Button("Save demo entity to file"))
        {
            var template = new ComponentsTemplate(
                new SpriteRenderer(),
                new Transform(),
                new HandleInputMovement(),
                new RigidBody(10, 11, 12, 13, 14, RigidBody.BodyType.Dynamic,
                    RigidBody.CollisionMode.ContinuousSpeculative),
                new ColliderBox(),
                new Speed(),
                new Player(),
                new Health());
            Loader.Serialize(template, "test template");
        }

        ImGui.NextColumn();
        if (ImGui.Button("Heal"))
        {
            ref var health = ref Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Health>();
            health.Value += health.Max.ModifiedValue;
        }

        ImGui.NextColumn();
        if (ImGui.Button("Spawn tween enemy"))
        {
            var template = Loader.LoadTemplate("Enemy");
            var world = Worlds.Instance.World;
            var e = world.NewEntityLong();
            template.ApplyTo(e.ID, world);
            e.Del<FollowPlayer>();

            var transform = e.Get<Transform>();
            var position = transform.Position;

            var tween = GTweenExtensions.Tween(
                    () => e.Get<Transform>().Position,
                    pos => e.Get<Transform>().Position = pos,
                    position + new Vector2(0, 200),
                    2f)
                .SetEasing(Easing.InOutCirc)
                .SetLoops(-1, ResetMode.PingPong);
            Tween.Add(tween, false);
        }

        ImGui.NextColumn();
        ImGui.Columns(1);
    }

    private void ShowStats()
    {
        ImGui.Text($"Total time: {Time.TotalTime:F2}");
        ImGui.Text($"Delta time: {Time.DeltaTime}");
        ImGui.Text($"FPS: {1 / Time.DeltaTime:F2}");
        ImGui.Text($"Entities: {Worlds.Instance.World.Entities.Count}");
        ImGui.Text($"Event Entities: {Worlds.Instance.EventWorld.Entities.Count}");
        if (!Worlds.Instance.MetaWorld.GetPlayer().Player.IsNull)
        {
            var health = Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Health>();
            var pos = Worlds.Instance.MetaWorld.GetPlayer().Player.Get<Transform>().Position;
            ImGui.Text($"Player Health: {health.Value} of {health.Max.ModifiedValue}");
            ImGui.Text($"Player Position: {pos.X:F2}, {pos.Y:F2}");
        }

        ImGui.Checkbox("Auto move player", ref _bools[0]);
    }

    public void FixedRun()
    {
        if (_bools[0])
        {
            var player = Worlds.Instance.MetaWorld.GetPlayer().Player;
            if (!player.IsNull)
            {
                player.MoveBySpeed(new Vector2(1, 0));
            }
        }
    }
}