using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ai.Renderer;

namespace Ai.Renderer {
	public class Popup {
		#region Brushes
		public static SolidBrush PlacementBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(233, 238, 238));
				case Drawing.ColorTheme.BlackBlue:
					return new SolidBrush(Color.FromArgb(125, 158, 201));
			}
			return new SolidBrush(Color.Black);
		}
		public static SolidBrush PlacementBrush() { return PlacementBrush(Drawing.ColorTheme.Blue); }
		public static SolidBrush SeparatorBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(221, 231, 238));
				case Drawing.ColorTheme.BlackBlue:
					return new SolidBrush(Color.FromArgb(163, 188, 218));
			}
			return new SolidBrush(Color.Black);
		}
		public static SolidBrush SeparatorBrush() { return SeparatorBrush(Drawing.ColorTheme.Blue); }
		public static SolidBrush BackgroundBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(250, 250, 250));
				case Drawing.ColorTheme.BlackBlue:
					return new SolidBrush(Color.FromArgb(84, 84, 84));
			}
			return new SolidBrush(Color.Black);
		}
		public static SolidBrush BackgroundBrush() { return BackgroundBrush(Drawing.ColorTheme.Blue); }
		public static SolidBrush NormalTextBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(85, 119, 163));
				case Drawing.ColorTheme.BlackBlue:
					return new SolidBrush(Color.FromArgb(255, 255, 255));
			}
			return new SolidBrush(Color.Black);
		}
		public static SolidBrush NormalTextBrush() { return NormalTextBrush(Drawing.ColorTheme.Blue); }
		public static SolidBrush DisabledTextBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
					return new SolidBrush(Color.FromArgb(118, 118, 118));
			}
			return new SolidBrush(Color.Black);
		}
		public static SolidBrush DisabledTextBrush() { return DisabledTextBrush(Drawing.ColorTheme.Blue); }
		#endregion
		#region Pen
		public static Pen SeparatorPen(Drawing.ColorTheme theme) {
			return new Pen(Color.FromArgb(197, 197, 197));
		}
		public static Pen SeparatorPen() { return SeparatorPen(Drawing.ColorTheme.Blue); }
		#endregion
	}
}