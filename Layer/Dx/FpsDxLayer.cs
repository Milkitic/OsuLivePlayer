using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Interface;
using OsuLivePlayer.Model;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Util.DxUtil;
using DW = SharpDX.DirectWrite;
using D2D = SharpDX.Direct2D1;
using Mathe = SharpDX.Mathematics.Interop;

namespace OsuLivePlayer.Layer.Dx
{
    internal class FpsDxLayer : DxLayer
    {
        private readonly DW.Factory _factoryWrite = new DW.Factory(); // For creating DirectWrite Elements
        private readonly DW.TextFormat _textFormat; // Text formats

        // Brushes
        private readonly D2D.Brush _whiteBrush;

        private readonly Stopwatch _sw = new Stopwatch();
        private readonly Stopwatch _bufferSw;
        private long _delay;
        private int _buffer;
        private readonly Queue<long> _delayQueue = new Queue<long>();
        private string _fps = "0 FPS";

        public FpsDxLayer(D2D.RenderTarget renderTarget, DxLoadObject settings, OsuModel osuModel) : base(renderTarget, settings, osuModel)
        {
            _whiteBrush = new D2D.SolidColorBrush(RenderTarget, new Mathe.RawColor4(1, 1, 1, 1));
            _textFormat = new DW.TextFormat(_factoryWrite, "Microsoft YaHei", 12);
            _bufferSw = new Stopwatch();
            _bufferSw.Start();
        }

        public override void Measure()
        {

        }

        public override void Draw()
        {
            _delay = _sw.ElapsedTicks;
            if (_delayQueue.Count >= 50)
                _delayQueue.Dequeue();
            _delayQueue.Enqueue(_delay);
            var tmp = Convert.ToInt32(_bufferSw.ElapsedMilliseconds / 100);

            if (_buffer != tmp)
            {
                _fps = Math.Floor(1 / (_delayQueue.AsEnumerable().Average() / Stopwatch.Frequency)) + " FPS";
                _buffer = tmp;
            }

            RenderTarget.DrawText(_fps, _textFormat, new Mathe.RawRectangleF(0, 0, 400, 200), _whiteBrush);

            _sw.Restart();
        }

        public override void Dispose()
        {
            _factoryWrite?.Dispose();
            _textFormat?.Dispose();
            _whiteBrush?.Dispose();
        }
    }
}
