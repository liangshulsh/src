using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Class that behave like System.Windows.Forms.ScrollBar.  This class doesn't have a window, its used owner's window.
    /// </summary>
    public class ScrollBar {
        #region Private Fields
        #region Rectangles
        /// <summary>
        /// The main rectangle of the scrollbar.
        /// </summary>
        Rectangle _rect = new Rectangle();
        /// <summary>
        /// The ractangle represent up / left button of the scrollbar.
        /// </summary>
        Rectangle _rect1 = new Rectangle();
        /// <summary>
        /// The rectangle represent down / right button of the scrollbar.
        /// </summary>
        Rectangle _rect2 = new Rectangle();
        /// <summary>
        /// The rectangle represent the bar of the scrollbar.
        /// </summary>
        Rectangle _rectBar = new Rectangle();
        #endregion
        #region Mouse Effects
        /// <summary>
        /// Determine whether the mouse pointer is moved over the scrollbar.
        /// </summary>
        bool _hover = false;
        /// <summary>
        /// Determine whether the mouse pointer is moved over the up / left button of the scrollbar.
        /// </summary>
        bool _hover1 = false;
        /// <summary>
        /// Determine whether the mouse pointer is moved over the down / right button of the scrollbar.
        /// </summary>
        bool _hover2 = false;
        /// <summary>
        /// Determine whether the mouse pointer is moved over the bar of the scrollbar.
        /// </summary>
        bool _hoverBar = false;
        /// <summary>
        /// Determine whether the mouse is pressed over the up / left button of the scrollbar.
        /// </summary>
        bool _pressed1 = false;
        /// <summary>
        /// Determine whether the mouse is pressed over the down / right button of the scrollbar.
        /// </summary>
        bool _pressed2 = false;
        /// <summary>
        /// Determine whether the mouse is pressed over the bar of the scrollbar.
        /// </summary>
        bool _pressedBar = false;
        #endregion
        #region Properties Fields
        Orientation _orientation = Orientation.Vertical;
        int _min = 0;
        int _max = 100;
        int _value = 0;
        int _barSize = 0;
        int _smallChange = 1;
        int _largeChange = 10;
        bool _visible = true;
        bool _enabled = true;
        Renderer.Drawing.ColorTheme _theme = Ai.Renderer.Drawing.ColorTheme.Blue;
        int _alpha = 255;
        #endregion
        #region Others
        System.Windows.Forms.Control _owner = null;
        int _lastValue = 0;
        Point _startDrag = new Point();
        Timer _timer = new Timer();
        #endregion
        #endregion
        #region Public Events
        /// <summary>
        /// Occurs when the value property of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> ValueChanged;
        /// <summary>
        /// Occurs when the minimum property of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> MinimumChanged;
        /// <summary>
        /// Occurs when the maximum property of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> MaximumChanged;
        /// <summary>
        /// Occurs when the enabled property of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> EnabledChanged;
        /// <summary>
        /// Occurs when the visible property of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;
        /// <summary>
        /// Occurs when the bar's size of the ScrollBar has been changed.
        /// </summary>
        public event EventHandler<EventArgs> BarSizeChanged;
        #endregion
        #region Constructor
        public ScrollBar(System.Windows.Forms.Control owner) {
            _owner = owner;
            _rect.Width = 17;
            _rect.Height = 100;
            calculateBar();
            relocate();
            relocateBar();
            _timer.Tick += _timer_Tick;
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Relocaates all of the used rectangle.
        /// </summary>
        private void relocate() {
            if (_orientation == Orientation.Vertical) {
                _rect1.X = _rect.X;
                _rect1.Y = _rect.Y;
                _rect1.Width = _rect.Width;
                _rect1.Height = 17;
                _rect2.X = _rect.X;
                _rect2.Y = _rect.Bottom - 18;
                _rect2.Width = _rect.Width;
                _rect2.Height = 17;
                _rectBar.X = _rect.X;
                _rectBar.Width = _rect.Width;
            } else {
                _rect1.X = _rect.X;
                _rect1.Y = _rect.Y;
                _rect1.Width = 17;
                _rect1.Height = _rect.Height;
                _rect2.X = _rect.Right - 18;
                _rect2.Y = _rect.Y;
                _rect2.Width = 17;
                _rect2.Height = _rect.Height;
                _rectBar.Y = _rect.Y;
                _rectBar.Height = _rect.Height;
            }
        }
        /// <summary>
        /// Relocates the position of the bar.
        /// </summary>
        private void relocateBar() {
            int space = 0;
            if (_orientation == Orientation.Vertical) {
                space = _rect.Height - (36 + _rectBar.Height);
                _rectBar.Y = _rect.Y + 18 + (int)(space * _value / (_max - _min));
            } else {
                space = _rect.Width - (36 + _rectBar.Width);
                _rectBar.X = _rect.X + 18 + (int)(space * _value / (_max - _min));
            }
        }
        /// <summary>
        /// Calculates the size of the bar.
        /// </summary>
        private void calculateBar() {
            int space = 0;
            if (_orientation == Orientation.Vertical) {
                space = _rect.Height - 36;
                if (_barSize == 0) _rectBar.Height = (int)(space / (_max - _min));
                else _rectBar.Height = (int)(space * _barSize / 100);
                if (_rectBar.Height <= 17) _rectBar.Height = 17;
            } else {
                space = _rect.Width - 36;
                if (_barSize == 0) _rectBar.Width = (int)(space / (_max - _min));
                else _rectBar.Width = (int)(space * _barSize / 100);
                if (_rectBar.Width <= 17) _rectBar.Width = 17;
            }
        }
        /// <summary>
        /// Draws the triangle sign of the ScrollBar.
        /// </summary>
        private void drawTriangle(Rectangle area, Renderer.Drawing.TriangleDirection direction, Graphics g) {
            Rectangle rect3 = new Rectangle();
            LinearGradientBrush brush3;
            GraphicsPath gp = new GraphicsPath();
            if (direction == Ai.Renderer.Drawing.TriangleDirection.Up || direction == Ai.Renderer.Drawing.TriangleDirection.Down) {
                rect3.Width = 9;
                rect3.Height = 5;
                rect3.X = area.X + (int)((area.Width - 9) / 2);
                rect3.Y = area.Y + (int)((area.Height - 5) / 2);
                if (_theme == Ai.Renderer.Drawing.ColorTheme.Silver) brush3 = new LinearGradientBrush(rect3, Color.Black, Color.White, 45);
                else brush3 = new LinearGradientBrush(rect3, Color.Black, Color.White, LinearGradientMode.Vertical);
                if (direction == Ai.Renderer.Drawing.TriangleDirection.Up) {
                    gp.AddLine(rect3.X, rect3.Bottom - 1, rect3.X + 4, rect3.Y);
                    gp.AddLine(rect3.X + 4, rect3.Y, rect3.Right - 1, rect3.Bottom - 1);
                    gp.AddLine(rect3.Right - 1, rect3.Bottom - 1, rect3.X, rect3.Bottom - 1);
                } else {
                    gp.AddLine(rect3.X, rect3.Y, rect3.Right - 1, rect3.Y);
                    gp.AddLine(rect3.Right - 1, rect3.Y, rect3.X + 4, rect3.Bottom - 1);
                    gp.AddLine(rect3.X + 4, rect3.Bottom - 1, rect3.X, rect3.Y);
                }
            } else {
                rect3.Width = 5;
                rect3.Height = 9;
                rect3.X = area.X + (int)((area.Width - 5) / 2);
                rect3.Y = area.Y + (int)((area.Height - 9) / 2);
                if (_theme == Ai.Renderer.Drawing.ColorTheme.Silver) brush3 = new LinearGradientBrush(rect3, Color.Black, Color.White, 45);
                else brush3 = new LinearGradientBrush(rect3, Color.Black, Color.White, LinearGradientMode.Horizontal);
                if (direction == Ai.Renderer.Drawing.TriangleDirection.Left) {
                    gp.AddLine(rect3.X, rect3.Y + 4, rect3.Right - 1, rect3.Y);
                    gp.AddLine(rect3.Right - 1, rect3.Y, rect3.Right - 1, rect3.Bottom - 1);
                    gp.AddLine(rect3.Right - 1, rect3.Bottom - 1, rect3.X, rect3.Y + 4);
                } else {
                    gp.AddLine(rect3.X, rect3.Y, rect3.Right - 1, rect3.Y + 4);
                    gp.AddLine(rect3.Right - 1, rect3.Y + 4, rect3.X, rect3.Bottom - 1);
                    gp.AddLine(rect3.X, rect3.Bottom - 1, rect3.X, rect3.Y);
                }
            }
            gp.CloseFigure();
            brush3.InterpolationColors = Renderer.ScrollBar.arrowBlend(_theme, _alpha);
            g.FillPath(brush3, gp);
            gp.Dispose();
            brush3.Dispose();
        }
        /// <summary>
        /// Draws bar's decoration.
        /// </summary>
        private void drawBars(Graphics g) {
            int x = 0;
            int y = 0;
            int i = 0;
            if (_orientation == Orientation.Vertical) {
                y = _rectBar.Y + (int)((_rectBar.Height - 7) / 2);
                x = _rectBar.X + (int)((_rectBar.Width - 9) / 2);
                i = 0;
                while (i < 4) {
                    g.DrawLine(new Pen(Color.FromArgb(127, 93, 93, 93)), x, y, x + 8, y);
                    y += 2;
                    i++;
                }
            } else {
                y = _rectBar.Y + (int)((_rectBar.Height - 9) / 2);
                x = _rectBar.X + (int)((_rectBar.Width - 7) / 2);
                i = 0;
                while (i < 4) {
                    g.DrawLine(new Pen(Color.FromArgb(127, 93, 93, 93)), x, y, x, y + 8);
                    x += 2;
                    i++;
                }
            }
        }
        /// <summary>
        /// Handles Tick event for _timer object.
        /// </summary>
        private void _timer_Tick(object sender, EventArgs e) {
            _timer.Interval = 100;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or Sets the minimum value allowed for the ScrollBar.
        /// </summary>
        public int Minimum {
            get { return _min; }
            set {
                if (_min != value) {
                    if (value <= _max) {
                        bool vChanged = false;
                        _min = value;
                        if (_value < _min) {
                            _value = _min;
                            vChanged = true;
                        }
                        calculateBar();
                        relocateBar();
                        if (vChanged) if (ValueChanged != null) ValueChanged(this, new EventArgs());
                        if (MinimumChanged != null) MinimumChanged(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Gets or Sets the maximum value allowed for the ScrollBar.
        /// </summary>
        public int Maximum {
            get { return _max; }
            set {
                if (_max != value) {
                    if (value >= _min) {
                        bool vChanged = false;
                        _max = value;
                        if (value > _max) {
                            value = _max;
                            vChanged = true;
                        }
                        calculateBar();
                        relocateBar();
                        if (vChanged) if (ValueChanged != null) ValueChanged(this, new EventArgs());
                        if (MaximumChanged != null) MaximumChanged(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the ScrollBar can respond to user interaction.
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set {
                if (_enabled != value) {
                    _enabled = value;
                    if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the ScrollBar and all its parent controls are displayed.
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;
                    if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the scroll box on the ScrollBar.
        /// </summary>
        public int Value {
            get { return _value; }
            set {
                if (_value != value) {
                    if (value >= _min && value <= _max) {
                        _value = value;
                        relocateBar();
                        if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the bar size of the ScrollBar.  The value must be larger or equal than 0 and less than 100.
        /// </summary>
        public int BarSize {
            get { return _barSize; }
            set {
                if (_barSize != value) {
                    if (value >= 0 && value < 100) {
                        _barSize = value;
                        calculateBar();
                        relocateBar();
                        if (BarSizeChanged != null) BarSizeChanged(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the X location of the ScrollBar.
        /// </summary>
        public int X {
            get { return _rect.X; }
            set {
                if (_rect.X != value) {
                    _rect.X = value;
                    relocate();
                    relocateBar();
                }
            }
        }
        /// <summary>
        /// Gets or sets the Y location of the ScrollBar.
        /// </summary>
        public int Y {
            get { return _rect.Y; }
            set {
                if (_rect.Y != value) {
                    _rect.Y = value;
                    relocate();
                    relocateBar();
                }
            }
        }
        /// <summary>
        /// Gets or sets the width of the ScrollBar.
        /// </summary>
        public int Width {
            get { return _rect.Width; }
            set {
                if (_rect.Width != value) {
                    if (_orientation == Orientation.Horizontal) {
                        if (value > 60) {
                            _rect.Width = value;
                            relocate();
                            calculateBar();
                            relocateBar();
                        }
                    } else {
                        if (value >= 17) {
                            _rect.Width = value;
                            relocate();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the ScrollBar.
        /// </summary>
        public int Height {
            get { return _rect.Height; }
            set {
                if (_rect.Height != value) {
                    if (_orientation == Orientation.Horizontal) {
                        if (value >= 17) {
                            _rect.Height = value;
                            relocate();
                        }
                    } else {
                        if (value > 60) {
                            _rect.Height = value;
                            relocate();
                            calculateBar();
                            relocateBar();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the display orientation of the ScrollBar.
        /// </summary>
        public Orientation Orientation {
            get { return _orientation; }
            set {
                if (_orientation != value) {
                    int rw = _rect.Width;
                    int rh = _rect.Height;
                    _orientation = value;
                    _rect.Width = rh;
                    _rect.Height = rw;
                    relocate();
                    calculateBar();
                    relocateBar();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value to be added to or subtracted from the Value property when the scroll box is moved a small distance.
        /// </summary>
        public int SmallChange {
            get { return _smallChange; }
            set { _smallChange = value; }
        }
        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the Value property when the scroll box is moved a large distance.
        /// </summary>
        public int LargeChange {
            get { return _largeChange; }
            set { _largeChange = value; }
        }
        /// <summary>
        /// Gets or sets alpha value used to draw the ScrollBar.
        /// </summary>
        public int Alpha {
            get { return _alpha; }
            set {
                if (_alpha != value) {
                    if (value >= 0 && value < 256) {
                        _alpha = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets color theme to be used to draw the ScrollBar.
        /// </summary>
        public Ai.Renderer.Drawing.ColorTheme Theme {
            get { return _theme; }
            set { _theme = value; }
        }
        public int Right { get { return _rect.Right; } }
        public int Bottom { get { return _rect.Bottom; } }
        public Rectangle Bounds { get { return _rect; } }
        public System.Windows.Forms.Control Owner { get { return _owner; } }
        #endregion
        #region Public Methods
        #region Mouse Event Handlers
        public bool mouseMove(MouseEventArgs e) {
            if (!(_visible && _enabled)) return false;
            bool changed = false;
            if (_pressedBar) { 
                // ScrollBar's bar being dragged.
                if (_orientation == Orientation.Vertical) {
                    if (e.Location.Y < _rect.Y - 50 || e.Location.Y > _rect.Bottom + 50) {
                        if (_value != _lastValue) {
                            _value = _lastValue;
                            relocateBar();
                            changed = true;
                            if (ValueChanged != null) ValueChanged(this, new EventArgs());
                        }
                    } else {
                        int delta = e.Location.Y - _startDrag.Y;
                        int space = _rect.Height - (_rectBar.Height + 36);
                        float counter = space / (_max - _min);
                        int dValue = (int)(delta / counter);
                        if (_lastValue + dValue >= _min && _lastValue + dValue <= _max) {
                            if (_lastValue + dValue != _value) {
                                _value = _lastValue + dValue;
                                if (_value < _min) _value = _min;
                                if (_value > _max) _value = _max;
                                relocateBar();
                                changed = true;
                                if (ValueChanged != null) ValueChanged(this, new EventArgs());
                            }
                        }
                    }
                } else {
                    if (e.Location.X < _rect.X - 50 || e.Location.X > _rect.Right + 50) {
                        if (_value != _lastValue) {
                            _value = _lastValue;
                            relocateBar();
                            changed = true;
                            if (ValueChanged != null) ValueChanged(this, new EventArgs());
                        }
                    } else {
                        int delta = e.Location.X - _startDrag.X;
                        int space = _rect.Width - (_rectBar.Width + 36);
                        float counter = space / (_max - _min);
                        int dValue = (int)(delta / counter);
                        if (_lastValue + dValue >= _min && _lastValue + dValue <= _max) {
                            if (_lastValue + dValue != _value) {
                                _value = _lastValue + dValue;
                                if (_value < _min) _value = _min;
                                if (_value > _max) _value = _max;
                                relocateBar();
                                changed = true;
                                if (ValueChanged != null) ValueChanged(this, new EventArgs());
                            }
                        }
                    }
                }
            }
            if (_rect.Contains(e.Location)) {
                if (!_hover) {
                    _hover = true;
                    changed = true;
                }
                if (_rect1.Contains(e.Location)) {
                    if (!_hover1) {
                        _hover1 = true;
                        changed = true;
                    }
                } else {
                    if (_hover1) {
                        _hover1 = false;
                        changed = true;
                    }
                }
                if (_rect2.Contains(e.Location)) {
                    if (!_hover2) {
                        _hover2 = true;
                        changed = true;
                    }
                } else {
                    if (_hover2) {
                        _hover2 = false;
                        changed = true;
                    }
                }
                if (_rectBar.Contains(e.Location)) {
                    if (!_hoverBar) {
                        _hoverBar = true;
                        changed = true;
                    }
                } else {
                    if (_hoverBar) {
                        _hoverBar = false;
                        changed = true;
                    }
                }
            } else {
                if (_hover) {
                    _hover = false;
                    changed = true;
                }
                if (_hover1) {
                    _hover1 = false;
                    changed = true;
                }
                if (_hover2) {
                    _hover2 = false;
                    changed = true;
                }
                if (_hoverBar) {
                    _hoverBar = false;
                    changed = true;
                }
            }
            return changed;
        }
        public bool mouseLeave(EventArgs e) {
            if (!(_visible && _enabled)) return false;
            if (_hover || _hover1 || _hover2 || _hoverBar) {
                _hover = false;
                _hover1 = false;
                _hover2 = false;
                _hoverBar = false;
                return true;
            }
            return false;
        }
        public bool mouseDown(MouseEventArgs e) {
            if (!(_visible && _enabled)) return false;
            if (_hover) {
                bool changed = false;
                if (_hover1 || _hover2 || _hoverBar) {
                    if (_hover1) {
                        if (!_pressed1) {
                            _pressed1 = true;
                            changed = true;
                        }
                        if (_value > _min) {
                            _value -= 1;
                            relocateBar();
                            if (ValueChanged != null) ValueChanged(this, new EventArgs());
                            changed = true;
                        }
                    } else if (_hover2) {
                        if (!_pressed2) {
                            _pressed2 = true;
                            changed = true;
                        }
                        if (_value < _max) {
                            _value += 1;
                            relocateBar();
                            if (ValueChanged != null) ValueChanged(this, new EventArgs());
                            changed = true;
                        }
                    } else {
                        if (!_pressedBar) {
                            _pressedBar = true;
                            changed = true;
                            _lastValue = _value;
                            _startDrag = e.Location;
                        }
                    }
                } else {
                    if (_orientation == Orientation.Vertical) {
                        if (e.Location.Y < _rectBar.Y) {
                            if (_value > _min) {
                                _value -= _largeChange;
                                if (_value < _min) _value = _min;
                                changed = true;
                            }
                        } else {
                            if (_value < _max) {
                                _value += _largeChange;
                                if (_value > _max) _value = _max;
                                changed = true;
                            }
                        }
                    } else {
                        if (e.Location.X < _rectBar.X) {
                            if (_value > _min) {
                                _value -= _largeChange;
                                if (_value < _min) _value = _min;
                                changed = true;
                            }
                        } else {
                            if (_value < _max) {
                                _value += _largeChange;
                                if (_value > _max) _value = _max;
                                changed = true;
                            }
                        }
                    }
                    if (changed) {
                        relocateBar();
                        if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    }
                }
                return changed;
            }
            return false;
        }
        public bool mouseUp(EventArgs e) {
            if (!(_visible && _enabled)) return false;
            if (_pressed1 || _pressed2 || _pressedBar) {
                _pressed1 = false;
                _pressed2 = false;
                _pressedBar = false;
                return true;
            }
            return false;
        }
        public bool mouseWheel(MouseEventArgs e) {
            if (!(_visible && _enabled)) return false;
            bool changed = false;
            if (e.Delta > 0) {
                if (_value > _min) {
                    _value -= _smallChange;
                    if (_value < _min) _value = _min;
                    relocateBar();
                    changed = true;
                    if (ValueChanged != null) ValueChanged(this, new EventArgs());
                }
            } else {
                if (_value < _max) {
                    _value += _smallChange;
                    if (_value > _max) _value = _max;
                    relocateBar();
                    changed = true;
                    if (ValueChanged != null) ValueChanged(this, new EventArgs());
                }
            }
            return changed;
        }
        #endregion
        #region Drawing
        public void draw(Graphics g) {
            if (!_visible) return;
            // Draw the background.
            LinearGradientBrush bgBrush;
            if (_orientation == Orientation.Vertical) bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Horizontal);
            else bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
            bgBrush.InterpolationColors = Renderer.ScrollBar.backgroundBlend(_theme, _alpha);
            g.FillRectangle(bgBrush, _rect);
            if (_enabled) {
                LinearGradientBrush bgBar, bgBtn1, bgBtn2;
                GraphicsPath gpBar, gpBtn1, gpBtn2;
                bgBar = null;
                bgBtn1 = null;
                bgBtn2 = null;
                gpBar = null;
                gpBtn1 = null;
                gpBtn2 = null;
                if (_hover || _pressed1 || _pressed2 || _pressedBar) {
                    if (_orientation == Orientation.Vertical) {
                        bgBar = new LinearGradientBrush(_rectBar, Color.White, Color.Black, LinearGradientMode.Horizontal);
                        bgBtn1 = new LinearGradientBrush(_rect1, Color.White, Color.Black, LinearGradientMode.Horizontal);
                        bgBtn2 = new LinearGradientBrush(_rect2, Color.White, Color.Black, LinearGradientMode.Horizontal);
                    } else {
                        bgBar = new LinearGradientBrush(_rectBar, Color.White, Color.Black, LinearGradientMode.Vertical);
                        bgBtn1 = new LinearGradientBrush(_rect1, Color.White, Color.Black, LinearGradientMode.Vertical);
                        bgBtn2 = new LinearGradientBrush(_rect2, Color.White, Color.Black, LinearGradientMode.Vertical);
                    }
                    if (_pressed1) bgBtn1.InterpolationColors = Renderer.ScrollBar.barPressedBlend(_theme, _alpha);
                    else if (_hover1) bgBtn1.InterpolationColors = Renderer.ScrollBar.barHLitedBlend(_theme, _alpha);
                    else bgBtn1.InterpolationColors = Renderer.ScrollBar.barNormalBlend(_theme, _alpha);
                    if (_pressed2) bgBtn2.InterpolationColors = Renderer.ScrollBar.barPressedBlend(_theme, _alpha);
                    else if (_hover2) bgBtn2.InterpolationColors = Renderer.ScrollBar.barHLitedBlend(_theme, _alpha);
                    else bgBtn2.InterpolationColors = Renderer.ScrollBar.barNormalBlend(_theme, _alpha);
                    if (_pressedBar) bgBar.InterpolationColors = Renderer.ScrollBar.barPressedBlend(_theme, _alpha);
                    else if (_hoverBar) bgBar.InterpolationColors = Renderer.ScrollBar.barHLitedBlend(_theme, _alpha);
                    else bgBar.InterpolationColors = Renderer.ScrollBar.barNormalBlend(_theme, _alpha);
                    gpBar = Renderer.Drawing.roundedRectangle(_rectBar, 1, 1, 1, 1);
                    gpBtn1 = Renderer.Drawing.roundedRectangle(_rect1, 1, 1, 1, 1);
                    gpBtn2 = Renderer.Drawing.roundedRectangle(_rect2, 1, 1, 1, 1);
                    g.FillPath(bgBar, gpBar);
                    g.FillPath(bgBtn1, gpBtn1);
                    g.FillPath(bgBtn2, gpBtn2);
                    if (_pressed1) g.DrawPath(Renderer.ScrollBar.barPressedBorderPen(_theme, _alpha), gpBtn1);
                    else if (_hover1) g.DrawPath(Renderer.ScrollBar.barHLitedBorderPen(_theme, _alpha), gpBtn1);
                    else g.DrawPath(Renderer.ScrollBar.barNormalBorderPen(_theme, _alpha), gpBtn1);
                    if (_pressed2) g.DrawPath(Renderer.ScrollBar.barPressedBorderPen(_theme, _alpha), gpBtn2);
                    else if (_hover2) g.DrawPath(Renderer.ScrollBar.barHLitedBorderPen(_theme, _alpha), gpBtn2);
                    else g.DrawPath(Renderer.ScrollBar.barNormalBorderPen(_theme, _alpha), gpBtn2);
                    if (_pressedBar) g.DrawPath(Renderer.ScrollBar.barPressedBorderPen(_theme, _alpha), gpBar);
                    else if (_hoverBar) g.DrawPath(Renderer.ScrollBar.barHLitedBorderPen(_theme, _alpha), gpBar);
                    else g.DrawPath(Renderer.ScrollBar.barNormalBorderPen(_theme, _alpha), gpBar);
                } else {
                    if (_orientation == Orientation.Vertical) bgBar = new LinearGradientBrush(_rectBar, Color.White, Color.Black, LinearGradientMode.Horizontal);
                    else bgBar = new LinearGradientBrush(_rectBar, Color.White, Color.Black, LinearGradientMode.Vertical);
                    gpBar = Renderer.Drawing.roundedRectangle(_rectBar, 1, 1, 1, 1);
                    bgBar.InterpolationColors = Renderer.ScrollBar.barNormalBlend(_theme, _alpha);
                    g.FillPath(bgBar, gpBar);
                    g.DrawPath(Renderer.ScrollBar.barNormalBorderPen(_theme, _alpha), gpBar);
                }
                drawBars(g);
                if (bgBar != null) bgBar.Dispose();
                if (bgBtn1 != null) bgBtn1.Dispose();
                if (bgBtn2 != null) bgBtn2.Dispose();
                if (gpBar != null) gpBar.Dispose();
                if (gpBtn1 != null) gpBtn1.Dispose();
                if (gpBtn2 != null) gpBtn2.Dispose();
            }
            if (_orientation == Orientation.Vertical) {
                drawTriangle(_rect1, Ai.Renderer.Drawing.TriangleDirection.Up, g);
                drawTriangle(_rect2, Ai.Renderer.Drawing.TriangleDirection.Down, g);
            } else {
                drawTriangle(_rect1, Ai.Renderer.Drawing.TriangleDirection.Left, g);
                drawTriangle(_rect2, Ai.Renderer.Drawing.TriangleDirection.Right, g);
            }
            bgBrush.Dispose();
        }
        #endregion
        #endregion
    }
}