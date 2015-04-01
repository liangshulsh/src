// Ai Software Control Library.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
	public class ToolStripDropDown : System.Windows.Forms.ToolStripDropDown {
		public enum SizingGripMode { BottomRight, Bottom, None }
		SizingGripMode _showSizingGrip = SizingGripMode.None;
		Point _startPoint;
		Rectangle _gripRect;
		bool _resizing = false;
		bool _opened = false;
		System.Windows.Forms.Control _owner = null;
		public ToolStripDropDown() {
            this.Closed += ToolStripDropDown_Closed;
            this.MouseDown += ToolStripDropDown_MouseDown;
            this.MouseMove += ToolStripDropDown_MouseMove;
            this.Opened += ToolStripDropDown_Opened;
            this.Resize += ToolStripDropDown_Resize;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.DropShadowEnabled = false;
			this.Padding = new Padding(2);
		}
		public ToolStripDropDown(System.Windows.Forms.Control owner) {
            this.Closed += ToolStripDropDown_Closed;
            this.MouseDown += ToolStripDropDown_MouseDown;
            this.MouseMove += ToolStripDropDown_MouseMove;
            this.Opened += ToolStripDropDown_Opened;
            this.Resize += ToolStripDropDown_Resize;
			_owner = owner;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.DropShadowEnabled = false;
			this.Padding = new Padding(2);
		}
        public Form getForm() {
			if (_owner != null) {
				if (_owner.GetType() == typeof(ToolStripDropDown)) {
                    ToolStripDropDown own = (ToolStripDropDown)_owner;
					return own.getForm();
				} else {
					return _owner.FindForm();
				}
			} else {
				return null;
			}
		}
		public SizingGripMode SizingGrip {
			get { return _showSizingGrip; }
			set {
				if (_showSizingGrip != value) {
					_showSizingGrip = value;
					if (_showSizingGrip != SizingGripMode.None) this.Padding = new Padding(2, 2, 2, 10);
					else this.Padding = new Padding(2);
					this.Invalidate();
				}
			}
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			Rectangle aRect;
			System.Drawing.Drawing2D.GraphicsPath oPath;
			LinearGradientBrush _splitBrush;
			Rectangle _splitRect = new Rectangle(0, this.Height - 10, this.Width, 10);
			aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
			oPath = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
			oPath.CloseFigure();
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			e.Graphics.FillPath(new SolidBrush(Color.FromArgb(250, 250, 250)), oPath);
			if (_showSizingGrip != SizingGripMode.None) {
				_splitBrush = new System.Drawing.Drawing2D.LinearGradientBrush(_splitRect,
                    Color.Black, Color.White, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
				_splitBrush.InterpolationColors = Ai.Renderer.Drawing.SizingGripBlend;
				e.Graphics.FillRectangle(_splitBrush, _splitRect);
				if (_showSizingGrip == SizingGripMode.BottomRight) Ai.Renderer.Drawing.drawGrip(e.Graphics, this.Width - 12, this.Height - 12);
                else Ai.Renderer.Drawing.drawVGrip(e.Graphics, _splitRect);
				e.Graphics.DrawLine(Ai.Renderer.Drawing.GripBorderPen, 0, _splitRect.Y, this.Width, _splitRect.Y);
				_splitBrush.Dispose();
			}
			e.Graphics.DrawPath(new Pen(Color.FromArgb(134, 134, 134)), oPath);
			oPath.Dispose();
			aRect = new Rectangle(0, 0, this.Width, this.Height);
			oPath = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
			this.Region = new Region(oPath);
			oPath.Dispose();
		}
		private void ToolStripDropDown_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e) { _opened = false; }
		private void ToolStripDropDown_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (_showSizingGrip != SizingGripMode.None && e.Button == System.Windows.Forms.MouseButtons.Left) {
				if (this.Cursor == Cursors.SizeNWSE) {
					_resizing = true;
					_startPoint = this.PointToScreen(e.Location);
					Win32API.ReleaseCapture();
					Win32API.SendMessage(this.Handle, (int)Win32API.WM_NCLBUTTONDOWN, (int)Win32API.HTBOTTOMRIGHT, 0);
				} else if (this.Cursor == Cursors.SizeNS) {
					_resizing = true;
					_startPoint = this.PointToScreen(e.Location);
					Win32API.ReleaseCapture();
					Win32API.SendMessage(this.Handle, (int)Win32API.WM_NCLBUTTONDOWN, (int)Win32API.HTBOTTOM, 0);
				}
			}
		}
		private void ToolStripDropDown_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (_showSizingGrip != SizingGripMode.None) {
				if (_showSizingGrip == SizingGripMode.BottomRight) {
					if (_gripRect.Contains(e.X, e.Y)) this.Cursor = Cursors.SizeNWSE;
					else this.Cursor = Cursors.Default;
				} else {
					Rectangle _splitRect = new Rectangle(0, this.Height - 10, this.Width, 10);
					if (_splitRect.Contains(e.X, e.Y)) this.Cursor = Cursors.SizeNS;
					else this.Cursor = Cursors.Default;
				}
			}
		}
		private void ToolStripDropDown_Opened(object sender, System.EventArgs e) {
			Point pt = new Point(0, 0);
			ToolStripControlHost aHost;
			pt = this.PointToScreen(pt);
			if (this.Items[0].GetType() == typeof(ToolStripControlHost)) {
                aHost = (ToolStripControlHost)this.Items[0];
				aHost.Control.Focus();
			}
			_opened = true;
		}
		private void ToolStripDropDown_Resize(object sender, System.EventArgs e) {
			_gripRect = new Rectangle(this.Width - 10, this.Height - 10, 10, 10);
			if (_opened) {
				Point pt = new Point(0, 0);
				pt = this.PointToScreen(pt);
			}	
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if (_resizing) {
				if (m.Msg == (int)Win32API.WM_NCCALCSIZE) {
					int dx = System.Windows.Forms.Control.MousePosition.X - _startPoint.X;
					int dy = System.Windows.Forms.Control.MousePosition.Y - _startPoint.Y;
					_resizing = false;
					if (this.Items[0] != null) {
                        ToolStripControlHost aHost = (ToolStripControlHost)this.Items[0];
						if (_showSizingGrip == SizingGripMode.BottomRight) aHost.Control.Width = aHost.Control.Width + dx;
						aHost.Control.Height = aHost.Control.Height + dy;
					}
				} else {
					base.WndProc(ref m);
				}
			} else {
				base.WndProc(ref m);
			}
		}
	}
}