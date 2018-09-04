using OsuLivePlayer.Util;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using Gdip = System.Drawing;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal class BitmapObject : AnimationObject, IDisposable
    {
        public Size2F Size => Bitmap.Size;
        public float Width => Size.Width;
        public float Height => Size.Height;

        protected readonly D2D.RenderTarget Target;
        protected readonly D2D.Bitmap Bitmap;
        private readonly Stopwatch _watch = new Stopwatch();
        private D2D.Brush _brush;

        // control
        protected readonly bool EnableLog;
        private bool _hasStarted, _hasFinished;

        #region Private statics

        private TimeRange _fadeTime = TimeRange.Default;
        private TimeRange _rectTime = TimeRange.Default;
        private TimeRange _inRectTime = TimeRange.Default;
        private Static<float> _opacity;
        private Static<float> _x, _y, _w, _h;
        private Static<float> _inX, _inY, _inW, _inH;

        private int MaxTime => TimeRange.GetMaxTime(_fadeTime, _rectTime, _inRectTime);
        private int MinTime => TimeRange.GetMinTime(_fadeTime, _rectTime, _inRectTime);

        private Static<Mathe.RawRectangleF> InRect => new Static<Mathe.RawRectangleF>
        {
            Default = new Mathe.RawRectangleF(_inX.Default, _inY.Default, _inX.Default + _inW.Default, _inY.Default + _inH.Default),
            Source = new Mathe.RawRectangleF(_inX.Source, _inY.Source, _inX.Source + _inW.Source, _inY.Source + _inH.Source),
            RealTime =
                new Mathe.RawRectangleF(_inX.RealTime, _inY.RealTime, _inX.RealTime + _inW.RealTime, _inY.RealTime + _inH.RealTime),
            Target = new Mathe.RawRectangleF(_inX.Target, _inY.Target, _inX.Target + _inW.Target, _inY.Target + _inH.Target)
        };

        private Static<Mathe.RawRectangleF> Rect => new Static<Mathe.RawRectangleF>
        {
            Default = new Mathe.RawRectangleF(_x.Default, _y.Default, _x.Default + _w.Default, _y.Default + _h.Default),
            Source = new Mathe.RawRectangleF(_x.Source, _y.Source, _x.Source + _w.Source, _y.Source + _h.Source),
            RealTime =
                new Mathe.RawRectangleF(_x.RealTime, _y.RealTime, _x.RealTime + _w.RealTime, _y.RealTime + _h.RealTime),
            Target = new Mathe.RawRectangleF(_x.Target, _y.Target, _x.Target + _w.Target, _y.Target + _h.Target)
        };

        #endregion private statics

        public BitmapObject(D2D.RenderTarget target, D2D.Bitmap bitmap, Mathe.RawPoint initPosision, bool enableLog = false)
        {
            Target = target;
            Bitmap = bitmap;

            _x.RealTime = initPosision.X;
            _y.RealTime = initPosision.Y;
            _w.RealTime = bitmap.Size.Width;
            _h.RealTime = bitmap.Size.Height;
            _x.TargetToRealTime();
            _y.TargetToRealTime();
            _w.TargetToRealTime();
            _h.TargetToRealTime();

            _inX.RealTime = 0;
            _inY.RealTime = 0;
            _inW.RealTime = bitmap.Size.Width;
            _inH.RealTime = bitmap.Size.Height;
            _inX.TargetToRealTime();
            _inY.TargetToRealTime();
            _inW.TargetToRealTime();
            _inH.TargetToRealTime();

            EnableLog = enableLog;
            _brush = new D2D.BitmapBrush(Target, bitmap);
        }

        public override void Move(EasingEnum easingEnum, int startTime, int endTime, Gdip.PointF startPoint, Gdip.PointF endPoint)
        {
            if (_rectTime.Max == int.MaxValue || endTime > _rectTime.Max)
            {
                _rectTime.Max = endTime;
                _x.Target = endPoint.X;
                _y.Target = endPoint.Y;
            }

            if (_rectTime.Min == int.MinValue || startTime < _rectTime.Min)
            {
                _rectTime.Min = startTime;
                _x.Source = startPoint.X;
                _y.Source = startPoint.Y;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _rectTime.Min)
            {
                _x.RealTimeToSource();
                _y.RealTimeToSource();
            }

            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _x.RealTime = startPoint.X + (float)easingEnum.Ease(t) * (endPoint.X - startPoint.X);
                _y.RealTime = startPoint.Y + (float)easingEnum.Ease(t) * (endPoint.Y - startPoint.Y);
            }

            if (ms >= _rectTime.Max)
            {
                _x.RealTimeToTarget();
                _y.RealTimeToTarget();
            }
        }

        public override void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth, float startHeight, float endWidth,
            float endHeight)
        {
            if (_rectTime.Max == int.MaxValue || endTime > _rectTime.Max)
            {
                _rectTime.Max = endTime;
                _w.Target = endWidth;
                _h.Target = endHeight;
            }

            // todo
        }


        public override void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity, float endOpacity)
        {
            if (_fadeTime.Max == int.MaxValue || endTime > _fadeTime.Max)
            {
                _fadeTime.Max = endTime;
                _opacity.Target = endOpacity;
            }

            if (_fadeTime.Min == int.MinValue || startTime < _fadeTime.Min)
            {
                _fadeTime.Min = startTime;
                _opacity.Source = startOpacity;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _fadeTime.Min)
            {
                _opacity.RealTimeToSource();
            }
            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _opacity.RealTime = startOpacity + (float)easingEnum.Ease(t) * (endOpacity - startOpacity);
            }

            if (ms >= _fadeTime.Max)
            {
                _opacity.RealTimeToTarget();
            }
        }

        /// <summary>
        /// Do not use with any MOVE and any SCALE at same time!
        /// </summary>
        public void FreeRect(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawRectangleF startRect,
            Mathe.RawRectangleF endRect)
        {
            if (_rectTime.Max == int.MaxValue || endTime > _rectTime.Max)
            {
                _rectTime.Max = endTime;
                _x.Target = endRect.Left;
                _y.Target = endRect.Top;
                _w.Target = endRect.Right - endRect.Left;
                _h.Target = endRect.Bottom - endRect.Top;
            }

            if (_rectTime.Min == int.MinValue || startTime < _rectTime.Min)
            {
                _rectTime.Min = startTime;
                _x.Source = startRect.Left;
                _y.Source = startRect.Top;
                _w.Source = startRect.Right - startRect.Left;
                _h.Source = startRect.Bottom - startRect.Top;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _rectTime.Min)
            {
                _x.RealTimeToSource();
                _y.RealTimeToSource();
                _w.RealTimeToSource();
                _h.RealTimeToSource();
            }

            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _x.RealTime = startRect.Left + (float)easingEnum.Ease(t) * (endRect.Left - startRect.Left);
                _y.RealTime = startRect.Top + (float)easingEnum.Ease(t) * (endRect.Top - startRect.Top);
                float r = startRect.Right + (float)easingEnum.Ease(t) * (endRect.Right - startRect.Right);
                float b = startRect.Bottom + (float)easingEnum.Ease(t) * (endRect.Bottom - startRect.Bottom);
                _w.RealTime = r - _x.RealTime;
                _h.RealTime = b - _y.RealTime;
            }

            if (ms >= _rectTime.Max)
            {
                _x.RealTimeToTarget();
                _y.RealTimeToTarget();
                _w.RealTimeToTarget();
                _h.RealTimeToTarget();
            }
        }

        /// <summary>
        /// todo: Still have bugs.
        /// </summary>
        public void FreeCutRect(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawRectangleF startRect,
            Mathe.RawRectangleF endRect)
        {
            if (_inRectTime.Max == int.MaxValue || endTime > _inRectTime.Max)
            {
                _inRectTime.Max = endTime;
                _inX.Target = endRect.Left;
                _inY.Target = endRect.Top;
                _inW.Target = endRect.Right - endRect.Left;
                _inH.Target = endRect.Bottom - endRect.Top;
            }

            if (_inRectTime.Min == int.MinValue || startTime < _inRectTime.Min)
            {
                _inRectTime.Min = startTime;
                _inX.Source = startRect.Left;
                _inY.Source = startRect.Top;
                _inW.Source = startRect.Right - startRect.Left;
                _inH.Source = startRect.Bottom - startRect.Top;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _inRectTime.Min)
            {
                _inX.RealTimeToSource();
                _inY.RealTimeToSource();
                _inW.RealTimeToSource();
                _inH.RealTimeToSource();
            }

            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _inX.RealTime = startRect.Left + (float)easingEnum.Ease(t) * (endRect.Left - startRect.Left);
                _inY.RealTime = startRect.Top + (float)easingEnum.Ease(t) * (endRect.Top - startRect.Top);
                float r = startRect.Right + (float)easingEnum.Ease(t) * (endRect.Right - startRect.Right);
                float b = startRect.Bottom + (float)easingEnum.Ease(t) * (endRect.Bottom - startRect.Bottom);
                _inW.RealTime = r - _inX.RealTime;
                _inH.RealTime = b - _inY.RealTime;
            }

            if (ms >= _inRectTime.Max)
            {
                _inX.RealTimeToTarget();
                _inY.RealTimeToTarget();
                _inW.RealTimeToTarget();
                _inH.RealTimeToTarget();
            }
        }

        public void StartDraw()
        {
            if (!_hasStarted)
            {
                _watch.Start();
                _hasStarted = true;
            }
            else
            {
                if (_watch.ElapsedMilliseconds > MaxTime)
                {
                    _watch.Stop();
                    _watch.Reset();
                    _hasFinished = true;
                }

                if (!_hasFinished)
                {
                    if (EnableLog) LogUtil.LogInfo(string.Format("[{0},{1},{2},{3}]", InRect.RealTime.Left,
                        InRect.RealTime.Top, InRect.RealTime.Right, InRect.RealTime.Bottom));
                }
            }
        }

        public void EndDraw()
        {
            if (!_hasFinished)
            {
                //Target.FillOpacityMask(Bitmap, _brush, D2D.OpacityMaskContent.TextNatural, Rect.RealTime, null);

                Target.DrawBitmap(Bitmap, Rect.RealTime, _opacity.RealTime, D2D.BitmapInterpolationMode.Linear/*, RtInRect*/); //todo: bug
            }
            else
            {
                if (EnableLog) LogUtil.LogInfo(string.Format("[{0},{1},{2},{3}]", InRect.RealTime.Left,
                    InRect.RealTime.Top, InRect.RealTime.Right, InRect.RealTime.Bottom));
                //Target.FillOpacityMask(Bitmap, _brush, D2D.OpacityMaskContent.TextGdiCompatible, Rect.Target, null);
                Target.DrawBitmap(Bitmap, Rect.Target, _opacity.Target, D2D.BitmapInterpolationMode.Linear/*, TarInRect*/); //todo: bug
            }
        }

        public BitmapObject Reset(Mathe.RawPoint posision) => new BitmapObject(Target, Bitmap, posision, EnableLog);

        public void Dispose()
        {
            Target?.Dispose();
            Bitmap?.Dispose();
        }
    }
}
