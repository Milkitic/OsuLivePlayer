using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuRTDataProvider.Listen;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;

namespace OsuLivePlayer.Layer.Dx
{
    internal class BgDxLayer : DxLayer
    {
        private readonly D2D.Bitmap _defaultBg, _coverBg;

        private D2D.Bitmap _oldBg, _newBg;
        private RectangleF _fixedRectOld, _fixedRect;
        private readonly RectangleF _windowRect;

        private string _currentMapPath;
        private BitmapObject _newBgObj, _oldBgObj;
        private bool _lastBgIsNull;

        private readonly OsuListenerManager.OsuStatus _status;
        private readonly Random _rnd = new Random();

        // Overall control
        private static bool _isStart;

        // Effect control;
        private int _transformStyle;

        public BgDxLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel)
            : base(renderTarget, settings, osuModel)
        {
            _currentMapPath = "";
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
            _windowRect = new RectangleF(0, 0, size.Width, size.Height);
        }

        public override void Measure() //todo: Will lead to NullReferenceException when recreated window on some maps: s/552702
        {
            if (!_isStart && _status != OsuModel.Status)
                _isStart = true;

            if (!_isStart) return;

            if (_currentMapPath != OsuModel.Idle.NowMap.Folder)
            {
                _currentMapPath = OsuModel.Idle.NowMap.Folder;
                var currentBgPath = Path.Combine(_currentMapPath, OsuModel.Idle.NowMap.BackgroundFilename);

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
                    _newBgObj = new BitmapObject(RenderTarget, _newBg,
                        new Point(size.Width / 2, size.Height / 2));
                if (_oldBg != null)
                    _oldBgObj = new BitmapObject(RenderTarget, _oldBg,
                        new Point(size.Width / 2, size.Height / 2));
                _transformStyle = _rnd.Next(0, 4);
            }
        }

        public override void Draw()
        {
            RenderTarget.Transform = new Matrix3x2
            {
                M11 = 1f, M12 = 0f,
                M21 = 0f, M22 = 1f,
                M31 = 100, M32 = 100
            };
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
                            new RectangleF(_fixedRect.Left - w / 2, _fixedRect.Top - h / 2,
                                _fixedRect.Right + w / 2, _fixedRect.Bottom + h / 2), _fixedRect);
                        break;
                    case 1:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 300, 0, 1);
                        _newBgObj.FreeRect(EasingEnum.EasingOut, 0, 300, _fixedRect, _fixedRect);
                        break;
                    case 3:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 100, 0, 1);
                        _newBgObj.FreeCutRect(EasingEnum.ElasticHalfOut, 0, 1000,
                            new RectangleF(0, 0, _newBg.Size.Width / 2f, _newBg.Size.Height),
                            new RectangleF(0, 0, _newBg.Size.Width, _newBg.Size.Height));
                        _newBgObj.FreeRect(EasingEnum.ElasticHalfOut, 0, 300, _fixedRect, _fixedRect);
                        break;
                    default:
                        _newBgObj.Fade(EasingEnum.EasingOut, 0, 300, 0, 1);
                        _newBgObj.FreeRect(EasingEnum.EasingOut, 0, 300,
                            new RectangleF(_fixedRect.Left + w / 2, _fixedRect.Top + h / 2,
                                _fixedRect.Right - w / 2, _fixedRect.Bottom - h / 2), _fixedRect);
                        break;
                }

                _newBgObj.EndDraw();
            }

            if (_coverBg != null)
                RenderTarget.DrawBitmap(_coverBg, _windowRect, 1, D2D.BitmapInterpolationMode.Linear);

            RenderTarget.Transform = new Matrix3x2
            {
                M11 = 1f, M12 = 0f,
                M21 = 0f, M22 = 1f,
                M31 = 0, M32 = 0
            };
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

        private RectangleF GetBgPosition(Size2F originSize)
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
                return new RectangleF(x, y, x + width, y + height);
            }
            else // more height
            {
                float scale = windowSize.Width / originSize.Width;
                float width = windowSize.Width;
                float height = originSize.Height * scale;
                float x = 0;
                float y = -(height - windowSize.Height) / 2;
                return new RectangleF(x, y, x + width, y + height);
            }
        }
    }


}
