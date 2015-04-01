// Ai Software Control Library.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using Ai.Renderer;

namespace Ai.Control {
	public class Button : System.Windows.Forms.Button {
		Boolean _onMouse = false;
		Boolean _pressed = false;
		Ai.Renderer.Drawing.ColorTheme _theme = Ai.Renderer.Drawing.ColorTheme.Blue;
		public Button() : base() {
            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.ResizeRedraw, true);
		}
		protected override void OnEnabledChanged(EventArgs e) { 
            this.Invalidate();
            base.OnEnabledChanged(e);
        }
        protected override void OnGotFocus(EventArgs e) { 
            this.Invalidate();
            base.OnGotFocus(e);
        }
		protected override void OnLostFocus(EventArgs e) { 
            this.Invalidate();
            base.OnLostFocus(e);
        }
        protected override void OnTextChanged(EventArgs e) { 
            this.Invalidate();
            base.OnTextChanged(e);
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				_pressed = true;
				this.Invalidate();
			}
            base.OnMouseDown(e);
		}
        protected override void OnMouseEnter(EventArgs e) {
			_onMouse = true;
			this.Invalidate();
            base.OnMouseEnter(e);
		}
        protected override void OnMouseLeave(EventArgs e) {
			_onMouse = false;
			_pressed = false;
			this.Invalidate();
            base.OnMouseLeave(e);
		}
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if (_pressed) {
				_pressed = false;
				this.Invalidate();
			}
            base.OnMouseUp(e);
		}
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			Rectangle rect;
			StringFormat txtFormat = new StringFormat();
			txtFormat.LineAlignment = StringAlignment.Center;
			txtFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
			switch (this.TextAlign) {
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomRight:
					txtFormat.LineAlignment = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
					txtFormat.LineAlignment = StringAlignment.Center;
					break;
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopRight:
					txtFormat.LineAlignment = StringAlignment.Near;
					break;
			}
			txtFormat.Alignment = StringAlignment.Center;
			switch (this.TextAlign) {
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					txtFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					txtFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					txtFormat.Alignment = StringAlignment.Far;
					break;
			}
			rect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			e.Graphics.Clear(this.BackColor);
			Ai.Renderer.Button.draw(e.Graphics, rect, _theme, 2, this.Enabled, _pressed, false, _onMouse, this.Focused);
			if (this.Enabled) e.Graphics.DrawString(this.Text, this.Font, Ai.Renderer.Drawing.NormalTextBrush(_theme), rect, txtFormat);
			else e.Graphics.DrawString(this.Text, this.Font, Ai.Renderer.Drawing.DisabledTextBrush(_theme), rect, txtFormat);
			if (base.Image != null) {
				Rectangle imgRect = Ai.Renderer.Drawing.getImageRectangle(
					base.Image, rect, 
					(int)(rect.Width > rect.Height ? (rect.Height - 4) : (rect.Width - 4)));
				if (this.Enabled) e.Graphics.DrawImage(base.Image, imgRect);
				else Ai.Renderer.Drawing.grayScaledImage(base.Image, imgRect, e.Graphics);
			}
			txtFormat.Dispose();
		}
        [DefaultValue(typeof(Ai.Renderer.Drawing.ColorTheme),"Blue")]
		public Ai.Renderer.Drawing.ColorTheme Theme {
			get { return _theme; }
			set {
				if (!typeof(Ai.Renderer.Drawing.ColorTheme).IsAssignableFrom(value.GetType())) {
					throw new ArgumentException("Value must be one of Ai.Renderer.Drawing.ColorTheme enumeration.", "value");
				} else {
					if (_theme != value) {
						_theme = value;
						this.Invalidate();
					}
				}
			}
		}
		public new Image Image {
			get { return base.Image; }
			set {
				base.Image = value;
				this.Invalidate();
			}
		}
	}
}