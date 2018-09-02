using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Util;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal class BitmapObject : AnimationObject
    {
        private readonly D2D.RenderTarget _target;
        private readonly D2D.Bitmap _bitmap;
        private bool _hasStarted, _hasFinished;
        private readonly Stopwatch _watch = new Stopwatch();

        private int MaxTime => Math.Max(_fadeMaxTime, _rectMaxTime);
        private int _fadeMaxTime = int.MaxValue, _fadeMinTime = int.MinValue;
        private int _rectMaxTime = int.MaxValue, _rectMinTime = int.MinValue;
        private int _rectInMaxTime = int.MaxValue, _rectInMinTime = int.MinValue;

        private Mathe.RawRectangleF RtRect => new Mathe.RawRectangleF(_rtX, _rtY, _rtX + _rtW, _rtY + _rtH);
        private Mathe.RawRectangleF RtInRect => new Mathe.RawRectangleF(_rtInX, _rtInY, _rtInX + _rtInW, _rtInY + _rtInH);
        private float _rtOpacity;
        private float _rtX, _rtY, _rtW, _rtH;
        private float _rtInX, _rtInY, _rtInW, _rtInH;

        private Mathe.RawRectangleF TarRect => new Mathe.RawRectangleF(_tarX, _tarY, _tarX + _tarW, _tarY + _tarH);
        private Mathe.RawRectangleF OriRect => new Mathe.RawRectangleF(_oriX, _oriY, _oriX + _oriW, _oriY + _oriH);

        private Mathe.RawRectangleF TarInRect => new Mathe.RawRectangleF(_tarInX, _tarInY, _tarInX + _tarInW, _tarInY + _tarInH);
        private Mathe.RawRectangleF OriInRect => new Mathe.RawRectangleF(_oriInX, _oriInY, _oriInX + _oriInW, _oriInY + _oriInH);

        private float _tarOpacity, _oriOpacity;
        private float _tarX, _tarY, _tarW, _tarH;
        private float _oriX, _oriY, _oriW, _oriH;
        private float _tarInX, _tarInY, _tarInW, _tarInH;
        private float _oriInX, _oriInY, _oriInW, _oriInH;

        private readonly bool _enableLog;

        public BitmapObject(D2D.RenderTarget target, D2D.Bitmap bitmap, Mathe.RawPoint posision, bool enableLog = false)
        {
            _target = target;
            _bitmap = bitmap;
            _rtX = posision.X;
            _rtY = posision.Y;
            _rtW = bitmap.Size.Width;
            _rtH = bitmap.Size.Height;
            _tarX = _rtX;
            _tarY = _rtY;
            _tarW = _rtW;
            _tarH = _rtH;

            _rtInX = 0;
            _rtInY = 0;
            _rtInW = bitmap.Size.Width;
            _rtInH = bitmap.Size.Height;
            _tarInX = _rtInX;
            _tarInY = _rtInY;
            _tarInW = _rtInW;
            _tarInH = _rtInH;

            _enableLog = enableLog;
        }

        public override void Move(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawPoint startPoint, Mathe.RawPoint endPoint)
        {
            if (_rectMaxTime == int.MaxValue || endTime > _rectMaxTime)
            {
                _rectMaxTime = endTime;
                _tarX = endPoint.X;
                _tarY = endPoint.Y;
            }

            // todo
        }

        public override void ScaleVec(EasingEnum easingEnum, int startTime, int endTime, float startWidth, float startHeight, float endWidth,
            float endHeight)
        {
            if (_rectMaxTime == int.MaxValue || endTime > _rectMaxTime)
            {
                _rectMaxTime = endTime;
                _tarW = endWidth;
                _tarH = endHeight;
            }

            // todo
        }


        public override void Fade(EasingEnum easingEnum, int startTime, int endTime, float startOpacity, float endOpacity)
        {
            if (_fadeMaxTime == int.MaxValue || endTime > _fadeMaxTime)
            {
                _fadeMaxTime = endTime;
                _tarOpacity = endOpacity;
            }

            if (_fadeMinTime == int.MinValue || startTime < _fadeMinTime)
            {
                _fadeMinTime = startTime;
                _oriOpacity = startOpacity;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _fadeMinTime)
            {
                _rtOpacity = _oriOpacity;
            }
            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _rtOpacity = startOpacity + (float)easingEnum.Ease(t) * (endOpacity - startOpacity);
            }

            if (ms >= _fadeMaxTime)
            {
                _rtOpacity = _tarOpacity;
            }
        }

        public void FreeRect(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawRectangleF startRect,
            Mathe.RawRectangleF endRect)
        {
            if (_rectMaxTime == int.MaxValue || endTime > _rectMaxTime)
            {
                _rectMaxTime = endTime;
                _tarX = endRect.Left;
                _tarY = endRect.Top;
                _tarW = endRect.Right - endRect.Left;
                _tarH = endRect.Bottom - endRect.Top;
            }

            if (_rectMinTime == int.MinValue || startTime < _rectMinTime)
            {
                _rectMinTime = startTime;
                _oriX = startRect.Left;
                _oriY = startRect.Top;
                _oriW = startRect.Right - startRect.Left;
                _oriH = startRect.Bottom - startRect.Top;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _rectMinTime)
            {
                _rtX = _oriX;
                _rtY = _oriY;
                _rtW = _oriW;
                _rtH = _oriH;
            }

            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _rtX = startRect.Left + (float)easingEnum.Ease(t) * (endRect.Left - startRect.Left);
                _rtY = startRect.Top + (float)easingEnum.Ease(t) * (endRect.Top - startRect.Top);
                float r = startRect.Right + (float)easingEnum.Ease(t) * (endRect.Right - startRect.Right);
                float b = startRect.Bottom + (float)easingEnum.Ease(t) * (endRect.Bottom - startRect.Bottom);
                _rtW = r - _rtX;
                _rtH = b - _rtY;
            }

            if (ms >= _rectMaxTime)
            {
                _rtX = _tarX;
                _rtY = _tarY;
                _rtW = _tarW;
                _rtH = _tarH;
            }
        }

        public void FreeCutRect(EasingEnum easingEnum, int startTime, int endTime, Mathe.RawRectangleF startRect,
            Mathe.RawRectangleF endRect)
        {
            if (_rectInMaxTime == int.MaxValue || endTime > _rectInMaxTime)
            {
                _rectInMaxTime = endTime;
                _tarInX = endRect.Left;
                _tarInY = endRect.Top;
                _tarInW = endRect.Right - endRect.Left;
                _tarInH = endRect.Bottom - endRect.Top;
            }

            if (_rectInMinTime == int.MinValue || startTime < _rectInMinTime)
            {
                _rectInMinTime = startTime;
                _oriInX = startRect.Left;
                _oriInY = startRect.Top;
                _oriInW = startRect.Right - startRect.Left;
                _oriInH = startRect.Bottom - startRect.Top;
            }

            float ms = _watch.ElapsedMilliseconds;
            if (!_hasFinished && ms <= _rectInMinTime)
            {
                _rtInX = _oriInX;
                _rtInY = _oriInY;
                _rtInW = _oriInW;
                _rtInH = _oriInH;
            }

            if (!_hasFinished && ms >= startTime && ms <= endTime)
            {
                var t = (ms - startTime) / (endTime - startTime);
                _rtInX = startRect.Left + (float)easingEnum.Ease(t) * (endRect.Left - startRect.Left);
                _rtInY = startRect.Top + (float)easingEnum.Ease(t) * (endRect.Top - startRect.Top);
                float r = startRect.Right + (float)easingEnum.Ease(t) * (endRect.Right - startRect.Right);
                float b = startRect.Bottom + (float)easingEnum.Ease(t) * (endRect.Bottom - startRect.Bottom);
                _rtInW = r - _rtInX;
                _rtInH = b - _rtInY;
            }

            if (ms >= _rectInMaxTime)
            {
                _rtInX = _tarInX;
                _rtInY = _tarInY;
                _rtInW = _tarInW;
                _rtInH = _tarInH;
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
                    if (_enableLog) LogUtil.LogInfo($"[{RtInRect.Left},{RtInRect.Top},{RtInRect.Right},{RtInRect.Bottom}]");
                }
            }
        }

        public void EndDraw()
        {
            if (!_hasFinished)
            {
                _target.DrawBitmap(_bitmap, RtRect, _rtOpacity, D2D.BitmapInterpolationMode.Linear/*, RtInRect*/); //todo
            }
            else
            {
                if (_enableLog) LogUtil.LogInfo($"[{TarInRect.Left},{TarInRect.Top},{TarInRect.Right},{TarInRect.Bottom}]");
                _target.DrawBitmap(_bitmap, TarRect, _tarOpacity, D2D.BitmapInterpolationMode.Linear/*, TarInRect*/); //todo
            }
        }
    }
}
