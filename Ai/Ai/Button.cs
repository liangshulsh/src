using System.Drawing;
using System.Drawing.Drawing2D;
using System;
using Ai.Renderer;

/// <summary>
/// Class for rendering button and menu item, just a little part that have supports for BlackBlue color theme.
/// </summary>
namespace Ai.Renderer {
    /// <summary>
    /// Provide functions and objects to render a button.
    /// </summary>
    public class Button {
        #region Enumerations
        /// <summary>
        /// Enumeration to determine a split button location.
        /// </summary>
        /// <remarks><seealso cref="drawSplit"/></remarks>
        public enum SplitLocation { Top, Left, Right, Bottom }
        /// <summary>
        /// Enumeration to determine where(e.g. highlited, pressed) an effect occurs.
        /// </summary>
        /// <remarks><seealso cref="drawSplit"/></remarks>
        public enum SplitEffectLocation { None, Button, Split}
        #endregion
        #region Color Blend
        public static ColorBlend SilverNormal(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 242, 242, 242);
            colors[1] = Color.FromArgb(alpha, 235, 235, 235);
            colors[2] = Color.FromArgb(alpha, 221, 221, 221);
            colors[3] = Color.FromArgb(alpha, 207, 207, 207);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend SilverNormal() { return SilverNormal(255); }
        public static ColorBlend SilverHLited(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 234, 246, 253);
            colors[1] = Color.FromArgb(alpha, 217, 240, 252);
            colors[2] = Color.FromArgb(alpha, 190, 230, 253);
            colors[3] = Color.FromArgb(alpha, 167, 217, 245);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend SilverHLited() { return SilverHLited(255); }
        public static ColorBlend SilverPressed(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 229, 244, 252);
            colors[1] = Color.FromArgb(alpha, 196, 229, 246);
            colors[2] = Color.FromArgb(alpha, 152, 209, 239);
            colors[3] = Color.FromArgb(alpha, 104, 179, 219);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend SilverPressed() { return SilverPressed(255); }
        public static ColorBlend RedNormal(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 243, 178, 170);
            colors[1] = Color.FromArgb(alpha, 227, 133, 120);
            colors[2] = Color.FromArgb(alpha, 208, 75, 55);
            colors[3] = Color.FromArgb(alpha, 218, 134, 109);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend RedNormal() { return RedNormal(255); }
        public static ColorBlend RedHLited(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 251, 157, 139);
            colors[1] = Color.FromArgb(alpha, 239, 110, 87);
            colors[2] = Color.FromArgb(alpha, 212, 40, 9);
            colors[3] = Color.FromArgb(alpha, 230, 148, 47);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend RedHLited() { return RedHLited(255); }
        public static ColorBlend RedPressed(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            ColorBlend result = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            pos[0] = 0f;
            pos[1] = 0.5f;
            pos[2] = 0.5f;
            pos[3] = 1f;
            colors[0] = Color.FromArgb(alpha, 210, 170, 147);
            colors[1] = Color.FromArgb(alpha, 187, 120, 93);
            colors[2] = Color.FromArgb(alpha, 140, 32, 8);
            colors[3] = Color.FromArgb(alpha, 143, 108, 32);
            result.Colors = colors;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend RedPressed() { return RedPressed(255); }
        /// <summary>
        /// Represent a color blend on a disabled button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend DisabledBlend(Drawing.ColorTheme theme) {
            ColorBlend aBlend = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(244, 249, 251);
                    colors[1] = Color.FromArgb(173, 211, 230);
                    colors[2] = Color.FromArgb(145, 192, 217);
                    colors[3] = Color.FromArgb(213, 236, 247);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(102, 115, 124);
                    colors[1] = Color.FromArgb(76, 88, 104);
                    colors[2] = Color.FromArgb(51, 65, 81);
                    colors[3] = Color.FromArgb(35, 42, 61);
                    break;
            }
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            aBlend.Colors = colors;
            aBlend.Positions = pos;
            return aBlend;
        }
        public static ColorBlend DisabledBlend() {
            ColorBlend aBlend = new ColorBlend();
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            colors[0] = Color.FromArgb(244, 249, 251);
            colors[1] = Color.FromArgb(173, 211, 230);
            colors[2] = Color.FromArgb(145, 192, 217);
            colors[3] = Color.FromArgb(213, 236, 247);
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            aBlend.Colors = colors;
            aBlend.Positions = pos;
            return aBlend;
        }
        /// <summary>
        /// Represent a color blend on a normal button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend NormalBlend(Drawing.ColorTheme theme) { 
            Color[] colors=new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(217, 240, 251);
                    colors[1] = Color.FromArgb(159, 212, 240);
                    colors[2] = Color.FromArgb(126, 188, 224);
                    colors[3] = Color.FromArgb(189, 233, 254);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(216, 216, 216);
                    colors[1] = Color.FromArgb(75, 77, 76);
                    colors[2] = Color.FromArgb(1, 3, 2);
                    colors[3] = Color.FromArgb(0, 0, 0);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend NormalBlend() {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            colors[0] = Color.FromArgb(217, 240, 251);
            colors[1] = Color.FromArgb(159, 212, 240);
            colors[2] = Color.FromArgb(126, 188, 224);
            colors[3] = Color.FromArgb(189, 233, 254);
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        /// <summary>
        /// Represent a color blend on a focused button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend FocusedBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(220, 252, 255);
                    colors[1] = Color.FromArgb(124, 194, 236);
                    colors[2] = Color.FromArgb(91, 172, 220);
                    colors[3] = Color.FromArgb(190, 247, 255);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(11, 146, 255);
                    colors[1] = Color.FromArgb(75, 77, 76);
                    colors[2] = Color.FromArgb(1, 3, 2);
                    colors[3] = Color.FromArgb(0, 0, 0);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend FocusedBlend() {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            colors[0] = Color.FromArgb(220, 252, 255);
            colors[1] = Color.FromArgb(124, 194, 236);
            colors[2] = Color.FromArgb(91, 172, 220);
            colors[3] = Color.FromArgb(190, 247, 255);
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        /// <summary>
        /// Represent a color blend on a highlited (mouse hover) button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend HLitedBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(255, 253, 232);
                    colors[1] = Color.FromArgb(255, 235, 159);
                    colors[2] = Color.FromArgb(255, 213, 67);
                    colors[3] = Color.FromArgb(255, 222, 90);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(116, 193, 255);
                    colors[1] = Color.FromArgb(0, 46, 92);
                    colors[2] = Color.FromArgb(1, 2, 7);
                    colors[3] = Color.FromArgb(0, 0, 0);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend HLitedBlend() {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            colors[0] = Color.FromArgb(255, 253, 232);
            colors[1] = Color.FromArgb(255, 235, 159);
            colors[2] = Color.FromArgb(255, 213, 67);
            colors[3] = Color.FromArgb(255, 222, 90);
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        /// <summary>
        /// Represent a color blend on a highlited split button, but mouse didn't hover on the button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend HLitedLightBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(255, 254, 246);
                    colors[1] = Color.FromArgb(255, 248, 221);
                    colors[2] = Color.FromArgb(255, 241, 186);
                    colors[3] = Color.FromArgb(255, 243, 197);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(116, 193, 255);
                    colors[1] = Color.FromArgb(64, 110, 156);
                    colors[2] = Color.FromArgb(65, 66, 71);
                    colors[3] = Color.FromArgb(64, 64, 64);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend HLitedLightBlend() {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            colors[0] = Color.FromArgb(255, 254, 246);
            colors[1] = Color.FromArgb(255, 248, 221);
            colors[2] = Color.FromArgb(255, 241, 186);
            colors[3] = Color.FromArgb(255, 243, 197);
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        /// <summary>
        /// Represent a color blend on a pressed button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend PressedBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) { 
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(244, 179, 120);
                    colors[1] = Color.FromArgb(253, 158, 67);
                    colors[2] = Color.FromArgb(253, 132, 18);
                    colors[3] = Color.FromArgb(254, 161, 80);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(60, 159, 212);
                    colors[1] = Color.FromArgb(34, 89, 133);
                    colors[2] = Color.FromArgb(17, 44, 82);
                    colors[3] = Color.FromArgb(39, 108, 148);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend PressedBlend() {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            colors[0] = Color.FromArgb(244, 179, 120);
            colors[1] = Color.FromArgb(253, 158, 67);
            colors[2] = Color.FromArgb(253, 132, 18);
            colors[3] = Color.FromArgb(254, 161, 80);
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        /// <summary>
        /// Represent a color blend on a selected button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend SelectedBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(244, 221, 199);
                    colors[1] = Color.FromArgb(254, 200, 146);
                    colors[2] = Color.FromArgb(254, 161, 66);
                    colors[3] = Color.FromArgb(253, 229, 136);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(77, 101, 152);
                    colors[1] = Color.FromArgb(32, 77, 146);
                    colors[2] = Color.FromArgb(3, 58, 137);
                    colors[3] = Color.FromArgb(5, 90, 176);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend SelectedBlend() { return SelectedBlend(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a color blend on a highlited selected button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>ColorBlend.</value>
        public static ColorBlend SelectedHLiteBlend(Drawing.ColorTheme theme) {
            Color[] colors = new Color[4];
            float[] pos = new float[4];
            ColorBlend blend = new ColorBlend();
            pos[0] = 0f;
            pos[1] = 0.4f;
            pos[2] = 0.4f;
            pos[3] = 1.0f;
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    colors[0] = Color.FromArgb(253, 187, 106);
                    colors[1] = Color.FromArgb(252, 176, 89);
                    colors[2] = Color.FromArgb(250, 160, 47);
                    colors[3] = Color.FromArgb(252, 181, 17);
                    break;
                case Drawing.ColorTheme.BlackBlue:
                    colors[0] = Color.FromArgb(106, 148, 186);
                    colors[1] = Color.FromArgb(33, 102, 144);
                    colors[2] = Color.FromArgb(0, 81, 128);
                    colors[3] = Color.FromArgb(4, 169, 235);
                    break;
            }
            blend.Colors = colors;
            blend.Positions = pos;
            return blend;
        }
        public static ColorBlend SelectedHLiteBlend() { return SelectedHLiteBlend(Drawing.ColorTheme.Blue); }
        #endregion
        #region Color Pen
        public static Pen SilverNormalPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 112, 112, 112));
        }
        public static Pen SilverNormalPen() { return SilverNormalPen(255); }
        public static Pen SilverHLitedPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 60, 127, 177));
        }
        public static Pen SilverHLitedPen() { return SilverHLitedPen(255); }
        public static Pen SilverPressedPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 44, 98, 139));
        }
        public static Pen SilverPressedPen() { return SilverPressedPen(255); }
        public static Pen RedNormalPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 74, 46, 40));
        }
        public static Pen RedNormalPen() { return RedNormalPen(255); }
        public static Pen RedHLitedPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 83, 40, 34));
        }
        public static Pen RedHLitedPen() { return RedHLitedPen(255); }
        public static Pen RedPressedPen(int alpha) {
            if (alpha < 0 || alpha > 255) return null;
            return new Pen(Color.FromArgb(alpha, 45, 22, 12));
        }
        public static Pen RedPressedPen() { return RedPressedPen(255); }
        /// <summary>
        /// Represent a border pen on a normal button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen NormalBorderPen(Drawing.ColorTheme theme) { 
            switch(theme){
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(141, 173, 194));
                case Drawing.ColorTheme.BlackBlue:
                    return new Pen(Color.FromArgb(0, 0, 0));
            }
            return new Pen(Color.Black);
        }
        public static Pen NormalBorderPen() { return NormalBorderPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a border pen on a disabled button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen DisabledBorderPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(161, 189, 207));
                case Drawing.ColorTheme.BlackBlue:
                    return new Pen(Color.FromArgb(31, 31, 31));
            }
            return new Pen(Color.Black);
        }
        public static Pen DisabledBorderPen() { return DisabledBorderPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a border pen on a focused button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen FocusedBorderPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(121, 157, 182));
            }
            return new Pen(Color.Black);
        }
        public static Pen FocusedBorderPen() { return FocusedBorderPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a border pen on a highlited button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen HLitedBorderPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(219, 206, 153));
            }
            return new Pen(Color.Black);
        }
        public static Pen HLitedBorderPen() { return HLitedBorderPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a border pen on a selected button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen SelectedBorderPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(158, 130, 85));
            }
            return new Pen(Color.Black);
        }
        public static Pen SelectedBorderPen() { return SelectedBorderPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a separator pen on a normal button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen NormalSeparatorPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(216, 194, 122));
                case Drawing.ColorTheme.BlackBlue:
                    return new Pen(Color.FromArgb(0, 146, 198));
            }
            return new Pen(Color.Black);
        }
        public static Pen NormalSeparatorPen() { return NormalSeparatorPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a separator pen on a highlited button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen HLitedSeparatorPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(205, 181, 131));
            }
            return new Pen(Color.Black);
        }
        public static Pen HLitedSeparatorPen() { return HLitedSeparatorPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a separator pen on a selected button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen SelectedSeparatorPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(176, 145, 96));
            }
            return new Pen(Color.Black);
        }
        public static Pen SelectedSeparatorPen() { return SelectedSeparatorPen(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a separator pen on a pressed button.
        /// </summary>
        /// <param name="theme">Theme used to paint.</param>
        /// <value>Pen</value>
        public static Pen PressedSeparatorPen(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return new Pen(Color.FromArgb(168, 131, 86));
            }
            return new Pen(Color.Black);
        }
        public static Pen PressedSeparatorPen() { return PressedSeparatorPen(Drawing.ColorTheme.Blue); }
        #endregion
        #region Text Brush
        public static SolidBrush NormalTextBrush(int alpha, Drawing.ColorTheme theme) { 
            switch(theme){
                case Drawing.ColorTheme.Blue:
                    return new SolidBrush(Color.FromArgb(alpha, 85, 119, 163));
                case Drawing.ColorTheme.BlackBlue:
                    return new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
                default:
                    return null;
            }
        }
        public static SolidBrush NormalTextBrush(int alpha) { return NormalTextBrush(alpha, Drawing.ColorTheme.Blue); }
        public static SolidBrush NormalTextBrush(Drawing.ColorTheme theme) { return NormalTextBrush(255, theme); }
        public static SolidBrush NormalTextBrush() { return NormalTextBrush(255, Drawing.ColorTheme.Blue); }
        public static SolidBrush DisabledTextBrush(int alpha, Drawing.ColorTheme theme) {
            switch (theme){
                case Drawing.ColorTheme.Blue:
                case Drawing.ColorTheme.BlackBlue:
                    return new SolidBrush(Color.FromArgb(alpha, 118, 118, 118));
                default:
                    return null;
            }
        }
        public static SolidBrush DisabledTextBrush(int alpha) { return DisabledTextBrush(alpha, Drawing.ColorTheme.Blue); }
        public static SolidBrush DisabledTextBrush(Drawing.ColorTheme theme) { return DisabledTextBrush(255, theme); }
        public static SolidBrush DisabledTextBrush() { return DisabledTextBrush(255, Drawing.ColorTheme.Blue); }
        #endregion
        #region Glowing Color
        /// <summary>
        /// Represent a glowing color on a normal button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color NormalGlow(Drawing.ColorTheme theme) { 
            switch(theme){
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(213, 236, 247);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(91, 95, 98);
                default:
                    return Color.Black;
            }
        }
        public static Color NormalGlow() { return NormalGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a disabled button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color DisabledGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(244, 249, 251);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(115, 124, 132);
                default:
                    return Color.Black;
            }
        }
        public static Color DisabledGlow() { return DisabledGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a focused button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color FocusedGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(189, 233, 254);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(60, 159, 180);
                default:
                    return Color.Black;
            }
        }
        public static Color FocusedGlow() { return FocusedGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a highlited button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color HLitedGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(255, 235, 173);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(3, 143, 196);
                default:
                    return Color.Black;
            }
        }
        public static Color HLitedGlow() { return HLitedGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a pressed button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color PressedGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(254, 160, 77);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(53, 156, 177);
                default:
                    return Color.Black;
            }
        }
        public static Color PressedGlow() { return PressedGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a selected button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color SelectedGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(253, 241, 176);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(151, 240, 239);
                default:
                    return Color.Black;
            }
        }
        public static Color SelectedGlow() { return SelectedGlow(Drawing.ColorTheme.Blue); }
        /// <summary>
        /// Represent a glowing color on a selected highlited button.
        /// </summary>
        /// <param name="theme">Them used to paint.</param>
        /// <value>Color</value>
        public static Color SelectedHLiteGlow(Drawing.ColorTheme theme) {
            switch (theme) {
                case Drawing.ColorTheme.Blue:
                    return Color.FromArgb(251, 179, 15);
                case Drawing.ColorTheme.BlackBlue:
                    return Color.FromArgb(62, 187, 235);
                default:
                    return Color.Black;
            }
        }
        public static Color SelectedHLiteGlow() { return SelectedHLiteGlow(Drawing.ColorTheme.Blue); }
        #endregion
        #region Draw Standard Button
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.  Default value = 3.</param>
        /// <param name="enabled">Determine whether the button is enabled.  Default value = true.</param>
        /// <param name="pressed">Determine whether the button is pressed.  Default value = false.</param>
        /// <param name="selected">Determine whether the button is selected.  Default value = false.</param>
        /// <param name="hlited">Determine whether the button is highlited.  Default value = false.</param>
        /// <param name="focused">Determine whether the button has input focus. Default value = false.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded,
            Boolean enabled, Boolean pressed, Boolean selected, Boolean hlited, Boolean focused) {
            if (g == null) return;
            if ((rect.Width > 0) && (rect.Width > 2 * rounded) && (rect.Height > 0) && (rect.Height > 2 * rounded)) {
                GraphicsPath btnPath = Drawing.roundedRectangle(rect, rounded, rounded, rounded, rounded);
                Pen borderPen = null;
                Color glowColor;
                GraphicsPath shadowPath = null;
                LinearGradientBrush bgBrush = new LinearGradientBrush(rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                if (enabled) {
                    if (pressed) { 
                        bgBrush.InterpolationColors = PressedBlend(theme);
                        glowColor = PressedGlow(theme);
                        shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                        borderPen = SelectedBorderPen(theme);
                    } else {
                        if (selected) {
                            borderPen = SelectedBorderPen(theme);
                            if (hlited) { 
                                bgBrush.InterpolationColors = SelectedHLiteBlend(theme);
                                glowColor = SelectedHLiteGlow(theme);
                                shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                            } else { 
                                bgBrush.InterpolationColors = SelectedBlend(theme);
                                glowColor = SelectedGlow(theme);
                                shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                            }
                        } else {
                            if (hlited) { 
                                bgBrush.InterpolationColors = HLitedBlend(theme);
                                glowColor = HLitedGlow(theme);
                                borderPen = HLitedBorderPen(theme);
                            } else {
                                if (focused) { 
                                    bgBrush.InterpolationColors = FocusedBlend(theme);
                                    glowColor = FocusedGlow(theme);
                                    borderPen = FocusedBorderPen(theme);
                                } else { 
                                    bgBrush.InterpolationColors = NormalBlend(theme);
                                    glowColor = NormalGlow(theme);
                                    borderPen = NormalBorderPen(theme);
                                }
                            }
                        }
                    }
                } else { 
                    bgBrush.InterpolationColors = DisabledBlend(theme);
                    glowColor = DisabledGlow(theme);
                    borderPen = DisabledBorderPen(theme);
                }
                GraphicsPath glowPath;
                if (rounded >= 0) { 
                    glowPath = Drawing.getGlowingPath(new Rectangle((int)(rect.X + rounded), rect.Y + (int)(0.4 * rect.Height), 
                        rect.Width - (int)(2 * rounded), (int)(0.6 * rect.Height)), Drawing.LightingGlowPoint.BottomCenter);
                } else { 
                    glowPath = Drawing.getGlowingPath(new Rectangle(rect.X, rect.Y + (int)(0.4 * rect.Height), 
                        rect.Width, (int)(0.6 * rect.Height)), Drawing.LightingGlowPoint.BottomCenter);
                }
                PathGradientBrush glowBrush = new PathGradientBrush(glowPath);
                Color[] sColors = new Color[2];
                sColors[0] = Color.Transparent;
                sColors[1] = Color.Transparent;
                glowBrush.CenterColor = glowColor;
                glowBrush.CenterPoint = new PointF(rect.X + (rect.Width / 2), rect.Bottom - 2);
                glowBrush.SurroundColors = sColors;
                g.FillPath(bgBrush, btnPath);
                g.FillPath(glowBrush, glowPath);
                if (shadowPath != null) { 
                    LinearGradientBrush shadowBrush =new LinearGradientBrush(new Rectangle(rect.X, rect.Y, rect.Width, 5), 
                        Color.FromArgb(63, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Vertical);
                    g.FillPath(shadowBrush, shadowPath);
                    shadowBrush.Dispose();
                    shadowPath.Dispose();
                }
                if (borderPen != null) { 
                    g.DrawPath(borderPen, btnPath);
                    borderPen.Dispose();
                }
                glowBrush.Dispose();
                glowPath.Dispose();
                btnPath.Dispose();
                bgBrush.Dispose();
            }
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine whether the button is pressed.</param>
        /// <param name="selected">Determine whether the button is selected.</param>
        /// <param name="hlited">Determine whether the button is highlited.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded,
            Boolean enabled, Boolean pressed, Boolean selected, Boolean hlited) {
            draw(g, rect, theme, rounded, enabled, pressed, selected, hlited, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine whether the button is pressed.</param>
        /// <param name="selected">Determine whether the button is selected.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded,
            Boolean enabled, Boolean pressed, Boolean selected) {
            draw(g, rect, theme, rounded, enabled, pressed, selected, false, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine whether the button is pressed.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded,
            Boolean enabled, Boolean pressed) {
            draw(g, rect, theme, rounded, enabled, pressed, false, false, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded,
            Boolean enabled) {
            draw(g, rect, theme, rounded, enabled, false, false, false, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme, uint rounded) {
            draw(g, rect, theme, rounded, true, false, false, false, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="theme">Theme used to paint.</param>
        public static void draw(Graphics g, Rectangle rect, Drawing.ColorTheme theme){
            draw(g, rect, theme, 3, true, false, false, false, false);
        }
        /// <summary>
        /// Draw a button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        public static void draw(Graphics g, Rectangle rect) {
            draw(g, rect, Drawing.ColorTheme.Blue, 3, true, false, false, false, false);
        }
        #endregion
        #region Draw Splitted Button
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.  Default value = 3.</param>
        /// <param name="enabled">Determine whether the button is enabled.  Default value = true.</param>
        /// <param name="pressed">Determine where the pressed state takes effect.  Default value = None.</param>
        /// <param name="selected">Determine whether the button is selected.  Default value = false.</param>
        /// <param name="hlited">Determine where the highlited state takes effect.  Default value = None.</param>
        /// <param name="focused">Determine whether the button has input focus.  Default value = false.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded, Boolean enabled,
            SplitEffectLocation pressed, Boolean selected,
            SplitEffectLocation hlited, Boolean focused) {
            if (g == null) return;
            switch (split) { 
                case SplitLocation.Bottom:
                case SplitLocation.Top:
                    if (rect.Height <= splitSize + rounded) return;
                    break;
                case SplitLocation.Left:
                case SplitLocation.Right:
                    if (rect.Width <= splitSize + rounded) return;
                    break;
            }
            if (splitSize <= rounded) return;
            // Creating paths.
            Rectangle splitRect = new Rectangle(0, 0, 0, 0);
            GraphicsPath btnPath;
            GraphicsPath splitPath = null;
            btnPath = Drawing.roundedRectangle(rect, rounded, rounded, rounded, rounded);
            switch (split) { 
                case SplitLocation.Top:
                    splitRect = new Rectangle(rect.X, rect.Y, rect.Width, (int)splitSize);
                    splitPath = Drawing.roundedRectangle(splitRect, rounded, rounded, 0, 0);
                    break;
                case SplitLocation.Left:
                    splitRect = new Rectangle(rect.X, rect.Y, (int)splitSize, rect.Height);
                    splitPath = Drawing.roundedRectangle(splitRect, rounded, 0, rounded, 0);
                    break;
                case SplitLocation.Right:
                    splitRect = new Rectangle((int)(rect.Right - splitSize), rect.Y, (int)splitSize, rect.Height);
                    splitPath = Drawing.roundedRectangle(splitRect, 0, rounded, 0, rounded);
                    break;
                case SplitLocation.Bottom:
                    splitRect = new Rectangle(rect.X, (int)(rect.Bottom - splitSize), rect.Width, (int)splitSize);
                    splitPath = Drawing.roundedRectangle(splitRect, 0, 0, rounded, rounded);
                    break;
            }
            // Creating background brushes.
            LinearGradientBrush btnBrush = new LinearGradientBrush(rect, Color.Black, Color.White,
                LinearGradientMode.Vertical);
            LinearGradientBrush splitBrush = null;
            GraphicsPath shadowPath = null;
            Pen borderPen = null;
            Pen separatorPen = null;
            Color glowColor;
            if (enabled) {
                if (pressed != SplitEffectLocation.None) {
                    splitBrush = new LinearGradientBrush(rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                    if (pressed == SplitEffectLocation.Button) { 
                        btnBrush.InterpolationColors = PressedBlend(theme);
                        splitBrush.InterpolationColors = HLitedLightBlend(theme);
                        shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                        glowColor = PressedGlow(theme);
                    } else { 
                        btnBrush.InterpolationColors = HLitedLightBlend(theme);
                        splitBrush.InterpolationColors = PressedBlend(theme);
                        glowColor = PressedGlow(theme);
                        switch (split) { 
                            case SplitLocation.Top:
                                shadowPath = Drawing.getInnerShadowPath(splitRect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded);
                                break;
                            case SplitLocation.Left:
                                shadowPath = Drawing.getInnerShadowPath(splitRect, Drawing.ShadowPoint.Top, 4, 4, rounded, 0, rounded, 0);
                                break;
                            case SplitLocation.Right:
                                shadowPath = Drawing.getInnerShadowPath(splitRect, Drawing.ShadowPoint.Top, 4, 4, 0, rounded, 0, rounded);
                                break;
                        }
                    }
                    separatorPen = PressedSeparatorPen(theme);
                } else {
                    if (selected) {
                        if (hlited != SplitEffectLocation.None) { 
                            splitBrush = new LinearGradientBrush(rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                            if (hlited == SplitEffectLocation.Button) { 
                                btnBrush.InterpolationColors = SelectedHLiteBlend(theme);
                                splitBrush.InterpolationColors = HLitedLightBlend(theme);
                                glowColor = SelectedHLiteGlow(theme);
                                shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                            } else { 
                                btnBrush.InterpolationColors = SelectedHLiteBlend(theme);
                                splitBrush.InterpolationColors = HLitedBlend(theme);
                                glowColor = SelectedHLiteGlow(theme);
                                shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                            }
                        } else { 
                            btnBrush.InterpolationColors = SelectedBlend(theme);
                            glowColor = SelectedGlow(theme);
                            shadowPath = Drawing.getInnerShadowPath(rect, Drawing.ShadowPoint.Top, 4, 4, rounded, rounded, rounded, rounded);
                        }
                        separatorPen = SelectedSeparatorPen(theme);
                    } else {
                        if (hlited != SplitEffectLocation.None) { 
                            splitBrush = new LinearGradientBrush(rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                            if (hlited == SplitEffectLocation.Button) { 
                                btnBrush.InterpolationColors = HLitedBlend(theme);
                                splitBrush.InterpolationColors = HLitedLightBlend(theme);
                                glowColor = HLitedGlow(theme);
                                separatorPen = HLitedSeparatorPen(theme);
                            } else { 
                                btnBrush.InterpolationColors = HLitedLightBlend(theme);
                                splitBrush.InterpolationColors = HLitedBlend(theme);
                                glowColor = HLitedGlow(theme);
                                separatorPen = HLitedSeparatorPen(theme);
                            }
                            borderPen = HLitedBorderPen(theme);
                        } else {
                            if (focused) { 
                                btnBrush.InterpolationColors = FocusedBlend(theme);
                                glowColor = FocusedGlow(theme);
                                borderPen = FocusedBorderPen(theme);
                            } else { 
                                btnBrush.InterpolationColors = NormalBlend(theme);
                                glowColor = NormalGlow(theme);
                                borderPen = NormalBorderPen(theme);
                            }
                        }
                    }
                }
            } else { 
                btnBrush.InterpolationColors = DisabledBlend(theme);
                glowColor = DisabledGlow(theme);
                borderPen = DisabledBorderPen(theme);
            }
            GraphicsPath glowPath;
            if (rounded >= 0)
                glowPath = Drawing.getGlowingPath(new Rectangle((int)(rect.X + rounded), rect.Y + (int)(0.4 * rect.Height),
                    rect.Width - (int)(2 * rounded), (int)(0.6 * rect.Height)), Drawing.LightingGlowPoint.BottomCenter);
            else
                glowPath = Drawing.getGlowingPath(new Rectangle(rect.X, rect.Y + (int)(0.4 * rect.Height),
                    rect.Width, (int)(0.6 * rect.Height)), Drawing.LightingGlowPoint.BottomCenter);
            PathGradientBrush glowBrush = new PathGradientBrush(glowPath);
            Color[] sColors = new Color[2];
            sColors[0] = Color.Transparent;
            sColors[1] = Color.Transparent;
            glowBrush.CenterColor = glowColor;
            glowBrush.CenterPoint = new PointF(rect.X + (rect.Width / 2), rect.Bottom - 2);
            glowBrush.SurroundColors = sColors;
            g.FillPath(btnBrush, btnPath);
            if (pressed == SplitEffectLocation.Split) {
                if (splitBrush != null) { 
                    g.FillPath(splitBrush, splitPath);
                    splitBrush.Dispose();
                    splitBrush = null;
                }
            }
            if (shadowPath != null) {
                LinearGradientBrush shadowBrush = new LinearGradientBrush(new Rectangle(rect.X, rect.Y, rect.Width, 5),
                    Color.FromArgb(63, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Vertical);
                g.FillPath(shadowBrush, shadowPath);
                shadowBrush.Dispose();
                shadowPath.Dispose();
            }
            if (splitBrush != null) { 
                g.FillPath(splitBrush, splitPath);
                splitBrush.Dispose();
                splitBrush = null;
            }
            g.FillPath(glowBrush, glowPath);
            // Drawing separator line.
            if (separatorPen != null) {
                Point p1 = new Point();
                Point p2 = new Point();
                switch (split) { 
                    case SplitLocation.Top:
                        p1 = new Point(splitRect.X + 2, splitRect.Bottom - 1);
                        p2 = new Point(splitRect.Right - 3, splitRect.Bottom - 1);
                        break;
                    case SplitLocation.Bottom:
                        p1 = new Point(splitRect.X + 2, splitRect.Y);
                        p2 = new Point(splitRect.Right - 3, splitRect.Y);
                        break;
                    case SplitLocation.Left:
                        p1 = new Point(splitRect.Right - 1, splitRect.Y + 2);
                        p2 = new Point(splitRect.Right - 1, splitRect.Bottom - 3);
                        break;
                    case SplitLocation.Right:
                        p1 = new Point(splitRect.X, splitRect.Y + 2);
                        p2 = new Point(splitRect.X, splitRect.Bottom - 3);
                        break;
                }
                g.DrawLine(separatorPen, p1, p2);
                separatorPen.Dispose();
            }
            if (borderPen != null) { 
                g.DrawPath(borderPen, btnPath);
                borderPen.Dispose();
            }
            glowBrush.Dispose();
            glowPath.Dispose();
            btnPath.Dispose();
            splitPath.Dispose();
            btnBrush.Dispose();
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine where the pressed state takes effect.</param>
        /// <param name="selected">Determine whether the button is selected.</param>
        /// <param name="hlited">Determine where the highlited state takes effect.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded, Boolean enabled,
            SplitEffectLocation pressed, Boolean selected,
            SplitEffectLocation hlited) {
            drawSplit(g, rect, split, splitSize, theme, rounded, enabled, pressed, selected, hlited, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine where the pressed state takes effect.</param>
        /// <param name="selected">Determine whether the button is selected.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded, Boolean enabled,
            SplitEffectLocation pressed, Boolean selected) {
            drawSplit(g, rect, split, splitSize, theme, rounded, enabled, pressed, selected, SplitEffectLocation.None, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        /// <param name="pressed">Determine where the pressed state takes effect.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded, Boolean enabled,
            SplitEffectLocation pressed) {
            drawSplit(g, rect, split, splitSize, theme, rounded, enabled, pressed, false, SplitEffectLocation.None, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        /// <param name="enabled">Determine whether the button is enabled.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded, Boolean enabled) {
            drawSplit(g, rect, split, splitSize, theme, rounded, enabled, SplitEffectLocation.None, false, SplitEffectLocation.None, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        /// <param name="rounded">Rounded range of the corners of the rectangle.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme, uint rounded) {
            drawSplit(g, rect, split, splitSize, theme, rounded, true, SplitEffectLocation.None, false, SplitEffectLocation.None, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        /// <param name="theme">Theme used to paint.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize, Drawing.ColorTheme theme) {
            drawSplit(g, rect, split, splitSize, theme, 3, true, SplitEffectLocation.None, false, SplitEffectLocation.None, false);
        }
        /// <summary>
        /// Draw a split button on a Graphics object.
        /// </summary>
        /// <param name="g">Graphics object where the button to be drawn.</param>
        /// <param name="rect">Bounding rectangle of the button.</param>
        /// <param name="split">Split location of the split button.</param>
        /// <param name="splitSize">Size of the split.</param>
        public static void drawSplit(Graphics g, Rectangle rect, SplitLocation split,
            uint splitSize) {
            drawSplit(g, rect, split, splitSize, Drawing.ColorTheme.Blue, 3, true, SplitEffectLocation.None, false, SplitEffectLocation.None, false);
        }
        #endregion
    }
}