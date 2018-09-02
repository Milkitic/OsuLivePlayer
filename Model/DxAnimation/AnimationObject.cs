using System;
using System.Diagnostics;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal abstract class AnimationObject
    {
        public abstract void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity,
            float endOpacity);
        public abstract void Move(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawPoint startPoint,
            Mathe.RawPoint endPoint);

        public abstract void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth,
            float startHeight, float endWidth, float endHeight);
    }
}
