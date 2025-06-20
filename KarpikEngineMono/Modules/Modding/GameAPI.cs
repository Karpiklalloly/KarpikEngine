using System.Diagnostics.CodeAnalysis;
using ImGuiNET;
using KarpikEngineMono.Modules.Utilities;
using MoonSharp.Interpreter;

namespace KarpikEngineMono.Modules.Modding;

[MoonSharpUserData]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class GameAPI
{
    private readonly string _modId;
    private readonly ModContainer _container;

    public GameAPI(string modId, ModContainer container)
    {
        _modId = modId;
        _container = container;
    }
    
    public void log(string message, LogLevel level = LogLevel.Debug)
    {
        Logger.Instance.Log(_modId, message, level); 
    }
    
    public void register_command(string name, Action callback)
    {
        
    }
    
    public DynValue require(string moduleName)
    {
        return _container.LoadModule(moduleName);
    }

    public void ImGUI_begin(string name)
    {
        ImGui.Begin(name);
    }
    
    public void ImGUI_text(string message)
    {
        ImGui.Text(message);
    }
    
    public void ImGUI_text_colored(string message, float r, float g, float b, float a)
    {
        ImGui.TextColored(new System.Numerics.Vector4(r, g, b, a), message);
    }

    public void ImGUI_separator()
    {
        ImGui.Separator();
    }

    public bool ImGUI_button(string label)
    {
        return ImGui.Button(label);
    }

    public void ImGUI_end()
    {
        ImGui.End();
    }
}