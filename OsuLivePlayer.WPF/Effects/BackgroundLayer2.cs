using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OSharp.Animation;
using OSharp.Animation.WPF;
using OsuRTDataProvider;
using OsuRTDataProvider.BeatmapInfo;

namespace OsuLivePlayer.WPF.Effects
{
    class BackgroundLayer2 : LayerBase
    {
        private OsuRTDataProviderPlugin _ortdpPlugin;
        private string _newPath;
        private string _oldPath;
        private Stopwatch _sw = new Stopwatch();
        private bool _cleared = false;
        public TimeSpan ClearInterval { get; set; } = TimeSpan.FromMilliseconds(10);

        public BackgroundLayer2(StoryboardCanvasHost group) : base(group)
        {
            _ortdpPlugin = MainWindow.OrtdpPlugin;
            Logger.LogInfo("background2 layer");
        }

        public override void StartListen()
        {
            var listener = _ortdpPlugin?.ListenerManager;
            if (listener != null)
            {
                listener.OnBeatmapChanged += OnBeatmapChanged;
                Logger.LogInfo("background layer ---> OnBeatmapChanged");
            }

            Task.Factory.StartNew(ClearCanvas, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(ClearEleList, TaskCreationOptions.LongRunning);
        }

        private void ClearEleList()
        {
            var sec = TimeSpan.FromSeconds(5);

            while (true)
            {
                Thread.Sleep(sec);
                Application.Current.Dispatcher?.Invoke(new Action(() =>
                {
                    var count = Group.ClearUnusefulElement();
                    if (count > 0)
                        Logger.LogDebug($"Cleared {count} storyboard objects.");
                }));
            }
        }

        private void ClearCanvas()
        {
            var ts = TimeSpan.FromMilliseconds(300);
            while (true)
            {
                if (_sw.Elapsed > ts && !_cleared)
                {
                    Application.Current.Dispatcher?.Invoke(new Action(() =>
                    {
                        int i = 0, j = 0;
                        for (var index = 0; index < Group.Canvas.Children.Count - 2;)
                        {
                            UIElement canvasChild = Group.Canvas.Children[index];
                            if (i > 1)
                            {
                                Group.Canvas.Children.Remove(canvasChild);

                                Logger.LogDebug($"cleared {(canvasChild as Image)?.Tag}");
                                j++;
                            }

                            i++;
                        }

                        if (j > 0)
                        {
                            Logger.LogInfo($"Cleared {j} UI elements");
                        }
                    }));

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    ClearInterval = TimeSpan.FromSeconds(3);
                    _sw.Stop();
                    _cleared = true;
                }
                else
                {
                    ClearInterval = TimeSpan.FromSeconds(3);
                }

                Thread.Sleep(ClearInterval);
            }
        }

        public override void StopListen()
        {
            var listener = _ortdpPlugin?.ListenerManager;
            if (listener != null)
            {
                listener.OnBeatmapChanged -= OnBeatmapChanged;
                Logger.LogInfo("background layer -x-> OnBeatmapChanged");
            }
        }

        private void OnBeatmapChanged(Beatmap map)
        {
            var folder = map.Folder;
            var bgName = map.BackgroundFilename;
            _sw.Restart();
            _cleared = false;
            ClearInterval = TimeSpan.FromMilliseconds(10);
            Application.Current.Dispatcher?.Invoke(new Action(() =>
            {
                try
                {
                    _newPath = Path.Combine(folder, bgName);
                    Logger.LogInfo(bgName);
                    if (!File.Exists(_newPath)) return;
                    ImageHelper.GetImageAndSource(_newPath, out var nowBitmap, out var nowImage);

                    var newOriginSize = new Vector2<double>(nowBitmap.Width, nowBitmap.Height);
                    var newUniformSize = ImageHelper.GetUniformSize(newOriginSize.X, newOriginSize.Y);
                    const double duration = 600;
                    if (true)
                    {
                        var newBg = Group.CreateElement(nowImage, Origins.Center, newOriginSize.X, newOriginSize.Y, -10);
                        newBg.ClearAfterFinish = false;

                        //obj.ApplyAnimation(_actionList[_rnd.Next(_actionList.Count)]);
                        newBg.ApplyAnimation(trans =>
                        {
                            var vec = new Vector2<double>(newUniformSize.X / newOriginSize.X,
                                newUniformSize.Y / newOriginSize.Y);
                            trans.ScaleVec(Easing.CubicOut,
                                0, duration,
                                new Vector2<double>(vec.X * 1.3, vec.Y * 1.3),
                                vec);
                            trans.Fade(Easing.CubicOut,
                                0, duration,
                                0, 1);
                        });

                        Group.Canvas.Children.Add(nowImage);
                        newBg.BeginAnimation();
                    }
                    _oldPath = _newPath;
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }));
        }
    }
}
