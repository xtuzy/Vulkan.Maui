using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Device = Silk.NET.Vulkan.Device;
using System.Diagnostics;
using static Vulkan.Maui.Shared.VulkanUtil;
namespace Vulkan.Maui.Shared
{
    internal unsafe class VulkanUtil
    {
        [Conditional("DEBUG")]
        public static void CheckResult(Result result)
        {
            if (result != Result.Success)
            {
                throw new Exception("Unsuccessful VkResult: " + result);
            }
        }

        public static bool TryFindMemoryType(PhysicalDeviceMemoryProperties memProperties, uint typeFilter, MemoryPropertyFlags properties, out uint typeIndex)
        {
            typeIndex = 0;

            for (int i = 0; i < memProperties.MemoryTypeCount; i++)
            {
                if (((typeFilter & (1 << i)) != 0)
                    && (memProperties.GetMemoryType((uint)i).PropertyFlags & properties) == properties)
                {
                    typeIndex = (uint)i;
                    return true;
                }
            }

            return false;
        }

        public static string[] EnumerateInstanceLayers(Vk vk)
        {
            uint propCount = 0;
            Result result = vk.EnumerateInstanceLayerProperties(ref propCount, null);
            CheckResult(result);
            if (propCount == 0)
            {
                return Array.Empty<string>();
            }

            LayerProperties[] props = new LayerProperties[propCount];
            vk.EnumerateInstanceLayerProperties(ref propCount, ref props[0]);

            string[] ret = new string[propCount];
            for (int i = 0; i < propCount; i++)
            {
                fixed (byte* layerNamePtr = props[i].LayerName)
                {
                    ret[i] = Util.GetString(layerNamePtr);
                }
            }

            return ret;
        }

        public static string[] EnumerateInstanceExtensions(Vk vk)
        {
            if (!TryLoadVulkan(vk))
            {
                return Array.Empty<string>();
            }

            uint propCount = 0;
            Result result = vk.EnumerateInstanceExtensionProperties((byte*)null, ref propCount, null);
            if (result != Result.Success)
            {
                return Array.Empty<string>();
            }

            if (propCount == 0)
            {
                return Array.Empty<string>();
            }

            ExtensionProperties[] props = new ExtensionProperties[propCount];
            vk.EnumerateInstanceExtensionProperties((byte*)null, ref propCount, ref props[0]);

            string[] ret = new string[propCount];
            for (int i = 0; i < propCount; i++)
            {
                fixed (byte* extensionNamePtr = props[i].ExtensionName)
                {
                    ret[i] = Util.GetString(extensionNamePtr);
                }
            }

            return ret;
        }

        static bool isLoaded = false;
        public static bool TryLoadVulkan(Vk vk)
        {
            if(isLoaded) return true;
            try
            {
                uint propCount;
                vk.EnumerateInstanceExtensionProperties((byte*)null, &propCount, null);
                isLoaded = true;
                return true;
            }
            catch { return false; }
        }

        internal static unsafe IntPtr GetInstanceProcAddr(Instance instance, string name)
        {
            int byteCount = Encoding.UTF8.GetByteCount(name);
            byte* utf8Ptr = stackalloc byte[byteCount + 1];

            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            return Vk.GetApi().GetInstanceProcAddr(instance, utf8Ptr);
        }

        internal static T GetInstanceProcAddr<T>(Instance instance, string name)
        {
            IntPtr funcPtr = GetInstanceProcAddr(instance, name);
            if (funcPtr != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
            }
            return default;
        }

        internal static unsafe IntPtr GetDeviceProcAddr(Device device, string name)
        {
            int byteCount = Encoding.UTF8.GetByteCount(name);
            byte* utf8Ptr = stackalloc byte[byteCount + 1];

            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            return Vk.GetApi().GetDeviceProcAddr(device, utf8Ptr);
        }

        internal static T GetDeviceProcAddr<T>(Device device, string name)
        {
            IntPtr funcPtr = GetDeviceProcAddr(device, name);
            if (funcPtr != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
            }
            return default;
        }
    }

    internal unsafe static class VkPhysicalDeviceMemoryPropertiesEx
    {
        public static MemoryType GetMemoryType(this PhysicalDeviceMemoryProperties memoryProperties, uint index)
        {
            return (&memoryProperties.MemoryTypes.Element0)[index];
        }
    }

    internal static class Util
    {
        internal static unsafe string GetString(byte* stringStart)
        {
            int characters = 0;
            while (stringStart[characters] != 0)
            {
                characters++;
            }

            return Encoding.UTF8.GetString(stringStart, characters);
        }
    }
}
