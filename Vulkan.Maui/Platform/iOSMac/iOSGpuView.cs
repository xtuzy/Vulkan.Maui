using CoreAnimation;
using CoreGraphics;
using System.Numerics;
using UIKit;
using Vulkan.Maui.Shared;
namespace Vulkan.Maui
{
    public class iOSGpuView : UIView, IGpuView
    {
        public iOSGpuView()
        {
            this.Layer.MasksToBounds = true; // IMPORTANT
        }

        CGSize oldFrame = CGSize.Empty;
        /// <summary>
        /// 构建Graphics Device时大小不能为0, Maui会调用此方法计算UIView的大小,因此在该方法中判断何时大小不为0
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override CGSize SizeThatFits(CGSize size)
        {
            var result = base.SizeThatFits(size);
            if (this.isFirstTimeLoad && result.Width > 0 && result.Height > 0)// For the first time, it has a non-zero size.
            {
                AppInfo = new VulkanAppInfo();
                Game?.OnGraphicsDeviceCreated();
                isFirstTimeLoad = false;
                oldFrame = result;
            }
            else if (result != oldFrame)//update size
            {
                game?.OnViewResize();
                oldFrame = result;
            }
            return result;
        }

        bool isFirstTimeLoad = true;
        public override void MovedToWindow()
        {
            base.MovedToWindow();
            /*if (firstTimeLoad)
            {
                ViewLoaded?.Invoke();
                firstTimeLoad = false;
            }
            else*/
            if (!isFirstTimeLoad)
            {
                Game?.OnGraphicsDeviceDestroyed();
                isFirstTimeLoad = true;
            }
        }

        public Vector2 FramebufferSize => new Vector2((float)(this.Frame.Width * DeviceDisplay.Current.MainDisplayInfo.Density), (float)(this.Frame.Width * DeviceDisplay.Current.MainDisplayInfo.Density));

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
