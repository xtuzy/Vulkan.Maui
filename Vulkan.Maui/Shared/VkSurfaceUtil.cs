#if __IOS__
using CoreAnimation;
using Foundation;
using ObjCRuntime;
#endif
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Vulkan.Extensions.MVK;

namespace Vulkan.Maui.Shared
{
    internal static unsafe class VkSurfaceUtil
    {
        internal static SurfaceKHR CreateSurface(VulkanAppInfo context, Instance instance, SwapchainSource swapchainSource)
        {
            // TODO a null GD is passed from VkSurfaceSource.CreateSurface for compatibility
            //      when VkSurfaceInfo is removed we do not have to handle gd == null anymore
            var doCheck = context != null;

            if (doCheck && !context.HasSurfaceExtension(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME))
                throw new Exception($"The required instance extension was not available: {CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME}");

            switch (swapchainSource)
            {
                case Win32SwapchainSource win32Source:
                    if (doCheck && !context.HasSurfaceExtension(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME))
                    {
                        throw new Exception($"The required instance extension was not available: {CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME}");
                    }
                    return CreateWin32(context, instance, win32Source);
#if ANDROID
                case AndroidSurfaceSwapchainSource androidSource:
                    if (doCheck && !context.HasSurfaceExtension(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME))
                    {
                        throw new Exception($"The required instance extension was not available: {CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME}");
                    }
                    return CreateAndroidSurface(context, instance, androidSource);
#elif __IOS__
                case UIViewSwapchainSource uiViewSource:
                    if (doCheck)
                    {
                        bool hasMetalExtension = context.HasSurfaceExtension(CommonStrings.VK_EXT_METAL_SURFACE_EXTENSION_NAME);
                        if (hasMetalExtension || context.HasSurfaceExtension(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME))
                        {
                            return CreateUIViewSurface(context, instance, uiViewSource, hasMetalExtension);
                        }
                        else
                        {
                            throw new Exception($"Neither macOS surface extension was available: " +
                                $"{CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME}, {CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME}");
                        }
                    }

                    return CreateUIViewSurface(context, instance, uiViewSource, false);
#endif
                default:
                    throw new Exception($"The provided SwapchainSource cannot be used to create a Vulkan surface.");
            }
        }

        private static SurfaceKHR CreateWin32(VulkanAppInfo context, Instance instance, Win32SwapchainSource win32Source)
        {
            Win32SurfaceCreateInfoKHR surfaceCI = new Win32SurfaceCreateInfoKHR();
            surfaceCI.Hwnd = win32Source.Hwnd;
            surfaceCI.Hinstance = win32Source.Hinstance;
            context.Vk.TryGetInstanceExtension(instance, out KhrWin32Surface khrWin32Surface);
            Result result = khrWin32Surface.CreateWin32Surface(instance, ref surfaceCI, null, out SurfaceKHR surface);
            VulkanUtil.CheckResult(result);
            return surface;
        }

#if ANDROID
        private static SurfaceKHR CreateAndroidSurface(VulkanAppInfo context, Instance instance, AndroidSurfaceSwapchainSource androidSource)
        {
            IntPtr aNativeWindow = AndroidRuntime.ANativeWindow_fromSurface(androidSource.JniEnv, androidSource.Surface.Handle);
            AndroidSurfaceCreateInfoKHR androidSurfaceCI = new AndroidSurfaceCreateInfoKHR();
            androidSurfaceCI.Window = (nint*)aNativeWindow;
            context.Vk.TryGetInstanceExtension(instance, out KhrAndroidSurface khrAndroidSurface);
            Result result = khrAndroidSurface.CreateAndroidSurface(instance, ref androidSurfaceCI, null, out SurfaceKHR surface);
            VulkanUtil.CheckResult(result);
            return surface;
        }
#endif

#if __IOS__
        private static SurfaceKHR CreateUIViewSurface(VulkanAppInfo context, Instance instance, UIViewSwapchainSource uiViewSource, bool hasExtMetalSurface)
        {
            UIKit.UIView uiView = uiViewSource.UIView;
            CAMetalLayer metalLayer = uiView.Layer as CAMetalLayer;
            if (metalLayer == null)
            {
                metalLayer = new CAMetalLayer();
                metalLayer.Frame = uiView.Frame;
                metalLayer.Opaque = true;
                uiView.Layer.AddSublayer(metalLayer);
            }

            if (hasExtMetalSurface)
            {
                MetalSurfaceCreateInfoEXT surfaceCI = new MetalSurfaceCreateInfoEXT();
                surfaceCI.SType = StructureType.MetalSurfaceCreateInfoExt;
                surfaceCI.PLayer = (nint*)metalLayer.Handle.Handle.ToPointer();
                SurfaceKHR surface;
                context.Vk.TryGetInstanceExtension(instance, out ExtMetalSurface extMetalSurface);
                Result result = extMetalSurface.CreateMetalSurface(instance, &surfaceCI, null, &surface);
                VulkanUtil.CheckResult(result);
                return surface;
            }
            else
            {
                IOSSurfaceCreateInfoMVK surfaceCI = new IOSSurfaceCreateInfoMVK();
                surfaceCI.PView = uiView.Handle.Handle.ToPointer();
                context.Vk.TryGetInstanceExtension(instance, out MvkIosSurface mvkIosSurface);
                Result result = mvkIosSurface.CreateIossurface(instance, ref surfaceCI, null, out SurfaceKHR surface);
                return surface;
            }
        }
#endif
    }
}
