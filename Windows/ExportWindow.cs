using System.Numerics;
using ImGuiNET;
using NativeFileDialog.Extended;
using Raylib_ImGui;
using Raylib_ImGui.Windows;
using SixLabors.ImageSharp;

namespace ImageOverlayer.Windows;

/// <summary>
/// Export image window
/// </summary>
public class ExportWindow : GuiWindow {
    /// <summary>
    /// Image window instance
    /// </summary>
    private readonly ImageWindow _window;
    
    /// <summary>
    /// Save file path
    /// </summary>
    private string _path;

    /// <summary>
    /// Creates a new export image window
    /// </summary>
    /// <param name="window">Image Window</param>
    public ExportWindow(ImageWindow window) {
        _window = window; _path = _window.Layers.Count == 0 ? "" : _window.Layers[0].Path;
    }
    
    /// <summary>
    /// Draw the GUI
    /// </summary>
    /// <param name="renderer">ImGui renderer</param>
    public override void DrawGUI(ImGuiRenderer renderer) {
        if (_window.Image == null) {
            renderer.OpenWindow(new PopupWindow("Failed to export image",
                "At least one enabled layer is required!"));
            IsOpen = false; return;
        }
        
        ImGui.OpenPopup($"Export image ##{ID}");
        if (ImGui.BeginPopupModal($"Export image ##{ID}", ref IsOpen, ImGuiWindowFlags.AlwaysAutoResize)) {
            ImGui.InputText("##path", ref _path, 255);
            ImGui.SameLine();
            if (ImGui.Button("Browse")) { 
                var path = (string?)NFD.SaveDialog(".", "filename.png",
                    new Dictionary<string, string> {
                        ["PNG image"] = "png"
                    });
                if (path != null) _path = path;
            }
            var split = ImGui.GetWindowWidth() / 2;
            ImGui.BeginDisabled(false);
            if (ImGui.Button("Save", new Vector2(split - 12, 30))) {
                try {
                    using var file = new FileStream(_path, FileMode.Create, FileAccess.Write);
                    _window.Image.SaveAsPng(file);
                } catch (Exception e) {
                    renderer.OpenWindow(new PopupWindow("Failed to export image", e.ToString()));
                }
                
                IsOpen = false;
            }
            ImGui.EndDisabled(); ImGui.SameLine();
            if (ImGui.Button("Cancel", new Vector2(split - 12, 30)))
                IsOpen = false;
            ImGui.EndPopup();
        }
    }
}