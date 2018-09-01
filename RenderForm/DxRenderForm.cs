using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Layer.Dx;
using OsuLivePlayer.Model;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuRTDataProvider.Listen;
using Sync;
using DX = SharpDX;
using D2D = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.RenderForm
{
    internal class DxRenderForm : Form
    {
        private static D2D.Factory Factory { get; } = new D2D.Factory(); // Factory for creating 2D elements
        private static D2D.RenderTarget RenderTarget { get; set; } // Target of rendering
        public static List<DxLayer> LayerList { get; set; }

        private Task[] _renderTask;

        private readonly bool _useVsync;
        private bool _preferUnicode;
        private readonly DxLoadSettings _settings;
        private readonly OsuModel _osuModel;

        public DxRenderForm(DxLoadSettings settings, OsuModel osuModel)
        {
            _settings = settings;
            _osuModel = osuModel;

            // Window settings
            ClientSize = settings.RenderSettings.WindowSize;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Render settings
            _useVsync = settings.RenderSettings.UseVsync;

            // Preference settings
            _preferUnicode = settings.PreferenceSettings.PreferUnicode;

            // Events
            Load += OnFormLoad;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (_osuModel != null)
                LogUtil.LogInfo("Form loads normally.");
            else
            {
                LogUtil.LogError("Form loads without real-time instance.");
                return;
            }

            Paint += OnPaint;
            // Initial settings
            var pixelFormat = new D2D.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Premultiplied);
            var winProp = new D2D.HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new DX.Size2(ClientSize.Width, ClientSize.Height),
                PresentOptions = _useVsync ? D2D.PresentOptions.None : D2D.PresentOptions.Immediately
            };
            var renderProp = new D2D.RenderTargetProperties(D2D.RenderTargetType.Default, pixelFormat, 96, 96,
                D2D.RenderTargetUsage.None, D2D.FeatureLevel.Level_DEFAULT);
            RenderTarget = new D2D.WindowRenderTarget(Factory, renderProp, winProp)
            {
                AntialiasMode = D2D.AntialiasMode.PerPrimitive,
                TextAntialiasMode = D2D.TextAntialiasMode.Grayscale,
                Transform = new Mathe.RawMatrix3x2 { M11 = 1f, M12 = 0f, M21 = 0f, M22 = 1f, M31 = 0, M32 = 0 }
            };

            LayerList = new List<DxLayer>
            {
                new BgDxLayer(RenderTarget, _settings, _osuModel)
            };

            _renderTask = new Task[LayerList.Count];

            // Avoid artifacts
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            Text = "Osu!Live Player (DX)";
            LogUtil.LogInfo("Form loaded.");
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) return;
            if (RenderTarget == null || RenderTarget.IsDisposed) return;

            // Begin rendering
            RenderTarget.BeginDraw();
            RenderTarget.Clear(new Mathe.RawColor4(0, 0, 0, 1));

            // Draw layers
            for (var i = 0; i < LayerList.Count; i++)
            {
                var item = LayerList[i];
                _renderTask[i] = Task.Run(() => { item.Measure(); });
            }
            Task.WaitAll(_renderTask);

            foreach (var item in LayerList)
                item.Draw();

            // End drawing
            RenderTarget.EndDraw();

            Invalidate();
        }

    }
}
