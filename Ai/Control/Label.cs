using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
    public class Label : System.Windows.Forms.Label {
        public Label() : base() {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.Paint += Label_Paint;
            this.TextAlign = ContentAlignment.MiddleLeft;
        }
        [DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
        public override ContentAlignment TextAlign {
            get { return base.TextAlign; }
            set { base.TextAlign = value; }
        }
        private void Label_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { 
            StringFormat strFormat = new StringFormat();
            Rectangle myRect = new Rectangle(0, 0, this.Width + 1, this.Height + 1);
            switch (this.TextAlign) {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    strFormat.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                    strFormat.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    strFormat.LineAlignment = StringAlignment.Near;
                    break;
            }
            switch (base.TextAlign) {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    strFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    strFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    strFormat.Alignment = StringAlignment.Far;
                    break;
            }
            if (this.AutoEllipsis) strFormat.Trimming = StringTrimming.EllipsisCharacter;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (this.BackColor == Color.Transparent) this.InvokePaintBackground(this, e);
            else e.Graphics.Clear(this.BackColor);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), myRect, strFormat);
            strFormat.Dispose();
        }
    }
}