// Ai Software Control Library.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Control for selecting a color.
    /// Provides :
    /// 1. Predefinded colors and 17 history of last used colors, the last history will be shown first.
    /// 2. Creating a custom color from its components (A, RGB, or HSB).
    /// 3. Directly show ColorDialog window when the button(not the split) is clicked.
    /// 4. Automatically add the last selected color to the color histories.
    /// </summary>
    [DefaultEvent("SelectedColorChanged"), DefaultProperty("SelectedColor")]
    public class ColorChooser : System.Windows.Forms.Control {
        public class ColorHistories : CollectionBase {
            ColorChooser _owner;
            #region Public Events;
            public event EventHandler<EventArgs> AfterInsert;
            public event EventHandler<EventArgs> AfterRemove;
            public event EventHandler<EventArgs> AfterClear;
            public event EventHandler<EventArgs> AfterSet;
            #endregion
            public ColorHistories(ColorChooser owner) : base() { _owner = owner; }
            public System.Drawing.Color this[int index] {
                get {
                    if (index < 0 || index >= List.Count) return System.Drawing.Color.Black;
                    else return (System.Drawing.Color)List[index];
                }
            }
            public int IndexOf(System.Drawing.Color color) { return List.IndexOf(color); }
            public System.Drawing.Color Add(System.Drawing.Color color) {
                int index = IndexOf(color);
                if (index > -1) List.RemoveAt(index);
                index = List.Add(color);
                if (List.Count > 17) {
                    while (List.Count > 17) List.RemoveAt(0);
                    index = IndexOf(color);
                }
                return (System.Drawing.Color)List[index];
            }
            public System.Drawing.Color Add(int alpha, int red, int green, int blue) {
                if (alpha < 0 || alpha > 255 || red < 0 || red > 255 || green < 0 || green > 255 ||
                    blue < 0 || blue > 255) return System.Drawing.Color.Transparent;
                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(alpha, red, green, blue);
                return this.Add(newColor);
            }
            public System.Drawing.Color Add(int red, int green, int blue) {
                if (red < 0 || red > 255 || green < 0 || green > 255 ||
                    blue < 0 || blue > 255) return System.Drawing.Color.Transparent;
                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(red, green, blue);
                return this.Add(newColor);
            }
            public System.Drawing.Color Add(int alpha, int hue, float saturation, float brightness) {
                if (alpha < 0 || alpha > 255 || hue < 0 || hue > 360 ||
                    saturation < 0.0F || saturation > 1.0F ||
                    brightness < 0.0F || brightness > 1.0F) return System.Drawing.Color.Transparent;
                System.Drawing.Color newColor = Ai.Renderer.Drawing.colorFromAHSB(alpha, hue, saturation, brightness);
                return this.Add(newColor);
            }
            public System.Drawing.Color Add(int hue, float saturation, float brightness) {
                if (hue < 0 || hue > 360 ||
                    saturation < 0.0F || saturation > 1.0F ||
                    brightness < 0.0F || brightness > 1.0F) return System.Drawing.Color.Transparent;
                System.Drawing.Color newColor = Ai.Renderer.Drawing.colorFromAHSB(255, hue, saturation, brightness);
                return this.Add(newColor);
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(ColorHistories histories) {
                foreach (System.Drawing.Color clr in histories) this.Add(clr);
            }
            public void Insert(int index, System.Drawing.Color color) {
                if (IndexOf(color) == -1) List.Insert(index, color);
            }
            public void Remove(System.Drawing.Color color) {
                if (List.Contains(color)) List.Remove(color);
            }
            public bool Contains(System.Drawing.Color color) { return List.Contains(color); }
            protected override void OnValidate(object value) {
                if (!typeof(System.Drawing.Color).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must System.Drawing.Color", "value");
            }
            protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new EventArgs()); }
            protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null) AfterRemove(this, new EventArgs()); }
            protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new EventArgs()); }
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                if (AfterSet != null) AfterSet(this, new EventArgs());
            }
        }
        #region Privates
        /// <summary>
        /// Define a component of a color.
        /// </summary>
        private enum ComponentName { 
            Alpha = 0,
            Red = 1,
            Green = 2,
            Blue = 3,
            Hue = 4,
            Saturation = 5,
            Brightness = 6
        }
        private class ComponentSlider { 
            ComponentName _component = ComponentName.Alpha;
            float _value = 0f;
            Point _location = new Point(0, 0);
            int _width = 100;
            int _height = 20;
            bool _enabled = true;
            float _max = 255f;
            string _customFormat = "0";
            Rectangle _slideRect = new Rectangle(-5, 5, 10, 10);
            bool _hoverSlide = false;
            bool _interceptMouseDown = false;
            Point _boxPoint = new Point(0, 0);
            bool _changed = false;  // Determine whether the object need to repaint.
            Rectangle _rectColor = new Rectangle(0, 0, 100, 10);
            public event EventHandler<EventArgs> ValueChanged;
            public ComponentSlider() { }
            public ComponentName Component {
                get { return _component; }
                set {
                    if (_component != value) {
                        _component = value;
                        switch (_component) { 
                            case ComponentName.Alpha:
                            case ComponentName.Red:
                            case ComponentName.Green:
                            case ComponentName.Blue:
                                _max = 255f;
                                break;
                            case ComponentName.Hue:
                                _max = 360f;
                                break;
                            case ComponentName.Saturation:
                            case ComponentName.Brightness:
                                _max = 1f;
                                break;
                        }
                        _slideRect.X = _location.X + (int)((_width * _value / _max) - 5);
                        if (_value > _max) _value = _max;
                        _changed = true;
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
            public float Value {
                get { return _value; }
                set {
                    if (_value != value) {
                        if (_value >= 0f && _value <= _max) { 
                            _value = value;
                            _slideRect.X = _location.X + (int)((_width * _value / _max) - 5);
                            _changed = true;
                            ValueChanged(this, new EventArgs());
                        }
                    }
                }
            }
            public Point Location {
                get { return _location; }
                set {
                    if (_location != value) { 
                        _location = value;
                        _slideRect.X = _location.X + (int)((_width * _value / _max) - 5);
                        _slideRect.Y = _location.Y + 10;
                        _rectColor = new Rectangle(_location.X, _location.Y, _width, 10);
                        _changed = true;
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
            public bool Enabled {
                get { return _enabled; }
                set {
                    if (_enabled != value) {
                        _enabled = value;
                        if (!_enabled) { 
                            _hoverSlide = false;
                            _interceptMouseDown = false;
                            _boxPoint = new Point(0, 0);
                        }
                        _changed = true;
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
            public int Width {
                get { return _width; }
                set {
                    if (value > 20) { 
                        _width = value;
                        _slideRect.X = _location.X + (int)((_width * _value / _max) - 5);
                        _rectColor = new Rectangle(_location.X, _location.Y, _width, 10);
                        _changed = true;
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
            public bool Changed {
                get { return _changed; }
                set { _changed = value; }
            }
            public string CustomFormat {
                get { return _customFormat; }
                set { _customFormat = value; }
            }
            public int Right { get { return _location.X + _width + 5; } }
            public int Bottom { get { return _location.Y + _height; } }
            public int Height { get { return _height; } }
            public bool InterceptMouseDown { get { return _interceptMouseDown; } }
            public Rectangle Bounds { get { return new Rectangle(_location.X, _location.Y, _width, _height); } }
            private void drawSlider(Graphics g) {
                LinearGradientBrush sliderBrush = new LinearGradientBrush(_slideRect,
                    System.Drawing.Color.Black, System.Drawing.Color.White, LinearGradientMode.Vertical);
                Pen borderPen;
                Point[] tPoints = new Point[4];
                if (_enabled) {
                    if (_hoverSlide) { 
                        sliderBrush.InterpolationColors = Ai.Renderer.Button.HLitedBlend();
                        borderPen = Ai.Renderer.Button.HLitedBorderPen();
                    } else { 
                        sliderBrush.InterpolationColors = Ai.Renderer.Button.NormalBlend();
                        borderPen = Ai.Renderer.Button.NormalBorderPen();
                    }
                } else { 
                    sliderBrush.InterpolationColors = Ai.Renderer.Button.DisabledBlend();
                    borderPen = Ai.Renderer.Button.DisabledBorderPen();
                }
                tPoints[0] = new Point(_slideRect.X + 5, _slideRect.Y);
                tPoints[1] = new Point(_slideRect.X + 9, _slideRect.Y + 7);
                tPoints[2] = new Point(_slideRect.X, _slideRect.Y + 7);
                tPoints[3] = tPoints[0];
                g.FillPolygon(sliderBrush, tPoints);
                g.DrawPolygon(borderPen, tPoints);
                sliderBrush.Dispose();
                borderPen.Dispose();
            }
            public void draw(Graphics g, bool focused) {
                if (_enabled) {
                    System.Drawing.Color fColor, eColor;
                    switch (_component) { 
                        case ComponentName.Alpha:
                            fColor = System.Drawing.Color.Black;
                            eColor = System.Drawing.Color.White;
                            break;
                        case ComponentName.Blue:
                            fColor = Color.Black;
                            eColor = Color.FromArgb(0, 0, 255);
                            break;
                        case ComponentName.Red:
                            fColor = Color.Black;
                            eColor = Color.FromArgb(255, 0, 0);
                            break;
                        case ComponentName.Green:
                            fColor = Color.Black;
                            eColor = Color.FromArgb(0, 255, 0);
                            break;
                        case ComponentName.Brightness:
                            fColor = Ai.Renderer.Drawing.colorFromAHSB(255, 0, 0, 0);
                            eColor = Ai.Renderer.Drawing.colorFromAHSB(255, 0, 0, 1.0F);
                            break;
                        case ComponentName.Hue:
                            fColor = Ai.Renderer.Drawing.colorFromAHSB(255, 0, 0, 0);
                            eColor = Ai.Renderer.Drawing.colorFromAHSB(255, 360.0F, 0, 0);
                            break;
                        case ComponentName.Saturation:
                            fColor = Ai.Renderer.Drawing.colorFromAHSB(255, 0, 0, 0);
                            eColor = Ai.Renderer.Drawing.colorFromAHSB(255, 0, 1.0F, 0);
                            break;
                        default:
                            fColor = System.Drawing.Color.Black;
                            eColor = System.Drawing.Color.White;
                            break;
                    }
                    LinearGradientBrush bgBrush = new LinearGradientBrush(_rectColor, 
                        fColor, eColor, LinearGradientMode.Horizontal);
                    g.FillRectangle(bgBrush, _rectColor);
                    g.DrawRectangle(Pens.Black, _rectColor);
                    bgBrush.Dispose();
                } else { 
                    g.FillRectangle(Brushes.Gray, _rectColor);
                    g.DrawRectangle(Pens.DimGray, _rectColor);
                }
                drawSlider(g);
                if (focused) { 
                    Pen aPen;
                    Rectangle rectFocus = new Rectangle(_location.X, _location.Y, _width, _height);
                    aPen = new Pen(Color.Black, 1);
                    aPen.DashStyle = DashStyle.Dot;
                    aPen.DashOffset = 0.1F;
                    g.DrawRectangle(aPen, rectFocus);
                    aPen.Dispose();
                }
            }
            public void draw(Graphics g) { draw(g, false); }
            public void mouseMove(Point p) { 
                if (_enabled) {
                    if (!_interceptMouseDown) {
                        if (_slideRect.Contains(p)) {
                            if (!_hoverSlide) {
                                _hoverSlide = true;
                                _changed = true;
                            }
                        } else {
                            if (_rectColor.Contains(p)) _boxPoint = p;
                            else _boxPoint = new Point(0, 0);
                            if (_hoverSlide) {
                                _hoverSlide = false;
                                _changed = true;
                            }
                        }
                    } else {
                        float _newValue = (float)((p.X - _location.X) * _max / _width);
                        if (_newValue < 0) _newValue = 0f;
                        if (_newValue > _max) _newValue = _max;
                        Value = _newValue;
                    }
                }
            }
            public void mouseDown() {
                if (_enabled) {
                    if (_hoverSlide) {
                        _interceptMouseDown = true;
                    } else {
                        if (_boxPoint.X > 0 || _boxPoint.Y > 0) {
                            float _newValue = (float)((_boxPoint.X - _location.X) * _max / _width);
                            if (_newValue < 0) _newValue = 0f;
                            if (_newValue > _max) _newValue = _max;
                            Value = _newValue;
                        }
                    }
                }
            }
            public void mouseLeave() { 
                if (_enabled) {
                    _boxPoint = new Point(0, 0);
                    if (_hoverSlide) {
                        _hoverSlide = false;
                        _changed = true;
                    }
                }
            }
            public void mouseUp() { _interceptMouseDown = false; }
            public void increaseValue() {
                float newValue;
                switch (_component) { 
                    case ComponentName.Alpha:
                    case ComponentName.Red:
                    case ComponentName.Green:
                    case ComponentName.Blue:
                    case ComponentName.Hue:
                        newValue = _value + 1;
                        break;
                    default:
                        newValue = _value + 0.01F;
                        break;
                }
                if (newValue > _max) newValue = _max;
                Value = newValue;
            }
            public void decreaseValue() {
                float newValue;
                switch (_component) {
                    case ComponentName.Alpha:
                    case ComponentName.Red:
                    case ComponentName.Green:
                    case ComponentName.Blue:
                    case ComponentName.Hue:
                        newValue = _value - 1;
                        break;
                    default:
                        newValue = _value - 0.01F;
                        break;
                }
                if (newValue > _max) newValue = 0f;
                Value = newValue;
            }
            public override string ToString() { return _value.ToString(_customFormat, Ai.Renderer.Drawing.en_us_ci); }
        }
        /// <summary>
        /// Control to display popup window to select existing colors or create it from each components.
        /// </summary>
        private class ColorPopup : System.Windows.Forms.Control { 
            ColorChooser _owner;
            Rectangle _rectPredefined;
            Rectangle _rectComponent;
            int _hoverIndex = -1;
            int _selectedIndex = 0;
            PredefinedColorControl _predefined;
            ComponentColorControl _component;
            /// <summary>
            /// Control to display history and predefined colors.
            /// </summary>
            private class PredefinedColorControl : System.Windows.Forms.Control { 
                const int ColorMargin = 4;
                const int HostHeight = 20;
                ColorPopup _owner;
                List<ColorHost> _hosts = new List<ColorHost>();
                ColorHost _hoverHost = null;
                ColorHost _focusedHost = null;
                ColorHost _tooltipHost = null;
                ToolTip _colorTooltip;
                private class ColorHost { 
                    PredefinedColorControl _owner;
                    System.Drawing.Color _color = System.Drawing.Color.Black;
                    Rectangle _rect;
                    bool _selected = false;
                    public ColorHost(PredefinedColorControl owner) { _owner = owner; }
                    public Rectangle Rectangle {
                        get { return _rect; }
                        set { _rect = value; }
                    }
                    public bool Selected {
                        get { return _selected; }
                        set { _selected = value; }
                    }
                    public System.Drawing.Color Color {
                        get { return _color; }
                        set { _color = value; }
                    }
                    public string TooltipText {
                        get { return "Alpha : " + _color.A + '\n' + "R : " + _color.R + '\n' + "G : " + _color.G + '\n' + "B : " + _color.B; }
                    }
                    public void draw(Graphics g, bool focused, bool hLited) { 
                        if(_selected||hLited||focused)Ai.Renderer.Button.draw(g, _rect, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, _selected, hLited, focused);
                        System.Drawing.Rectangle rectColor = new System.Drawing.Rectangle(_rect.X + ColorMargin, 
                            _rect.Y + ColorMargin, (int)(_rect.Width - (2 * ColorMargin)), (int)(_rect.Height - (2 * ColorMargin)));
                        g.FillRectangle(new SolidBrush(_color), rectColor);
                        g.DrawRectangle(Pens.Black, rectColor);
                    }
                    public void draw(Graphics g, bool focused) { draw(g, focused, false); }
                    public void draw(Graphics g) { draw(g, false, false); }
                }
                public PredefinedColorControl(ColorPopup owner) {
                    this.KeyDown += PredefinedColorControl_KeyDown;
                    this.MouseDown += PredefinedColorControl_MouseDown;
                    this.MouseLeave += PredefinedColorControl_MouseLeave;
                    this.MouseMove += PredefinedColorControl_MouseMove;
                    this.Paint += PredefinedColorControl_Paint;
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    this.SetStyle(ControlStyles.ResizeRedraw, true);
                    this.SetStyle(ControlStyles.Selectable, true);
                    _owner = owner;
                    recreateColor();
                    base.Width = (7 * HostHeight) + 94;
                    base.Height = 55 + (17 * HostHeight);
                    this.SetStyle(ControlStyles.FixedHeight, true);
                    this.SetStyle(ControlStyles.FixedWidth, true);
                    _colorTooltip = new ToolTip(this);
                    _colorTooltip.AnimationSpeed = 20;
                    _colorTooltip.EnableAutoClose = false;
                    _colorTooltip.Draw += _colorTooltip_Draw;
                    _colorTooltip.Popup += _colorTooltip_Popup;
                }
                public void setSelectedColor() { 
                    recreateColor();
                    foreach (ColorHost ch in _hosts) {
                        if (ch.Color == _owner._owner._selectedColor) ch.Selected = true;
                        else ch.Selected = false;
                    }
                    this.Invalidate();
                }
                public void fireKeyDown(System.Windows.Forms.KeyEventArgs e) {
                    PredefinedColorControl_KeyDown(this, e);
                }
                private void recreateColor() {
                    // Setting up hosts for 17 maximum colors history.
                    int i, j;
                    int x, y;
                    ColorHost aHost;
                    _hosts.Clear();
                    i = 0;
                    x = 3;
                    y = 22;
                    while (i < 17 && i < _owner._owner._mruColors.Count) {
                        aHost = new ColorHost(this);
                        aHost.Color = _owner._owner._mruColors[_owner._owner._mruColors.Count - (i + 1)];
                        aHost.Rectangle = new Rectangle(x, y, 70, HostHeight);
                        _hosts.Add(aHost);
                        i = i + 1;
                        y = y + HostHeight + 2;
                    }
                    // Setting up predefined colors.
                    aHost = new ColorHost(this);
                    aHost.Color = System.Drawing.Color.Black;
                    aHost.Rectangle = new Rectangle(75, 22, (7 * HostHeight) + 12, HostHeight);
                    _hosts.Add(aHost);
                    x = 75;
                    j = 0;
                    while (j < 7) {
                        y = 22 + HostHeight + 2;
                        i = 15;
                        while (i < 256) {
                            aHost = new ColorHost(this);
                            aHost.Rectangle = new Rectangle(x, y, HostHeight, HostHeight);
                            switch (j) {
                                case 0:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, i, 0, 0);
                                    break;
                                case 1:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, 0, i, 0);
                                    break;
                                case 2:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, 0, 0, i);
                                    break;
                                case 3:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, 0, i, i);
                                    break;
                                case 4:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, i, 0, i);
                                    break;
                                case 5:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, i, i, 0);
                                    break;
                                case 6:
                                    aHost.Color = System.Drawing.Color.FromArgb(255, i, i, i);
                                    break;
                            }
                            _hosts.Add(aHost);
                            i = i + 16;
                            y = y + HostHeight + 2;
                        }
                        j++;
                        x = x + HostHeight + 2;
                    }
                }
                private void PredefinedColorControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
                    bool changed = false;
                    switch (e.KeyData) {
                        case Keys.Up:
                            if (_focusedHost == null) {
                                _focusedHost = _hosts[_hosts.Count - 1];
                            } else {
                                int focusedIndex = _hosts.IndexOf(_focusedHost);
                                if (focusedIndex == 0) _focusedHost = _hosts[_hosts.Count - 1];
                                else _focusedHost = _hosts[focusedIndex - 1];
                            }
                            _tooltipHost = _focusedHost;
                            _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                            changed = true;
                            break;
                        case Keys.Down:
                            if (_focusedHost == null) {
                                _focusedHost = _hosts[0];
                            } else {
                                int focusedIndex = _hosts.IndexOf(_focusedHost);
                                if (focusedIndex == _hosts.Count - 1) _focusedHost = _hosts[0];
                                else _focusedHost = _hosts[focusedIndex + 1];
                            }
                            _tooltipHost = _focusedHost;
                            _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                            changed = true;
                            break;
                        case Keys.Left:
                            if (_focusedHost != null) {
                                bool find = false;
                                int i = _hosts.IndexOf(_focusedHost) - 1;
                                while (i >= 0 && !find) {
                                    if (_hosts[i].Rectangle.Y == _focusedHost.Rectangle.Y) find = true;
                                    else i = i - 1;
                                }
                                if (find) {
                                    _focusedHost = _hosts[i];
                                    _tooltipHost = _focusedHost;
                                    _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                                    changed = true;
                                }
                            } else {
                                _focusedHost = _hosts[_hosts.Count - 1];
                                _tooltipHost = _focusedHost;
                                _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                                changed = true;
                            }
                            break;
                        case Keys.Right:
                            if (_focusedHost != null) {
                                bool find = false;
                                int i = _hosts.IndexOf(_focusedHost) + 1;
                                while (i < _hosts.Count && !find) {
                                    if (_hosts[i].Rectangle.Y == _focusedHost.Rectangle.Y) find = true;
                                    else i++;
                                }
                                if (find) {
                                    _focusedHost = _hosts[i];
                                    _tooltipHost = _focusedHost;
                                    _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                                    changed = true;
                                }
                            } else {
                                _focusedHost = _hosts[0];
                                _tooltipHost = _focusedHost;
                                _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                                changed = true;
                            }
                            break;
                        case Keys.Return:
                            _colorTooltip.hide();
                            _owner._owner._mustRaiseEvent = _owner._owner._selectedColor != _focusedHost.Color;
                            _owner._owner._selectedColor = _focusedHost.Color;
                            // Close popup.
                            _owner._owner._tdown.Hide();
                            break;
                        case Keys.Tab:
                            _colorTooltip.hide();
                            // Switch to component control.
                            _owner.switchControl();
                            break;
                        case Keys.Escape:
                            _colorTooltip.hide();
                            // Close popup.
                            _owner._owner._tdown.Hide();
                            break;
                    }
                    if (changed) this.Invalidate();
                }
                private void PredefinedColorControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                        if (_hoverHost != null) {
                            _colorTooltip.hide();
                            // Highlited color is selected and popup closed.
                            _owner._owner._mustRaiseEvent = _owner._owner._selectedColor != _hoverHost.Color;
                            _owner._owner._selectedColor = _hoverHost.Color;
                            // Close popup.
                            _owner._owner._tdown.Hide();
                        }
                    }
                }
                private void PredefinedColorControl_MouseLeave(object sender, System.EventArgs e) {
                    if (_tooltipHost == _hoverHost) {
                        _tooltipHost = null;
                        _colorTooltip.hide();
                    }
                    if (_hoverHost != null) {
                        _hoverHost = null;
                        this.Invalidate();
                    }
                }
                private void PredefinedColorControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
                    bool find = false;
                    bool changed = false;
                    int i = 0;
                    while (i < _hosts.Count && !find) {
                        if (_hosts[i].Rectangle.Contains(e.Location)) {
                            if (_hoverHost != _hosts[i]) {
                                _hoverHost = _hosts[i];
                                _tooltipHost = _hoverHost;
                                _colorTooltip.show(this, new Rectangle(-Left, -Top, _owner.Width, _owner.Height));
                                changed = true;
                            }
                            find = true;
                        }
                        i++;
                    }
                    if (!find) {
                        if (_hoverHost != null) {
                            _colorTooltip.hide();
                            _hoverHost = null;
                            changed = true;
                        }
                    }
                    if (changed) this.Invalidate();
                }
                private void PredefinedColorControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.Clear(System.Drawing.Color.FromArgb(250, 250, 250));
                    e.Graphics.DrawString("MRU", Ai.Renderer.ToolTip.TextFont, Brushes.Black, 3, 5);
                    e.Graphics.DrawString("Predefined", Ai.Renderer.ToolTip.TextFont, Brushes.Black, 75, 5);
                    foreach (ColorHost ch in _hosts) ch.draw(e.Graphics, ch == _focusedHost, ch == _hoverHost);
                    e.Graphics.DrawRectangle(Ai.Renderer.Button.NormalBorderPen(), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }
                private void _colorTooltip_Draw(object sender, DrawEventArgs e) {
                    if (_tooltipHost != null) Ai.Renderer.ToolTip.drawToolTip(e.Graphics, new Rectangle((int)e.Rectangle.X, (int)e.Rectangle.Y, (int)e.Rectangle.Width, (int)e.Rectangle.Height), "", _tooltipHost.TooltipText, null);
                }
                private void _colorTooltip_Popup(object sender, PopupEventArgs e) {
                    if (_tooltipHost != null) e.Size = Ai.Renderer.ToolTip.measureSize("", _tooltipHost.TooltipText, null);
                }
            }
            /// <summary>
            /// Control to create custom color based on its components (A, RGB, HSB).
            /// </summary>
            private class ComponentColorControl : System.Windows.Forms.Control { 
                private const int sliderLeft = 25;
                private const int sliderWidth = 150;
                ColorPopup _owner;
                List<ComponentSlider> _sliders;
                Rectangle _buttonRectangle;
                Rectangle _pickerRectangle;
                Rectangle _pickerPreview;
                bool _hoverPicker = false;
                bool _pickerFocused = false;
                bool _hoverButton = false;
                bool _onPicking = false;
                System.Drawing.Color _currentColor = System.Drawing.Color.Black;
                bool _suspendEvent = false;
                ComponentSlider _focusedSlider = null;
                Point _lastPoint;
                Color _pickedColor;
                Timer tmrCapture = new Timer();
                public ComponentColorControl(ColorPopup owner) {
                    this.KeyDown += ComponentColorControl_KeyDown;
                    this.MouseDown += ComponentColorControl_MouseDown;
                    this.MouseLeave += ComponentColorControl_MouseLeave;
                    this.MouseMove += ComponentColorControl_MouseMove;
                    this.MouseUp += ComponentColorControl_MouseUp;
                    this.Paint += ComponentColorControl_Paint;
                    this.VisibleChanged += ComponentColorControl_VisibleChanged;
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    this.SetStyle(ControlStyles.ResizeRedraw, true);
                    this.SetStyle(ControlStyles.Selectable, true);
                    _owner = owner;
                    _sliders = new List<ComponentSlider>();
                    // Setting up sliders
                    ComponentSlider aSlider;
                    int lastY = 10;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Alpha;
                    aSlider.CustomFormat = "0";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 45;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Red;
                    aSlider.CustomFormat = "0";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 25;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Green;
                    aSlider.CustomFormat = "0";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 25;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Blue;
                    aSlider.CustomFormat = "0";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 45;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Hue;
                    aSlider.CustomFormat = "0.00";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 25;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Saturation;
                    aSlider.CustomFormat = "0.00";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 25;
                    aSlider = new ComponentSlider();
                    aSlider.Component = ComponentName.Brightness;
                    aSlider.CustomFormat = "0.00";
                    aSlider.Width = sliderWidth;
                    aSlider.Location = new Point(sliderLeft, lastY);
                    _sliders.Add(aSlider);
                    lastY = lastY + 25;
                    _pickerRectangle = new Rectangle(3, lastY, 228, 30);
                    lastY = lastY + 35;
                    _pickerPreview = new Rectangle(3, lastY, 228, 30);
                    lastY = lastY + 47;
                    _buttonRectangle = new Rectangle(sliderLeft, lastY, sliderWidth, 22);
                    // Setting up ValueChanged event handler for ComponentSliders
                    foreach (ComponentSlider cs in _sliders) cs.ValueChanged += slider_ValueChanged;
                    base.Width = 234;
                    base.Height = 55 + (17 * 20);
                    this.SetStyle(ControlStyles.FixedHeight, true);
                    this.SetStyle(ControlStyles.FixedWidth, true);
                    tmrCapture.Enabled = false;
                    tmrCapture.Tick += tmrCapture_Tick;
                }
                public void setSelectedColor() { 
                    _suspendEvent = true;
                    _currentColor = _owner._owner._selectedColor;
                    foreach (ComponentSlider cs in _sliders) {
                        switch (cs.Component) {
                            case ComponentName.Alpha:
                                cs.Value = _owner._owner._selectedColor.A;
                                cs.Enabled = _owner._owner._alphaEnabled;
                                break;
                            case ComponentName.Red:
                                cs.Value = _owner._owner._selectedColor.R;
                                break;
                            case ComponentName.Green:
                                cs.Value = _owner._owner._selectedColor.G;
                                break;
                            case ComponentName.Blue:
                                cs.Value = _owner._owner._selectedColor.B;
                                break;
                            case ComponentName.Hue:
                                cs.Value = _owner._owner._selectedColor.GetHue();
                                break;
                            case ComponentName.Saturation:
                                cs.Value = _owner._owner._selectedColor.GetSaturation();
                                break;
                            case ComponentName.Brightness:
                                cs.Value = _owner._owner._selectedColor.GetBrightness();
                                break;
                        }
                    }
                    _pickerFocused = false;
                    _onPicking = false;
                    _focusedSlider = null;
                    this.Invalidate();
                    _suspendEvent = false;
                }
                public void fireKeyDown(System.Windows.Forms.KeyEventArgs e) { ComponentColorControl_KeyDown(this, e); }
                private void slider_ValueChanged(object sender, EventArgs e) {
                    if (!_suspendEvent) {
                        _suspendEvent = true;   // Avoiding more events fired.
                        ComponentSlider slider = (ComponentSlider)sender;
                        // Change current color
                        switch (slider.Component) {
                            case ComponentName.Alpha:
                                _currentColor = System.Drawing.Color.FromArgb((int)slider.Value, _currentColor.R, _currentColor.G, _currentColor.B);
                                break;
                            case ComponentName.Red:
                                _currentColor = System.Drawing.Color.FromArgb(_currentColor.A, (int)slider.Value, _currentColor.G, _currentColor.B);
                                break;
                            case ComponentName.Green:
                                _currentColor = System.Drawing.Color.FromArgb(_currentColor.A, _currentColor.R, (int)slider.Value, _currentColor.B);
                                break;
                            case ComponentName.Blue:
                                _currentColor = System.Drawing.Color.FromArgb(_currentColor.A, _currentColor.R, _currentColor.G, (int)slider.Value);
                                break;
                            case ComponentName.Hue:
                                _currentColor = Ai.Renderer.Drawing.colorFromAHSB(_currentColor.A, slider.Value, 
                                    _currentColor.GetSaturation(), _currentColor.GetBrightness());
                                break;
                            case ComponentName.Saturation:
                                _currentColor = Ai.Renderer.Drawing.colorFromAHSB(_currentColor.A, _currentColor.GetHue(), 
                                    slider.Value, _currentColor.GetBrightness());
                                break;
                            case ComponentName.Brightness:
                                _currentColor = Ai.Renderer.Drawing.colorFromAHSB(_currentColor.A, _currentColor.GetHue(), 
                                    _currentColor.GetSaturation(), slider.Value);
                                break;
                        }
                        // Change the other slider's value
                        switch (slider.Component) {
                            case ComponentName.Red:
                            case ComponentName.Green:
                            case ComponentName.Blue:
                                foreach (ComponentSlider cs in _sliders) {
                                    switch (cs.Component) {
                                        case ComponentName.Hue:
                                            cs.Value = _currentColor.GetHue();
                                            break;
                                        case ComponentName.Saturation:
                                            cs.Value = _currentColor.GetSaturation();
                                            break;
                                        case ComponentName.Brightness:
                                            cs.Value = _currentColor.GetBrightness();
                                            break;
                                    }
                                }
                                break;
                            case ComponentName.Hue:
                            case ComponentName.Saturation:
                            case ComponentName.Brightness:
                                foreach (ComponentSlider cs in _sliders) {
                                    switch (cs.Component) {
                                        case ComponentName.Red:
                                            cs.Value = _currentColor.R;
                                            break;
                                        case ComponentName.Green:
                                            cs.Value = _currentColor.G;
                                            break;
                                        case ComponentName.Blue:
                                            cs.Value = _currentColor.B;
                                            break;
                                    }
                                }
                                break;
                        }
                        _suspendEvent = false;
                        this.Invalidate();
                    }
                }
                private void ComponentColorControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
                    if (!_onPicking) {
                        switch (e.KeyData) {
                            case Keys.Down:
                                int nextIndex;
                                _pickerFocused = false;
                                if (_focusedSlider == null) nextIndex = 0;
                                else nextIndex = _sliders.IndexOf(_focusedSlider) + 1;
                                if (nextIndex < _sliders.Count) {
                                    if (!_sliders[nextIndex].Enabled) nextIndex = nextIndex + 1;
                                }
                                if (nextIndex < _sliders.Count) {
                                    _focusedSlider = _sliders[nextIndex];
                                    _pickerFocused = false;
                                } else {
                                    _pickerFocused = true;
                                    _focusedSlider = null;
                                }
                                this.Invalidate();
                                break;
                            case Keys.Up:
                                if (_focusedSlider == null && !_pickerFocused) {
                                    _pickerFocused = true;
                                } else {
                                    if (_pickerFocused) {
                                        _focusedSlider = _sliders[_sliders.Count - 1];
                                        _pickerFocused = false;
                                    } else {
                                        int prevIndex = _sliders.IndexOf(_focusedSlider) - 1;
                                        if (prevIndex < 0) {
                                            _pickerFocused = true;
                                            _focusedSlider = null;
                                        } else {
                                            if (prevIndex == 0 && !_sliders[0].Enabled) {
                                                _pickerFocused = true;
                                                _focusedSlider = null;
                                            } else {
                                                _focusedSlider = _sliders[prevIndex];
                                                _pickerFocused = false;
                                            }
                                        }
                                    }
                                }
                                this.Invalidate();
                                break;
                            case Keys.Space:
                                if (_pickerFocused) {
                                    _onPicking = true;
                                    tmrCapture.Enabled = true;
                                    this.Invalidate();
                                }
                                break;
                            case Keys.Left:
                                if (_focusedSlider != null) _focusedSlider.decreaseValue();
                                break;
                            case Keys.Right:
                                if (_focusedSlider != null) _focusedSlider.increaseValue();
                                break;
                            case Keys.Return:
                                _owner._owner._mustRaiseEvent = _owner._owner._selectedColor != _currentColor;
                                _owner._owner._selectedColor = _currentColor;
                                // Close popup.
                                _owner._owner._tdown.Hide();
                                break;
                            case Keys.Tab:
                                // Switch to predefined control.
                                _owner.switchControl();
                                break;
                            case Keys.Escape:
                                // Close popup.
                                _owner._owner._tdown.Hide();
                                break;
                        }
                    } else {
                        switch (e.KeyData) {
                            case Keys.Space:
                                _onPicking = false;
                                tmrCapture.Enabled = false;
                                this.Invalidate();
                                break;
                            case Keys.Return:
                                _suspendEvent = true;
                                _currentColor = _pickedColor;
                                foreach (ComponentSlider cs in _sliders) {
                                    switch (cs.Component) {
                                        case ComponentName.Red:
                                            cs.Value = _currentColor.R;
                                            break;
                                        case ComponentName.Green:
                                            cs.Value = _currentColor.G;
                                            break;
                                        case ComponentName.Blue:
                                            cs.Value = _currentColor.B;
                                            break;
                                        case ComponentName.Hue:
                                            cs.Value = _currentColor.GetHue();
                                            break;
                                        case ComponentName.Saturation:
                                            cs.Value = _currentColor.GetSaturation();
                                            break;
                                        case ComponentName.Brightness:
                                            cs.Value = _currentColor.GetBrightness();
                                            break;
                                    }
                                }
                                this.Invalidate();
                                _suspendEvent = false;
                                break;
                        }
                    }
                }
                private void ComponentColorControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
                    if (!_onPicking) {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                            if (_hoverButton) {
                                _owner._owner._mustRaiseEvent = _owner._owner._selectedColor != _currentColor;
                                _owner._owner._selectedColor = _currentColor;
                                // Close popup.
                                _owner._owner._tdown.Hide();
                            } else {
                                if (_hoverPicker) {
                                    _focusedSlider = null;
                                    _pickerFocused = true;
                                    _onPicking = true;
                                    tmrCapture.Enabled = true;
                                } else {
                                    Point pCursor = System.Windows.Forms.Control.MousePosition;
                                    bool changed = false;
                                    pCursor = this.PointToClient(pCursor);
                                    foreach (ComponentSlider cs in _sliders) {
                                        cs.mouseDown();
                                        if (cs.Enabled) {
                                            if (cs.Bounds.Contains(pCursor)) {
                                                if (_focusedSlider != cs) {
                                                    _focusedSlider = cs;
                                                    changed = true;
                                                }
                                            }
                                        }
                                        changed = changed || cs.Changed;
                                    }
                                    if (changed) this.Invalidate();
                                }
                            }
                        }
                    }
                }
                private void ComponentColorControl_MouseLeave(object sender, System.EventArgs e) {
                    bool changed = false;
                    foreach (ComponentSlider cs in _sliders) {
                        cs.mouseLeave();
                        changed = changed || cs.Changed;
                    }
                    if (_hoverButton) {
                        _hoverButton = false;
                        changed = true;
                    }
                    if (_hoverPicker) {
                        _hoverPicker = false;
                        changed = true;
                    }
                    if (changed) this.Invalidate();
                }
                private void ComponentColorControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
                    if (!_onPicking) {
                        bool changed = false;
                        foreach (ComponentSlider cs in _sliders) {
                            cs.mouseMove(e.Location);
                            changed = changed || cs.Changed;
                        }
                        bool mouseIntercepted = false;
                        foreach (ComponentSlider cs in _sliders) mouseIntercepted = mouseIntercepted || cs.InterceptMouseDown;
                        if (_buttonRectangle.Contains(e.Location)) {
                            if (!mouseIntercepted) {
                                if (!_hoverButton) {
                                    _hoverButton = true;
                                    changed = true;
                                }
                            }
                        } else {
                            if (_hoverButton) {
                                _hoverButton = false;
                                changed = true;
                            }
                        }
                        if (_pickerRectangle.Contains(e.Location)) {
                            if (!mouseIntercepted) {
                                if (!_hoverPicker) {
                                    _hoverPicker = true;
                                    changed = true;
                                }
                            }
                        } else {
                            if (_hoverPicker) {
                                _hoverPicker = false;
                                changed = true;
                            }
                        }
                        if (changed) this.Invalidate();
                    }
                }
                private void ComponentColorControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
                    bool changed = false;
                    foreach (ComponentSlider cs in _sliders) {
                        cs.mouseUp();
                        changed = changed || cs.Changed;
                    }
                    if (changed) this.Invalidate();
                }
                private void ComponentColorControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
                    StringFormat nameFormat = new StringFormat();
                    StringFormat valueFormat = new StringFormat();
                    Rectangle valueRect, nameRect;
                    int i;
                    Rectangle rectColor = new Rectangle(_buttonRectangle.X + 4, _buttonRectangle.Y + 4, _buttonRectangle.Width - 8, _buttonRectangle.Height - 8);
                    nameRect = new Rectangle(3, 0, 20, 30);
                    valueRect = new Rectangle(172, 0, 45, 30);
                    nameFormat.Alignment = StringAlignment.Near;
                    valueFormat.Alignment = StringAlignment.Far;
                    nameFormat.LineAlignment = StringAlignment.Center;
                    valueFormat.LineAlignment = StringAlignment.Center;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.Clear(Color.FromArgb(250, 250, 250));
                    i = 0;
                    while (i < _sliders.Count) {
                        nameRect.Y = _sliders[i].Location.Y - 7;
                        valueRect.Y = _sliders[i].Location.Y - 7;
                        e.Graphics.DrawString(_sliders[i].Component.ToString().Substring(0, 1), 
                            Ai.Renderer.ToolTip.TextFont, Brushes.Black, nameRect, nameFormat);
                        e.Graphics.DrawString(_sliders[i].ToString(), 
                            Ai.Renderer.ToolTip.TextFont, Brushes.Black, valueRect, valueFormat);
                        _sliders[i].draw(e.Graphics, _sliders[i] == _focusedSlider);
                        if (i == 0) {
                            Rectangle groupRect = new Rectangle(3, _sliders[i].Bottom, this.Width - 6, 25);
                            e.Graphics.DrawString("RGB Components", Ai.Renderer.ToolTip.TextFont, 
                                Brushes.Black, groupRect, nameFormat);
                        } else if (i == 3) {
                            Rectangle groupRect = new Rectangle(3, _sliders[i].Bottom, this.Width - 6, 25);
                            e.Graphics.DrawString("HSB Components", Ai.Renderer.ToolTip.TextFont, 
                                Brushes.Black, groupRect, nameFormat);
                        }
                        _sliders[i].Changed = false;
                        i++;
                    }
                    // Checkbox for color picker
                    Rectangle rectChk = new Rectangle(_pickerRectangle.X, _pickerRectangle.Y, 
                        20, _pickerRectangle.Height);
                    Rectangle rectText = new Rectangle(_pickerRectangle.X + 21, _pickerRectangle.Y, 
                        _pickerRectangle.Width - 25, _pickerRectangle.Height);
                    Ai.Renderer.CheckBox.drawCheckBox(e.Graphics, rectChk, 
                        _onPicking ? CheckState.Checked : CheckState.Unchecked, 14, true, _hoverPicker);
                    if (!_onPicking) e.Graphics.DrawString("Pick color on screen.", 
                            Ai.Renderer.ToolTip.TextFont, Brushes.Black, rectText, nameFormat);
                    else e.Graphics.DrawString("Press enter to select current color, space to disable.", 
                            Ai.Renderer.ToolTip.TextFont, Brushes.Black, rectText, nameFormat);
                    if (_pickerFocused) {
                        Pen aPen;
                        aPen = new Pen(Color.Black, 1);
                        aPen.DashStyle = DashStyle.Dot;
                        aPen.DashOffset = 0.1F;
                        e.Graphics.DrawRectangle(aPen, rectText);
                        aPen.Dispose();
                    }
                    if (_onPicking) {
                        // Picking a color from screen.
                        Point pCursor = System.Windows.Forms.Control.MousePosition;
                        Point previewLocation = new Point(_pickerPreview.X + 50, _pickerPreview.Y);
                        Rectangle colorRect = new Rectangle(_pickerPreview.X + 85, _pickerPreview.Y, 30, 30);
                        Bitmap aBmp = new Bitmap(30, 30);
                        Graphics gBmp = Graphics.FromImage(aBmp);
                        gBmp.CopyFromScreen(pCursor.X - 15, pCursor.Y - 15, 0, 0, new Size(30, 30));
                        gBmp.Dispose();
                        e.Graphics.DrawImage(aBmp, previewLocation);
                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(previewLocation.X, previewLocation.Y, 30, 30));
                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(previewLocation.X + 14, previewLocation.Y + 14, 4, 4));
                        e.Graphics.FillRectangle(new SolidBrush(aBmp.GetPixel(15, 15)), colorRect);
                        _pickedColor = aBmp.GetPixel(15, 15);
                        e.Graphics.DrawRectangle(Pens.Black, colorRect);
                        aBmp.Dispose();
                        e.Graphics.DrawString("Preview :", Ai.Renderer.ToolTip.TextFont, Brushes.Black, 
                            _pickerPreview, nameFormat);
                        colorRect.X = colorRect.Right + 5;
                        colorRect.Width = this.Width - (colorRect.Left + 5);
                        colorRect.Height = 40;
                        e.Graphics.DrawString("R : " + _pickedColor.R + '\n' + 
                            "G : " + _pickedColor.G + '\n' + "B : " + _pickedColor.B, 
                            Ai.Renderer.ToolTip.TextFont, Brushes.Black, colorRect, nameFormat);
                        // End picking
                    }
                    Ai.Renderer.Button.draw(e.Graphics, _buttonRectangle, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, false, _hoverButton);
                    e.Graphics.FillRectangle(new SolidBrush(_currentColor), rectColor);
                    e.Graphics.DrawRectangle(Pens.Black, rectColor);
                    e.Graphics.DrawRectangle(Ai.Renderer.Button.NormalBorderPen(), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                    nameFormat.Dispose();
                    valueFormat.Dispose();
                }
                private void ComponentColorControl_VisibleChanged(object sender, System.EventArgs e) {
                    if (this.Visible) {
                        if (_onPicking) {
                            _lastPoint = System.Windows.Forms.Control.MousePosition;
                            tmrCapture.Enabled = true;
                        }
                    } else {
                        _lastPoint = new Point(0, 0);
                        tmrCapture.Enabled = false;
                    }
                }
                private void tmrCapture_Tick(object sender, System.EventArgs e) {
                    Point currentPoint = System.Windows.Forms.Control.MousePosition;
                    if (currentPoint != _lastPoint) {
                        _lastPoint = currentPoint;
                        this.Invalidate();
                    }
                }
            }
            public ColorPopup(ColorChooser owner) {
                this.KeyDown += ColorPopup_KeyDown;
                this.MouseDown += ColorPopup_MouseDown;
                this.MouseLeave += ColorPopup_MouseLeave;
                this.MouseMove += ColorPopup_MouseMove;
                this.Paint += ColorPopup_Paint;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                this.SetStyle(ControlStyles.Selectable, true);
                _owner = owner;
                _rectPredefined = new Rectangle(3, 3, 100, 22);
                _rectComponent = new Rectangle(105, 3, 100, 22);
                _predefined = new PredefinedColorControl(this);
                _component = new ComponentColorControl(this);
                _predefined.Left = 3;
                _predefined.Top = 27;
                _component.Left = 3;
                _component.Top = 27;
                _component.Visible = false;
                this.Controls.Add(_predefined);
                this.Controls.Add(_component);
                this.Width = 243;
                this.Height = 425;
                this.SetStyle(ControlStyles.FixedHeight, true);
                this.SetStyle(ControlStyles.FixedWidth, true);
            }
            protected override bool IsInputKey(System.Windows.Forms.Keys keyData) {
                switch (keyData) {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Left:
                    case Keys.Return:
                    case Keys.Tab:
                    case Keys.Escape:
                    case Keys.Space:
                        return true;
                    default:
                        return base.IsInputKey(keyData);
                }
            }
            public void setSelectedColor() {
                _predefined.setSelectedColor();
                _component.setSelectedColor();
            }
            private void switchControl() {
                if (_selectedIndex == 0) {
                    _selectedIndex = 1;
                    _predefined.Visible = false;
                    _component.Visible = true;
                } else {
                    _selectedIndex = 0;
                    _predefined.Visible = true;
                    _component.Visible = false;
                }
                this.Invalidate();
            }
            private void ColorPopup_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
                if (_selectedIndex == 0) _predefined.fireKeyDown(e);
                else _component.fireKeyDown(e);
            }
            private void ColorPopup_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    if (_hoverIndex != _selectedIndex) switchControl();
                }
            }
            private void ColorPopup_MouseLeave(object sender, System.EventArgs e) {
                if (_hoverIndex != -1) {
                    _hoverIndex = -1;
                    this.Invalidate();
                }
            }
            private void ColorPopup_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
                bool changed = false;
                if (_rectPredefined.Contains(e.Location)) {
                    if (_hoverIndex != 0) {
                        _hoverIndex = 0;
                        changed = true;
                    }
                } else {
                    if (_rectComponent.Contains(e.Location)) {
                        if (_hoverIndex != 1) {
                            _hoverIndex = 1;
                            changed = true;
                        }
                    } else {
                        if (_hoverIndex != -1) {
                            _hoverIndex = -1;
                            changed = true;
                        }
                    }
                }
                if (changed) this.Invalidate();
            }
            private void ColorPopup_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.Clear(this.BackColor);
                if (_selectedIndex == 0 || _hoverIndex == 0) Ai.Renderer.Button.draw(e.Graphics, _rectPredefined, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, _selectedIndex == 0, _hoverIndex == 0);
                if (_selectedIndex == 1 || _hoverIndex == 1) Ai.Renderer.Button.draw(e.Graphics, _rectComponent, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, _selectedIndex == 1, _hoverIndex == 1);
                e.Graphics.DrawString("Select", Ai.Renderer.ToolTip.TextFont, Brushes.Black, _rectPredefined, strFormat);
                e.Graphics.DrawString("Create", Ai.Renderer.ToolTip.TextFont, Brushes.Black, _rectComponent, strFormat);
                strFormat.Dispose();
            }
        }
        #endregion
        #region Declarations
        ToolStripDropDown _tdown;
        ColorHistories _mruColors;
        System.Drawing.Color _selectedColor = System.Drawing.Color.Black;
        bool _mustRaiseEvent = false;
        bool _alphaEnabled = true;
        Rectangle _rectSub;
        bool _hoverSub = false;
        bool _onMouse = false;
        bool _pressed = false;
        bool _resumePainting = true;
        ColorPopup cPopup;
        Ai.Renderer.Drawing.ColorTheme _theme = Ai.Renderer.Drawing.ColorTheme.Blue;
        #endregion
        public event EventHandler<EventArgs> SelectedColorChanged;
        private void raiseMyEvent() { 
            _mustRaiseEvent = false;
            _mruColors.Add(_selectedColor);
            if (SelectedColorChanged != null) SelectedColorChanged(this, new EventArgs());
        }
        public ColorChooser() : base() {
            // Attaching event handlers
            this.EnabledChanged += ColorChooser_EnabledChanged;
            this.GotFocus += ColorChooser_GotFocus;
            this.KeyDown += ColorChooser_KeyDown;
            this.LostFocus += ColorChooser_LostFocus;
            this.MouseDown += ColorChooser_MouseDown;
            this.MouseEnter += ColorChooser_MouseEnter;
            this.MouseLeave += ColorChooser_MouseLeave;
            this.MouseMove += ColorChooser_MouseMove;
            this.MouseUp += ColorChooser_MouseUp;
            this.Paint += ColorShooser_Paint;
            this.Resize += ColorChooser_Resize;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            _mruColors = new ColorHistories(this);
            ToolStripControlHost pHost;
            _tdown = new ToolStripDropDown(this);
            _rectSub = new Rectangle(this.Width - 12, 0, 12, this.Height);
            cPopup = new ColorPopup(this);
            pHost = new ToolStripControlHost(cPopup);
            _tdown.Items.Clear();
            _tdown.Items.Add(pHost);
            _tdown.Closed += _tdown_Closed;
        }
        /// <summary>
        /// Gets the collection of System.Drawing.Color objects assigned to the current tree node.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of System.Drawing.Color objects assigned to the current tree node.")]
        public ColorHistories Histories { get { return _mruColors; } }
        [DefaultValue(true)]
        public bool AlphaEnabled {
            get { return _alphaEnabled; }
            set { _alphaEnabled = value; }
        }
        [Browsable(false)]
        public string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }
        [DefaultValue(typeof(System.Drawing.Color), "Black")]
        public System.Drawing.Color SelectedColor {
            get { return _selectedColor; }
            set {
                if (_selectedColor != value) { 
                    _selectedColor = value;
                    _mruColors.Add(_selectedColor);
                    if (_resumePainting) this.Invalidate();
                    if (SelectedColorChanged != null) SelectedColorChanged(this, new EventArgs());
                }
            }
        }
        [DefaultValue(typeof(Ai.Renderer.Drawing.ColorTheme), "Blue")]
        public Ai.Renderer.Drawing.ColorTheme Theme {
            get { return _theme; }
            set {
                if (typeof(Ai.Renderer.Drawing.ColorTheme).IsAssignableFrom(value.GetType())) {
                    throw new ArgumentException("Value must be one of Ai.Renderer.Drawing.ColorTheme enumeration.", "value");
                } else {
                    if (_theme != value) _theme = value;
                }
            }
        }
        private void ColorChooser_EnabledChanged(object sender, System.EventArgs e) { this.Invalidate(); }
        private void ColorChooser_GotFocus(object sender, System.EventArgs e) {
            if (_resumePainting) this.Invalidate();
        }
        private void ColorChooser_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Return:
                    ColorDialog clrDlg = new ColorDialog();
                    bool colorChanged = false;
                    clrDlg.Color = _selectedColor;
                    if (clrDlg.ShowDialog() == DialogResult.OK) {
                        _selectedColor = clrDlg.Color;
                        _mruColors.Add(_selectedColor);
                        colorChanged = true;
                    }
                    clrDlg.Dispose();
                    this.Invalidate();
                    if (colorChanged) if (SelectedColorChanged != null) SelectedColorChanged(this, new EventArgs());
                    break;
                case Keys.Apps:
                    Point scrPoint = new Point(0, this.Height);
                    _resumePainting = false;
                    cPopup.setSelectedColor();
                    _tdown.Show(this, scrPoint.X, scrPoint.Y);
                    break;
            }
        }
        private void ColorChooser_LostFocus(object sender, System.EventArgs e) {
            if (_resumePainting) this.Invalidate();
        }
        private void ColorChooser_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (!_pressed) {
                    _pressed = true;
                    PaintEventArgs pe;
                    pe = new PaintEventArgs(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
                    this.InvokePaint(this, pe);
                    pe.Dispose();
                }
                if (_hoverSub) {
                    Point scrPoint = new Point(0, this.Height);
                    _resumePainting = false;
                    cPopup.setSelectedColor();
                    _tdown.Show(this, scrPoint.X, scrPoint.Y);
                } else {
                    ColorDialog clrDlg = new ColorDialog();
                    bool colorChanged = false;
                    clrDlg.Color = _selectedColor;
                    if (clrDlg.ShowDialog() == DialogResult.OK) {
                        _selectedColor = clrDlg.Color;
                        _mruColors.Add(_selectedColor);
                        colorChanged = true;
                    }
                    clrDlg.Dispose();
                    this.Invalidate();
                    if (colorChanged) if (SelectedColorChanged != null) SelectedColorChanged(this, new EventArgs());
                }
            }
        }
        private void ColorChooser_MouseEnter(object sender, System.EventArgs e) {
            _onMouse = true;
            if (_resumePainting) this.Invalidate();
        }
        private void ColorChooser_MouseLeave(object sender, System.EventArgs e) {
            _onMouse = false;
            _pressed = false;
            _hoverSub = false;
            if (_resumePainting) this.Invalidate();
        }
        private void ColorChooser_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (_rectSub.Contains(e.X, e.Y)) {
                if (!_hoverSub) {
                    _hoverSub = true;
                    if (_resumePainting) this.Invalidate();
                }
            } else {
                if (_hoverSub) {
                    _hoverSub = false;
                    if (_resumePainting) this.Invalidate();
                }
            }
        }
        private void ColorChooser_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (_pressed && !_hoverSub) {
                ColorDialog cDialog ;
                cDialog = new ColorDialog();
                cDialog.Color = _selectedColor;
                if (cDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                    _selectedColor = cDialog.Color;
                    this.Invalidate();
                }
                cDialog.Dispose();
            }
        }
        private void ColorShooser_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            Rectangle _rectColor, rect;
            Ai.Renderer.Button.SplitEffectLocation pressedLocation = Ai.Renderer.Button.SplitEffectLocation.None;
            Ai.Renderer.Button.SplitEffectLocation hoverLocation = Ai.Renderer.Button.SplitEffectLocation.None;
            if (this.Enabled) {
                if (_pressed) {
                    if (_hoverSub) pressedLocation = Ai.Renderer.Button.SplitEffectLocation.Split;
                    else pressedLocation = Ai.Renderer.Button.SplitEffectLocation.Button;
                } else {
                    if (_onMouse) {
                        if (_hoverSub) hoverLocation = Ai.Renderer.Button.SplitEffectLocation.Split;
                        else hoverLocation = Ai.Renderer.Button.SplitEffectLocation.Button;
                    }
                }
            }
            rect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
            _rectColor = new Rectangle(4, 4, this.Width - 17, this.Height - 8);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.Clear(this.BackColor);
            Ai.Renderer.Button.drawSplit(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), 
                Ai.Renderer.Button.SplitLocation.Right, 10, _theme, 2, this.Enabled, 
                pressedLocation, false, hoverLocation, this.Focused);
            e.Graphics.FillRectangle(new SolidBrush(_selectedColor), _rectColor);
            e.Graphics.DrawRectangle(Pens.Black, _rectColor);
            if (this.Enabled) 
                Ai.Renderer.Drawing.drawTriangle(e.Graphics, _rectSub, Color.FromArgb(21, 66, 139), Color.White, Ai.Renderer.Drawing.TriangleDirection.Down);
            else
                Ai.Renderer.Drawing.drawTriangle(e.Graphics, _rectSub, Color.Gray, Color.White, Ai.Renderer.Drawing.TriangleDirection.Down);
        }
        private void ColorChooser_Resize(object sender, System.EventArgs e) { _rectSub = new Rectangle(this.Width - 12, 0, 12, this.Height); }
        private void _tdown_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e) {
            _resumePainting = true;
            this.Invalidate();
            if (_mustRaiseEvent) raiseMyEvent();
        }
        protected override bool IsInputKey(System.Windows.Forms.Keys keyData) {
            switch (keyData)
            {
                case Keys.Return:
                case Keys.Apps:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }
    }
}