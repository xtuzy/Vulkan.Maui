using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Vulkan.Maui.Shared
{
    public class HeadlessGpuView : IGpuView
    {
        public HeadlessGpuView(Vector2 size)
        {
            FramebufferSize = size;
        }

        public void OnUnloaded()
        {
            Game?.OnGraphicsDeviceDestroyed();
        }

        public void OnLoaded()
        {
            AppInfo = new VulkanAppInfo();
            Game?.OnGraphicsDeviceCreated();
        }

        public Vector2 FramebufferSize { get; set; }

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
