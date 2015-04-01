using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ai.Renderer {
    /// <summary>
    /// Porvide functions to render a checkbox.
    /// </summary>
    public class CheckBox {
        #region Draw Box
        /// <summary>
        /// Draw a box of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="x">X location of the box.</param>
        /// <param name="y">Y location of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawBox(Graphics g, int x, int y, uint size,
            Boolean enabled, Boolean hLited) {
            if (g == null) return;
            if (size == 0) return;
            Rectangle rectBox = new Rectangle(x, y, (int)size, (int)size);
            g.FillRectangle(Brushes.White, rectBox);
            if (enabled && hLited) g.DrawRectangle(new Pen(Color.FromArgb(62, 106, 170)), rectBox);
            else g.DrawRectangle(new Pen(Color.FromArgb(142, 143, 143)), rectBox);
            if (enabled && size > 6) {
                Rectangle innerRect = new Rectangle(x + 2, y + 2, (int)(size - 4), (int)(size - 4));
                Rectangle brushRect = new Rectangle(x + 1, y + 1, (int)(size - 2), (int)(size - 2));
                LinearGradientBrush borderBrush=new LinearGradientBrush(brushRect, Color.FromArgb(174, 179, 185), 
                    Color.FromArgb(233, 233, 234), 45);
                LinearGradientBrush fillBrush = new LinearGradientBrush(brushRect, Color.Black,
                    Color.White, 45);
                Color[] fillColors = new Color[3];
                float[] fillPos = new float[3];
                ColorBlend fillBlend = new ColorBlend();
                if (hLited) { 
                    fillColors[0] = Color.Yellow;
                    fillColors[1] = Color.FromArgb(232, 232, 232);
                } else { 
                    fillColors[0] = Color.FromArgb(203, 207, 213);
                    fillColors[1] = Color.FromArgb(232, 232, 232);
                }
                fillColors[2] = Color.FromArgb(246, 246, 246);
                fillPos[0] = 0.0F;
                fillPos[1] = 0.5F;
                fillPos[2] = 1.0F;
                fillBlend.Colors = fillColors;
                fillBlend.Positions = fillPos;
                fillBrush.InterpolationColors = fillBlend;
                g.FillRectangle(fillBrush, innerRect);
                g.DrawRectangle(new Pen(borderBrush), innerRect);
                borderBrush.Dispose();
                fillBrush.Dispose();
            }
        }
        /// <summary>
        /// Draw a box of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="x">X location of the box.</param>
        /// <param name="y">Y location of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawBox(Graphics g, int x, int y, uint size,
            Boolean enabled) { drawBox(g, x, y, size, enabled, false); }
        /// <summary>
        /// Draw a box of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="x">X location of the box.</param>
        /// <param name="y">Y location of the box.</param>
        /// <param name="size">Size of the box.</param>
        public static void drawBox(Graphics g, int x, int y, uint size) { drawBox(g, x, y, size, true, false); }
        /// <summary>
        /// Draw a box of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="x">X location of the box.</param>
        /// <param name="y">Y location of the box.</param>
        public static void drawBox(Graphics g, int x, int y) { drawBox(g, x, y, 14, true, false); }
        /// <summary>
        /// Draw a box of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="p">Location of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawBox(Graphics g, Point p, uint size,
            Boolean enabled, Boolean hLited) { drawBox(g, p.X, p.Y, size, enabled, hLited); }
        /// <summary>
        /// Draw a box of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="p">Location of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawBox(Graphics g, Point p, uint size,
            Boolean enabled) { drawBox(g, p.X, p.Y, size, enabled, false); }
        /// <summary>
        /// Draw a box of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="p">Location of the box.</param>
        /// <param name="size">Size of the box.</param>
        public static void drawBox(Graphics g, Point p, uint size) { drawBox(g, p.X, p.Y, size, true, false); }
        /// <summary>
        /// Draw a box of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="p">Location of the box.</param>
        public static void drawBox(Graphics g, Point p) { drawBox(g, p.X, p.Y, 14, true, false); }
        /// <summary>
        /// Draw a box of a checkbox in the center of a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="rect">Rectangle where the box to be drawn.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawBox(Graphics g, Rectangle rect, uint size,
            Boolean enabled, Boolean hLited) {
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawBox(g, x, y, size, enabled, hLited);
        }
        /// <summary>
        /// Draw a box of a checkbox in the center of a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="rect">Rectangle where the box to be drawn.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawBox(Graphics g, Rectangle rect, uint size,
            Boolean enabled) {
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawBox(g, x, y, size, enabled, false);
        }
        /// <summary>
        /// Draw a box of a checkbox in the center of a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="rect">Rectangle where the box to be drawn.</param>
        /// <param name="size">Size of the box.</param>
        public static void drawBox(Graphics g, Rectangle rect, uint size) {
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawBox(g, x, y, size, true, false);
        }
        /// <summary>
        /// Draw a box of a checkbox in the center of a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the box to be drawn.</param>
        /// <param name="rect">Rectangle where the box to be drawn.</param>
        public static void drawBox(Graphics g, Rectangle rect) {
            int x = (int)(rect.X + ((rect.Width - 14) / 2));
            int y = (int)(rect.Y + ((rect.Height - 14) / 2));
            drawBox(g, x, y, 14, true, false);
        }
        #endregion
        #region Draw Check
        /// <summary>
        /// Draw a check of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="x">X location of the check.</param>
        /// <param name="y">Y location of the check.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheck(Graphics g, int x, int y, CheckState state, uint size, Boolean enabled) {
            if (size <= 4) return;
            switch (state) { 
                case CheckState.Checked:
                    Point[] points = new Point[3];
                    points[0] = new Point(x + 1, (int)(y + (size / 2)));
                    points[1] = new Point((int)(x + (size / 2)), (int)(y + size - 1));
                    points[2] = new Point((int)(x + size - 1), y + 1);
                    if (enabled) g.DrawLines(new Pen(Color.FromArgb(62, 106, 170), 2), points);
                    else g.DrawLines(new Pen(Color.DimGray, 2), points);
                    break;
                case CheckState.Indeterminate:
                    Rectangle innerRect = new Rectangle(x + 1, y + 1, (int)(size - 2), (int)(size - 2));
                    LinearGradientBrush brush;
                    if (enabled) brush = new LinearGradientBrush(innerRect, Color.Chartreuse, Color.Green, 45);
                    else brush = new LinearGradientBrush(innerRect, Color.Silver, Color.Gray, 45);
                    g.FillRectangle(brush, innerRect);
                    brush.Dispose();
                    break;
            }
        }
        /// <summary>
        /// Draw a check of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="x">X location of the check.</param>
        /// <param name="y">Y location of the check.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        public static void drawCheck(Graphics g, int x, int y, CheckState state, uint size) { drawCheck(g, x, y, state, size, true); }
        /// <summary>
        /// Draw a check of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="x">X location of the check.</param>
        /// <param name="y">Y location of the check.</param>
        /// <param name="state">State of the check.</param>
        public static void drawCheck(Graphics g, int x, int y, CheckState state) { drawCheck(g, x, y, state, 8, true); }
        /// <summary>
        /// Draw a check of a checkbox on x, y coordinate.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="x">X location of the check.</param>
        /// <param name="y">Y location of the check.</param>
        public static void drawCheck(Graphics g, int x, int y) { drawCheck(g, x, y, CheckState.Checked, 8, true); }
        /// <summary>
        /// Draw a check of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="p">P location of the check.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheck(Graphics g, Point p, CheckState state, uint size, Boolean enabled) {
            drawCheck(g, p.X, p.Y, state, size, enabled);
        }
        /// <summary>
        /// Draw a check of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="p">P location of the check.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        public static void drawCheck(Graphics g, Point p, CheckState state, uint size) { drawCheck(g, p.X, p.Y, state, size, true); }
        /// <summary>
        /// Draw a check of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="p">P location of the check.</param>
        /// <param name="state">State of the check.</param>
        public static void drawCheck(Graphics g, Point p, CheckState state) { drawCheck(g, p.X, p.Y, state, 8, true); }
        /// <summary>
        /// Draw a check of a checkbox on p location.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="p">P location of the check.</param>
        public static void drawCheck(Graphics g, Point p) { drawCheck(g, p.X, p.Y, CheckState.Checked, 8, true); }
        /// <summary>
        /// Draw a check of a checkbox inside a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="rect">Rectangle where the check to be drawn.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheck(Graphics g, Rectangle rect, CheckState state, uint size, Boolean enabled) {
            int x = rect.X + (int)((rect.Width - size) / 2);
            int y = rect.Y + (int)((rect.Height - size) / 2);
            drawCheck(g, x, y, state, size, enabled);
        }
        /// <summary>
        /// Draw a check of a checkbox inside a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="rect">Rectangle where the check to be drawn.</param>
        /// <param name="state">State of the check.</param>
        /// <param name="size">Size of the check.</param>
        public static void drawCheck(Graphics g, Rectangle rect, CheckState state, uint size) {
            int x = rect.X + (int)((rect.Width - size) / 2);
            int y = rect.Y + (int)((rect.Height - size) / 2);
            drawCheck(g, x, y, state, size, true);
        }
        /// <summary>
        /// Draw a check of a checkbox inside a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="rect">Rectangle where the check to be drawn.</param>
        /// <param name="state">State of the check.</param>
        public static void drawCheck(Graphics g, Rectangle rect, CheckState state) {
            int x = rect.X + (int)((rect.Width - 8) / 2);
            int y = rect.Y + (int)((rect.Height - 8) / 2);
            drawCheck(g, x, y, state, 8, true);
        }
        /// <summary>
        /// Draw a check of a checkbox inside a rectangle.
        /// </summary>
        /// <param name="g">Graphics object where the check to be drawn.</param>
        /// <param name="rect">Rectangle where the check to be drawn.</param>
        public static void drawCheck(Graphics g, Rectangle rect) {
            int x = rect.X + (int)((rect.Width - 8) / 2);
            int y = rect.Y + (int)((rect.Height - 8) / 2);
            drawCheck(g, x, y, CheckState.Checked, 8, true);
        }
        #endregion
        #region Draw CheckBox
        /// <summary>
        /// Draw a CheckBox on x, y coordinate.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="x">X location of the CheckBox.</param>
        /// <param name="y">Y location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawCheckBox(Graphics g, int x, int y, CheckState state,
            uint size, Boolean enabled, Boolean hLited) {
            if (size <= 8) return;
            drawBox(g, x, y, size, enabled, hLited);
            drawCheck(g, x + 2, y + 2, state, size - 4, enabled);
        }
        /// <summary>
        /// Draw a CheckBox on x, y coordinate.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="x">X location of the CheckBox.</param>
        /// <param name="y">Y location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheckBox(Graphics g, int x, int y, CheckState state,
            uint size, Boolean enabled) { drawCheckBox(g, x, y, state, size, enabled, false); }
        /// <summary>
        /// Draw a CheckBox on x, y coordinate.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="x">X location of the CheckBox.</param>
        /// <param name="y">Y location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, int x, int y, CheckState state,
            uint size) { drawCheckBox(g, x, y, state, size, true, false); }
        /// <summary>
        /// Draw a CheckBox on x, y coordinate.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="x">X location of the CheckBox.</param>
        /// <param name="y">Y location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, int x, int y, CheckState state) { drawCheckBox(g, x, y, state, 14, true, false); }
        /// <summary>
        /// Draw a CheckBox on x, y coordinate.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="x">X location of the CheckBox.</param>
        /// <param name="y">Y location of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, int x, int y) { drawCheckBox(g, x, y, CheckState.Checked, 14, true, false); }
        /// <summary>
        /// Draw a CheckBox on p location.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="p">Location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawCheckBox(Graphics g, Point p, CheckState state,
            uint size, Boolean enabled, Boolean hLited) {
            if (size <= 8) return;
            drawBox(g, p.X, p.Y, size, enabled, hLited);
            drawCheck(g, p.X + 2, p.Y + 2, state, size - 4, enabled);
        }
        /// <summary>
        /// Draw a CheckBox on p location.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="p">Location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheckBox(Graphics g, Point p, CheckState state,
            uint size, Boolean enabled) { drawCheckBox(g, p, state, size, enabled, false); }
        /// <summary>
        /// Draw a CheckBox on p location.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="p">Location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, Point p, CheckState state,
            uint size) { drawCheckBox(g, p, state, size, true, false); }
        /// <summary>
        /// Draw a CheckBox on p location.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="p">Location of the CheckBox.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, Point p, CheckState state) { drawCheckBox(g, p, state, 14, true, false); }
        /// <summary>
        /// Draw a CheckBox on p location.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="p">Location of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, Point p) { drawCheckBox(g, p, CheckState.Checked, 14, true, false); }
        /// <summary>
        /// Draw a CheckBox inside a rectangle.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="rect">Rectangle where the CheckBox to be drawn.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        /// <param name="hLited">Determine whether checkbox is highlited.</param>
        public static void drawCheckBox(Graphics g, Rectangle rect, CheckState state,
            uint size, Boolean enabled, Boolean hLited) {
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawBox(g, x, y, size, enabled, hLited);
            drawCheck(g, x + 2, y + 2, state, size - 4, enabled);
        }
        /// <summary>
        /// Draw a CheckBox inside a rectangle.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="rect">Rectangle where the CheckBox to be drawn.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        /// <param name="enabled">Determine whether checkbox is enabled.</param>
        public static void drawCheckBox(Graphics g, Rectangle rect, CheckState state,
            uint size, Boolean enabled) { drawCheckBox(g, rect, state, size, enabled, false); }
        /// <summary>
        /// Draw a CheckBox inside a rectangle.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="rect">Rectangle where the CheckBox to be drawn.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        /// <param name="size">Size of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, Rectangle rect, CheckState state,
            uint size) { drawCheckBox(g, rect, state, size, true, false); }
        /// <summary>
        /// Draw a CheckBox inside a rectangle.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="rect">Rectangle where the CheckBox to be drawn.</param>
        /// <param name="state">CheckState of the CheckBox.</param>
        public static void drawCheckBox(Graphics g, Rectangle rect, CheckState state) { drawCheckBox(g, rect, state, 14, true, false); }
        /// <summary>
        /// Draw a CheckBox inside a rectangle.
        /// </summary>
        /// <remarks>Minimum value of size is 8.</remarks>
        /// <param name="g">Graphics object where the CheckBox to be drawn.</param>
        /// <param name="rect">Rectangle where the CheckBox to be drawn.</param>
        public static void drawCheckBox(Graphics g, Rectangle rect) { drawCheckBox(g, rect, CheckState.Checked, 14, true, false); }
        #endregion
    }
}
