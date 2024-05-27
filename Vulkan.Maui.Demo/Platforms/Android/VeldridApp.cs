using Android.Runtime;
using Android.Views;
using Veldrid;

namespace Vulkan.Maui.Demo
{
    public class VeldridApp
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

        /// <summary>
        /// Android的View在代码中宽高本身使用像素为单位, 无需转换
        /// </summary>
        public uint Width => (uint)View.Width;
        public uint Height => (uint)View.Height;

        public VeldridApp(VeldridPlatformView view, GraphicsBackend backend = GraphicsBackend.OpenGLES)
        {
            if (!(backend == GraphicsBackend.Vulkan || backend == GraphicsBackend.OpenGLES))
            {
                throw new NotSupportedException($"{backend} is not supported on Android.");
            }
            _backend = backend;

            View = view;
            View.AndroidSurfaceCreated += CreateGraphicsDevice;
            View.AndroidSurfaceChanged += OnViewSizeChanged;
            View.AndroidSurfaceDestoryed += DestroyGraphicsDevice;
        }

        private void CreateGraphicsDevice(ISurfaceHolder holder)
        {
            var Options = new GraphicsDeviceOptions(false, PixelFormat.R16_UNorm, false, ResourceBindingModel.Improved, true, true);

            if (_backend == GraphicsBackend.Vulkan)
            {
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    Options.SwapchainDepthFormat,
                    Options.SyncToVerticalBlank);

                if (GraphicsDevice == null)
                {
                    GraphicsDevice = GraphicsDevice.CreateVulkan(Options, sd);
                }

                SwapChain = GraphicsDevice.MainSwapchain;
            }
            else
            {
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    Options.SwapchainDepthFormat,
                    Options.SyncToVerticalBlank);
                GraphicsDevice = GraphicsDevice.CreateOpenGLES(Options, sd);
                SwapChain = GraphicsDevice.MainSwapchain;
            }

            Game?.OnGraphicsDeviceCreated();

            if (Animator == null)
            {
                Animator = new ValueAnimator();
                Animator.set(RenderLoop);
            }
            Animator.start();
        }

        private void OnViewSizeChanged()
        {
            if (GraphicsDevice != null)
            {
                SwapChain.Resize((uint)Width, (uint)Height);
                Game?.OnViewResize();
            }
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
            }
        }

        private void RenderLoop()
        {
            if (GraphicsDevice != null)
                Game?.OnRender(16);
        }
    }
}
