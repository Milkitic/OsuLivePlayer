using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using DXIO = SharpDX.IO;

namespace OsuLivePlayer.Util.DxUtil
{
    public static class WicUtil
    {
        public static D2D.Bitmap LoadBitmap(this D2D.RenderTarget renderTarget, string imagePath)
        {
            WIC.ImagingFactory imagingFactory = new WIC.ImagingFactory();
            DXIO.NativeFileStream fileStream = new DXIO.NativeFileStream(imagePath,
                DXIO.NativeFileMode.Open, DXIO.NativeFileAccess.Read);

            WIC.BitmapDecoder bitmapDecoder = new WIC.BitmapDecoder(imagingFactory, fileStream, WIC.DecodeOptions.CacheOnDemand);
            WIC.BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

            WIC.FormatConverter converter = new WIC.FormatConverter(imagingFactory);
            converter.Initialize(frame, WIC.PixelFormat.Format32bppPRGBA);

            return D2D.Bitmap.FromWicBitmap(renderTarget, converter);
        }
    }
}
