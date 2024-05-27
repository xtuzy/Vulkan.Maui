using System;
using System.Runtime.InteropServices;

namespace Vulkan.Maui.Shared
{
    /// <summary>
    /// A platform-specific object representing a renderable surface.
    /// A SwapchainSource can be created with one of several static factory methods.
    /// A SwapchainSource is used to describe a Swapchain (see <see cref="SwapchainDescription"/>).
    /// </summary>
    public abstract class SwapchainSource
    {
        internal SwapchainSource() { }

        /// <summary>
        /// Creates a new SwapchainSource for a Win32 window.
        /// </summary>
        /// <param name="hwnd">The Win32 window handle.</param>
        /// <param name="hinstance">The Win32 instance handle.</param>
        /// <returns>A new SwapchainSource which can be used to create a <see cref="Swapchain"/> for the given Win32 window.
        /// </returns>
        public static SwapchainSource CreateWin32(IntPtr hwnd, IntPtr hinstance) => new Win32SwapchainSource(hwnd, hinstance);

        /// <summary>
        /// Creates a new SwapchainSource for a UWP SwapChain panel.
        /// </summary>
        /// <param name="swapChainPanel">A COM object which must implement the <see cref="Vortice.DXGI.ISwapChainPanelNative"/>
        /// or <see cref="Vortice.DXGI.ISwapChainBackgroundPanelNative"/> interface. Generally, this should be a SwapChainPanel
        /// or SwapChainBackgroundPanel contained in your application window.</param>
        /// <param name="logicalDpi">The logical DPI of the swapchain panel.</param>
        /// <returns>A new SwapchainSource which can be used to create a <see cref="Swapchain"/> for the given UWP panel.
        /// </returns>
#if WINDOWS
        public static SwapchainSource CreateUwp(Microsoft.UI.Xaml.Controls.SwapChainPanel swapChainPanel, float logicalDpi)
            => new UwpSwapchainSource(swapChainPanel, logicalDpi);
#endif

        /// <summary>
        /// Creates a new SwapchainSource for the given UIView.
        /// </summary>
        /// <param name="uiView">The UIView's native handle.</param>
        /// <returns>A new SwapchainSource which can be used to create a Metal <see cref="Swapchain"/> or an OpenGLES
        /// <see cref="GraphicsDevice"/> for the given UIView.
        /// </returns>
#if __IOS__
        public static SwapchainSource CreateUIView(UIKit.UIView uiView) => new UIViewSwapchainSource(uiView);
#endif
        /// <summary>
        /// Creates a new SwapchainSource for the given Android Surface.
        /// </summary>
        /// <param name="surfaceHandle">The handle of the Android Surface.</param>
        /// <param name="jniEnv">The Java Native Interface Environment handle.</param>
        /// <returns>A new SwapchainSource which can be used to create a Vulkan <see cref="Swapchain"/> or an OpenGLES
        /// <see cref="GraphicsDevice"/> for the given Android Surface.</returns>
#if ANDROID
        public static SwapchainSource CreateAndroidSurface(Android.Views.Surface surfaceHandle, IntPtr jniEnv)
            => new AndroidSurfaceSwapchainSource(surfaceHandle, jniEnv);
#endif
        /// <summary>
        /// Creates a new SwapchainSource for the given NSView.
        /// </summary>
        /// <param name="nsView">A pointer to an NSView.</param>
        /// <returns>A new SwapchainSource which can be used to create a Metal <see cref="Swapchain"/> for the given NSView.
        /// </returns>
        public static SwapchainSource CreateNSView(IntPtr nsView)
            => new NSViewSwapchainSource(nsView);
    }

    internal class Win32SwapchainSource : SwapchainSource
    {
        public IntPtr Hwnd { get; }
        public IntPtr Hinstance { get; }

        public Win32SwapchainSource(IntPtr hwnd, IntPtr hinstance)
        {
            Hwnd = hwnd;
            Hinstance = hinstance;
        }
    }

#if WINDOWS
    internal class UwpSwapchainSource : SwapchainSource
    {
        public Microsoft.UI.Xaml.Controls.SwapChainPanel SwapChainPanelNative { get; }
        public float LogicalDpi { get; }

        public UwpSwapchainSource(Microsoft.UI.Xaml.Controls.SwapChainPanel swapChainPanelNative, float logicalDpi)
        {
            SwapChainPanelNative = swapChainPanelNative;
            LogicalDpi = logicalDpi;
        }
    }
#endif

#if __IOS__
    internal class UIViewSwapchainSource : SwapchainSource
    {
        public UIKit.UIView UIView { get; }

        public UIViewSwapchainSource(UIKit.UIView uiView)
        {
            UIView = uiView;
        }
    }
#endif

#if ANDROID
    internal class AndroidSurfaceSwapchainSource : SwapchainSource
    {
        public Android.Views.Surface Surface { get; }
        public IntPtr JniEnv { get; }

        public AndroidSurfaceSwapchainSource(Android.Views.Surface surfaceHandle, IntPtr jniEnv)
        {
            Surface = surfaceHandle;
            JniEnv = jniEnv;
        }
    }
#endif

    internal class NSViewSwapchainSource : SwapchainSource
    {
        public IntPtr NSView { get; }

        public NSViewSwapchainSource(IntPtr nsView)
        {
            NSView = nsView;
        }
    }
}
