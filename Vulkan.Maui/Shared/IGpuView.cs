using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Vulkan.Maui.Shared
{
    public interface IGpuView
    {
        /// <summary>
        /// Use pixels as units.
        /// </summary>
        Vector2 FramebufferSize { get; }

        GameBase Game { get; set; }

        /// <summary>
        /// generate some information according to gpuview.
        /// </summary>
        VulkanAppInfo AppInfo { get; set; }
    }
}
