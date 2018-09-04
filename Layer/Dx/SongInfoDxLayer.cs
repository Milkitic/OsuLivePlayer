using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.DxAnimation;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuLivePlayer.Util.GdipUtil;
using OsuRTDataProvider.Listen;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using Gdip = System.Drawing;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class SongInfoDxLayer : DxLayer
    {
        //private D2D.Bitmap _newTitleBmp, _newArtistBmp, _oldTitleBmp, _oldArtistBmp;
        private UcodeString _newArtist = UcodeString.Default,
            _newTitle = UcodeString.Default,
            _oldArtist = UcodeString.Default,
            _oldTitle = UcodeString.Default;

        private StringObject[] _newTitleObjs, _newArtistObjs, _oldTitleObjs, _oldArtistObjs;
        private float[] _newTitleRndX, _newTitleRndY;
        private float[] _newArtistRndX, _newArtistRndY;
        private float[] _oldTitleRndX, _oldTitleRndY;
        private float[] _oldArtistRndX, _oldArtistRndY;

        private readonly OsuListenerManager.OsuStatus _status;
        private readonly Gdip.FontFamily _westernF, _easternF;
        private readonly Gdip.Font _wAFont, _eAFont, _wTFont, _eTFont;
        private readonly Gdip.Brush _wBrush, _eBrush;

        // Overall control
        private static bool _isStart;
        private bool PreferUcode => Settings.Preference.PreferUnicode;

        // Effect control
        private int _artistX = 50, _artistY = 30; // todo: Make them configurable
        private int _titleX = 50, _titleY = 65;

        private readonly Random _rnd = new Random();

        public SongInfoDxLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel)
            : base(renderTarget, settings, osuModel)
        {
            FileInfo eastInfo =
                new FileInfo(Path.Combine(OsuLivePlayerPlugin.GeneralConfig.WorkPath, "df-gokubutokaisho-w12.ttc"));
            FileInfo westInfo = new FileInfo(Path.Combine(OsuLivePlayerPlugin.GeneralConfig.WorkPath, "MOD20.ttf"));
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

            _eTFont = new Gdip.Font(_easternF, 25);
            _wTFont = new Gdip.Font(_westernF, 25);
            _eAFont = new Gdip.Font(_easternF, 20);
            _wAFont = new Gdip.Font(_westernF, 20);

            _wBrush = new Gdip.SolidBrush(Gdip.Color.White);
            _eBrush = new Gdip.SolidBrush(Gdip.Color.White);

            _status = OsuModel.Status;
        }

        public override void Measure()
        {
            if (!_isStart && _status != OsuModel.Status)
                _isStart = true;

            if (!_isStart) return;

            _oldArtist = _newArtist;
            _newArtist = new UcodeString(OsuModel.Idle.NowMap.Artist, OsuModel.Idle.NowMap.ArtistUnicode, PreferUcode);
            if (_oldArtist.ToPreferredString() != _newArtist.ToPreferredString())
            {
                Gdip.Font nFont = _newArtist.IsWestern ? _wAFont : _eAFont;
                Gdip.Brush nBrush = _newArtist.IsWestern ? _wBrush : _eBrush;

                _oldArtistObjs = _newArtistObjs?.Select(k => k?.Reset(new Mathe.RawPoint(0, 0))).ToArray();
                string artist = _newArtist.ToPreferredString();
                D2D.Bitmap[] bmps = StringUtil.GetCharsBitmap(artist, nFont, nBrush).Select(RenderTarget.LoadBitmap).ToArray();
                _newArtistObjs = new StringObject[artist.Length];

                _oldArtistRndX = _newArtistRndX;
                _oldArtistRndY = _newArtistRndY;
                for (var i = 0; i < _oldArtistRndX?.Length; i++)
                {
                    _oldArtistRndX[i] = (float)_rnd.NextDouble() * 50 - 100;
                    _oldArtistRndY[i] = (float)_rnd.NextDouble() * 100 - 50;
                }

                _newArtistRndX = new float[artist.Length];
                _newArtistRndY = new float[artist.Length];

                for (var i = 0; i < _newArtistObjs.Length; i++)
                {
                    _newArtistObjs[i] = new StringObject(RenderTarget, bmps[i], new Mathe.RawPoint(0, 0));
                    _newArtistRndX[i] = (float)_rnd.NextDouble() * 50 + 50;
                    _newArtistRndY[i] = (float)_rnd.NextDouble() * 100 - 50;
                }
            }

            _oldTitle = _newTitle;
            _newTitle = new UcodeString(OsuModel.Idle.NowMap.Title, OsuModel.Idle.NowMap.TitleUnicode, PreferUcode);
            if (_oldTitle.ToPreferredString() != _newTitle.ToPreferredString())
            {
                Gdip.Font nFont = _newTitle.IsWestern ? _wTFont : _eTFont;
                Gdip.Brush nBrush = _newTitle.IsWestern ? _wBrush : _eBrush;

                _oldTitleObjs = _newTitleObjs?.Select(k => k?.Reset(new Mathe.RawPoint(0, 0))).ToArray();
                string title = _newTitle.ToPreferredString();
                D2D.Bitmap[] bmps = StringUtil.GetCharsBitmap(title, nFont, nBrush).Select(RenderTarget.LoadBitmap).ToArray();
                _newTitleObjs = new StringObject[title.Length];

                _oldTitleRndX = _newTitleRndX;
                _oldTitleRndY = _newTitleRndY;
                for (var i = 0; i < _oldTitleRndX?.Length; i++)
                {
                    _oldTitleRndX[i] = (float)_rnd.NextDouble() * 50 - 100;
                    _oldTitleRndY[i] = (float)_rnd.NextDouble() * 100 - 50;
                }

                _newTitleRndX = new float[title.Length];
                _newTitleRndY = new float[title.Length];

                for (var i = 0; i < title.Length; i++)
                {
                    _newTitleObjs[i] = new StringObject(RenderTarget, bmps[i], new Mathe.RawPoint(0, 0));
                    _newTitleRndX[i] = (float)_rnd.NextDouble() * 50 + 50;
                    _newTitleRndY[i] = (float)_rnd.NextDouble() * 100 - 50;
                }
            }
        }

        public override void Draw()
        {
            if (!_isStart) return;

            float xOffset = 0;
            int startT = 50, effctT = 500, stepT = 10;

            if (_newArtistObjs != null)
                for (var i = 0; i < _newArtistObjs.Length; i++)
                {
                    var item = _newArtistObjs[i];
                    if (item == null) continue;
                    item.StartDraw();
                    item.Move(EasingEnum.EasingOut, startT + i * stepT, startT + effctT + i * stepT,
                        new Gdip.PointF(_artistX + _newArtistRndX[i] + xOffset, _artistY + _newArtistRndY[i]),
                        new Gdip.PointF(_artistX + xOffset, _artistY));
                    item.Fade(EasingEnum.EasingOut, 0 + i * stepT, effctT + i * stepT, 0, 1);
                    item.EndDraw();
                    xOffset += item.Width - 21;
                }

            startT = 75;
            xOffset = 0;
            effctT = 400;
            stepT = 8;
            if (_newTitleObjs != null)
                for (var i = 0; i < _newTitleObjs.Length; i++)
                {
                    var item = _newTitleObjs[i];
                    if (item == null) continue;
                    item.StartDraw();
                    item.Move(EasingEnum.EasingOut, startT + i * stepT, startT + effctT + i * stepT,
                        new Gdip.PointF(_titleX + _newTitleRndX[i] + xOffset, _titleY + _newTitleRndY[i]),
                        new Gdip.PointF(_titleX + xOffset, _titleY));
                    item.Fade(EasingEnum.EasingOut, 0 + i * stepT, effctT + i * stepT, 0, 1);
                    item.EndDraw();
                    xOffset += item.Width - 21;
                }

            startT = 0;
            xOffset = 0;
            if (_oldArtistObjs != null)
                for (var i = 0; i < _oldArtistObjs.Length; i++)
                {
                    var item = _oldArtistObjs[i];
                    if (item == null) continue;
                    item.StartDraw();
                    item.Move(EasingEnum.EasingIn, startT + i * stepT, startT + effctT + i * stepT,
                        new Gdip.PointF(_artistX + xOffset, _artistY),
                        new Gdip.PointF(_artistX + _oldArtistRndX[i] + xOffset, _artistY + _oldArtistRndY[i]));
                    item.Fade(EasingEnum.EasingIn, 0 + i * stepT, effctT + i * stepT, 1, 0);
                    item.EndDraw();
                    xOffset += item.Width - 21;
                }

            startT = 25;
            xOffset = 0;
            if (_oldTitleObjs != null)
                for (var i = 0; i < _oldTitleObjs.Length; i++)
                {
                    var item = _oldTitleObjs[i];
                    if (item == null) continue;
                    item.StartDraw();
                    item.Move(EasingEnum.EasingIn, startT + i * stepT, startT + effctT + i * stepT,
                        new Gdip.PointF(_titleX + xOffset, _titleY),
                        new Gdip.PointF(_titleX + _oldTitleRndX[i] + xOffset, _titleY + _oldTitleRndY[i]));
                    item.Fade(EasingEnum.EasingIn, 0 + i * stepT, effctT + i * stepT, 1, 0);
                    item.EndDraw();
                    xOffset += item.Width - 21;
                }
        }

        public override void Dispose()
        {
            if (_newTitleObjs != null) foreach (var t in _newTitleObjs) t?.Dispose();
            if (_newArtistObjs != null) foreach (var t in _newArtistObjs) t?.Dispose();
            if (_oldTitleObjs != null) foreach (var t in _oldTitleObjs) t?.Dispose();
            if (_oldArtistObjs != null) foreach (var t in _oldArtistObjs) t?.Dispose();
            _westernF?.Dispose();
            _easternF?.Dispose();
            _wAFont?.Dispose();
            _eAFont?.Dispose();
            _wTFont?.Dispose();
            _eTFont?.Dispose();
            _wBrush?.Dispose();
            _eBrush?.Dispose();
        }

        private class UcodeString
        {
            public readonly string Str;
            public readonly string Ustr;
            private readonly bool _useUcode;

            public bool IsWestern => !_useUcode || (Ustr == Str || string.IsNullOrEmpty(Ustr));

            public UcodeString(string str, string ustr, bool useUcode)
            {
                Str = str;
                Ustr = ustr;
                _useUcode = useUcode;
            }

            public string ToPreferredString() => _useUcode ? string.IsNullOrEmpty(Ustr) ? Str : Ustr : Str;
            public override string ToString() => string.IsNullOrEmpty(Ustr) ? Str : Ustr;

            public static UcodeString Default => new UcodeString("", "", false);
        }
    }
}
