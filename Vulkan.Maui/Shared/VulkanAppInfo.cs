using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Vk;

namespace Vulkan.Maui.Shared
{
    public class VulkanAppInfo
    {
        private VulkanSurface? _vk;
        public VulkanSurface VkSurface => _vk ??= new VulkanSurface(this);

        public SwapchainSource SwapchainSource;

        List<FixedUtf8String> _surfaceExtensions = new List<FixedUtf8String>();
        public bool HasSurfaceExtension(FixedUtf8String extension)
        {
            return _surfaceExtensions.Contains(extension);
        }

        /// <summary>
        /// load Vulkan api by it.
        /// </summary>
        public Vk Vk = Vk.GetApi();

        /// <summary>
        /// For <see cref="InstanceCreateInfo.PpEnabledExtensionNames"/>
        /// </summary>
        internal StackList<IntPtr, Size64Bytes> InstanceExtensions = new StackList<IntPtr, Size64Bytes>();

        public VulkanAppInfo()
        {
            CreateInstanceInfo(InstanceCreateFlags.None, new string[] { });
        }

        public unsafe (StackList<IntPtr, Size64Bytes> PpEnabledLayerNames, StackList<IntPtr, Size64Bytes> PpEnabledExtensionNames) CreateInstanceInfo(InstanceCreateFlags instanceCreateFlagsSetting, string[] instanceExtensionsSetting)
        {
            HashSet<string> availableInstanceLayers = new HashSet<string>(VulkanUtil.EnumerateInstanceLayers(Vk));
            HashSet<string> availableInstanceExtensions = new HashSet<string>(VulkanUtil.EnumerateInstanceExtensions(Vk));

            StackList<IntPtr, Size64Bytes> instanceLayers = new StackList<IntPtr, Size64Bytes>();

            if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_portability_subset))
            {
                _surfaceExtensions.Add(CommonStrings.VK_KHR_portability_subset);
            }

            if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_portability_enumeration))
            {
                InstanceExtensions.Add(CommonStrings.VK_KHR_portability_enumeration);
                instanceCreateFlagsSetting |= InstanceCreateFlags.EnumeratePortabilityBitKhr;
            }

            if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME))
            {
                _surfaceExtensions.Add(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
                }
            }
            else if (OperatingSystem.IsAndroid() || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_EXT_METAL_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_EXT_METAL_SURFACE_EXTENSION_NAME);
                }
                else // Legacy MoltenVK extensions
                {
                    if (availableInstanceExtensions.Contains(CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME))
                    {
                        _surfaceExtensions.Add(CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME);
                    }
                    if (availableInstanceExtensions.Contains(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME))
                    {
                        _surfaceExtensions.Add(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME);
                    }
                }
            }

            foreach (var ext in _surfaceExtensions)
            {
                InstanceExtensions.Add(ext);
            }

            bool hasDeviceProperties2 = availableInstanceExtensions.Contains(CommonStrings.VK_KHR_get_physical_device_properties2);
            if (hasDeviceProperties2)
            {
                InstanceExtensions.Add(CommonStrings.VK_KHR_get_physical_device_properties2);
            }

            string[] requestedInstanceExtensions = instanceExtensionsSetting ?? Array.Empty<string>();
            List<FixedUtf8String> tempStrings = new List<FixedUtf8String>();
            foreach (string requiredExt in requestedInstanceExtensions)
            {
                if (!availableInstanceExtensions.Contains(requiredExt))
                {
                    throw new Exception($"The required instance extension was not available: {requiredExt}");
                }

                FixedUtf8String utf8Str = new FixedUtf8String(requiredExt);
                InstanceExtensions.Add(utf8Str);
                tempStrings.Add(utf8Str);
            }

            return (instanceLayers, InstanceExtensions);
        }
    }
}
