using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulkan.Maui.Shared
{
    public class VulkanSurface : IVkSurface
    {
        private readonly VulkanAppInfo _AppInfo;

        public VulkanSurface(VulkanAppInfo appInfo)
        {
            _AppInfo = appInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">From <see cref="Silk.NET.Vulkan.Instance"/></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        public unsafe VkNonDispatchableHandle Create<T>(VkHandle instance, T* allocator) where T : unmanaged
        {
            VkNonDispatchableHandle ret = new VkNonDispatchableHandle();
            if (_AppInfo.SwapchainSource != null)
            {
                var surfaceKHR = VkSurfaceUtil.CreateSurface(_AppInfo, new Instance(instance.Handle), _AppInfo.SwapchainSource); ;
                ret.Handle = surfaceKHR.Handle;
            }
            return ret;
        }

        public unsafe byte** GetRequiredExtensions(out uint count)
        {
            count = _AppInfo.InstanceExtensions.Count;
            return (byte**)_AppInfo.InstanceExtensions.Data;
        }
    }
}
