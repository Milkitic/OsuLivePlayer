using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuRTDataProvider.Listen;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class BgDxLayer : DxLayer
    {
        private D2D.Bitmap _oldBg, _newBg;
        private string _currentMapPath;
        private Mathe.RawRectangleF _fixedRect;
        
        private readonly OsuListenerManager.OsuStatus _status;

        // Overall control
        private bool _isStart;

        public BgDxLayer(D2D.RenderTarget renderTarget, DxLoadSettings settings, OsuModel osuModel)
            : base(renderTarget, settings, osuModel)
        {
            _currentMapPath = "";
            _status = OsuModel.Status;
        }

        public override void Measure()
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
                    _newBg = RenderTarget.LoadBitmap(currentBgPath);
                    _fixedRect = GetBgPosition(_newBg.Size);
                }

                // todo
            }
        }

        public override void Draw()
        {
            if (!_isStart) return;

            if (_newBg != null)
                RenderTarget.DrawBitmap(_newBg, _fixedRect, 1, D2D.BitmapInterpolationMode.Linear);
            // todo
        }

        public override void Dispose()
        {
            _oldBg.Dispose();
            _newBg.Dispose();
        }

        private Mathe.RawRectangleF GetBgPosition(Size2F originSize)
        {
            var windowSize = Settings.RenderSettings.WindowSize;
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
