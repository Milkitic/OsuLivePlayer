using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OSharp.Animation;

namespace OsuLivePlayer.WPF.Effects
{
    public static class ImageHelper
    {
        static ImageHelper()
        {
            Task.Factory.StartNew(() =>
            {
                var sec = TimeSpan.FromSeconds(30);

                while (true)
                {
                    Thread.Sleep(sec);
                    var now = DateTime.Now;
                    var sb = Bitmaps.Where(k => now - k.Value.Item1 > sec).ToList();
                    for (var i = 0; i < sb.Count; i++)
                    {
                        var keyValuePair = sb[i];
                        Bitmaps.TryRemove(keyValuePair.Key, out _);
                    }

                    Logger.LogDebug($"cleared {sb.Count} cached images");
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static readonly ConcurrentDictionary<string, (DateTime, ImageSource)> Bitmaps =
            new ConcurrentDictionary<string, (DateTime, ImageSource)>();

        public static void GetImageAndSource(string path, out ImageSource bitmap, out Image image)
        {
            if (!Bitmaps.ContainsKey(path))
            {
                ////new Uri(path)
                //bitmap = new BitmapImage();
                //bitmap.BeginInit();

                //bitmap.CacheOption = BitmapCacheOption.OnLoad;
                //bitmap.UriSource = new Uri(path);
                //bitmap.EndInit();
                //bitmap.Freeze();

                //Bitmaps.TryAdd(path, (DateTime.Now, bitmap));

                bitmap = CreateImageSource(path);


            }
            else
            {
                bitmap = Bitmaps[path].Item2;
            }

            image = new Image { Source = bitmap, Tag = Path.GetFileNameWithoutExtension(path) };
        }

        public static Vector2<double> GetUniformSize(double bitmapWidth, double bitmapHeight)
        {
            var bgRatio = bitmapWidth / bitmapHeight;
            double w = 854, h = 480;
            var canvasRatio = w / h;
            // deal with different size of image
            if (bgRatio >= canvasRatio) // more width
            {
                double scale = h / bitmapHeight;
                double height = h;
                double width = bitmapWidth * scale;
                return new Vector2<double>(width, height);
            }
            else // more height
            {
                double scale = w / bitmapWidth;
                double width = w;
                double height = bitmapHeight * scale;
                return new Vector2<double>(width, height);
            }
        }

        private static BitmapSource CreateImageSource(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();
            BitmapSource prgbaSource = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
            WriteableBitmap bmp = new WriteableBitmap(prgbaSource);
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] pixelData = new int[w * h];
            //int widthInBytes = 4 * w;
            int widthInBytes = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8); //equals 4*w
            bmp.CopyPixels(pixelData, widthInBytes, 0);
            bmp.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInBytes, 0);
            bitmap = null;
            return bmp;
        }
    }
}