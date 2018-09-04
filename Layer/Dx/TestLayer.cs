using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util.DxUtil;
using OsuLivePlayer.Util.GdipUtil;
using SharpDX;
using Gdip = System.Drawing;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class TestLayer : DxLayer
    {
        private readonly StringObject[] _titleObjs;
        private const float X = 10, Y = 100;
        public TestLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel) : base(renderTarget, settings, osuModel)
        {
            string ok = "影翔鼓舞 - Oriental Blossom";
            var font = new Gdip.Font("等线", 12, Gdip.FontStyle.Bold);
            var brush = new Gdip.SolidBrush(Gdip.Color.White);
            D2D.Bitmap[] bmps = StringUtil.GetCharsBitmap(ok, font, brush).Select(renderTarget.LoadBitmap).ToArray();
            _titleObjs = new StringObject[ok.Length];
            for (var i = 0; i < _titleObjs.Length; i++)
                _titleObjs[i] = new StringObject(RenderTarget, bmps[i], new Mathe.RawPoint(0, 0));
        }

        public override void Measure()
        {

        }

        public override void Draw()
        {
            float xOffset = 0;
            for (var i = 0; i < _titleObjs.Length; i++)
            {
                var item = _titleObjs[i];
                if (item == null) continue;
                item.StartDraw();
                item.Move(EasingEnum.EasingOut, 0 + i * 50, 5000 + i * 50, new Gdip.PointF(X + xOffset, Y + 50),
                    new Gdip.PointF(X + xOffset, Y));
                item.Fade(EasingEnum.EasingOut, 0 + i * 50, 5000 + i * 50, 0, 1);
                item.EndDraw();
                xOffset += item.Width;
            }
        }

        public override void Dispose()
        {

        }
    }
}
