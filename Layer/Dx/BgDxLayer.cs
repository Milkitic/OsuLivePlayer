using Milkitic.OsuLib.Model;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuRTDataProvider.Listen;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;
using WIC = SharpDX.WIC;

namespace OsuLivePlayer.Layer.Dx
{
    internal class BgDxLayer : DxLayer
    {
        private readonly D2D.Bitmap _defaultBg, _coverBg;

        private D2D.Bitmap _oldBg, _newBg;
        private Mathe.RawRectangleF _fixedRectOld, _fixedRect;
        private readonly Mathe.RawRectangleF _windowRect;

        private string _crtMapPath = "", _crtMapBg = "", _crtMapFile = "";
        private BitmapObject _newBgObj, _oldBgObj, _newBgObjEffect;
        private bool _lastBgIsNull;

        private readonly OsuListenerManager.OsuStatus _status;
        private readonly Random _rnd = new Random();

        // Overall control
        private static bool _isStart;

        // Effect control;
        private int _transformStyle;
        private double[] _barList, _11List;
        private OsuFile.TimeRange[] _kiaiList;
        private readonly Stopwatch _sw = new Stopwatch();

        public BgDxLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel)
            : base(renderTarget, settings, osuModel)
        {
            _status = OsuModel.Status;
            string defName = "default.png";
            string covName = "cover.png";
            var defBgPath = Path.Combine(OsuLivePlayerPlugin.GeneralConfig.WorkPath, defName);
            var covBgPath = Path.Combine(OsuLivePlayerPlugin.GeneralConfig.WorkPath, covName);
            if (File.Exists(defBgPath))
                _defaultBg = renderTarget.LoadBitmap(defBgPath);
            else
                LogUtil.LogError($"Can not find \"{defName}\"");

            if (File.Exists(covBgPath))
                _coverBg = renderTarget.LoadBitmap(covBgPath);
            else
                LogUtil.LogError($"Can not find \"{covName}\"");

            var size = Settings.Render.WindowSize;
            _windowRect = new Mathe.RawRectangleF(0, 0, size.Width, size.Height);
        }

        public override void Measure() //todo: Will lead to NullReferenceException when recreated window on some maps: s/552702
        {
            if (!_isStart && _status != OsuModel.Status)
                _isStart = true;

            if (!_isStart) return;

            if (_crtMapFile != OsuModel.Idle.NowMap.FilenameFull)
            {
                string oldMapBg = _crtMapBg;
                string oldMapPath = _crtMapPath;
                _crtMapFile = OsuModel.Idle.NowMap.FilenameFull;
                _crtMapBg = OsuModel.Idle.NowMap.BackgroundFilename;
                _crtMapPath = OsuModel.Idle.NowMap.Folder;

                LogUtil.LogInfo((_crtMapBg == null).ToString());
                if (oldMapPath != _crtMapPath || oldMapBg != _crtMapBg)
                {
                    var currentBgPath = Path.Combine(_crtMapPath, OsuModel.Idle.NowMap.BackgroundFilename);
                    if (File.Exists(currentBgPath))
                    {
                        _oldBg = _newBg;
                        _newBg = RenderTarget.LoadBitmap(currentBgPath);
                        _lastBgIsNull = false;
                    }
                    else if (_defaultBg != null)
                    {
                        if (_lastBgIsNull) return;
                        _oldBg = _newBg;
                        _newBg = _defaultBg;
                        _lastBgIsNull = true;
                    }
                    else
                        return;

                    _fixedRectOld = _fixedRect;

                    _fixedRect = GetBgPosition(_newBg.Size);
                    var size = Settings.Render.WindowSize;
                    if (_newBg != null)
                    {
                        _newBgObj = new BitmapObject(RenderTarget, _newBg, Origin.Default,
                            new Mathe.RawPoint(size.Width / 2, size.Height / 2));
                        _newBgObjEffect = new BitmapObject(RenderTarget, _newBg, Origin.Default,
                            new Mathe.RawPoint(size.Width / 2, size.Height / 2), true);
                    }
                    if (_oldBg != null)
                        _oldBgObj = new BitmapObject(RenderTarget, _oldBg, Origin.Default,
                            new Mathe.RawPoint(size.Width / 2, size.Height / 2));
                    _transformStyle = _rnd.Next(0, 3);
                    if (_sw.ElapsedMilliseconds < 600 && _sw.ElapsedMilliseconds != 0)
                        _transformStyle = 99;
                }

                try
                {
                    OsuFile file = new OsuFile(OsuModel.Idle.NowMap.FilenameFull);
                    _barList = file.GetTimingBars();
                    _11List = file.GetTimings(1);
                    _kiaiList = file.GetTimingKiais();
                }
                catch (NotSupportedException e)
                {
                    LogUtil.LogError(e.Message);
                    _barList = new double[0];
                    _11List = new double[0];
                    _kiaiList = new OsuFile.TimeRange[0];
                }

                _sw.Restart();
            }
        }

