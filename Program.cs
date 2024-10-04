using System.Numerics;
using System.Reflection;
using ImageOverlayer.Windows;
using ImGuiNET;
using Raylib_CsLo;
using Raylib_ImGui;
using Raylib_ImGui.Windows;

var renderer = new ImGuiRenderer();
Raylib.SetTraceLogLevel(4);
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(1280, 720, "Image Overlayer");
var window = new ImageWindow();
renderer.OpenWindow(window);
renderer.OpenWindow(new LayersWindow(window));
renderer.SwitchContext();
renderer.RecreateFontTexture();
ImGui.PushStyleVar(ImGuiStyleVar.WindowTitleAlign,
    new Vector2(0.5f, 0.5f));
ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6);
ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding, 12);
ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

ImFontPtr fontPointer;
unsafe {
    var font = Assembly.GetCallingAssembly()
        .GetEmbeddedResource("DisposableDroid.ttf");
    fixed (byte* p = font) fontPointer = 
        ImGui.GetIO().Fonts.AddFontFromMemoryTTF(
            (IntPtr)p, font.Length, 20,
            ImGuiNative.ImFontConfig_ImFontConfig(),
            ImGui.GetIO().Fonts.GetGlyphRangesDefault());
    
    renderer.RecreateFontTexture();
}

while (!Raylib.WindowShouldClose()) {
    if (Raylib.IsFileDropped()) {
        foreach (var path in Raylib.GetDroppedFilesAndClear()) {
            try {
                window.Layers.Add(new ImageWindow.Layer(path));
            } catch (Exception e) {
                renderer.OpenWindow(new PopupWindow($"Failed to open {path}", e.ToString()));
            }
        }
        
        window.Update();
    }
    renderer.Update(); Raylib.BeginDrawing(); ImGui.NewFrame();
    Raylib.ClearBackground(new Color(42, 44, 48, 255));
    ImGui.PushFont(fontPointer);
    ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(),
        ImGuiDockNodeFlags.PassthruCentralNode);
    renderer.DrawWindows();
    renderer.RenderImGui();
    Raylib.EndDrawing();
}

Raylib.CloseWindow();