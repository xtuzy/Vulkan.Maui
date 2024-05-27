#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Diagnostics;
using Vulkan.Maui.Shared;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Vulkan.Maui.Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainController : Microsoft.UI.Xaml.Window
    {
        public MainController()
        {
            //WinUI Set window special size
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(500, 500));

            if (false)
            {
                var gpuView = new VeldridPlatformView() { Width = 300, Height = 300 };
                var scrollView = new Microsoft.UI.Xaml.Controls.ScrollViewer()
                {
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.Maui.Graphics.Colors.Blue.ToWindowsColor()),
                };
                var imageView = new SkiaSharp.Views.Windows.SKXamlCanvas();
                scrollView.Content = new StackPanel()
                {
                    Children =
                {
                    gpuView,
                    new Microsoft.UI.Xaml.Controls.Button()
                    {
                        HorizontalAlignment=Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        VerticalAlignment=Microsoft.UI.Xaml.VerticalAlignment.Center,
                        Content = "Click"
                    },
                    imageView
                }
                };
                var app = new VeldridApp(gpuView);
                app.Game = new HelloTriangle();

                this.Content = scrollView;

                SKBitmap bitmap = null;
                try
                {
                    var headless = new HeadlessHelloTriangle();
                    headless.CreateResources();
                    bitmap = headless.SaveRgba32ToSKBitmap(headless.OnRender());
                    headless.Dispose();
                }
                catch (Exception ex) { }
                imageView.PaintSurface += (sender, e) =>
                {
                    if (bitmap != null)
                    {
                        e.Surface.Canvas.DrawBitmap(bitmap, 0, 0);
                    }
                };
            }
            else
            {
                var gpuView = new HeadlessGpuView(new System.Numerics.Vector2(400, 400));
                (gpuView as IGpuView).Game = new HelloTriangleGame();
                gpuView.OnLoaded();
            }
        }
    }
}
#endif
