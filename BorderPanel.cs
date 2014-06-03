using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SlackInterface
{
    public class BorderPanel : Panel
    {
        public Color BorderColor = Color.FromArgb(0xe0, 0xe0, 0xe0);
        public int BorderWidth = 1;
        public int BorderRadius;

        public BorderPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (BorderRadius == 0)
            {
                Rectangle rc = ClientRectangle;
                rc.Inflate(-(int)Math.Round(BorderWidth / 2.0 + .5), -(int)Math.Round(BorderWidth / 2.0 + .5));

                rc.Y = rc.Y - 1;
                rc.Height = rc.Height + 1;

                e.Graphics.DrawRectangle(new Pen(BorderColor, BorderWidth), rc);
            }
            else
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath gp = Extensions.Create(0, 0, Width-2, Height-2, BorderRadius);

                e.Graphics.DrawPath(new Pen(BorderColor, BorderWidth), gp);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Invalidate();
        }
    }
}
