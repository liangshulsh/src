// Ai Software Control Library.
// Date Created : June 26, 2012.
// Implementation based on : http://www.codeproject.com/Articles/61485/Winforms-SkinFramework

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Ai.Control {
    #region Skin Manager
    /// <summary>
    /// Provides the functionality for skinning a form.
    /// </summary>
    public class SkinForm : Component {
        /// <summary>
        /// Provides a way to hook the window messages processed by the skinned form.
        /// </summary>
        private class FormHook : NativeWindow {
            #region Fields
            private readonly BufferedGraphicsContext _bufferContext;
            private BufferedGraphics _bufferGraphics;
            private Size _currentCacheSize;
            private SkinForm _owner = null;
            private bool _RIDRegistered = false;
            #endregion
            #region Constructor and Destrcutor
            public FormHook(SkinForm owner) {
                _owner = owner;
                _bufferContext = BufferedGraphicsManager.Current;
                _bufferGraphics = null;
            }
            ~FormHook() { unregisterHandler(); }
            #endregion
            #region Private Functions
            #region Form Event Handlers
            /// <summary>
            /// Called when the handle of the form is created.
            /// </summary>
            private void form_HandleCreated(object sender, EventArgs e) {
                AssignHandle(((Form)sender).Handle);
                // Registring form for raw input
                Win32API.RAWINPUTDEVICE[] rid = new Win32API.RAWINPUTDEVICE[1];
                rid[0].usUsagePage = 0x01;
                rid[0].usUsage = 0x06;
                rid[0].dwFlags = Win32API.RIDEV_INPUTSINK;
                rid[0].hwndTarget = ((Form)sender).Handle;
                _RIDRegistered = Win32API.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0]));
                if (isProcessNCArea()) {
                    updateStyle();
                    updateCaption();
                }
            }
            /// <summary>
            /// Called when the handle of the form is destroyed.
            /// </summary>
            private void form_HandleDestroyed(object sender, EventArgs e) { ReleaseHandle(); }
            /// <summary>
            /// Called when the parent of the form is disposed.
            /// </summary>
            private void form_Disposed(object sender, EventArgs e) {
                if (_owner._form != null) unregisterHandler();
            }
            /// <summary>
            /// Called when the text on the form has changed.
            /// </summary>
            private void form_TextChanged(object sender, EventArgs e) {
                if (_owner._skin != null) _owner._skin.onFormTextChanged();
            }
            #endregion
            /// <summary>
            /// Registers form event handlers.
            /// </summary>
            private void registerHandler() {
                if (_owner._form != null) {
                    _owner._form.HandleCreated += form_HandleCreated;
                    _owner._form.HandleDestroyed += form_HandleDestroyed;
                    _owner._form.Disposed += form_Disposed;
                    _owner._form.TextChanged += form_TextChanged;
                }
            }
            /// <summary>
            /// Unregister form event handlers.
            /// </summary>
            private void unregisterHandler() {
                ReleaseHandle();
                if (_owner._form != null) {
                    _owner._form.HandleCreated -= form_HandleCreated;
                    _owner._form.Disposed -= form_Disposed;
                    _owner._form.TextChanged -= form_TextChanged;
                }
            }
            /// <summary>
            /// Gets a value indicating whether the non-client area of the form should be processed.
            /// </summary>
            private bool isProcessNCArea() {
                return !(_owner._form == null || _owner._form.MdiParent != null && 
                    _owner._form.WindowState == FormWindowState.Maximized);
            }
            /// <summary>
            /// Invokes the default window procedure associated with this window.
            /// </summary>
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            protected override void WndProc(ref Message m) {
                bool suppressOriginalMessage = false;
                switch (m.Msg) { 
                    case Win32API.WM_STYLECHANGED:
                        updateStyle();
                        if (_owner._skin != null) _owner._skin.setRegion(_owner._form.Size);
                        break;
                    #region Form Activation
                    case Win32API.WM_ACTIVATEAPP:
                        if (_owner._skin != null) _owner._skin.FormIsActive = (int)m.WParam != 0;
                        onNCPaint(true);
                        break;
                    case Win32API.WM_ACTIVATE:
                        if (_owner._skin != null) _owner._skin.FormIsActive = ((int)Win32API.WA_ACTIVE == (int)m.WParam || (int)Win32API.WA_CLICKACTIVE == (int)m.WParam);
                        onNCPaint(true);
                        break;
                    case Win32API.WM_MDIACTIVATE:
                        if (m.WParam == _owner._form.Handle) {
                            if (_owner._skin != null) _owner._skin.FormIsActive = false;
                        } else if (m.LParam == _owner._form.Handle) {
                            if (_owner._skin != null) _owner._skin.FormIsActive = true;
                        }
                        onNCPaint(true);
                        break;
                    #endregion
                    #region Mouse Events
                    case Win32API.WM_NCLBUTTONDOWN:
                    case Win32API.WM_NCRBUTTONDOWN:
                    case Win32API.WM_NCMBUTTONDOWN:
                        suppressOriginalMessage = onNCMouseDown(ref m);
                        break;
                    case Win32API.WM_NCLBUTTONUP:
                    case Win32API.WM_NCMBUTTONUP:
                    case Win32API.WM_NCRBUTTONUP:
                        suppressOriginalMessage = onNCMouseUp(ref m);
                        break;
                    case Win32API.WM_NCMOUSEMOVE:
                        suppressOriginalMessage = onNCMouseMove(ref m);
                        break;
                    case Win32API.WM_NCMOUSELEAVE:
                    case Win32API.WM_MOUSELEAVE:
                    case Win32API.WM_MOUSEHOVER:
                        _owner._skin.onMouseLeave();
                        break;
                    case Win32API.WM_NCLBUTTONDBLCLK:
                        suppressOriginalMessage = onNCDoubleClick(ref m);
                        break;
                    #endregion
                    #region Non-client Hit Test
                    case Win32API.WM_NCHITTEST:
                        suppressOriginalMessage = onNCHitTest(ref m);
                        break;
                    #endregion
                    #region Painting and sizing operation
                    case Win32API.WM_NCPAINT:
                        if (onNCPaint(true)) {
                            m.Result = (IntPtr)1;
                            suppressOriginalMessage = true;
                        }
                        break;
                    case Win32API.WM_NCCALCSIZE:
                        if (m.WParam == (IntPtr)1) {
                            if (!isProcessNCArea()) break;
                            Win32API.NCCALCSIZE_PARAMS p = (Win32API.NCCALCSIZE_PARAMS)m.GetLParam(typeof(Win32API.NCCALCSIZE_PARAMS));
                            if (_owner._skin != null) p = _owner._skin.calculateNonClient(p);
                            Marshal.StructureToPtr(p, m.LParam, true);
                            suppressOriginalMessage = true;
                        }
                        break;
                    case Win32API.WM_SHOWWINDOW:
                        if (_owner._skin != null) _owner._skin.setRegion(_owner._form.Size);
                        break;
                    case Win32API.WM_SIZE:
                        onResize(m);
                        break;
                    case Win32API.WM_GETMINMAXINFO:
                        suppressOriginalMessage = calculateMaximumSize(ref m);
                        break;
                    case Win32API.WM_WINDOWPOSCHANGING:
                        Win32API.WINDOWPOS wndPos = (Win32API.WINDOWPOS)m.GetLParam(typeof(Win32API.WINDOWPOS));
                        if ((wndPos.flags & Win32API.SWP_NOSIZE) == 0) {
                            if (_owner._skin != null) _owner._skin.setRegion(new Size(wndPos.cx, wndPos.cy));
                        }
                        break;
                    case Win32API.WM_WINDOWPOSCHANGED:
                        if (_owner._form.WindowState == FormWindowState.Maximized) _owner._form.Region = null;
                        Win32API.WINDOWPOS wndPos2 = (Win32API.WINDOWPOS)m.GetLParam(typeof(Win32API.WINDOWPOS));
                        if ((wndPos2.flags & (int)Win32API.SWP_NOSIZE) == 0) {
                            updateCaption();
                            onNCPaint(true);
                        }
                        break;
                    #endregion
                    #region Raw Input
                    case Win32API.WM_INPUT:
                        if (_owner._skin != null) {
                            if (_owner._skin.FormIsActive) {
                                uint dwSize = 0, receivedBytes;
                                uint szRIHeader = (uint)Marshal.SizeOf(typeof(Win32API.RAWINPUTHEADER));
                                int res = Win32API.GetRawInputData(m.LParam, Win32API.RID_INPUT, IntPtr.Zero, ref dwSize, szRIHeader);
                                if (res == 0) {
                                    IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                                    if (buffer != IntPtr.Zero) {
                                        receivedBytes = (uint)Win32API.GetRawInputData(m.LParam, Win32API.RID_INPUT, buffer, ref dwSize, szRIHeader);
                                        Win32API.RAWINPUT raw = (Win32API.RAWINPUT)Marshal.PtrToStructure(buffer, typeof(Win32API.RAWINPUT));
                                        if (raw.header.dwType == Win32API.RIM_TYPEKEYBOARD) {
                                            // Process keyboard event.
                                            if (raw.keyboard.Message == Win32API.WM_KEYDOWN || raw.keyboard.Message == Win32API.WM_SYSKEYDOWN) {
                                                ushort key = raw.keyboard.VKey;
                                                Keys kd = (Keys)Enum.Parse(typeof(Keys), Enum.GetName(typeof(Keys), key));
                                                if (kd != System.Windows.Forms.Control.ModifierKeys) kd = kd | System.Windows.Forms.Control.ModifierKeys;
                                                // Call skin's onKeyDown function.
                                                KeyEventArgs ke = new KeyEventArgs(kd);
                                                suppressOriginalMessage = _owner._skin.onKeyDown(ke);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                }
                if(!suppressOriginalMessage) base.WndProc(ref m);
            }
            /// <summary>
            /// Called when the form is resized.
            /// </summary>
            private void onResize(Message m) {
                if (_owner._skin == null) return;
                updateCaption();
                // Update form style
                if (_owner._form.MdiParent != null) {
                    if ((int)m.WParam == 0) updateStyle();
                    if ((int)m.WParam == 2) _owner._form.Refresh();
                }
                // Update form region
                bool wasMinMax = (_owner._form.WindowState == FormWindowState.Maximized || 
                    _owner._form.WindowState == FormWindowState.Minimized);
                Win32API.RECT wndRect = new Win32API.RECT();
                Win32API.GetWindowRect(_owner._form.Handle, ref wndRect);
                Rectangle rect = new Rectangle(wndRect.Left, wndRect.Top, wndRect.Right - wndRect.Left, wndRect.Bottom - wndRect.Top - 1);
                if (wasMinMax && _owner._form.WindowState == FormWindowState.Normal && 
                    rect.Size == _owner._form.RestoreBounds.Size) {
                    _owner._skin.setRegion(new Size(wndRect.Right - wndRect.Left, wndRect.Bottom - wndRect.Top));
                    onNCPaint(true);
                }
            }
            /// <summary>
            /// Called when the mouse pointer is moved over the non-client area of the form.
            /// </summary>
            private bool onNCMouseMove(ref Message m) {
                if (_owner._skin == null) return false;
                // Prepare the MouseEventArgs data for mouse move event.
                Point pMove = new Point(m.LParam.ToInt32());
                Win32API.RECT rFormMove = new Win32API.RECT();
                Win32API.GetWindowRect(_owner._form.Handle, ref rFormMove);
                pMove.X -= rFormMove.X;
                pMove.Y -= rFormMove.Y;
                MouseEventArgs meMove = new MouseEventArgs(MouseButtons.None, 0, pMove.X, pMove.Y, 0);
                // Calls the mouse up event handler of the skin to handle the mouse move event of the non-client area.
                return _owner._skin.onMouseMove(meMove);
            }
            /// <summary>
            /// Called when the mouse button is pressed over the non-client area of the form.
            /// </summary>
            private bool onNCMouseDown(ref Message m) {
                if (_owner._skin == null) return false;
                // Prepare the MouseEventArgs data for mouse down event.
                Point pDown = new Point(m.LParam.ToInt32());
                Win32API.RECT rFormDown = new Win32API.RECT();
                Win32API.GetWindowRect(_owner._form.Handle, ref rFormDown);
                pDown.X -= rFormDown.X;
                pDown.Y -= rFormDown.Y;
                MouseButtons bDown = MouseButtons.None;
                if (m.Msg == Win32API.WM_NCLBUTTONDOWN) bDown = MouseButtons.Left;
                if (m.Msg == Win32API.WM_NCRBUTTONDOWN) bDown = bDown | MouseButtons.Right;
                if (m.Msg == Win32API.WM_NCMBUTTONDOWN) bDown = bDown | MouseButtons.Middle;
                MouseEventArgs meDown = new MouseEventArgs(bDown, 0, pDown.X, pDown.Y, 0);
                // Calls the mouse down event handler of the skin to handle the mouse down event of the non-client area.
                if (_owner._skin.onMouseDown(meDown)) {
                    m.Result = (IntPtr)1;
                    return true;
                } else return false;
            }
            /// <summary>
            /// Called when the mouse button is released.
            /// </summary>
            private bool onNCMouseUp(ref Message m) {
                if (_owner._skin == null) return false;
                // Prepare the MouseEventArgs data for mouse up event.
                Point pUp = new Point(m.LParam.ToInt32());
                Win32API.RECT rFormUp = new Win32API.RECT();
                Win32API.GetWindowRect(_owner._form.Handle, ref rFormUp);
                pUp.X -= rFormUp.X;
                pUp.Y -= rFormUp.Y;
                MouseButtons bUp = MouseButtons.None;
                if (m.Msg == Win32API.WM_NCLBUTTONUP) bUp = MouseButtons.Left;
                if (m.Msg == Win32API.WM_NCRBUTTONUP) bUp = bUp | MouseButtons.Right;
                if (m.Msg == Win32API.WM_NCMBUTTONUP) bUp = bUp | MouseButtons.Middle;
                MouseEventArgs meUp = new MouseEventArgs(bUp, 0, pUp.X, pUp.Y, 0);
                // Calls the mouse up event handler of the skin to handle the mouse up event of the non-client area.
                if (_owner._skin.onMouseUp(meUp)) {
                    m.Result = (IntPtr)1;
                    return true;
                } else return false;
            }
            /// <summary>
            /// Called when the left button of the mouse is double-clicked over the non-client area of the form.
            /// </summary>
            private bool onNCDoubleClick(ref Message m) {
                if (_owner._skin == null) return false;
                if ((int)m.WParam == Win32API.HTCAPTION) return false;
                else return _owner._skin.onDoubleClick();
            }
            /// <summary>
            /// Called when the maximum size of the form is need to be calculated.
            /// </summary>
            private bool calculateMaximumSize(ref Message m) {
                if (_owner._form.Parent == null) {
                    // create minMax info for maximize data
                    Win32API.MINMAXINFO info = (Win32API.MINMAXINFO)m.GetLParam(typeof(Win32API.MINMAXINFO));
                    Rectangle rect = SystemInformation.WorkingArea;
                    Size fullBorderSize = new Size(SystemInformation.Border3DSize.Width + SystemInformation.BorderSize.Width,
                        SystemInformation.Border3DSize.Height + SystemInformation.BorderSize.Height);

                    info.ptMaxPosition.x = rect.Left - fullBorderSize.Width;
                    info.ptMaxPosition.y = rect.Top - fullBorderSize.Height;
                    info.ptMaxSize.x = rect.Width + fullBorderSize.Width * 2;
                    info.ptMaxSize.y = rect.Height + fullBorderSize.Height * 2;

                    info.ptMinTrackSize.y += SkinBase.getCaptionHeight(_owner._form);


                    if (!_owner._form.MaximumSize.IsEmpty) {
                        info.ptMaxSize.x = Math.Min(info.ptMaxSize.x, _owner._form.MaximumSize.Width);
                        info.ptMaxSize.y = Math.Min(info.ptMaxSize.y, _owner._form.MaximumSize.Height);
                        info.ptMaxTrackSize.x = Math.Min(info.ptMaxTrackSize.x, _owner._form.MaximumSize.Width);
                        info.ptMaxTrackSize.y = Math.Min(info.ptMaxTrackSize.y, _owner._form.MaximumSize.Height);
                    }

                    if (!_owner._form.MinimumSize.IsEmpty) {
                        info.ptMinTrackSize.x = Math.Max(info.ptMinTrackSize.x, _owner._form.MinimumSize.Width);
                        info.ptMinTrackSize.y = Math.Max(info.ptMinTrackSize.y, _owner._form.MinimumSize.Height);
                    }

                    // set wished maximize size
                    Marshal.StructureToPtr(info, m.LParam, true);

                    m.Result = (IntPtr)0;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Called when the form-style need to be updated.
            /// </summary>
            private void updateStyle() {
                // remove the border style
                int currentStyle = Win32API.GetWindowLong(Handle, Win32API.GWL_STYLE);
                if ((currentStyle & Win32API.WS_BORDER) != 0) {
                    currentStyle &= ~Win32API.WS_BORDER;
                    Win32API.SetWindowLong(_owner._form.Handle, Win32API.GWL_STYLE, currentStyle);
                    Win32API.SetWindowPos(_owner._form.Handle, (IntPtr)0, -1, -1, -1, -1, 
                        (int)(Win32API.SWP_NOZORDER | Win32API.SWP_NOSIZE | Win32API.SWP_NOMOVE | 
                        Win32API.SWP_FRAMECHANGED | Win32API.SWP_NOREDRAW | Win32API.SWP_NOACTIVATE));
                }
            }
            /// <summary>
            /// Called when the caption form and all its component need to be updated.
            /// </summary>
            private void updateCaption() {
                if (_owner._skin == null) return;
                Win32API.RECT rc = new Win32API.RECT();
                Win32API.GetWindowRect(_owner._form.Handle, ref rc);
                Rectangle rect = new Rectangle(0, 0, rc.Width, rc.Height);
                _owner._skin.updateBar(rect);
            }
            /// <summary>
            /// Called when the non-client area of the form need to be repainted.
            /// </summary>
            private bool onNCPaint(bool updateBuffer) {
                if (_owner._skin == null) return false;
                if (!isProcessNCArea()) return false;
                bool result = false;
                IntPtr hdc = (IntPtr)0;
                Graphics g = null;
                try {
                    if (_owner._form.MdiParent != null && _owner._form.WindowState == FormWindowState.Maximized) {
                        _currentCacheSize = Size.Empty;
                        return false;
                    }
                    // Gets the form's bound.
                    Win32API.RECT rectScreen = new Win32API.RECT();
                    Win32API.GetWindowRect(_owner._form.Handle, ref rectScreen);
                    Rectangle rectBounds = rectScreen.ToRectangle();
                    rectBounds.Offset(-rectBounds.X, -rectBounds.Y);
                    // Create graphics object.
                    hdc = Win32API.GetDCEx(_owner._form.Handle, (IntPtr)0, (Win32API.DCX_CACHE | Win32API.DCX_CLIPSIBLINGS | Win32API.DCX_WINDOW));
                    g = Graphics.FromHdc(hdc);
                    // Clipping
                    Region fReg = new Region(rectBounds);
                    Rectangle rb = rectBounds;
                    rb.Y += _owner._skin._rectBar.Height;
                    rb.Height -= _owner._skin._rectBar.Height;
                    if (_owner._form.WindowState != FormWindowState.Maximized) {
                        rb.X += _owner._skin._rectBorderLeft.Width;
                        rb.Width -= _owner._skin._rectBorderLeft.Width + _owner._skin._rectBorderRight.Width;
                        rb.Height -= _owner._skin._rectBorderBottom.Height;
                    }
                    fReg.Exclude(rb);
                    IntPtr hReg = fReg.GetHrgn(g);
                    Win32API.SelectClipRgn(hdc, hReg);
                    // create new buffered graphics if needed
                    if (_bufferGraphics == null || _currentCacheSize != rectBounds.Size) {
                        if (_bufferGraphics != null) _bufferGraphics.Dispose();
                        _bufferGraphics = _bufferContext.Allocate(g, new Rectangle(0, 0, rectBounds.Width, rectBounds.Height));
                        _currentCacheSize = rectBounds.Size;
                        updateBuffer = true;
                    }
                    if (updateBuffer) {
                        PaintEventArgs pe = new PaintEventArgs(_bufferGraphics.Graphics, rectBounds);
                        result = _owner._skin.onPaint(pe);
                    }
                    // Render buffered graphics
                    if (_bufferGraphics != null) _bufferGraphics.Render(g);
                } catch (Exception) {
                    result = false;
                }
                // Cleaning up
                if (hdc != (IntPtr)0) {
                    Win32API.SelectClipRgn(hdc, (IntPtr)0);
                    Win32API.ReleaseDC(_owner._form.Handle, hdc);
                }
                if (g != null) g.Dispose();
                return result;
            }
            /// <summary>
            /// Called when the hit-test is performed over the non-client area of the form.
            /// </summary>
            private bool onNCHitTest(ref Message m) {
                if (_owner._skin == null) return false;
                if (!isProcessNCArea()) return false;
                Point p = new Point(m.LParam.ToInt32());
                Rectangle rect = SkinBase.getScreenRect(_owner._form);
                p.X -= rect.X;
                p.Y -= rect.Y;
                m.Result = (IntPtr)_owner._skin.nonClientHitTest(p);
                return true;
            }
            #endregion
            #region Public Functions
            public void attachForm() {
                if (_owner._form != null) {
                    if (_owner._form.Handle != IntPtr.Zero) form_HandleCreated(_owner._form, new EventArgs());
                    registerHandler();
                }
            }
            public void detachForm() {
                unregisterHandler();
            }
            #endregion
        }
        #region Fields
        SkinBase _skin = null;
        Form _form = null;
        FormHook _hook;
        #endregion
        #region Constructor
        public SkinForm() {
            _hook = new FormHook(this);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets the skin used to skinning the form.
        /// </summary>
        public SkinBase Skin {
            get { return _skin; }
            set {
                if (_skin != value) {
                    if (value == null) _hook.detachForm();
                    _skin = value;
                    if (_skin != null) {
                        _skin.Form = _form;
                    }
                    if (_form != null && _skin != null) _form.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the form to be skinned.
        /// </summary>
        public Form Form {
            get { return _form; }
            set {
                if (_form != value) {
                    if (_form != null && !IsDesignMode) _form.Disposed -= Form_Disposed;
                    _hook.detachForm();
                    _form = value;
                    if (_skin != null) {
                        _skin.Form = _form;
                    }
                    if (_form != null && !IsDesignMode) {
                        if (_skin != null) _hook.attachForm();
                    }
                }
            }
        }
        #endregion
        #region Form Disposed event handler
        private void Form_Disposed(object sender, EventArgs e) {
            Dispose();
        }
        #endregion
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_hook != null) _hook.detachForm();
                if (_skin != null) _skin.Dispose();
            }
            base.Dispose(disposing);
        }
        internal static bool IsDesignMode { get; set; }
    }
    internal class SkinFormDesigner : ComponentDesigner {
        public override void Initialize(IComponent component) {
            SkinForm.IsDesignMode = true;
            base.Initialize(component);
        }
    }
    #endregion
    #region Base Classes
    /// <summary>
    /// Provides a base class to represent a button placed on the bar of a form.
    /// </summary>
    public abstract class BarFormButton {
        #region Protected Fields
        protected bool _enabled = true;
        protected bool _visible = true;
        protected int _hitTest = Win32API.HTNOWHERE;
        protected Image _image = null;
        protected string _tooltip = "";
        protected object _tag = null;
        protected Keys _shorcut = Keys.None;
        #endregion
        #region Public Events
        /// <summary>
        /// Occurs when the Enabled property value has changed.
        /// </summary>
        public event EventHandler<EventArgs> EnabledChanged;
        /// <summary>
        /// Occurs when the Visible property value changes.
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;
        /// <summary>
        /// Occurs when the Image property value changes.
        /// </summary>
        public event EventHandler<EventArgs> ImageChanged;
        /// <summary>
        /// Occurs when a button has been clicked.
        /// </summary>
        public event EventHandler<EventArgs> Click;
        #endregion
        public BarFormButton(int hittest) {
            _hitTest = hittest;
        }
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the button can respond to user interaction.
        /// </summary>
        public virtual bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the button is displayed.
        /// </summary>
        public virtual bool Visible {
            get { return _visible; }
            set { _visible = value; }
        }
        public int HitTest { get { return _hitTest; } }
        /// <summary>
        /// Gets or sets the image displayed in the control.
        /// </summary>
        public virtual Image Image {
            get { return _image; }
            set { _image = value; }
        }
        /// <summary>
        /// Gets or sets the tooltip displayed when the mouse hover over the button.
        /// </summary>
        public virtual string Tooltip {
            get { return _tooltip; }
            set { _tooltip = value; }
        }
        /// <summary>
        /// Gets or sets an object associated with BarFormButton.
        /// </summary>
        public virtual object Tag {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Gets or sets the shortcut key for the button.
        /// </summary>
        public virtual Keys Shortcut {
            get { return _shorcut; }
            set { _shorcut = value; }
        }
        #endregion
        #region Protected Functions
        /// <summary>
        /// Raises EnabledChanged event.
        /// </summary>
        protected virtual void onEnabledChanged() {
            if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
        }
        /// <summary>
        /// Raises VisibleChanged event.
        /// </summary>
        protected virtual void onVisibleChanged() {
            if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
        }
        /// <summary>
        /// Raises ImageChanged event.
        /// </summary>
        protected virtual void onImageChanged() {
            if (ImageChanged != null) ImageChanged(this, new EventArgs());
        }
        #endregion
        #region Public Functions
        /// <summary>
        /// Raises Click event.
        /// </summary>
        public virtual void onClick() {
            if (Click != null) Click(this, new EventArgs());
        }
        #endregion
    }
    #region Standard Button
    /// <summary>
    /// Represent a minimize button in a form.
    /// </summary>
    public class MinimizeButton : BarFormButton { public MinimizeButton() : base(Win32API.HTMINBUTTON) { } }
    /// <summary>
    /// Represent a maximize button in a form.
    /// </summary>
    public class MaximizeButton : BarFormButton { public MaximizeButton() : base(Win32API.HTMAXBUTTON) { } }
    /// <summary>
    /// Represent a close button in a form.
    /// </summary>
    public class CloseButton : BarFormButton { public CloseButton() : base(Win32API.HTCLOSE) { } }
    /// <summary>
    /// Represent a custom on form's caption area.
    /// </summary>
    public class CustomButton : BarFormButton { public CustomButton() : base(Win32API.HTOBJECT) { } }
    #endregion
    /// <summary>
    /// Provides a base class for skim used in a form.
    /// Inherit this class to create custom skin, and passed the skin to the Skin property of the SkinForm component.
    /// </summary>
    public abstract class SkinBase : IDisposable {
        Form _form = null;
        #region Protected Fields
        protected MinimizeButton _minimizeButton = new MinimizeButton();
        protected MaximizeButton _maximizeButton = new MaximizeButton();
        protected CloseButton _closeButton = new CloseButton();
        protected bool _formIsActive = true;
        #region Starndard Rectangle for Non-client area
        protected Rectangle _rectClient;
        protected Rectangle _rectIcon;
        protected internal Rectangle _rectBar;
        protected Rectangle _rectBorderTop;
        protected internal Rectangle _rectBorderLeft;
        protected internal Rectangle _rectBorderBottom;
        protected internal Rectangle _rectBorderRight;
        protected Rectangle _rectBorderTopLeft;
        protected Rectangle _rectBorderTopRight;
        protected Rectangle _rectBorderBottomLeft;
        protected Rectangle _rectBorderBottomRight;
        #endregion
        #endregion
        public SkinBase() { 
            // Attaching event for standard system button.
            _minimizeButton.Click += minButton_Click;
            _maximizeButton.Click += maxButton_Click;
            _closeButton.Click += closeButton_Click;
        }
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the form is an active form.
        /// </summary>
        public virtual bool FormIsActive {
            get { return _formIsActive; }
            set { _formIsActive = value; }
        }
        /// <summary>
        /// Gets the minimize button of the form.
        /// </summary>
        public virtual MinimizeButton MinimizeButton { get { return _minimizeButton; } }
        /// <summary>
        /// Gets the maximize button of the form.
        /// </summary>
        public virtual MaximizeButton MaximizeBUtton { get { return _maximizeButton; } }
        /// <summary>
        /// Gets the close button of the form.
        /// </summary>
        public virtual CloseButton CloseBUtton { get { return _closeButton; } }
        /// <summary>
        /// Gets the skinned form.
        /// </summary>
        public virtual Form Form { 
            get { return _form; }
            internal set { _form = value; }
        }
        /// <summary>
        /// Gets the client rectangle of the form.
        /// </summary>
        public virtual Rectangle ClientRectangle { get { return _rectClient; } }
        #endregion
        #region Static Functions
        /// <summary>
        /// Gets a value indicating if the maximize box needs to be drawn on the specified form.
        /// </summary>
        public static bool drawMaximizeBox(Form form) {
            return form.MaximizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow && 
                form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }
        /// <summary>
        /// Gets a value indicating if the minimize box needs to be drawn on the specified form.
        /// </summary>
        public static bool drawMinimizeBox(Form form) {
            return form.MinimizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow && 
                form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }
        /// <summary>
        /// Calculates the border size for the given form.
        /// </summary>
        public static Size getBorderSize(Form form) {
            Size border = new Size(0, 0);

            // Check for Caption
            int style = Win32API.GetWindowLong(form.Handle, Win32API.GWL_STYLE);
            bool caption = (style & (int)(Win32API.WS_CAPTION)) != 0;
            int factor = SystemInformation.BorderMultiplierFactor - 1;

            OperatingSystem system = Environment.OSVersion;
            bool isVista = system.Version.Major >= 6 && VisualStyleInformation.IsEnabledByUser;

            switch (form.FormBorderStyle) {
                case FormBorderStyle.FixedToolWindow:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.FixedDialog:
                    border = SystemInformation.FixedFrameBorderSize;
                    break;
                case FormBorderStyle.SizableToolWindow:
                case FormBorderStyle.Sizable:
                    if (isVista)
                        border = SystemInformation.FrameBorderSize;
                    else
                        border = SystemInformation.FixedFrameBorderSize +
                            (caption ? SystemInformation.BorderSize + new Size(factor, factor)
                                : new Size(factor, factor));
                    break;
                case FormBorderStyle.Fixed3D:
                    border = SystemInformation.FixedFrameBorderSize + SystemInformation.Border3DSize;
                    break;
            }

            return border;
        }
        /// <summary>
        /// Gets the size for the given form.
        /// </summary>
        public static Size getCaptionButtonSize(Form form) {
            Size buttonSize = form.FormBorderStyle != FormBorderStyle.SizableToolWindow && 
                form.FormBorderStyle != FormBorderStyle.FixedToolWindow ? SystemInformation.CaptionButtonSize 
                : SystemInformation.ToolWindowCaptionButtonSize;
            // looks better with this height
            buttonSize.Height--;
            return buttonSize;
        }
        /// <summary>
        /// Gets the height of the caption.
        /// </summary>
        public static int getCaptionHeight(Form form) {
            return form.FormBorderStyle != FormBorderStyle.SizableToolWindow && 
                form.FormBorderStyle != FormBorderStyle.FixedToolWindow ? SystemInformation.CaptionHeight + 2 
                : SystemInformation.ToolWindowCaptionHeight + 1;
        }
        /// <summary>
        /// Gets a value indicating whether the given form has a system menu.
        /// </summary>
        public static bool hasMenu(Form form) {
            return form.FormBorderStyle == FormBorderStyle.Sizable || 
                form.FormBorderStyle == FormBorderStyle.Fixed3D || 
                form.FormBorderStyle == FormBorderStyle.FixedSingle;
        }
        /// <summary>
        /// Gets the screen rect of the given form.
        /// </summary>
        public static Rectangle getScreenRect(Form form) {
            return (form.Parent != null) ? form.Parent.RectangleToScreen(form.Bounds) : form.Bounds;
        }
        #endregion
        #region Internal Functions
        /// <summary>
        /// Called when the text property of the form has been changed.
        /// </summary>
        protected internal abstract void onFormTextChanged();
        /// <summary>
        /// Called when the left button of the mouse is double-clicked on the non-client area of the form.
        /// </summary>
        protected internal abstract bool onDoubleClick();
        /// <summary>
        /// Called when the mouse pointer is moved over the non-client area of the form.
        /// </summary>
        protected internal abstract bool onMouseMove(MouseEventArgs e);
        /// <summary>
        /// Called when the mouse pointer is over the non-client area of the form and a mouse button is pressed.
        /// </summary>
        protected internal abstract bool onMouseDown(MouseEventArgs e);
        /// <summary>
        /// Called when the mouse pointer is over the non-client area of the form and a mouse button is released.
        /// </summary>
        protected internal abstract bool onMouseUp(MouseEventArgs e);
        /// <summary>
        /// Called when the mouse pointer is leaving the non-client area of the form.
        /// </summary>
        protected internal abstract bool onMouseLeave();
        /// <summary>
        /// Called when the non-client area of the form is redrawn
        /// </summary>
        protected internal abstract bool onPaint(PaintEventArgs e);
        /// <summary>
        /// Called when one of the registered keys of the skin is pressed.
        /// </summary>
        protected internal abstract bool onKeyDown(KeyEventArgs e);
        /// <summary>
        /// Called when the form need to set its region.
        /// </summary>
        protected internal abstract bool setRegion(Size size);
        /// <summary>
        /// Called when the non-client are of the form need to be calculated.
        /// </summary>
        protected internal abstract Win32API.NCCALCSIZE_PARAMS calculateNonClient(Win32API.NCCALCSIZE_PARAMS p);
        /// <summary>
        /// Called when the bar of the form is updated.
        /// </summary>
        protected internal abstract void updateBar(Rectangle rect);
        /// <summary>
        /// Called when the hit-test is performed on the non-client area of the form.
        /// </summary>
        protected internal abstract int nonClientHitTest(Point p);
        #endregion
        #region Public Functions
        /// <summary>
        /// Instructs the skinned form to paint its non-client area.
        /// </summary>
        public void paintNC() {
            if (_form != null) {
                try {
                    Win32API.SendMessage(_form.Handle, Win32API.WM_NCPAINT, 1, 0);
                } catch (Exception) { }
            }
        }
        #endregion
        #region Event Handler
        private void minButton_Click(object sender, EventArgs e) {
            if (_form != null) _form.WindowState = FormWindowState.Minimized;
        }
        private void maxButton_Click(object sender, EventArgs e) {
            if (_form != null) {
                if (_form.WindowState == FormWindowState.Maximized) _form.WindowState = FormWindowState.Normal;
                else _form.WindowState = FormWindowState.Maximized;
            }
        }
        private void closeButton_Click(object sender, EventArgs e) {
            if (_form != null) _form.Close();
        }
        #endregion
        #region IDisposable Members
        protected bool _disposed = false;
        public virtual void Dispose() {
            if (!_disposed) {
                _disposed = true;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides a collection of BarFormButton object.
    /// </summary>
    public class BarFormButtonCollection : CollectionBase {
        #region Public Events
        /// <summary>
        /// Occurs when the collection is being cleared.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Clearing;
        /// <summary>
        /// Occurs when the collection is has been cleared.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterClear;
        /// <summary>
        /// Occurs when an element being inserted into the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Inserting;
        /// <summary>
        /// Occurs when an element has been inserted into the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterInsert;
        /// <summary>
        /// Occures when an element being removed from the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Removing;
        /// <summary>
        /// Occurs when an element has been removes from the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterRemove;
        /// <summary>
        /// Occurs when the value of an element in the collection is being sets.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Setting;
        /// <summary>
        /// Occurs when the value of an element in the collection has been sets.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterSet;
        #endregion
        public BarFormButtonCollection() : base() { }
        /// <summary>
        /// Gets a BarFormButton object in the collection specified by its index.
        /// </summary>
        /// <param name="index">A zero-based index of the item in the collection.</param>
        /// <returns>A BarFormButton object if succeeded, null, otherwise.</returns>
        public BarFormButton this[int index] {
            get {
                if (index >= 0 && index < List.Count) {
                    return (BarFormButton)List[index];
                }
                return null;
            }
        }
        /// <summary>
        /// Adds a BarFormButton object into the collection.
        /// </summary>
        public BarFormButton Add(BarFormButton button) {
            if (button == null) return null;
            // System button cannot be added here.
            if (button.HitTest == Win32API.HTCLOSE || button.HitTest == Win32API.HTMINBUTTON 
                || button.HitTest == Win32API.HTMAXBUTTON) return null;
            if (!List.Contains(button)) {
                int index = List.Add(button);
                return (BarFormButton)List[index];
            }
            return null;
        }
        /// <summary>
        /// Gets a zero-based value represent the index of a BarFormButton object in the collection.
        /// </summary>
        public int IndexOf(BarFormButton button) { return List.IndexOf(button); }
        /// <summary>
        /// Determine whether a BarFormButton object is exist in the collection.
        /// </summary>
        public bool Contains(BarFormButton button) { return List.Contains(button); }
        /// <summary>
        /// Inserts a BarFormButton object into the collection at the specified index.
        /// </summary>
        public void Insert(int index, BarFormButton button) {
            if (button == null) return;
            // System button cannot be added here.
            if (button.HitTest == Win32API.HTCLOSE || button.HitTest == Win32API.HTMINBUTTON
                || button.HitTest == Win32API.HTMAXBUTTON) return;
            if (!List.Contains(button)) List.Insert(index, button);
        }
        /// <summary>
        /// Ramoves a BarFormButton object from the collection.
        /// </summary>
        public void Remove(BarFormButton button) {
            if (List.Contains(button)) List.Remove(button);
        }
        /// <summary>
        /// Adds a collection of BarFormButton object into the collection.
        /// </summary>
        public void AddRange(BarFormButtonCollection buttons) {
            foreach (BarFormButton button in buttons) this.Add(button);
        }
        #region Internal Overriden Functions
        protected override void OnValidate(object value) {
            if (!typeof(BarFormButton).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.BarFormButton", "value");
        }
        protected override void OnClear() {
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
    /// <summary>
    /// Provides a tab button object in a tabbed form.
    /// </summary>
    public sealed class TabFormButton {
        #region Fields
        Image _image = null;
        string _text = "";
        bool _selected = false;
        object _tag = null;
        // Tooltip component.
        string _toolTipTitle = "";
        string _toolTip = "";
        Image _toolTIpImage = null;
        #endregion
        #region Events
        /// <summary>
        /// Occurs when the text of the TabFormButton has been changed.
        /// </summary>
        public event EventHandler<EventArgs> TextChanged;
        /// <summary>
        /// Occurs when the Image of the TabFormButton has been changed.
        /// </summary>
        public event EventHandler<EventArgs> ImageChanged;
        /// <summary>
        /// Occurs when the Selected property of the TabFormButton has been changed.
        /// </summary>
        public event EventHandler<EventArgs> SelectedChanged;
        /// <summary>
        /// Occurs when the TabFormButton being closed.
        /// </summary>
        public event EventHandler<CancelEventArgs> TabClosing;
        /// <summary>
        /// Occurs when the TabFormButton has been closed.
        /// </summary>
        public event EventHandler<EventArgs> TabClosed;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a TabFormButton.
        /// </summary>
        public TabFormButton() { }
        /// <summary>
        /// Creates a TabFormButton with specified text.
        /// </summary>
        public TabFormButton(string text) { _text = text; }
        /// <summary>
        /// Creates a TabFormButton object with specified text and image.
        /// </summary>
        public TabFormButton(string text, Image image) {
            _text = text;
            _image = image;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the text associated with TabFormButton.
        /// </summary>
        public string Text {
            get { return _text; }
            set {
                if (_text != value) {
                    _text = value;
                    if (TextChanged != null) TextChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the Image associated with TabFormButton.
        /// </summary>
        public Image Image {
            get { return _image; }
            set {
                if (_image != value) {
                    _image = value;
                    if (ImageChanged != null) ImageChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the TabFormButton is the selected tab.
        /// </summary>
        public bool Selected {
            get { return _selected; }
            set {
                if (_selected != value) {
                    _selected = value;
                    if (SelectedChanged != null) SelectedChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets an object associated with the TabFromButton.
        /// </summary>
        public object Tag {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Gets or sets the tooltip's title associated with TabFormButton.
        /// </summary>
        public string ToolTipTitle {
            get { return _toolTipTitle; }
            set { _toolTipTitle = value; }
        }
        /// <summary>
        /// Gets or sets the tooltip associated with TabFormButton.
        /// </summary>
        public string ToolTip {
            get { return _toolTip; }
            set { _toolTip = value; }
        }
        /// <summary>
        /// Gets or sets the tooltip's image associated with the TabFormButton.
        /// </summary>
        public Image ToolTipImage {
            get { return _toolTIpImage; }
            set { _toolTIpImage = value; }
        }
        #endregion
        #region Functions
        /// <summary>
        /// Closes the TabFormButton object.
        /// </summary>
        public void close() {
            CancelEventArgs ce = new CancelEventArgs(false);
            if (TabClosing != null) TabClosing(this, ce);
            if (ce.Cancel) return;
            if (TabClosed != null) TabClosed(this, new EventArgs());
        }
        #endregion
    }
    /// <summary>
    /// Provides a collection of TabFormButton object.
    /// </summary>
    public class TabFormButtonCollection : CollectionBase {
        #region Public Events
        /// <summary>
        /// Occurs when the collection is being cleared.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Clearing;
        /// <summary>
        /// Occurs when the collection is has been cleared.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterClear;
        /// <summary>
        /// Occurs when an element being inserted into the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Inserting;
        /// <summary>
        /// Occurs when an element has been inserted into the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterInsert;
        /// <summary>
        /// Occures when an element being removed from the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Removing;
        /// <summary>
        /// Occurs when an element has been removes from the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterRemove;
        /// <summary>
        /// Occurs when the value of an element in the collection is being sets.
        /// </summary>
        public event EventHandler<CollectionEventArgs> Setting;
        /// <summary>
        /// Occurs when the value of an element in the collection has been sets.
        /// </summary>
        public event EventHandler<CollectionEventArgs> AfterSet;
        #endregion
        public TabFormButtonCollection() : base() { }
        /// <summary>
        /// Gets a TabFormButton object in the collection specified by its index.
        /// </summary>
        /// <param name="index">A zero-based index of the item in the collection.</param>
        /// <returns>A TabFormButton object if succeeded, null, otherwise.</returns>
        public TabFormButton this[int index] {
            get {
                if (index >= 0 && index < List.Count) return (TabFormButton)List[index];
                return null;
            }
        }
        /// <summary>
        /// Adds a TabFormButton object into the collection.
        /// </summary>
        public TabFormButton Add(TabFormButton tab) {
            if (!List.Contains(tab)) {
                int index = List.Add(tab);
                return (TabFormButton)List[index];
            }
            return null;
        }
        /// <summary>
        /// Adds a new TabFormButton object with a specifed text into the collection.
        /// </summary>
        public TabFormButton Add(string text) {
            return this.Add(new TabFormButton(text));
        }
        /// <summary>
        /// Adds a new TabFormButton object with specified text and image into the collection.
        /// </summary>
        public TabFormButton Add(string text, Image image) {
            return this.Add(new TabFormButton(text, image));
        }
        /// <summary>
        /// Gets a zero-based value represent the index of a TabFromButton object in the collection.
        /// </summary>
        public int IndexOf(TabFormButton tab) { return List.IndexOf(tab); }
        /// <summary>
        /// Determine whether a TabFormButton object is exist in the collection.
        /// </summary>
        public bool Contains(TabFormButton tab) { return List.Contains(tab); }
        /// <summary>
        /// Insert a TabFormButton object into the collection with specified index.
        /// </summary>
        public void Insert(int index, TabFormButton tab) {
            if (!List.Contains(tab)) List.Insert(index, tab);
        }
        /// <summary>
        /// Removes a TabFormButton object form the collection.
        /// </summary>
        public void Remove(TabFormButton tab) {
            if (List.Contains(tab)) List.Remove(tab);
        }
        /// <summary>
        /// Adds a collection of TabFormButton object into the collection.
        /// </summary>
        public void AddRange(TabFormButtonCollection tabs) {
            foreach (TabFormButton tab in tabs) this.Add(tab);
        }
        #region Internal Overriden Functions
        protected override void OnValidate(object value) {
            if (!typeof(TabFormButton).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.TabFormButton", "value");
        }
        protected override void OnClear() {
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
    #region Skin Implementation
    /// <summary>
    /// Custom implementation of SkinBase class.
    /// </summary>
    public sealed class AiSkin : SkinBase {
        public enum ColorTheme { 
            Blue,
            BlackBlue
        }
        private const int MIN_TAB_WIDTH = 75;
        private const int MAX_TAB_WIDTH = 300;
        private const int TAB_HEIGHT = 23;
        private const int BUTTON_HEIGHT = 20;
        private const int BUTTON_WIDTH = 30;
        private const int CLOSE_WIDTH = 50;
        #region Objects Host
        private class ButtonHost {
            BarFormButton _button;
            AiSkin _owner;
            Rectangle _rect;
            bool _hover = false;
            bool _pressed = false;
            public ButtonHost(AiSkin owner, BarFormButton button) {
                _owner = owner;
                _button = button;
                _rect.Width = BUTTON_WIDTH;
                _rect.Height = BUTTON_HEIGHT;
            }
            public BarFormButton Button { get { return _button; } }
            public AiSkin Owner { get { return _owner; } }
            public Rectangle Bounds { get { return _rect; } }
            public bool Hover {
                get { return _hover; }
                set { _hover = value; }
            }
            public bool Pressed {
                get { return _pressed; }
                set { _pressed = value; }
            }
            public int X {
                get { return _rect.X; }
                set { _rect.X = value; }
            }
            public int Y {
                get { return _rect.Y; }
                set { _rect.Y = value; }
            }
            public int Width {
                get { return _rect.Width; }
                set { _rect.Width = value; }
            }
            public int Height {
                get { return _rect.Height; }
                set { _rect.Height = value; }
            }
            public Point Location {
                get { return _rect.Location; }
                set { _rect.Location = value; }
            }
            public Size Size {
                get { return _rect.Size; }
                set { _rect.Size = value; }
            }
            public bool mouseMove(MouseEventArgs e) {
                if (!(_button.Enabled && _button.Visible)) return false;
                bool changed = false;
                if (_rect.Contains(e.Location)) {
                    if (!_hover) {
                        if (_owner._hoverHost != this) {
                            if (_owner._hoverHost != null) {
                                if (_owner._hoverHost is ButtonHost) {
                                    ButtonHost host = (ButtonHost)_owner._hoverHost;
                                    host.Hover = false;
                                }
                                if (_owner._hoverHost is TabHost) {
                                    TabHost tab = (TabHost)_owner._hoverHost;
                                    tab.Hover = false;
                                    tab.HoverClose = false;
                                }
                            }
                        }
                        _hover = true;
                        _owner._hoverHost = this;
                        changed = true;
                        if (_button.Tooltip != "") {
                            _owner._currentToolTipTitle = "";
                            _owner._currentToolTipImage = null;
                            _owner._currentToolTip = _button.Tooltip;
                            _owner.showToolTip(_rect);
                        }
                    }
                } else {
                    if (_hover) {
                        _hover = false;
                        if (_owner._hoverHost == this) {
                            _owner._hoverHost = null;
                            _owner._toolTip.hide();
                        }
                        changed = true;
                    }
                }
                return changed;
            }
            public bool mouseDown(MouseEventArgs e) {
                if (!(_button.Enabled && _button.Visible)) return false;
                bool changed = false;
                if (e.Button == MouseButtons.Left) {
                    if (_hover) {
                        if (!_pressed) {
                            if (_owner._pressedHost != this) {
                                if (_owner._pressedHost != null) {
                                    if (_owner._pressedHost is ButtonHost) {
                                        ButtonHost host = (ButtonHost)_owner._pressedHost;
                                        host._pressed = false;
                                    }
                                    if (_owner._pressedHost is TabHost) {
                                        TabHost tab = (TabHost)_owner._pressedHost;
                                        tab.Pressed = false;
                                        tab.PressedClose = false;
                                    }
                                }
                            }
                            _pressed = true;
                            changed = true;
                            _owner._pressedHost = this;
                            _owner._toolTip.hide();
                        }
                    }
                }
                return changed;
            }
            public bool mouseUp(MouseEventArgs e) {
                if (!(_button.Enabled && _button.Visible)) return false;
                bool changed = false;
                if (_pressed) {
                    _pressed = false;
                    changed = true;
                    _owner._pressedHost = null;
                    if (e.Button == MouseButtons.Left && _hover) _button.onClick();
                }
                return changed;
            }
            public bool mouseLeave() {
                if (!(_button.Enabled && _button.Visible)) return false;
                bool changed = false;
                if (_hover) {
                    _hover = false;
                    if (_owner._hoverHost == this) _owner._hoverHost = null;
                    _owner._toolTip.hide();
                    changed = true;
                }
                return changed;
            }
            public void draw(Graphics g, int leftRound, int rightRound) {
                if (!_button.Visible) return;
                // Draw the button background.
                LinearGradientBrush bg = new LinearGradientBrush(_rect, Color.White, Color.Black, LinearGradientMode.Vertical);
                GraphicsPath gp;
                Pen border;
                gp = Renderer.Drawing.roundedRectangle(_rect, 0, 0, (uint)leftRound, (uint)rightRound);
                if (_button.Enabled) {
                    if (_pressed && _hover) {
                        if (_button.HitTest == Win32API.HTCLOSE) {
                            bg.InterpolationColors = _owner._closePressedBlend;
                            border = new Pen(_owner._closePressedBorder);
                        } else {
                            bg.InterpolationColors = _owner._buttonPressedBlend;
                            border = new Pen(_owner._buttonPressedBorder);
                        }
                    } else {
                        if (_hover) {
                            if (_button.HitTest == Win32API.HTCLOSE) {
                                bg.InterpolationColors = _owner._closeHLiteBlend;
                                border = new Pen(_owner._closeHLiteBorder);
                            } else {
                                bg.InterpolationColors = _owner._buttonHLiteBlend;
                                border = new Pen(_owner._buttonHLiteBorder);
                            }
                        } else {
                            if (_button.HitTest == Win32API.HTCLOSE) {
                                bg.InterpolationColors = _owner._closeNormalBlend;
                                border = new Pen(_owner._closeNormalBorder);
                            } else {
                                bg.InterpolationColors = _owner._buttonNormalBlend;
                                border = new Pen(_owner._buttonNormalBorder);
                            }
                        }
                    }
                } else {
                    if (_button.HitTest == Win32API.HTCLOSE) {
                        bg.InterpolationColors = _owner._closeDisabledBlend;
                        border = new Pen(_owner._closeDisabledBorder);
                    } else {
                        bg.InterpolationColors = _owner._buttonDisabledBlend;
                        border = new Pen(_owner._buttonDisabledBorder);
                    }
                }
                if(_owner.FormIsActive || _hover) g.FillPath(bg, gp);
                g.DrawPath(new Pen(Color.FromArgb(127, 255, 255, 255), 2), gp);
                g.DrawPath(border, gp);
                bg.Dispose();
                gp.Dispose();
                border.Dispose();
                switch (_button.HitTest) { 
                    case Win32API.HTCLOSE:
                        drawClose(g, _rect, _button.Enabled);
                        break;
                    case Win32API.HTMAXBUTTON:
                        drawMaximize(g, _rect, _button.Enabled);
                        break;
                    case Win32API.HTMINBUTTON:
                        drawMinimize(g, _rect, _button.Enabled);
                        break;
                    default:
                        if (_button.Image != null) {
                            Rectangle rectImg = Renderer.Drawing.getImageRectangle(_button.Image, _rect, 13);
                            if (_button.Enabled) g.DrawImage(_button.Image, rectImg);
                            else Renderer.Drawing.grayScaledImage(_button.Image, rectImg, g);
                        }
                        break;
                }
            }
            #region System Button Drawing
            private void drawClose(Graphics g, Rectangle rButton, bool enabled) {
                int x ,y;
                x = rButton.X + ((rButton.Width - 13) / 2);
                y = rButton.Y + ((rButton.Height - 10) / 2);
                Point[] pX = new Point[13];
                pX[0] = new Point(x, y);
                pX[1] = new Point(x + 4, y);
                pX[2] = new Point(x + 6, y + 2);
                pX[3] = new Point(x + 8, y);
                pX[4] = new Point(x + 12, y);
                pX[5] = new Point(x + 8, y + 4);
                pX[6] = new Point(x + 12, y + 9);
                pX[7] = new Point(x + 8, y + 9);
                pX[8] = new Point(x + 6, y + 6);
                pX[9] = new Point(x + 4, y + 9);
                pX[10] = new Point(x, y + 9);
                pX[11] = new Point(x + 4, y + 4);
                pX[12] = pX[0];
                Point[] pXUpper = new Point[5];
                pXUpper[0] = pX[0];
                pXUpper[1] = pX[1];
                pXUpper[2] = pX[2];
                pXUpper[3] = pX[3];
                pXUpper[4] = pX[4];
                Point[] pXLower = new Point[5];
                pXLower[0] = pX[6];
                pXLower[1] = pX[7];
                pXLower[2] = pX[8];
                pXLower[3] = pX[9];
                pXLower[4] = pX[10];
                if (enabled) {
                    g.DrawPolygon(Pens.Black, pX);
                    g.FillPolygon(Brushes.White, pX);
                } else {
                    g.DrawPolygon(Pens.DimGray, pX);
                    g.FillPolygon(Brushes.LightGray, pX);
                }
            }
            private void drawMaximize(Graphics g, Rectangle rButton, bool enabled) {
                GraphicsPath gp;
                int x, y;
                if (_owner.Form.WindowState == FormWindowState.Maximized) {
                    x = rButton.X + ((rButton.Width - 13) / 2);
                    y = rButton.Y + ((rButton.Height - 13) / 2);
                    Rectangle rectOuter = new Rectangle(x + 3, y, 10, 10);
                    Rectangle rectInner = new Rectangle(x + 6, y + 3, 4, 4);
                    gp = new GraphicsPath();
                    gp.AddRectangle(rectOuter);
                    gp.AddRectangle(rectInner);
                    if (enabled) {
                        g.DrawPath(Pens.Black, gp);
                        g.FillPath(Brushes.White, gp);
                    } else {
                        g.DrawPath(Pens.DimGray, gp);
                        g.FillPath(Brushes.LightGray, gp);
                    }
                    gp.Dispose();
                    rectOuter.X -= 3;
                    rectOuter.Y += 3;
                    rectInner.X -= 3;
                    rectInner.Y += 3;
                    gp = new GraphicsPath();
                    gp.AddRectangle(rectOuter);
                    gp.AddRectangle(rectInner);
                    if (enabled) {
                        g.DrawPath(Pens.Black, gp);
                        g.FillPath(Brushes.White, gp);
                    } else {
                        g.DrawPath(Pens.DimGray, gp);
                        g.FillPath(Brushes.LightGray, gp);
                    }
                    gp.Dispose();
                } else {
                    x = rButton.X + ((rButton.Width - 13) / 2);
                    y = rButton.Y + ((rButton.Height - 10) / 2);
                    Rectangle rectOuter = new Rectangle(x, y, 13, 10);
                    Rectangle rectInner = new Rectangle(x + 3, y + 3, 7, 4);
                    gp = new GraphicsPath();
                    gp.AddRectangle(rectOuter);
                    gp.AddRectangle(rectInner);
                    if (enabled) {
                        g.DrawPath(Pens.Black, gp);
                        g.FillPath(Brushes.White, gp);
                    } else {
                        g.DrawPath(Pens.DimGray, gp);
                        g.FillPath(Brushes.LightGray, gp);
                    }
                    gp.Dispose();
                }
            }
            private void drawMinimize(Graphics g, Rectangle rButton, bool enabled) {
                int x, y;
                x = rButton.X + ((rButton.Width - 13) / 2);
                y = rButton.Y + ((rButton.Height - 10) / 2) + 6;
                Rectangle rect = new Rectangle(x, y, 13, 4);
                if (enabled) {
                    g.DrawRectangle(Pens.Black, rect);
                    g.FillRectangle(Brushes.White, rect);
                } else {
                    g.DrawRectangle(Pens.DimGray, rect);
                    g.FillRectangle(Brushes.LightGray, rect);
                }
            }
            #endregion
        }
        private class TabHost {
            bool _hover = false;
            bool _hoverClose = false;
            bool _pressed = false;
            bool _pressedClose = false;
            TabFormButton _tab;
            Rectangle _rect;
            Rectangle _rectClose;
            AiSkin _owner = null;
            public TabHost(AiSkin owner, TabFormButton tab) {
                _owner = owner;
                _tab = tab;
                _rect = new Rectangle(0, 8, MIN_TAB_WIDTH, TAB_HEIGHT);
                _rectClose = new Rectangle(_rect.Right - 15, _rect.Top + 5, 10, 10);
            }
            #region Properties
            public int X {
                get { return _rect.X; }
                set {
                    if (_rect.X != value) {
                        int dx = value - _rect.X;
                        _rect.X = value;
                        _rectClose.X += dx;
                    }
                }
            }
            public int Y {
                get { return _rect.Y; }
                set {
                    if (_rect.Y != value) {
                        int dy = value - _rect.Y;
                        _rect.Y = value;
                        _rectClose.Y += dy;
                    }
                }
            }
            public int Width {
                get { return _rect.Width; }
                set {
                    if (_rect.Width != value) {
                        _rect.Width = value;
                        _rectClose.X = _rect.Right - 15;
                    }
                }
            }
            public int Height {
                get { return _rect.Height; }
                set { _rect.Height = value; }
            }
            public Point Location {
                get { return _rect.Location; }
                set {
                    if (_rect.Location != value) {
                        int dx = value.X - _rect.X;
                        _rect.Location = value;
                        _rectClose.X += dx;
                    }
                }
            }
            public Size Size {
                get { return _rect.Size; }
                set {
                    if (_rect.Size != value) {
                        _rect.Size = value;
                        _rectClose.X = _rect.Right - 15;
                    }
                }
            }
            public bool Hover {
                get { return _hover; }
                set { _hover = value; }
            }
            public bool HoverClose {
                get { return _hoverClose; }
                set { _hoverClose = value; }
            }
            public bool Pressed {
                get { return _pressed; }
                set { _pressed = value; }
            }
            public bool PressedClose {
                get { return _pressedClose; }
                set { _pressedClose = value; }
            }
            public TabFormButton Tab { get { return _tab; } }
            public AiSkin Owner { get { return _owner; } }
            public Rectangle Bounds { get { return _rect; } }
            #endregion
            #region Functions
            private bool needToolTip() {
                if (_tab.ToolTip != "" || _tab.ToolTipTitle != "" || _tab.ToolTipImage != null) return true;
                return false;
            }
            private GraphicsPath createTabPath() {
                GraphicsPath gp = new GraphicsPath();
                if (_tab.Selected) {
                    gp.AddArc(new Rectangle(_rect.X - 4, _rect.Bottom - 5, 4, 4), 90f, -90f);
                    gp.AddLine(_rect.X, _rect.Bottom - 3, _rect.X, _rect.Y + 3);
                } else {
                    gp.AddLine(_rect.X, _rect.Bottom - 1, _rect.X, _rect.Y + 3);
                }
                gp.AddArc(new Rectangle(_rect.X, _rect.Y, 6, 6), 180f, 90f);
                gp.AddLine(_rect.X + 3, _rect.Y, _rect.Right - 4, _rect.Y);
                gp.AddArc(new Rectangle(_rect.Right - 7, _rect.Y, 6, 6), 270f, 90f);
                if (_tab.Selected) {
                    gp.AddLine(_rect.Right - 1, _rect.Y + 3, _rect.Right - 1, _rect.Bottom - 3);
                    gp.AddArc(new Rectangle(_rect.Right - 1, _rect.Bottom - 5, 4, 4), 180f, -90f);
                } else {
                    gp.AddLine(_rect.Right - 1, _rect.Y + 3, _rect.Right - 1, _rect.Bottom - 1);
                }
                return gp;
            }
            public bool mouseMove(MouseEventArgs e) {
                bool changed = false;
                if (_rect.Contains(e.Location)) {
                    if (_rectClose.Contains(e.Location)) {
                        if (!_hoverClose) {
                            if (_owner._hoverHost != this) {
                                if (_owner._hoverHost != null) {
                                    if (_owner._hoverHost is ButtonHost) {
                                        ButtonHost button = (ButtonHost)_owner._hoverHost;
                                        button.Hover = false;
                                    }
                                }
                            }
                            _hoverClose = true;
                            _hover = true;
                            _owner._hoverHost = this;
                            if (needToolTip()) {
                                _owner._currentToolTipTitle = _tab.ToolTipTitle;
                                _owner._currentToolTip = _tab.ToolTip;
                                _owner._currentToolTipImage = _tab.ToolTipImage;
                                _owner.showToolTip(_rect);
                            }
                            changed = true;
                        }
                    } else {
                        if (_hoverClose) {
                            _hoverClose = false;
                            changed = true;
                        }
                        if (!_hover) {
                            if (_owner._hoverHost != this) {
                                if (_owner._hoverHost != null) {
                                    if (_owner._hoverHost is ButtonHost) {
                                        ButtonHost button = (ButtonHost)_owner._hoverHost;
                                        button.Hover = false;
                                    }
                                }
                            }
                            _hover = true;
                            _owner._hoverHost = this;
                            if (needToolTip()) {
                                _owner._currentToolTipTitle = _tab.ToolTipTitle;
                                _owner._currentToolTip = _tab.ToolTip;
                                _owner._currentToolTipImage = _tab.ToolTipImage;
                                _owner.showToolTip(_rect);
                            }
                            changed = true;
                        }
                    }
                } else {
                    if (_hover) {
                        _hoverClose = false;
                        _hover = false;
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                        changed = true;
                        _owner._toolTip.hide();
                    }
                }
                return changed;
            }
            public bool mouseDown(MouseEventArgs e) {
                bool changed = false;
                if (e.Button == MouseButtons.Left) {
                    if (_hover) {
                        if (_hoverClose) {
                            if (!_pressedClose) {
                                if (_owner._pressedHost != this) {
                                    if (_owner._pressedHost != null) {
                                        if (_owner._pressedHost is ButtonHost) {
                                            ButtonHost button = (ButtonHost)_owner._pressedHost;
                                            button.Pressed = false;
                                        }
                                        if (_owner._pressedHost is TabHost) {
                                            TabHost tab = (TabHost)_owner._pressedHost;
                                            tab._pressed = false;
                                            tab._pressedClose = false;
                                        }
                                    }
                                }
                                _pressedClose = true;
                                changed = true;
                                _owner._pressedHost = this;
                                _owner._toolTip.hide();
                            }
                        }
                        if (!_pressed) {
                            if (_owner._pressedHost != this) {
                                if (_owner._pressedHost != null) {
                                    if (_owner._pressedHost is ButtonHost) {
                                        ButtonHost button = (ButtonHost)_owner._pressedHost;
                                        button.Pressed = false;
                                    }
                                    if (_owner._pressedHost is TabHost) {
                                        TabHost tab = (TabHost)_owner._pressedHost;
                                        tab._pressed = false;
                                        tab._pressedClose = false;
                                    }
                                }
                            }
                            _pressed = true;
                            _owner._pressedHost = this;
                            changed = true;
                            _owner._toolTip.hide();
                        }
                    }
                }
                return changed;
            }
            public bool mouseUp(MouseEventArgs e) {
                bool changed = false;
                if (_pressed || _pressedClose) {
                    if (_hoverClose) {
                        if (_pressedClose) _tab.close();
                    } else {
                        if (_hover) {
                            if (!_tab.Selected) {
                                // Search selected tab, and deselect it if any.
                                _owner._suppressEvent = true;
                                int i = 0;
                                while (i < _owner._tabs.Count) {
                                    if (_owner._tabs[i] != _tab) {
                                        if (_owner._tabs[i].Selected) {
                                            _owner._tabs[i].Selected = false;
                                            break;
                                        }
                                    }
                                    i++;
                                }
                                _tab.Selected = true;
                                _owner._suppressEvent = false;
                            }
                        }
                    }
                    _pressed = false;
                    _pressedClose = false;
                    changed = true;
                    _owner._pressedHost = null;
                }
                return changed;
            }
            public bool mouseLeave() {
                bool changed = false;
                if (_hover || _hoverClose) {
                    _hover = false;
                    _hoverClose = false;
                    changed = true;
                    if (_owner._hoverHost == this) _owner._hoverHost = null;
                    _owner._toolTip.hide();
                }
                return changed;
            }
            public void draw(Graphics g) {
                GraphicsPath gp = createTabPath();
                if (_hover || _tab.Selected) {
                    if (_hover) {
                        Rectangle rectHLite = new Rectangle(_rect.X - 1, _rect.Y - 1, _rect.Width + 2, _rect.Height + 1);
                        LinearGradientBrush bgBorderHLite = new LinearGradientBrush(rectHLite, Color.FromArgb(127, 255, 255, 0),
                            Color.Transparent, LinearGradientMode.Vertical);
                        Pen hLitePen = new Pen(bgBorderHLite, 3f);
                        g.DrawPath(hLitePen, gp);
                        hLitePen.Dispose();
                        bgBorderHLite.Dispose();
                    } else {
                        Pen hLitePen = new Pen(Color.FromArgb(127, 255, 255, 255), 2f);
                        g.DrawPath(hLitePen, gp);
                        hLitePen.Dispose();
                    }
                    LinearGradientBrush bg = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                    if (_tab.Selected) bg.InterpolationColors = _owner._tbSelectedBlend;
                    else bg.InterpolationColors = _owner._tbHLiteBlend;
                    gp.CloseFigure();
                    g.FillPath(bg, gp);
                    bg.Dispose();
                    gp.Dispose();
                }
                // Drawing the objects.
                int x = _rect.X + 3;
                // Draw the tab image if any.
                if (_tab.Image != null) {
                    Rectangle rectImg = new Rectangle(x, _rect.Y + 3, 16, 16);
                    rectImg = Renderer.Drawing.getImageRectangle(_tab.Image, rectImg, 16);
                    g.DrawImage(_tab.Image, rectImg);
                    x = rectImg.Right + 2;
                }
                // Draw the tab text.
                SolidBrush txtBrush;
                Font f;
                if (_tab.Selected) {
                    txtBrush = new SolidBrush(_owner._tbTextSelected);
                    f = _owner._tabSelectedFont;
                } else {
                    txtBrush = new SolidBrush(_owner._tbTextNormal);
                    f = _owner._tabNormalFont;
                }
                Rectangle rectTxt = new Rectangle(x, _rect.Y, _rectClose.X - (x + 2), _rect.Height);
                g.DrawString(_tab.Text, f, txtBrush, rectTxt, _owner._sfTab);
                txtBrush.Dispose();
                // Draw the tab close sign.
                Pen penXT = null;
                if (_hoverClose || _pressedClose) {
                    if (_hoverClose && _pressedClose) {
                        penXT = new Pen(Color.FromArgb(146, 12, 18), 2f);
                    } else {
                        if (_hoverClose) penXT = new Pen(Color.FromArgb(255, 0, 0), 2f);
                    }
                }
                if (penXT == null) penXT = new Pen(Color.Black, 2f);
                Point[] px1 = new Point[2];
                Point[] px2 = new Point[2];
                px1[0] = new Point(_rectClose.X + 1, _rectClose.Y + 1);
                px1[1] = new Point(_rectClose.Right - 2, _rectClose.Bottom - 2);
                px2[0] = new Point(_rectClose.X + 1, _rectClose.Bottom - 2);
                px2[1] = new Point(_rectClose.Right - 2, _rectClose.Y + 1);
                Pen penXS = new Pen(Color.White, 3f);
                g.DrawLine(penXS, px1[0], px1[1]);
                g.DrawLine(penXS, px2[0], px2[1]);
                g.DrawLine(penXT, px1[0], px1[1]);
                g.DrawLine(penXT, px2[0], px2[1]);
                penXS.Dispose();
                penXT.Dispose();
                // Draw tha tab border
                Pen penBorder;
                gp = createTabPath();
                if (_tab.Selected) penBorder = new Pen(_owner._tbSelectedBorder);
                else penBorder = new Pen(_owner._tbNormalBorder);
                g.DrawPath(penBorder, gp);
                gp.Dispose();
                penBorder.Dispose();
            }
            #endregion
        }
        #endregion
        #region Fields
        BarFormButtonCollection _customButtons = new BarFormButtonCollection();
        TabFormButtonCollection _tabs = new TabFormButtonCollection();
        Font _textFont = new Font("Segoe UI", 9, FontStyle.Regular);
        Font _tabNormalFont = new Font("Segoe UI", 8, FontStyle.Regular);
        Font _tabSelectedFont = new Font("Segoe UI", 8, FontStyle.Bold);
        bool _isTabbedForm = false;
        #region Coloring
        // Form
        ColorBlend _formActiveBgBlend;
        ColorBlend _formInactiveBgBlend;
        Color _formActiveBorder;
        Color _formInactiveBorder;
        // Form Text
        Color _formActiveTextColor;
        Color _formInactiveTextColor;
        // Form Close Button
        ColorBlend _closeNormalBlend;
        ColorBlend _closeHLiteBlend;
        ColorBlend _closeDisabledBlend;
        ColorBlend _closePressedBlend;
        Color _closeNormalBorder;
        Color _closeHLiteBorder;
        Color _closeDisabledBorder;
        Color _closePressedBorder;
        // Form Caption Button
        ColorBlend _buttonNormalBlend;
        ColorBlend _buttonHLiteBlend;
        ColorBlend _buttonDisabledBlend;
        ColorBlend _buttonPressedBlend;
        Color _buttonNormalBorder;
        Color _buttonHLiteBorder;
        Color _buttonDisabledBorder;
        Color _buttonPressedBorder;
        // Tab Area
        ColorBlend _taActiveBlend;
        ColorBlend _taInactiveBlend;
        Color _taActiveBorder;
        Color _taInactiveBorder;
        // Tab Button
        ColorBlend _tbSelectedBlend;
        ColorBlend _tbHLiteBlend;
        Color _tbSelectedBorder;
        Color _tbNormalBorder;
        Color _tbTextSelected;
        Color _tbTextNormal;
        #endregion
        // Tooltip
        string _currentToolTip = "";
        string _currentToolTipTitle = "";
        Image _currentToolTipImage = null;
        ToolTip _toolTip;
        // Hosts
        object _hoverHost = null;
        object _pressedHost = null;
        List<ButtonHost> _buttonHosts = new List<ButtonHost>();
        List<TabHost> _tabHosts = new List<TabHost>();
        // System Button Host
        ButtonHost _minHost = null;
        ButtonHost _maxHost = null;
        ButtonHost _closeHost = null;
        // String format.
        StringFormat _sfTab;
        // Others
        bool _suppressEvent = false;
        Rectangle _rectTab;
        ColorTheme _theme = ColorTheme.Blue;
        #endregion
        public AiSkin() : base() { 
            // Applying custom measurement for bar, border, and status.
            _rectBar.Height = 30;
            _rectBorderTop.Height = 5;
            _rectBorderLeft.Width = 5;
            _rectBorderRight.Width = 5;
            _rectBorderBottom.Height = 5;
            _rectBorderTopLeft.Size = new Size(5, 5);
            _rectBorderTopRight.Size = new Size(5, 5);
            _rectBorderBottomLeft.Size = new Size(5, 5);
            _rectBorderBottomRight.Size = new Size(5, 5);
            _rectIcon = new Rectangle(0, 0, 30, 30);
            _rectBar.Location = new Point(0, 0);
            _rectBorderTopLeft.Location = new Point(0, 0);
            _rectBorderLeft.Location = new Point(0, 5);
            _rectBorderTop.Location = new Point(5, 0);
            // System Button Host
            _minimizeButton.Tooltip = "Minimize";
            _closeButton.Tooltip = "Close";
            _closeHost = new ButtonHost(this, _closeButton);
            _maxHost = new ButtonHost(this, _maximizeButton);
            _minHost = new ButtonHost(this, _minimizeButton);
            _closeHost.Width = CLOSE_WIDTH;
            _maxHost.Width = BUTTON_WIDTH;
            _minHost.Width = BUTTON_WIDTH;
            _closeHost.Height = BUTTON_HEIGHT;
            _maxHost.Height = BUTTON_HEIGHT;
            _minHost.Height = BUTTON_HEIGHT;
            // String format
            _sfTab = new StringFormat();
            _sfTab.LineAlignment = StringAlignment.Center;
            _sfTab.Alignment = StringAlignment.Near;
            _sfTab.Trimming = StringTrimming.EllipsisCharacter;
            _sfTab.FormatFlags = _sfTab.FormatFlags | StringFormatFlags.NoWrap;
            // Setting up tooltip
            _toolTip = new ToolTip();
            _toolTip.AnimationSpeed = 20;
            _toolTip.OwnerDraw = true;
            _toolTip.Draw += tooltip_Draw;
            _toolTip.Popup += tooltip_Popup;
            // Tab area
            _rectTab.Y = 5;
            _rectTab.Height = _rectBar.Height - 5;
            // Attaching event handlers
            _customButtons.AfterInsert += buttons_AfterInsert;
            _customButtons.Removing += buttons_Removing;
            _customButtons.AfterRemove += buttons_AfterRemove;
            _customButtons.Clearing += buttons_Clearing;
            _customButtons.AfterClear += buttons_AfterClear;
            _tabs.AfterInsert += tabs_AfterInsert;
            _tabs.Removing += tabs_Removing;
            _tabs.AfterRemove += tabs_AfterRemove;
            _tabs.Clearing += tabs_Clearing;
            _tabs.AfterClear += tabs_AfterClear;
            _minimizeButton.EnabledChanged += button_EnabledChanged;
            _minimizeButton.VisibleChanged += button_VisibleChanged;
            _maximizeButton.EnabledChanged += button_EnabledChanged;
            _maximizeButton.VisibleChanged += button_VisibleChanged;
            _closeButton.EnabledChanged += button_EnabledChanged;
            _closeButton.VisibleChanged += button_VisibleChanged;
            // Initialize color theme
            initBlueTheme();
        }
        #region Private Functions
        private void showToolTip(Rectangle rect) {
            if (Form != null) {
                rect.Y -= _rectBar.Height;
                rect.X -= _rectBar.X;
                _toolTip.show(Form, rect);
            }
        }
        private void arrangeObjects() {
            int x = _rectBar.Right - 2;
            if (_closeButton.Visible) {
                _closeHost.X = x - _closeHost.Width;
                x = _closeHost.X;
            }
            if (_maximizeButton.Visible) {
                _maxHost.X = x - _maxHost.Width;
                x = _maxHost.X;
            }
            if (_minimizeButton.Visible) {
                _minHost.X = x - _minHost.Width;
                x = _minHost.X;
            }
            // Updates custom button's location here...
            int i = _buttonHosts.Count - 1;
            while (i >= 0) {
                if (_buttonHosts[i].Button.Visible) {
                    _buttonHosts[i].X = x - BUTTON_WIDTH;
                    x = _buttonHosts[i].X;
                }
                i--;
            }
            arrangeTabs();
        }
        private void arrangeTabs() {
            int x = 0;
            bool found = false;
            // Gets the first visible ButtonHost.
            foreach (ButtonHost host in _buttonHosts) {
                if (host.Button.Visible) {
                    x = host.X - 5;
                    found = true;
                    break;
                }
            }
            if (!found) {
                if (_minimizeButton.Visible) {
                    x = _minHost.X - 5;
                    found = true;
                }
            }
            if (!found) {
                if (_maximizeButton.Visible) {
                    x = _maxHost.X - 5;
                    found = true;
                }
            }
            if (!found) {
                if (_closeButton.Visible) {
                    x = _closeHost.X = 5;
                    found = true;
                }
            }
            if (!found) {
                x = _rectBar.Right - 2;
            }
            // Create a temporary graphics object to measure form text
            if (Form != null) {
                Bitmap aBmp = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage(aBmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                SizeF tSize = g.MeasureString(Form.Text, _textFont);
                _rectTab.X = _rectIcon.Right + (int)tSize.Width + 5;
                _rectTab.Width = x - _rectTab.X;
                g.Dispose();
                aBmp.Dispose();
            }
            if (_tabHosts.Count == 0) return;
            int i = 0;
            x = _rectTab.X + 3;
            int tabWidth = (_rectTab.Width - 4) / _tabHosts.Count;
            if (tabWidth > MAX_TAB_WIDTH) tabWidth = MAX_TAB_WIDTH;
            else tabWidth -= 2;
            while (i < _tabHosts.Count) {
                _tabHosts[i].X = x;
                _tabHosts[i].Width = tabWidth;
                x += tabWidth + 2;
                i++;
            }
        }
        private void initBlueTheme() {
            // Form
            _formActiveBgBlend = Renderer.Button.NormalBlend();
            _formInactiveBgBlend = Renderer.Button.DisabledBlend();
            _formActiveBorder = Renderer.Button.NormalBorderPen().Color;
            _formInactiveBorder = Renderer.Button.DisabledBorderPen().Color;
            _formActiveTextColor = Color.Black;
            _formInactiveTextColor = Color.Gray;
            // Close button
            _closeNormalBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(234, 178, 169), Color.FromArgb(227, 133, 120), 
                Color.FromArgb(208, 75, 55), Color.FromArgb(218, 134, 109) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeNormalBorder = Color.FromArgb(90, 47, 39);
            _closeHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(251, 157, 139), Color.FromArgb(239, 110, 87), 
                Color.FromArgb(212, 40, 9), Color.FromArgb(164, 25, 6) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeHLiteBorder = Color.FromArgb(138, 89, 41);
            _closePressedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(211, 170, 148), Color.FromArgb(187, 120, 93), 
                Color.FromArgb(140, 32, 8), Color.FromArgb(144, 109, 33) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closePressedBorder = Color.FromArgb(149, 100, 41);
            _closeDisabledBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(230, 199, 201), Color.FromArgb(221, 172, 170), 
                Color.FromArgb(210, 138, 132), Color.FromArgb(215, 173, 165) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeDisabledBorder = Color.FromArgb(102, 64, 59);
            // Bar form button
            _buttonNormalBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(225, 234, 241), Color.FromArgb(201, 212, 222), 
                Color.FromArgb(165, 181, 196), Color.FromArgb(180, 197, 217) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonNormalBorder = Color.FromArgb(84, 90, 96);
            _buttonDisabledBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(229, 236, 244), Color.FromArgb(205, 216, 226), 
                Color.FromArgb(172, 187, 201), Color.FromArgb(186, 201, 221) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonDisabledBorder = Color.FromArgb(101, 110, 119);
            _buttonHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(184, 219, 242), Color.FromArgb(146, 197, 226), 
                Color.FromArgb(63, 143, 195), Color.FromArgb(10, 254, 251) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonHLiteBorder = Color.FromArgb(44, 133, 148);
            _buttonPressedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(162, 187, 205), Color.FromArgb(89, 165, 181), 
                Color.FromArgb(19, 116, 132), Color.FromArgb(10, 253, 250) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonPressedBorder = Color.FromArgb(32, 169, 185);
            // Tab area
            _taActiveBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(86, 125, 175), Color.FromArgb(101, 145, 205) }, new float[] { 0f, 1f });
            _taInactiveBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(88, 126, 177), Color.FromArgb(158, 189, 230) }, new float[] { 0f, 1f });
            _taActiveBorder = _formActiveBorder;
            _taInactiveBorder = _formInactiveBorder;
            // Tab Button
            _tbSelectedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(252, 253, 254), Color.FromArgb(226, 239, 251), 
                Color.FromArgb(210, 230, 250) }, new float[] { 0f, 0.5f, 1f });
            _tbHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(127, 236, 245, 252), Color.FromArgb(191, 191, 210, 229), 
                Color.FromArgb(63, 152, 180, 210) }, new float[] { 0f, 0.5f, 1f });
            _tbSelectedBorder = Color.FromArgb(105, 161, 191);
            _tbNormalBorder = Color.FromArgb(145, 150, 152);
            _tbTextNormal = _tbTextSelected = Color.Black;
        }
        private void initBlackBlueTheme() {
            // Form
            _formActiveBgBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(59, 59, 59), Color.FromArgb(0, 69, 236), 
                Color.FromArgb(0, 0, 0), Color.FromArgb(89, 89, 89) }, new float[] { 0f, 0.4f, 0.4f, 1f });
            _formInactiveBgBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(59, 59, 59), Color.FromArgb(110, 144, 226), 
                Color.FromArgb(59, 59, 59), Color.FromArgb(89, 89, 89) }, new float[] { 0f, 0.4f, 0.4f, 1f });
            _formActiveBorder = Color.FromArgb(0, 69, 236);
            _formInactiveBorder = Color.FromArgb(110, 144, 226);
            _formActiveTextColor = Color.White;
            _formInactiveTextColor = Color.LightGray;
            // Close button
            _closeNormalBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(234, 178, 169), Color.FromArgb(227, 133, 120), 
                Color.FromArgb(208, 75, 55), Color.FromArgb(218, 134, 109) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeNormalBorder = Color.FromArgb(90, 47, 39);
            _closeHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(251, 157, 139), Color.FromArgb(239, 110, 87), 
                Color.FromArgb(212, 40, 9), Color.FromArgb(164, 25, 6) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeHLiteBorder = Color.FromArgb(138, 89, 41);
            _closePressedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(211, 170, 148), Color.FromArgb(187, 120, 93), 
                Color.FromArgb(140, 32, 8), Color.FromArgb(144, 109, 33) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closePressedBorder = Color.FromArgb(149, 100, 41);
            _closeDisabledBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(230, 199, 201), Color.FromArgb(221, 172, 170), 
                Color.FromArgb(210, 138, 132), Color.FromArgb(215, 173, 165) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _closeDisabledBorder = Color.FromArgb(102, 64, 59);
            // Bar form button
            _buttonNormalBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(248, 248, 248), Color.FromArgb(121, 111, 109), 
                Color.FromArgb(0, 0, 7), Color.FromArgb(0, 144, 215) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonNormalBorder = Color.FromArgb(84, 90, 96);
            _buttonDisabledBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(255, 255, 255), Color.FromArgb(108, 100, 97), 
                Color.FromArgb(15, 14, 19), Color.FromArgb(152, 146, 148) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonDisabledBorder = Color.FromArgb(101, 110, 119);
            _buttonHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(179, 224, 255), Color.FromArgb(115, 201, 255), 
                Color.FromArgb(4, 118, 178), Color.FromArgb(61, 183, 250) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonHLiteBorder = Color.FromArgb(44, 133, 148);
            _buttonPressedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(0, 145, 208), Color.FromArgb(1, 73, 121), 
                Color.FromArgb(2, 10, 23), Color.FromArgb(10, 145, 203) }, new float[] { 0f, 0.5f, 0.5f, 1f });
            _buttonPressedBorder = Color.FromArgb(32, 169, 185);
            // Tab area
            _taActiveBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(0, 21, 64), Color.FromArgb(35, 86, 143) }, new float[] { 0f, 1f });
            _taInactiveBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(0, 125, 171), Color.FromArgb(1, 151, 210) }, new float[] { 0f, 1f });
            _taActiveBorder = _formActiveBorder;
            _taInactiveBorder = _formInactiveBorder;
            // Tab Button
            _tbSelectedBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(59, 216, 247), Color.FromArgb(30, 143, 187), 
                Color.FromArgb(0, 77, 133) }, new float[] { 0f, 0.5f, 1f });
            _tbHLiteBlend = Renderer.Drawing.createColorBlend(new Color[] { Color.FromArgb(127, 236, 245, 252), Color.FromArgb(191, 191, 210, 229), 
                Color.FromArgb(63, 152, 180, 210) }, new float[] { 0f, 0.5f, 1f });
            _tbSelectedBorder = Color.FromArgb(105, 161, 191);
            _tbNormalBorder = Color.FromArgb(145, 150, 152);
            _tbTextNormal = _tbTextSelected = Color.Black;
        }
        private GraphicsPath createTabBorderPath(Rectangle exc) {
            GraphicsPath gp = new GraphicsPath();
            if (exc.Width > 0) gp.AddLine(exc.X - 2, exc.Bottom - 1, _rectTab.X, _rectTab.Bottom - 1);
            gp.AddLine(_rectTab.X, _rectTab.Bottom - 1, _rectTab.X, _rectTab.Y + 3);
            gp.AddArc(new Rectangle(_rectTab.X, _rectTab.Y, 6, 6), 180f, 90f);
            gp.AddLine(_rectTab.X + 3, _rectTab.Y, _rectTab.Right - 4, _rectTab.Y);
            gp.AddArc(new Rectangle(_rectTab.Right - 7, _rectTab.Y, 6, 6), 270f, 90f);
            gp.AddLine(_rectTab.Right - 1, _rectTab.Y + 3, _rectTab.Right - 1, _rectTab.Bottom - 1);
            if (exc.Width > 0) gp.AddLine(_rectTab.Right - 1, _rectTab.Bottom - 1, exc.Right + 1, exc.Bottom - 1);
            else gp.AddLine(_rectTab.Right - 1, _rectTab.Bottom - 1, _rectTab.X, _rectTab.Bottom - 1);
            return gp;
        }
        #region Event Handlers
        #region Custom buttons collection
        private void buttons_AfterInsert(object sender, CollectionEventArgs e) {
            BarFormButton button = (BarFormButton)e.Item;
            button.EnabledChanged += button_EnabledChanged;
            button.VisibleChanged += button_VisibleChanged;
            button.ImageChanged += button_ImageChanged;
            ButtonHost aHost = new ButtonHost(this, button);
            aHost.Y = 0;
            if (e.Index >= _buttonHosts.Count) _buttonHosts.Add(aHost);
            else _buttonHosts.Insert(e.Index, aHost);
            if (button.Visible) {
                arrangeObjects();
                paintNC();
            }
        }
        private void buttons_Removing(object sender, CollectionEventArgs e) {
            BarFormButton button = (BarFormButton)e.Item;
            button.EnabledChanged -= button_EnabledChanged;
            button.VisibleChanged -= button_VisibleChanged;
            button.ImageChanged -= button_ImageChanged;
        }
        private void buttons_AfterRemove(object sender, CollectionEventArgs e) {
            BarFormButton button = (BarFormButton)e.Item;
            ButtonHost ahost = null;
            foreach (ButtonHost host in _buttonHosts) {
                if (host.Button == button) {
                    ahost = host;
                    break;
                }
            }
            if (_hoverHost == ahost) _hoverHost = null;
            if (_pressedHost == ahost) _pressedHost = null;
            if (ahost != null) _buttonHosts.Remove(ahost);
            if (button.Visible) {
                arrangeObjects();
                paintNC();
            }
        }
        private void buttons_Clearing(object sender, CollectionEventArgs e) {
            int i = 0;
            while (i < _customButtons.Count) {
                _customButtons[i].EnabledChanged -= button_EnabledChanged;
                _customButtons[i].VisibleChanged -= button_VisibleChanged;
                _customButtons[i].ImageChanged -= button_ImageChanged;
                i++;
            }
        }
        private void buttons_AfterClear(object sender, CollectionEventArgs e) {
            if (_hoverHost is ButtonHost) _hoverHost = null;
            if (_pressedHost is ButtonHost) _pressedHost = null;
            _buttonHosts.Clear();
            arrangeObjects();
            paintNC();
        }
        #endregion
        #region TabForms collection
        private void tabs_AfterInsert(object sender, CollectionEventArgs e) {
            TabFormButton tab = (TabFormButton)e.Item;
            TabHost host = new TabHost(this, tab);
            host.Y = _rectTab.Y + 3;
            if (tab.Selected) {
                if (_tabHosts.Count > 0) {
                    // Deselect for another selected tab, if any.
                    _suppressEvent = true;
                    int i = 0;
                    while (i < _tabHosts.Count) {
                        if (_tabHosts[i].Tab.Selected) {
                            _tabHosts[i].Tab.Selected = false;
                            break;
                        }
                        i++;
                    }
                    _suppressEvent = false;
                }
            }
            if (e.Index >= _tabHosts.Count) _tabHosts.Add(host);
            else _tabHosts.Insert(e.Index, host);
            if (_tabHosts.Count == 1) {
                if (!tab.Selected) { 
                    // Select the tab if there just one tab.
                    _suppressEvent = true;
                    tab.Selected = true;
                    _suppressEvent = false;
                }
            }
            arrangeTabs();
            if (_isTabbedForm) paintNC();
        }
        private void tabs_Removing(object sender, CollectionEventArgs e) {
            TabFormButton tab = (TabFormButton)e.Item;
            tab.TextChanged -= tab_TextChanged;
            tab.ImageChanged -= tab_ImageChanged;
            tab.SelectedChanged -= tab_SelectedChanged;
            tab.TabClosed -= tab_TabClosed;
        }
        private void tabs_AfterRemove(object sender, CollectionEventArgs e) {
            TabFormButton tab = (TabFormButton)e.Item;
            if (tab.Selected) {
                if (_tabHosts.Count > 1) { 
                    // Select the first tab.
                    _suppressEvent = true;
                    _tabHosts[0].Tab.Selected = true;
                    _suppressEvent = false;
                }
            }
            int i = 0;
            while (i < _tabHosts.Count) {
                if (_tabHosts[i].Tab == tab) {
                    if (_hoverHost == _tabHosts[i]) _hoverHost = null;
                    if (_pressedHost == _tabHosts[i]) _pressedHost = null;
                    _tabHosts.RemoveAt(i);
                    break;
                }
                i++;
            }
            arrangeTabs();
            if (_isTabbedForm) paintNC();
        }
        private void tabs_Clearing(object sender, CollectionEventArgs e) {
            int i = 0;
            while (i < _tabs.Count) {
                _tabs[i].TextChanged -= tab_TextChanged;
                _tabs[i].ImageChanged -= tab_ImageChanged;
                _tabs[i].SelectedChanged -= tab_SelectedChanged;
                _tabs[i].TabClosed -= tab_TabClosed;
                i++;
            }
        }
        private void tabs_AfterClear(object sender, CollectionEventArgs e) {
            if (_hoverHost is TabHost) _hoverHost = null;
            if (_pressedHost is TabHost) _pressedHost = null;
            _tabHosts.Clear();
            if (_isTabbedForm) paintNC();
        }
        #endregion
        #region Custom button
        private void button_EnabledChanged(object sender, EventArgs e) {
            BarFormButton button = (BarFormButton)sender;
            if (button.Visible) paintNC();
        }
        private void button_VisibleChanged(object sender, EventArgs e) {
            arrangeObjects();
            paintNC();
        }
        private void button_ImageChanged(object sender, EventArgs e) {
            BarFormButton button = (BarFormButton)sender;
            if (button.Visible) paintNC();
        }
        #endregion
        #region TabForm
        private void tab_TextChanged(object sender, EventArgs e) { if (_isTabbedForm)paintNC(); }
        private void tab_ImageChanged(object sender, EventArgs e) { if (_isTabbedForm)paintNC(); }
        private void tab_SelectedChanged(object sender, EventArgs e) {
            if (!_suppressEvent) {
                _suppressEvent = true;
                TabFormButton tab = (TabFormButton)sender;
                if (tab.Selected) {
                    // Deselect another tab if any.
                    int i = 0;
                    while (i < _tabs.Count) {
                        if (_tabs[i].Selected && _tabs[i] != tab) {
                            _tabs[i].Selected = false;
                            break;
                        }
                        i++;
                    }
                } else {
                    bool foundSelected = false;
                    foreach (TabFormButton aTab in _tabs) {
                        if (aTab.Selected) {
                            foundSelected = true;
                            break;
                        }
                    }
                    if (!foundSelected) _tabs[0].Selected = true;
                }
                if (_isTabbedForm) paintNC();
                _suppressEvent = false;
            }
        }
        private void tab_TabClosed(object sender, EventArgs e) {
            _tabs.Remove((TabFormButton)sender);
        }
        #endregion
        #region ToolTip
        private void tooltip_Draw(object sender, Ai.Control.DrawEventArgs e) {
            Renderer.ToolTip.drawToolTip(e.Graphics, new Rectangle((int)e.Rectangle.X, (int)e.Rectangle.Y, 
                (int)e.Rectangle.Width, (int)e.Rectangle.Height), _currentToolTipTitle, _currentToolTip, _currentToolTipImage);
            _currentToolTipTitle= "";
            _currentToolTip = "";
            _currentToolTipImage = null;
        }
        private void tooltip_Popup(object sender, Ai.Control.PopupEventArgs e) {
            e.Size = Renderer.ToolTip.measureSize(_currentToolTipTitle, _currentToolTip, _currentToolTipImage);
        }
        #endregion
        #endregion
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets font object to draw the text form.
        /// </summary>
        public Font TextFont {
            get { return _textFont; }
            set {
                if (value == null) return;
                if (_textFont != value) {
                    _textFont = value;
                    arrangeTabs();
                    paintNC();
                }
            }
        }
        /// <summary>
        /// Gets or sets font object to draw normal tab.
        /// </summary>
        public Font TabNormalFont {
            get { return _tabNormalFont; }
            set {
                if (value == null) return;
                if (_tabNormalFont != value) {
                    _tabNormalFont = value;
                    if (_isTabbedForm) paintNC();
                }
            }
        }
        /// <summary>
        /// Gets or sets font object to draw selected tab.
        /// </summary>
        public Font TabSelectedFont {
            get { return _tabSelectedFont; }
            set {
                if (value == null) return;
                if (_tabSelectedFont != value) {
                    _tabSelectedFont = value;
                    if (_isTabbedForm) paintNC();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the skin is using tabs.
        /// </summary>
        public bool IsTabbedForm {
            get { return _isTabbedForm; }
            set {
                if (_isTabbedForm != value) {
                    _isTabbedForm = value;
                    paintNC();
                }
            }
        }
        /// <summary>
        /// Gets or sets color theme to draw the skin.
        /// </summary>
        public ColorTheme Theme {
            get { return _theme; }
            set {
                if (_theme != value) {
                    _theme = value;
                    switch (_theme) { 
                        case ColorTheme.Blue:
                            initBlueTheme();
                            break;
                        case ColorTheme.BlackBlue:
                            initBlackBlueTheme();
                            break;
                        default :
                            initBlueTheme();
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Gets a collection containing all BarFormButton in the skin.
        /// </summary>
        public BarFormButtonCollection CustomButtons { get { return _customButtons; } }
        /// <summary>
        /// Gets a collection containing all TabFormButton in the skin.
        /// </summary>
        public TabFormButtonCollection Tabs { get { return _tabs; } }
        #endregion
        #region Overrides
        protected internal override bool onDoubleClick() {
            // No need to handle the double click event here.
            return false;
        }
        protected internal override bool onMouseMove(MouseEventArgs e) {
            bool changed = false;
            if (_minHost.Button.Visible) changed = changed || _minHost.mouseMove(e);
            if (_maxHost.Button.Visible) changed = changed || _maxHost.mouseMove(e);
            if (_closeHost.Button.Visible) changed = changed || _closeHost.mouseMove(e);
            int i = 0;
            while (i < _buttonHosts.Count) {
                if (_buttonHosts[i].Button.Visible) changed = changed || _buttonHosts[i].mouseMove(e);
                i++;
            }
            if (_isTabbedForm) {
                i = 0;
                while (i < _tabHosts.Count) {
                    changed = changed || _tabHosts[i].mouseMove(e);
                    i++;
                }
            }
            if (changed) paintNC();
            return changed;
        }
        protected internal override bool onMouseDown(MouseEventArgs e) {
            bool changed = false;
            if (_minHost.Button.Visible) changed = changed || _minHost.mouseDown(e);
            if (_maxHost.Button.Visible) changed = changed || _maxHost.mouseDown(e);
            if (_closeHost.Button.Visible) changed = changed || _closeHost.mouseDown(e);
            int i = 0;
            while (i < _buttonHosts.Count) {
                if (_buttonHosts[i].Button.Visible) changed = changed || _buttonHosts[i].mouseDown(e);
                i++;
            }
            if (_isTabbedForm) {
                i = 0;
                while (i < _tabHosts.Count) {
                    changed = changed || _tabHosts[i].mouseDown(e);
                    i++;
                }
            }
            if (changed) paintNC();
            return changed;
        }
        protected internal override bool onMouseUp(MouseEventArgs e) {
            bool changed = false;
            if (_minHost.Button.Visible) changed = changed || _minHost.mouseUp(e);
            if (_maxHost.Button.Visible) changed = changed || _maxHost.mouseUp(e);
            if (_closeHost.Button.Visible) changed = changed || _closeHost.mouseUp(e);
            int i = 0;
            while (i < _buttonHosts.Count) {
                if (_buttonHosts[i].Button.Visible) changed = changed || _buttonHosts[i].mouseUp(e);
                i++;
            }
            if (_isTabbedForm) {
                i = 0;
                while (i < _tabHosts.Count) {
                    changed = changed || _tabHosts[i].mouseUp(e);
                    i++;
                }
            }
            if (changed) paintNC();
            return changed;
        }
        protected internal override bool onMouseLeave() {
            bool changed = false;
            if (_minHost.Button.Visible) changed = changed || _minHost.mouseLeave();
            if (_maxHost.Button.Visible) changed = changed || _maxHost.mouseLeave();
            if (_closeHost.Button.Visible) changed = changed || _closeHost.mouseLeave();
            int i = 0;
            while (i < _buttonHosts.Count) {
                if (_buttonHosts[i].Button.Visible) changed = changed || _buttonHosts[i].mouseLeave();
                i++;
            }
            if (_isTabbedForm) {
                i = 0;
                while (i < _tabHosts.Count) {
                    changed = changed || _tabHosts[i].mouseLeave();
                    i++;
                }
            }
            if (changed) paintNC();
            return changed;
        }
        protected internal override bool onPaint(PaintEventArgs e) {
            if (Form == null) return false;
            int i;
            Rectangle rect = e.ClipRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            GraphicsPath gp = Renderer.Drawing.roundedRectangle(rect, 4, 4, 3, 3);
            LinearGradientBrush formBg = new LinearGradientBrush(rect, Color.Black, Color.White, 30f);
            Pen formBorder;
            if (FormIsActive) {
                formBg.InterpolationColors = _formActiveBgBlend;
                formBorder = new Pen(_formActiveBorder, 2f);
            } else {
                formBg.InterpolationColors = _formInactiveBgBlend;
                formBorder = new Pen(_formInactiveBorder, 2f);
            }
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // Draw form background.
            e.Graphics.Clear(Color.Transparent);
            e.Graphics.FillPath(formBg, gp);
            formBg.Dispose();
            // Draw the icon and the text.
            if (Form.Icon != null) e.Graphics.DrawIcon(Form.Icon, new Rectangle(_rectIcon.X + 4, _rectIcon.Y + 4, _rectIcon.Width - 8, _rectIcon.Height - 8));
            Rectangle rectText = new Rectangle(_rectIcon.Right + 1, 0, _rectBar.Width - (_rectIcon.Right + 1), _rectBar.Height);
            e.Graphics.DrawString(Form.Text, _textFont, new SolidBrush(FormIsActive ? _formActiveTextColor : _formInactiveTextColor), 
                rectText, _sfTab);
            // Draw the system and custom buttons
            ButtonHost lastButton = null;
            if (_closeButton.Visible) lastButton = _closeHost;
            if (lastButton == null) {
                if (_maximizeButton.Visible) lastButton = _maxHost;
            }
            if (lastButton == null) {
                if (_minimizeButton.Visible) lastButton = _minHost;
            }
            if (lastButton == null) {
                i = _buttonHosts.Count - 1;
                while (i >= 0) {
                    if (_buttonHosts[i].Button.Visible) {
                        lastButton = _buttonHosts[i];
                        break;
                    }
                    i--;
                }
            }
            bool first = true;
            foreach (ButtonHost host in _buttonHosts) {
                if (host.Button.Visible) {
                    host.draw(e.Graphics, first ? 3 : 0, host == lastButton ? 3 : 0);
                    first = false;
                }
            }
            if (_minimizeButton.Visible) {
                _minHost.draw(e.Graphics, first ? 3 : 0, _minHost == lastButton ? 3 : 0);
                first = false;
            }
            if (_maximizeButton.Visible) {
                _maxHost.draw(e.Graphics, first ? 3 : 0, _maxHost == lastButton ? 3 : 0);
                first = false;
            }
            if (_closeButton.Visible) {
                _closeHost.draw(e.Graphics, first ? 3 : 0, _closeHost == lastButton ? 3 : 0);
                first = false;
            }
            if (_isTabbedForm) { 
                // Draw tabs.
                GraphicsPath gpTab = Renderer.Drawing.roundedRectangle(_rectTab, 3, 3, 0, 0);
                LinearGradientBrush bgTab = new LinearGradientBrush(_rectTab, Color.Black, Color.White, LinearGradientMode.Vertical);
                LinearGradientBrush tabShadow = new LinearGradientBrush(_rectTab, Color.Black, Color.White, LinearGradientMode.Vertical);
                ColorBlend shadowBlend = new ColorBlend();
                Color[] colors = new Color[4];
                float[] pos = new float[4];
                pos[0] = 0f;
                pos[1] = 0.3f;
                pos[2] = 0.8f;
                pos[3] = 1f;
                colors[0] = Color.FromArgb(95, 0, 0, 0);
                colors[1] = Color.FromArgb(0, 0, 0, 0);
                colors[2] = Color.FromArgb(0, 0, 0, 0);
                colors[3] = Color.FromArgb(95, 0, 0, 0);
                shadowBlend.Colors = colors;
                shadowBlend.Positions = pos;
                tabShadow.InterpolationColors = shadowBlend;
                Pen tabBorder;
                if (FormIsActive) {
                    bgTab.InterpolationColors = _taActiveBlend;
                    tabBorder = new Pen(_taActiveBorder);
                } else {
                    bgTab.InterpolationColors = _taInactiveBlend;
                    tabBorder = new Pen(_taInactiveBorder);
                }
                e.Graphics.FillPath(bgTab, gpTab);
                bgTab.Dispose();
                if (_tabHosts.Count > 0) {
                    TabHost selectedHost = null;
                    foreach (TabHost tab in _tabHosts) {
                        if (tab.Tab.Selected) selectedHost = tab;
                        else tab.draw(e.Graphics);
                    }
                    e.Graphics.FillPath(tabShadow, gpTab);
                    if (selectedHost != null) selectedHost.draw(e.Graphics);
                    tabShadow.Dispose();
                    gpTab.Dispose();
                    gpTab = createTabBorderPath(selectedHost != null ? selectedHost.Bounds : new Rectangle(0, 0, 0, 0));
                    e.Graphics.DrawPath(tabBorder, gpTab);
                    gpTab.Dispose();
                    tabBorder.Dispose();
                } else {
                    e.Graphics.FillPath(tabShadow, gpTab);
                    e.Graphics.DrawPath(tabBorder, gpTab);
                    tabShadow.Dispose();
                    gpTab.Dispose();
                    tabBorder.Dispose();
                }
            }
            // Form border
            e.Graphics.DrawPath(formBorder, gp);
            gp.Dispose();
            return true;
        }
        protected internal override bool onKeyDown(KeyEventArgs e) {
            if (_isTabbedForm) {
                if (e.KeyData == (Keys.Control | Keys.Tab)) {
                    // Handles the "Ctrl + Tab" key to select next tab.
                    if (_tabHosts.Count > 1) {
                        TabHost selHost = null;
                        foreach (TabHost host in _tabHosts) {
                            if (host.Tab.Selected) {
                                selHost = host;
                                break;
                            }
                        }
                        if (selHost != null) {
                            int selIndex = _tabHosts.IndexOf(selHost);
                            int nextIndex = (selIndex + 1) % _tabHosts.Count;
                            // Select the tab next index.
                            _suppressEvent = true;
                            selHost.Tab.Selected = false;
                            _tabHosts[nextIndex].Tab.Selected = true;
                            paintNC();
                            _suppressEvent = false;
                            return true;
                        }
                    }
                }
            }
            foreach (BarFormButton button in _customButtons) {
                if (button.Enabled && button.Visible) {
                    if (e.KeyData == button.Shortcut) {
                        button.onClick();
                        return true;
                    }
                }
            }
            return false;
        }
        protected internal override bool setRegion(Size size) {
            Rectangle rect = new Rectangle(new Point(0, 0), size);
            GraphicsPath gp = Renderer.Drawing.roundedRectangle(rect, 4, 4, 3, 3);
            Region rg = new Region(gp);
            Form.Region = rg;
            gp.Dispose();
            return true;
        }
        protected internal override Win32API.NCCALCSIZE_PARAMS calculateNonClient(Win32API.NCCALCSIZE_PARAMS p) {
            // Check if we don't need to calculate the client area.
            if (Form == null || Form.WindowState == FormWindowState.Minimized || 
                (Form.WindowState == FormWindowState.Minimized && Form.MdiParent != null)) return p;
            // Calculate the valid client area of the form here, that is stored in rect0 of the p parameter.
            p.rect0.Top += _rectBar.Height;
            _rectClient.Y = _rectBar.Height + 1;
            if (Form.WindowState == FormWindowState.Maximized) { 
                // The form is maximized, thus the borders will not be calculated and the status bar only will be calculated.
                //p.rect0.Bottom -= _rectStatus.Height;
                _rectClient.X = 0;
                _rectClient.Width = p.rect0.Right - (p.rect0.Left + 1);
                _rectClient.Height = p.rect0.Bottom - (p.rect0.Top + 1);
            } else { 
                // Deflate the left, right, and bottom of the rect0 by the left border width, 
                // right border width, and sum of the status and bottom border height.
                p.rect0.Left += _rectBorderLeft.Width;
                p.rect0.Right -= _rectBorderRight.Width;
                p.rect0.Bottom -= _rectBorderBottom.Height;
                _rectClient.X = _rectBorderLeft.Width + 1;
                _rectClient.Width = p.rect0.Right - (p.rect0.Left + 2);
                _rectClient.Height = p.rect0.Bottom - (p.rect0.Top + 2);
            }
            return p;
        }
        protected internal override void updateBar(Rectangle rect) {
            if (Form == null || Form.WindowState == FormWindowState.Minimized ||
                (Form.WindowState == FormWindowState.Maximized && Form.MdiParent != null)) return;
            if (Form.WindowState == FormWindowState.Maximized) _maximizeButton.Tooltip = "Restore Down";
            else _maximizeButton.Tooltip = "Maximize";
            // Update all the non-client component here based on the rect parameter.
            if (Form.WindowState == FormWindowState.Maximized) {
                _rectBar = new Rectangle(0, 0, rect.Width, _rectBar.Height);
                //_rectStatus = new Rectangle(0, rect.Height - _rectStatus.Height, rect.Width, _rectStatus.Height);
            } else {
                _rectBar = new Rectangle(5, 0, rect.Width - 10, _rectBar.Height);
                //_rectStatus = new Rectangle(5, rect.Height - (_rectStatus.Height + _rectBorderBottom.Height), rect.Width - 10, _rectStatus.Height);
                _rectBorderTopRight.Location = new Point(rect.Width - 5, 0);
                _rectBorderRight.Location = new Point(rect.Width - 5, 5);
                _rectBorderRight.Height = rect.Height - 10;
                _rectBorderBottomRight.Location = new Point(rect.Width - 5, rect.Height - 5);
                _rectBorderBottom.Location = new Point(0, rect.Height - 5);
                _rectBorderBottom.Width = rect.Width - 10;
                _rectBorderBottomLeft.Location = new Point(0, rect.Height - 5);
                _rectBorderLeft.Height = rect.Height - 10;
                _rectBorderTop.X = _rectBar.X;
                _rectBorderTop.Width = _rectBar.Width;
            }
            _rectIcon.Location = new Point(_rectBar.X, 0);
            arrangeObjects();
        }
        protected internal override int nonClientHitTest(Point p) {
            if (_rectClient.Contains(p)) return Win32API.HTCLIENT;
            if (_rectIcon.Contains(p)) return Win32API.HTMENU;
            // Always return HTOBJECT instead of the correcponding hittest value, to prevent the default tooltip to be shown.
            if (_minimizeButton.Enabled && _minimizeButton.Visible) {
                if (_minHost.Bounds.Contains(p)) return Win32API.HTOBJECT;
            }
            if (_maximizeButton.Enabled && _maximizeButton.Visible) {
                if (_maxHost.Bounds.Contains(p)) return Win32API.HTOBJECT;
            }
            if (_closeButton.Enabled && _closeButton.Visible) {
                if (_closeHost.Bounds.Contains(p)) return Win32API.HTOBJECT;
            }
            // Test for custom bar button, if any of them, the return the HTOBJECT
            if (Form.FormBorderStyle == FormBorderStyle.Sizable || Form.FormBorderStyle == FormBorderStyle.SizableToolWindow 
                && Form.WindowState != FormWindowState.Maximized) { 
                // Test for borders.
                // Corners
                if (_rectBorderTopLeft.Contains(p)) return Win32API.HTTOPLEFT;
                if (_rectBorderTopRight.Contains(p)) return Win32API.HTTOPRIGHT;
                if (_rectBorderBottomLeft.Contains(p)) return Win32API.HTBOTTOMLEFT;
                if (_rectBorderBottomRight.Contains(p)) return Win32API.HTBOTTOMRIGHT;
                // vertical and horizontal
                if (_rectBorderTop.Contains(p)) return Win32API.HTTOP;
                if (_rectBorderLeft.Contains(p)) return Win32API.HTLEFT;
                if (_rectBorderRight.Contains(p)) return Win32API.HTRIGHT;
                if (_rectBorderBottom.Contains(p)) return Win32API.HTBOTTOM;
            }
            if (Form.WindowState != FormWindowState.Maximized) {
                // Test for bar form.
                if (_rectBar.Contains(p)) return Win32API.HTCAPTION;
            }
            // Default return value.
            return Win32API.HTNOWHERE;
        }
        protected internal override void onFormTextChanged() {
            arrangeTabs();
            paintNC();
        }
        #endregion
    }
    #endregion
}