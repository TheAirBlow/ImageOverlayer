using System.Numerics;
using ImGuiNET;
using NativeFileDialog.Extended;
using Raylib_ImGui;
using Raylib_ImGui.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageOverlayer.Windows;

/// <summary>
/// Layers window
/// </summary>
public class LayersWindow : GuiWindow {
    /// <summary>
    /// Image window instance
    /// </summary>
    private readonly ImageWindow _window;

    private readonly List<int> _toRemove = new();

    /// <summary>
    /// Creates a new layers window
    /// </summary>
    /// <param name="window">Image Window</param>
    public LayersWindow(ImageWindow window) {
        _window = window;
    }
    
    /// <summary>
    /// Draw the GUI
    /// </summary>
    /// <param name="renderer">ImGui renderer</param>
    public override void DrawGUI(ImGuiRenderer renderer) {
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize - new Vector2(2 + ImGui.GetIO().DisplaySize.X / 2, 1));
        ImGui.SetNextWindowPos(new Vector2(1 + ImGui.GetIO().DisplaySize.X / 2, 1));
        if (ImGui.Begin($"##{ID}", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysVerticalScrollbar)) {
            if (ImGui.Button("Remove all layers")) {
                _window.Layers.Clear();
                _window.Update();
            }
            ImGui.SameLine();
            if (ImGui.Button("Add image")) {
                var path = (string?)NFD.OpenDialog(".",
                    new Dictionary<string, string> {
                        ["PNG file"] = "png"
                    });
                if (path != null) {
                    try {
                        _window.Layers.Add(new ImageWindow.Layer(path));
                    } catch (Exception e) {
                        renderer.OpenWindow(new PopupWindow($"Failed to open {path}", e.ToString()));
                    }
                    
                    _window.Update();
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Export image"))
                renderer.OpenWindow(new ExportWindow(_window));
            
            if (ImGui.BeginTable("##layers", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders)) {
                ImGui.TableSetupColumn("Preview");
                ImGui.TableSetupColumn("Settings");
                ImGui.TableHeadersRow();
                for (var i = 0; i < _window.Layers.Count; i++) {
                    var layer = _window.Layers[i];
                    ImGui.TableNextColumn();
                    ImGui.Image(layer.Handle, new Vector2(85, 85));
                    ImGui.TableNextColumn();
                    var visible = layer.Render;
                    var x = layer.Offset.X;
                    var y = layer.Offset.Y;
                    if (ImGui.Button($"X##{i}")) _toRemove.Add(i);
                    ImGui.SameLine(); ImGui.BeginDisabled(i == 0);
                    if (ImGui.Button($"<##{i}")) {
                        var old = _window.Layers[i - 1];
                        _window.Layers[i - 1] = layer;
                        _window.Layers[i] = old;
                        _window.Update();
                    }
                    ImGui.EndDisabled(); ImGui.SameLine();
                    ImGui.BeginDisabled(i == _window.Layers.Count - 1);
                    if (ImGui.Button($">##{i}")) {
                        var old = _window.Layers[i + 1];
                        _window.Layers[i + 1] = layer;
                        _window.Layers[i] = old;
                        _window.Update();
                    }
                    ImGui.EndDisabled(); ImGui.SameLine();
                    ImGui.Checkbox($"{Path.GetFileName(layer.Path)}##{i}", ref visible);
                    ImGui.InputInt($"Offset X##{i}", ref x);
                    ImGui.InputInt($"Offset Y##{i}", ref y);
                    if (x != layer.Offset.X || y != layer.Offset.Y) {
                        layer.Offset = new Point(x, y); _window.Update();
                    }

                    if (visible != layer.Render) {
                        layer.Render = visible; _window.Update();
                    }
                }
                ImGui.EndTable();
            }

            ImGui.End();
        }

        if (_toRemove.Count != 0) {
            foreach (var i in _toRemove)
                _window.Layers.RemoveAt(i);
            _toRemove.Clear(); _window.Update();
        }
    }
}