using System.Numerics;
using ImGuiNET;
using Raylib_ImGui;
using SixLabors.ImageSharp;

namespace ImageOverlayer;

/// <summary>
/// Various utilities
/// </summary>
public static class Utilities {
    /// <summary>
    /// Returns an ImGui handle for specified image
    /// </summary>
    /// <param name="image">Image</param>
    /// <returns>ImGui handle</returns>
    public static IntPtr GetHandle(this Image image) {
        using var stream = new MemoryStream(); image.SaveAsPng(stream);
        return stream.GetBuffer().LoadAsTexture(".png").CreateBinding();
    }

    /// <summary>
    /// Restricts specified vector by ImGui window size.
    /// Keeps aspect ratio intact.
    /// </summary>
    /// <param name="size">Vector</param>
    /// <returns>Restricted vector</returns>
    public static Vector2 RestrictByWindow(this Size size) {
        var windowSize = ImGui.GetWindowSize();
        var width = Math.Clamp(size.Width, 0, windowSize.X);
        var height = width * ((float)size.Height / size.Width);
        return new Vector2(width, height);
    }
}