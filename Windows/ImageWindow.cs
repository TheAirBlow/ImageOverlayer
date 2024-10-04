using System.Numerics;
using ImGuiNET;
using Raylib_ImGui;
using Raylib_ImGui.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageOverlayer.Windows;

/// <summary>
/// Image window
/// </summary>
public class ImageWindow : GuiWindow {
    /// <summary>
    /// List of image layers
    /// </summary>
    public List<Layer> Layers = new();

    /// <summary>
    /// Combined native image
    /// </summary>
    public Image? Image;

    /// <summary>
    /// Combined image ImGui handle
    /// </summary>
    public IntPtr? Handle;

    /// <summary>
    /// Current error message
    /// </summary>
    private string Message = "";
    
    /// <summary>
    /// An image layer
    /// </summary>
    public class Layer {
        /// <summary>
        /// Native image
        /// </summary>
        public Image Image;
        
        /// <summary>
        /// ImGui image handle
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        /// Image offset
        /// </summary>
        public Point Offset = new();

        /// <summary>
        /// Should this layer be rendered
        /// </summary>
        public bool Render = true;

        /// <summary>
        /// Path to the file
        /// </summary>
        public string Path;

        /// <summary>
        /// Creates a new layer
        /// </summary>
        /// <param name="path">Path</param>
        public Layer(string path) {
            Image = Image.Load(path); Path = path;
            Handle = Image.GetHandle();
        }
    }

    /// <summary>
    /// Creates a new image window
    /// </summary>
    public ImageWindow() => Update();

    /// <summary>
    /// Updates the combined image
    /// </summary>
    public void Update() => new Thread(RealUpdate).Start();

    /// <summary>
    /// Update implementation
    /// </summary>
    private void RealUpdate() {
        try {
            Message = "Rendering is in progress, please wait...";
            if (Layers.Count == 0) {
                Message = "Empty preview - no layers were added!";
                Image = null; Handle = null; return;
            }
            
            var layers = Layers.Where(layer => layer.Render).ToList();
            if (layers.Count == 0) {
                Message = "Empty preview - no enabled layers!";
                Image = null; Handle = null; return;
            }

            var image = new Image<Rgba64>(layers[0].Image.Width, layers[0].Image.Height);
            image.Mutate(ctx => {
                foreach (var layer in layers) ctx.DrawImage(layer.Image,
                    layer.Offset, PixelColorBlendingMode.Normal, 1f);
            });
            Image = image;
            Handle = null;
        } catch (Exception e) {
            Image = null; Handle = null;
            Message = e.ToString();
        }
    }
    
    /// <summary>
    /// Draw the GUI
    /// </summary>
    /// <param name="renderer">ImGui renderer</param>
    public override void DrawGUI(ImGuiRenderer renderer) {
        if (Image != null && Handle == null) Handle = Image.GetHandle();
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize - new Vector2(2 + ImGui.GetIO().DisplaySize.X / 2, 1));
        ImGui.SetNextWindowPos(new Vector2(1, 1));
        if (ImGui.Begin($"##{ID}", ImGuiWindowFlags.NoDecoration)) {
            if (Handle.HasValue) ImGui.Image(Handle.Value, Image!.Size.RestrictByWindow());
            else ImGui.Text(Message); ImGui.End();
        }
    }
}