using ImGuiNET;

namespace KarpikEngine.Modules.Graphics;

internal interface IDebugCommand
{
    public void Execute();
}

internal struct BeginCommand(string name) : IDebugCommand
{
    public string Name = name;

    public void Execute() => ImGui.Begin(Name);
}