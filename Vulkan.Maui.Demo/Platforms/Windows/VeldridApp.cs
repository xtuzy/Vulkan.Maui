using System;
using System.Diagnostics;
using Veldrid;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Vulkan.Maui.Demo
{
    public sealed partial class VeldridApp
    {
        VeldridPlatformView View;

        GameBase game;
        public GameBase Game
        {
            get
            {
                return game;
            }
            set
            {
                game = value;
                game.PlatformVeldrid = this;
            }
        }

        GraphicsBackend _backend;
        public GraphicsDevice GraphicsDevice;
        public Swapchain SwapChain;

        ValueAnimator Animator;

        public uint Width => (uint)(View.RenderSize.Width);
        public uint Height => (uint)(View.RenderSize.Height);

        public VeldridApp(VeldridPlatformView view)
        {
            View = view;
            View.CompositionScaleChanged += OnViewScaleChanged;
            View.SizeChanged += OnViewSizeChanged;

            View.Loaded += OnLoaded;
            View.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DestroyGraphicsDevice();
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            CreateGraphicsDevice();
        }

        private void CreateGraphicsDevice()
        {
            var Options = new GraphicsDeviceOptions(false, PixelFormat.R32_Float, true, ResourceBindingModel.Improved);
            var logicalDpi = 96.0f * View.CompositionScaleX;
            var renderWidth = View.RenderSize.Width;
            var renderHeight = View.RenderSize.Height;

            GraphicsDevice = GraphicsDevice.CreateD3D11(Options, this.View, renderWidth, renderHeight, logicalDpi);
            SwapChain = GraphicsDevice.MainSwapchain;

            Game?.OnGraphicsDeviceCreated();
            if (Animator == null)
            {
                Animator = new ValueAnimator();
                Animator.set(RenderLoop);
            }
            Animator.start();
        }

        private void DestroyGraphicsDevice()
        {
            if (GraphicsDevice != null)
            {
                var tempDevice = GraphicsDevice;
                GraphicsDevice = null;//先设置null阻止渲染循环

                Game?.OnGraphicsDeviceDestroyed();
                tempDevice.WaitForIdle();
                tempDevice.Dispose();
                if (Animator != null)
                    Animator.cancel();
            }
        }

        /// <summary>
        /// View will still run it.
        /// </summary>
        private void RenderLoop()
        {
            if (GraphicsDevice != null)
            {
                try
                {
                    Game?.OnRender(16);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + ex);
                }
            }
        }

        private void OnViewSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            if (GraphicsDevice != null)
            {
                SwapChain.Resize(Width, Height);
                Game?.OnViewResize();
            }
        }

        private void OnViewScaleChanged(SwapChainPanel sender, object args)
        {
            if (GraphicsDevice != null)
            {
                SwapChain.Resize(Width, Height);
                Game?.OnViewResize();
            }
        }
    }
}