        public override void Draw()
        {
            if (!_isStart) return;

            if (_oldBg != null)
            {
                _oldBgObj.StartDraw();
                _oldBgObj.Fade(EasingEnum.Linear, 0, 300, 1, 1);
                _oldBgObj.FreeRect(EasingEnum.Linear, 0, 0, _fixedRectOld, _fixedRectOld);
                _oldBgObj.EndDraw();
            }
            if (_newBg != null)
            {
                float w = 100, h = w * (_fixedRect.Bottom - _fixedRect.Top) / (_fixedRect.Right - _fixedRect.Left);

                _newBgObj.StartDraw();
                switch (_transformStyle)
                {
                    case 0:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 300, 0, 1);
                        _newBgObj.FreeRect(EasingEnum.EasingOut, 0, 300,
                            new Mathe.RawRectangleF(_fixedRect.Left - w / 2, _fixedRect.Top - h / 2,
                                _fixedRect.Right + w / 2, _fixedRect.Bottom + h / 2), _fixedRect);
                        break;
                    case 1:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 300, 0, 1);
                        _newBgObj.FreeRect(EasingEnum.EasingOut, 0, 300, _fixedRect, _fixedRect);
                        break;
                    case 2:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 300, 0, 1);
                        _newBgObj.FreeRect(EasingEnum.EasingOut, 0, 300,
                            new Mathe.RawRectangleF(_fixedRect.Left + w / 2, _fixedRect.Top + h / 2,
                                _fixedRect.Right - w / 2, _fixedRect.Bottom - h / 2), _fixedRect);
                        break;
                    default:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 100, 0, 1);
                        _newBgObj.Rotate(EasingEnum.ElasticHalfOut, 0, 300, 30, 0);
                        _newBgObj.FreeRect(EasingEnum.ElasticHalfOut, 0, 300,
                            new Mathe.RawRectangleF(_fixedRect.Left + w, _fixedRect.Top + h * 2,
                                _fixedRect.Right - w * 2, _fixedRect.Bottom - h * 2), _fixedRect);
                        break;
                }

                _newBgObj.EndDraw();
            }

            if (_newBgObj != null && _newBgObj.IsFinished && _11List?.Length > 0)
            {
                _newBgObjEffect.StartDraw();

                for (var i = 0; i < _11List.Length; i++)
                {
                    var item = _11List[i];
                    int t = (int)item;
                    double effectT = i == _11List.Length - 1 ? 4000 : (_11List[i + 1] - t) / 1d;
                    float w = 20;
                    float h = w * (_fixedRect.Bottom - _fixedRect.Top) / (_fixedRect.Right - _fixedRect.Left);
                    if (_kiaiList.Any(k => k.StartTime <= item && k.EndTime >= item) && !_barList.Contains(item))
                    {
                        _newBgObjEffect.Fade(EasingEnum.EasingOut, t, t + (int)effectT, 1, 0);
                        _newBgObjEffect.FreeRect(EasingEnum.EasingOut, t, t + (int)effectT, _fixedRect,
                            new Mathe.RawRectangleF(_fixedRect.Left - w, _fixedRect.Top - h,
                                _fixedRect.Right + w, _fixedRect.Bottom + h));
                    }
                }

                for (var i = 0; i < _barList.Length; i++)
                {
                    var item = _barList[i];
                    int t = (int)item;
                    double effectT;
                    float fade = 1;
                    float w = 40;
                    var easingRetc = EasingEnum.EasingOut;
                    var easingFade = EasingEnum.EasingOut;
                    if (i == _barList.Length - 1)
                        effectT = 4000;
                    else
                    {
                        if (_kiaiList.Any(k => item >= k.StartTime && item < k.EndTime))
                        {
                            if (i != _barList.Length - 1 &&
                                _kiaiList.Any(k => item == k.StartTime && _barList[i + 1] > k.EndTime)) // 一闪而过的Kiai
                            {
                                effectT = (_barList[i + 1] - t) * 1;
                                w = 50;
                                easingRetc = EasingEnum.QuintOut;
                                easingFade = EasingEnum.EasingIn;
                                fade = 0.7f;
                            }
                            else
                            {
                                w = 30;
                                effectT = (_11List[1] - _11List[0]) * 1d;
                            }
                        }
                        else
                            effectT = (_barList[i + 1] - t) * 2d / 3;
                    }
                    float h = w * (_fixedRect.Bottom - _fixedRect.Top) / (_fixedRect.Right - _fixedRect.Left);

                    _newBgObjEffect.Fade(easingFade, t, t + (int)effectT, fade, 0);
                    _newBgObjEffect.FreeRect(easingRetc, t, t + (int)effectT, _fixedRect,
                        new Mathe.RawRectangleF(_fixedRect.Left - w, _fixedRect.Top - h,
                            _fixedRect.Right + w, _fixedRect.Bottom + h));
                }

                _newBgObjEffect.EndDraw();
                _newBgObjEffect.AdjustTime(OsuModel.GamePlay.CurrentOffset);
            }

            if (_coverBg != null)
                RenderTarget.DrawBitmap(_coverBg, _windowRect, 1, D2D.BitmapInterpolationMode.Linear);
        }

        public override void Dispose()
        {
            _defaultBg?.Dispose();
            _coverBg?.Dispose();
            _oldBg?.Dispose();
            _newBg?.Dispose();
            _newBgObj?.Dispose();
            _oldBgObj?.Dispose();
        }

        private Mathe.RawRectangleF GetBgPosition(Size2F originSize)
        {
            var windowSize = Settings.Render.WindowSize;
            var windowRatio = windowSize.Width / (float)windowSize.Height;

            var bgRatio = originSize.Width / originSize.Height;

            // deal with different size of image
            if (bgRatio >= windowRatio) // more width
            {
                float scale = windowSize.Height / originSize.Height;
                float height = windowSize.Height;
                float width = originSize.Width * scale;
                float x = -(width - windowSize.Width) / 2;
                float y = 0;
                return new Mathe.RawRectangleF(x, y, x + width, y + height);
            }
            else // more height
            {
                float scale = windowSize.Width / originSize.Width;
                float width = windowSize.Width;
                float height = originSize.Height * scale;
                float x = 0;
                float y = -(height - windowSize.Height) / 2;
                return new Mathe.RawRectangleF(x, y, x + width, y + height);
            }
        }
    }


}
