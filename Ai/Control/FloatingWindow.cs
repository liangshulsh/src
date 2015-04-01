// Ai Software Library.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Ai.Control {
    public class FloatingWindow {
        #region Public Events
        /// <summary>
        /// Occurs when the floating window is redrawn.
        /// </summary>
        public event EventHandler<PaintEventArgs> Paint;
        /// <summary>
        /// Occurs when the mouse pointer is moved over the floating window.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;
        /// <summary>
        /// Occurs when the mouse pointer is over the floating window and a mouse button is pressed.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseDown;
        /// <summary>
        /// Occurs when the mouse pointer is over the floating window and a mouse button is released.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseUp;
        /// <summary>
        /// Occurs when the mouse pointer leaves the floating window.
        /// </summary>
        public event EventHandler<EventArgs> MouseLeave;
        /// <summary>
        /// Occurs when the mouse pointer enters   the floating window.
        /// </summary>
        public event EventHandler<EventArgs> MouseEnter;
        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;
        #endregion
        #region Members
        bool _active = false;
        bool _visible = false;
        bool _showInTaskbar = false;
        bool _modal = false;
        bool _selectable = true;
        Size _size;
        Point _location;
        System.Windows.Forms.Form _owner = null;
        FloatingForm _form;
        List<Keys> _keys;
        #endregion
        #region Private Class
        private class FloatingForm : System.Windows.Forms.Form {
            Bitmap _bmp;
            FloatingWindow _owner = null;
            #region Constructor
            public FloatingForm(FloatingWindow owner) {
                _owner = owner;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                if (_owner._owner != null) Owner = _owner._owner;
                if (!_owner._selectable) this.SetStyle(ControlStyles.Selectable, false);
                this.KeyPreview = true;
                this.Width = _owner._size.Width;
                this.Height = _owner._size.Height;
                this.ShowInTaskbar = _owner._showInTaskbar;
                this.MouseMove += _MouseMove;
                this.MouseDown += _MouseDown;
                this.MouseLeave += _MouseLeave;
                this.MouseEnter += _MouseEnter;
                this.MouseUp += _MouseUp;
                this.Resize += _Resize;
                this.Left = _owner._location.X;
                this.Top = _owner._location.Y;
                _bmp = new Bitmap(this.Width, this.Height);
                redrawWindow();
                if (!_owner._active) {
                    Win32API.ShowWindow(this.Handle, (int)Win32API.SW_SHOWNA);
                    Win32API.SetWindowPos(this.Handle, Win32API.HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height,
                        (int)(Win32API.SWP_NOSIZE | Win32API.SWP_NOMOVE | Win32API.SWP_NOACTIVATE));
                } else {
                    Win32API.ShowWindow(this.Handle, (int)Win32API.SW_SHOWNORMAL);
                }
            }
            #endregion
            #region Overrides
            protected override CreateParams CreateParams {
                get {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle = cp.ExStyle | Win32API.WS_EX_LAYERED;
                    return cp;
                }
            }
            protected override void Dispose(bool disposing) {
                if (disposing) {
                    if (_bmp != null) _bmp.Dispose();
                }
                base.Dispose(disposing);
            }
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
                foreach (Keys k in _owner._keys) {
                    if (k == keyData) {
                        KeyEventArgs ke = new KeyEventArgs(keyData);
                        _owner.onKeyDown(ke);
                        return ke.Handled;
                    }
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            #endregion
            #region Private Methods
            private void drawBitmap() {
                Graphics g = Graphics.FromImage(_bmp);
                Rectangle r = new Rectangle(0, 0, _owner._size.Width, _owner._size.Height);
                _owner.onPaint(g, r);
            }
            private void setBitmap(Bitmap aBmp) {
                IntPtr screenDC = Win32API.GetDC(IntPtr.Zero);
                IntPtr memDC = Win32API.CreateCompatibleDC(screenDC);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;
                try {
                    hBitmap = aBmp.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = Win32API.SelectObject(memDC, hBitmap);

                    Win32API.SIZE size = new Win32API.SIZE(aBmp.Width, aBmp.Height);
                    Win32API.POINT pointSource = new Win32API.POINT(0, 0);
                    Win32API.POINT topPos = new Win32API.POINT(this.Left, this.Top);
                    Win32API.BLENDFUNCTION blend = new Win32API.BLENDFUNCTION();
                    blend.BlendOp = Win32API.AC_SRC_OVER;
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = 255;
                    blend.AlphaFormat = Win32API.AC_SRC_ALPHA;
                    Win32API.UpdateLayeredWindow(this.Handle, screenDC, ref topPos,
                        ref size, memDC, ref pointSource, 0, ref blend, Win32API.ULW_ALPHA);
                } finally {
                    Win32API.ReleaseDC(IntPtr.Zero, screenDC);
                    if (hBitmap != IntPtr.Zero) {
                        Win32API.SelectObject(memDC, oldBitmap);
                        Win32API.DeleteObject(hBitmap);
                    }
                    Win32API.DeleteDC(memDC);
                }
            }
            private void _MouseDown(object sender, MouseEventArgs e) {
                _owner.onMouseDown(e);
            }
            private void _MouseMove(object sender, MouseEventArgs e) {
                _owner.onMouseMove(e);
            }
            private void _MouseUp(object sender, MouseEventArgs e) { _owner.onMouseUp(e); }
            private void _MouseEnter(object sender, EventArgs e) {
                _owner.onMouseEnter(e);
            }
            private void _MouseLeave(object sender, EventArgs e) {
                _owner.onMouseLeave(e);
            }
            private void _Resize(object sender, EventArgs e) {
                //if (_bmp != null) _bmp.Dispose();
                //_bmp = new Bitmap(this.Width, this.Height);
                //redrawWindow();
            }
            #endregion
            #region Internal Methods
            internal void redrawWindow() {
                drawBitmap();
                setBitmap(_bmp);
            }
            #endregion
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Raises Paint event.
        /// </summary>
        /// <param name="g">Graphics object where the Paint event will be raised.</param>
        /// <param name="rect">The bounding rectangle of the graphics object.</param>
        private void onPaint(Graphics g, Rectangle rect) {
            PaintEventArgs pe = new PaintEventArgs(g, rect);
            if (Paint != null) Paint(this, pe);
        }
        /// <summary>
        /// Raises MouseMove event.
        /// </summary>
        /// <param name="e">A MouseEventArgs object contains data needed by the event.</param>
        private void onMouseMove(MouseEventArgs e) {
            if (MouseMove != null) MouseMove(this, e);
        }
        /// <summary>
        /// Raises MouseDown event.
        /// </summary>
        /// <param name="e">A MouseEventArgs object contains the data needed by the event.</param>
        private void onMouseDown(MouseEventArgs e) {
            if (MouseDown != null) MouseDown(this, e);
        }
        /// <summary>
        /// Raises MouseUp event.
        /// </summary>
        /// <param name="e">A MouseEventArgs object contains the data needed by the event.</param>
        private void onMouseUp(MouseEventArgs e) {
            if (MouseUp != null) MouseUp(this, e);
        }
        /// <summary>
        /// Raises MouseEnter event.
        /// </summary>
        /// <param name="e">An EventArgs object contains data needed by the event.</param>
        private void onMouseEnter(EventArgs e) {
            if (MouseEnter != null) MouseEnter(this, e);
        }
        /// <summary>
        /// Raises MouseLeave event.
        /// </summary>
        /// <param name="e">An EventArgs object contains data needed by the event.</param>
        private void onMouseLeave(EventArgs e) {
            if (MouseLeave != null) MouseLeave(this, e);
        }
        /// <summary>
        /// Raises KeyDown event.
        /// </summary>
        /// <param name="e"></param>
        private void onKeyDown(KeyEventArgs e) {
            if (KeyDown != null) KeyDown(this, e);
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Floating window constructor.
        /// </summary>
        public FloatingWindow() {
            _form = null;
            _location = new Point();
            _size = new Size();
            _keys = new List<Keys>();
        }
        /// <summary>
        /// Invalidates the entire surface of the control and causes the control to be redrawn.
        /// </summary>
        public void invalidateWindow() {
            if (_form != null) {
                _form.redrawWindow();
            }
        }
        /// <summary>
        /// Show the window.
        /// </summary>
        public void show() {
            if (_form != null) _form.Dispose();
            _form = new FloatingForm(this);
            _visible = true;
        }
        /// <summary>
        /// Hides the window.
        /// </summary>
        public void hide() {
            if (_form != null) {
                _form.Visible = false;
                _form.Dispose();
                _form = null;
            }
            _visible = false;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating the window appears as an active window.
        /// </summary>
        public bool Active {
            get { return _active; }
            set { _active = value; }
        }
        /// <summary>
        /// Gets a value indicating the window is displayed.
        /// </summary>
        public bool Visible {
            get { return _visible; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the window is displayed in the Windows taskbar.
        /// </summary>
        public bool ShowInTaskbar {
            get { return _showInTaskbar; }
            set { _showInTaskbar = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this window is displayed modally.
        /// </summary>
        public bool Modal {
            get { return _modal; }
            set { _modal = value; }
        }
        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        public Size Size {
            get { return _size; }
            set {
                if (_size != value) {
                    _size = value;
                    if (_form != null) {
                        _form.Size = _size;
                        _form.redrawWindow();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the window relative to the upper-left corner of the screen.
        /// </summary>
        public Point Location {
            get { return _location; }
            set {
                if (_location != value) {
                    _location = value;
                    if (_form != null) _form.Location = _location;
                }
            }
        }
        public System.Windows.Forms.Form Owner {
            get { return _owner; }
            set { _owner = value; }
        }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int Width {
            get { return _size.Width; }
            set {
                if (_size.Width != value) {
                    _size.Width = value;
                    if (_form != null) {
                        _form.Size = _size;
                        _form.redrawWindow();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int Height {
            get { return _size.Height; }
            set {
                if (_size.Height != value) {
                    _size.Height = value;
                    if (_form != null) {
                        _form.Size = _size;
                        _form.redrawWindow();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the left coordinate of the window.
        /// </summary>
        public int X {
            get { return _location.X; }
            set {
                if (_location.X != value) {
                    _location.X = value;
                    if (_form != null) _form.Left = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the top coordinate of the window.
        /// </summary>
        public int Y {
            get { return _location.Y; }
            set {
                if (_location.Y != value) {
                    _location.Y = value;
                    if (_form != null) _form.Top = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value to determine whether the form can receive focus.
        /// </summary>
        public bool Selectable {
            get { return _selectable; }
            set {
                _selectable = value;
            }
        }
        /// <summary>
        /// Gets the window handle of the current floating window.
        /// </summary>
        public IntPtr Handle {
            get { 
                if (_form != null)return _form.Handle;
                return IntPtr.Zero;
            }
        }
        /// <summary>
        /// Gets the form object of the current floating window.
        /// </summary>
        public Form Form { get { return _form; } }
        /// <summary>
        /// Gets list of keys that will be processed in keydown event.
        /// </summary>
        public List<Keys> Keys { get { return _keys; } }
        #endregion
    }
}