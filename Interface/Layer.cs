using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Model;
using OsuLivePlayer.Util.DxUtil;
using SharpDX.Direct2D1;

namespace OsuLivePlayer.Interface
{
    public abstract class Layer
    {
        protected Layer(RenderTarget renderTarget, DxLoadSettings settings, OsuModel osuModel)
        {
            OsuModel = osuModel;
            Settings = settings;
            RenderTarget = renderTarget;
        }

        protected readonly OsuModel OsuModel;
        protected readonly DxLoadSettings Settings;
        protected readonly RenderTarget RenderTarget;
        public abstract void Measure();
        public abstract void Draw();
        public abstract void Dispose();
    }
}
