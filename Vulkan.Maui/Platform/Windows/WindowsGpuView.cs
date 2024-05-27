using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Silk.NET.Vulkan;
using Vulkan.Maui.Shared;

namespace Vulkan.Maui
{
    public class WindowsGpuView : SwapChainPanel, IGpuView
    {
        public WindowsGpuView()
        {
            this.CompositionScaleChanged += OnViewScaleChanged;
            this.SizeChanged += OnViewSizeChanged;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnViewScaleChanged(SwapChainPanel sender, object args)
        {
            Game?.OnViewResize();
        }

        private void OnViewSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            Game?.OnViewResize();
        }

        private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Game?.OnGraphicsDeviceDestroyed();
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AppInfo = new VulkanAppInfo();
            Game?.OnGraphicsDeviceCreated();
        }

        public Vector2 FramebufferSize => new Vector2((float)this.RenderSize.Width, (float)this.RenderSize.Height);

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
                game.GpuView = this;
            }
        }

        public VulkanAppInfo AppInfo { get; set; }
    }
}
