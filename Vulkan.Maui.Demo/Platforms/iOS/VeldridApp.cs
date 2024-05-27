using System.Diagnostics;
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

        public uint Width => (uint)(View.Frame.Width * DeviceDisplay.Current.MainDisplayInfo.Density);
        public uint Height => (uint)(View.Frame.Height * DeviceDisplay.Current.MainDisplayInfo.Density);

        public VeldridApp(VeldridPlatformView view, GraphicsBackend backend = GraphicsBackend.Metal)
        {
            if (!(backend == GraphicsBackend.Metal || backend == GraphicsBackend.OpenGLES || backend == GraphicsBackend.Vulkan))
                throw new NotSupportedException($"Not support {backend} backend on iOS or Maccatalyst.");
            _backend = backend;

            View = view;
            View.ViewLoaded += CreateGraphicsDevice;
            View.SizeChanged += OnViewSizeChanged;
            View.ViewRemoved += DestroyGraphicsDevice;
        }

        private void RenderLoop()
        {
            if (GraphicsDevice != null)
            {
                try
                {
                    Game?.OnRender(16);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + e);
                }
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
                if (Animator != null)
                    Animator.cancel();
            }
        }

        private void CreateGraphicsDevice()
        {
            var Options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved, true, true);

            SwapchainSource ss = SwapchainSource.CreateUIView(View.Handle);
            SwapchainDescription scd = new SwapchainDescription(
                ss,
                (uint)View.Frame.Width,//MTLSwapchain内部自动转换成Pixel
                (uint)View.Frame.Height,
                PixelFormat.R32_Float,
                false);
            if (_backend == GraphicsBackend.Metal)
            {
                GraphicsDevice = GraphicsDevice.CreateMetal(Options, scd);
                SwapChain = GraphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.OpenGLES)
            {
                GraphicsDevice = GraphicsDevice.CreateOpenGLES(Options, scd);
                SwapChain = GraphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.Vulkan)
            {
                //need use MoltenVK nuget package
                GraphicsDevice = GraphicsDevice.CreateVulkan(Options, scd);
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
                //MTLSwapchain内部自动转换成Pixel
                SwapChain.Resize((uint)View.Frame.Width, (uint)View.Frame.Height);
                Game?.OnViewResize();
            }
        }
    }
}
