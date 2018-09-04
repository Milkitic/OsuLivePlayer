using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuLivePlayer.Util.GdipUtil;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using Gdip = System.Drawing;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class TestLayer : DxLayer
    {
        private StringObject[] _titleObjs;
        private const float X = 10, Y = 100;
        public TestLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel) : base(renderTarget, settings, osuModel)
        {
            string ok = "影翔鼓舞 - Oriental Blossom";
            var font = new Gdip.Font("等线", 25, Gdip.FontStyle.Bold);
            var brush = new Gdip.SolidBrush(Gdip.Color.White);
            D2D.Bitmap[] bmps = StringUtil.GetCharsBitmap(ok, font, brush).Select(renderTarget.LoadBitmap).ToArray();
            _titleObjs = new StringObject[ok.Length];
            for (var i = 0; i < _titleObjs.Length; i++)
                _titleObjs[i] = new StringObject(RenderTarget, bmps[i], Origin.TopLeft, new Mathe.RawPoint(0, 0));
        }

        public override void Measure()
        {
            if (_titleObjs.All(t => t.IsFinished))
            {
                string ok = "影翔鼓舞 - Oriental Blossom";
                var font = new Gdip.Font("等线", 25, Gdip.FontStyle.Bold);
                var brush = new Gdip.SolidBrush(Gdip.Color.White);
                D2D.Bitmap[] bmps = StringUtil.GetCharsBitmap(ok, font, brush).Select(RenderTarget.LoadBitmap).ToArray();
                _titleObjs = new StringObject[ok.Length];
                for (var i = 0; i < _titleObjs.Length; i++)
                    _titleObjs[i] = new StringObject(RenderTarget, bmps[i], Origin.Centre, new Mathe.RawPoint(0, 0));
                LogUtil.LogInfo("Refresh");
            }
        }

        public override void Draw()
        {
            float xOffset = 0;
            int step = 0;
            for (var i = 0; i < _titleObjs.Length; i++)
            {
                var item = _titleObjs[i];
                if (item == null) continue;
                item.StartDraw();
                item.Move(EasingEnum.QuadInOut, 0 + i * step, 500 + i * step, new Gdip.PointF(X + xOffset, Y + 50),
                    new Gdip.PointF(X + xOffset, Y + 50));
                //item.Move(EasingEnum.QuadInOut, 500 + i * step, 1000 + i * step, new Gdip.PointF(X + xOffset, Y),
                //    new Gdip.PointF(X + xOffset, Y + 50));
                //item.Fade(EasingEnum.EasingOut, 0 + i * step, 500 + i * step, 1, 0);
                //item.Fade(EasingEnum.EasingOut, 500 + i * step, 1000 + i * step, 0, 1);
                item.Rotate(EasingEnum.Linear, 0 + i * step, 1000 + i * step, 0, 360);
                item.ScaleVec(EasingEnum.Linear, 0 + i * step, 500 + i * step, 1, 1, 2, 2);
                item.ScaleVec(EasingEnum.Linear, 500 + i * step, 1000 + i * step, 2, 2, 1, 1);
                item.EndDraw();
                xOffset += item.Width;
            }
        }

        public override void Dispose()
        {

        }
    }
}
