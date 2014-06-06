using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace Slack
{
    public static class Extensions
    {
        public static Image RoundCorners(this Image StartImage, int CornerRadius, Color BackgroundColor)
        {
            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            Graphics g = Graphics.FromImage(RoundedImage);
            g.Clear(BackgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush brush = new TextureBrush(StartImage);
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
            gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
            gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
            g.FillPath(brush, gp);
            return RoundedImage;
        }

        public static Stream DownloadFile(this Uri path)
        {
            HttpClient client = new HttpClient();
            Stream res = new MemoryStream(client.GetByteArrayAsync(path).Result);
            client.Dispose();
            return res;
        }

        public static void DownloadFileAsync(this Uri path, Action<Stream> callback)
        {
            ThreadPool.QueueUserWorkItem((o) => callback(DownloadFile(path)));
        }

        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        /// <summary>
        /// I don't even know what I was doing.
        /// </summary>
        public static GraphicsPath CreateFill(int realX, int realY, int realWidth, int realHeight, int borderRadius, RectangleCorners corners)
        {
            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            for (int radius = 0; radius < borderRadius; ++radius)
            {
                int x = realX + radius;
                int y = realY + radius;
                int width = realWidth - radius * 2;
                int height = realHeight - radius * 2;

                int xw = x + width;
                int yh = y + height;
                int xwr = xw - borderRadius;
                int yhr = yh - borderRadius;
                int xr = x + borderRadius;
                int yr = y + borderRadius;
                int r2 = borderRadius * 2;
                int xwr2 = xw - r2;
                int yhr2 = yh - r2;

                //Top Left Corner
                if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
                    p.AddArc(x, y, r2, r2, 180, 90);
                else
                {
                    p.AddLine(x, yr, x, y);
                    p.AddLine(x, y, xr, y);
                }

                //Top Edge
                p.AddLine(xr, y, xwr, y);

                //Top Right Corner
                if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
                    p.AddArc(xwr2, y, r2, r2, 270, 90);
                else
                {
                    p.AddLine(xwr, y, xw, y);
                    p.AddLine(xw, y, xw, yr);
                }

                //Right Edge
                p.AddLine(xw, yr, xw, yhr);

                //Bottom Right Corner
                if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
                    p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
                else
                {
                    p.AddLine(xw, yhr, xw, yh);
                    p.AddLine(xw, yh, xwr, yh);
                }

                //Bottom Edge
                p.AddLine(xwr, yh, xr, yh);

                //Bottom Left Corner
                if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
                    p.AddArc(x, yhr2, r2, r2, 90, 90);
                else
                {
                    p.AddLine(xr, yh, x, yh);
                    p.AddLine(x, yh, x, yhr);
                }

                //Left Edge
                p.AddLine(x, yhr, x, yr);
            }

            p.FillMode = FillMode.Alternate;

            p.CloseFigure();

            return p;
        }

        public static GraphicsPath Create(int x, int y, int width, int height,
                                          int radius, RectangleCorners corners)
        {
            int xw = x + width;
            int yh = y + height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = x + radius;
            int yr = y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            //Top Left Corner
            if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
                p.AddArc(x, y, r2, r2, 180, 90);
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            //Top Edge
            p.AddLine(xr, y, xwr, y);

            //Top Right Corner
            if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            //Right Edge
            p.AddLine(xw, yr, xw, yhr);

            //Bottom Right Corner
            if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            //Bottom Edge
            p.AddLine(xwr, yh, xr, yh);

            //Bottom Left Corner
            if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            //Left Edge
            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        public static GraphicsPath RoundBorders(this Rectangle rect, int radius, RectangleCorners c)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }

        public static GraphicsPath Create(int x, int y, int width, int height, int radius)
        { return Create(x, y, width, height, radius, RectangleCorners.All); }

        public static GraphicsPath RoundBorders(this Rectangle rect, int radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

        public static GraphicsPath Create(int x, int y, int width, int height)
        { return Create(x, y, width, height, 5); }

        public static GraphicsPath Create(Rectangle rect)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height); }

        /// <summary>
        /// Needs a handle to work.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="placeHolder"></param>
        public static void SetPlaceholder(this TextBox that, string placeHolder)
        {
            Externals.User32.SendMessage(that.Handle, Externals.User32.EM_SETCUEBANNER, 0, placeHolder);
        }
    }
}
