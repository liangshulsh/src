using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ai.Renderer;

namespace Ai.Renderer {
	/// <summary>
	/// Class for rendering column header.
	/// </summary>
	public class Column {
		#region Color Blend
		/// <summary>
		/// Represent a color blend for a normal column.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <return>ColorBlend</return>
		public static ColorBlend NormalBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0.0F;
            pos[1] = 0.4F;
            pos[2] = 0.4F;
            pos[3] = 1.0F;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(255, 255, 255);
                    colors[1] = Color.FromArgb(255, 255, 255);
                    colors[2] = Color.FromArgb(247, 248, 250);
                    colors[3] = Color.FromArgb(241, 242, 244);
					break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend NormalBlend() { return NormalBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for a selected column.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <return>ColorBlend</return>
		public static ColorBlend SelectedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0.0F;
            pos[1] = 0.4F;
            pos[2] = 0.4F;
            pos[3] = 1.0F;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(242, 249, 252);
                    colors[1] = Color.FromArgb(242, 249, 252);
                    colors[2] = Color.FromArgb(225, 241, 249);
                    colors[3] = Color.FromArgb(216, 236, 246);
					break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend SelectedBlend() { return SelectedBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for a highlited column.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <return>ColorBlend</return>
		public static ColorBlend HLitedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0.0F;
            pos[1] = 0.4F;
            pos[2] = 0.4F;
            pos[3] = 1.0F;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(227, 247, 255);
                    colors[1] = Color.FromArgb(227, 247, 255);
                    colors[2] = Color.FromArgb(189, 237, 255);
                    colors[3] = Color.FromArgb(183, 231, 251);
					break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend HLitedBlend() { return HLitedBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for a highlited column's dropdown.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <return>ColorBlend</return>
		public static ColorBlend HLitedDropDownBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0.0F;
            pos[1] = 0.4F;
            pos[2] = 0.4F;
            pos[3] = 1.0F;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(205, 242, 255);
                    colors[1] = Color.FromArgb(205, 242, 255);
                    colors[2] = Color.FromArgb(140, 224, 255);
                    colors[3] = Color.FromArgb(136, 217, 251);
					break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend HLitedDropDownBlend() { return HLitedDropDownBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for a pressed column.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <return>ColorBlend</return>
		public static ColorBlend PressedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0.0F;
            pos[1] = 0.4F;
            pos[2] = 0.4F;
            pos[3] = 1.0F;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(188, 228, 249);
                    colors[1] = Color.FromArgb(188, 228, 249);
                    colors[2] = Color.FromArgb(141, 214, 247);
                    colors[3] = Color.FromArgb(138, 209, 245);
					break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend PressedBlend() { return PressedBlend(Drawing.ColorTheme.Blue); }
		#endregion
		#region Border Pen
		/// <summary>
		/// Represent a pen for a normal column border.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen</returns>
		public static Pen NormalBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(213, 213, 213));
			}
			return new Pen(Color.Black);
		}
		public static Pen NormalBorderPen() { return NormalBorderPen(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a pen for an active column border.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen</returns>
		public static Pen ActiveBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(147, 201, 227));
			}
			return new Pen(Color.Black);
		}
		public static Pen ActiveBorderPen() { return ActiveBorderPen(Drawing.ColorTheme.Blue); }
		#endregion
		#region Text Brushes
		public static SolidBrush TextBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(62, 106, 170));
			}
			return null;
		}
		public static SolidBrush TextBrush() { return TextBrush(Drawing.ColorTheme.Blue); }
		public static SolidBrush DisabledTextBrush(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new SolidBrush(Color.FromArgb(118, 118, 118));
			}
			return null;
		}
		public static SolidBrush DisabledTextBrush() { return DisabledTextBrush(Drawing.ColorTheme.Blue); }
		#endregion
		#region Drawing
		public static void drawPressedShadow(Graphics g, Rectangle rect, uint stepCount) {
			if (stepCount == 0) return;
			if (rect.Width == 0 || rect.Height == 0) return;
			int alpha = 4 + (int)(stepCount * 8);
			int i = 0;
			Point[] shadowPoints = new Point[4];
			shadowPoints[0].X = rect.X;
            shadowPoints[0].Y = rect.Bottom - 1;
            shadowPoints[1].X = rect.X;
            shadowPoints[1].Y = rect.Y;
            shadowPoints[2].X = rect.Right - 1;
            shadowPoints[2].Y = rect.Y;
            shadowPoints[3].X = rect.Right - 1;
            shadowPoints[3].Y = rect.Bottom - 1;
			while (i < stepCount) {
				g.DrawLine(new Pen(Color.FromArgb(alpha, 0, 0, 0)), shadowPoints[0], shadowPoints[1]);
				g.DrawLine(new Pen(Color.FromArgb(alpha, 0, 0, 0)), shadowPoints[1], shadowPoints[2]);
				g.DrawLine(new Pen(Color.FromArgb(alpha, 0, 0, 0)), shadowPoints[2], shadowPoints[3]);
				alpha -= 8;
				i += 1;
				shadowPoints[0].X += 1;
				shadowPoints[1].X += 1;
				shadowPoints[1].Y += 1;
				shadowPoints[2].X -= 1;
				shadowPoints[2].Y += 1;
				shadowPoints[3].X -= 1;
			}
		}
        public static void drawPressedShadow(Graphics g, Rectangle rect) { drawPressedShadow(g, rect, 3); }
		#endregion
	}
}