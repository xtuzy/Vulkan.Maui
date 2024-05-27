using MainApp = Android.App.Application;
using Android.Runtime;
using Android.App;
namespace Vulkan.Maui.Demo
{
    [Application]
    public class MainApplication : MainApp
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}
