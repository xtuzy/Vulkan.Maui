#if ANDROID
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Diagnostics;
using Vulkan.Maui.Shared;
using Color = Android.Graphics.Color;
namespace Vulkan.Maui.Demo
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainController : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            if (false)
            {
                var gpuView = new VeldridPlatformView(this);
                var layout = new LinearLayout(this) { };
                layout.AddView(gpuView);
                layout.SetBackgroundColor(Color.Blue);
                SetContentView(layout);
                var app = new VeldridApp(gpuView, Veldrid.GraphicsBackend.Vulkan);
                app.Game = new HelloTriangle();
            }
            else
            {
                var gpuView = new AndroidGpuView(this);
                var layout = new LinearLayout(this) { };
                layout.AddView(gpuView);
                layout.SetBackgroundColor(Color.Blue);
                SetContentView(layout);
                (gpuView as IGpuView).Game = new HelloTriangleGame();
            }
        }
    }
}
#endif
