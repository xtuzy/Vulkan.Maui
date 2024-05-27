using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System.Diagnostics;
using System.Numerics;
using Vulkan.Maui.Shared;
namespace Vulkan.Maui
{
    public class AndroidGpuView : SurfaceView, ISurfaceHolderCallback, IGpuView
    {
        public AndroidGpuView(Context context):base(context)
        {
            Holder.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            AppInfo = new VulkanAppInfo()
            {
                SwapchainSource = SwapchainSource.CreateAndroidSurface(holder.Surface, JNIEnv.Handle)
            };

            Game?.OnGraphicsDeviceCreated();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Game?.OnGraphicsDeviceDestroyed();
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Game?.OnViewResize();
        }

        public Vector2 FramebufferSize => new Vector2(this.Width, this.Height);

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
