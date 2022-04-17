using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace PlogonScript;

internal class PrimaryWindow : Window
{
    private readonly Configuration _configuration;
    private readonly ScriptManager _scriptManager;

    public PrimaryWindow(ScriptManager scriptManager, Configuration configuration) : base("PlogonScript")
    {
        _scriptManager = scriptManager;
        _configuration = configuration;

        Size = new Vector2(1200, 675);
        SizeCondition = ImGuiCond.FirstUseEver;
        Flags |= ImGuiWindowFlags.MenuBar;
    }

    private string? SelectedScriptName
    {
        get => _configuration.SelectedScript;
        set => _configuration.SelectedScript = value;
    }

    private Script? SelectedScript
    {
        get
        {
            if (SelectedScriptName == null) return null;

            _scriptManager.Scripts.TryGetValue(SelectedScriptName, out var script);
            return script;
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();

        if (SelectedScriptName == null || !_scriptManager.Scripts.ContainsKey(SelectedScriptName))
            SelectedScriptName = _scriptManager.Scripts.Keys.FirstOrDefault();
    }

    public override void Draw()
    {
        DrawMenuBar();
        DrawLeftPane();
        ImGui.SameLine();
        DrawRightPane();
    }

    private void DrawMenuBar()
    {
        if (!ImGui.BeginMenuBar()) return;

        if (ImGui.MenuItem("Open Scripts Folder"))
            _scriptManager.OpenFolder();

        ImGui.EndMenuBar();
    }

    private void DrawLeftPane()
    {
        ImGui.BeginChild("left pane", new Vector2(150, 0), true);

        foreach (var script in _scriptManager.Scripts.Values)
        {
            if (ImGui.Selectable(script.DisplayName, script.Filename == SelectedScriptName))
                SelectedScriptName = script.Filename;
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button,
                script.Loaded
                    ? new Vector4(0.54f, 0.60f, 0.06f, 1.0f)
                    : new Vector4(0.74f, 0.08f, 0.31f, 1.0f));
            ImGui.SmallButton("  ");
            ImGui.PopStyleColor();
        }

        ImGui.EndChild();
    }

    private void DrawRightPane()
    {
        ImGui.BeginChild("item view", new Vector2(0, 0), true, ImGuiWindowFlags.MenuBar);
        if (SelectedScript != null)
        {
            if (ImGui.BeginMenuBar())
            {
                string name = SelectedScript.Metadata.Name, author = SelectedScript.Metadata.Author;
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X * 0.2f);
                ImGui.InputText("Name", ref name, 32);
                ImGui.SameLine();
                ImGui.InputText("Author", ref author, 32);
                SelectedScript.Metadata = new ScriptMetadata(name, author);

                if (ImGui.MenuItem("Save", SelectedScript.Metadata.Valid))
                    SelectedScript.SaveContents();

                var originalAutoload = _configuration.AutoloadedScripts.GetValueOrDefault(SelectedScript.Filename);
                var imguiAutoload = originalAutoload;
                ImGui.Checkbox("Autoload", ref imguiAutoload);
                if (imguiAutoload != originalAutoload)
                {
                    _configuration.AutoloadedScripts[SelectedScript.Filename] = imguiAutoload;
                    _configuration.Save();
                }

                if (SelectedScript.Loaded)
                {
                    if (ImGui.MenuItem("Reload"))
                    {
                        SelectedScript.Unload();
                        SelectedScript.Load();
                    }

                    if (ImGui.MenuItem("Unload")) SelectedScript.Unload();
                }
                else
                {
                    if (ImGui.MenuItem("Load")) SelectedScript.Load();
                }

                ImGui.PopItemWidth();
            }

            ImGui.EndMenuBar();

            var contents = SelectedScript.Contents;
            ImGui.InputTextMultiline("##source", ref contents, 16384,
                new Vector2(-float.Epsilon, -float.Epsilon), ImGuiInputTextFlags.AllowTabInput);
            SelectedScript.Contents = contents;
        }

        ImGui.EndChild();
    }
}