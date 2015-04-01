using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ai.Renderer {
    public static class ScrollBar {
        /// <summary>
        /// Define the color blend used to fill the background of the scrollbar.
        /// </summary>
        public static ColorBlend backgroundBlend(Drawing.ColorTheme theme, int alpha) {
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[3];
            float[] pos = new float[3];
            pos[0] = 0f;
            pos[1] = 0.3f;
            pos[2] = 1f;
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    colors[0] = Color.FromArgb(alpha, 227, 227, 227);
                    colors[1] = Color.FromArgb(alpha, 237, 237, 237);
                    colors[2] = Color.FromArgb(alpha, 242, 242, 242);
                    break;
                default:
                    colors[0] = Color.FromArgb(alpha, 149, 176, 210);
                    colors[1] = Color.FromArgb(alpha, 126, 157, 198);
                    colors[2] = Color.FromArgb(alpha, 124, 155, 193);
                    break;
            }
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend backgroundBlend(Drawing.ColorTheme theme) { return backgroundBlend(theme, 255); }
        public static ColorBlend backgroundBlend(int alpha) { return backgroundBlend(Drawing.ColorTheme.Blue, alpha); }
        public static ColorBlend backgroundBlend() { return backgroundBlend(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Gets Pen object to draw the bar's border of the scrollbar, in normal state.
        /// </summary>
        public static Pen barNormalBorderPen(Drawing.ColorTheme theme, int alpha) {
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    return new Pen(Color.FromArgb(alpha, 151, 151, 151));
                default:
                    return new Pen(Color.FromArgb(alpha, 89, 105, 145));
            }
        }
        public static Pen barNormalBorderPen(Drawing.ColorTheme theme) { return barNormalBorderPen(theme, 255); }
        public static Pen barNormalBorderPen(int alpha) { return barNormalBorderPen(Drawing.ColorTheme.Blue, alpha); }
        public static Pen barNormalBorderPen() { return barNormalBorderPen(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Gets Pen object to draw the bar's border of the scrollbar, in hlited state.
        /// </summary>
        public static Pen barHLitedBorderPen(Drawing.ColorTheme theme, int alpha) {
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    return new Pen(Color.FromArgb(alpha, 60, 127, 177));
                default:
                    return new Pen(Color.FromArgb(alpha, 60, 110, 176));
            }
        }
        public static Pen barHLitedBorderPen(Drawing.ColorTheme theme) { return barHLitedBorderPen(theme, 255); }
        public static Pen barHLitedBorderPen(int alpha) { return barHLitedBorderPen(Drawing.ColorTheme.Blue, alpha); }
        public static Pen barHLitedBorderPen() { return barHLitedBorderPen(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Gets Pen object to draw the bar's border of the scrollbar, in pressed state.
        /// </summary>
        public static Pen barPressedBorderPen(Drawing.ColorTheme theme, int alpha) {
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    return new Pen(Color.FromArgb(alpha, 24, 89, 138));
                default:
                    return new Pen(Color.FromArgb(alpha, 23, 73, 138));
            }
        }
        public static Pen barPressedBorderPen(Drawing.ColorTheme theme) { return barPressedBorderPen(theme, 255); }
        public static Pen barPressedBorderPen(int alpha) { return barPressedBorderPen(Drawing.ColorTheme.Blue, alpha); }
        public static Pen barPressedBorderPen() { return barPressedBorderPen(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Define the color blend used to fill the bar of the scrollbar, in normal state.
        /// </summary>
        public static ColorBlend barNormalBlend(Drawing.ColorTheme theme, int alpha) {
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    colors[0] = Color.FromArgb(alpha, 245, 245, 245);
                    colors[1] = Color.FromArgb(alpha, 233, 233, 235);
                    colors[2] = Color.FromArgb(alpha, 217, 218, 220);
                    colors[3] = Color.FromArgb(alpha, 192, 192, 196);
                    break;
                default:
                    colors[0] = Color.FromArgb(alpha, 225, 227, 230);
                    colors[1] = Color.FromArgb(alpha, 225, 229, 238);
                    colors[2] = Color.FromArgb(alpha, 200, 212, 225);
                    colors[3] = Color.FromArgb(alpha, 182, 196, 216);
                    break;
            }
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend barNormalBlend(Drawing.ColorTheme theme) { return barNormalBlend(theme, 255); }
        public static ColorBlend barNormalBlend(int alpha) { return barNormalBlend(Drawing.ColorTheme.Blue, alpha); }
        public static ColorBlend barNormalBlend() { return barNormalBlend(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Define the color blend used to fill the bar of the scrollbar, in hlited state.
        /// </summary>
        public static ColorBlend barHLitedBlend(Drawing.ColorTheme theme, int alpha) {
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    colors[0] = Color.FromArgb(alpha, 227, 244, 252);
                    colors[1] = Color.FromArgb(alpha, 214, 238, 251);
                    colors[2] = Color.FromArgb(alpha, 169, 219, 246);
                    colors[3] = Color.FromArgb(alpha, 156, 202, 227);
                    break;
                default:
                    colors[0] = Color.FromArgb(alpha, 190, 208, 232);
                    colors[1] = Color.FromArgb(alpha, 202, 223, 250);
                    colors[2] = Color.FromArgb(alpha, 170, 203, 246);
                    colors[3] = Color.FromArgb(alpha, 211, 228, 250);
                    break;
            }
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend barHLitedBlend(Drawing.ColorTheme theme) { return barHLitedBlend(theme, 255); }
        public static ColorBlend barHLitedBlend(int alpha) { return barHLitedBlend(Drawing.ColorTheme.Blue, alpha); }
        public static ColorBlend barHLitedBlend() { return barHLitedBlend(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Define the color blend used to fill the bar of the scrollbar, in pressed state.
        /// </summary>
        public static ColorBlend barPressedBlend(Drawing.ColorTheme theme, int alpha) {
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    colors[0] = Color.FromArgb(alpha, 206, 237, 250);
                    colors[1] = Color.FromArgb(alpha, 181, 228, 247);
                    colors[2] = Color.FromArgb(alpha, 111, 202, 240);
                    colors[3] = Color.FromArgb(alpha, 102, 106, 221);
                    break;
                default:
                    colors[0] = Color.FromArgb(alpha, 156, 187, 229);
                    colors[1] = Color.FromArgb(alpha, 164, 199, 246);
                    colors[2] = Color.FromArgb(alpha, 110, 166, 240);
                    colors[3] = Color.FromArgb(alpha, 180, 209, 247);
                    break;
            }
            result.Colors = colors;
            result.Positions = pos; 
            return result;
        }
        public static ColorBlend barPressedBlend(Drawing.ColorTheme theme) { return barPressedBlend(theme, 255); }
        public static ColorBlend barPressedBlend(int alpha) { return barPressedBlend(Drawing.ColorTheme.Blue, alpha); }
        public static ColorBlend barPressedBlend() { return barPressedBlend(Drawing.ColorTheme.Blue, 255); }
        /// <summary>
        /// Define the color blend used to fill the arrow of the scroll bar.
        /// </summary>
        public static ColorBlend arrowBlend(Drawing.ColorTheme theme, int alpha) {
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[2];
            float[] pos = new float[2];
            pos[0] = 0f;
            pos[1] = 1f;
            switch (theme) { 
                case Drawing.ColorTheme.Silver:
                    colors[0] = Color.FromArgb(alpha, 69, 69, 69);
                    colors[1] = Color.FromArgb(alpha, 198, 198, 198);
                    break;
                default:
                    colors[0] = Color.FromArgb(alpha, 110, 126, 166);
                    colors[1] = Color.FromArgb(alpha, 66, 75, 99);
                    break;
            }
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend arrowBlend(Drawing.ColorTheme theme) { return arrowBlend(theme, 255); }
        public static ColorBlend arrowBlend(int alpha) { return arrowBlend(Drawing.ColorTheme.Blue, alpha); }
        public static ColorBlend arrowBlend() { return arrowBlend(Drawing.ColorTheme.Blue, 255); }
    }
}