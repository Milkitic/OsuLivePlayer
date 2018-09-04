using System;
using System.Diagnostics;
using System.Linq;
using D2D = SharpDX.Direct2D1;
using Gdip = System.Drawing;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal abstract class AnimationObject
    {
        public abstract void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity,
            float endOpacity);
        public abstract void Move(EasingEnum easingEnum, int startTime, int endTime, Gdip.PointF startPoint,
            Gdip.PointF endPoint);
        public abstract void Rotate(EasingEnum easingEnum, int startTime, int endTime, float startDeg, float endDeg);
        public abstract void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth,
            float startHeight, float endWidth, float endHeight);

        protected struct TimeRange
        {
            public int Max { get; set; }
            public int Min { get; set; }

            public static TimeRange Default => new TimeRange { Max = int.MaxValue, Min = int.MinValue };

            public static int GetMaxTime(params TimeRange[] timeRanges) => timeRanges.Where(t => t.Max != int.MaxValue).Max(t => t.Max);
            public static int GetMinTime(params TimeRange[] timeRanges) => timeRanges.Where(t => t.Min != int.MinValue).Min(t => t.Min);
        }

        protected struct Static<T>
        {
            public T Source { get; set; }
            public T RealTime { get; set; }
            public T Target { get; set; }

            public static explicit operator Static<T>(T value) => new Static<T>
            {
                Source = value,
                RealTime = value,
                Target = value
            };

            public static explicit operator T(Static<T> value) => value.Source;

            public void TargetToRealTime() => Target = RealTime;
            public void RealTimeToSource() => RealTime = Source;
            public void RealTimeToTarget() => RealTime = Target;
        }
    }
}
