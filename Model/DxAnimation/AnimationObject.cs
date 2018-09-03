using System;
using System.Diagnostics;
using System.Linq;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;
using Gdip = System.Drawing;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal abstract class AnimationObject 
    {
        public abstract void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity,
            float endOpacity);
        public abstract void Move(EasingEnum easingEnum, int startTime, int endTime, Gdip.PointF startPoint,
            Gdip.PointF endPoint);

        public abstract void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth,
            float startHeight, float endWidth, float endHeight);

        protected struct TimeRange
        {
            public int Max { get; set; }
            public int Min { get; set; }

            public static TimeRange Default => new TimeRange { Max = int.MaxValue, Min = int.MinValue };

            public static int GetMaxTime(params TimeRange[] timeRanges) => timeRanges.Max(t => t.Max);
            public static int GetMinTime(params TimeRange[] timeRanges) => timeRanges.Min(t => t.Min);
        }
        
        protected struct Static<T>
        {
            public Static(T defaultValue) : this() => Default = defaultValue;

            public T Default { get; set; }
            public T Source { get; set; }
            public T RealTime { get; set; }
            public T Target { get; set; }

            public void TargetToRealTime() => Target = RealTime;
            public void RealTimeToSource() => RealTime = Source;
            public void RealTimeToTarget() => RealTime = Target;
        }
    }
}
