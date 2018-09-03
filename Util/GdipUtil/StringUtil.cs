using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuLivePlayer.Util.GdipUtil
{
    internal static class StringUtil
    {
        public static Bitmap[] GetCharsBitmap(string content, Font font, Brush brush) =>
            content.Select(s => GetStringBitmap(s.ToString(), font, brush)).ToArray();

        public static Bitmap GetStringBitmap(string content, Font font, Brush brush)
        {
            SizeF strSize;
            Bitmap bmp = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (content != "" && content.Trim() == "")
                {
                    strSize = g.MeasureString(content.Replace(" ", "."), font);
                }
                else
                    strSize = g.MeasureString(content, font);
            }

            bmp = new Bitmap((int)Math.Ceiling(strSize.Width) + 10, (int)Math.Ceiling(strSize.Height) + 10);

            using (StringFormat format = StringFormat.GenericTypographic)
            using (Pen pen = new Pen(Color.FromArgb(96, 0, 0, 0), 2))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                RectangleF rect = new RectangleF(5, 5, strSize.Width, strSize.Height);

                float dpi = g.DpiY;
                using (GraphicsPath gp = GetStringPath(content, dpi, rect, font, format))
                {
                    g.DrawPath(pen, gp); //描边
                    g.FillPath(brush, gp); //填充
                }
                //g.DrawString(content, font, brush, 1, 1);
            }

            return bmp;
        }

        private static GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            // Convert font size into appropriate coordinates
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);

            return path;
        }
    }
}
