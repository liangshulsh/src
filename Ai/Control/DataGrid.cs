// Ai Software Control Library.
// Created by : Burhanudin Ashari (red_moon@CodeProject) @ December 08, 2011.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;

namespace Ai.Control {
    /// <summary>
    /// Represent a collection of data grid items that contains cells.
    /// </summary>
    public class DataGrid : System.Windows.Forms.Control {
        #region Private Constant
        /// <summary>
        /// Defines the minimum size of each ColumnHeaderHost allowed to resize.
        /// </summary>
        private const int MIN_COLUMN_WIDTH = 20;
        #endregion
        #region Public Classes
        /// <summary>
        /// Represent a collection of DataGridHeader in data grid.
        /// </summary>
        public class DataGridHeaderCollection : CollectionBase {
            DataGrid _owner = null;
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
            #region Constructor
            public DataGridHeaderCollection(DataGrid owner) : base() { _owner = owner; }
            #endregion
            #region Public Members
            /// <summary>
            /// Gets a DataGridHeader object in the collection specified by its index.
            /// </summary>
            /// <param name="index">The index of the item in the collection to get.</param>
            /// <returns>A DataGridHeader object located at the specified index within the collection.</returns>
            public DataGridHeader this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (DataGridHeader)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Returns the index within the collection of the specified item.
            /// </summary>
            /// <param name="item">A DataGridItem object representing the item to locate in the collection.</param>
            /// <returns>The zero-based index where the item is located within the collection; otherwise, negative one (-1).</returns>
            public int IndexOf(DataGridHeader header) { return List.IndexOf(header); }
            /// <summary>
            /// Adds a DataGridHeader to the list of headers for a DataGrid.
            /// </summary>
            public DataGridHeader Add(DataGridHeader header) {
                header._owner = _owner;
                int index = List.Add(header);
                return (DataGridHeader)List[index];
            }
            /// <summary>
            /// Adds the elements of the specified collection to the end of the collection.
            /// </summary>
            /// <param name="items"></param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(DataGridHeaderCollection headers) {
                foreach (DataGridHeader hd in headers) this.Add(hd);
            }
            /// <summary>
            /// Inserts an element into the collection at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which item should be inserted.</param>
            /// <param name="item">The DataGridHeader object to insert.</param>
            public void Insert(int index, DataGridHeader header) {
                if (header != null) {
                    header._owner = _owner;
                    List.Insert(index, header);
                }
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the collection.
            /// </summary>
            /// <param name="item">The object to remove from the collection.</param>
            /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
            public bool Remove(DataGridHeader header) {
                if (List.Contains(header)) {
                    List.Remove(header);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Determines whether an element is in the collection.
            /// </summary>
            /// <param name="item">The object to locate in the collection.</param>
            /// <returns>true if item is found in the collection; otherwise, false</returns>
            public bool Contains(DataGridHeader header) { return List.Contains(header); }
            /// <summary>
            /// Determines whether the collection contains a specific key.
            /// </summary>
            /// <param name="key">The name of the DataGridItem to locate in the collection.</param>
            /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
            public bool ContainsKey(string key) {
                foreach (DataGridHeader hd in List) {
                    if (hd.Name == key) return true;
                }
                return false;
            }
            #endregion
            #region Overiden Methods
            protected override void OnValidate(object value) {
                if (!typeof(DataGridHeader).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.DataGridHeader", "value");
            }
            protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
            protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
            protected override void OnInsert(int index, object value) {
                if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
            protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
            protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null)AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
            protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            #endregion
        }
        /// <summary>
        /// Represent a collection of Checked DataGridItem objects in a DataGrid.
        /// </summary>
        public class CheckedDataGridItemCollection : CollectionBase {
            internal DataGrid _owner;
            #region Constructor
            public CheckedDataGridItemCollection(DataGrid owner) : base() { _owner = owner; }
            #endregion
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
            #region Public Functions
            /// <summary>
            /// Gets a DataGridItem object in the collection specified by its index.
            /// </summary>
            [Description("Gets a DataGridItem object in the collection specified by its index.")]
            public DataGridItem this[int index] {
                get {
                    if (index >= 0 & index < List.Count) return (DataGridItem)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a DataGridItem object in the collection.
            /// </summary>
            [Description("Gets the index of a ListViewItem object in the collection.")]
            public int IndexOf(DataGridItem item) { return this.List.IndexOf(item); }
            /// <summary>
            /// Add a DataGridItem object to the collection.
            /// </summary>
            [Description("Add a DataGridItem object to the collection.")]
            internal DataGridItem Add(DataGridItem item) {
                // Avoid adding the same item multiple times.
                if (!this.Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    return (DataGridItem)List[index];
                }
                return item;
            }
            /// <summary>
            /// Add a DataGridItem collection to the collection.
            /// </summary>
            [Description("Add a ListViewItem collection to the collection."),
                System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            internal void AddRange(CheckedDataGridItemCollection items) {
                foreach (DataGridItem item in items) this.Add(item);
            }
            /// <summary>
            /// Remove a DataGridItem object from the collection.
            /// </summary>
            [Description("Remove a DataGridItem object from the collection.")]
            public void Remove(DataGridItem item) {
                if (List.Contains(item)) List.Remove(item);
            }
            /// <summary>
            /// Determine whether a DataGridItem object exist in the collection.
            /// </summary>
            [Description("Determine whether a ListViewItem object exist in the collection.")]
            public bool Contains(DataGridItem item) { return List.Contains(item); }
            #endregion
            #region Protected Overriden Methods
            [Description("Performs additional custom processes when validating a value.")]
            protected override void OnValidate(object value) {
                if (!typeof(DataGridItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.DataGridItem", "value");
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
        #region Core Engines.  Classes to handle all objects, key and mouse event handlers, and as a visual representation of every object associated with DataGrid.
        /// <summary>
        /// Class to hosts all columns and handles all of the operations.
        /// </summary>
        private class ColumnHeaderControl : System.Windows.Forms.Control {
            #region Constants
            /// <summary>
            /// Defines the width and height of the drag operation's sign.
            /// </summary>
            private const int SIGN_SIZE = 25;
            /// <summary>
            /// Defines the alpha value used to draw the drag operation's sign.
            /// </summary>
            private const int SIGN_ALPHA = 127;
            /// <summary>
            /// Defines the height of the button group drawn in ColumnHeaderControl.
            /// </summary>
            private const int GROUP_SIZE = 25;
            /// <summary>
            /// Defines the width of the separator between column header.
            /// </summary>
            private const int SEPARATOR_SIZE = 5;
            #endregion
            #region Object Hosts
            /// <summary>
            /// Represent a host for an object corresponds with DataGrid header.
            /// </summary>
            private abstract class ObjectHost {
                #region Internal Events
                public event EventHandler<EventArgs> MouseRightDown;
                public event EventHandler<EventArgs> PartDown;
                #endregion
                #region Internal Members
                internal Rectangle _rect;
                internal Rectangle _partRect;
                internal bool _onHover = false;
                internal bool _onPressed = false;
                internal bool _onPartHover = false;
                internal bool _onPartPressed = false;
                internal bool _selected = false;
                internal Point _startDrag;
                internal Point _currentDrag;
                internal object _host;
                internal ObjectHost _leftHost;
                internal ObjectHost _rightHost;
                internal ColumnHeaderControl _owner;
                internal DragHelper.DragEffect _de;
                #endregion
                public ObjectHost(ColumnHeaderControl owner) {
                    _owner = owner;
                    _leftHost = null;
                    _rightHost = null;
                    _de = DragHelper.DragEffect.None;
                }
                #region Public Properties
                /// <summary>
                /// Gets hosted object.
                /// </summary>
                public object Host { get { return _host; } }
                /// <summary>
                ///  Gets owner of the host.
                /// </summary>
                public ColumnHeaderControl Owner { get { return _owner; } }
                /// <summary>
                /// Gets bounding rectangle of the host.
                /// </summary>
                public Rectangle Bounds { get { return _rect; } }
                /// <summary>
                /// Gets the distance, in pixels, between the left edge of the host and the left edge of its container's client area.
                /// </summary>
                public int Left { get { return _rect.Left; } }
                /// <summary>
                /// Gets the distance, in pixels, between the right edge of the host and the left edge of its container's client area.
                /// </summary>
                public int Right { get { return _rect.Right; } }
                /// <summary>
                /// Gets the distance, in pixels, between the top edge of the host and the top edge of its container's client area.
                /// </summary>
                public int Top { get { return _rect.Top; } }
                /// <summary>
                /// Gets the distance, in pixels, between the bottom edge of the host and the top edge of its container's client area.
                /// </summary>
                public int Bottom { get { return _rect.Bottom; } }
                /// <summary>
                /// Gets the action's sign of the current drag operation performed to this host.
                /// </summary>
                public DragHelper.DragEffect DragEffect { get { return _de; } }
                /// <summary>
                /// Gets the minimum size allowed of the host.  This property only used when the host is DataGridHeader object, otherwise, its always return 0.
                /// </summary>
                public virtual int MinResize { get { return 0; } }
                /// <summary>
                /// Gets the maximum size allowed of the host.  This property only used when the host is DataGridHeader object, otherwise, its always return 0.
                /// </summary>
                public virtual int MaxResize { get { return 0; } }
                /// <summary>
                /// Gets or sets ObjectHost object that placed at the left of this host.
                /// </summary>
                public ObjectHost LeftHost {
                    get { return _leftHost; }
                    set { _leftHost = value; }
                }
                /// <summary>
                /// Gets or sets ObjectHost that placed at the right of the host.
                /// </summary>
                public ObjectHost RightHost {
                    get { return _rightHost; }
                    set { _rightHost = value; }
                }
                /// <summary>
                /// Gets or sets a value indicating the mouse pointer is moved over the host.
                /// </summary>
                public bool Hover {
                    get { return _onHover; }
                    set { _onHover = value; }
                }
                /// <summary>
                /// Gets or sets a value indicating a left button of the mouse is pressed over the host.
                /// </summary>
                public bool Pressed {
                    get { return _onPressed; }
                    set { _onPressed = value; }
                }
                /// <summary>
                /// Gets or sets a value indicating the mouse pointer is moved over the part of the host.
                /// </summary>
                public bool PartHover {
                    get { return _onPartHover; }
                    set { _onPartPressed = value; }
                }
                /// <summary>
                /// Gets or sets a value indicating the left button of the mouse is pressed over the part of the host.
                /// </summary>
                public bool PartPressed {
                    get { return _onPartPressed; }
                    set { _onPartPressed = value; }
                }
                /// <summary>
                /// Gets or set a value indicating the host is currently selected.
                /// </summary>
                public bool Selected {
                    get { return _selected; }
                    set { _selected = value; }
                }
                /// <summary>
                /// Gets or sets the distance, in pixels, between the left edge of the host and the left edge of its container's client area.
                /// </summary>
                public int X {
                    get { return _rect.X; }
                    set {
                        if (_rect.X != value) {
                            int dx = value - _rect.X;
                            _rect.X = value;
                            _partRect.X += dx;
                        }
                    }
                }
                /// <summary>
                /// Gets or sets the distance, in pixels, between the top edge of the host and the top edge of its container's client area.
                /// </summary>
                public int Y {
                    get { return _rect.Y; }
                    set {
                        _rect.Y = value;
                        _partRect.Y = value;
                    }
                }
                /// <summary>
                /// Gets or sets the width of the host.
                /// </summary>
                public virtual int Width {
                    get { return _rect.Width; }
                    set {
                        int dWidth = value - _rect.Width;
                        _rect.Width = value;
                        _partRect.X += dWidth;
                    }
                }
                /// <summary>
                /// Gets or sets the height of the host.
                /// </summary>
                public virtual int Height {
                    get { return _rect.Height; }
                    set {
                        _rect.Height = value;
                        _partRect.Height = value;
                    }
                }
                /// <summary>
                /// Gets or sets the coordinates of the upper-left corner of the host relative to the upper-left corner of its container.
                /// </summary>
                public Point Location {
                    get { return _rect.Location; }
                    set {
                        int dX = value.X - _rect.X;
                        _rect.Location = value;
                        _partRect.X += dX;
                        _partRect.Y = _rect.Y;
                    }
                }
                /// <summary>
                /// Gets or sets the height and width of the host.
                /// </summary>
                public Size Size {
                    get { return _rect.Size; }
                    set {
                        int dWidth = value.Width - _rect.Width;
                        _rect.Size = value;
                        _partRect.X += dWidth;
                        _partRect.Height = _rect.Height;
                    }
                }
                /// <summary>
                /// Gets or sets the starting point of drag drop operation of the host.
                /// </summary>
                public Point CurrentDrag {
                    get { return _currentDrag; }
                    set { _currentDrag = value; }
                }
                /// <summary>
                /// Gets or sets the starting point of the drag operation.
                /// </summary>
                public Point StartDrag {
                    get { return _startDrag; }
                    set { _startDrag = value; }
                }
                #endregion
                #region Public Methods
                // Drawing Objects.
                /// <summary>
                /// Draws the hosted object to the specified graphics object.
                /// </summary>
                /// <param name="g">Graphics objects where the hosted object will be drawn.</param>
                public abstract void draw(Graphics g);
                /// <summary>
                /// Draws the hosted object being dragged on specified graphics object with current drag location.
                /// </summary>
                /// <param name="g">Graphics object where the host will be drawn.</param>
                /// <param name="p">Current location of drag operation.</param>
                public abstract DragHelper.DragEffect drawDrag(Graphics g);
                /// <summary>
                /// Draws the drag drop operation's sign on specified Graphics object.
                /// </summary>
                /// <param name="g">A Graphics object where the sign to be drawn.</param>
                /// <param name="effect">One of the DragHelper.DragEffect value represent the sign of the drag drop operation.</param>
                public void drawDragEffect(Graphics g, DragHelper.DragEffect effect) {
                    if (effect != DragHelper.DragEffect.None) {
                        GraphicsPath gp = new GraphicsPath();
                        Color signColor = Color.Black;
                        switch (effect) {
                            case DragHelper.DragEffect.Insert:
                                gp = Renderer.Drawing.getInsertSignPath(new Point(0, 0), ColumnHeaderControl.SIGN_SIZE, 6);
                                signColor = Color.FromArgb(ColumnHeaderControl.SIGN_ALPHA, 0, 0, 255);
                                break;
                            case DragHelper.DragEffect.Stop:
                                gp = Renderer.Drawing.getStopSignPath(new Point(0, 0), ColumnHeaderControl.SIGN_SIZE, 5);
                                signColor = Color.FromArgb(ColumnHeaderControl.SIGN_ALPHA, 255, 0, 0);
                                break;
                            case DragHelper.DragEffect.Swap:
                                gp = Renderer.Drawing.getSwapSignPath(new Point(0, 0), ColumnHeaderControl.SIGN_SIZE);
                                signColor = Color.FromArgb(ColumnHeaderControl.SIGN_ALPHA, 0, 255, 255);
                                break;
                        }
                        g.FillPath(new SolidBrush(signColor), gp);
                        g.DrawPath(Pens.Black, gp);
                        gp.Dispose();
                    }
                }
                // Mouse Events Handler
                /// <summary>
                /// Handles the double click event of the mouse.
                /// </summary>
                public abstract void mouseDoubleClick();
                /// <summary>
                /// Handles the move event of the mouse.
                /// </summary>
                /// <param name="e">A MouseEventArgs that contains the event data.</param>
                /// <returns>True, if the event has an effect to the host, false, otherwise.</returns>
                public abstract bool mouseMove(MouseEventArgs e);
                /// <summary>
                /// Handles the event when the mouse button is being pressed.
                /// </summary>
                /// <param name="e">A MouseEventArgs that contains the event data.</param>
                /// <returns>True, if the event has an effect to the host, false, otherwise.</returns>
                public abstract bool mouseDown(MouseEventArgs e);
                /// <summary>
                /// Handles the event when the mouse button is being released.
                /// </summary>
                /// <param name="e">A MouseEventArgs that contains the event data.</param>
                /// <returns>True, if the event has an effect to the host, false, otherwise.</returns>
                public abstract bool mouseUp(MouseEventArgs e);
                /// <summary>
                /// Handles the event when the mouse pointer is leaving the control.
                /// </summary>
                /// <returns>True, if the event has an effect to the host, false, otherwise.</returns>
                public bool mouseLeave() {
                    bool stateChanged = false;
                    if (_onPartHover || _onHover) {
                        _onPartHover = false;
                        _onHover = false;
                        stateChanged = true;
                    }
                    return stateChanged;
                }
                internal void onMouseRightDown(EventArgs e) {
                    if (MouseRightDown != null) MouseRightDown(this, e);
                }
                internal void onPartDown(EventArgs e) {
                    if (PartDown != null) PartDown(this, e);
                }
                #endregion
            }
            /// <summary>
            /// Represent the host for a separator of a column header.
            /// </summary>
            private class SeparatorHost : ObjectHost {
                int _dWidth = 0;
                public SeparatorHost(ColumnHeaderControl owner) : base(owner) { }
                public override void draw(Graphics g) {
                    int x = _rect.X + (int)(_rect.Width / 2);
                    g.DrawLine(Renderer.Button.NormalBorderPen(), x, _rect.Y + 3, x, _rect.Bottom - 4);
                    g.DrawLine(Pens.White, x + 1, _rect.Y + 3, x + 1, _rect.Bottom - 4);
                }
                public override DragHelper.DragEffect drawDrag(Graphics g) {
                    Rectangle r = _rect;
                    int dx;
                    Point p = _currentDrag;
                    dx = p.X - _startDrag.X;
                    if (dx < 0) {
                        if (_rect.X + dx < 0) dx = -_rect.X;
                        if (_rect.X + dx < _leftHost._rect.X + MIN_COLUMN_WIDTH)
                            dx = (_leftHost._rect.X + MIN_COLUMN_WIDTH) - _rect.X;
                    } else {
                        if (_rect.Right + dx > (_owner._columnArea.Right - MIN_COLUMN_WIDTH)) dx = (_owner._groupArea.Right - MIN_COLUMN_WIDTH) - _rect.Right;
                        if (_rightHost != null) {
                            if (_rect.X + dx > (_rightHost._rect.Right - MIN_COLUMN_WIDTH))
                                dx = (_rightHost._rect.Right - MIN_COLUMN_WIDTH) - _rect.X;
                        }
                    }
                    _dWidth = dx;
                    r.X += dx;
                    int x = r.X + (int)(r.Width / 2);
                    g.DrawLine(Renderer.Button.NormalBorderPen(), x, r.Y + 3, x, r.Bottom - 4);
                    g.DrawLine(Pens.White, x + 1, r.Y + 3, x + 1, r.Bottom - 4);
                    return DragHelper.DragEffect.None;
                }
                public override void mouseDoubleClick() {
                    if (_leftHost != null) {
                        DataGridHeader header = (DataGridHeader)_leftHost.Host;
                        if (!header.Visible) return;
                        int maxWidth = 0;
                        //maxWidth = _owner._owner.getSubItemMaxWidth(header);
                        if (maxWidth < MinResize) maxWidth = MinResize;
                        header.SizeType = ColumnSizeType.Fixed;
                        header.Width = maxWidth;
                    }
                }
                public override bool mouseMove(MouseEventArgs e) {
                    _currentDrag = e.Location;
                    if (_leftHost != null) {
                        DataGridHeader header = (DataGridHeader)_leftHost.Host;
                        if (!header.Visible) return false;
                    }
                    if (!_owner.Bounds.Contains(e.Location)) return false;
                    bool stateChanged = false;
                    if (_rect.Contains(e.Location)) {
                        if (_owner._hoverHost != null) {
                            if (_owner._hoverHost != this) {
                                _owner._hoverHost._onHover = false;
                                _owner._hoverHost._onPartHover = false;
                            }
                        }
                        _owner._hoverHost = this;
                    } else {
                        if (_onHover || _onPartHover) {
                            _onHover = false;
                            _onPartHover = false;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                    }
                    return stateChanged;
                }
                public override bool mouseDown(MouseEventArgs e) {
                    if (_leftHost != null) {
                        DataGridHeader header = (DataGridHeader)_leftHost.Host;
                        if (!header.Visible) return false;
                    }
                    if (e.Button == MouseButtons.Right) {
                        onMouseRightDown(new EventArgs());
                        return false;
                    }
                    bool stateChanged = false;
                    if (e.Button == MouseButtons.Left) {
                        if (_onHover) {
                            if (!_onPressed) {
                                _onPressed = true;
                                stateChanged = true;
                                _startDrag = e.Location;
                                _owner._draggedHost = this;
                            }
                        }
                    }
                    return stateChanged;
                }
                public override bool mouseUp(MouseEventArgs e) {
                    if (!_onPressed) return false;
                    _onPressed = false;
                    _owner._draggedHost = null;
                    // Adds the header width by distance of drag operation on x-coordinate.
                    DataGridHeader header = (DataGridHeader)_leftHost._host;
                    if (!header.Visible) return false;
                    header.SizeType = ColumnSizeType.Fixed;
                    header.Width += _dWidth;
                    _dWidth = 0;
                    return true;
                }
            }
            /// <summary>
            /// Represent the host for connector between two DataGridGroup object.
            /// </summary>
            private class ConnectorHost : ObjectHost {
                public ConnectorHost(ColumnHeaderControl owner) : base(owner) { }
                public override void draw(Graphics g) {
                    GraphicsPath gp = new GraphicsPath();
                    Point p1, p2;
                    if (_onHover) {
                        LinearGradientBrush bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                        bgBrush.InterpolationColors = Renderer.Button.SilverNormal();
                        GraphicsPath gpBg = Renderer.Drawing.roundedRectangle(_rect, 3, 3, 3, 3);
                        g.FillPath(bgBrush, gpBg);
                        g.DrawPath(Renderer.Button.SilverNormalPen(), gpBg);
                        gpBg.Dispose();
                        bgBrush.Dispose();
                    }
                    if (_leftHost != null) {
                        p1 = new Point(_rect.X + (int)(_rect.Width / 2), _rect.Y + 2);
                        p2 = new Point(p1.X, _rect.Y + (int)(_rect.Height / 2) - 5);
                        gp.AddLine(p1, p2);
                        gp.AddArc(p1.X, _rect.Y + (int)(_rect.Height / 2) - 10, 10, 10, 180, -90);
                        p1 = new Point(_rect.X + (int)(_rect.Width / 2) + 5, _rect.Y + (int)(_rect.Height / 2));
                        p2 = new Point(_rect.Right - 3, p1.Y);
                        gp.AddLine(p1, p2);
                    } else {
                        p1 = new Point(_rect.X + 2, _rect.Y + (int)(_rect.Height / 2));
                        p2 = new Point(_rect.Right - 3, p1.Y);
                        gp.AddLine(p1, p2);
                    }
                    g.DrawPath(Pens.Black, gp);
                    p1 = new Point(p2.X - 4, p2.Y - 4);
                    g.DrawLine(Pens.Black, p1, p2);
                    p1 = new Point(p2.X - 4, p2.Y + 4);
                    g.DrawLine(Pens.Black, p1, p2);
                }
                public override DragHelper.DragEffect drawDrag(Graphics g) {
                    return DragHelper.DragEffect.None;
                }
                public override void mouseDoubleClick() {
                    return;
                }
                public override bool mouseMove(MouseEventArgs e) {
                    _currentDrag = e.Location;
                    if (!_owner.Bounds.Contains(e.Location)) return false;
                    bool stateChanged = false;
                    if (_rect.Contains(e.Location)) {
                        if (!_onHover) {
                            _onHover = true;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost != null) {
                            if (_owner._hoverHost != this) {
                                _owner._hoverHost._onHover = false;
                                _owner._hoverHost._onPartHover = false;
                            }
                        }
                        _owner._hoverHost = this;
                    } else {
                        if (_onHover || _onPartHover) {
                            _onHover = false;
                            _onPartHover = false;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                    }
                    return stateChanged;
                }
                public override bool mouseDown(MouseEventArgs e) {
                    if (e.Button == MouseButtons.Right) {
                        onMouseRightDown(new EventArgs());
                        return false;
                    }
                    bool stateChanged = false;
                    if (e.Button == MouseButtons.Left) {
                        if (_onHover) {
                            if (!_onPressed) {
                                _onPressed = true;
                                stateChanged = true;
                                _startDrag = e.Location;
                            }
                        }
                    }
                    return stateChanged;
                }
                public override bool mouseUp(MouseEventArgs e) {
                    if (!(_onPressed || _onPartPressed)) return false;
                    _onPressed = false;
                    _onPartPressed = false;
                    return true;
                }
            }
            /// <summary>
            /// Represent the host for a DataGridGroup object.
            /// </summary>
            private class DataGridGroupHost : ObjectHost {
                public DataGridGroupHost(ColumnHeaderControl owner, DataGridGroup group) : base(owner) {
                    _host = group;
                }
                public override void draw(Graphics g) {
                    LinearGradientBrush bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                    LinearGradientBrush closeBrush = new LinearGradientBrush(_partRect, Color.Black, Color.White, LinearGradientMode.Vertical);
                    Pen borderPen, closePen;
                    StringFormat sf = new StringFormat();
                    DataGridGroup aGroup = (DataGridGroup)_host;
                    GraphicsPath gpBg = Renderer.Drawing.roundedRectangle(_rect, 3, 3, 3, 3);
                    GraphicsPath gpClose = Renderer.Drawing.roundedRectangle(_partRect, 0, 3, 0, 3);
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                    if (_onPressed) {
                        bgBrush.InterpolationColors = Renderer.Button.SilverPressed();
                        closeBrush.InterpolationColors = Renderer.Button.RedNormal();
                        borderPen = Renderer.Button.SilverPressedPen();
                        closePen = Renderer.Button.RedNormalPen();
                    } else {
                        if (_onHover) {
                            if (_onPartPressed) {
                                bgBrush.InterpolationColors = Renderer.Button.SilverNormal();
                                closeBrush.InterpolationColors = Renderer.Button.RedPressed();
                                borderPen = Renderer.Button.SilverNormalPen();
                                closePen = Renderer.Button.RedPressedPen();
                            } else {
                                if (_onPartHover) {
                                    bgBrush.InterpolationColors = Renderer.Button.SilverNormal();
                                    closeBrush.InterpolationColors = Renderer.Button.RedHLited();
                                    borderPen = Renderer.Button.SilverNormalPen();
                                    closePen = Renderer.Button.RedHLitedPen();
                                } else {
                                    bgBrush.InterpolationColors = Renderer.Button.SilverHLited();
                                    closeBrush.InterpolationColors = Renderer.Button.RedNormal();
                                    borderPen = Renderer.Button.SilverHLitedPen();
                                    closePen = Renderer.Button.RedNormalPen();
                                }
                            }
                        } else {
                            bgBrush.InterpolationColors = Renderer.Button.SilverNormal();
                            closeBrush.InterpolationColors = Renderer.Button.RedNormal();
                            borderPen = Renderer.Button.SilverNormalPen();
                            closePen = Renderer.Button.RedNormalPen();
                        }
                    }
                    g.FillPath(bgBrush, gpBg);
                    g.FillPath(closeBrush, gpClose);
                    Rectangle sRect = _partRect;
                    sRect.X += 1;
                    sRect.Y += 1;
                    Renderer.Drawing.drawCross(g, sRect, 8, Color.White, true);
                    Renderer.Drawing.drawCross(g, _partRect, 8, Color.Black, true);
                    g.DrawLine(closePen, _partRect.X, _partRect.Y, _partRect.X, _partRect.Bottom - 1);
                    g.DrawPath(borderPen, gpBg);
                    g.DrawString(aGroup.Header.Text, _owner._owner._headerFont, (_owner.Enabled ? Renderer.Column.TextBrush() : Renderer.Drawing.DisabledTextBrush()), _rect, sf);
                    bgBrush.Dispose();
                    closeBrush.Dispose();
                    borderPen.Dispose();
                    closePen.Dispose();
                    gpBg.Dispose();
                    gpClose.Dispose();
                }
                public override DragHelper.DragEffect drawDrag(Graphics g) {
                    Rectangle r = _rect;
                    Rectangle pr = _partRect;
                    int dx, dy;
                    Point p = _currentDrag;
                    dx = p.X - _startDrag.X;
                    dy = p.Y - _startDrag.Y;
                    if (_rect.X + dx < 0) dx = -_rect.X;
                    if (_rect.Right + dx > _owner._groupArea.Right) dx = _owner._groupArea.Right - _rect.Right;
                    if (_rect.Y + dy < 0) dy = -_rect.Y;
                    if (_rect.Bottom + dy > _owner._groupArea.Bottom) dy = _owner._groupArea.Bottom - _rect.Bottom;
                    r.X += dx;
                    r.Y += dy;
                    pr.X += dx;
                    pr.Y += dy;
                    // Drawing
                    LinearGradientBrush bgBrush = new LinearGradientBrush(r, Color.Black, Color.White, LinearGradientMode.Vertical);
                    LinearGradientBrush closeBrush = new LinearGradientBrush(pr, Color.Black, Color.White, LinearGradientMode.Vertical);
                    Pen borderPen, closePen;
                    StringFormat sf = new StringFormat();
                    DataGridGroup aGroup = (DataGridGroup)_host;
                    GraphicsPath gpBg = Renderer.Drawing.roundedRectangle(r, 3, 3, 3, 3);
                    GraphicsPath gpClose = Renderer.Drawing.roundedRectangle(pr, 0, 3, 0, 3);
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                    bgBrush.InterpolationColors = Renderer.Button.SilverNormal(127);
                    closeBrush.InterpolationColors = Renderer.Button.RedNormal(127);
                    borderPen = Renderer.Button.SilverNormalPen(127);
                    closePen = Renderer.Button.RedNormalPen(127);
                    g.FillPath(bgBrush, gpBg);
                    g.FillPath(closeBrush, gpClose);
                    Rectangle sRect = pr;
                    sRect.X += 1;
                    sRect.Y += 1;
                    Renderer.Drawing.drawCross(g, sRect, 8, Color.White, true);
                    Renderer.Drawing.drawCross(g, pr, 8, Color.Black, true);
                    g.DrawLine(closePen, pr.X, pr.Y, pr.X, pr.Bottom - 1);
                    g.DrawPath(borderPen, gpBg);
                    g.DrawString(aGroup.Header.Text, _owner._owner._headerFont, (_owner.Enabled ? Renderer.Column.TextBrush() : Renderer.Drawing.DisabledTextBrush()), r, sf);
                    bgBrush.Dispose();
                    closeBrush.Dispose();
                    borderPen.Dispose();
                    closePen.Dispose();
                    gpBg.Dispose();
                    gpClose.Dispose();
                    return _de;
                }
                public override void mouseDoubleClick() {
                    return;
                }
                public override bool mouseMove(MouseEventArgs e) {
                    _currentDrag = e.Location;
                    if (!_owner.Bounds.Contains(e.Location)) return false;
                    bool stateChanged = false;
                    if (_rect.Contains(e.Location)) {
                        if (_partRect.Width > 0 && _owner._draggedHost == null) {
                            if (_partRect.Contains(e.Location)) {
                                if (!_onPartHover) {
                                    _onPartHover = true;
                                    stateChanged = true;
                                }
                            } else {
                                if (_onPartHover) {
                                    _onPartHover = false;
                                    stateChanged = true;
                                }
                            }
                        }
                        if (!_onHover) {
                            _onHover = true;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost != null) {
                            if (_owner._hoverHost != this) {
                                _owner._hoverHost._onHover = false;
                                _owner._hoverHost._onPartHover = false;
                            }
                        }
                        _owner._hoverHost = this;
                    } else {
                        if (_onHover || _onPartHover) {
                            _onHover = false;
                            _onPartHover = false;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                    }
                    if (_owner._hoverHost == null) {
                        _de = DragHelper.DragEffect.Stop;
                    } else {
                        if (_owner._hoverHost == this) {
                            _de = DragHelper.DragEffect.None;
                        } else {
                            if (_owner._hoverHost.Host is DataGridGroup) {
                                _de = DragHelper.DragEffect.Swap;
                            } else {
                                if (_owner._hoverHost.Host == "C") {
                                    _de = DragHelper.DragEffect.Insert;
                                } else {
                                    _de = DragHelper.DragEffect.Stop;
                                }
                            }
                        }
                    }
                    return stateChanged;
                }
                public override bool mouseDown(MouseEventArgs e) {
                    if (e.Button == MouseButtons.Right) {
                        onMouseRightDown(new EventArgs());
                        return false;
                    }
                    bool stateChanged = false;
                    if (e.Button == MouseButtons.Left) {
                        if (_onHover) {
                            if (_onPartHover) {
                                if (!_onPartPressed) {
                                    _onPartPressed = true;
                                    stateChanged = true;
                                }
                            } else {
                                if (!_onPressed) {
                                    _onPressed = true;
                                    stateChanged = true;
                                    _startDrag = e.Location;
                                    _owner._draggedHost = this;
                                }
                            }
                        }
                    }
                    return stateChanged;
                }
                public override bool mouseUp(MouseEventArgs e) {
                    if (!(_onPressed || _onPartPressed)) return false;
                    if (e.Button == MouseButtons.Left) {
                        if (_owner._draggedHost == this) {
                            _onPressed = false;
                            // This host is beeing dropped.
                            if (_owner._hoverHost != null) {
                                if (_owner._hoverHost != this) {
                                    if (_owner._hoverHost is ConnectorHost) {
                                        // This host is dropped over a connector between groups.
                                        int connIdx = _owner._connectors.IndexOf((ConnectorHost)_owner._hoverHost);
                                        DataGridGroup group = (DataGridGroup)_host;
                                        _owner._owner._groups.moveGroup(connIdx, group);
                                    } else if (_owner._hoverHost is DataGridGroupHost) {
                                        // This host is dropped over a group.
                                        DataGridGroup group1 = (DataGridGroup)_host;
                                        DataGridGroup group2 = (DataGridGroup)_owner._hoverHost._host;
                                        _owner._owner._groups.swapGroup(group1, group2);
                                    }
                                }
                            } else {
                                if (_owner._groupArea.Contains(e.Location)) {
                                    if (e.Location.Y <= _owner._groupArea.Y + 5) {
                                        // Move group to the first order.
                                        DataGridGroup group = (DataGridGroup)_host;
                                        _owner._owner._groups.moveFirst(group);
                                    } else if (e.Location.Y >= _owner._groupArea.Bottom - 5) {
                                        DataGridGroup group = (DataGridGroup)_host;
                                        _owner._owner._groups.moveLast(group);
                                    }
                                }
                            }
                        } else {
                            // User presses the close button of the group.
                            _onPartPressed = false;
                            if (_onPartHover) {
                                // Removes the group from the DataGrid.
                                DataGridGroup group = (DataGridGroup)_host;
                                _owner._owner._groups.Remove(group);
                            }
                        }
                    } else {
                        _onPressed = false;
                        _onPartPressed = false;
                    }
                    return true;
                }
            }
            /// <summary>
            /// Represent the host for a DataGridHeader object.
            /// </summary>
            private class ColumnHeaderHost : ObjectHost {
                ColumnFilterHandle _filterHandle;
                public ColumnHeaderHost(ColumnHeaderControl owner, DataGridHeader header) : base(owner) {
                    _host = header;
                    _filterHandle = new ColumnFilterHandle(header);
                }
                public ColumnFilterHandle FilterHandle { get { return _filterHandle; } }
                public override int MinResize {
                    get {
                        DataGridHeader header = (DataGridHeader)_host;
                        int minWidth = 10;
                        if (header.Image != null) minWidth = minWidth + 18;
                        if (header.EnableFiltering && header.SortOrder != SortOrder.None) minWidth = minWidth + 10;
                        if (header.EnableFiltering) minWidth = minWidth + 10;
                        if (minWidth < 25) minWidth = 25;
                        return minWidth;
                    }
                }
                public override int MaxResize {
                    get {
                        int maxWidth = 0;
                        maxWidth = _owner.Width - (_rect.Left + 5);
                        if (_owner._owner._vscroll.Visible) {
                            maxWidth = maxWidth - _owner._owner._vscroll.Width;
                        } else {
                            if (_owner._owner._showColumnOptions) maxWidth = maxWidth - 10;
                        }
                        return maxWidth;
                    }
                }
                public override void mouseDoubleClick() { }
                public override bool mouseMove(MouseEventArgs e) {
                    _currentDrag = e.Location;
                    DataGridHeader header = (DataGridHeader)_host;
                    if (!header.Visible) return false;
                    if (!_owner.Bounds.Contains(e.Location)) return false;
                    bool stateChanged = false;
                    if (_rect.Contains(e.Location)) {
                        if (_partRect.Width > 0 && _owner._draggedHost == null) {
                            if (_partRect.Contains(e.Location)) {
                                if (!_onPartHover) {
                                    _onPartHover = true;
                                    stateChanged = true;
                                }
                            } else {
                                if (_onPartHover) {
                                    _onPartHover = false;
                                    stateChanged = true;
                                }
                            }
                        }
                        if (!_onHover) {
                            _onHover = true;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost != null) {
                            if (_owner._hoverHost != this) {
                                _owner._hoverHost._onHover = false;
                                _owner._hoverHost._onPartHover = false;
                            }
                        }
                        _owner._hoverHost = this;
                    } else {
                        if (_onHover || _onPartHover) {
                            _onHover = false;
                            _onPartHover = false;
                            stateChanged = true;
                        }
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                    }
                    if (_rightHost != null) stateChanged = stateChanged || _rightHost.mouseMove(e);
                    if (_owner._draggedHost == this) {
                        if (_owner._hoverHost == null) {
                            if (_owner._groupArea.Contains(e.Location)) {
                                if (_owner._owner._groups.Contains((DataGridHeader)_host)) _de = DragHelper.DragEffect.Stop;
                                else _de = DragHelper.DragEffect.Insert;
                            } else {
                                _de = DragHelper.DragEffect.Stop;
                            }
                        } else {
                            if (_owner._hoverHost == this) {
                                _de = DragHelper.DragEffect.None;
                            } else {
                                if (_owner._hoverHost is DataGridGroupHost) {
                                    if (_owner._owner._groups.Contains((DataGridHeader)_host)) _de = DragHelper.DragEffect.Stop;
                                    else _de = DragHelper.DragEffect.Swap;
                                } else {
                                    if (_owner._hoverHost is ConnectorHost) {
                                        if (_owner._owner._groups.Contains((DataGridHeader)_host)) _de = DragHelper.DragEffect.Stop;
                                        else _de = DragHelper.DragEffect.Insert;
                                    } else {
                                        if (_owner._hoverHost is SeparatorHost) _de = DragHelper.DragEffect.Insert;
                                        else _de = DragHelper.DragEffect.Swap;
                                    }
                                }
                            }
                        }
                    }
                    return stateChanged;
                }
                public override bool mouseDown(MouseEventArgs e) {
                    DataGridHeader header = (DataGridHeader)_host;
                    if (!header.Visible) return false;
                    if (e.Button == MouseButtons.Right) {
                        onMouseRightDown(new EventArgs());
                        return false;
                    }
                    bool stateChanged = false;
                    if (e.Button == MouseButtons.Left) {
                        if (_onHover) {
                            if (_onPartHover) {
                                if (!_onPartPressed) {
                                    _onPartPressed = true;
                                    onPartDown(new EventArgs());
                                }
                            } else {
                                if (!_onPressed) {
                                    _onPressed = true;
                                    stateChanged = true;
                                    _startDrag = e.Location;
                                    _owner._draggedHost = this;
                                }
                            }
                        }
                    }
                    if (_rightHost != null) stateChanged = stateChanged || _rightHost.mouseDown(e);
                    return stateChanged;
                }
                public override bool mouseUp(MouseEventArgs e) {
                    DataGridHeader header = (DataGridHeader)_host;
                    if (!header.Visible) return false;
                    _de = DragHelper.DragEffect.None;
                    if (!(_onPressed || _onPartPressed)) return false;
                    _onPressed = false;
                    _onPartPressed = false;
                    _owner._draggedHost = null;
                    if (_owner._columnArea.Contains(e.Location)) {
                        // Host is dropped inside column's area.
                        if (_startDrag.X == _currentDrag.X) {
                            // Performs sort operation.
                            if (header.EnableSorting) {
                                if (header.SortOrder == SortOrder.Ascending) header.SortOrder = SortOrder.Descending;
                                else header.SortOrder = SortOrder.Ascending;
                            }
                        } else {
                            // Check for hovering host.
                            if (_owner._hoverHost != this) {
                                if (_owner._hoverHost != _owner._optionHost) {
                                    if (_owner._hoverHost == null) {
                                        // Move the column to the last position.
                                        //_owner.moveHeader(this);
                                    } else {
                                        if (_owner._hoverHost is SeparatorHost) {
                                            // Moves header along the headers arrangement to the target host.
                                            ObjectHost target = _owner._hoverHost.LeftHost;
                                            //if (target != null) _owner.moveHeader(this, target);
                                        } else {
                                            // Swap the host to the target.
                                            ObjectHost target = _owner._hoverHost;
                                            //_owner.swapHeaderHost(this, target);
                                        }
                                    }
                                }
                            }
                        }
                    } else if (_owner._groupArea.Contains(e.Location)) {
                        // Check if the header is already used as a group.
                        if (!_owner._owner._groups.Contains(header)) {
                            if (_owner._hoverHost != null) {
                                if (_owner._hoverHost.Host is DataGridGroup) {
                                    // Swaps the pointed group with this header.
                                    DataGridGroup dg = (DataGridGroup)_owner._hoverHost.Host;
                                    dg.Header = header;
                                } else {
                                    // Inserts the header as new group in the position of the connector.
                                    int connIdx = _owner._connectors.IndexOf((ConnectorHost)_owner._hoverHost);
                                    DataGridGroup dg = new DataGridGroup();
                                    dg.Header = header;
                                    _owner._owner._groups.Insert(connIdx, dg);
                                }
                            } else {
                                DataGridGroup dg = new DataGridGroup();
                                dg.Header = header;
                                _owner._owner._groups.Add(dg);
                            }
                            return true;
                        }
                    }
                    return false;
                }
                public override void draw(Graphics g) {
                    DataGridHeader _column = (DataGridHeader)_host;
                    if (!_column.Visible) return;
                    Rectangle rectTxt = _rect;
                    StringFormat sf = new StringFormat();
                    Rectangle rectImg = _rect;
                    int sortSignX = _rect.Right - 8;
                    if (_column.Image != null) {
                        rectImg.X = rectTxt.X;
                        rectImg.Size = Renderer.Drawing.scaleImage(_column.Image, 16);
                        rectImg.Y = (_rect.Height - rectImg.Height) / 2;
                        rectImg.X = rectTxt.X + 18;
                        rectImg.Width = rectTxt.Width - 18;
                    }
                    if (_column.EnableFiltering) {
                        rectTxt.Width -= 11;
                        sortSignX = _rect.Right - 18;
                    }
                    if (_column.SortOrder != SortOrder.None && _column.EnableSorting) rectTxt.Width = rectTxt.Width - 9;
                    sf.LineAlignment = StringAlignment.Center;
                    switch (_column.TextAlign) {
                        case HorizontalAlignment.Center:
                            sf.Alignment = StringAlignment.Center;
                            break;
                        case HorizontalAlignment.Left:
                            sf.Alignment = StringAlignment.Near;
                            break;
                        case HorizontalAlignment.Right:
                            sf.Alignment = StringAlignment.Far;
                            break;
                    }
                    sf.FormatFlags = sf.FormatFlags | StringFormatFlags.NoWrap;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    LinearGradientBrush bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                    bgBrush.InterpolationColors = Ai.Renderer.Column.NormalBlend();
                    g.FillRectangle(bgBrush, _rect);
                    bgBrush.Dispose();
                    bgBrush = null;
                    if (_owner.Enabled) {
                        LinearGradientBrush splitBrush = null;
                        Rectangle splitRect = new Rectangle(_rect.Right - 10, _rect.Y, 10, _rect.Height);
                        Pen linePen = Ai.Renderer.Column.NormalBorderPen();
                        if (_onHover) {
                            bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                            if (_column.EnableFiltering) {
                                splitBrush = new LinearGradientBrush(splitRect, Color.Black, Color.White, LinearGradientMode.Vertical);
                                if (_onPartPressed) {
                                    splitBrush.InterpolationColors = Ai.Renderer.Column.PressedBlend();
                                } else {
                                    if (_onPartHover) splitBrush.InterpolationColors = Ai.Renderer.Column.HLitedDropDownBlend();
                                    else splitBrush.InterpolationColors = Ai.Renderer.Column.HLitedBlend();
                                }
                            }
                            if (_onPressed) bgBrush.InterpolationColors = Ai.Renderer.Column.PressedBlend();
                            else bgBrush.InterpolationColors = Ai.Renderer.Column.HLitedBlend();
                        } else {
                            if (_selected) {
                                bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
                                bgBrush.InterpolationColors = Ai.Renderer.Column.SelectedBlend();
                            }
                        }
                        if (bgBrush != null) {
                            linePen = Ai.Renderer.Column.ActiveBorderPen();
                            g.FillRectangle(bgBrush, _rect);
                            bgBrush.Dispose();
                        }
                        if (splitBrush != null) {
                            g.FillRectangle(splitBrush, splitRect);
                            splitBrush.Dispose();
                        }
                        if (_column.EnableFiltering) {
                            g.DrawLine(linePen, _rect.Right - 10, _rect.Y + 1, _rect.Right - 10, _rect.Bottom - 2);
                            Ai.Renderer.Drawing.drawTriangle(g, _rect.Right - 8, _rect.Y + (int)((_rect.Height - 6) / 2),
                                Color.FromArgb(21, 66, 139), Color.White, Ai.Renderer.Drawing.TriangleDirection.Down);
                        }
                    } else {
                        if (_column.EnableFiltering) {
                            g.DrawLine(Pens.Gray, _rect.Right - 10, _rect.Y + 1, _rect.Right - 10, _rect.Bottom - 2);
                            Ai.Renderer.Drawing.drawTriangle(g, _rect.Right - 8, _rect.Y + (int)((_rect.Height - 6) / 2),
                                Color.Gray, Color.White, Ai.Renderer.Drawing.TriangleDirection.Down);
                        }
                    }
                    if (_column.Image != null) {
                        if (_owner.Enabled) g.DrawImage(_column.Image, rectImg);
                        else Ai.Renderer.Drawing.grayScaledImage(_column.Image, rectImg, g);
                    }
                    g.DrawString(_column.Text, _owner.Font, (_owner.Enabled ? Ai.Renderer.Column.TextBrush() : Ai.Renderer.Drawing.DisabledTextBrush()), rectTxt, sf);
                    if (_column.EnableSorting) {
                        switch (_column.SortOrder) {
                            case SortOrder.Ascending:
                                Ai.Renderer.Drawing.drawTriangle(g, sortSignX, _rect.Y + (int)((_rect.Height - 6) / 2),
                                    (_owner.Enabled ? Color.FromArgb(21, 66, 139) : Color.Gray), Color.White, Ai.Renderer.Drawing.TriangleDirection.Up);
                                break;
                            case SortOrder.Descending:
                                Ai.Renderer.Drawing.drawTriangle(g, sortSignX, _rect.Y + (int)((_rect.Height - 6) / 2),
                                    (_owner.Enabled ? Color.FromArgb(21, 66, 139) : Color.Gray), Color.White, Ai.Renderer.Drawing.TriangleDirection.Down);
                                break;
                        }
                    }
                    if (_onPressed) Ai.Renderer.Column.drawPressedShadow(g, _rect, 3);
                    if (_rightHost != null) _rightHost.draw(g);
                }
                public override DragHelper.DragEffect drawDrag(Graphics g) {
                    int dx;
                    int dy;
                    Point p = _currentDrag;
                    string strText = "";
                    DataGridHeader column = (DataGridHeader)_host;
                    Rectangle r = _rect;
                    dx = p.X - _startDrag.X;
                    if (_rect.X + dx < 0) dx = -_rect.X;
                    if (_rect.Right + dx > _owner._groupArea.Right) dx = _owner._groupArea.Right - _rect.Right;
                    r.X += dx;
                    dy = p.Y - _startDrag.Y;
                    if (_rect.Y + dy < 0) dy = -_rect.Y;
                    if (_rect.Bottom + dy > _owner._columnArea.Bottom) dy = _owner._columnArea.Bottom - _rect.Bottom;
                    r.Y += dy;
                    strText = column.Text;
                    SizeF txtSize = g.MeasureString(strText, _owner._owner._headerFont);
                    r.Width = (int)txtSize.Width + 2;
                    r.Height = (int)txtSize.Height + 2;
                    GraphicsPath border = Renderer.Drawing.roundedRectangle(r, 2, 2, 2, 2);
                    StringFormat sf = new StringFormat();
                    LinearGradientBrush bgBrush = new LinearGradientBrush(r, Color.Black, Color.White, LinearGradientMode.Vertical);
                    bgBrush.InterpolationColors = Renderer.Button.SilverNormal(97);
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.FillPath(bgBrush, border);
                    g.DrawPath(Renderer.Button.NormalBorderPen(), border);
                    g.DrawString(strText, _owner._owner._headerFont, new SolidBrush(Color.FromArgb(97, 0, 0, 0)), r, sf);
                    bgBrush.Dispose();
                    sf.Dispose();
                    border.Dispose();
                    if (_owner._hoverHost == null) {
                        if (_owner._groupArea.Contains(p)) {
                            if (_owner._owner._groups.Contains((DataGridHeader)_host)) return DragHelper.DragEffect.Stop;
                            else return DragHelper.DragEffect.Insert;
                        } else {
                            return DragHelper.DragEffect.Stop;
                        }
                    } else {
                        if (_owner._hoverHost == this) {
                            return DragHelper.DragEffect.None;
                        } else {
                            if (_owner._hoverHost is DataGridGroupHost) {
                                if (_owner._owner._groups.Contains((DataGridHeader)_host)) return DragHelper.DragEffect.Stop;
                                else return DragHelper.DragEffect.Swap;
                            } else {
                                if (_owner._hoverHost is ConnectorHost) {
                                    if (_owner._owner._groups.Contains((DataGridHeader)_host)) return DragHelper.DragEffect.Stop;
                                    else return DragHelper.DragEffect.Insert;
                                } else {
                                    if (_owner._hoverHost is SeparatorHost) return DragHelper.DragEffect.Insert;
                                    else return DragHelper.DragEffect.Swap;
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Represent the host for option button of the ColumnHeaders.
            /// </summary>
            private class OptionHost : ObjectHost {
                public OptionHost(ColumnHeaderControl owner) : base(owner) {
                    _rect.Width = 15;
                    _rect.Height = 15;
                }
                public override int Height {
                    get { return base.Height; }
                    set { base.Height = 15; }
                }
                public override int Width {
                    get { return base.Width; }
                    set { base.Width = 15; }
                }
                public override void draw(Graphics g) {
                    Rectangle rectBG = new Rectangle(_rect.X, _owner._columnArea.Y, 15, _owner._columnArea.Height);
                    LinearGradientBrush bgBrush = new LinearGradientBrush(rectBG, Color.White, Color.Black, LinearGradientMode.Vertical);
                    g.FillRectangle(bgBrush, rectBG);
                    bgBrush.Dispose();
                    if (_onHover || _onPressed) {
                        GraphicsPath bgPath = Renderer.Drawing.roundedRectangle(_rect, 2, 2, 2, 2);
                        bgBrush = new LinearGradientBrush(_rect, Color.White, Color.Black, LinearGradientMode.Vertical);
                        if (_onHover) bgBrush.InterpolationColors = Renderer.Button.HLitedBlend();
                        else bgBrush.InterpolationColors = Renderer.Button.PressedBlend();
                        g.FillPath(bgBrush, bgPath);
                        if (_onHover) g.DrawPath(Renderer.Button.HLitedBorderPen(), bgPath);
                        else g.DrawPath(Renderer.Button.SelectedBorderPen(), bgPath);
                        bgBrush.Dispose();
                        bgPath.Dispose();
                    }
                    Renderer.Drawing.drawTriangle(g, _rect, Color.FromArgb(86, 125, 176), Color.FromArgb(63, 0, 0, 0), Ai.Renderer.Drawing.TriangleDirection.Down);
                }
                public override DragHelper.DragEffect drawDrag(Graphics g) { return DragHelper.DragEffect.None; }
                public override void mouseDoubleClick() { return; }
                public override bool mouseMove(MouseEventArgs e) {
                    bool changed = false;
                    if (_rect.Contains(e.Location)) {
                        if (!_onHover) {
                            _onHover = true;
                            changed = true;
                        }
                        if (_owner._hoverHost != null) {
                            if (_owner._hoverHost != this) {
                                _owner._hoverHost._onHover = false;
                                _owner._hoverHost._onPartHover = false;
                            }
                        }
                        _owner._hoverHost = this;
                    } else {
                        if (_onHover) {
                            _onHover = false;
                            changed = true;
                        }
                        if (_owner._hoverHost == this) _owner._hoverHost = null;
                    }
                    return changed;
                }
                public override bool mouseDown(MouseEventArgs e) {
                    if (_onHover) {
                        if (e.Button == MouseButtons.Left) {
                            _onPressed = true;
                            OptionControl ctrl = new OptionControl(_owner, _owner.Font);
                            ToolStripControlHost aHost = new ToolStripControlHost(ctrl);
                            aHost.Padding = new Padding(0);
                            _owner._toolStrip.Items.Clear();
                            _owner._toolStrip.Items.Add(aHost);
                            Point scrLoc = _owner.PointToScreen(new Point(_rect.Right - ctrl.Width, _owner.Height + 2));
                            if (scrLoc.X < 0) scrLoc = _owner.PointToScreen(new Point(_rect.X, _owner.Height + 2));
                            if (scrLoc.Y + ctrl.Height + 5 > Screen.PrimaryScreen.WorkingArea.Height) 
                                scrLoc.Y -= (ctrl.Height + 5 + _owner._columnArea.Height);
                            _owner._resumePainting = false;
                            PaintEventArgs pe = null;
                            pe = new PaintEventArgs(_owner.CreateGraphics(), new Rectangle(0, 0, _owner.Width, _owner.Height));
                            _owner.InvokePaint(_owner, pe);
                            pe.Dispose();
                            _owner._toolStrip.Show(scrLoc);
                        }
                    }
                    return false;
                }
                public override bool mouseUp(MouseEventArgs e) {
                    if (_onPressed) {
                        _onPressed = false;
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// Control to display header options in the popup window.
            /// </summary>
            /// <remarks>This control contains two sections, first for column visibility, and second for column frozen.</remarks>
            private class OptionControl : System.Windows.Forms.Control {
                /// <summary>
                /// Host to control item options operations.
                /// </summary>
                private class ItemHost {
                    Rectangle _rect;
                    ColumnHeader _column;
                    bool _checked = false;
                    int _displayFor = 0;
                    bool _onHover = false;
                    OptionControl _owner;
                    public ItemHost(ColumnHeader column, int displayFor, OptionControl owner) {
                        _column = column;
                        _displayFor = displayFor;
                        _owner = owner;
                        if (_displayFor == 0) _checked = _column.Visible;
                        else _checked = _column.Frozen;
                    }
                    /// <summary>
                    /// Determine x location of the host.
                    /// </summary>
                    public int X {
                        get { return _rect.X; }
                        set { _rect.X = value; }
                    }
                    /// <summary>
                    /// Determine y location of the host.
                    /// </summary>
                    public int Y {
                        get { return _rect.Y; }
                        set { _rect.Y = value; }
                    }
                    /// <summary>
                    /// Determine the size of the host.
                    /// </summary>
                    public Size Size {
                        get { return _rect.Size; }
                        set { _rect.Size = value; }
                    }
                    /// <summary>
                    /// Determine the bounding rectangle of the host.
                    /// </summary>
                    public Rectangle Bounds {
                        get { return _rect; }
                        set { _rect = value; }
                    }
                    /// <summary>
                    /// Determine checked state of the host.
                    /// </summary>
                    public bool Checked {
                        get { return _checked; }
                        set { _checked = value; }
                    }
                    /// <summary>
                    /// Gets a ColumnHeader object contrined within the host.
                    /// </summary>
                    public ColumnHeader Column { get { return _column; } }
                    /// <summary>
                    /// Gets left location of the host.
                    /// </summary>
                    public int Left { get { return _rect.X; } }
                    /// <summary>
                    /// Gets top location of the host.
                    /// </summary>
                    public int Top { get { return _rect.Y; } }
                    /// <summary>
                    /// Gets the rightmost location of the host.
                    /// </summary>
                    public int Right { get { return _rect.Right; } }
                    /// <summary>
                    /// Gets the bottommost location of the host.
                    /// </summary>
                    public int Bottom { get { return _rect.Bottom; } }
                    /// <summary>
                    /// Gets a display functions of the host.
                    /// </summary>
                    /// <remarks>Returns 0 for column visibility, and 1 for column frozen.</remarks>
                    public int DisplayFor { get { return _displayFor; } }
                    /// <summary>
                    /// Test whether the mouse pointer is moved over the host.
                    /// </summary>
                    public bool mouseMove(MouseEventArgs e) {
                        if (_rect.Contains(e.Location)) {
                            if (!_onHover) {
                                _onHover = true;
                                _owner._hoverHost = this;
                                return true;
                            }
                        } else {
                            if (_onHover) {
                                _owner._hoverHost = null;
                                _onHover = false;
                                return true;
                            }
                        }
                        return false;
                    }
                    /// <summary>
                    /// Test whether the mouse left button is pressed over the host.
                    /// </summary>
                    public bool mouseDown() {
                        if (_onHover) {
                            _checked = !_checked;
                            return true;
                        }
                        return false;
                    }
                    /// <summary>
                    /// Test whether the mouse pointer is leave the host.
                    /// </summary>
                    public bool mouseLeave() {
                        if (_onHover) {
                            _onHover = false;
                            _owner._hoverHost = null;
                            return true;
                        }
                        return false;
                    }
                    /// <summary>
                    /// Draw the on the specified graphics object.
                    /// </summary>
                    /// <param name="g">Graphics object where the host must be drawn.</param>
                    public void draw(Graphics g) {
                        Rectangle chkRect = default(Rectangle);
                        Rectangle txtRect = default(Rectangle);
                        chkRect = new Rectangle(_rect.X, _rect.Y, 22, _rect.Height);
                        txtRect = new Rectangle(_rect.X + 22, _rect.Y, _rect.Width - 22, _rect.Height);
                        if (_onHover) Ai.Renderer.Button.draw(g, _rect, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, false, true);
                        if (_checked) Ai.Renderer.CheckBox.drawCheck(g, chkRect, CheckState.Checked);
                        g.DrawString(_column.Text, _owner.Font, Ai.Renderer.Drawing.NormalTextBrush(), txtRect, _owner.txtFormat);
                    }
                }
                StringFormat txtFormat = new StringFormat();
                // Visibility
                Rectangle _txtVisibilityRect = new Rectangle(0, 0, 0, 0);
                Rectangle _chkVisibilityRect = new Rectangle(0, 0, 0, 0);
                CheckState _chkVisibilityState;
                bool _chkVisibilityHover = false;
                List<ItemHost> _itemsVisibility = new List<ItemHost>();
                VScrollBar _vscVisibility = new VScrollBar();
                // Freeze
                Rectangle _txtFreezeRect = new Rectangle(0, 0, 0, 0);
                Rectangle _chkFreezeRect = new Rectangle(0, 0, 0, 0);
                CheckState _chkFreezeState;
                bool _chkFreezeHover = false;
                List<ItemHost> _itemsFreeze = new List<ItemHost>();
                VScrollBar _vscFreeze = new VScrollBar();
                // Button OK
                Rectangle _btnRect;
                bool _btnHover = false;
                // Owner
                ColumnHeaderControl _owner;
                // Hover host
                ItemHost _hoverHost = null;
                public OptionControl(ColumnHeaderControl owner, Font font) {
                    Paint += OptionControl_Paint;
                    MouseWheel += OptionControl_MouseWheel;
                    MouseMove += OptionControl_MouseMove;
                    MouseLeave += OptionControl_MouseLeave;
                    MouseDown += OptionControl_MouseDown;
                    _owner = owner;
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    this.SetStyle(ControlStyles.ResizeRedraw, true);
                    this.Font = font;
                    // Setting up ...
                    int maxWidth = 0;
                    int itemWidth = 0;
                    ItemHost iHost = null;
                    // Populating hosts and gather maximum width
                    foreach (ColumnHeaderHost oh in _owner._headers) {
                        ColumnHeader ch = (ColumnHeader)oh.Host;
                        itemWidth = 0;
                        if (ch.EnableHidden) {
                            iHost = new ItemHost(ch, 0, this);
                            _itemsVisibility.Add(iHost);
                            itemWidth = TextRenderer.MeasureText(ch.Text, this.Font).Width + 5;
                        }
                        if (ch.EnableFrozen) {
                            iHost = new ItemHost(ch, 1, this);
                            _itemsFreeze.Add(iHost);
                            itemWidth = TextRenderer.MeasureText(ch.Text, this.Font).Width + 5;
                        }
                        if (maxWidth < itemWidth) maxWidth = itemWidth;
                    }
                    maxWidth = maxWidth + 22;
                    _vscVisibility.Visible = false;
                    _vscFreeze.Visible = false;
                    if (_itemsVisibility.Count + _itemsFreeze.Count > 20) {
                        _vscVisibility.Visible = _itemsVisibility.Count > 10;
                        if (_vscVisibility.Visible) _vscVisibility.Maximum = _itemsVisibility.Count - 10;
                        _vscFreeze.Visible = _itemsFreeze.Count > 10;
                        if (_vscFreeze.Visible) _vscFreeze.Maximum = _itemsFreeze.Count - 10;
                    }
                    if (_vscVisibility.Visible || _vscFreeze.Visible) {
                        if (maxWidth + _vscVisibility.Width + 4 < 110) maxWidth = 110 - (_vscVisibility.Width + 4);
                    } else {
                        if (maxWidth < 110) maxWidth = 110;
                    }
                    _vscVisibility.Left = maxWidth + 4;
                    _vscFreeze.Left = maxWidth + 4;
                    // Hosts size and x location
                    foreach (ItemHost ih in _itemsVisibility) {
                        ih.Size = new Size(maxWidth, 20);
                        ih.X = 2;
                    }
                    foreach (ItemHost ih in _itemsFreeze) {
                        ih.Size = new Size(maxWidth, 20);
                        ih.X = 2;
                    }
                    // y location
                    int y = 0;
                    if (_itemsVisibility.Count > 0) {
                        int i = 0;
                        int max = 0;
                        _chkVisibilityRect = new Rectangle(0, 0, 22, 22);
                        _txtVisibilityRect = new Rectangle(22, 0, TextRenderer.MeasureText("Visible", this.Font).Width + 5, 22);
                        y = 23;
                        _vscVisibility.Top = y;
                        i = 0;
                        max = (_vscVisibility.Visible ? 10 : _itemsVisibility.Count);
                        while (i < max) {
                            _itemsVisibility[i].Y = y;
                            y = _itemsVisibility[i].Bottom;
                            i++;
                        }
                        _vscVisibility.Height = max * 20;
                        y++;
                    }
                    if (_itemsFreeze.Count > 0) {
                        int i = 0;
                        int max = 0;
                        _chkFreezeRect = new Rectangle(0, y, 22, 22);
                        _txtFreezeRect = new Rectangle(22, y, TextRenderer.MeasureText("Freeze", this.Font).Width + 5, 22);
                        y = y + 23;
                        _vscFreeze.Top = y;
                        i = 0;
                        max = (_vscFreeze.Visible ? 10 : _itemsFreeze.Count);
                        while (i < max) {
                            _itemsFreeze[i].Y = y;
                            y = _itemsFreeze[i].Bottom;
                            i++;
                        }
                        _vscFreeze.Height = max * 20;
                        y++;
                    }
                    if (_vscVisibility.Visible || _vscFreeze.Visible) this.Width = _vscVisibility.Right;
                    else this.Width = maxWidth + 4;
                    this.Height = y + 28;
                    this.Controls.Add(_vscVisibility);
                    this.Controls.Add(_vscFreeze);
                    _btnRect = new Rectangle(this.Width - 80, y + 3, 75, 21);
                    checkCheckedState();
                    _vscVisibility.ValueChanged += vScroll_ValueChanged;
                    _vscFreeze.ValueChanged += vScroll_ValueChanged;
                    txtFormat.Alignment = StringAlignment.Near;
                    txtFormat.LineAlignment = StringAlignment.Center;
                    this.Invalidate();
                }
                private void vScroll_ValueChanged(object sender, EventArgs e) {
                    VScrollBar vsc = (VScrollBar)sender;
                    int y = vsc.Top;
                    int i = 0;
                    if (sender == _vscVisibility) {
                        while (i < 10) {
                            _itemsVisibility[i + vsc.Value].Y = y;
                            y = _itemsVisibility[i + vsc.Value].Bottom;
                            i++;
                        }
                    } else {
                        while (i < 10) {
                            _itemsFreeze[i + vsc.Value].Y = y;
                            y = _itemsFreeze[i + vsc.Value].Bottom;
                            i++;
                        }
                    }
                    this.Invalidate();
                }
                /// <summary>
                /// Change the check state of each section.
                /// </summary>
                private void checkCheckedState() {
                    int chkCount = 0;
                    foreach (ItemHost ih in _itemsVisibility) {
                        if (ih.Checked) chkCount = chkCount + 1;
                    }
                    if (chkCount == 0) _chkVisibilityState = CheckState.Unchecked;
                    else if (chkCount == _itemsVisibility.Count) _chkVisibilityState = CheckState.Checked;
                    else _chkVisibilityState = CheckState.Indeterminate;
                    chkCount = 0;
                    foreach (ItemHost ih in _itemsFreeze) {
                        if (ih.Checked) chkCount = chkCount + 1;
                    }
                    if (chkCount == 0) _chkFreezeState = CheckState.Unchecked;
                    else if (chkCount == _itemsFreeze.Count) _chkFreezeState = CheckState.Checked;
                    else _chkFreezeState = CheckState.Indeterminate;
                }
                private void OptionControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                        bool changed = false;
                        int i = 0;
                        if (_chkVisibilityHover) {
                            if (_chkVisibilityState == CheckState.Indeterminate || _chkVisibilityState == CheckState.Unchecked) {
                                _chkVisibilityState = CheckState.Checked;
                                foreach (ItemHost ih in _itemsVisibility) ih.Checked = true;
                            } else {
                                _chkVisibilityState = CheckState.Unchecked;
                                foreach (ItemHost ih in _itemsVisibility) ih.Checked = false;
                            }
                            changed = true;
                        }
                        if (!changed) {
                            if (_vscVisibility.Visible) {
                                i = 0;
                                while (i < 10) {
                                    changed = changed || _itemsVisibility[i + _vscVisibility.Value].mouseDown();
                                    i++;
                                }
                            } else {
                                foreach (ItemHost ih in _itemsVisibility) changed = changed || ih.mouseDown();
                            }
                        }
                        if (!changed) {
                            if (_chkFreezeHover) {
                                if (_chkFreezeState == CheckState.Indeterminate || _chkFreezeState == CheckState.Unchecked) {
                                    _chkFreezeState = CheckState.Checked;
                                    foreach (ItemHost ih in _itemsFreeze) ih.Checked = true;
                                } else {
                                    _chkFreezeState = CheckState.Unchecked;
                                    foreach (ItemHost ih in _itemsFreeze) ih.Checked = false;
                                }
                                changed = true;
                            }
                        }
                        if (!changed) {
                            if (_vscFreeze.Visible) {
                                i = 0;
                                while (i < 10) {
                                    changed = changed || _itemsFreeze[i + _vscFreeze.Value].mouseDown();
                                    i++;
                                }
                            } else {
                                foreach (ItemHost ih in _itemsFreeze) changed = changed || ih.mouseDown();
                            }
                        }
                        checkCheckedState();
                        if (!changed) {
                            if (_btnHover) {
                                // Applying all changes and close the popup window.
                                foreach (ItemHost ih in _itemsVisibility) ih.Column.Visible = ih.Checked;
                                foreach (ItemHost ih in _itemsFreeze) ih.Column.Frozen = ih.Checked;
                                _owner._toolStrip.Close();
                                return;
                            }
                        }
                        if (changed) this.Invalidate();
                    }
                }
                private void OptionControl_MouseLeave(object sender, System.EventArgs e) {
                    bool changed = false;
                    if (_chkVisibilityHover) {
                        _chkVisibilityHover = false;
                        changed = true;
                    }
                    if (_chkFreezeHover) {
                        _chkFreezeHover = false;
                        changed = true;
                    }
                    if (_btnHover) {
                        _btnHover = false;
                        changed = true;
                    }
                    foreach (ItemHost ih in _itemsVisibility) changed = changed || ih.mouseLeave();
                    foreach (ItemHost ih in _itemsFreeze) changed = changed || ih.mouseLeave();
                    if (changed) this.Invalidate();
                }
                private void OptionControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
                    bool changed = false;
                    int i = 0;
                    if (_chkVisibilityRect.Contains(e.Location)) {
                        _chkVisibilityHover = true;
                        changed = true;
                    } else {
                        if (_chkVisibilityHover) {
                            _chkVisibilityHover = false;
                            changed = true;
                        }
                    }
                    if (_chkFreezeRect.Contains(e.Location)) {
                        _chkFreezeHover = true;
                        changed = true;
                    } else {
                        if (_chkFreezeHover) {
                            _chkFreezeHover = false;
                            changed = true;
                        }
                    }
                    if (_btnRect.Contains(e.Location)) {
                        _btnHover = true;
                        changed = true;
                    } else {
                        if (_btnHover) {
                            _btnHover = false;
                            changed = true;
                        }
                    }
                    if (_vscVisibility.Visible) {
                        i = 0;
                        while (i < 10) {
                            changed = changed || _itemsVisibility[i + _vscVisibility.Value].mouseMove(e);
                            i = i + 1;
                        }
                    } else {
                        foreach (ItemHost ih in _itemsVisibility) changed = changed | ih.mouseMove(e);
                    }
                    if (_vscFreeze.Visible) {
                        i = 0;
                        while (i < 10) {
                            changed = changed || _itemsFreeze[i + _vscFreeze.Value].mouseMove(e);
                            i++;
                        }
                    } else {
                        foreach (ItemHost ih in _itemsFreeze) changed = changed | ih.mouseMove(e);
                    }
                    if (changed) this.Invalidate();
                }
                private void OptionControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
                    if (_hoverHost == null) return;
                    if (_hoverHost.DisplayFor == 0) {
                        if (_vscVisibility.Visible) {
                            if (e.Delta < 0) {
                                if (_vscVisibility.Value > 0) _vscVisibility.Value -= 1;
                            } else {
                                if (_vscVisibility.Value < _vscVisibility.Maximum) _vscVisibility.Value += 1;
                            }
                        }
                    } else {
                        if (_vscFreeze.Visible) {
                            if (e.Delta < 0) {
                                if (_vscFreeze.Value > 0) _vscFreeze.Value -= 1;
                            } else {
                                if (_vscFreeze.Value < _vscFreeze.Maximum) _vscFreeze.Value += 1;
                            }
                        }
                    }
                }
                private void OptionControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
                    StringFormat btnFormat = new StringFormat();
                    int i = 0;
                    btnFormat.Alignment = StringAlignment.Center;
                    btnFormat.LineAlignment = StringAlignment.Center;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    // Check area
                    e.Graphics.Clear(Ai.Renderer.Popup.BackgroundBrush().Color);
                    e.Graphics.FillRectangle(Ai.Renderer.Popup.PlacementBrush(), new Rectangle(0, 0, 22, this.Height));
                    e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 22, 0, 22, this.Height);
                    if (_itemsVisibility.Count > 0) {
                        e.Graphics.FillRectangle(Ai.Renderer.Popup.SeparatorBrush(), new Rectangle(0, _chkVisibilityRect.Y, this.Width, _chkVisibilityRect.Height));
                        e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 0, _chkVisibilityRect.Bottom, this.Width, _chkVisibilityRect.Bottom);
                        Ai.Renderer.CheckBox.drawCheckBox(e.Graphics, _chkVisibilityRect, _chkVisibilityState, 14, true, _chkVisibilityHover);
                        e.Graphics.DrawString("Visible", this.Font, Ai.Renderer.Drawing.NormalTextBrush(), _txtVisibilityRect, txtFormat);
                        if (_vscVisibility.Visible) {
                            while (i < 10) {
                                _itemsVisibility[i + _vscVisibility.Value].draw(e.Graphics);
                                i++;
                            }
                        } else {
                            foreach (ItemHost ih in _itemsVisibility) ih.draw(e.Graphics);
                        }
                    }
                    if (_itemsFreeze.Count > 0) {
                        e.Graphics.FillRectangle(Ai.Renderer.Popup.SeparatorBrush(), new Rectangle(0, _chkFreezeRect.Y, this.Width, _chkFreezeRect.Height));
                        e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 0, _chkFreezeRect.Bottom, this.Width, _chkFreezeRect.Bottom);
                        Ai.Renderer.CheckBox.drawCheckBox(e.Graphics, _chkFreezeRect, _chkFreezeState, 14, true, _chkFreezeHover);
                        e.Graphics.DrawString("Freeze", this.Font, Ai.Renderer.Drawing.NormalTextBrush(), _txtFreezeRect, txtFormat);
                        if (_vscFreeze.Visible) {
                            i = 0;
                            while (i < 10) {
                                _itemsFreeze[i + _vscFreeze.Value].draw(e.Graphics);
                                i++;
                            }
                        } else {
                            foreach (ItemHost ih in _itemsFreeze) ih.draw(e.Graphics);
                        }
                    }
                    e.Graphics.FillRectangle(Ai.Renderer.Popup.BackgroundBrush(), new Rectangle(0, _btnRect.Y - 2, this.Width, this.Height - (_btnRect.Y - 2)));
                    e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 0, _btnRect.Y - 2, this.Width, _btnRect.Y - 2);
                    Ai.Renderer.Button.draw(e.Graphics, _btnRect, Ai.Renderer.Drawing.ColorTheme.Blue, 3, true, false, false, _btnHover);
                    e.Graphics.DrawString("OK", this.Font, Renderer.Drawing.NormalTextBrush(), _btnRect, btnFormat);
                    btnFormat.Dispose();
                }
            }
            #endregion
            #region Declarations.
            DataGrid _owner = null;
            FloatingWindow _wndSign = null;
            // Checkbox
            Rectangle _chkRect;
            bool _chkHover = false;
            System.Windows.Forms.CheckState _chkState = CheckState.Unchecked;
            // Tooltip
            Rectangle _currentToolTipRect = new Rectangle();
            string _currentToolTip = "";
            string _currentToolTipTitle = "";
            Image _currentToolTipImage = null;
            // Areas
            Rectangle _groupArea;
            Rectangle _columnArea;
            // Hosts
            List<ColumnHeaderHost> _headers;
            List<ConnectorHost> _connectors;
            List<SeparatorHost> _separators;
            List<DataGridGroupHost> _groups;
            ObjectHost _hoverHost = null;
            ObjectHost _selectedHost;
            ObjectHost _draggedHost = null;
            OptionHost _optionHost = null;
            // Internal use
            private ToolStripDropDown _toolStrip;
            private ToolTip _toolTip;
            int _frozenRight = 0;
            FilterChooser _shownChooser = null;
            bool _resumePainting = true;
            #endregion
            #region Internal Events
            /// <summary>
            /// Occurs when the column order has been changed.
            /// </summary>
            internal event EventHandler<ColumnEventArgs> ColumnOrderChanged;
            /// <summary>
            /// Occurs when the column filter has been changed.
            /// </summary>
            internal event EventHandler<ColumnEventArgs> AfterColumnFilter;
            /// <summary>
            /// Occurs when the column custom filter is choosen.
            /// </summary>
            internal event EventHandler<ColumnEventArgs> AfterColumnCustomFilter;
            /// <summary>
            /// Occurs when the CheckBox checked status has been changed.
            /// </summary>
            internal event EventHandler<EventArgs> CheckedChanged;
            /// <summary>
            /// Occurs when the ColumnHeader width has been changed.
            /// </summary>
            internal event EventHandler<ColumnEventArgs> AfterColumnResize;
            #endregion
            public ColumnHeaderControl(DataGrid owner, Font font) {
                _owner = owner;
                _headers = new List<ColumnHeaderHost>();
                _connectors = new List<ConnectorHost>();
                _separators = new List<SeparatorHost>();
                _groups = new List<DataGridGroupHost>();
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                this.SetStyle(ControlStyles.Selectable, false);
                this.Dock = DockStyle.Top;
                this.Font = font;
                this.Height = (this.Font.Height + 8) * 2;
                _chkRect = new Rectangle(3, (int)((this.Height - 16) / 2), 14, 14);
                // ToolTip initializing
                _toolTip = new ToolTip(this);
                _toolTip.AnimationSpeed = 20;
                _toolTip.Draw += _toolTip_Draw;
                _toolTip.Popup += _toolTip_Popup;
                // ToolStrip initializing
                _toolStrip = new ToolStripDropDown(this);
                _toolStrip.SizingGrip = ToolStripDropDown.SizingGripMode.BottomRight;
                _toolStrip.Closed += _toolStrip_Closed;
                // Attaching event handler
                Resize += ColumnHeaderControl_Resize;
                Paint += ColumnHeaderControl_Paint;
                MouseUp += ColumnHeaderControl_MouseUp;
                MouseMove += ColumnHeaderControl_MouseMove;
                MouseLeave += ColumnHeaderControl_MouseLeave;
                MouseDown += ColumnHeaderControl_MouseDown;
                EnabledChanged += ColumnHeaderControl_EnabledChanged;
                DoubleClick += ColumnHeaderControl_DoubleClick;
            }
            #region Tooltip Events
            private void _toolTip_Draw(object sender, DrawEventArgs e) {
                Ai.Renderer.ToolTip.drawToolTip(e.Graphics, new Rectangle((int)e.Rectangle.X, (int)e.Rectangle.Y, (int)e.Rectangle.Width, (int)e.Rectangle.Height), _currentToolTipTitle, _currentToolTip, _currentToolTipImage);
                _currentToolTipTitle = "";
                _currentToolTip = "";
                _currentToolTipImage = null;
            }
            private void _toolTip_Popup(object sender, PopupEventArgs e) {
                e.Size = Ai.Renderer.ToolTip.measureSize(_currentToolTipTitle, _currentToolTip, _currentToolTipImage);
            }
            #endregion
            #region ToolStrip Events
            private void _toolStrip_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e) {
                if (_optionHost != null) _optionHost.Pressed = false;
                _resumePainting = true;
                this.Invalidate();
                if (_shownChooser != null) {
                    if (_shownChooser.Result != FilterChooserResult.Cancel) {
                        if (_shownChooser.Result == FilterChooserResult.OK) {
                            if (AfterColumnFilter != null) AfterColumnFilter(this, new ColumnEventArgs(_shownChooser.FilterHandle.Column));
                        } else {
                            if (AfterColumnCustomFilter != null) AfterColumnCustomFilter(this, new ColumnEventArgs(_shownChooser.FilterHandle.Column));
                        }
                    }
                    _shownChooser.Dispose();
                }
            }
            #endregion
            #region ColumnHeaderControl's Event Handlers
            private void ColumnHeaderControl_DoubleClick(object sender, System.EventArgs e) {
                _toolTip.hide();
                if (_hoverHost != null) _hoverHost.mouseDoubleClick();
                Point mp = System.Windows.Forms.Control.MousePosition;
                mp = this.PointToClient(mp);
                MouseEventArgs me = new MouseEventArgs(MouseButtons.None, 0, mp.X, mp.Y, 0);
                int i = 0;
                bool changed = false;
                while (i < _groups.Count) {
                    changed = changed || _groups[i].mouseMove(me);
                    i++;
                }
                i = 0;
                while (i < _connectors.Count) {
                    changed = changed || _connectors[i].mouseMove(me);
                    i++;
                }
                i = 0;
                while (i < _headers.Count) {
                    changed = changed || _headers[i].mouseMove(me);
                    i++;
                }
                i = 0;
                while (i < _separators.Count) {
                    changed = changed || _separators[i].mouseMove(me);
                    i++;
                }
                if (changed) this.Invalidate();
            }
            private void ColumnHeaderControl_EnabledChanged(object sender, System.EventArgs e) { this.Invalidate(); }
            private void ColumnHeaderControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
                _toolTip.hide();
                if (_optionHost != null) {
                    if (_optionHost.Hover) {
                        _optionHost.mouseDown(e);
                        if (_optionHost.Pressed) {
                            this.Invalidate();
                            _resumePainting = false;
                            // Show column option popup window.
                            OptionControl optCtrl = new OptionControl(this, Ai.Renderer.ToolTip.TextFont);
                            ToolStripControlHost anHost = new ToolStripControlHost(optCtrl);
                            Point scrPoint = this.PointToScreen(new Point(_optionHost.X, _optionHost.Bottom + 1));
                            if (scrPoint.X + optCtrl.Width + 6 > Screen.PrimaryScreen.WorkingArea.Width) scrPoint.X = scrPoint.X - (optCtrl.Width - 4);
                            if (scrPoint.Y + optCtrl.Height + 6 > Screen.PrimaryScreen.WorkingArea.Height) scrPoint.Y = scrPoint.Y - (optCtrl.Height + this.Height + 8);
                            _toolStrip.Items.Clear();
                            _toolStrip.Items.Add(anHost);
                            _toolStrip.Show(scrPoint);
                            return;
                        }
                    }
                }
                if (e.Button == MouseButtons.Left) {
                    if (_chkHover) {
                        if (_chkState == System.Windows.Forms.CheckState.Indeterminate || _chkState == System.Windows.Forms.CheckState.Unchecked) _chkState = System.Windows.Forms.CheckState.Checked;
                        else _chkState = System.Windows.Forms.CheckState.Unchecked;
                        if (_resumePainting) this.Invalidate();
                        if (CheckedChanged != null) CheckedChanged(this, new EventArgs());
                        return;
                    }
                }
                int i = 0;
                bool changed = false;
                while (i < _groups.Count) {
                    changed = changed || _groups[i].mouseDown(e);
                    i++;
                }
                i = 0;
                while (i < _connectors.Count) {
                    changed = changed || _connectors[i].mouseDown(e);
                    i++;
                }
                i = 0;
                while (i < _headers.Count) {
                    changed = changed || _headers[i].mouseDown(e);
                    i++;
                }
                if (changed) this.Invalidate();
            }
            private void ColumnHeaderControl_MouseLeave(object sender, System.EventArgs e) {
                bool changed = false;
                int i = 0;
                while (i < _groups.Count) {
                    changed = changed || _groups[i].mouseLeave();
                    i++;
                }
                i = 0;
                while (i < _connectors.Count) {
                    changed = changed || _connectors[i].mouseLeave();
                    i++;
                }
                i = 0;
                while (i < _headers.Count) {
                    changed = changed || _headers[i].mouseLeave();
                    i++;
                }
                i = 0;
                while (i < _separators.Count) {
                    changed = changed || _separators[i].mouseLeave();
                    i++;
                }
                if (_optionHost != null) changed = changed || _optionHost.mouseLeave();
                if (changed && _resumePainting) this.Invalidate();
            }
            private void ColumnHeaderControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
                bool changed = false;
                int i;
                List<ObjectHost> unfrozenColumns = new List<ObjectHost>();
                List<ObjectHost> frozenColumns = new List<ObjectHost>();
                // Separating objects.
                i = 0;
                while (i < _headers.Count) {
                    DataGridHeader header = (DataGridHeader)_headers[i].Host;
                    if (header.Visible) {
                        if (header.Frozen) frozenColumns.Add(_headers[i]);
                        if (!header.Frozen) unfrozenColumns.Add(_headers[i]);
                    }
                    i++;
                }
                // Testing mouse move event to unfrozen column(s).
                i = 0;
                while (i < unfrozenColumns.Count) {
                    changed = changed || unfrozenColumns[i].mouseMove(e);
                    i++;
                }
                // Testing mouse move event to frozen column(s).
                i = 0;
                while (i < frozenColumns.Count) {
                    changed = changed || frozenColumns[i].mouseMove(e);
                    i++;
                }
                // Testing mouse move event to DataGridGroup(s).
                i = 0;
                while (i < _groups.Count) {
                    changed = changed || _groups[i].mouseMove(e);
                    i++;
                }
                // Testing mouse move event to connector between groups.
                i = 0;
                while (i < _connectors.Count) {
                    changed = changed || _connectors[i].mouseMove(e);
                    i++;
                }
                // Testing to column option if visible.
                if (_owner._showColumnOptions) changed = changed || _optionHost.mouseMove(e);
                if (changed && _resumePainting) this.Invalidate();
                // Drawing drag sign if any.
                if (_draggedHost != null) {
                    if (_draggedHost.DragEffect != DragHelper.DragEffect.None) {
                        if (_wndSign != null) {
                            Point scrLoc = this.PointToScreen(_draggedHost.CurrentDrag);
                            scrLoc.X += 15;
                            scrLoc.Y += 15;
                            _wndSign.Location = scrLoc;
                            if (!_wndSign.Visible) {
                                _wndSign.show();
                                return;
                            }
                            _wndSign.invalidateWindow();
                        }
                    }
                }
            }
            private void ColumnHeaderControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
                bool changed = false;
                int i = 0;
                while (i < _groups.Count) {
                    changed = changed || _groups[i].mouseUp(e);
                    i++;
                }
                i = 0;
                while (i < _connectors.Count) {
                    changed = changed || _connectors[i].mouseUp(e);
                    i++;
                }
                i = 0;
                while (i < _headers.Count) {
                    changed = changed || _headers[i].mouseUp(e);
                    i++;
                }
                if (changed && _resumePainting) this.Invalidate();
            }
            private void ColumnHeaderControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                Rectangle rectCircle = new Rectangle(0, 0, (int)this.Width * 2, (int)this.Height * 2);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                // Fill the background.
                e.Graphics.Clear(Color.FromArgb(175, 201, 235));
                LinearGradientBrush bg = new LinearGradientBrush(rect, Color.FromArgb(158, 190, 230),
                    Color.FromArgb(86, 125, 176), LinearGradientMode.Vertical);
                e.Graphics.FillEllipse(bg, rectCircle);
                bg.Dispose();
                bg = new LinearGradientBrush(_columnArea, Color.White, Color.Black, LinearGradientMode.Vertical);
                bg.InterpolationColors = Renderer.Column.NormalBlend();
                e.Graphics.FillRectangle(bg, _columnArea);
                bg.Dispose();
                // Drawing the objects.
                // Draws the groups.
                foreach (DataGridGroupHost gh in _groups) gh.draw(e.Graphics);
                // Draws the connectors of the group.
                foreach (ConnectorHost ch in _connectors) ch.draw(e.Graphics);
                // Draws the visible and unfrozen column first.
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader header = (DataGridHeader)ch.Host;
                    if (header.Visible && !header.Frozen) ch.draw(e.Graphics);
                }
                // Draws the visible and frozen column.
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader header = (DataGridHeader)ch.Host;
                    if (header.Visible && header.Frozen) ch.draw(e.Graphics);
                }
                // Draws the column option if needed.
                if (_owner._showColumnOptions) {
                    if (_optionHost != null) _optionHost.draw(e.Graphics);
                }
                // Draws the dragged object if any.
                if (_draggedHost != null) _draggedHost.drawDrag(e.Graphics);
                // Columns border.
                e.Graphics.DrawRectangle(Renderer.Column.NormalBorderPen(), _columnArea);
            }
            private void ColumnHeaderControl_Resize(object sender, System.EventArgs e) {
                _columnArea.X = 0;
                _columnArea.Height = _owner._headerFont.Height + 4;
                _columnArea.Y = _owner.Height - _columnArea.Height;
                _columnArea.Width = this.Width;
                _optionHost.X = this.Width - (_optionHost.Width + 2);
                arrangeHeaders();
            }
            #endregion
            #region Private Functions
            /// <summary>
            /// Show filter options window in a popup window.
            /// </summary>
            /// <param name="chooser">A FilterChooser object to be shown.</param>
            /// <param name="dropdownRect">A Rectangle where the drop down sign is.</param>
            private void showFilterPopup(FilterChooser chooser, Rectangle dropdownRect) {
                ToolStripControlHost anHost = null;
                Point scrLoc = default(Point);
                chooser.Width = 200;
                anHost = new ToolStripControlHost(chooser);
                anHost.Padding = new Padding(0);
                _toolStrip.Items.Clear();
                _toolStrip.Items.Add(anHost);
                scrLoc = this.PointToScreen(new Point(dropdownRect.Right - 200, this.Height + 2));
                if (scrLoc.X < 0) scrLoc = this.PointToScreen(new Point(dropdownRect.X, this.Height + 2));
                if (scrLoc.Y + chooser.Height + 5 > Screen.PrimaryScreen.WorkingArea.Height) scrLoc.Y = scrLoc.Y - (chooser.Height + 5 + this.Height);
                _resumePainting = false;
                // Painting the column control before the dropdown window is shown.
                PaintEventArgs pe = null;
                pe = new PaintEventArgs(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
                this.InvokePaint(this, pe);
                pe.Dispose();
                _shownChooser = chooser;
                _toolStrip.Show(scrLoc);
            }
            /// <summary>
            /// Swaps the prosition of two DataGridGroup in the Groups collection of the DataGrid.
            /// </summary>
            /// <param name="host1">First host of the DataGridGroup object.</param>
            /// <param name="host2">Second host of the DataGridGroup object.</param>
            private void swapGroupHosts(DataGridGroupHost host1, DataGridGroupHost host2) {
                if (host1 == null || host2 == null) return;
                if (host1 == host2) return;
                if (_groups.IndexOf(host1) == -1 || _groups.IndexOf(host2) == -1) return;
                ConnectorHost c1, c2;
                c1 = _connectors[_groups.IndexOf(host1)];
                c2 = _connectors[_groups.IndexOf(host2)];
                DataGridGroupHost tmpHost = host1;
                host1 = host2;
                host2 = tmpHost;
                int tmpX = host1.X, tmpY = host1.Y;
                host1.X = host2.X;
                host1.Y = host2.Y;
                host2.X = tmpX;
                host2.Y = tmpY;
                c1.RightHost = host2;
                c2.RightHost = host1;
                // Rearrange the position of both groups in the collection of the DataGrid.
            }
            /// <summary>
            /// Moves a DataGridGroupHost to a specified index in its collection.
            /// </summary>
            /// <param name="index">Index of the collection where the host will be moved.</param>
            /// <param name="host">A DataGridGroupHost to be moved.</param>
            private void moveGroupHost(int index, DataGridGroupHost host) {
                if (host == null) return;
                if (index == -1) return;
                if (_groups.IndexOf(host) == -1) return;
                if (_groups.IndexOf(host) == index) return;
                int gIdx = _groups.IndexOf(host);
                int i = 0;
                if (gIdx < index) {
                    i = gIdx;
                    while (i < index) {
                        _groups[i] = _groups[i + 1];
                        i++;
                    }
                } else {
                    i = gIdx;
                    while (i > index) {
                        _groups[i] = _groups[i - 1];
                        i--;
                    }
                }
                _groups[i] = host;
                i = 0;
                while (i < _connectors.Count) {
                    if (i > 0) _connectors[i].LeftHost = _groups[i - 1];
                    _connectors[i].RightHost = _groups[i];
                    i++;
                }
                // Rearrange the position of the groups here both in visual position and in the collection of the DataGrid.
            }
            /// <summary>
            /// Swaps the position of two ColumnHeaderHosts.
            /// </summary>
            /// <param name="host1">First ColumnHeaderHost to be swaps.</param>
            /// <param name="host2">Second ColumnHeaderHost to be swaps.</param>
            private void swapHeaderHosts(ColumnHeaderHost host1, ColumnHeaderHost host2) {
                if (host1 == null || host2 == null) return;
                if (host1 == host2) return;
                if (_headers.IndexOf(host1) == -1 || _headers.IndexOf(host2) == -1) return;
                SeparatorHost s1, s2;
                s1 = _separators[_headers.IndexOf(host1)];
                s2 = _separators[_headers.IndexOf(host2)];
                ColumnHeaderHost tmpHost = host1;
                host1 = host2;
                host2 = tmpHost;
                host1.RightHost = s1;
                host2.RightHost = s2;
                // Arrange the headers.
            }
            /// <summary>
            /// Moves a ColumnHeaderHost to a specified connector between header.
            /// </summary>
            /// <param name="sHost">A ConnectorHost object where the ColumnHeaderHost is dropped.</param>
            /// <param name="host">A ColumnHeaderHost that will be moved.</param>
            private void moveHeaderHost(ColumnHeaderHost host, SeparatorHost sHost) {
                if (sHost == null || host == null) return;
                ColumnHeaderHost tHost = (ColumnHeaderHost)sHost.LeftHost;
                if (tHost == host || tHost == null) return;
                int sourceIdx = _headers.IndexOf(host);
                int targetIdx = _headers.IndexOf(tHost);
                int i = sourceIdx;
                if (sourceIdx < targetIdx) {
                    while (i < targetIdx) {
                        _headers[i] = _headers[i + 1];
                        i++;
                    }
                } else {
                    while (i > targetIdx) {
                        _headers[i] = _headers[i - 1];
                        i--;
                    }
                }
                _headers[i] = host;
                i = 0;
                while (i < _connectors.Count) {
                    _headers[i].RightHost = _separators[i];
                    _separators[i].LeftHost = _headers[i];
                    i++;
                }
                // Rearrange the visual position of the headers and separators here.
            }
            /// <summary>
            /// Moves a ColumnHeaderHost object to the last position of the headers arrangement.
            /// </summary>
            /// <param name="host">A ColumnHederHost object to be moved.</param>
            private void moveHeaderHost(ColumnHeaderHost host) {
                if (host == null) return;
                int i = _headers.IndexOf(host);
                if (i == _headers.Count - 1) return;
                while (i < _headers.Count - 1) {
                    _headers[i] = _headers[i + 1];
                    i++;
                }
                _headers[i] = host;
                i = 0;
                while (i < _separators.Count) {
                    _headers[i].RightHost = _separators[i];
                    _separators[i].LeftHost = _headers[i];
                    i++;
                }
                // Rearrange the visual position of the column headers and its separator here.
            }
            /// <summary>
            /// Arrange the host's location of the DataGridGroup and its connector.
            /// </summary>
            private void arrangeGroups() {
                int x = 2, y = 2;
                int i = 0;
                while (i < _connectors.Count) {
                    _connectors[i].X = x;
                    _connectors[i].Y = y;
                    x += GROUP_SIZE;
                    _groups[i].X = x;
                    _groups[i].Y = y;
                    y += GROUP_SIZE;
                    i++;
                }
                this.Height = y + _columnArea.Height + 2;
            }
            private void arrangeHeaders() { arrangeHeaders(0); }
            private void arrangeHeaders(int x) {
                int availWidth = this.Width - 5;
                int spaceLeft = 1;
                int frozenWidth = 0;
                int i, j;
                if (_owner._rowNumbers) {
                    int numbersWidth = (int)(_owner._gObj.MeasureString(_owner._items.Count.ToString(), _owner.Font).Width + 2);
                    spaceLeft += numbersWidth;
                    availWidth -= numbersWidth;
                }
                if (_owner._checkBoxes) {
                    spaceLeft += 22;
                    availWidth -= 22;
                }
                availWidth -= _owner._vscroll.Width;
                // No need to create if no space available.
                if (availWidth > 0) {
                    // Create rectangle for frozen columns first.
                    i = 0;
                    while (i < _headers.Count) {
                        DataGridHeader header = (DataGridHeader)_headers[i].Host;
                        if (header.Frozen && header.Visible) {
                            switch (header.SizeType) {
                                case ColumnSizeType.Fixed:
                                    _headers[i].Width = header.Width;
                                    break;
                                case ColumnSizeType.Auto:
                                    //_hosts[i].Width = _owner.getSubItemMaxWidth(header);
                                    break;
                                case ColumnSizeType.Percentage:
                                    _headers[i].Width = (int)(header.Width * availWidth / 100);
                                    break;
                            }
                            _headers[i].Height = _owner._headerFont.Height + 4;
                            frozenWidth += _headers[i].Width + SEPARATOR_SIZE + 1;
                        }
                        i++;
                    }
                    availWidth -= frozenWidth;
                    // Create reactangle for the rest of the visible columns.
                    int fillSizeCount = 0;
                    i = 0;
                    while (i < _headers.Count) {
                        DataGridHeader header = (DataGridHeader)_headers[i].Host;
                        if (header.Visible && !header.Frozen) {
                            switch (header.SizeType) {
                                case ColumnSizeType.Fixed:
                                    _headers[i].Width = header.Width;
                                    break;
                                case ColumnSizeType.Auto:
                                    //_hosts[i].Width = _owner.getSubItemMaxWidth(header);
                                    break;
                                case ColumnSizeType.Fill:
                                    fillSizeCount = fillSizeCount + 1;
                                    break;
                                case ColumnSizeType.Percentage:
                                    _headers[i].Width = (int)(header.Width * availWidth / 100);
                                    break;
                            }
                            _headers[i].Height = _owner._headerFont.Height + 4;
                            if (header.SizeType != ColumnSizeType.Fill) availWidth -= (_headers[i].Width + SEPARATOR_SIZE + 1);
                        }
                        i++;
                    }
                    if (fillSizeCount > 0) {
                        i = 0;
                        while (i < _headers.Count) {
                            DataGridHeader header = (DataGridHeader)_headers[i].Host;
                            if (header.Visible && header.SizeType == ColumnSizeType.Fill)
                                _headers[i].Width = (int)((availWidth - (SEPARATOR_SIZE * fillSizeCount)) / fillSizeCount);
                            i++;
                        }
                    }
                }
                _optionHost.X = this.Right - 11;
                _optionHost.Height = _owner._headerFont.Height + 4;
                _optionHost.Width = 10;
                moveHeaders(x);
            }
            /// <summary>
            /// Moves the host's x location of the DataGridHeader and its separator.
            /// </summary>
            /// <param name="x">The left most location where the arrangement starts.</param>
            private void moveHeaders(int x) {
                int startX = 1;
                int i;
                if (_owner._checkBoxes) startX += 22;
                if (_owner._rowNumbers) {
                    int numbersWidth = (int)(_owner._gObj.MeasureString(_owner._items.Count.ToString(), _owner.Font).Width + 2);
                    startX += numbersWidth;
                }
                // Moves frozen columns
                i = 0;
                while (i < _headers.Count) {
                    DataGridHeader header = (DataGridHeader)_headers[i].Host;
                    if (header.Frozen && header.Visible) {
                        _headers[i].X = startX;
                        SeparatorHost spr = (SeparatorHost)_headers[i].RightHost;
                        spr.X = _headers[i].Right;
                        startX = spr.Right;
                    }
                    i++;
                }
                _frozenRight = startX;
                // Moves the rest of the columns
                startX = _frozenRight + x;
                i = 0;
                while (i < _headers.Count) {
                    DataGridHeader header = (DataGridHeader)_headers[i].Host;
                    if (!header.Frozen && header.Visible) {
                        _headers[i].X = startX;
                        SeparatorHost spr = (SeparatorHost)_headers[i].RightHost;
                        spr.X = _headers[i].Right;
                        startX = spr.Right;
                    }
                    i++;
                }
            }
            #endregion
            #region Public Functions
            // Index related functions.
            /// <summary>
            /// Returns the index of a DataGridHeader object contained within host in collection.
            /// </summary>
            /// <param name="header">DataGridHeader object to be sought.</param>
            /// <returns>Returns a zero based index represent the index of the header in collections, if found, otherwise, returns -1.</returns>
            public int getHeaderIndex(DataGridHeader header) {
                if (header == null) return -1;
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) return _headers.IndexOf(ch);
                }
                return -1;
            }
            /// <summary>
            /// Gets the displayed index of a specified column.
            /// </summary>
            /// <param name="header">A DataGridHeader object to be sought.</param>
            /// <returns>Return a zero based index indicating the displayed index of the column if found, otherwise, -1.</returns>
            /// <remarks>The frozen column will always shown before the unfrozen column despite the column arrangements.</remarks>
            public int getDisplayedIndex(DataGridHeader header) {
                if (header == null) return -1;
                int idx = -1;
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader dh = (DataGridHeader)ch.Host;
                    if (dh.Visible && dh.Frozen) idx++;
                    if (header == dh) return idx;
                }
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader dh = (DataGridHeader)ch.Host;
                    if (dh.Visible && !dh.Frozen) idx++;
                    if (header == dh) return idx;
                }
                return -1;
            }
            /// <summary>
            /// Returns the index of a DataGridGroup object conatined within host in collection.
            /// </summary>
            /// <param name="group">A DataGridGrop object to be sought.</param>
            /// <returns>Returns a zero based value represent the index of the header in collections, if found, otherwise, returns -1.</returns>
            public int getGroupIndex(DataGridGroup group) {
                if (group == null) return -1;
                foreach (DataGridGroupHost dh in _groups) {
                    if (dh.Host == group) return _groups.IndexOf(dh);
                }
                return -1;
            }
            // Column filter related functions.
            /// <summary>
            /// Clear all filter parameters on all columns.
            /// </summary>
            public void clearFilters() {
                int i = 0;
                while (i < _headers.Count) {
                    _headers[i].FilterHandle.Items.Clear();
                    i++;
                }
            }
            /// <summary>
            /// Adds a filter parameter holds by a host specified by a DataGridHeader.
            /// </summary>
            /// <param name="header">A DataGridHeader object that holds by the host.</param>
            /// <param name="item">A filter to be added.</param>
            public void addFilter(DataGridHeader header, object item) {
                if (header == null || item == null) return;
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) {
                        if (ch.FilterHandle != null) ch.FilterHandle.addFilter(item);
                        break;
                    }
                }
            }
            /// <summary>
            /// Adds a filter parameter holds by a host specified by the index of DataGridHeader in DataGridHeaderCollection.
            /// </summary>
            /// <param name="index">Index of the DataGridHeader in DataGridHeaderCollection.</param>
            /// <param name="item">A filter to be added.</param>
            public void addFilter(int index, object item) {
                DataGridHeader header = _owner._headers[index];
                addFilter(header, item);
            }
            /// <summary>
            /// Reload filter's parameters holds by a host using specified DataGridHeader.
            /// </summary>
            /// <param name="header">A DataGridHeader object that holds by the host.</param>
            /// <param name="filters">List of parameters to fill the filter.</param>
            public void reloadFilter(DataGridHeader header, List<object> filters) {
                if (header == null || filters == null) return;
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) {
                        if (ch.FilterHandle != null) ch.FilterHandle.reloadFilter(filters);
                        break;
                    }
                }
            }
            /// <summary>
            /// Reload filter's parameters holds by a host using specified index of the DataGridHeader.
            /// </summary>
            /// <param name="index">Index of a DataGridHeader object in DataGridHeaderCollection.</param>
            /// <param name="filters">List of parameter to fill the filter.</param>
            public void reloadFilter(int index, List<object> filters) {
                DataGridHeader header = _owner._headers[index];
                reloadFilter(header, filters);
            }
            // Host related.
            /// <summary>
            /// Adds a DataGridHeader object to be hosted in header control at end of the collection.
            /// </summary>
            /// <param name="header">A DataGridHeader object to be hosted.</param>
            /// <returns>If succeeded, returns the displayed index of the DataGridHeader object, otherwise, -1 will be returned.</returns>
            public int addHeader(DataGridHeader header) {
                if (header == null) return -1;
                ColumnHeaderHost ch = new ColumnHeaderHost(this, header);
                SeparatorHost sh = new SeparatorHost(this);
                ch.RightHost = sh;
                sh.LeftHost = ch;
                _headers.Add(ch);
                _separators.Add(sh);
                arrangeHeaders();
                return getDisplayedIndex(header);
            }
            /// <summary>
            /// Inserts a DataGridHeader object to be hosted in header control at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which DataGridHeader should be inserted.</param>
            /// <param name="header">A DataGridHeader object to be hosted.</param>
            /// <returns>If succeeded, returns the displayed index of the DataGridHeader object, otherwise, -1 will be returned.</returns>
            public int addHeader(int index, DataGridHeader header) {
                if (header == null || index == -1 || index > _headers.Count) return -1;
                ColumnHeaderHost ch = new ColumnHeaderHost(this, header);
                SeparatorHost sh = new SeparatorHost(this);
                ch.RightHost = sh;
                sh.LeftHost = ch;
                _headers.Insert(index, ch);
                _separators.Add(sh);
                arrangeHeaders();
                return getDisplayedIndex(header);
            }
            /// <summary>
            /// Adds a DataGridGroup object to be hosted in header control at end of the collection.
            /// </summary>
            /// <param name="group">A DataGridGroup object to be hosted.</param>
            /// <returns>If succeeded, returns the displayed index of the DataGridGroup object, otherwise, -1 will be returned.</returns>
            public int addGroup(DataGridGroup group) {
                if (group == null) return -1;
                DataGridGroupHost newHost = new DataGridGroupHost(this, group);
                DataGridGroupHost lastHost = null;
                if (_groups.Count > 0) lastHost = _groups[_groups.Count - 1];
                ConnectorHost ch = new ConnectorHost(this);
                ch.LeftHost = lastHost;
                ch.RightHost = newHost;
                newHost.LeftHost = ch;
                _groups.Add(newHost);
                _connectors.Add(ch);
                arrangeGroups();
                return _groups.IndexOf(newHost);
            }
            /// <summary>
            /// Inserts a DataGridGroup object to be hosted in header control at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which DataGridGroup should be inserted.</param>
            /// <param name="group">A DataGridHeader object to be hosted.</param>
            /// <returns>If succeeded, returns the displayed index of the DataGridGroup object, otherwise, -1 will be returned.</returns>
            public int addGroup(int index, DataGridGroup group) {
                if (group == null || index < 0 || index > _groups.Count) return -1;
                DataGridGroupHost prevHost = null;
                ConnectorHost nextConn = null;
                DataGridGroupHost newHost = new DataGridGroupHost(this, group);
                ConnectorHost newConn = new ConnectorHost(this);
                newConn.RightHost = newHost;
                newHost.LeftHost = newConn;
                if (index > 0) prevHost = _groups[index - 1];
                if (index < _groups.Count - 1) nextConn = _connectors[index];
                newConn.LeftHost = prevHost;
                if (nextConn != null) nextConn.LeftHost = newHost;
                _groups.Insert(index, newHost);
                _connectors.Insert(index, newConn);
                arrangeGroups();
                return _groups.IndexOf(newHost);
            }
            /// <summary>
            /// Determines whether a DataGridHeader object is already hosted in the ColumnHeaderControl.
            /// </summary>
            /// <param name="header">A DataGridHeader object to locate in the ColumnHeaderControl.</param>
            /// <returns>True if header is found, otherwise, false.</returns>
            public bool containsHeader(DataGridHeader header) {
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) return true;
                }
                return false;
            }
            /// <summary>
            /// Determines whether a DataGridGroup object is already hosted in the ColumnHeaderControl.
            /// </summary>
            /// <param name="group">A DataGridGroup object to locate in the ColumnHeaderControl.</param>
            /// <returns>True if header is found, otherwise, false.</returns>
            public bool containsGroup(DataGridGroup group) {
                foreach (DataGridGroupHost dh in _groups) {
                    if (dh.Host == group) return true;
                }
                return false;
            }
            /// <summary>
            /// Removes a host that contains a specified DataGridGroup object from ColumnHeaderControl.
            /// </summary>
            /// <param name="group">A DataGridGroup object that contained within host.</param>
            /// <returns>True if succeeded, otherwise, false.</returns>
            public bool removeGroup(DataGridGroup group) {
                if (containsGroup(group)) {
                    DataGridGroupHost groupHost = null, prevHost = null, nextHost = null;
                    ConnectorHost groupConn = null, nextConn = null;
                    foreach (DataGridGroupHost dh in _groups) {
                        if (dh.Host == group) {
                            int idx = _groups.IndexOf(dh);
                            groupHost = dh;
                            if (idx > 0) prevHost = _groups[idx - 1];
                            if (idx < _groups.Count - 1) nextHost = _groups[idx + 1];
                            break;
                        }
                    }
                    if (groupHost == null) return false;
                    groupConn = _connectors[_groups.IndexOf(groupHost)];
                    if (nextHost != null) nextConn = _connectors[_groups.IndexOf(nextHost)];
                    if (_hoverHost == groupHost) _hoverHost = null;
                    _groups.Remove(groupHost);
                    _connectors.Remove(groupConn);
                    if (nextConn != null) nextConn.LeftHost = prevHost;
                    arrangeGroups();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Removes a host that contains a specified DataGridGroup object from ColumnHeaderControl, also, 
            /// removes the DataGridGroup that corresponds with header.
            /// </summary>
            /// <param name="header">A DataGridHeader object that contained within host.</param>
            /// <returns>True if succeeded, otherwise, false.</returns>
            public bool removeHeader(DataGridHeader header) {
                if (containsHeader(header)) {
                    ColumnHeaderHost hHost = null;
                    SeparatorHost sHost = null;
                    // Removes the DataGridGroup that correspond with the header.
                    DataGridGroup rGroup = null;
                    foreach (DataGridGroupHost gh in _groups) {
                        DataGridGroup dg = (DataGridGroup)gh.Host;
                        if (dg.Header == header) {
                            rGroup = dg;
                            break;
                        }
                    }
                    if (rGroup != null) removeGroup(rGroup);
                    if (_owner._groups.Contains(rGroup)) _owner._groups.Remove(rGroup);
                    // Removes the ColumnHeaderHost and its separator.
                    foreach (ColumnHeaderHost ch in _headers) {
                        if (ch.Host == header) {
                            hHost = ch;
                            break;
                        }
                    }
                    if (hHost == null) return false;
                    sHost = (SeparatorHost)hHost.RightHost;
                    _headers.Remove(hHost);
                    _separators.Remove(sHost);
                    arrangeHeaders();
                    return true;
                }
                return false;
            }
            // Hosted object related.
            /// <summary>
            /// Gets a ColumnHeader specified by its index, and the Frozen and Visible property is ignored.
            /// </summary>
            /// <param name="index">The zero based index of the column.</param>
            /// <returns>A DataGridHeader object if found, otherwise, null value will be returned.</returns>
            public DataGridHeader columnAt(int index) {
                if (index < 0 || index >= _headers.Count) return null;
                return (DataGridHeader)_headers[index].Host;
            }
            /// <summary>
            /// Gets a ColumnHeader specified by its index, and the Frozen and Visible property is calculated.
            /// </summary>
            /// <param name="index">The zero based index of the dispayed index of the column.</param>
            /// <returns>A DataGridHeader object if found, otherwise, null value will be returned.</returns>
            /// <remarks>This function only return the visible column, however, the frozen column(s) is shown before the unfrozen column(s), 
            /// and doesn't matter with the column arrangement in the headers collection of the DataGrid.</remarks>
            public DataGridHeader displayedColumnAt(int index) {
                if (index < 0 || index >= _headers.Count) return null;
                int idx = -1;
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader dh = (DataGridHeader)ch.Host;
                    if (dh.Visible && dh.Frozen) idx++;
                    if (idx == index) return dh;
                }
                foreach (ColumnHeaderHost ch in _headers) {
                    DataGridHeader dh = (DataGridHeader)ch.Host;
                    if (dh.Visible && !dh.Frozen) idx++;
                    if (idx == index) return dh;
                }
                return null;
            }
            /// <summary>
            /// Gets the bounding rectangle of a column header.
            /// </summary>
            /// <param name="header">A DataGridHeader object to be shought its boundary.</param>
            /// <returns>A rectangle object represent the bounding rectangle of the column displayed on the ColumnHeaderControl if found, 
            /// otherwise, the default value of rectangle will be returned.</returns>
            public Rectangle columnRectangle(DataGridHeader header) {
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) return ch.Bounds;
                }
                return new Rectangle();
            }
            /// <summary>
            /// Gets a ColumnFilterHandle object specified by its DataGridHeader.
            /// </summary>
            /// <param name="header">A DataGridHeader object that corresponds with the filter.</param>
            /// <returns>A ColumnFilterHandle object if the DataGridHeader is found within the ColumnHeaderControl, otherwise, null value will be returned.</returns>
            public ColumnFilterHandle filterHandler(DataGridHeader header) {
                foreach (ColumnHeaderHost ch in _headers) {
                    if (ch.Host == header) return ch.FilterHandle;
                }
                return null;
            }
            #endregion
            #region Properties
            /// <summary>
            /// Gets or sets the check state of the checkbox displayed in the ColumnHeaderControl.
            /// </summary>
            public System.Windows.Forms.CheckState CheckState {
                get { return _chkState; }
                set {
                    if (_chkState != value) {
                        _chkState = value;
                        if (_owner._checkBoxes) this.Invalidate();
                    }
                }
            }
            /// <summary>
            /// Gets a value indicating column resize operation is performed.
            /// </summary>
            public bool ColumnResizing { get { return false; } }
            /// <summary>
            /// Gets current column resize position.
            /// </summary>
            public int ResizingX { get { return 0; } }
            /// <summary>
            /// Gets total width of the visible columns.
            /// </summary>
            public int ColumnsWidth {
                get {
                    int result = 0;
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible) result += ch.Width + SEPARATOR_SIZE + 1;
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets total width of all visible and unfrozen column(s).
            /// </summary>
            public int UnfrozenWidth {
                get {
                    int result = 0;
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && !dh.Frozen) result += ch.Width + SEPARATOR_SIZE + 1;
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets total width of all visible and frozen column(s).
            /// </summary>
            public int FrozenWidth {
                get {
                    int result = 0;
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && !dh.Frozen) result += ch.Width + SEPARATOR_SIZE + 1;
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets all frozen and visible columns in the collection, sorted by its display index.
            /// </summary>
            public List<DataGridHeader> FrozenColumns {
                get {
                    List<DataGridHeader> result = new List<DataGridHeader>();
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && dh.Frozen) result.Add(dh);
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets all unfrozen and visible columns in the collection, sorted by its display index.
            /// </summary>
            public List<DataGridHeader> UnfrozenColumns {
                get {
                    List<DataGridHeader> result = new List<DataGridHeader>();
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && !dh.Frozen) result.Add(dh);
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets all the bounding rectangle of the frozen and visible column(s) in the collection, sorted by its display index.
            /// </summary>
            public List<Rectangle> FrozenRectangles {
                get {
                    List<Rectangle> result = new List<Rectangle>();
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && dh.Frozen) {
                            Rectangle r = ch.Bounds;
                            r.Width += SEPARATOR_SIZE + 1;
                            result.Add(r);
                        }
                    }
                    return result;
                }
            }
            /// <summary>
            /// Gets all the bounding rectangle of the unfrozen and visible column(s) in the collection, sorted by its display index.
            /// </summary>
            public List<Rectangle> UnfrozenRectangles {
                get {
                    List<Rectangle> result = new List<Rectangle>();
                    foreach (ColumnHeaderHost ch in _headers) {
                        DataGridHeader dh = (DataGridHeader)ch.Host;
                        if (dh.Visible && !dh.Frozen) {
                            Rectangle r = ch.Bounds;
                            r.Width += SEPARATOR_SIZE + 1;
                            result.Add(r);
                        }
                    }
                    return result;
                }
            }
            #endregion
        }
        #endregion
        #region Members
        // Components
        internal DataGridHeaderCollection _headers;
        DataGridGroupCollection _groups;
        DataGridItemCollection _items;
        CheckedDataGridItemCollection _checkedItems;
        VScrollBar _vscroll;
        HScrollBar _hscroll;
        ColumnHeaderControl _header;        
        // Properties
        bool _checkBoxes = false;
        bool _rowNumbers = false;
        bool _showColumnOptions = true;
        bool _allowMultiline = false;
        bool _showGroupOnHeader = false;
        bool _cellToolTip = false;
        CultureInfo _ci = Renderer.Drawing.en_us_ci;
        Font _headerFont = new Font("Segoe UI", 8, FontStyle.Regular);
        // Internal use
        Graphics _gObj;
        #endregion
    }
}