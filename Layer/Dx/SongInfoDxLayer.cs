using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuLivePlayer.Util.GdipUtil;
using OsuRTDataProvider.Listen;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using Gdip = System.Drawing;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class SongInfoDxLayer : DxLayer
    {
        private D2D.Bitmap _newTitleBmp, _newArtistBmp, _oldTitleBmp, _oldArtistBmp;
        private UcodeString _newArtist, _newTitle;

        private Size2F OldTitleSize => _oldTitleBmp.Size;
        private Size2F OldArtistSize => _oldArtistBmp.Size;
        private Size2F NewTitleSize => _newTitleBmp.Size;
        private Size2F NewArtistSize => _newArtistBmp.Size;

        private Mathe.RawRectangleF OldTitleRect =>
            new Mathe.RawRectangleF(_titleX, _titleY, _titleX + OldTitleSize.Width, _titleY + OldTitleSize.Height);
        private Mathe.RawRectangleF OldArtistRect =>
            new Mathe.RawRectangleF(_artistX, _artistY, _artistX + OldArtistSize.Width, _artistY + OldArtistSize.Height);
        private Mathe.RawRectangleF NewTitleRect =>
            new Mathe.RawRectangleF(_titleX, _titleY, _titleX + NewTitleSize.Width, _titleY + NewTitleSize.Height);
        private Mathe.RawRectangleF NewArtistRect =>
            new Mathe.RawRectangleF(_artistX, _artistY, _artistX + NewArtistSize.Width, _artistY + NewArtistSize.Height);

        private BitmapObject _newTitleObj, _newArtistObj, _oldTitleObj, _oldArtistObj;

        private readonly OsuListenerManager.OsuStatus _status;
        private readonly Gdip.FontFamily _westernF, _easternF;

        // Overall control
        private bool _isStart;

        // Effect control
        private int _artistX = 50, _artistY = 30;
        private int _titleX = 50, _titleY = 65;

        public SongInfoDxLayer(D2D.RenderTarget renderTarget, DxLoadSettings settings, OsuModel osuModel)
            : base(renderTarget, settings, osuModel)
        {
            FileInfo eastInfo = new FileInfo(Path.Combine(OsuLivePlayerPlugin.Config.WorkPath, "df-gokubutokaisho-w12.ttc"));
            FileInfo westInfo = new FileInfo(Path.Combine(OsuLivePlayerPlugin.Config.WorkPath, "MOD20.ttf"));
            if (!eastInfo.Exists)
                _easternF = new Gdip.FontFamily("等线");
            else
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(eastInfo.FullName);
                _easternF = pfc.Families[0];
                LogUtil.LogInfo($"Eastern family was {(_easternF != null ? "ok" : "not found")}");
            }

            if (!westInfo.Exists)
                _westernF = new Gdip.FontFamily("等线");
            else
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(westInfo.FullName);
                _westernF = pfc.Families[0];
                LogUtil.LogInfo($"Western family was {(_westernF != null ? "ok" : "not found")}");
            }

            _status = OsuModel.Status;
        }

        public override void Measure()
        {
            if (!_isStart && _status != OsuModel.Status)
                _isStart = true;

            if (!_isStart) return;

            if (_newArtist.Str != OsuModel.Idle.NowMap.Artist || _newArtist.Ustr != OsuModel.Idle.NowMap.ArtistUnicode)
            {
                _newArtist = new UcodeString(OsuModel.Idle.NowMap.Artist, OsuModel.Idle.NowMap.ArtistUnicode);
                _oldArtistBmp = _newArtistBmp;

                using (Gdip.Font fArtist = _newArtist.IsWestern ? new Gdip.Font(_westernF, 20) : new Gdip.Font(_easternF, 20))
                using (var brush = new Gdip.SolidBrush(Gdip.Color.White))
                {
                    _newArtistBmp = RenderTarget.LoadBitmap(StringUtil.GetStringBitmap(_newArtist.ToString(), fArtist, brush));
                }

                if (_oldArtistBmp != null) _oldArtistObj = new BitmapObject(RenderTarget, _oldArtistBmp, new Mathe.RawPoint(0, 0));
                _newArtistObj = new BitmapObject(RenderTarget, _newArtistBmp, new Mathe.RawPoint(0, 0));
            }

            if (_newTitle.Str != OsuModel.Idle.NowMap.Title || _newTitle.Ustr != OsuModel.Idle.NowMap.TitleUnicode)
            {
                _newTitle = new UcodeString(OsuModel.Idle.NowMap.Title, OsuModel.Idle.NowMap.TitleUnicode);
                _oldTitleBmp = _newTitleBmp;

                using (Gdip.Font fTitle = _newTitle.IsWestern ? new Gdip.Font(_westernF, 30) : new Gdip.Font(_easternF, 30))
                using (var brush = new Gdip.SolidBrush(Gdip.Color.White))
                {
                    _newTitleBmp = RenderTarget.LoadBitmap(StringUtil.GetStringBitmap(_newTitle.ToString(), fTitle, brush));
                }
                if (_oldTitleBmp != null) _oldTitleObj = new BitmapObject(RenderTarget, _oldTitleBmp, new Mathe.RawPoint(0, 0));
                _newTitleObj = new BitmapObject(RenderTarget, _newTitleBmp, new Mathe.RawPoint(0, 0));
            }
        }

        public override void Draw()
        {
            if (!_isStart) return;

            if (_oldTitleObj != null)
            {
                _oldTitleObj.StartDraw();
                _oldTitleObj.Fade(EasingEnum.Linear, 0, 100, 1, 0);
                _oldTitleObj.FreeRect(EasingEnum.Linear, 0, 0, OldTitleRect, OldTitleRect);
                _oldTitleObj.EndDraw();
            }
            if (_oldArtistObj != null)
            {
                _oldArtistObj.StartDraw();
                _oldArtistObj.Fade(EasingEnum.Linear, 0, 100, 1, 0);
                _oldArtistObj.FreeRect(EasingEnum.Linear, 0, 0, OldArtistRect, OldArtistRect);
                _oldArtistObj.EndDraw();
            }

            if (_newTitleObj != null)
            {
                _newTitleObj.StartDraw();
                _newTitleObj.Fade(EasingEnum.Linear, 100, 200, 0, 1);
                _newTitleObj.FreeRect(EasingEnum.Linear, 0, 0, NewTitleRect, NewTitleRect);
                _newTitleObj.EndDraw();
            }

            if (_newArtistObj != null)
            {
                _newArtistObj.StartDraw();
                _newArtistObj.Fade(EasingEnum.Linear, 100, 200, 0, 1);
                _newArtistObj.FreeRect(EasingEnum.Linear, 0, 0, NewArtistRect, NewArtistRect);
                _newArtistObj.EndDraw();
            }

        }

        public override void Dispose()
        {

        }

        private struct UcodeString
        {
            public readonly string Str;
            public readonly string Ustr;

            public bool IsWestern => Ustr == Str || string.IsNullOrEmpty(Ustr);

            public UcodeString(string str, string ustr)
            {
                Str = str;
                Ustr = ustr;
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Ustr) ? Str : Ustr;
            }


        }
    }
}
