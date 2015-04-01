using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ai.Renderer;

namespace Ai.Renderer {
	/// <summary>
	/// Class for rendering list item.
	/// </summary>
	public class ListItem {
		#region Color Blend
		/// <summary>
		/// Represent a color blend for selected item in a list that lost it focus input.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>ColorBlend.</returns>
		public static ColorBlend SelectedBlurBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[2];
			float[] pos = new float[2];
			ColorBlend blend = new ColorBlend();
			pos[0] = 0.0F;
            pos[1] = 1.0F;
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
					colors[0] = Color.FromArgb(248, 248, 248);
					colors[1] = Color.FromArgb(229, 229, 229);
                    break;
			}
			blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend SelectedBlurBlend() { return SelectedBlurBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for selected item in focused list.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>ColorBlend.</returns>
		public static ColorBlend SelectedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[2];
			float[] pos = new float[2];
			ColorBlend blend = new ColorBlend();
			pos[0] = 0.0F;
            pos[1] = 1.0F;
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
					colors[0] = Color.FromArgb(220, 235, 252);
					colors[1] = Color.FromArgb(193, 219, 252);
                    break;
			}
			blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend SelectedBlend() { return SelectedBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for selected and highlighted item.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>ColorBlend.</returns>
		public static ColorBlend SelectedHLiteBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[2];
			float[] pos = new float[2];
			ColorBlend blend = new ColorBlend();
			pos[0] = 0.0F;
            pos[1] = 1.0F;
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(220, 235, 252);
                    colors[1] = Color.FromArgb(193, 219, 252);
                    break;
			}
			blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend SelectedHLiteBlend() { return SelectedHLiteBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for highlighted item.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>ColorBlend.</returns>
		public static ColorBlend HLitedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[2];
			float[] pos = new float[2];
			ColorBlend blend = new ColorBlend();
			pos[0] = 0.0F;
            pos[1] = 1.0F;
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
					colors[0] = Color.FromArgb(252, 253, 254);
					colors[1] = Color.FromArgb(235, 243, 253);
                    break;
			}
			blend.Colors = colors;
            blend.Positions = pos;
            return blend;
		}
		public static ColorBlend HLitedBlend() { return HLitedBlend(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a color blend for pressed item.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>ColorBlend.</returns>
		public static ColorBlend PressedBlend(Drawing.ColorTheme theme) {
			Color[] colors = new Color[2];
			float[] pos = new float[2];
			ColorBlend blend = new ColorBlend();
			pos[0] = 0.0F;
            pos[1] = 1.0F;
			switch (theme) {
				case Drawing.ColorTheme.Blue:
				case Drawing.ColorTheme.BlackBlue:
					colors[0] = Color.FromArgb(160, 189, 227);
					colors[1] = Color.FromArgb(255, 255, 255);
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
		/// Represent a border pen for selected item in a list that lost it focus input.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen.</returns>
		public static Pen SelectedBlurBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(217, 217, 217));
			}
			return new Pen(Color.Black);
		}
		public static Pen SelectedBlurBorderPen() { return SelectedBlurBorderPen(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a border pen for selected item in a focused list.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen.</returns>
		public static Pen SelectedBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(177, 217, 229));
			}
			return new Pen(Color.Black);
		}
		public static Pen SelectedBorderPen() { return SelectedBorderPen(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a border pen for highlighted item in a list.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen.</returns>
		public static Pen HLitedBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(185, 215, 252));
			}
			return new Pen(Color.Black);
		}
		public static Pen HLitedBorderPen() { return HLitedBorderPen(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a border pen for selected and highlighted item in a list.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen.</returns>
		public static Pen SelectedHLiteBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(132, 172, 221));
			}
			return new Pen(Color.Black);
		}
		public static Pen SelectedHLiteBorderPen() { return SelectedHLiteBorderPen(Drawing.ColorTheme.Blue); }
		/// <summary>
		/// Represent a border pen for pressed item in a list.
		/// </summary>
		/// <param name="theme">Theme used to paint.</param>
		/// <returns>Pen.</returns>
		public static Pen PressedBorderPen(Drawing.ColorTheme theme) {
			switch (theme) {
				case Drawing.ColorTheme.Blue:
					return new Pen(Color.FromArgb(104, 140, 175));
			}
			return new Pen(Color.Black);
		}
		public static Pen PressedBorderPen() { return PressedBorderPen(Drawing.ColorTheme.Blue); }
		#endregion
		#region Draw
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		/// <param name="enabled">Determine whether the list is enabled.</param>
		/// <param name="focused">Determine whether the list has input focus.</param>
		/// <param name="pressed">Determine whether the item is on the pressed state.</param>
		/// <param name="selected">Determine whether the item is selected.</param>
		/// <param name="hLited">Determine whether the item is highlighted.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded, Boolean enabled, Boolean focused, Boolean pressed,
			Boolean selected, Boolean hLited) {
			if (g == null) return;
			if (rect.Width == 0 || rect.Height == 0) return;
			LinearGradientBrush itemBrush = new LinearGradientBrush(rect, Color.Black,
				Color.White, LinearGradientMode.Vertical);
			GraphicsPath itemPath = Drawing.roundedRectangle(rect, rounded, rounded, rounded, rounded);
			Pen itemBorder = null;
			if (enabled) {
				if (pressed) {
					itemBrush.InterpolationColors = PressedBlend(theme);
					itemBorder = PressedBorderPen(theme);
				} else {
					if (selected) {
						if (hLited) {
							itemBrush.InterpolationColors = SelectedHLiteBlend(theme);
							itemBorder = SelectedHLiteBorderPen(theme);
						} else {
							if (focused) {
								itemBrush.InterpolationColors = SelectedBlend(theme);
								itemBorder = SelectedBorderPen(theme);
							} else {
								itemBrush.InterpolationColors = SelectedBlurBlend(theme);
								itemBorder = SelectedBlurBorderPen(theme);
							}
						}
					} else {
						if (hLited) {
							itemBrush.InterpolationColors = HLitedBlend(theme);
							itemBorder = HLitedBorderPen(theme);
						}
					}
				}
			} else {
				if (selected) {
					Color[] colors = new Color[2];
					colors[0] = Color.LightGray;
					colors[1] = Color.LightGray;
					itemBorder = new Pen(Color.Gray);
					itemBrush.LinearColors = colors;
				}
			}
			if (itemBorder != null) {
				g.FillPath(itemBrush, itemPath);
				g.DrawPath(itemBorder, itemPath);
				itemBorder.Dispose();
			}
			itemBrush.Dispose();
			itemPath.Dispose();
		}
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		/// <param name="enabled">Determine whether the list is enabled.</param>
		/// <param name="focused">Determine whether the list has input focus.</param>
		/// <param name="pressed">Determine whether the item is on the pressed state.</param>
		/// <param name="selected">Determine whether the item is selected.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded, Boolean enabled, Boolean focused, Boolean pressed,
			Boolean selected) { draw(g, rect, theme, rounded, enabled, focused, pressed, selected, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		/// <param name="enabled">Determine whether the list is enabled.</param>
		/// <param name="focused">Determine whether the list has input focus.</param>
		/// <param name="pressed">Determine whether the item is on the pressed state.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded, Boolean enabled, Boolean focused, Boolean pressed) { draw(g, rect, theme, rounded, enabled, focused, pressed, false, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		/// <param name="enabled">Determine whether the list is enabled.</param>
		/// <param name="focused">Determine whether the list has input focus.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded, Boolean enabled, Boolean focused) { draw(g, rect, theme, rounded, enabled, focused, false, false, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		/// <param name="enabled">Determine whether the list is enabled.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded, Boolean enabled) { draw(g, rect, theme, rounded, enabled, false, false, false, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		/// <param name="rounded">Rounded range of each rectangle's corner.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme,
			uint rounded) { draw(g, rect, theme, rounded, true, false, false, false, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		/// <param name="theme">Theme used to paint.</param>
		public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme) { draw(g, rect, theme, 2, true, false, false, false, false); }
		/// <summary>
		/// Draw an item background in the list.
		/// </summary>
		/// <param name="g">Graphics object where the item background to be drawn.</param>
		/// <param name="rect">Bounding rectangle of the item.</param>
		public static void draw(Graphics g, Rectangle rect) { draw(g, rect, Drawing.ColorTheme.Blue, 2, true, false, false, false, false); }
		#endregion
	}
}