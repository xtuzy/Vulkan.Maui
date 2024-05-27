using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Vulkan.Maui.Shared
{
    public abstract class GameBase : IDisposable
    {
        public IGpuView GpuView { get; internal set; }

        public GameBase()
        {
        }

        /// <summary>
        /// The platform's graphics device is ready, we can create <see cref="Silk.NET.Vulkan.Device"/>, <see cref="Silk.NET.Vulkan.Instance"/>, and set resources.
        /// </summary>
        public virtual void OnGraphicsDeviceCreated()
        {
        }

        /// <summary>
        /// When platform's uiview be removed, the platform's graphics device will be destroyed, we can clean some memory.
        /// </summary>
        public virtual void OnGraphicsDeviceDestroyed()
        {
        }

        /// <summary>
        /// draw something on loop.
        /// </summary>
        /// <param name="deltaMillisecond"></param>
        public abstract void DrawFrame(float deltaMillisecond);

        /// <summary>
        /// When size of uiview be changed.
        /// </summary>
        public virtual void OnViewResize() { }

        public virtual void Dispose()
        {
        }
    }
}
