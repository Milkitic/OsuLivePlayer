using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OSharp.Animation;
using OSharp.Animation.WPF;
using OsuRTDataProvider;
using OsuRTDataProvider.BeatmapInfo;

namespace OsuLivePlayer.WPF.Effects
{
    public class BackgroundLayer : LayerBase
    {
        private OsuRTDataProviderPlugin _ortdpPlugin;
        private Random _rnd = new Random();
        private Dictionary<string, Action<ITransformable<double>>> _actionDic;
        private List<Action<ITransformable<double>>> _actionList;

        private Vector2<double> _newUniformSize;
        private Vector2<double> _newOriginSize;
        private string _newPath;
        private Vector2<double> _oldUniformSize;
        private Vector2<double> _oldOriginSize;
        private string _oldPath;

        private ImageSource _oldBitmap;
        private Image _oldImage;
        private ImageSource _nowBitmap;
        private Image _nowImage;
        private int _nowIndex = int.MinValue;
        private int NowIndex
        {
            get
            {
                var i = _nowIndex++;
                if (i == int.MaxValue) _nowIndex = int.MinValue;
                return i;
            }
        }

        public BackgroundLayer(StoryboardCanvasHost group) : base(group)
        {
            _ortdpPlugin = MainWindow.OrtdpPlugin;
            Logger.LogInfo("background layer");
        }

        public override void StartListen()
        {
            var listener = _ortdpPlugin?.ListenerManager;
            if (listener != null)
            {
                listener.OnBeatmapChanged += OnBeatmapChanged;
                Logger.LogInfo("background layer ---> OnBeatmapChanged");
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

            Application.Current.Dispatcher?.BeginInvoke(new Action(() =>
            {
                try
                {
                    _newPath = Path.Combine(folder, bgName);

                    ClearCanvas();
                    ImageHelper.GetImageAndSource(_newPath, out _nowBitmap, out _nowImage);
                    CreateAndApplyAnimation();

                    _oldPath = _newPath;
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }));
        }

        private void CreateAndApplyAnimation()
        {
            _newOriginSize = new Vector2<double>(_nowBitmap.Width, _nowBitmap.Height);
            _newUniformSize = ImageHelper.GetUniformSize(_newOriginSize.X, _newOriginSize.Y);
            const double duration = 600;

            var firstCreate = CreateEffectsDictionary(duration);
            if (!firstCreate)
            {
                ImageHelper.GetImageAndSource(_oldPath, out _oldBitmap, out _oldImage);
                var oldBg = Group.CreateElement(_oldImage, Origins.Center, _oldOriginSize.X, _oldOriginSize.Y, -11);
                oldBg.ClearAfterFinish = false;
                oldBg.ApplyAnimation(trans =>
                {
                    var vec = new Vector2<double>(_oldUniformSize.X / _oldOriginSize.X, _oldUniformSize.Y / _oldOriginSize.Y);
                    trans.ScaleVec(Easing.CubicOut,
                        0, duration,
                        vec,
                        vec);
                });
            }

            var newBg = Group.CreateElement(_nowImage, Origins.Center, _newOriginSize.X, _newOriginSize.Y, -10);
            newBg.ClearAfterFinish = false;

            //obj.ApplyAnimation(_actionList[_rnd.Next(_actionList.Count)]);
            newBg.ApplyAnimation(_actionDic["FadeInAndScaleOut"]);
            Group.PlayWhole();

            _oldOriginSize = _newOriginSize;
            _oldUniformSize = _newUniformSize;
        }

        private bool CreateEffectsDictionary(double duration)
        {
            if (_actionDic != null) return false;

            _actionDic = new Dictionary<string, Action<ITransformable<double>>>
            {
                ["FadeInAndScaleOut"] = trans =>
                {
                    var vec = new Vector2<double>(_newUniformSize.X / _newOriginSize.X,
                        _newUniformSize.Y / _newOriginSize.Y);
                    trans.ScaleVec(Easing.CubicOut,
                        0, duration,
                        new Vector2<double>(vec.X * 1.3, vec.Y * 1.3),
                        vec);
                    trans.Fade(Easing.CubicOut,
                        0, duration,
                        0, 1);
                },
            };
            _actionList = _actionDic.Values.ToList();

            return true;
        }

        private void ClearCanvas()
        {
            if (!(Group is StoryboardGroup sg)) return;
            sg.Canvas.Children.Clear();
            sg.Storyboard?.Stop();
            sg.Storyboard?.Children.Clear();
        }
    }
}