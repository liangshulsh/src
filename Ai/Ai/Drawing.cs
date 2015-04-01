using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Ai.Renderer {
    public class Drawing{
        public static CultureInfo en_us_ci = new CultureInfo("en-us");
        public static Font defaultFont = new Font("Segoe UI", 8, FontStyle.Regular);
        #region Enumerations
        /// <summary>
        /// Color theme used for rendering objects.
        /// </summary>
        public enum ColorTheme{
            Blue = 0,
            BlackBlue = 1,
            Silver = 2
        }
        /// <summary>
        /// Enumeration used to determine contents of a given tooltip parameter.
        /// </summary>
        public enum ToolTipContent { 
            TitleOnly, TitleAndText, TitleAndImage, All, ImageOnly,
            ImageAndText, TextOnly, Empty
        }
        /// <summary>
        /// Enumeration used to determine starting point of glowing light.
        /// </summary>
        public enum LightingGlowPoint { 
            TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, 
            BottomLeft, BottomCenter, BottomRight, Custom
        }
        /// <summary>
        /// Enumeration used to determine the shadow location.
        /// </summary>
        public enum ShadowPoint { 
            Top, TopLeft, TopRight, Left, 
            Right, Bottom, BottomLeft, BottomRight
        }
        /// <summary>
        /// Enumeration used to determine the direction of a triangle.
        /// </summary>
        public enum TriangleDirection { 
            Up, Left, Right, Down, UpLeft, 
            UpRight, DownLeft, DownRight
        }
        public enum GripMode { 
            Left, Right
        }
        public enum TabButtonLocation { 
            Left,
            Top,
            Right,
            Bottom
        }
        #endregion
        #region Drawing Path
        /// <summary>
        /// Create a rounded corner rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to be rounded.</param>
        /// <param name="topLeft">Range of the top left corner from the rectangle to be rounded.</param>
        /// <param name="topRight">Range of the top right corner from the rectangle to be rounded.</param>
        /// <param name="bottomLeft">Range of the bottom left corner from the rectangle to be rounded.</param>
        /// <param name="bottomRight">Range of the bottom right corner from the rectangle to be rounded.</param>
        /// <returns>A GraphicsPath object that represent a rectangle that have its corners rounded.</returns>
        /// <remarks>The <c>range</c> must be greater than or equal with zero, and must be less then or equal with a half of its rectangle's width or height.
        /// If the <c>range</c> value less than zero, then its return the rect parameter.
        /// If rectangle width greater than its height, then maximum value of <c>range</c> is a half of rectangle height.
        /// There are optionally rounded on its four corner.</remarks>
        public static GraphicsPath roundedRectangle(Rectangle rect, 
            uint topLeft, uint topRight, uint bottomLeft, uint bottomRight) {
            GraphicsPath result = new GraphicsPath();
            if (rect.Width > 0 && rect.Height > 0) {
                int maxAllowed;
                if (rect.Height < rect.Width){
                    maxAllowed = (int)(rect.Height / 2);
                }else {
                    maxAllowed = (int)(rect.Width / 2);
                }
                PointF startPoint, endPoint;
                if (topLeft > 0 && topLeft < maxAllowed) {
                    result.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180f, 90f);
                    startPoint = new PointF(rect.X + topLeft, rect.Y);
                    endPoint = new PointF(rect.X, rect.Y + topLeft);
                } else {
                    startPoint = new PointF(rect.X, rect.Y);
                    endPoint = new PointF(rect.X, rect.Y);
                }
                if (topRight > 0 && topRight < maxAllowed) {
                    result.AddLine(startPoint.X, startPoint.Y, rect.Right - (topRight + 1), rect.Y);
                    result.AddArc(rect.Right - ((topRight * 2) + 1), rect.Y, topRight * 2, topRight * 2, 270f, 90f);
                    startPoint = new PointF(rect.Right - 1, rect.Y + topRight);
                } else {
                    result.AddLine(startPoint.X, startPoint.Y, rect.Right - 1, rect.Y);
                    startPoint = new PointF(rect.Right - 1, rect.Y);
                }
                if (bottomRight > 0 && bottomRight < maxAllowed) {
                    result.AddLine(startPoint.X, startPoint.Y, startPoint.X, rect.Bottom - (bottomRight + 1));
                    result.AddArc(rect.Right - ((bottomRight * 2) + 1), rect.Bottom - ((bottomRight * 2) + 1), bottomRight * 2, bottomRight * 2, 0f, 90f);
                    startPoint = new PointF(rect.Right - (bottomRight + 1), rect.Bottom - 1);
                } else {
                    result.AddLine(startPoint.X, startPoint.Y, startPoint.X, rect.Bottom - 1);
                    startPoint = new PointF(rect.Right - 1, rect.Bottom - 1);
                }
                if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                    result.AddLine(startPoint.X, startPoint.Y, rect.X + bottomLeft, startPoint.Y);
                    result.AddArc(rect.X, rect.Bottom - ((bottomLeft * 2) + 1), bottomLeft * 2, bottomLeft * 2, 90f, 90f);
                    startPoint = new PointF(rect.X, rect.Bottom - (bottomLeft + 1));
                } else {
                    result.AddLine(startPoint.X, startPoint.Y, rect.X, startPoint.Y);
                    startPoint = new PointF(rect.X, startPoint.Y);
                }
                result.AddLine(startPoint, endPoint);
                result.CloseFigure();
                return result;
            }
            // Return the rect param.
            result.AddRectangle(rect);
            return result;
        }
        public static GraphicsPath roundedRectangle(Rectangle rect) {
            return roundedRectangle(rect, 0, 0, 0, 0);
        }
        /// <summary>
        /// Create a lighting glow path from a rectangle.
        /// </summary>
        /// <returns>A GraphicsPath object that represent a lighting glow.</returns>
        /// <param name="rect">The rectangle where lighting glow path to be created.</param>
        /// <param name="glowPoint">One of <see cref="LightingGlowPoint">LightingGlowPoint</see> enumeration value.  Determine where the light starts.</param>
        /// <param name="percentWidth">Percentage of rectangle's width used to create the path.</param>
        /// <param name="percentHeight">Percentage of rectangle's height used to create the path.</param>
        /// <param name="customX">X location where the light starts.  Used when glowPoint value is LightingGlowPoint.Custom.</param>
        /// <param name="customY">Y location where the light starts.  Used when glowPoint value is LightingGlowPoint.Custom.</param>
        public static GraphicsPath getGlowingPath(Rectangle rect, 
            LightingGlowPoint glowPoint, uint percentWidth, uint percentHeight,
            int customX, int customY) {
            Rectangle arcRect;
            GraphicsPath ePath = new GraphicsPath();
            switch (glowPoint) { 
                case LightingGlowPoint.TopLeft :
                    arcRect = new Rectangle((int)(rect.X - (rect.Width * percentWidth / 100)),
                        (int)(rect.Y - (rect.Height * percentHeight / 100)),(int)(rect.Width * percentWidth * 2 / 100),
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddLine(rect.X, rect.Y, (rect.X + (rect.Width * percentWidth / 100)), rect.Y);
                    ePath.AddArc(arcRect, 0, 90);
                    ePath.AddLine(rect.X, rect.Y + (rect.Height * percentHeight / 100), rect.X, rect.Y);
                    break;
                case LightingGlowPoint.TopCenter :
                    arcRect = new Rectangle((int)((rect.X + (rect.Width / 2)) - (rect.Width * percentWidth / 200)),
                        (int)(rect.Y - (rect.Height * percentHeight / 100)), (int)(rect.Width * percentWidth / 100), 
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddLine(rect.X + (rect.Width * (100 - percentWidth) / 200),
                        rect.Y, rect.Right - (rect.Width * (100 - percentWidth) / 200), rect.Y);
                    ePath.AddArc(arcRect, 0, 180);
                    break;
                case LightingGlowPoint.TopRight :
                    arcRect = new Rectangle((int)(rect.Right - (rect.Width * percentWidth / 100)), 
                        (int)(rect.Y - (rect.Height * percentHeight / 100)), (int)(rect.Width * percentWidth * 2 / 100), 
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddLine(rect.Right - (rect.Width * percentWidth / 100), 
                        rect.Y, rect.Right, rect.Y);
                    ePath.AddLine(rect.Right, rect.Y, rect.Right, rect.Y + (rect.Height * percentHeight / 100));
                    ePath.AddArc(arcRect, 90, 90);
                    break;
                case LightingGlowPoint.MiddleLeft :
                    arcRect = new Rectangle((int)(rect.X - (rect.Width * percentWidth / 100)), 
                        (int)((rect.Y + (rect.Height / 2)) - (rect.Height * percentHeight / 200)), 
                        (int)(rect.Width * percentWidth * 2 / 100), (int)(rect.Height * percentHeight / 100));
                    ePath.AddArc(arcRect, 270, 180);
                    ePath.AddLine(rect.X, rect.Bottom - (rect.Height * (100 - percentHeight) / 200),
                        rect.X, rect.Y + (rect.Height * (100 - percentHeight) / 200));
                    break;
                case LightingGlowPoint.MiddleCenter :
                    arcRect = new Rectangle((int)((rect.X + (rect.Width / 2)) - (rect.Width * percentWidth / 200)), 
                        (int)((rect.Y + (rect.Height / 2)) - (rect.Height * percentHeight / 200)), 
                        (int)(rect.Width * percentWidth / 100), (int)(rect.Height * percentHeight / 100));
                    ePath.AddEllipse(arcRect);
                    break;
                case LightingGlowPoint.MiddleRight :
                    arcRect = new Rectangle((int)(rect.Right - (rect.Width * percentWidth / 100)), 
                        (int)((rect.Y + (rect.Height / 2)) - (rect.Height * percentHeight / 200)), 
                        (int)(rect.Width * percentWidth * 2 / 100), (int)(rect.Height * percentHeight / 100));
                    ePath.AddLine(rect.Right, rect.Bottom - (rect.Height * (100 - percentHeight) / 200), 
                        rect.Right, rect.Y + (rect.Height * (100 - percentHeight) / 200));
                    ePath.AddArc(arcRect, 90, 180);
                    break;
                case LightingGlowPoint.BottomLeft :
                    arcRect = new Rectangle((int)(rect.X - (rect.Width * percentWidth / 100)), 
                        (int)(rect.Bottom - (rect.Height * percentHeight / 100)), (int)(rect.Width * percentWidth * 2 / 100), 
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddArc(arcRect, 270, 90);
                    ePath.AddLine((int)(rect.X + (rect.Width * percentWidth / 100)), rect.Bottom, rect.X, rect.Bottom);
                    ePath.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom - (rect.Height * percentHeight / 100));
                    break;
                case LightingGlowPoint.BottomCenter :
                    arcRect = new Rectangle((int)((rect.X + (rect.Width / 2)) - (rect.Width * percentWidth / 200)), 
                        (int)(rect.Bottom - (rect.Height * percentHeight / 100)), (int)(rect.Width * percentWidth / 100), 
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddArc(arcRect, 180, 180);
                    ePath.AddLine(rect.X + (rect.Width * (100 - percentWidth) / 200),
                        rect.Bottom, rect.Right - (rect.Width * (100 - percentWidth) / 200), rect.Bottom);
                    break;
                case LightingGlowPoint.BottomRight :
                    arcRect = new Rectangle((int)(rect.Right - (rect.Width * percentWidth / 100)), 
                        (int)(rect.Bottom - (rect.Height * percentHeight / 100)), (int)(rect.Width * percentWidth * 2 / 100), 
                        (int)(rect.Height * percentHeight * 2 / 100));
                    ePath.AddArc(arcRect, 180, 90);
                    ePath.AddLine(rect.Right, rect.Bottom - (rect.Height * percentHeight / 100), 
                        rect.Right, rect.Bottom);
                    ePath.AddLine(rect.Right, rect.Bottom, rect.Right - (rect.Width * percentWidth / 100),
                        rect.Bottom);
                    break;
                case LightingGlowPoint.Custom :
                    arcRect = new Rectangle((int)((rect.X + customX) - (rect.Width * percentWidth / 200)), 
                        (int)((rect.Y + customY) - (rect.Height * percentHeight / 200)), 
                        (int)(rect.Width * percentWidth / 100), (int)(rect.Height * percentHeight / 100));
                    ePath.AddEllipse(arcRect);
                    break;
            }
            ePath.CloseFigure();
            return ePath;
        }
        public static GraphicsPath getGlowingPath(Rectangle rect) {
            return getGlowingPath(rect, LightingGlowPoint.BottomCenter, 100, 100, 0, 0);
        }
        public static GraphicsPath getGlowingPath(Rectangle rect,
            LightingGlowPoint glowPoint) {
            return getGlowingPath(rect, glowPoint, 100, 100, 0, 0);
        }
        public static GraphicsPath getGlowingPath(Rectangle rect,
            LightingGlowPoint glowPoint, uint percentWidth, uint percentHeight) {
            return getGlowingPath(rect, glowPoint, percentWidth, percentHeight, 0, 0);
        }
        /// <summary>
        /// Create a GraphicsPath object represent an inner shadow of a rectangle.
        /// </summary>
        /// <returns>A GraphicsPath object that represent an inner shadow.</returns>
        /// <param name="rect">The rectangle where shadow path to be created.</param>
        /// <param name="shadow">One of <see cref="ShadowPoint">ShadowPoint</see> enumeration value.  Determine the place of the shadow inside the rectangle.</param>
        /// <param name="verticalRange">Shadow height, calculated from top or bottom of the rectange.</param>
        /// <param name="horizontalRange">Shadow width, calculated from left or right of the rectangle.</param>
        /// <param name="topLeft">Rounded range of the rectangle's top left corner.</param>
        /// <param name="topRight">Rounded range of the rectangle's top right corner.</param>
        /// <param name="bottomLeft">Rounded range of the rectangle's bottom left corner.</param>
        /// <param name="bottomRight">Rounded range of the rectangle's bottom right corner.</param>
        /// <remarks><seealso cref="ShadowPoint"/></remarks>
        public static GraphicsPath getInnerShadowPath(Rectangle rect,
            ShadowPoint shadow, uint verticalRange, uint horizontalRange,
            uint topLeft, uint topRight, uint bottomLeft, uint bottomRight) {
            GraphicsPath result = new GraphicsPath();
            if (rect.Width > 0 && rect.Height > 0) {
                int maxAllowed;
                if (rect.Height < rect.Width) maxAllowed = (int)(rect.Height / 2);
                else maxAllowed = (int)(rect.Width / 2);
                if (verticalRange < (int)(rect.Height / 2) && horizontalRange < (int)(rect.Width / 2)){
                    switch (shadow) {
                        case ShadowPoint.Top:
                        case ShadowPoint.TopLeft:
                        case ShadowPoint.TopRight:
                            // Shadow from top.
                            PointF startPoint, endPoint;
                            if (topLeft > 0 && topLeft < maxAllowed) {
                                result.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
                                startPoint = new PointF(rect.X + topLeft, rect.Y);
                                endPoint = new PointF(rect.X, rect.Y + topLeft);
                            } else {
                                startPoint = new PointF(rect.X, rect.Y);
                                endPoint = new PointF(rect.X, rect.Y);
                            }
                            if (topRight > 0 && topRight < maxAllowed) {
                                result.AddLine(startPoint.X, startPoint.Y, rect.Right - (topRight + 1), rect.Y);
                                result.AddArc(rect.Right - ((topRight * 2) + 1), rect.Y, topRight * 2, topRight * 2, 270, 90);
                                startPoint = new PointF(rect.Right - 1, rect.Y + topRight);
                            } else {
                                result.AddLine(startPoint.X, startPoint.Y, rect.Right - 1, rect.Y);
                                startPoint = new PointF(rect.Right - 1, rect.Y);
                            }
                            if (shadow == ShadowPoint.TopRight) {
                                if (bottomRight > 0 && bottomRight < maxAllowed) {
                                    result.AddLine(startPoint.X, startPoint.Y,
                                        startPoint.X, rect.Bottom - (bottomRight + 1));
                                    result.AddArc(rect.Right - ((bottomRight * 2) + 1),
                                        rect.Bottom - ((bottomRight * 2) + 1), bottomRight * 2, bottomRight * 2, 0, 90);
                                    startPoint = new PointF(rect.Right - (bottomRight + 1), rect.Bottom - 1);
                                } else {
                                    result.AddLine(startPoint.X, startPoint.Y, startPoint.X, rect.Bottom - 1);
                                    startPoint = new PointF(startPoint.X, rect.Bottom - 1);
                                }
                                result.AddLine(startPoint.X, startPoint.Y, startPoint.X - horizontalRange, startPoint.Y);
                                startPoint = new PointF(startPoint.X - horizontalRange, startPoint.Y);
                                if (bottomRight > 0 && bottomRight < maxAllowed) {
                                    result.AddArc(startPoint.X - bottomRight,
                                        rect.Bottom - ((bottomRight * 2) + 1), bottomRight * 2, bottomRight * 2, 90, -90);
                                    startPoint = new PointF(startPoint.X + bottomRight, rect.Bottom - (bottomRight + 1));
                                }
                                if (topRight > 0 && topRight < maxAllowed) {
                                    result.AddLine(startPoint.X, startPoint.Y,
                                        startPoint.X, rect.Y + topRight + verticalRange);
                                    result.AddArc(rect.Right - (horizontalRange + (topRight * 2) + 1),
                                        rect.Y + verticalRange, topRight * 2, topRight * 2, 0, -90);
                                    startPoint = new PointF(rect.Right - (horizontalRange + topRight + 1),
                                        rect.Y + verticalRange);
                                } else {
                                    result.AddLine(startPoint.X, startPoint.Y, startPoint.X, rect.Y + verticalRange);
                                    startPoint = new PointF(rect.Right - (horizontalRange + 1), rect.Y + verticalRange);
                                }
                            } else {
                                result.AddLine(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y + verticalRange);
                                startPoint = new PointF(startPoint.X, startPoint.Y + verticalRange);
                                if (topRight > 0 && topRight < maxAllowed) {
                                    result.AddArc(rect.Right - ((topRight * 2) + 1),
                                        startPoint.Y - topRight, topRight * 2, topRight * 2, 0, -90);
                                    startPoint = new PointF(rect.Right - 1, startPoint.Y - topRight);
                                }
                            }
                            if (shadow == ShadowPoint.TopLeft) {
                                if (topLeft > 0 && topLeft < maxAllowed)
                                {
                                    result.AddLine(startPoint, new PointF(rect.X + horizontalRange + topLeft, startPoint.Y));
                                    result.AddArc(rect.X + horizontalRange, rect.Y + verticalRange, topLeft * 2,
                                        topLeft * 2, 270, -90);
                                    startPoint = new PointF(rect.X + horizontalRange, rect.Y + verticalRange + topLeft);
                                } else {
                                    result.AddLine(startPoint, new PointF(rect.X + horizontalRange, startPoint.Y));
                                    startPoint = new PointF(rect.X + horizontalRange, rect.Y + verticalRange);
                                }
                                if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Bottom - (bottomLeft + 1)));
                                    result.AddArc(rect.X + horizontalRange, rect.Bottom - ((bottomLeft * 2) + 1),
                                        bottomLeft * 2, bottomLeft * 2, 180, -90);
                                    result.AddLine(rect.X + horizontalRange + bottomLeft,
                                        rect.Bottom - 1, rect.X + bottomLeft, rect.Bottom - 1);
                                    result.AddArc(rect.X, rect.Bottom - ((bottomLeft * 2) - 1),
                                        bottomLeft * 2, bottomLeft * 2, 90, 90);
                                    startPoint = new PointF(rect.X, rect.Bottom - (bottomLeft + 1));
                                } else {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Bottom - 1));
                                    result.AddLine(startPoint.X, rect.Bottom - 1, rect.X, rect.Bottom - 1);
                                    startPoint = new PointF(rect.X, rect.Bottom - 1);
                                }
                            } else {
                                if (topLeft > 0 && topLeft < maxAllowed) {
                                    result.AddLine(startPoint.X, startPoint.Y, rect.X + topLeft, startPoint.Y);
                                    result.AddArc(rect.X, startPoint.Y, topLeft * 2, topLeft * 2, 270, -90);
                                    startPoint = new PointF(rect.X, startPoint.Y + topLeft);
                                } else {
                                    result.AddLine(startPoint.X, startPoint.Y, rect.X, startPoint.Y);
                                    startPoint = new PointF(rect.X, startPoint.Y);
                                }
                            }
                            result.AddLine(startPoint, endPoint);
                            result.CloseFigure();
                            return result;
                        case ShadowPoint.Bottom:
                        case ShadowPoint.BottomLeft:
                        case ShadowPoint.BottomRight:
                            // Shadow from bottom.
                            //PointF startPoint, endPoint;
                            if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                                result.AddArc(rect.X, rect.Bottom - ((bottomLeft * 2) + 1),
                                    bottomLeft * 2, bottomLeft * 2, 180, -90);
                                startPoint = new PointF(rect.X + bottomLeft, rect.Bottom - 1);
                                endPoint = new PointF(rect.X, rect.Bottom - (bottomLeft + 1));
                            } else {
                                startPoint = new PointF(rect.X, rect.Bottom - 1);
                                endPoint = new PointF(rect.X, rect.Bottom - 1);
                            }
                            if (bottomRight > 0 && bottomRight < maxAllowed) {
                                result.AddLine(startPoint, new PointF(rect.Right - (bottomRight + 1), rect.Bottom - 1));
                                result.AddArc(rect.Right - ((bottomRight * 2) + 1),
                                    rect.Bottom - ((bottomRight * 2) + 1), bottomRight * 2, bottomRight * 2, 90, -90);
                                startPoint = new PointF(rect.Right - 1, rect.Bottom - (bottomRight + 1));
                            } else {
                                result.AddLine(startPoint, new PointF(rect.Right - 1, rect.Bottom - 1));
                                startPoint = new PointF(rect.Right - 1, rect.Bottom - 1);
                            }
                            if (shadow == ShadowPoint.BottomRight) {
                                if (topRight > 0 && topRight < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Y + topRight + 1));
                                    result.AddArc(rect.Right - ((topRight * 2) + 1), rect.Y, topRight * 2, topRight * 2, 0, -90);
                                    startPoint = new PointF(rect.Right - (topRight + 1), rect.Y);
                                } else {
                                    result.AddLine(startPoint, new PointF(rect.Right - 1, rect.Y));
                                    startPoint = new PointF(rect.Right - 1, rect.Y);
                                }
                                result.AddLine(startPoint, new PointF(startPoint.X - horizontalRange, rect.Y));
                                startPoint = new PointF(startPoint.X - horizontalRange, rect.Y);
                                if (topRight > 0 && topRight < maxAllowed) {
                                    result.AddArc(startPoint.X - topRight, rect.Y, topRight * 2, topRight * 2, 270, 90);
                                    startPoint = new PointF(startPoint.X + topRight, rect.Y + topRight);
                                }
                                if (bottomRight > 0 && bottomRight < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(startPoint.X,
                                        rect.Bottom - (bottomRight + verticalRange + 1)));
                                    result.AddArc(rect.Right - (horizontalRange + (bottomRight * 2) + 1),
                                        rect.Bottom - (verticalRange + (bottomRight * 2) + 1),
                                        bottomRight * 2, bottomRight * 2, 0, 90);
                                    startPoint = new PointF(rect.Right - (horizontalRange + bottomRight + 1),
                                        rect.Bottom - (verticalRange + 1));
                                } else {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Bottom - (verticalRange + 1)));
                                    startPoint = new PointF(startPoint.X, rect.Bottom - (verticalRange + 1));
                                }
                            } else {
                                result.AddLine(startPoint, new PointF(startPoint.X, startPoint.Y - verticalRange));
                                startPoint = new PointF(startPoint.X, startPoint.Y - verticalRange);
                                if (bottomRight > 0 && bottomRight < maxAllowed) {
                                    result.AddArc(rect.Right - ((bottomRight * 2) + 1),
                                        startPoint.Y - bottomRight, bottomRight * 2, bottomRight * 2, 0, 90);
                                    startPoint = new PointF(rect.Right - (bottomRight + 1), startPoint.Y + bottomRight);
                                }
                            }
                            if (shadow == ShadowPoint.BottomLeft) {
                                if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(rect.X + horizontalRange + bottomLeft, startPoint.Y));
                                    result.AddArc(rect.X + horizontalRange, rect.Bottom - (verticalRange + (bottomLeft * 2) + 1),
                                        bottomLeft * 2, bottomLeft * 2, 90, 90);
                                    startPoint = new PointF(rect.X + horizontalRange,
                                        rect.Bottom - (verticalRange + bottomLeft + 1));
                                } else {
                                    result.AddLine(startPoint, new PointF(rect.X + horizontalRange, startPoint.Y));
                                    startPoint = new PointF(rect.X + horizontalRange, rect.Bottom - (verticalRange + 1));
                                }
                                if (topLeft > 0 && topLeft < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Y + topLeft));
                                    result.AddArc(rect.X + horizontalRange, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
                                    result.AddLine(rect.X + horizontalRange + topLeft, rect.Y, rect.X + topLeft, rect.Y);
                                    result.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 270, -90);
                                    startPoint = new PointF(rect.X, rect.Y + topLeft);
                                } else {
                                    result.AddLine(startPoint, new PointF(startPoint.X, rect.Y));
                                    result.AddLine(startPoint.X, rect.Y, rect.X, rect.Y);
                                    startPoint = new PointF(rect.X, rect.Y);
                                }
                            } else {
                                if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                                    result.AddLine(startPoint, new PointF(rect.X + bottomLeft, startPoint.Y));
                                    result.AddArc(rect.X, rect.Bottom - (verticalRange + (bottomLeft * 2) + 1),
                                        bottomLeft * 2, bottomLeft * 2, 90, 90);
                                    startPoint = new PointF(rect.X, rect.Bottom - (verticalRange + bottomLeft + 1));
                                } else {
                                    result.AddLine(startPoint, new PointF(rect.X, startPoint.Y));
                                    startPoint = new PointF(rect.X, startPoint.Y);
                                }
                            }
                            result.AddLine(startPoint, endPoint);
                            result.CloseFigure();
                            return result;
                        case ShadowPoint.Left:
                            // Shadow from left
                            //PointF startPoint, endPoint;
                            if (topLeft > 0 && topLeft < maxAllowed) {
                                endPoint = new PointF(rect.X, rect.Y + topLeft);
                                result.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
                                result.AddLine(rect.X + topLeft, rect.Y, rect.X + horizontalRange + topLeft, rect.Y);
                                result.AddArc(rect.X + horizontalRange, rect.Y, topLeft * 2, topLeft * 2, 270, -90);
                                startPoint = new PointF(rect.X + horizontalRange, rect.Y + topLeft);
                            } else {
                                endPoint = new PointF(rect.X, rect.Y);
                                result.AddLine(rect.X, rect.Y, rect.X + horizontalRange, rect.Y);
                                startPoint = new PointF(rect.X + horizontalRange, rect.Y);
                            }
                            if (bottomLeft > 0 && bottomLeft < maxAllowed) {
                                result.AddLine(startPoint, new PointF(rect.X + horizontalRange,
                                    rect.Bottom - (bottomLeft + 1)));
                                result.AddArc(rect.X + horizontalRange, rect.Bottom - ((bottomLeft * 2) + 1),
                                    bottomLeft * 2, bottomLeft * 2, 180, -90);
                                result.AddLine(rect.X + horizontalRange + bottomLeft, rect.Bottom - 1,
                                    rect.X + bottomLeft, rect.Bottom - 1);
                                result.AddArc(rect.X, rect.Bottom - ((bottomLeft * 2) + 1),
                                    bottomLeft * 2, bottomLeft * 2, 90, -90);
                                startPoint = new PointF(rect.X, rect.Bottom - (bottomLeft + 1));
                            } else {
                                result.AddLine(startPoint, new PointF(rect.X + horizontalRange, rect.Bottom - 1));
                                result.AddLine(rect.X + horizontalRange, rect.Bottom - 1, rect.X, rect.Bottom - 1);
                                startPoint = new PointF(rect.X, rect.Bottom - 1);
                            }
                            result.AddLine(startPoint, endPoint);
                            result.CloseFigure();
                            return result;
                        case ShadowPoint.Right:
                            // Shadow from right
                            //PointF startPoint, endPoint;
                            if (topRight > 0 && topRight < maxAllowed) {
                                endPoint = new PointF(rect.Right - (horizontalRange + 1), rect.Y + topLeft);
                                result.AddArc(rect.Right - ((topRight * 2) + horizontalRange + 1),
                                    rect.Y, topRight * 2, topRight * 2, 0, -90);
                                result.AddLine(rect.Right - (topRight + horizontalRange + 1),
                                    rect.Y, rect.Right - (topRight + 1), rect.Y);
                                result.AddArc(rect.Right - ((topRight * 2) + 1), rect.Y,
                                    topRight * 2, topRight * 2, 270, 90);
                                startPoint = new PointF(rect.Right - 1, rect.Y + topRight);
                            } else {
                                endPoint = new PointF(rect.Right - (horizontalRange + 1), rect.Y);
                                result.AddLine(endPoint, new PointF(rect.Right - 1, rect.Y));
                                startPoint = new PointF(rect.Right - 1, rect.Y);
                            }
                            if (bottomRight > 0 && bottomRight < maxAllowed) {
                                result.AddLine(startPoint, new PointF(rect.Right - 1, rect.Bottom - (bottomRight + 1)));
                                result.AddArc(rect.Right - ((bottomRight * 2) + 1), rect.Bottom - ((bottomRight * 2) + 1),
                                    bottomRight * 2, bottomRight * 2, 0, 90);
                                result.AddLine(rect.Right - (bottomRight + 1), rect.Bottom - 1,
                                    rect.Right - (bottomRight + horizontalRange + 1), rect.Bottom - 1);
                                result.AddArc(rect.Right - ((bottomRight * 2) + horizontalRange + 1),
                                    rect.Bottom - ((bottomRight * 2) + 1), bottomRight * 2, bottomRight * 2, 90, -90);
                                startPoint = new PointF(rect.Right - (horizontalRange + 1), rect.Bottom - (bottomRight + 1));
                            } else {
                                result.AddLine(startPoint, new PointF(rect.Right - 1, rect.Bottom - 1));
                                result.AddLine(rect.Right - 1, rect.Bottom - 1, rect.Right - (horizontalRange + 1), rect.Bottom - 1);
                                startPoint = new PointF(rect.Right - (horizontalRange + 1), rect.Bottom - 1);
                            }
                            result.AddLine(startPoint, endPoint);
                            result.CloseFigure();
                            return result;
                    }
                }
            }
            result.AddRectangle(rect);
            return result;
        }
        public static GraphicsPath getInnerShadowPath(Rectangle rect) {
            return getInnerShadowPath(rect, ShadowPoint.Top, 2, 2, 0, 0, 0, 0);
        }
        public static GraphicsPath getInnerShadowPath(Rectangle rect, ShadowPoint shadow) {
            return getInnerShadowPath(rect, shadow, 2, 2, 0, 0, 0, 0);
        }
        public static GraphicsPath getInnerShadowPath(Rectangle rect, ShadowPoint shadow,
            uint verticalRange, uint horizontalRange) {
            return getInnerShadowPath(rect, shadow, verticalRange, horizontalRange, 0, 0, 0, 0);
        }
        public static GraphicsPath getInnerShadowPath(Rectangle rect, ShadowPoint shadow,
            uint topLeft, uint topRight, uint bottomLeft, uint bottomRight) {
            return getInnerShadowPath(rect, shadow, 2, 2, topLeft, topRight, bottomLeft, bottomRight);
        }
        /// <summary>
        /// Create a GraphicsPath object to draw a tab page.
        /// </summary>
        /// <param name="rect">An area where the tab is located.</param>
        /// <param name="tabWidth">Width of the tab button.</param>
        /// <param name="tabHeight">Height of the tab button.</param>
        /// <param name="rounded">Diameter of a circle to rounding the path's corners.</param>
        /// <param name="location">Location where the button of the tab should be placed.</param>
        /// <param name="range">Range of the button from the rect location.
        /// If location parameter set to TabButtonLocation.Top or Bottom, it will be measured from rect.X location,
        /// otherwise, rect.Y location.</param>
        /// <returns>Return a GrphicsPath object that represent a tab page, or null if the process failed.</returns>
        public static GraphicsPath getTabPath(Rectangle rect, int tabWidth, int tabHeight, 
            int rounded, TabButtonLocation location, int range) {
            if (rect.Width < rounded || rect.Height < rounded) return null;
            if (tabWidth < rounded || tabHeight < rounded) return null;
            if (range > 0 && range < rounded) return null;
            if (location == TabButtonLocation.Top || location == TabButtonLocation.Bottom) {
                if (rect.X + range + tabWidth > rect.Right - 1) return null;
                if (rect.Y + tabHeight + rounded > rect.Bottom - 1) return null;
            } else {
                if (rect.Y + range + tabWidth > rect.Bottom - 1) return null;
                if (rect.X + tabHeight + rounded > rect.Right - 1) return null;
            }
            GraphicsPath gp = new GraphicsPath();
            try {
                switch (location) {
                    case TabButtonLocation.Top:
                        if (range > 0) {
                            gp.AddLine(rect.X, rect.Y + rect.Height - ((int)(rounded / 2) + 1), rect.X, rect.Y + tabHeight + (int)(rounded / 2));
                            gp.AddArc(rect.X, rect.Y + tabHeight, rounded, rounded, 180, 90);
                            gp.AddLine(rect.X + (int)(rounded / 2), rect.Y + tabHeight, rect.X + range - (int)(rounded / 2), rect.Y + tabHeight);
                            gp.AddArc(rect.X + range - rounded, rect.Y + tabHeight - rounded, rounded, rounded, 90, -90);
                            gp.AddLine(rect.X + range, rect.Y + tabHeight - (int)(rounded / 2), rect.X + range, rect.Y + (int)(rounded / 2));
                        } else {
                            gp.AddLine(rect.X, rect.Y + rect.Height - ((int)(rounded / 2) + 1), rect.X, rect.Y + (int)(rounded / 2));
                        }
                        gp.AddArc(rect.X + range, rect.Y, rounded, rounded, 180, 90);
                        gp.AddLine(rect.X + (int)(rounded / 2) + range, rect.Y, rect.X + tabWidth + range - (int)(rounded / 2), rect.Y);
                        gp.AddArc(rect.X + tabWidth + range - rounded, rect.Y, rounded, rounded, 270, 90);
                        if (rect.X + range + tabWidth < rect.Right - 1) {
                            gp.AddLine(rect.X + tabWidth + range, rect.Y + (int)(rounded / 2), rect.X + tabWidth + range, rect.Y + tabHeight - (int)(rounded / 2));
                            gp.AddArc(rect.X + tabWidth + range, rect.Y + tabHeight - rounded, rounded, rounded, 180, -90);
                            gp.AddLine(rect.X + tabWidth + (int)(rounded / 2) + range, rect.Y + tabHeight, rect.Right - ((int)(rounded / 2) + 1), rect.Y + tabHeight);
                            gp.AddArc(rect.Right - (rounded + 1), rect.Y + tabHeight, rounded, rounded, 270, 90);
                            gp.AddLine(rect.Right - 1, rect.Y + tabHeight + (int)(rounded / 2), rect.Right - 1, rect.Y + rect.Height - ((int)(rounded / 2) + 1));
                        } else {
                            gp.AddLine(rect.Right - 1, rect.Y + (int)(rounded / 2), rect.Right - 1, rect.Y + rect.Height - ((int)(rounded / 2) + 1));
                        }
                        gp.AddArc(rect.X + rect.Width - (rounded + 1), rect.Y + rect.Height - (rounded + 1), rounded, rounded, 0, 90);
                        gp.AddLine(rect.X + rect.Width - ((int)(rounded / 2) + 1), rect.Y + rect.Height - 1, rect.X + (int)(rounded / 2), rect.Y + rect.Height - 1);
                        gp.AddArc(rect.X, rect.Y + rect.Height - (rounded + 1), rounded, rounded, 90, 90);
                        break;
                    case TabButtonLocation.Bottom:
                        gp.AddArc(rect.X, rect.Y, rounded, rounded, 180, 90);
                        gp.AddLine(rect.X + (int)(rounded / 2), rect.Y, rect.Right - ((int)(rounded / 2) + 1), rect.Y);
                        gp.AddArc(rect.Right - (rounded + 1), rect.Y, rounded, rounded, 270, 90);
                        if (rect.X + range + tabWidth < rect.Right - 1) {
                            gp.AddLine(rect.Right - 1, rect.Y + (int)(rounded / 2), rect.Right - 1, rect.Bottom - ((int)(rounded / 2) + tabHeight + 1));
                            gp.AddArc(rect.Right - (rounded + 1), rect.Bottom - (tabHeight + rounded + 1), rounded, rounded, 0, 90);
                            gp.AddLine(rect.Right - ((int)(rounded / 2) + 1), rect.Bottom - (tabHeight + 1), rect.X + range + tabWidth + (int)(rounded / 2), rect.Bottom - (tabHeight + 1));
                            gp.AddArc(rect.X + range + tabWidth, rect.Bottom - (tabHeight + 1), rounded, rounded, 270, -90);
                            gp.AddLine(rect.X + range + tabWidth, rect.Bottom + (int)(rounded / 2) - (tabHeight + 1), rect.X + range + tabWidth, rect.Bottom - ((int)(rounded / 2) + 1));
                        } else {
                            gp.AddLine(rect.Right - 1, rect.Y + (int)(rounded / 2), rect.Right - 1, rect.Bottom - ((int)(rounded / 2) + 1));
                        }
                        gp.AddArc(rect.X + range + tabWidth - rounded, rect.Bottom - (rounded + 1), rounded, rounded, 0, 90);
                        gp.AddLine(rect.X + range + tabWidth - (int)(rounded / 2), rect.Bottom - 1, rect.X + range + (int)(rounded / 2), rect.Bottom - 1);
                        gp.AddArc(rect.X + range, rect.Bottom - (rounded + 1), rounded, rounded, 90, 90);
                        if (range > 0) {
                            gp.AddLine(rect.X + range, rect.Bottom - ((int)(rounded / 2) + 1), rect.X + range, rect.Bottom + (int)(rounded / 2) - (tabHeight + 1));
                            gp.AddArc(rect.X + range - rounded, rect.Bottom - (tabHeight + 1), rounded, rounded, 0, -90);
                            gp.AddLine(rect.X + range - (int)(rounded / 2), rect.Bottom - (tabHeight + 1), rect.X + (int)(rounded / 2), rect.Bottom - (tabHeight + 1));
                            gp.AddArc(rect.X, rect.Bottom - (tabHeight + rounded + 1), rounded, rounded, 90, 90);
                            gp.AddLine(rect.X, rect.Bottom - (tabHeight + (int)(rounded / 2) + 1), rect.X, rect.Y + (int)(rounded / 2));
                        } else {
                            gp.AddLine(rect.X, rect.Bottom - ((int)(rounded / 2) + 1), rect.X, rect.Y + (int)(rounded / 2));
                        }
                        break;
                    case TabButtonLocation.Left:
                        gp.AddArc(rect.Right - (rounded + 1), rect.Y, rounded, rounded, 270, 90);
                        gp.AddLine(rect.Right - 1, rect.Y + (int)(rounded / 2), rect.Right - 1, rect.Bottom - ((int)(rounded / 2) + 1));
                        gp.AddArc(rect.Right - (rounded + 1), rect.Bottom - (rounded + 1), rounded, rounded, 0, 90);
                        if (rect.Y + range + tabWidth < rect.Bottom - 1) {
                            gp.AddLine(rect.Right - ((int)(rounded / 2) + 1), rect.Bottom - 1, rect.X + tabHeight + (int)(rounded / 2), rect.Bottom - 1);
                            gp.AddArc(rect.X + tabHeight, rect.Bottom - (rounded + 1), rounded, rounded, 90, 90);
                            gp.AddLine(rect.X + tabHeight, rect.Bottom - ((int)(rounded / 2) + 1), rect.X + tabHeight, rect.Y + range + tabWidth + (int)(rounded / 2));
                            gp.AddArc(rect.X + tabHeight - rounded, rect.Y + range + tabWidth, rounded, rounded, 0, -90);
                            gp.AddLine(rect.X + tabHeight - (int)(rounded / 2), rect.Y + range + tabWidth, rect.X + (int)(rounded / 2), rect.Y + range + tabWidth);
                        } else {
                            gp.AddLine(rect.Right - ((int)(rounded / 2) + 1), rect.Bottom - 1, rect.X + (int)(rounded / 2), rect.Bottom - 1);
                        }
                        gp.AddArc(rect.X, rect.Y + range + tabWidth - rounded, rounded, rounded, 90, 90);
                        gp.AddLine(rect.X, rect.Y + range + tabWidth - (int)(rounded / 2), rect.X, rect.Y + range + (int)(rounded / 2));
                        gp.AddArc(rect.X, rect.Y + range, rounded, rounded, 180, 90);
                        if (range > 0) {
                            gp.AddLine(rect.X + (int)(rounded / 2), rect.Y + range, rect.X + tabHeight - (int)(rounded / 2), rect.Y + range);
                            gp.AddArc(rect.X + tabHeight - rounded, rect.Y + range - rounded, rounded, rounded, 90, -90);
                            gp.AddLine(rect.X + tabHeight, rect.Y + range - (int)(rounded / 2), rect.X + tabHeight, rect.Y + (int)(rounded / 2));
                            gp.AddArc(rect.X + tabHeight, rect.Y, rounded, rounded, 180, 90);
                            gp.AddLine(rect.X + tabHeight + (int)(rounded / 2), rect.Y, rect.Right - ((int)(rounded / 2) + 1), rect.Y);
                        } else {
                            gp.AddLine(rect.X + (int)(rounded / 2), rect.Y, rect.Right - ((int)(rounded / 2) + 1), rect.Y);
                        }
                        break;
                    case TabButtonLocation.Right:
                        gp.AddArc(rect.X, rect.Bottom - (rounded + 1), rounded, rounded, 90, 90);
                        gp.AddLine(rect.X, rect.Bottom - ((int)(rounded / 2) + 1), rect.X, rect.Y + (int)(rounded / 2));
                        gp.AddArc(rect.X, rect.Y, rounded, rounded, 180, 90);
                        if (range > 0) {
                            gp.AddLine(rect.X + (int)(rounded / 2), rect.Y, rect.Right - ((int)(rounded / 2) + tabHeight + 1), rect.Y);
                            gp.AddArc(rect.Right - (tabHeight + rounded + 1), rect.Y, rounded, rounded, 270, 90);
                            gp.AddLine(rect.Right - (tabHeight + 1), rect.Y + (int)(rounded / 2), rect.Right - (tabHeight + 1), rect.Y + range - (int)(rounded / 2));
                            gp.AddArc(rect.Right - (tabHeight + 1), rect.Y + range - rounded, rounded, rounded, 180, -90);
                            gp.AddLine(rect.Right + (int)(rounded) - (tabHeight + 1), rect.Y + range, rect.Right - ((int)(rounded / 2) + 1), rect.Y + range);
                        } else {
                            gp.AddLine(rect.X + (int)(rounded / 2), rect.Y, rect.Right - ((int)(rounded / 2) + 1), rect.Y);
                        }
                        gp.AddArc(rect.Right - (rounded + 1), rect.Y + range, rounded, rounded, 270, 90);
                        gp.AddLine(rect.Right - 1, rect.Y + range + (int)(rounded / 2), rect.Right - 1, rect.Y + range + tabWidth - (int)(rounded / 2));
                        gp.AddArc(rect.Right - (rounded + 1), rect.Y + range + tabWidth - rounded, rounded, rounded, 0, 90);
                        if (rect.Y + range + tabWidth < rect.Bottom - 1) {
                            gp.AddLine(rect.Right - ((int)(rounded / 2) + 1), rect.X + range + tabWidth, rect.Right + (int)(rounded / 2) - (tabHeight + 1), rect.Y + range + tabWidth);
                            gp.AddArc(rect.Right - (tabHeight + 1), rect.Y + range + tabWidth, rounded, rounded, 270, -90);
                            gp.AddLine(rect.Right - (tabHeight + 1), rect.Y + range + tabWidth + (int)(rounded / 2), rect.Right - (tabHeight + 1), rect.Bottom - ((int)(rounded / 2) + 1));
                            gp.AddArc(rect.Right - (tabHeight + rounded + 1), rect.Bottom - (rounded + 1), rounded, rounded, 0, 90);
                            gp.AddLine(rect.Right - (tabHeight + (int)(rounded / 2) + 1), rect.Bottom - 1, rect.X + (int)(rounded / 2), rect.Bottom - 1);
                        } else {
                            gp.AddLine(rect.Right - ((int)(rounded / 2) + 1), rect.Bottom - 1, rect.X + (int)(rounded / 2), rect.Bottom - 1);
                        }
                        break;
                }
            } catch (Exception) {
            }
            gp.CloseFigure();
            return gp;
        }
        public static GraphicsPath getStopSignPath(Rectangle rect, int size, int thickness) {
            if (rect.Width <= 0 || rect.Height <= 0) return null;
            if (size > rect.Width || size > rect.Height) return null;
            if (size <= 0) return null;
            if (thickness >= size / 2) return null;
            int dAngle = (int)(Math.Asin((double)(thickness / 2) / ((size / 2) - thickness)) * 180 / Math.PI);
            GraphicsPath result = new GraphicsPath();
            GraphicsPath p1 = new GraphicsPath();
            GraphicsPath p2 = new GraphicsPath();
            p1.AddArc(new Rectangle(rect.X + (int)((rect.Width - size) / 2) + thickness, rect.X + (int)((rect.Width - size) / 2) + thickness, size - (thickness * 2), size - (thickness * 2)), 45 - dAngle, -(180 - (2 * dAngle)));
            p1.CloseFigure();
            p2.AddArc(new Rectangle(rect.X + (int)((rect.Width - size) / 2) + thickness, rect.X + (int)((rect.Width - size) / 2) + thickness, size - (thickness * 2), size - (thickness * 2)), 45 + dAngle, (180 - (2 * dAngle)));
            p2.CloseFigure();
            result.AddEllipse(new Rectangle(rect.X + (int)((rect.Width - size) / 2), rect.X + (int)((rect.Height - size) / 2), size, size));
            result.AddPath(p1, false);
            result.AddPath(p2, false);
            p1.Dispose();
            p2.Dispose();
            return result;
        }
        public static GraphicsPath getStopSignPath(Point p, int size, int thickness) {
            return getStopSignPath(new Rectangle(p.X, p.Y, size, size), size, thickness);
        }
        public static GraphicsPath getSwapSignPath(Rectangle rect, int size) {
            if (rect.Width <= 0 || rect.Height <= 0) return null;
            if (size < 0) return null;
            if (size > rect.Width || size > rect.Height) return null;
            Point p = new Point();
            p.X = rect.X + (int)((rect.Width - size) / 2);
            p.Y = rect.Y + (int)((rect.Height - size) / 2);
            GraphicsPath result = new GraphicsPath();
            GraphicsPath p1 = new GraphicsPath();
            GraphicsPath p2 = new GraphicsPath();
            p1.AddLine(p.X + (int)(size / 3), p.Y + (int)((size * 2 / 3) - (size * 0.2)), p.X + (int)(size / 3), p.Y + (int)(size * 2 / 3));
            p1.AddLine(p.X + (int)(size / 3), p.Y + (int)(size * 2 / 3), p.X, p.Y + (int)(size / 3));
            p1.AddLine(p.X, p.Y + (int)(size / 3), p.X + (int)(size / 3), p.Y);
            p1.AddLine(p.X + (int)(size / 3), p.Y, p.X + (int)(size / 3), p.Y + (int)(size * 0.2));
            p1.AddLine(p.X + (int)(size / 3), p.Y + (int)(size * 0.2), p.X + (int)(size * 0.55), p.Y + (int)(size * 0.2));
            p1.AddLine(p.X + (int)(size * 0.55), p.Y + (int)(size * 0.2), p.X + (int)(size * 0.55), p.Y + (int)((size * 2 / 3) - (size * 0.2)));
            p1.AddLine(p.X + (int)(size * 0.55), p.Y + (int)((size * 2 / 3) - (size * 0.2)), p.X + (int)(size / 3), p.Y + (int)((size * 2 / 3) - (size * 0.2)));
            p1.CloseFigure();
            p2.AddLine(p.X + (int)(size * 2 / 3), p.Y + (int)(size / 3), p.X + (int)(size * 2 / 3), p.Y + (int)((size / 3) + (size * 0.2)));
            p2.AddLine(p.X + (int)(size * 2 / 3), p.Y + (int)((size / 3) + (size * 0.2)), p.X + (int)(size * 0.45), p.Y + (int)((size / 3) + (size * 0.2)));
            p2.AddLine(p.X + (int)(size * 0.45), p.Y + (int)((size / 3) + (size * 0.2)), p.X + (int)(size * 0.45), p.Y + (int)(size * 0.8));
            p2.AddLine(p.X + (int)(size * 0.45), p.Y + (int)(size * 0.8), p.X + (int)(size * 2 / 3), p.Y + (int)(size * 0.8));
            p2.AddLine(p.X + (int)(size * 2 / 3), p.Y + (int)(size * 0.8), p.X + (int)(size * 2 / 3), p.Y + (size - 1));
            p2.AddLine(p.X + (int)(size * 2 / 3), p.Y + (size - 1), p.X + (size - 1), p.Y + (int)(size * 2 / 3));
            p2.AddLine(p.X + (size - 1), p.Y + (int)(size * 2 / 3), p.X + (int)(size * 2 / 3), p.Y + (int)(size / 3));
            p2.CloseFigure();
            result.AddPath(p1, false);
            result.AddPath(p2, false);
            p1.Dispose();
            p2.Dispose();
            return result;
        }
        public static GraphicsPath getSwapSignPath(Point p, int size) {
            return getSwapSignPath(new Rectangle(p.X, p.Y, size, size), size);
        }
        public static GraphicsPath getInsertSignPath(Point p, int size, int thickness) {
            if (size <= 0) return null;
            if (thickness <= 0) return null;
            if (thickness > size / 2) return null;
            GraphicsPath result = new GraphicsPath();
            int dxy = (int)((((size * 2 / 3) * Math.Sqrt(2d) / 2) - (thickness / 2)) / Math.Sqrt(2d));
            result.AddLine(p.X, p.Y + (int)(size / 3), p.X, p.Y + size - 1);
            result.AddLine(p.X, p.Y + size - 1, p.X + (int)(size * 2 / 3), p.Y + size - 1);
            result.AddLine(p.X + (int)(size * 2 / 3), p.Y + size - 1, p.X + (int)(size * 2 / 3) - dxy, p.Y + size - (dxy + 1));
            result.AddLine(p.X + (int)(size * 2 / 3) - dxy, p.Y + size - (dxy + 1), p.X + size - 1, p.Y);
            result.AddLine(p.X + size - 1, p.Y, p.X + dxy, p.Y + (int)(size / 3) + dxy);
            result.CloseFigure();
            return result;
        }
        public static GraphicsPath getInsertSignPath(Rectangle rect, int size, int thickness) {
            if (rect.Width <= 0 || rect.Height <= 0) return null;
            if (size > rect.Width || size > rect.Height) return null;
            Point p = new Point(rect.X + (int)((rect.Width - size) / 2), rect.Y + (int)((rect.Height - size) / 2));
            return getInsertSignPath(p, size, thickness);
        }
        #endregion
        #region Triangle Drawing
        /// <summary>
        /// Draw a shadowed triangle specified by x and y location, overloaded.
        /// </summary>
        /// <param name="g">Graphics object where the triangle to be drawn.</param>
        /// <param name="x">X location of the triangle.</param>
        /// <param name="y">Y location of the triangle.</param>
        /// <param name="color">Color of the triangle.</param>
        /// <param name="shadowColor">Shadow color of the triangle.</param>
        /// <param name="direction"><see cref="TriangleDirection">TriangleDirection</see>, direction of the triangle.</param>
        /// <param name="size">Size of the triangle.</param>
        /// <remarks></remarks>
        public static void drawTriangle(Graphics g, int x, int y, Color color,
            Color shadowColor, TriangleDirection direction, uint size) {
            if (size > 0) {
                PointF[] points = new PointF[4];
                PointF[] shadowPoints = new PointF[4];
                int i;
                // Create triangle's points.
                switch (direction) { 
                    case TriangleDirection.Up:
                        points[0] = new PointF(x, y + (size - 1));
                        points[1] = new PointF(x + size, y + (size - 1));
                        points[2] = new PointF(x + (size / 2), y);
                        break;
                    case TriangleDirection.Down:
                        points[0] = new PointF(x, y);
                        points[1] = new PointF(x + size, y);
                        points[2] = new PointF(x + (size / 2), y + (size - 1));
                        break;
                    case TriangleDirection.Left:
                        points[0] = new PointF(x + (size - 1), y);
                        points[1] = new PointF(x + (size - 1), y + size);
                        points[2] = new PointF(x, y + (size / 2));
                        break;
                    case TriangleDirection.Right:
                        points[0] = new PointF(x, y);
                        points[1] = new PointF(x, y + size);
                        points[2] = new PointF(x + (size - 1), y + (size / 2));
                        break;
                    case TriangleDirection.UpRight:
                    case TriangleDirection.UpLeft:
                        points[0] = new PointF(x, y);
                        points[1] = new PointF(x + size, y);
                        if (direction == TriangleDirection.UpLeft)points[2] = new PointF(x, y + size);
                        else points[2] = new PointF(x + size, y + size);
                        break;
                    case TriangleDirection.DownRight:
                    case TriangleDirection.DownLeft:
                        points[0] = new PointF(x, y + size);
                        points[1] = new PointF(x + size, y + size);
                        if (direction == TriangleDirection.DownLeft)points[2] = new PointF(x, y);
                        else points[2] = new PointF(x + size, y);
                        break;
                }
                points[3] = points[0];
                // Create triangle shadow's points
                if (direction == TriangleDirection.Down || direction == TriangleDirection.Up ||
                    direction == TriangleDirection.Left || direction == TriangleDirection.Right) { 
                    i = 0;
                    while (i < 4) {
                        shadowPoints[i] = new PointF(points[i].X, points[i].Y + 2);
                        i += 1;
                    }
                } else {
                    i = 0;
                    while (i < 4) {
                        shadowPoints[i] = new PointF(points[i].X + 1, points[i].Y + 2);
                        i += 1;
                    }
                }
                g.FillPolygon(new SolidBrush(shadowColor), shadowPoints);
                g.FillPolygon(new SolidBrush(color), points);
            }
        }
        public static void drawTriangle(Graphics g, int x, int y, Color color, Color shadowColor) {
            drawTriangle(g, x, y, color, shadowColor, TriangleDirection.Down, 6);
        }
        public static void drawTriangle(Graphics g, int x, int y, Color color, 
            Color shadowColor, TriangleDirection direction) {
            drawTriangle(g, x, y, color, shadowColor, direction, 6);
        }
        public static void drawTriangle(Graphics g, int x, int y, Color color, Color shadowColor, uint size) {
            drawTriangle(g, x, y, color, shadowColor, TriangleDirection.Down, size);
        }
        public static void drawTriangle(Graphics g, Point p, Color color, 
            Color shadowColor, TriangleDirection direction, uint size) {
            drawTriangle(g, p.X, p.Y, color, shadowColor, direction, size);
        }
        public static void drawTriangle(Graphics g, Point p, Color color, Color shadowColor){
            drawTriangle(g, p.X, p.Y, color, shadowColor, TriangleDirection.Down, 6);
        }
        public static void drawTriangle(Graphics g, Point p, Color color, 
            Color shadowColor, TriangleDirection direction){
            drawTriangle(g, p.X, p.Y, color, shadowColor, direction, 6);
        }
        public static void drawTriangle(Graphics g, Point p, Color color, Color shadowColor, uint size){
            drawTriangle(g, p.X, p.Y, color, shadowColor, TriangleDirection.Down, size);
        }
        public static void drawTriangle(Graphics g, Rectangle rect, Color color,
            Color shadowColor, TriangleDirection direction, uint size) { 
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawTriangle(g, x, y, color, shadowColor, direction, size);
        }
        public static void drawTriangle(Graphics g, Rectangle rect, Color color,
            Color shadowColor){
            int x = rect.X + ((rect.Width - 6) / 2);
            int y = rect.Y + ((rect.Height - 6) / 2);
            drawTriangle(g, x, y, color, shadowColor, TriangleDirection.Down, 6);
        }
        public static void drawTriangle(Graphics g, Rectangle rect, Color color,
            Color shadowColor, TriangleDirection direction){
            int x = rect.X + ((rect.Width - 6) / 2);
            int y = rect.Y + ((rect.Height - 6) / 2);
            drawTriangle(g, x, y, color, shadowColor, direction, 6);
        }
        public static void drawTriangle(Graphics g, Rectangle rect, Color color,
            Color shadowColor, uint size){
            int x = (int)(rect.X + ((rect.Width - size) / 2));
            int y = (int)(rect.Y + ((rect.Height - size) / 2));
            drawTriangle(g, x, y, color, shadowColor, TriangleDirection.Down, size);
        }
        #endregion
        #region Image Operations
        /// <summary>
        /// Get a resized bounding rectangle of an image specified by maxSize.
        /// </summary>
        /// <param name="img">Image to measure.</param>
        /// <param name="maxSize">Maximum width or height of the result.</param>
        /// <returns>A rectangle represent resized bounding of an image.</returns>
        /// <remarks>If image is nothing, a (0, 0, 0, 0) rectangle returned.</remarks>
        public static Rectangle getImageRectangle(Image img, uint maxSize) {
            Rectangle iRect = new Rectangle(0, 0, 0, 0);
            if (img != null){
                if (img.Width <= maxSize && img.Height <= maxSize) { 
                    iRect.Width = img.Width;
                    iRect.Height = img.Height;
                } else {
                    if (img.Width == img.Height) { 
                        iRect.Width = (int)maxSize;
                        iRect.Height = (int)maxSize;
                    } else {
                        if (img.Width > img.Height) {
                            iRect.Width = (int)maxSize;
                            iRect.Height = (int)(img.Height * maxSize / img.Width);
                        } else {
                            iRect.Height = (int)maxSize;
                            iRect.Width = (int)(img.Width * maxSize / img.Height);
                        }
                    }
                }
            }
            return iRect;
        }
        /// <summary>
        /// Get a resized bounding rectangle of an image specified by maxSize inside of a rectangle.
        /// </summary>
        /// <param name="img">Image to measure.</param>
        /// <param name="rect">Rectangle where the result to be placed.</param>
        /// <param name="maxSize">Maximum width or height of the result.</param>
        /// <returns>A rectangle represent resized bounding of an image.</returns>
        /// <remarks>If image is nothing, rect parameter returned.</remarks>
        public static Rectangle getImageRectangle(Image img, Rectangle rect, int maxSize) {
            if (img != null) {
                Rectangle iRect = new Rectangle(0, 0, 0, 0);
                if (img.Width <= maxSize && img.Height <= maxSize) { 
                    iRect.Width = img.Width;
                    iRect.Height = img.Height;
                } else {
                    if (img.Width == img.Height) {
                        iRect.Width = (int)maxSize;
                        iRect.Height = (int)maxSize;
                    } else {
                        if (img.Width > img.Height) {
                            iRect.Width = (int)maxSize;
                            iRect.Height = (int)(img.Height * maxSize / img.Width);
                        } else {
                            iRect.Height = (int)maxSize;
                            iRect.Width = (int)(img.Width * maxSize / img.Height);
                        }
                    }
                }
                iRect.X = rect.X + ((rect.Width - iRect.Width) / 2);
                iRect.Y = rect.Y + ((rect.Height - iRect.Height) / 2);
                return iRect;
            }
            return rect;
        }
        /// <summary>
        /// Get a resized bounding rectangle of an image specified by maximum width and maximum height inside of a rectangle.
        /// </summary>
        /// <param name="img">Image to measure.</param>
        /// <param name="rect">Rectangle where the result to be placed.</param>
        /// <param name="maxWidth">Maximum width of the result.</param>
        /// <param name="maxHeight">Maximum height of the result.</param>
        /// <returns>A rectangle represent resized bounding of an image.</returns>
        /// <remarks>If image is nothing, rect parameter returned.</remarks>
        public static Rectangle getImageRectangle(Image img, Rectangle rect, int maxWidth, int maxHeight) {
            if (img != null) {
                Rectangle iRect = new Rectangle(0, 0, 0, 0);
                if (img.Width <= maxWidth && img.Height <= maxHeight) { 
                    iRect.Width = img.Width;
                    iRect.Height = img.Height;
                } else {
                    if (img.Width == img.Height) {
                        iRect.Width = (int)(maxWidth > maxHeight ? maxHeight : maxWidth);
                        iRect.Height = (int)(maxWidth > maxHeight ? maxHeight : maxWidth);
                    } else {
                        if (img.Width / img.Height > maxWidth / maxHeight) {
                            iRect.Width = (int)maxWidth;
                            iRect.Height = (int)(img.Height * maxWidth / img.Width);
                        } else {
                            iRect.Height = (int)maxHeight;
                            iRect.Width = (int)(img.Width * maxHeight / img.Height);
                        }
                    }
                }
                iRect.X = rect.X + ((rect.Width - iRect.Width) / 2);
                iRect.Y = rect.Y + ((rect.Height - iRect.Height) / 2);
                return iRect;
            }
            return rect;
        }
        /// <summary>
        /// Resize an image to fit maximum value.
        /// </summary>
        /// <param name="image">Image to measure.</param>
        /// <param name="max">Maximum width or height of the result.</param>
        /// <returns>A size represent resized image size.</returns>
        /// <remarks>If image is nothing, a (0, 0) size returned.</remarks>
        public static Size scaleImage(Image image, uint max) {
            Size result = new Size(0, 0);
            if (image != null) {
                if (image.Width == image.Height) {
                    result = new Size((int)max, (int)max);
                } else {
                    if (image.Width > image.Height) {
                        result = new Size((int)max, (int)(max * image.Height / image.Width));
                    } else {
                        result = new Size((int)(max * image.Width / image.Height), (int)max);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Draw a grayscaled image from an image inside a rectangle.
        /// </summary>
        /// <param name="image">Image to be drawn.</param>
        /// <param name="rect">Rectangle where a grayscaled image to be drawn.</param>
        /// <param name="g">Graphics object where the grayscaled image to be drawn.</param>
        public static void grayScaledImage(Image image, Rectangle rect, Graphics g) {
            if (image != null) {
                System.Drawing.Imaging.ColorMatrix grayMatrix = new System.Drawing.Imaging.ColorMatrix();
                int i, j;
                System.Drawing.Imaging.ImageAttributes imgAttr = new System.Drawing.Imaging.ImageAttributes();
                i = 0;
                while (i < 5) {
                    j = 0;
                    while (j < 5) {
                        grayMatrix[i, j] = 0.0f;
                        j = +1;
                    }
                    i = +1;
                }
                grayMatrix[0, 0] = 0.299f;
                grayMatrix[0, 1] = 0.299f;
                grayMatrix[0, 2] = 0.299f;
                grayMatrix[1, 0] = 0.587f;
                grayMatrix[1, 1] = 0.587f;
                grayMatrix[1, 2] = 0.587f;
                grayMatrix[2, 0] = 0.114f;
                grayMatrix[2, 1] = 0.114f;
                grayMatrix[2, 2] = 0.114f;
                grayMatrix[3, 3] = 1.0f;
                grayMatrix[4, 4] = 1.0f;
                imgAttr.SetColorMatrix(grayMatrix);
                g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttr);
            }
        }
        #endregion
        #region Miscellaneous
        /// <summary>
        /// Get a color structure from an AHSB value
        /// </summary>
        /// <param name="a">Alpha value from the color.</param>
        /// <param name="h">Hue value from the color.</param>
        /// <param name="s">Saturation value from the color.</param>
        /// <param name="b">Brightness value from the color.</param>
        /// <returns>A color structure represent AHSB value.</returns>
        public static Color colorFromAHSB(int a, Single h, Single s, Single b) { 
            // see : http://130.113.54.154/~monger/hsl-rgb.html
            if (h < 0.0f || h > 360.0f || s < 0.0f || s > 1.0f || b < 0.0f || b > 1.0f) return Color.Black;
            if (s == 0.0f) return Color.FromArgb(a, 255 * (int)b, 255 * (int)b, 255 * (int)b);
            float temp1, temp2;
            float hConv = h / 360;
            float[] tmps = new float[3];
            int i;
            if (b < 0.5f) temp2 = b * (1 + s);
            else temp2 = (b + s) - (b * s);
            temp1 = (2 * b) - temp2;
            tmps[0] = hConv + (1 / 3);
            tmps[1] = hConv;
            tmps[2] = hConv - (1 / 3);
            i = 0;
            while (i < 3) {
                if (tmps[i] < 0) tmps[i] = tmps[i] + 1.0f;
                if (tmps[i] > 1) tmps[i] = tmps[i] - 1.0f;
                if (6.0f * tmps[i] < 1) tmps[i] = temp1 + (temp2 - temp1) * 6.0f * tmps[i];
                else if (2.0f * tmps[i] < 1) tmps[i] = temp2;
                else if (3.0f * tmps[i] < 2) tmps[i] = temp1 + (temp2 - temp1) * ((2.0f / 3.0f) - tmps[i]) * 6.0f;
                i = +1;
            }
            return Color.FromArgb(a, (int)(255 * tmps[0]), (int)(255 * tmps[1]), (int)(255 * tmps[2]));
        }
        public static Rectangle combineRect(Rectangle rect1, Rectangle rect2) {
            Rectangle rect3 = rect1;
            if (rect2.X < rect3.X) rect3.X = rect2.X;
            if (rect2.Y < rect3.Y) rect3.Y = rect2.Y;
            int rectRight = rect1.Right > rect2.Right ? rect1.Right : rect2.Right;
            int rectBottom = rect1.Bottom > rect2.Bottom ? rect1.Bottom : rect2.Bottom;
            rect3.Width = rectRight - rect3.X;
            rect3.Height = rectBottom - rect3.Y;
            return rect3;
        }
        public static ColorBlend SizingGripBlend {
            get {
                ColorBlend aBlend = new ColorBlend();
                Color[] colors = new Color[2];
                float[] pos = new float[2];
                colors[0] = Color.White;
                colors[1] = Color.FromArgb(223, 233, 239);
                pos[0] = 0f;
                pos[1] = 1f;
                aBlend.Colors = colors;
                aBlend.Positions = pos;
                return aBlend;
            }
        }
        public static Pen GripBorderPen {
            get { 
                return new Pen(Color.FromArgb(221, 231, 238));
            }
        }
        public static Brush GripDotBrush {
            get {
                return new SolidBrush(Color.FromArgb(82, 116, 167));
            }
        }
        public static ColorBlend dragBackgroundBlend(int alpha) {
            Color[] clrs = new Color[2];
            float[] pos = new float[2];
            pos[0] = 0f;
            pos[1] = 1f;
            clrs[0] = Color.FromArgb(alpha, 253, 254, 255);
            clrs[1] = Color.FromArgb(alpha, 196, 203, 219);
            ColorBlend result = new ColorBlend();
            result.Colors = clrs;
            result.Positions = pos;
            return result;
        }
        public static ColorBlend dragBackgroundBlend() { return dragBackgroundBlend(255); }
        public static void drawRightBottomGrid(Graphics g, int x, int y) {
            Rectangle rectDot = new Rectangle(0, 0, 2, 2);
            rectDot.X = x + 5;
            rectDot.Y = y + 4;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
            rectDot.X = x + 5;
            rectDot.Y = y + 7;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
            rectDot.X = x + 1;
            rectDot.Y = y + 7;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
        }
        public static void drawLeftBottomGrid(Graphics g, int x, int y) {
            Rectangle rectDot = new Rectangle(0, 0, 2, 2);
            rectDot.X = x + 1;
            rectDot.Y = y + 4;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
            rectDot.X = x + 5;
            rectDot.Y = y + 7;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
            rectDot.X = x + 1;
            rectDot.Y = y + 7;
            g.FillEllipse(Brushes.White, rectDot);
            rectDot.X = rectDot.X + 1;
            rectDot.Y = rectDot.Y - 1;
            g.FillEllipse(GripDotBrush, rectDot);
        }
        public static void drawBottomGrid(Graphics g, int x, int y) {
            Rectangle rectDot = new Rectangle(0, 0, 2, 2);
            int i;
            rectDot.X = x + 3;
            rectDot.Y = y + 3;
            i = 0;
            while (i < 4) { 
                g.FillEllipse(Brushes.White, rectDot);
                rectDot.X = rectDot.X - 1;
                rectDot.Y = rectDot.Y - 1;
                g.FillEllipse(GripDotBrush, rectDot);
                rectDot.X = rectDot.X + 6;
                rectDot.Y = rectDot.Y + 1;
                i++;
            }
        }
        public static void drawGrip(Graphics g, Point p, GripMode mode) {
            if (mode == GripMode.Right) drawRightBottomGrid(g, p.X, p.Y);
            else drawLeftBottomGrid(g, p.X, p.Y);
        }
        public static void drawGrip(Graphics g, Point p) {
            drawRightBottomGrid(g, p.X, p.Y);
        }
        public static void drawGrip(Graphics g, int x, int y, GripMode mode) {
            if (mode == GripMode.Right) drawRightBottomGrid(g, x, y);
            else drawLeftBottomGrid(g, x, y);
        }
        public static void drawGrip(Graphics g, int x, int y) {
            drawRightBottomGrid(g, x, y);
        }
        public static void drawVGrip(Graphics g, Rectangle rect) {
            Rectangle aRect = new Rectangle(rect.X + (int)((rect.Width - 20) / 2), rect.Y + 1, 20, 7);
            drawBottomGrid(g, aRect.X, aRect.Y);
        }
        public static void drawCross(Graphics g, Rectangle rect, int width, int height, Color color, int thickness, bool isDiagonal) {
            if (width <= 0 || height <= 0 || thickness <= 0) return;
            if (rect.Width == 0 || rect.Height == 0) return;
            if (width > rect.Width || height > rect.Height) return;
            Pen crossPen = new Pen(color, thickness);
            int x1, x2;
            int y1, y2;
            x1 = rect.X + ((int)((rect.Width - width) / 2));
            x2 = x1 + width - 1;
            y1 = rect.Y + ((int)((rect.Height - height) / 2));
            y2 = y1 + height - 1;
            if (isDiagonal) {
                g.DrawLine(crossPen, x1, y1, x2, y2);
                g.DrawLine(crossPen, x2, y1, x1, y2);
            } else {
                int xc = rect.X + ((int)(rect.Width / 2));
                int yc = rect.Y + ((int)(rect.Height / 2));
                g.DrawLine(crossPen, x1, yc, x2, yc);
                g.DrawLine(crossPen, xc, y1, xc, y2);
            }
            crossPen.Dispose();
        }
        public static void drawCross(Graphics g, Rectangle rect, int width, int height, Color color, bool isDiagonal) {
            drawCross(g, rect, width, height, color, 1, isDiagonal);
        }
        public static void drawCross(Graphics g, Rectangle rect, int size, Color color, int thickness, bool isDiagonal) {
            drawCross(g, rect, size, size, color, thickness, isDiagonal);
        }
        public static void drawCross(Graphics g, Rectangle rect, int size, Color color, bool isDiagonal) {
            drawCross(g, rect, size, size, color, 1, isDiagonal);
        }
        public static SolidBrush NormalTextBrush(ColorTheme theme) {
            switch (theme) { 
                case ColorTheme.Blue:
                    return new SolidBrush(Color.Black);
                case ColorTheme.BlackBlue:
                    return new SolidBrush(Color.White);
            }
            return new SolidBrush(Color.Black);
        }
        public static SolidBrush NormalTextBrush() {
            return new SolidBrush(Color.Black);
        }
        public static SolidBrush DisabledTextBrush(ColorTheme theme) {
            switch (theme) { 
                case ColorTheme.BlackBlue:
                case ColorTheme.Blue:
                    return new SolidBrush(Color.FromArgb(118, 118, 118));
                default:
                    return null;
            }
        }
        public static SolidBrush DisabledTextBrush() {
            return new SolidBrush(Color.FromArgb(118, 118, 118));
        }
        public static ColorBlend BarGlow {
            get {
                Color[] colors = new Color[4];
                float[] pos = new float[4];
                ColorBlend glowBlend = new ColorBlend();
                colors[0] = Color.FromArgb(190, 255, 255, 255);
                colors[1] = Color.FromArgb(15, 255, 255, 255);
                colors[2] = Color.FromArgb(31, 255, 255, 255);
                colors[3] = Color.FromArgb(127, 255, 255, 255);
                pos[0] = 0.0f;
                pos[1] = 0.4f;
                pos[2] = 0.4f;
                pos[3] = 1.0f;
                glowBlend.Colors = colors;
                glowBlend.Positions = pos;
                return glowBlend;
            }
        }
        public static void drawTextGlow(Graphics g, String text, Font font,
            RectangleF area, StringFormat format, Color glow, uint count, uint alphaStep) {
            if (text == "") return;
            if (font == null) return;
            if (count <= 1) return;
            if (alphaStep <= 0 || alphaStep > 255) return;
            GraphicsPath textPath = new GraphicsPath();
            Color colorStep = Color.FromArgb((int)alphaStep, glow.R, glow.G, glow.B);
            int penWidth = (int)count;
            float emSize = g.MeasureString(text, font).Height * 2 / 3;
            textPath.AddString(text, font.FontFamily, (int)font.Style, emSize, area, format);
            while (penWidth > 1) {
                Pen penGlow = new Pen(colorStep, penWidth);
                g.DrawPath(penGlow, textPath);
                penWidth--;
                penGlow.Dispose();
            }
            textPath.Dispose();
        }
        public static void drawTextGlow(Graphics g, String text, Font font,
            PointF p, StringFormat format, Color glow, uint count, uint alphaStep){
            if (text == "") return;
            if (font == null) return;
            if (count <= 1) return;
            if (alphaStep <= 0 || alphaStep > 255) return;
            GraphicsPath textPath = new GraphicsPath();
            Color colorStep = Color.FromArgb((int)alphaStep, glow.R, glow.G, glow.B);
            int penWidth = (int)count;
            textPath.AddString(text, font.FontFamily, (int)font.Style, font.Size, p, format);
            while (penWidth > 1)
            {
                Pen penGlow = new Pen(colorStep, penWidth);
                g.DrawPath(penGlow, textPath);
                penWidth--;
                penGlow.Dispose();
            }
            textPath.Dispose();
        }
        public static PointF[] getStarPoints(Point center, float range) {
            PointF[] pts = new PointF[11];
            int i = 0;
            while (i < 10) {
                if (i % 2 == 0) { 
                    pts[i].X = center.X + (float)(Math.Cos((90 + (i * 36)) * Math.PI / 180) * range);
                    pts[i].Y = center.Y - (float)(Math.Sin((90 + (i * 36)) * Math.PI / 180) * range);
                } else {
                    pts[i].X = center.X + (float)(Math.Cos((90 + (i * 36)) * Math.PI / 180) * 0.4 * range);
                    pts[i].Y = center.Y - (float)(Math.Sin((90 + (i * 36)) * Math.PI / 180) * 0.4 * range);
                }
                i++;
            }
            pts[10] = pts[0];
            return pts;
        }
        public static PointF[] getTrianglePoints(Point center, float range, TriangleDirection direction) {
            PointF[] pts = new PointF[4];
            int stAngle = 0;
            int i = 0;
            switch (direction) {
                case TriangleDirection.Down: 
                    stAngle = 270;
                    break;
                case TriangleDirection.DownLeft:
                    stAngle = 225;
                    break;
                case TriangleDirection.DownRight:
                    stAngle = 315;
                    break;
                case TriangleDirection.Left:
                    stAngle = 180;
                    break;
                case TriangleDirection.Right:
                    stAngle = 0;
                    break;
                case TriangleDirection.Up:
                    stAngle = 90;
                    break;
                case TriangleDirection.UpLeft:
                    stAngle = 135;
                    break;
                case TriangleDirection.UpRight:
                    stAngle = 45;
                    break;
            }
            while (i < 3) {
                pts[i].X = center.X + (float)(Math.Cos((stAngle + (i * 120)) * Math.PI / 180) * range);
                pts[i].Y = center.Y - (float)(Math.Sin((stAngle + (i * 120)) * Math.PI / 180) * range);
                i++;
            }
            pts[3] = pts[0];
            return pts;
        }
        public static PointF[] getTrianglePoints(Point center, float range){
            PointF[] pts = new PointF[4];
            int stAngle = 90;
            int i = 0;
            while (i < 3) {
                pts[i].X = center.X + (float)(Math.Cos((stAngle + (i * 120)) * Math.PI / 180) * range);
                pts[i].Y = center.Y - (float)(Math.Sin((stAngle + (i * 120)) * Math.PI / 180) * range);
                i++;
            }
            pts[3] = pts[0];
            return pts;
        }
        public static ColorBlend createColorBlend(Color[] colors, float[] pos) {
            ColorBlend cb = new ColorBlend();
            cb.Colors = colors;
            cb.Positions = pos;
            return cb;
        }
        #endregion
    }
}
