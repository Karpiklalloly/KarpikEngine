using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace KarpikEngineMono.Modules.Graphics;

public static class DebugGraphics
{
    private static Queue<Action> Actions = new();

    public static void Begin(string name)
    {
        Actions.Enqueue(() => ImGui.Begin(name));
    }

    public static void End()
    {
        Actions.Enqueue(ImGui.End);
    }

    public static void Text(string text)
    {
        Actions.Enqueue(() => ImGui.Text(text));
    }

    public static void ColoredText(string text, Vector4 color)
    {
        Actions.Enqueue(() => ImGui.TextColored(color, text));
    }

    public static void Button(string text, Action onClick)
    {
        Actions.Enqueue(() =>
        {
            if (ImGui.Button(text))
            {
                onClick?.Invoke();
            }
        });
    }

    public static void Button(string text, Vector2 size, Action onClick)
    {
        Actions.Enqueue(() =>
        {
            if (ImGui.Button(text, size))
            {
                onClick?.Invoke();
            }
        });
    }

    internal static void Draw()
    {
        while (Actions.Count > 0)
        {
            Actions.Dequeue().Invoke();
        }
    }
}