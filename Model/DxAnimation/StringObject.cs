using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Model.DxAnimation
{
    class StringObject:AnimationObject
    {
        public override void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity, float endOpacity)
        {
            throw new NotImplementedException();
        }

        public override void Move(EasingEnum easingEnum, int startTime, int endTime, RawPoint startPoint, RawPoint endPoint)
        {
            throw new NotImplementedException();
        }

        public override void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth, float startHeight, float endWidth,
            float endHeight)
        {
            throw new NotImplementedException();
        }
    }
}
