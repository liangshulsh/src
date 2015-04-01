using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Ai.Renderer;

namespace Ai.Renderer {
	public class ToolTip {
		public static Font TitleFont = new Font("Segoe UI", 8, FontStyle.Bold);
		public static Font TextFont = new Font("Segoe UI", 8, FontStyle.Regular);
		#region Enumerations
		/// <summary>
		/// Describing the content of a tooltip information.
		/// Tooltip information has 3 component, title, text, and image.
		/// </summary>
		public enum Content {
			TitleOnly,
			TitleAndText,
			TitleAndImage,
			All,
			ImageOnly,
			ImageAndText,
			TextOnly,
			Empty
		}
		#endregion
		#region Drawing Component
		/// <summary>
		/// A brush for drawing a string in tooltip.
		/// </summary>
		/// <returns>Brush.</returns>
		public static Brush TextBrush {
			get { return new SolidBrush(Color.FromArgb(118, 118, 118)); }
		}
		/// <summary>
		/// A pen for drawing line separator in tooltip.
		/// </summary>
		/// <returns>Pen.</returns>
		public static Pen SeparatorPen {
			get { return new Pen(Color.FromArgb(158, 187, 221)); }
		}
		/// <summary>
		/// Get the content of the tooltip information.
		/// </summary>
		/// <param name="title">Tooltip title.</param>
		/// <param name="text">Tooltip text.</param>
		/// <param name="image">Tooltip image.</param>
		/// <returns><seealso cref="Content"/></returns>
		public static Content getContent(String title, String text, Image image) {
			if ((title != "") && (text != "") && (image != null)) { return Content.All;
			} else {
				if (title != "") {
					if (image != null) {
						return Content.TitleAndImage;
					} else {
						if (text != "") return Content.TitleAndText;
						else return Content.TitleOnly;
					}
				} else {
					if (image != null) {
						if (text != "") return Content.ImageAndText;
						else return Content.ImageOnly;
					} else {
						if (text != "") return Content.TextOnly;
					}
				}
			}
			return Content.Empty;
		}
		/// <summary>
		/// Determine if a tooltip information isnot empty.
		/// </summary>
		/// <param name="title">Tooltip title.</param>
		/// <param name="text">Tooltip text.</param>
		/// <param name="image">Tooltip image.</param>
		/// <returns>Boolean.</returns>
		public static Boolean containsToolTip(String title, String text, Image image) {
			return ((title != "") || (text != "") || (image != null));
		}
		/// <summary>
		/// Measure the size of a tooltip based on its contents.
		/// </summary>
		/// <param name="title">Tooltip title.</param>
		/// <param name="text">Tooltip text.</param>
		/// <param name="image">Tooltip image.</param>
		/// <returns>Size.</returns>
		public static Size measureSize(String title, String text, Image image) {
			Size result = new Size(0, 0);
			int lText = 0;
			Size tSize = new Size(0, 0);
			int y;
			switch (getContent(title, text, image)) {
				case Content.All:
					tSize = TextRenderer.MeasureText(title, TitleFont);
					result.Width = tSize.Width + 8;
					result.Height = tSize.Height + 16 + image.Height;
					y = tSize.Height + 12;
					lText = image.Width + 8;
					tSize = TextRenderer.MeasureText(text, TextFont);
					if (result.Height < y + tSize.Height + 4) result.Height = y + tSize.Height + 4;
					if (result.Width < lText + tSize.Width + 4) result.Width = lText + tSize.Width + 4;
					break;
				case Content.TitleAndImage:
					result.Height = image.Height + 8;
					tSize = TextRenderer.MeasureText(title, TitleFont);
					if (result.Height < tSize.Height + 8) result.Height = tSize.Height + 8;
					result.Width = 12 + image.Width + tSize.Width;
					break;
				case Content.TitleAndText:
					tSize = TextRenderer.MeasureText(title, TitleFont);
					result.Height = tSize.Height + 12;
					result.Width = tSize.Width + 8;
					y = tSize.Height + 12;
					tSize = TextRenderer.MeasureText(text, TextFont);
					if (result.Width < tSize.Width + 8) result.Width = tSize.Width + 8;
					result.Height = y + tSize.Height + 4;
					break;
				case Content.TitleOnly:
					tSize = TextRenderer.MeasureText(title, TitleFont);
					result.Height = tSize.Height + 8;
					result.Width = tSize.Width + 8;
					break;
				case Content.ImageAndText:
					result.Height = image.Height + 8;
					tSize = TextRenderer.MeasureText(text, TextFont);
					if (result.Height < tSize.Height + 8) result.Height = tSize.Height + 8;
					result.Width = 12 + image.Width + tSize.Width;
					break;
				case Content.ImageOnly:
					result.Width = image.Width + 8;
					result.Height = image.Height + 8;
					break;
				case Content.TextOnly:
					tSize = TextRenderer.MeasureText(text, TextFont);
					result.Height = tSize.Height + 8;
					result.Width = tSize.Width + 8;
					break;
			}
			return result;
		}
		/// <summary>
		/// Draw tooltip information on a tooltip window.
		/// </summary>
		/// <param name="g">Graphics object used to paint.</param>
		/// <param name="rect">Bounding rectangle where tooltip information to be drawn.</param>
		/// <param name="title">Tooltip title.</param>
		/// <param name="text">Tooltip text.</param>
		/// <param name="image">Tooltip image.</param>
		public static void drawToolTip(Graphics g, Rectangle rect, String title, String text, Image image) {
			if (g == null) return;
			if ((rect.Width == 0) || (rect.Height == 0)) return;
			SizeF tSize;
			int y;
			switch (getContent(title, text, image)) {
				case Content.All:
					g.DrawString(title, TitleFont, TextBrush, rect.X + 4, rect.Y + 4);
                    tSize = g.MeasureString(title, TitleFont);
                    y = (int)(8 + tSize.Height);
                    g.DrawLine(SeparatorPen, rect.X + 4, y, rect.Right - 4, y);
                    g.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), rect.X + 4, y + 1, rect.Right - 4, y + 1);
                    y = y + 4;
                    g.DrawImage(image, rect.X + 4, y, image.Width, image.Height);
                    g.DrawString(text, TextFont, TextBrush, rect.X + image.Width + 8, y);
					return;
				case Content.TitleAndImage:
                    g.DrawImage(image, rect.X + 4, rect.Y + 4, image.Width, image.Height);
                    g.DrawString(title, TitleFont, TextBrush, rect.X + 8 + image.Width, rect.Y + 4);
					return;
				case Content.TitleAndText:
                    g.DrawString(title, TitleFont, TextBrush, rect.X + 4, rect.Y + 4);
                    tSize = g.MeasureString(title, TitleFont);
                    y = (int)(8 + tSize.Height);
                    g.DrawLine(SeparatorPen, rect.X + 4, y, rect.Right - 4, y);
                    g.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), rect.X + 4, y + 1, rect.Right - 4, y + 1);
                    y = y + 4;
                    g.DrawString(text, TextFont, TextBrush, rect.X + 4, y);
					return;
				case Content.TitleOnly:
                    g.DrawString(title, TitleFont, TextBrush, rect.X + 4, rect.Y + 4);
					return;
				case Content.ImageAndText:
                    g.DrawImage(image, rect.X + 4, rect.Y + 4, image.Width, image.Height);
                    g.DrawString(text, TextFont, TextBrush, rect.X + 8 + image.Width, rect.Y + 4);
					return;
				case Content.ImageOnly:
                    g.DrawImage(image, rect.X + 4, rect.Y + 4, image.Width, image.Height);
					return;
				case Content.TextOnly:
                    g.DrawString(text, TextFont, TextBrush, rect.X + 4, rect.Y + 4);
					return;
			}
		}
		#endregion
	}
}