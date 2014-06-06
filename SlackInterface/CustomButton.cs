using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SlackInterface
{
    public class CustomButton : Button
    {
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }

        public int BorderRadius;
        public int BorderWidth;
        public Color BorderColor;

        public CustomButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Font = new Font(CustomFonts.Fonts.Families[1], 16);
            ForeColor = Color.FromArgb(255, 255, 255, 255);
            BackColor = Color.Transparent;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = FlatAppearance.MouseDownBackColor = FlatAppearance.CheckedBackColor = Color.Transparent;

            BorderRadius = 4;
            BorderWidth = 1;
            BorderColor = Color.FromArgb(0x00, 0x99, 0xcc);
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
                GraphicsPath gp = Extensions.Create(0, 0, Width - 2, Height - 2, BorderRadius);
                Pen p = new Pen(BorderColor, BorderWidth);

                e.Graphics.DrawPath(p, gp);
                e.Graphics.FillPath(p.Brush, gp);

                StringFormat formatting = (StringFormat)StringFormat.GenericTypographic.Clone();
                formatting.Alignment = StringAlignment.Center;
                formatting.LineAlignment = StringAlignment.Center;

                float emsize = e.Graphics.DpiY * Font.Size / 72;
                gp = new GraphicsPath();
                gp.StartFigure();
                gp.AddString(Text, Font.FontFamily, (int)Font.Style, emsize, new Point(Width / 2, Height / 2), formatting);
                gp.CloseFigure();
                e.Graphics.FillPath(new Pen(ForeColor, 1).Brush, gp);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
    }
}