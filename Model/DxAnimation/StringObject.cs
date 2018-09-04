using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Util.DxUtil;
using OsuLivePlayer.Util.GdipUtil;
using SharpDX;
using SharpDX.Direct2D1;
using Gdip = System.Drawing;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Model.DxAnimation
{
    internal class StringObject : BitmapObject
    {
        public StringObject(RenderTarget target, string words, Gdip.Font font, Gdip.Brush brush, Mathe.RawPoint initPosision,
            bool enableLog = false) : base(target, GetStringBitmap(target, words, font, brush), initPosision, enableLog) { }

        public StringObject(RenderTarget target, Bitmap bitmap, Mathe.RawPoint initPosision, bool enableLog = false)
            : base(target, bitmap, initPosision, enableLog)
        {
        }

        private static Bitmap GetStringBitmap(RenderTarget target, string words, Gdip.Font font, Gdip.Brush brush) =>
            target.LoadBitmap(StringUtil.GetStringBitmap(words, font, brush));

        public new StringObject Reset(Mathe.RawPoint posision) => new StringObject(Target, Bitmap, posision, EnableLog);
    }
}
