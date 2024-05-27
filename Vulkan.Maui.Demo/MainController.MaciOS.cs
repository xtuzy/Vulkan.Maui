#if __IOS__
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Vulkan.Maui.Demo
{
    internal class MainController : UIViewController
    {
        public MainController(UIWindow window)
        {
            var gpuView = new VeldridPlatformView();
            var layout = new UIStackView(window!.Frame) { BackgroundColor = UIColor.Blue };
            layout.Add(gpuView);
            this.View = layout;
            var app = new VeldridApp(gpuView);
            app.Game = new HelloTriangle();
        }
    }
}
#endif
