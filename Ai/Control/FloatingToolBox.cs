// Ai Software Control Library.
// Date created : June 25, 2012.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace Ai.Control {
    #region Public Classes
    /// <summary>
    /// Represent an item of the FloatingToolBox.
    /// </summary>
    public abstract class ToolBoxItem {
        #region Public Events
        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        public event EventHandler<EventArgs> Click;
        /// <summary>
        /// Occurs when the Enabled property value has changed.
        /// </summary>
        public event EventHandler<EventArgs> EnabledChanged;
        /// <summary>
        /// Occurs when the Visible property value changes.
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;
        /// <summary>
        /// Occurs when the Text property value changes.
        /// </summary>
        public event EventHandler<EventArgs> TextChanged;
        /// <summary>
        /// Occurs when the Image property value changes.
        /// </summary>
        public event EventHandler<EventArgs> ImageChanged;
        #endregion
        #region Internal Fields
        internal bool _enabled = true;
        internal bool _visible = true;
        internal string _text = "";
        internal Image _image = null;
        internal string _toolTip = "";
        internal string _toolTipTitle = "";
        internal FloatingToolBox _owner = null;
        internal bool _acceptMouseEvent = true;
        internal string _name = "";
        Keys _shortcut = Keys.None;
        #endregion
        #region Constructor
        public ToolBoxItem() { }
        #endregion
        #region Internal Functions
        /// <summary>
        /// Raises Click event.
        /// </summary>
        internal void onClick() {
            if (Click != null) Click(this, new EventArgs());
        }
        /// <summary>
        /// Raises EnabledChanged event.
        /// </summary>
        internal void onEnabledChanged() {
            if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
        }
        /// <summary>
        /// Raises VisibleChanged event.
        /// </summary>
        internal void onVisibleChanged() {
            if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
        }
        /// <summary>
        /// Raises TextChanged event.
        /// </summary>
        internal void onTextChanged() {
            if (TextChanged != null) TextChanged(this, new EventArgs());
        }
        /// <summary>
        /// Raises ImageChanged event.
        /// </summary>
        internal void onImageChanged() {
            if (ImageChanged != null) ImageChanged(this, new EventArgs());
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the item can respond to user interaction.
        /// </summary>
        public virtual bool Enabled {
            get { return _enabled; }
            set {
                if (_enabled != value) {
                    _enabled = value;
                    onEnabledChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the shortcut key associated with the item.
        /// </summary>
        public virtual Keys Shortcut {
            get { return _shortcut; }
            set { _shortcut = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the item is displayed.
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;
                    onVisibleChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the FlaotingToolBox that owned this item.
        /// </summary>
        public FloatingToolBox Owner {
            get { return _owner; }
            internal set { _owner = value; }
        }
        /// <summary>
        /// Gets or sets the text associated with this item.
        /// </summary>
        public virtual string Text {
            get { return _text; }
            set {
                if (_text != value) {
                    _text = value;
                    onTextChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the image associated with this item.
        /// </summary>
        public virtual Image Image {
            get { return _image; }
            set {
                if (_image != value) {
                    _image = value;
                    onImageChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the tooltip associated with this item.
        /// </summary>
        public virtual string ToolTip {
            get { return _toolTip; }
            set { _toolTip = value; }
        }
        /// <summary>
        /// Gets or sets the title of the tooltip associated with this item.
        /// </summary>
        public virtual string ToolTipTitle {
            get { return _toolTipTitle; }
            set { _toolTipTitle = value; }
        }
        internal bool AcceptMouseEvent { get { return _acceptMouseEvent; } }
        #endregion
    }
    /// <summary>
    /// Represent a button of the FloatingToolBox.
    /// </summary>
    public sealed class ToolBoxButton : ToolBoxItem {
        public ToolBoxButton() : base() { }
    }
    /// <summary>
    /// Represent a label of the FloatingToolBox.
    /// </summary>
    public sealed class ToolBoxLabel : ToolBoxItem {
        public ToolBoxLabel() : base() {
            _acceptMouseEvent = false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Keys Shortcut {
            get { return base.Shortcut; }
            set { base.Shortcut = value; }
        }
    }
    /// <summary>
    /// Represent a separator of the FloatingToolBox.
    /// </summary>
    public sealed class ToolBoxSeparator : ToolBoxItem {
        public ToolBoxSeparator() : base() {
            _acceptMouseEvent = false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Enabled {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image Image {
            get { return base.Image; }
            set { base.Image = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToolTip {
            get { return base.ToolTip; }
            set { base.ToolTip = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToolTipTitle {
            get { return base.ToolTipTitle; }
            set { base.ToolTipTitle = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Keys Shortcut {
            get { return base.Shortcut; }
            set { base.Shortcut = value; }
        }
    }
    #endregion
    /// <summary>
    /// Represent a toolbox window.
    /// </summary>
    public class FloatingToolBox : IDisposable {
        internal const int BUTTON_SIZE = 26;
        internal const int IMAGE_SIZE = 22;
        #region Public Classes
        /// <summary>
        /// Represent a collection of ToolBoxItem objects in a FloatingToolBox.
        /// </summary>
        public class ToolBoxItemCollection : CollectionBase {
            FloatingToolBox _owner = null;
            public ToolBoxItemCollection(FloatingToolBox owner) : base() {
                _owner = owner;
            }
            #region Public Events
            public event EventHandler<CollectionEventArgs> Clearing;
            public event EventHandler<CollectionEventArgs> AfterClear;
            public event EventHandler<CollectionEventArgs> Inserting;
            public event EventHandler<CollectionEventArgs> AfterInsert;
            public event EventHandler<CollectionEventArgs> Removing;
            public event EventHandler<CollectionEventArgs> AfterRemove;
            public event EventHandler<CollectionEventArgs> Setting;
            public event EventHandler<CollectionEventArgs> AfterSet;
            #endregion
            /// <summary>
            /// Gets a ToolBoxItem object in the collection specified by its index.
            /// </summary>
            /// <param name="index">A zero-based index of the item in the collection.</param>
            /// <returns>A ToolBoxItem object if succeeded, null, otherwise.</returns>
            public ToolBoxItem this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (ToolBoxItem)List[index];
                    return null;
                }
            }
            #region Public Functions
            public int IndexOf(ToolBoxItem item) { return List.IndexOf(item); }
            /// <summary>
            /// Add a ToolBoxItem object to the collection.
            /// </summary>
            public ToolBoxItem Add(ToolBoxItem item) {
                if (!List.Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    item.EnabledChanged += _owner.item_EnabledChanged;
                    item.VisibleChanged += _owner.item_VisibleChanged;
                    item.TextChanged += _owner.item_TextChanged;
                    item.ImageChanged += _owner.item_ImageChanged;
                    return (ToolBoxItem)List[index];
                }
                return null;
            }
            /// <summary>
            /// Add a ToolBoxItem collection to the collection.
            /// </summary>
            public void AddRange(ToolBoxItemCollection items) {
                foreach (ToolBoxItem item in items) this.Add(item);
            }
            /// <summary>
            /// Insert a ToolBoxItem object to the collection at specified index.
            /// </summary>
            public void Insert(int index, ToolBoxItem item) {
                if (!List.Contains(item)) {
                    item._owner = _owner;
                    List.Insert(index, item);
                    item.EnabledChanged += _owner.item_EnabledChanged;
                    item.VisibleChanged += _owner.item_VisibleChanged;
                    item.TextChanged += _owner.item_TextChanged;
                    item.ImageChanged += _owner.item_ImageChanged;
                }
            }
            /// <summary>
            /// Remove a ToolBoxItem object from the collection.
            /// </summary>
            public void Remove(ToolBoxItem item) {
                if (List.Contains(item)) List.Remove(item);
            }
            /// <summary>
            /// Determine whether a ToolBoxItem object exist in the collection.
            /// </summary>
            public bool Contains(ToolBoxItem item) { return List.Contains(item); }
            #endregion
            #region Internal Overriden Functions
            protected override void OnValidate(object value) {
                if (!typeof(ToolBoxItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.ToolBoxItem", "value");
            }
            protected override void OnClear() {
                int i = 0;
                while (i < List.Count) {
                    ToolBoxItem item = (ToolBoxItem)List[i];
                    item.EnabledChanged -= _owner.item_EnabledChanged;
                    item.VisibleChanged -= _owner.item_VisibleChanged;
                    item.TextChanged -= _owner.item_TextChanged;
                    item.ImageChanged -= _owner.item_ImageChanged;
                    i++;
                }
                if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
            }
            protected override void OnClearComplete() {
                if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
            }
            protected override void OnInsert(int index, object value) {
                if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) {
                if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
            }
            protected override void OnRemove(int index, object value) {
                ToolBoxItem item = (ToolBoxItem)value;
                item.EnabledChanged -= _owner.item_EnabledChanged;
                item.VisibleChanged -= _owner.item_VisibleChanged;
                item.TextChanged -= _owner.item_TextChanged;
                item.ImageChanged -= _owner.item_ImageChanged;
                if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
            }
            protected override void OnRemoveComplete(int index, object value) {
                if (AfterRemove != null) AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
            }
            protected override void OnSet(int index, object oldValue, object newValue) {
                if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            #endregion
        }
        #endregion
        #region Private Classes
        private abstract class ItemHost {
            internal ToolBoxItem _item = null;
            internal FloatingToolBox _owner = null;
            internal Rectangle _rect;
            internal bool _hover = false;
            internal bool _pressed = false;
            public ItemHost(FloatingToolBox owner, ToolBoxItem item) {
                _item = item;
                _owner = owner;
                _rect.Height = BUTTON_SIZE;
            }
            public int X {
                get { return _rect.X; }
                set { _rect.X = value; }
            }
            public int Y {
                get { return _rect.Y; }
                set { _rect.Y = value; }
            }
            public bool Hover {
                get { return _hover; }
                set { _hover = value; }
            }
            public bool Pressed {
                get { return _pressed; }
                set { _pressed = value; }
            }
            public int Height { get { return _rect.Height; } }
            public int Width { get { return _rect.Width; } }
            public FloatingToolBox Owner { get { return _owner; } }
            public ToolBoxItem Item { get { return _item; } }
            public bool mouseMove(MouseEventArgs e) {
                if (!_item._acceptMouseEvent) return false;
                if (!(_item.Enabled && _item.Visible)) return false;
                bool changed = false;
                if (_rect.Contains(e.Location)) {
                    if (!_hover) {
                        if (_owner._hoverHost != this) {
                            if (_owner._hoverHost != null) _owner._hoverHost._hover = false;
                        }
                        _hover = true;
                        _owner._hoverHost = this;
                        changed = true;
                        if ((_item.ToolTip != "" || _item.ToolTipTitle != "") && !_pressed) {
                            _owner._currentTitle = _item.ToolTipTitle;
                            _owner._currentToolTip = _item.ToolTip;
                            _owner._toolTip.show(_owner._owner, _rect);
                        }
                    }
                } else {
                    if (_hover) {
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                        _hover = false;
                        changed = true;
                    }
                }
                return changed;
            }
            public bool mouseDown(MouseEventArgs e) {
                if (!_item._acceptMouseEvent) return false;
                if (!(_item.Enabled && _item.Visible)) return false;
                bool changed = false;
                if (e.Button == MouseButtons.Left) {
                    if (_hover) {
                        if (!_pressed) {
                            if (_owner._pressedHost != this) {
                                if (_owner._pressedHost != null) _owner._pressedHost._pressed = false;
                                _pressed = true;
                                changed = true;
                                _owner._pressedHost = this;
                                _owner._toolTip.hide();
                            }
                        }
                    }
                }
                return changed;
            }
            public bool mouseUp(MouseEventArgs e) {
                if (!_item._acceptMouseEvent) return false;
                if (!(_item.Enabled && _item.Visible)) return false;
                bool changed = false;
                if (_pressed) {
                    if (_owner._pressedHost == this) _owner._pressedHost = null;
                    _pressed = false;
                    changed = true;
                    if (e.Button == MouseButtons.Left && _hover) _item.onClick();
                }
                return changed;
            }
            public bool mouseLeave() {
                if (!_item._acceptMouseEvent) return false;
                if (!(_item.Enabled && _item.Visible)) return false;
                if (_hover) {
                    _hover = false;
                    if (_owner._hoverHost == this) _owner._hoverHost = null;
                    _owner._toolTip.hide();
                    return true;
                }
                return false;
            }
            public abstract void measureSize(Graphics g);
            public abstract void draw(Graphics g);
        }
        private class ButtonHost : ItemHost {
            public ButtonHost(FloatingToolBox owner, ToolBoxButton button) : base(owner, button) { }
            public override void measureSize(Graphics g) {
                int w = 4;
                if (_item.Image != null) w += IMAGE_SIZE;
                if (_item.Text != "") {
                    SizeF tSize = g.MeasureString(_item.Text, _owner._font);
                    w += (int)tSize.Width + 5;
                }
                _rect.Width = w;
            }
            public override void draw(Graphics g) {
                if (_item.Visible) {
                    if (_item.Enabled) {
                        if (_hover || _pressed) {
                            GraphicsPath gp = Renderer.Drawing.roundedRectangle(_rect, 2, 2, 2, 2);
                            LinearGradientBrush bg = new LinearGradientBrush(_rect, Color.White, Color.Black, LinearGradientMode.Vertical);
                            if (_hover && _pressed) {
                                bg.InterpolationColors = Renderer.Button.PressedBlend();
                            } else {
                                if (_hover) bg.InterpolationColors = Renderer.Button.HLitedBlend();
                                else bg.InterpolationColors = Renderer.Button.SelectedBlend();
                            }
                            g.FillPath(bg, gp);
                            if (!_pressed) g.DrawPath(Renderer.Button.HLitedBorderPen(), gp);
                            else g.DrawPath(Renderer.Button.SelectedBorderPen(), gp);
                            gp.Dispose();
                            bg.Dispose();
                        }
                    }
                    int lastX = _rect.X;
                    if (_item.Image != null) {
                        Rectangle rectImg = new Rectangle(_rect.X, _rect.Y, _rect.Height, _rect.Height);
                        rectImg = Renderer.Drawing.getImageRectangle(_item.Image, rectImg, IMAGE_SIZE);
                        if (_item.Enabled) g.DrawImage(_item.Image, rectImg);
                        else Renderer.Drawing.grayScaledImage(_item.Image, rectImg, g);
                        lastX += _rect.Height;
                    }
                    if (_item.Text != "") {
                        Rectangle rectText = new Rectangle(lastX, _rect.Y, _rect.Width - (lastX - _rect.X), _rect.Height);
                        SolidBrush tb;
                        if (_item.Enabled) tb = new SolidBrush(Color.Black);
                        else tb = new SolidBrush(Color.Gray);
                        g.DrawString(_item.Text, _owner._font, tb, rectText, _owner._sf);
                        tb.Dispose();
                    }
                }
            }
        }
        private class LabelHost : ItemHost {
            public LabelHost(FloatingToolBox owner, ToolBoxLabel label) : base(owner, label) { }
            public override void measureSize(Graphics g) {
                int w = 4;
                if (_item.Image != null) w += IMAGE_SIZE;
                if (_item.Text != "") {
                    SizeF tSize = g.MeasureString(_item.Text, _owner._font);
                    w += (int)tSize.Width + 5;
                }
                _rect.Width = w;
            }
            public override void draw(Graphics g) {
                if (_item.Visible) {
                    int lastX = _rect.X;
                    if (_item.Image != null) {
                        Rectangle rectImg = new Rectangle(_rect.X, _rect.Y, _rect.Height, _rect.Height);
                        rectImg = Renderer.Drawing.getImageRectangle(_item.Image, rectImg, IMAGE_SIZE);
                        if (_item.Enabled) g.DrawImage(_item.Image, rectImg);
                        else Renderer.Drawing.grayScaledImage(_item.Image, rectImg, g);
                        lastX += _rect.Height;
                    }
                    if (_item.Text != "") {
                        Rectangle rectText = new Rectangle(lastX, _rect.Y, _rect.Width - (lastX - _rect.X), _rect.Height);
                        SolidBrush tb;
                        if (_item.Enabled) tb = new SolidBrush(Color.Black);
                        else tb = new SolidBrush(Color.Gray);
                        g.DrawString(_item.Text, _owner._font, tb, rectText, _owner._sf);
                        tb.Dispose();
                    }
                }
            }
        }
        private class SeparatorHost : ItemHost {
            public SeparatorHost(FloatingToolBox owner, ToolBoxSeparator separator) : base(owner, separator) { }
            public override void measureSize(Graphics g) {
                _rect.Width = 2;
            }
            public override void draw(Graphics g) {
                if (_item.Visible) {
                    g.DrawLine(Renderer.Button.NormalBorderPen(), _rect.X, _rect.Y, _rect.X, _rect.Bottom);
                    g.DrawLine(Pens.White, _rect.X + 1, _rect.Y, _rect.X + 1, _rect.Bottom);
                }
            }
        }
        #endregion
        #region Fields
        Form _owner = null;
        FloatingWindow _fWindow = null;
        int _alphaHover = 223;
        int _alphaLeave = 63;
        GraphicsPath _gp, _gpShadow;
        Rectangle _iconRect;
        ToolTip _toolTip;
        bool _hover = false;
        ToolBoxItemCollection _items;
        Font _font = new Font("Segoe UI", 8, FontStyle.Regular);
        StringFormat _sf;
        // Hosts
        List<ItemHost> _hosts;
        ItemHost _hoverHost = null;
        ItemHost _pressedHost = null;
        // Tooltip
        string _currentToolTip = "";
        string _currentTitle = "";
        #endregion
        #region Constructor
        public FloatingToolBox(Form owner) {
            _owner = owner;
            // Initializing
            _fWindow = new FloatingWindow();
            float deg = (float)(Math.Asin(15d / 25d) * 180 / Math.PI);
            _iconRect = new Rectangle(0, -10, 50, 50);
            _toolTip = new Ai.Control.ToolTip();
            _items = new ToolBoxItemCollection(this);
            _sf = new StringFormat();
            _hosts = new List<ItemHost>();
            // Setting up floating window.
            _fWindow.Height = 45;
            _fWindow.ShowInTaskbar = false;
            _fWindow.Active = false;
            _fWindow.Modal = false;
            _fWindow.Selectable = false;
            _fWindow.Owner = _owner;
            _fWindow.Paint += _Paint;
            _fWindow.MouseMove += _MouseMove;
            _fWindow.MouseDown += _MouseDown;
            _fWindow.MouseUp += _MouseUp;
            _fWindow.MouseLeave += _MouseLeave;
            _fWindow.MouseEnter += _MouseEnter;
            _fWindow.KeyDown += _KeyDown;
            // Setting up tooltip
            _toolTip.AnimationSpeed = 20;
            _toolTip.Draw += tooltip_Draw;
            _toolTip.Popup += tooltip_Popup;
            // Setting up string format.
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
            // Setting up items.
            _items.AfterClear += _items_AfterClear;
            _items.AfterInsert += _items_AfterInsert;
            _items.AfterRemove += _items_AfterRemove;
            measureWindow();
        }
        #endregion
        #region Public Properties
        public int AlphaHover {
            get { return _alphaHover; }
            set {
                if (_alphaHover != value) {
                    if (value >= 0 && value < 256) _alphaHover = value;
                }
            }
        }
        public int AlphaLeave {
            get { return _alphaLeave; }
            set {
                if (_alphaLeave != value) {
                    if (value >= 0 && value < 256) _alphaLeave = value;
                }
            }
        }
        public int X {
            get { return _fWindow.X; }
            set { _fWindow.X = value; }
        }
        public int Y {
            get { return _fWindow.Y; }
            set { _fWindow.Y = value; }
        }
        public int Width { get { return _fWindow.Width; } }
        public int Height { get { return _fWindow.Height; } }
        public bool Visible { get { return _fWindow.Visible; } }
        public Form Owner { get { return _owner; } }
        public ToolBoxItemCollection Items { get { return _items; } }
        #endregion
        #region Public Functions
        public void show() { _fWindow.show(); }
        public void hide() { _fWindow.hide(); }
        #endregion
        #region Private Functions
        private void collectKeys() {
            _fWindow.Keys.Clear();
            foreach (ToolBoxItem item in _items) {
                if (item.Shortcut != Keys.None) _fWindow.Keys.Add(item.Shortcut);
            }
        }
        private void measureWindow() {
            float deg = (float)(Math.Asin(15d / 25d) * 180 / Math.PI);
            if (_gp != null) _gp.Dispose();
            if (_gpShadow != null) _gpShadow.Dispose();
            _gp = new GraphicsPath();
            _gpShadow = new GraphicsPath();
            _gp.AddArc(_iconRect, deg, 180f);
            _gpShadow.AddArc(new Rectangle(_iconRect.X + 2, _iconRect.Y + 2, _iconRect.Width, _iconRect.Height), deg, 180f);
            int x = _iconRect.Width;
            if (_items.Count > 0) {
                int i = 0;
                // Temporary only
                Bitmap bmp = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                while (i < _hosts.Count) {
                    _hosts[i].measureSize(g);
                    _hosts[i].X = x;
                    _hosts[i].Y = 2;
                    if (_hosts[i].Item.Visible) x += _hosts[i].Width + 2;
                    i++;
                }
                g.Dispose();
                bmp.Dispose();
            }
            _gp.AddArc(new Rectangle(x - 40, _iconRect.Y, 50, 50), 360f - deg, 2 * deg);
            _gpShadow.AddArc(new Rectangle(x - 38, _iconRect.Y + 2, 50, 50), 360f - deg, 2 * deg);
            _gp.CloseFigure();
            _gpShadow.CloseFigure();
            _fWindow.Width = x + 15;
        }
        // Event handlers.
        private void item_EnabledChanged(object sender, EventArgs e) {
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void item_VisibleChanged(object sender, EventArgs e) {
            measureWindow();
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void item_TextChanged(object sender, EventArgs e) {
            measureWindow();
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void item_ImageChanged(object sender, EventArgs e) {
            measureWindow();
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void _items_AfterInsert(object sender, CollectionEventArgs e) {
            ItemHost newHost = null;
            if (e.Item is ToolBoxButton) newHost = new ButtonHost(this, (ToolBoxButton)e.Item);
            else if (e.Item is ToolBoxLabel) newHost = new LabelHost(this, (ToolBoxLabel)e.Item);
            else if (e.Item is ToolBoxSeparator) newHost = new SeparatorHost(this, (ToolBoxSeparator)e.Item);
            if (newHost != null) {
                if (e.Index >= _hosts.Count) _hosts.Add(newHost);
                else _hosts.Insert(e.Index, newHost);
                measureWindow();
                collectKeys();
                if (_fWindow.Visible) _fWindow.invalidateWindow();
            }
        }
        private void _items_AfterClear(object sender, CollectionEventArgs e) {
            _hosts.Clear();
            _hoverHost = null;
            _pressedHost = null;
            measureWindow();
            _fWindow.Keys.Clear();
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void _items_AfterRemove(object sender, CollectionEventArgs e) {
            int i = 0;
            while (i < _hosts.Count) {
                if (_hosts[i].Item == e.Item) {
                    if (_hoverHost == _hosts[i]) _hoverHost = null;
                    if (_pressedHost == _hosts[i]) _pressedHost = null;
                    _hosts.RemoveAt(i);
                    break;
                }
                i++;
            }
            measureWindow();
            collectKeys();
            if (_fWindow.Visible) _fWindow.invalidateWindow();
        }
        private void _Paint(object sender, PaintEventArgs e) {
            Bitmap bmp = new Bitmap(_fWindow.Width, _fWindow.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.Transparent);
            // Draw shadow.
            g.FillPath(new SolidBrush(Color.FromArgb(63, 0, 0, 0)), _gpShadow);
            // Button background.
            Rectangle rect = new Rectangle(0, 0, _fWindow.Width, 30);
            LinearGradientBrush bgLine = new LinearGradientBrush(rect, Color.White, Color.Black, LinearGradientMode.Vertical);
            bgLine.InterpolationColors = Renderer.Button.NormalBlend();
            g.FillPath(bgLine, _gp);
            bgLine.Dispose();
            // Icon background
            GraphicsPath gp = new GraphicsPath();
            rect.X = (int)(25 + (25 * Math.Sqrt(2) / 2)) - 50;
            rect.Y = (int)(15 + (25 * Math.Sqrt(2) / 2)) - 50;
            rect.Width = 100;
            rect.Height = 100;
            gp.AddEllipse(rect);
            PathGradientBrush bg = new PathGradientBrush(gp);
            bg.InterpolationColors = Renderer.Button.NormalBlend();
            g.FillEllipse(bg, _iconRect);
            bg.Dispose();
            gp.Dispose();
            // Draw the icon.
            if (_owner != null) {
                if (_owner.Icon != null) g.DrawIcon(_owner.Icon, new Rectangle(10, 0, 30, 30));
            }
            // Draw the hosts.
            foreach (ItemHost ih in _hosts) ih.draw(g);
            // Draw outline.
            g.DrawPath(Ai.Renderer.Button.NormalBorderPen(), _gp);
            g.Dispose();
            // Drawing the bitmap into floating window.
            float fTrans = (_hover ? (float)_alphaHover / 255f : (float)_alphaLeave / 255f);
            float[][] ptsArray ={new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, fTrans, 0}, 
                new float[] {0, 0, 0, 0, 1}};
            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            ImageAttributes imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            e.Graphics.Clear(Color.Transparent);
            e.Graphics.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 
                0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgAttributes);
            bmp.Dispose();
        }
        private void _MouseMove(object sender, MouseEventArgs e) {
            int i = 0;
            bool changed = false;
            while (i < _hosts.Count) {
                changed = changed || _hosts[i].mouseMove(e);
                i++;
            }
            if (changed) _fWindow.invalidateWindow();
            if (_hoverHost == null) _toolTip.hide();
        }
        private void _MouseDown(object sender, MouseEventArgs e) {
            Rectangle rect = new Rectangle(0, 0, _iconRect.Width, _iconRect.Height - 10);
            if (rect.Contains(e.Location)) {
                if (e.Button == MouseButtons.Left) {
                    Win32API.ReleaseCapture();
                    Win32API.SendMessage(_fWindow.Handle, Win32API.WM_NCLBUTTONDOWN, (int)Win32API.HTCAPTION, 0);
                }
            } else {
                int i = 0;
                bool changed = false;
                while (i < _hosts.Count) {
                    changed = changed || _hosts[i].mouseDown(e);
                    i++;
                }
                if (changed) _fWindow.invalidateWindow();
            }
        }
        private void _MouseUp(object sender, MouseEventArgs e) {
            int i = 0;
            bool changed = false;
            while (i < _hosts.Count) {
                changed = changed || _hosts[i].mouseUp(e);
                i++;
            }
            if (changed) _fWindow.invalidateWindow();
        }
        private void _MouseLeave(object sender, EventArgs e) {
            _hover = false;
            int i = 0;
            while (i < _hosts.Count) {
                _hosts[i].mouseLeave();
                i++;
            }
            _fWindow.invalidateWindow();
            _toolTip.hide();
        }
        private void _MouseEnter(object sender, EventArgs e) {
            _hover = true;
            _fWindow.invalidateWindow();
        }
        private void _KeyDown(object sender, KeyEventArgs e) {
            ToolBoxItem item = null;
            foreach (ToolBoxItem ti in _items) {
                if (ti.Shortcut == e.KeyData) {
                    item = ti;
                    break;
                }
            }
            if (item != null) {
                e.Handled = true;
                item.onClick();
            }
        }
        private void tooltip_Draw(object sender, Ai.Control.DrawEventArgs e) {
            Ai.Renderer.ToolTip.drawToolTip(e.Graphics, new Rectangle((int)e.Rectangle.X, (int)e.Rectangle.Y, (int)e.Rectangle.Width, (int)e.Rectangle.Height), _currentTitle, _currentToolTip, null);
            _currentTitle = "";
            _currentToolTip = "";
        }
        private void tooltip_Popup(object sender, Ai.Control.PopupEventArgs e) {
            e.Size = Ai.Renderer.ToolTip.measureSize(_currentTitle, _currentToolTip, null);
        }
        #endregion
        #region IDisposable Members
        bool _disposed = false;
        public void Dispose() {
            if (!_disposed) {
                if (_gp != null) {
                    _gp.Dispose();
                    _gp = null;
                }
                if (_gpShadow != null) {
                    _gpShadow.Dispose();
                    _gpShadow = null;
                }
                if (_toolTip != null) {
                    _toolTip.Dispose();
                    _toolTip = null;
                }
                if (_font != null) {
                    _font.Dispose();
                    _font = null;
                }
                if (_sf != null) {
                    _sf.Dispose();
                    _sf = null;
                }
                if (_fWindow.Visible) _fWindow.hide();
                _disposed = true;
            }
        }
        #endregion
    }
}