using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util.DxUtil;
using SharpDX.Direct2D1;

namespace OsuLivePlayer.Interface
{
    internal abstract class DxLayer : Layer
    {
        protected DxLayer(RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel) : base(renderTarget, settings, osuModel)
        {
        }
    }
}
