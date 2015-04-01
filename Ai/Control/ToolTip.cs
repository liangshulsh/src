// Ai Software Control Library.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;

namespace Ai.Control {
    /// <summary>
    /// Specifies the location of the tooltip will be shown.
    /// </summary>
    public enum ToolTipLocation { 
        Auto,           // Tooltip location will automatically calculated based on caller(Control, ToolStripItem) bounds, usually under the caller.
        MousePointer,   // Tooltip will be shown around mouse pointer.
        CustomScreen,   // Tooltip will be shown on a location in the screen specified by CustomLocation
        CustomClient    // Tooltip will be shown on a location relative to the client area on the caller.
    }
    /// <summary>
    /// Provide data when raising popup event of the ToolTip.
    /// </summary>
    public class PopupEventArgs : EventArgs {
        SizeF _size;
        public PopupEventArgs() : base() { }
        /// <summary>
        /// Gets or sets the size of the ToolTip window.
        /// </summary>
        public SizeF Size {
            get { return _size; }
            set { _size = value; }
        }
    }
    /// <summary>
    /// Provide data when raising draw event of the ToolTip.
    /// </summary>
    public class DrawEventArgs : EventArgs {
        System.Drawing.Graphics _g;
        RectangleF _rect;
        public DrawEventArgs(System.Drawing.Graphics g, RectangleF rect) : base() {
            _g = g;
            _rect = rect;
        }
        /// <summary>
        /// Gets the Graphics object to draw the ToolTip.
        /// </summary>
        public System.Drawing.Graphics Graphics {
            get { return _g; }
        }
        /// <summary>
        /// Gets the area available for the ToolTip.
        /// </summary>
        public RectangleF Rectangle {
            get { return _rect; }
        }
    }
    /// <summary>
    /// Custom tooltip.
    /// </summary>
    [ProvideProperty("ToolTip", typeof(object)), 
    ProvideProperty("ToolTipTitle",typeof(object)), 
    ProvideProperty("ToolTipImage", typeof(object))]
    public class ToolTip : System.ComponentModel.Component, System.ComponentModel.IExtenderProvider {
        #region Public Events
        /// <summary>
        /// Event before tooltip is displayed.  This event is raised when OwnerDraw property is set to true.
        /// </summary>
        public event EventHandler<PopupEventArgs> Popup;
        /// <summary>
        /// Event when the tooltip surface is drawn.  This event is raised when OwnerDraw property is set to true.
        /// </summary>
        public event EventHandler<DrawEventArgs> Draw;
        /// <summary>
        /// Event when the tooltip background is drawn.  This event is raised when OwnerDrawBackground property is set to true.
        /// </summary>
        public event EventHandler<DrawEventArgs> DrawBackground;
        #endregion
        #region Private Fields
        System.Windows.Forms.Control _owner = null;         // ToolTip's owner.
        System.Windows.Forms.Control _control = null;
        uint _animationSpeed = 20;                          // In milliseconds, interval to fade - in / out.
        Boolean _showShadow = true;
        ToolTipForm _form;                                  // Form to display the tooltip.
        uint _autoClose = 3000;                             // In milliseconds, tooltip will automatically closed if this period passed.
        Boolean _enableAutoClose = true;
        Boolean _ownerDraw = false;
        Boolean _ownerDrawBackground = false;
        ToolTipLocation _location = ToolTipLocation.Auto;
        Point _customLocation = new Point(0, 0);
        // Supports for IExtenderProvider
        Hashtable _texts, _titles, _images;
        // Supports for mouse events
        List<object> _objEvents = new List<object>();
        #endregion
        #region ToolTip Constructors
        /// <summary>
        /// Constructor of the tooltip with an owner control specified.
        /// </summary>
        public ToolTip(System.Windows.Forms.Control owner) : base() { 
            _owner = owner;
            _texts = new Hashtable();
            _titles = new Hashtable();
            _images = new Hashtable();
            _ownerDraw = true;
        }
        /// <summary>
        /// Constructor of the tooltip with no parameter specified.
        /// </summary>
        public ToolTip() : base() { 
            _texts = new Hashtable();
            _titles = new Hashtable();
            _images = new Hashtable();
        }
        #endregion
        #region ToolTip Window
        private class ToolTipForm : System.Windows.Forms.Form {
            public static Boolean _showShadow;
            #region Private Fields
            Boolean _closing = false;
            private const int BORDER_MARGIN = 1;
            Rectangle _rect;
            GraphicsPath _path;
            Bitmap bgBitmap, tBitmap;
            Timer _timer, _tmrClose;
            Point mNormalPos;
            Rectangle mCurrentBounds;
            ToolTip mPopup;
            DateTime mTimerStarted;
            double mProgress;
            int mx, _my;
            int _alpha = 100;
            private static Image mBackgroundImage;
            #endregion
            #region Constructor
            public ToolTipForm(ToolTip popup, System.Drawing.Size size) : base() {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._owner != null) {
                    Form pForm = mPopup._owner.FindForm();
                    if (pForm != null) pForm.AddOwnedForm(this);
                } else {
                    if (mPopup._control != null) {
                        Form pForm = mPopup._control.FindForm();
                        if (pForm != null) pForm.AddOwnedForm(this);
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow) { 
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                } else { 
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate();
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = (int)mPopup._autoClose;
                _tmrClose.Tick += autoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0) {
                    _alpha = 0;
                    _timer.Interval = (int)mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += showing;
                    _timer.Start();
                    showing(null, null);
                } else {
                    setBitmap(bgBitmap);
                }
                Win32API.ShowWindow(this.Handle, (int)Win32API.SW_SHOWNA);
                Win32API.SetWindowPos(this.Handle, Win32API.HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, 
                    (int)(Win32API.SWP_NOSIZE | Win32API.SWP_NOMOVE | Win32API.SWP_NOACTIVATE));
                if (mPopup._enableAutoClose) _tmrClose.Start();
            }
            public ToolTipForm(ToolTip popup, System.Drawing.Size size, Point location) : base() {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._owner != null) {
                    Form pForm = mPopup._owner.FindForm();
                    if (pForm != null) pForm.AddOwnedForm(this);
                } else {
                    if (mPopup._control != null) {
                        Form pForm = mPopup._control.FindForm();
                        if (pForm != null) pForm.AddOwnedForm(this);
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow) {
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                } else {
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate(location);
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = (int)mPopup._autoClose;
                _tmrClose.Tick += autoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0) {
                    _alpha = 0;
                    _timer.Interval = (int)mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += showing;
                    _timer.Start();
                    showing(null, null);
                } else {
                    setBitmap(bgBitmap);
                }
                Win32API.ShowWindow(this.Handle, (int)Win32API.SW_SHOWNA);
                Win32API.SetWindowPos(this.Handle, Win32API.HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height,
                    (int)(Win32API.SWP_NOSIZE | Win32API.SWP_NOMOVE | Win32API.SWP_NOACTIVATE));
                if (mPopup._enableAutoClose) _tmrClose.Start();
            }
            public ToolTipForm(ToolTip popup, System.Drawing.Size size, Rectangle rect) : base() {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._owner != null) {
                    Form pForm = mPopup._owner.FindForm();
                    if (pForm != null) pForm.AddOwnedForm(this);
                } else {
                    if (mPopup._control != null) {
                        Form pForm = mPopup._control.FindForm();
                        if (pForm != null) pForm.AddOwnedForm(this);
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow) {
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                } else {
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate(rect);
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = (int)mPopup._autoClose;
                _tmrClose.Tick += autoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0) {
                    _alpha = 0;
                    _timer.Interval = (int)mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += showing;
                    _timer.Start();
                    showing(null, null);
                } else {
                    setBitmap(bgBitmap);
                }
                Win32API.ShowWindow(this.Handle, Win32API.SW_SHOWNA);
                Win32API.SetWindowPos(this.Handle, Win32API.HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height,
                    (Win32API.SWP_NOSIZE | Win32API.SWP_NOMOVE | Win32API.SWP_NOACTIVATE));
                if (mPopup._enableAutoClose) _tmrClose.Start();
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
                    if (_tmrClose != null) _tmrClose.Dispose();
                    if (_timer != null) _timer.Dispose();
                    if (bgBitmap != null) bgBitmap.Dispose();
                    if (tBitmap != null) tBitmap.Dispose();
                }
                base.Dispose(disposing);
            }
            #endregion
            #region Drawing ToolTip Window
            private void drawTransparentBitmap() {
                Graphics g = Graphics.FromImage(tBitmap);
                int x, y;
                System.Drawing.Color aColor, tColor;
                g.Clear(System.Drawing.Color.Transparent);
                g.Dispose();
                y = 0;
                while (y < bgBitmap.Height) {
                    x = 0;
                    while (x < bgBitmap.Width) { 
                        aColor = bgBitmap.GetPixel(x, y);
                        tColor = System.Drawing.Color.FromArgb((int)(_alpha * aColor.A / 100), aColor.R, aColor.G, aColor.B);
                        tBitmap.SetPixel(x, y, tColor);
                        x = x + 1;
                    }
                    y = y + 1;
                }
            }
            private void drawBackground(Graphics g) {
                if (!mPopup._ownerDrawBackground) {
                    if (mPopup._showShadow) {
                        System.Drawing.Drawing2D.LinearGradientBrush bgBrush;
                        GraphicsPath aPath;
                        Rectangle aRect = new Rectangle(0, 0, this.Width - 4, this.Height - 4);
                        Rectangle rectShadow = new Rectangle(4, 4, this.Width - 4, this.Height - 4);
                        GraphicsPath pathShadow = Ai.Renderer.Drawing.roundedRectangle(rectShadow, 4, 4, 4, 4);
                        PathGradientBrush shadowBrush = new PathGradientBrush(pathShadow);
                        Color[] sColor = new Color[4];
                        float[] sPos = new float[4];
                        ColorBlend sBlend = new ColorBlend();
                        sColor[0] = Color.FromArgb(0, 0, 0, 0);
                        sColor[1] = Color.FromArgb(16, 0, 0, 0);
                        sColor[2] = Color.FromArgb(32, 0, 0, 0);
                        sColor[3] = Color.FromArgb(128, 0, 0, 0);
                        if (rectShadow.Width > rectShadow.Height) { 
                            sPos[0] = 0.0F;
                            sPos[1] = 4 / rectShadow.Width;
                            sPos[2] = 8 / rectShadow.Width;
                            sPos[3] = 1.0F;
                        } else {
                            if (rectShadow.Width < rectShadow.Height) { 
                                sPos[0] = 0.0F;
                                sPos[1] = 4 / rectShadow.Height;
                                sPos[2] = 8 / rectShadow.Height;
                                sPos[3] = 1.0F;
                            } else { 
                                sPos[0] = 0.0F;
                                sPos[1] = 4 / rectShadow.Width;
                                sPos[2] = 8 / rectShadow.Width;
                                sPos[3] = 1.0F;
                            }
                        }
                        sBlend.Colors = sColor;
                        sBlend.Positions = sPos;
                        shadowBrush.InterpolationColors = sBlend;
                        if (rectShadow.Width > rectShadow.Height) { 
                            shadowBrush.CenterPoint =new Point( 
                                rectShadow.X + (rectShadow.Width / 2), 
                                rectShadow.Bottom - (rectShadow.Width / 2));
                        } else {
                            if (rectShadow.Width == rectShadow.Height) shadowBrush.CenterPoint = new Point(
                                rectShadow.X + (rectShadow.Width / 2),
                                rectShadow.Y + (rectShadow.Height / 2));
                            else shadowBrush.CenterPoint = new Point(
                                rectShadow.Right - (rectShadow.Height / 2),
                                rectShadow.Y + (rectShadow.Height / 2));
                        }
                        aPath = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                        bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(aRect, 
                            Color.FromArgb(255, 255, 255), Color.FromArgb(201, 217, 239), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);
                        g.FillPath(shadowBrush, pathShadow);
                        g.FillPath(bgBrush, aPath);
                        g.DrawPath(new Pen(Color.FromArgb(118, 118, 118)), aPath);
                        bgBrush.Dispose();
                        aPath.Dispose();
                        pathShadow.Dispose();
                        shadowBrush.Dispose();
                    } else {
                        System.Drawing.Drawing2D.LinearGradientBrush bgBrush;
                        GraphicsPath aPath;
                        Rectangle aRect = new Rectangle(0, 0, this.Width, this.Height);
                        aPath = Ai.Renderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                        bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, this.Width, this.Height),
                            Color.FromArgb(255, 255, 255), Color.FromArgb(201, 217, 239), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);
                        g.FillPath(bgBrush, aPath);
                        g.DrawPath(new Pen(Color.FromArgb(118, 118, 118)), aPath);
                        bgBrush.Dispose();
                        aPath.Dispose();
                    }
                } else { 
                    g.Clear(Color.Transparent);
                    mPopup.invokeDrawBackground(g, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }
            }
            private void drawBitmap() {
                Graphics g = Graphics.FromImage(bgBitmap);
                Rectangle rect = new Rectangle(0, 0, 0, 0);
                drawBackground(g);
                if (!mPopup._ownerDrawBackground) {
                    if (mPopup._showShadow) { 
                        rect.X = 3;
                        rect.Y = 3;
                        rect.Width = this.Width - 10;
                        rect.Height = this.Height - 10;
                    } else { 
                        rect.X = 3;
                        rect.Y = 3;
                        rect.Width = this.Width - 6;
                        rect.Height = this.Height - 6;
                    }
                } else {
                    rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                }
                mPopup.invokeDraw(g, rect);
                g.Dispose();
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
                    Win32API.BLENDFUNCTION blend= new Win32API.BLENDFUNCTION();
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
            #endregion
            #region Display Functions
            /// <summary>
            /// Auto locate of the ToolTip window, based on current mouse location.
            /// </summary>
            private void ReLocate() {
                int rW, rH;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                Cursor mCursor = mPopup._control.Cursor;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = System.Windows.Forms.Control.MousePosition;
                mNormalPos.X = (int)(mNormalPos.X + mCursor.Size.Width);
                mNormalPos.Y = (int)(mNormalPos.Y + mCursor.Size.Height);
                if (mNormalPos.X + rW > workingArea.Width) mNormalPos.X = (int)(mNormalPos.X - rW);
                if (mNormalPos.Y + rH > workingArea.Height) mNormalPos.Y = (int)(mNormalPos.Y - (rH + mCursor.Size.Height));
            }
            /// <summary>
            /// Locate the ToolTip window into specified location.
            /// </summary>
            private void ReLocate(Point location) {
                int rW, rH;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = mPopup._control.PointToScreen(location);
                if (mNormalPos.X + rW > workingArea.Width) mNormalPos.X = (int)(mNormalPos.X - rW);
                if (mNormalPos.Y + rH > workingArea.Height) mNormalPos.Y = (int)(mNormalPos.Y - rH);
            }
            /// <summary>
            /// Locate the ToolTip window to avoid overlapping specified area.
            /// </summary>
            private void ReLocate(Rectangle rect) {
                int rW, rH;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                Point askedLoc = new Point(0, 0);
                askedLoc.X = rect.X;
                askedLoc.Y = rect.Bottom + 5;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = mPopup._control.PointToScreen(askedLoc);
                if (mNormalPos.X + rW > workingArea.Width) mNormalPos.X = (int)(mNormalPos.X - (rW - rect.Width));
                if (mNormalPos.Y + rH > workingArea.Height) mNormalPos.Y = (int)(mNormalPos.Y - (rH + rect.Height + 10));
            }
            /// <summary>
            /// Timer event handler.
            /// </summary>
            private void showing(object sender, EventArgs e) {
                if (!_closing) {
                    if (_alpha == 100) {
                        _timer.Stop();
                    } else {
                        try {
                            _alpha += 10;
                            drawTransparentBitmap();
                            setBitmap(tBitmap);
                        } catch (Exception) {
                            _timer.Stop();
                        }
                    }
                } else {
                    if (_alpha == 0) {
                        _timer.Stop();
                        _timer.Tick -= showing;
                        invokeClose();
                    } else {
                        try {
                            _alpha -= 10;
                            drawTransparentBitmap();
                            setBitmap(tBitmap);
                        } catch (Exception) {
                            _timer.Stop();
                        }
                    }
                }
            }
            internal void doClose() {
                if (mPopup._animationSpeed > 0) {
                    _closing = true;
                    _timer.Start();
                } else {
                    invokeClose();
                }
            }
            internal void invokeClose() {
                try { 
                } finally {
                    mPopup._form.Close();
                    mPopup._form = null;
                    if (mPopup._owner != null) {
                        System.Windows.Forms.Form pForm = mPopup._owner.FindForm();
                        if (pForm != null) pForm.RemoveOwnedForm(this);
                    } else {
                        if (mPopup._control != null) {
                            System.Windows.Forms.Form pForm = mPopup._control.FindForm();
                            if (pForm != null) pForm.RemoveOwnedForm(this);
                        }
                    }
                }
            }
            private void autoClosing(object sender, EventArgs e) { doClose(); }
            #endregion
        }
        #endregion
        #region Private functions
        private void invokeDraw(Graphics g, Rectangle rect) {
            if (_ownerDraw || _ownerDrawBackground) {
                DrawEventArgs de = new DrawEventArgs(g, rect);
                if (Draw != null) Draw(this, de);
            } else {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                Ai.Renderer.ToolTip.drawToolTip(g, rect, tTitle, tText, tImage);
            }
        }
        private void invokeDrawBackground(Graphics g, Rectangle rect) {
            DrawEventArgs de = new DrawEventArgs(g, rect);
            if (DrawBackground != null) DrawBackground(this, de);
        }
        private void setObjectEvent(object obj) {
            if (hasToolTip(obj)) {
                if (!_objEvents.Contains(obj)) {
                    if (obj is System.Windows.Forms.Control) {
                        System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                        ctrl.MouseEnter += new EventHandler(ctrlMouseEnter);
                        ctrl.MouseLeave += new EventHandler(ctrlMouseLeave);
                        ctrl.MouseDown += new MouseEventHandler(ctrlMouseDown);
                        _objEvents.Add(obj);
                    } else if (obj is System.Windows.Forms.ToolStripItem) {
                        System.Windows.Forms.ToolStripItem anItem = (System.Windows.Forms.ToolStripItem)obj;
                        anItem.MouseEnter += new EventHandler(tsiMouseEnter);
                        anItem.MouseLeave += new EventHandler(ctrlMouseLeave);
                        anItem.MouseDown += new MouseEventHandler(ctrlMouseDown);
                        _objEvents.Add(obj);
                    }
                }
            } else {
                if (_objEvents.Contains(obj)) {
                    if (obj is System.Windows.Forms.Control) {
                        System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                        ctrl.MouseEnter -= new EventHandler(ctrlMouseEnter);
                        ctrl.MouseLeave -= new EventHandler(ctrlMouseLeave);
                        ctrl.MouseDown -= new MouseEventHandler(ctrlMouseDown);
                        _objEvents.Remove(obj);
                    } else if (obj is System.Windows.Forms.ToolStripItem) {
                        System.Windows.Forms.ToolStripItem anItem = (System.Windows.Forms.ToolStripItem)obj;
                        anItem.MouseEnter -= new EventHandler(tsiMouseEnter);
                        anItem.MouseLeave -= new EventHandler(ctrlMouseLeave);
                        anItem.MouseDown -= new MouseEventHandler(ctrlMouseDown);
                        _objEvents.Remove(obj);
                    }
                }
            }
        }
        private Boolean hasToolTip(object obj) {
            string tTitle = GetToolTipTitle(obj);
            string tText = GetToolTip(obj);
            Image tImage = GetToolTipImage(obj);
            return Ai.Renderer.ToolTip.containsToolTip(tTitle, tText, tImage);
        }
        #endregion
        #region Mouse Event Handler
        // Event attached to System.Windows.Forms.Control object.
        private void ctrlMouseEnter(object sender, EventArgs e) {
            _control = (System.Windows.Forms.Control)sender;
            switch (_location) { 
                case ToolTipLocation.Auto:
                    Rectangle ctrlRect = new Rectangle(0, 0, _control.Bounds.Width, _control.Bounds.Height);
                    show(_control, ctrlRect);
                    break;
                case ToolTipLocation.MousePointer:
                    show(_control);
                    break;
                case ToolTipLocation.CustomClient:
                    show(_control, _customLocation);
                    break;
                case ToolTipLocation.CustomScreen:
                    Point clientLocation = _control.PointToClient(_customLocation);
                    show(_control, clientLocation);
                    break;
            }
        }
        private void ctrlMouseLeave(object sender, EventArgs e) {
            if (sender == _control) {
                _control = null;
                hide();
            }
        }
        private void ctrlMouseDown(object sender, MouseEventArgs e) { hide(); }
        // Event attached to System.Windows.Forms.ToolStripItem object.
        private void tsiMouseEnter(object sender, EventArgs e) {
            ToolStripItem anItem = (ToolStripItem)sender;
            _control = anItem.GetCurrentParent();
            switch (_location) { 
                case ToolTipLocation.Auto:
                    Rectangle itemRect = new Rectangle(anItem.Bounds.X, 0, anItem.Bounds.Width, _control.Height - 2);
                    show(_control, itemRect);
                    break;
                case ToolTipLocation.MousePointer:
                    show(_control);
                    break;
                case ToolTipLocation.CustomClient:
                    show(_control, _customLocation);
                    break;
                case ToolTipLocation.CustomScreen:
                    Point clientLocation = _control.PointToClient(_customLocation);
                    show(_control, clientLocation);
                    break;
            }
        }
        #endregion
        #region Show ToolTip
        /// <summary>
        /// Show ToolTip with specified control.
        /// </summary>
        public void show(System.Windows.Forms.Control control) {
            ToolTipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null) _form.invokeClose();
            Size tooltipSize;
            if (_ownerDraw || _ownerDrawBackground) {
                PopupEventArgs pe = new PopupEventArgs();
                if (Popup != null) Popup(this, pe);
                tooltipSize = new Size((int)pe.Size.Width, (int)pe.Size.Height);
            } else {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Ai.Renderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new ToolTipForm(this, tooltipSize);
        }
        /// <summary>
        /// Show ToolTip with specified control and location.  The ToolTip location is relative to the control.
        /// </summary>
        public void show(System.Windows.Forms.Control control, Point location) {
            ToolTipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null) _form.invokeClose();
            Size tooltipSize;
            if (_ownerDraw || _ownerDrawBackground) {
                PopupEventArgs pe = new PopupEventArgs();
                if (Popup != null) Popup(this, pe);
                tooltipSize = new Size((int)pe.Size.Width, (int)pe.Size.Height);
            } else {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Ai.Renderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new ToolTipForm(this, tooltipSize, location);
        }
        /// <summary>
        /// Show ToolTip with specified control and rectangle area.  This area is where the tooltip must avoid to overlap.
        /// </summary>
        public void show(System.Windows.Forms.Control control, Rectangle rect) {
            ToolTipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null) _form.invokeClose();
            Size tooltipSize;
            if (_ownerDraw || _ownerDrawBackground) {
                PopupEventArgs pe = new PopupEventArgs();
                if (Popup != null) Popup(this, pe);
                tooltipSize = new Size((int)pe.Size.Width, (int)pe.Size.Height);
            } else {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Ai.Renderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new ToolTipForm(this, tooltipSize, rect);
        }
        /// <summary>
        /// Hide the ToolTip.
        /// </summary>
        public void hide() {
            try { _form.doClose(); }
            catch (Exception) { }
        }
        #endregion
        #region Extended Property for ToolTip
        [EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
            typeof(System.Drawing.Design.UITypeEditor)), DefaultValue("")]
        public string GetToolTip(object obj) {
            string tText = (string)_texts[obj];
            if (tText == null) tText = string.Empty;
            return tText;
        }
        [EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
            typeof(System.Drawing.Design.UITypeEditor))]
        public void SetToolTip(object obj, string value) {
            if (value == null) value = string.Empty;
            if (value.Length == 0) _texts.Remove(obj);
            else _texts[obj] = value;
            /*if (value.Length == 0) _texts.Remove(obj);
            else _texts.Add(obj, value);*/
            setObjectEvent(obj);
        }
        [DefaultValue("")]
        public string GetToolTipTitle(object obj) {
            string tTitle = (string)_titles[obj];
            if (tTitle == null) tTitle = string.Empty;
            return tTitle;
        }
        public void SetToolTipTitle(object obj, string value) {
            if (value == null) value = string.Empty;
            if (value.Length == 0) _titles.Remove(obj);
            else _titles[obj] = value;
            /*if (value.Length == 0) _titles.Remove(obj);
            else _titles.Add(obj, value);*/
            setObjectEvent(obj);
        }
        [DefaultValue(typeof(Image),"Null")]
        public Image GetToolTipImage(object obj) { return (Image)_images[obj]; }
        public void SetToolTipImage(object obj, Image value) {
            if (value == null) _images.Remove(obj);
            else _images[obj] = value;
            /*if (value == null) _images.Remove(obj);
            else _images.Add(obj, value);*/
            setObjectEvent(obj);
        }
        /// <summary>
        /// Implementation of System.ComponentModel.IExtenderProvider.CanExtend
        /// </summary>
        public Boolean CanExtend(object extendee) {
            if (extendee is System.Windows.Forms.Control) {
                if (extendee is System.Windows.Forms.Form) return false;
                if (extendee is System.Windows.Forms.ContainerControl) return false;
                return true;
            }
            if (extendee is System.Windows.Forms.ToolStripItem) return true;
            return false;
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _texts.Clear();
                _titles.Clear();
                _images.Clear();
                if (_form != null) _form.doClose();
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Specifies fade effect period when the tooltip is displayed or hiden, in milliseconds.
        /// </summary>
        [DefaultValue(20), Description("Specifies fade effect period when the tooltip is displayed or hiden, in milliseconds.")]
        public uint AnimationSpeed {
            get { return _animationSpeed; }
            set { _animationSpeed = value; }
        }
        /// <summary>
        /// Show the shadow effect of the tooltip.  This property is ignored when OwnerDrawBackground property is set to true.
        /// </summary>
        [DefaultValue(true), Description("Show the shadow effect of the tooltip.  This property is ignored when OwnerDrawBackground property is set to true.")]
        public Boolean ShowShadow {
            get { return _showShadow; }
            set { _showShadow = value; }
        }
        /// <summary>
        /// Period of time the ToolTip is displayed, in milliseconds.
        /// </summary>
        [DefaultValue(3000), Description("Period of time the ToolTip is displayed, in milliseconds.")]
        public uint AutoClose {
            get { return _autoClose; }
            set { _autoClose = value; }
        }
        /// <summary>
        /// Automatically close the ToolTip when the specified time in AutoClose property has been passed.
        /// </summary>
        [DefaultValue(true), Description("Automatically close the ToolTip when the specified time in AutoClose property has been passed.")]
        public Boolean EnableAutoClose {
            get { return _enableAutoClose; }
            set { _enableAutoClose = value; }
        }
        /// <summary>
        /// ToolTip surface will be manually drawn by your code.
        /// </summary>
        [DefaultValue(false), Description("ToolTip surface will be manually drawn by your code.")]
        public Boolean OwnerDraw {
            get { return _ownerDraw; }
            set { _ownerDraw = value; }
        }
        /// <summary>
        /// ToolTip background will be manually drawn by your code.
        /// If this property is set to true, the Draw and Popup event will be raised as well, 
        /// and the whole ToolTip will be drawn by your code.
        /// </summary>
        [DefaultValue(false), Description("ToolTip background will be manually drawn by your code.")]
        public Boolean OwnerDrawBackground {
            get { return _ownerDrawBackground; }
            set { _ownerDrawBackground = value; }
        }
        /// <summary>
        /// Determine how the ToolTip will be located.
        /// </summary>
        [DefaultValue(typeof(ToolTipLocation), "Auto"), Description("Determine how the ToolTip will be located.")]
        public ToolTipLocation Location {
            get { return _location; }
            set { _location = value; }
        }
        /// <summary>
        /// Custom location where the ToolTip will be displayed.
        /// Used when the Location property is set CustomScreen or CustomClient.
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0"), Description("Custom location where the ToolTip will be displayed.")]
        public Point CustomLocation {
            get { return _customLocation; }
            set { _customLocation = value; }
        }
        #endregion
    }
}