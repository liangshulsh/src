// Ai Software Control Library.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using Ai.Renderer;

namespace Ai.Control {
    /// <summary>
    /// Control to show a sequence of data graphically.
    /// </summary>
    public class Chart : System.Windows.Forms.Control {
        #region Public Classes
        /// <summary>
        /// Represent a collection of Axis in a Chart
        /// </summary>
        public class AxisCollection : System.Collections.CollectionBase {
            private Chart _owner;
            public AxisCollection(Chart owner) : base() {
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
            /// Gets an Axis object in the collection specified by its index.
            /// </summary>
            public Axis this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (Axis)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a Axis object in the collection.
            /// </summary>
            public int IndexOf(Axis item) { return List.IndexOf(item); }
            /// <summary>
            /// Determine whether an Axis object exist in the collection.
            /// </summary>
            public Boolean Contains(Axis item) { return List.Contains(item); }
            /// <summary>
            /// Add an Axis object to the collection.
            /// </summary>
            public Axis Add(Axis item) {
                // Avoid adding the same item multiple times.
                if (!this.Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    return (Axis)List[index];
                }
                return item;
            }
            /// <summary>
            /// Add a Axis collection to the collection.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(AxisCollection items) {
                foreach (Axis ax in items) this.Add(ax);
            }
            /// <summary>
            /// Insert an Axis object to the collection at specified index.
            /// </summary>
            public void Insert(int index, Axis item) {
                item._owner = _owner;
                List.Insert(index, item);
            }
            /// <summary>
            /// Remove an Axis object from the collection.
            /// </summary>
            public void Remove(Axis item) {
                if (List.Contains(item)) List.Remove(item);
            }
            #region Event Handlers
            /// <summary>
            /// Performs additional custom processes when validating a value.
            /// </summary>
            protected override void OnValidate(object value) {
                if (!typeof(Axis).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.Axis!", "value");
            }
            protected override void OnClear() {
                Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
            }
            protected override void OnClearComplete() {
                AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
            }
            protected override void OnInsert(int index, object value) {
                Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) {
                AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
            }
            protected override void OnRemove(int index, object value) {
                Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
            }
            protected override void OnRemoveComplete(int index, object value) {
                AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
            }
            protected override void OnSet(int index, object oldValue, object newValue) {
                Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            #endregion
        }
        /// <summary>
        /// Represent a collection of ValueAxis in a Chart
        /// </summary>
        public class ValueAxisCollection : System.Collections.CollectionBase {
            Chart _owner;
            public ValueAxisCollection(Chart owner) : base() { _owner = owner; }
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
            /// Gets a ValueAxis object in the collection specified by its index.
            /// </summary>
            public ValueAxis this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (ValueAxis)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a ValueAxis object in the collection.
            /// </summary>
            public int IndexOf(ValueAxis item) { return List.IndexOf(item); }
            /// <summary>
            /// Determine whether a ValueAxis object exist in the collection.
            /// </summary>
            public Boolean Contains(ValueAxis item) { return List.Contains(item); }
            /// <summary>
            /// Add a ValueAxis object to the collection.
            /// </summary>
            public ValueAxis Add(ValueAxis item) {
                if (!Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    return (ValueAxis)List[index];
                }
                return item;
            }
            /// <summary>
            /// Add a ValueAxis collection to the collection.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(ValueAxisCollection items) {
                foreach (ValueAxis vAx in items) this.Add(vAx);
            }
            /// <summary>
            /// Insert a ValueAxis object to the collection at specified index.
            /// </summary>
            public void Insert(int index, ValueAxis item) {
                item._owner = _owner;
                List.Insert(index, item);
            }
            /// <summary>
            /// Remove a ValueAxis object from the collection.
            /// </summary>
            public void Remove(ValueAxis item) {
                if (List.Contains(item)) List.Remove(item);
            }
            #region Event Handlers
            /// <summary>
            /// Performs additional custom processes when validating a value.
            /// </summary>
            protected override void OnValidate(object value) {
                if (!typeof(ValueAxis).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.ValueAxis!", "value");
            }
            protected override void OnClear() {
                Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
            }
            protected override void OnClearComplete() {
                AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
            }
            protected override void OnInsert(int index, object value) {
                Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) {
                AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
            }
            protected override void OnRemove(int index, object value) {
                Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
            }
            protected override void OnRemoveComplete(int index, object value) {
                AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
            }
            protected override void OnSet(int index, object oldValue, object newValue) {
                Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            #endregion
        }
        /// <summary>
        /// Represent a collection of ChartItem in a Chart.
        /// </summary>
        public class ChartItemCollection : System.Collections.CollectionBase {
            Chart _owner;
            public ChartItemCollection(Chart owner) : base() { _owner = owner; }
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
            /// Gets a ChartItem object in the collection specified by its index.
            /// </summary>
            public ChartItem this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (ChartItem)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a ChartItem object in the collection.
            /// </summary>
            public int IndexOf(ChartItem item) { return List.IndexOf(item); }
            /// <summary>
            /// Determine whether a ChartItem object exist in the collection.
            /// </summary>
            public Boolean Contains(ChartItem item) { return List.Contains(item); }
            /// <summary>
            /// Add a ChartItem object to the collection.
            /// </summary>
            public ChartItem Add(ChartItem item) {
                if (!Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    return (ChartItem)List[index];
                }
                return item;
            }
            /// <summary>
            /// Add a ChartItem collection to the collection.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(ChartItemCollection items) {
                foreach (ChartItem ci in items) this.Add(ci);
            }
            /// <summary>
            /// Insert a ChartItem object to the collection at specified index.
            /// </summary>
            public void Insert(int index, ChartItem item) {
                item._owner = _owner;
                List.Insert(index, item);
            }
            /// <summary>
            /// Remove a ChartItem object from the collection.
            /// </summary>
            public void Remove(ChartItem item) {
                if (List.Contains(item)) List.Remove(item);
            }
            #region Event Handlers
            /// <summary>
            /// Performs additional custom processes when validating a value.
            /// </summary>
            protected override void OnValidate(object value) {
                if (!typeof(ChartItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.ChartItem!", "value");
            }
            protected override void OnClear() {
                Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
            }
            protected override void OnClearComplete() {
                AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
            }
            protected override void OnInsert(int index, object value) {
                Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) {
                AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
            }
            protected override void OnRemove(int index, object value) {
                Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
            }
            protected override void OnRemoveComplete(int index, object value) {
                AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
            }
            protected override void OnSet(int index, object oldValue, object newValue) {
                Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
            }
            #endregion
        }
        #endregion
        #region Private Fields
        private const int LINE_LENGTH = 5;
        private const int MAX_BAR_WIDTH = 5;
        StringFormat _hLeftFormat = new StringFormat();             // Horizontal String Format with Left Alignment.
        StringFormat _hRightFormat = new StringFormat();            // Horizontal String Format with Right Alignment.
        StringFormat _hCenterFormat = new StringFormat();           // Horizontal String Format with Center Alignment.
        StringFormat _vStrFormat = new StringFormat();              // Vertical String Format.
        StringFormat _vCenterFormat = new StringFormat();           // Vertical String Format with Center Alignment.
        Font _chartFont = new Font("Segoe UI", 7, FontStyle.Regular);
        ChartItemCollection _items;
        AxisCollection _catAxis;
        ValueAxisCollection _priAxis, _secAxis;
        AxisHost _catHost, _priHost, _secHost;
        ChartHeader _header;
        RectangleF _chartArea;
        ItemHost.ValueHost _hoverValue = null;
        List<ItemHost> _itemHosts;
        Pen _dottedPen = new Pen(System.Drawing.Color.Black);
        ToolTip _tooltip;
        #endregion
        #region Chart Engines, classes to handle visualization of all objects in a Chart.
        /// <summary>
        /// Class to handle visualization of an Axis in a Chart.
        /// </summary>
        private class AxisHost {
            System.Windows.Forms.Orientation _orientation = System.Windows.Forms.Orientation.Horizontal;
            RectangleF _rect = new RectangleF(0, 0, 0, 0);
            string _title = "";
            System.Collections.CollectionBase _axises = null;
            Chart _owner;
            Boolean _drawTitle = false;
            Boolean _drawGrid = false;
            public AxisHost(Chart owner, System.Collections.CollectionBase axises) {
                _owner = owner;
                _axises = axises;
            }
            /// <summary>
            /// Gets or sets the location of an AxisHost.
            /// </summary>
            public PointF Location {
                get { return _rect.Location; }
                set { _rect.Location = value; }
            }
            /// <summary>
            /// Gets or sets the size of an AxisHost.
            /// </summary>
            public SizeF Size {
                get { return _rect.Size; }
                set { _rect.Size = value; }
            }
            /// <summary>
            /// Gets or sets the title of a collection of Axis in a Chart.
            /// </summary>
            public string Title {
                get { return _title; }
                set { _title = value; }
            }
            /// <summary>
            /// Gets or sets the orientation of an AxisHost.
            /// </summary>
            public System.Windows.Forms.Orientation Orientation {
                get { return _orientation; }
                set { _orientation = value; }
            }
            /// <summary>
            /// Gets or sets a value indicating the axis title should be drawn.
            /// </summary>
            public Boolean DrawTitle {
                get { return _drawTitle; }
                set { _drawTitle = value; }
            }
            /// <summary>
            /// Gets or sets a value indicating the axis gridlines should be drawn.
            /// </summary>
            public Boolean DrawGrid {
                get { return _drawGrid; }
                set { _drawGrid = value; }
            }
            /// <summary>
            /// Gets the axis collection attached to the host.
            /// </summary>
            public System.Collections.CollectionBase Axises {
                get { return _axises; }
            }
            /// <summary>
            /// Gets the owner of the host.
            /// </summary>
            public Chart Owner {
                get { return _owner; }
            }
            /// <summary>
            /// Gets the location of an Axis in a Chart, specified by axis object.
            /// </summary>
            public float getAxisLocation(object axis) {
                if (_axises.Count == 0) return 0f;
                int axisIndex = -1;
                Boolean isCategory = false;
                if (_axises.GetType() == typeof(AxisCollection)) {
                    isCategory = true;
                    if (axis.GetType() == typeof(Axis)) {
                        AxisCollection ac = (AxisCollection)_axises;
                        axisIndex = ac.IndexOf((Axis)axis);
                    }
                } else {
                    if (axis.GetType() == typeof(ValueAxis)) {
                        ValueAxisCollection vac = (ValueAxisCollection)_axises;
                        axisIndex = vac.IndexOf((ValueAxis)axis);
                    }
                }
                if (axisIndex == -1) return 0f;
                if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
                    if (isCategory) {
                        return (float)(_rect.X + (axisIndex * _rect.Width / _axises.Count) + (_rect.Width / (_axises.Count * 2)));
                    } else {
                        if (_axises.Count == 1) return (float)(_rect.X + (_rect.Width / 2));
                        else return (float)(_rect.X + (axisIndex * _rect.Width / (_axises.Count - 1)));
                    }
                } else {
                    if (isCategory) {
                        return (float)(_rect.Bottom - (axisIndex * _rect.Height / _axises.Count) + (_rect.Height / (_axises.Count * 2)));
                    } else {
                        if (_axises.Count == 1) return (float)(_rect.Bottom - (_rect.Height / 2));
                        else return (float)(_rect.Bottom - (axisIndex * _rect.Height / (_axises.Count - 1)));
                    }
                }
            }
            /// <summary>
            /// Gets the location of an Axis in a Chart, specified by axis's index.
            /// </summary>
            public float getAxisLocation(int index) {
                if (_axises.Count == 0) return 0f;
                if (index < 0 || index >= _axises.Count) return 0f;
                Boolean isCategory = _axises.GetType() == typeof(AxisCollection);
                if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
                    if (isCategory) {
                        return (float)(_rect.X + (index * _rect.Width / _axises.Count) + (_rect.Width / (_axises.Count * 2)));
                    } else {
                        if (_axises.Count == 1) return (float)(_rect.X + (_rect.Width / 2));
                        else return (float)(_rect.X + (index * _rect.Width / (_axises.Count - 1)));
                    }
                } else {
                    if (isCategory) {
                        return (float)(_rect.Bottom - (index * _rect.Height / _axises.Count) + (_rect.Height / (_axises.Count * 2)));
                    } else {
                        if (_axises.Count == 1) return (float)(_rect.Bottom - (_rect.Height / 2));
                        else return (float)(_rect.Bottom - (index * getAxisRange()));
                    }
                }
            }
            /// <summary>
            /// Gets width or height range used for an axis.
            /// </summary>
            public float getAxisRange() {
                if (_axises.Count == 0) return 0f;
                if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
                    if (_axises.GetType() == typeof(AxisCollection)) {
                        return (float)(_rect.Width / _axises.Count);
                    } else {
                        if (_axises.Count > 1) return (float)(_rect.Width / (_axises.Count - 1));
                        else return 0f;
                    }
                } else {
                    if (_axises.GetType() == typeof(AxisCollection)) {
                        return (float)(_rect.Height / _axises.Count);
                    } else {
                        if (_axises.Count > 1) return (float)(_rect.Height / (_axises.Count - 1));
                        else return 0f;
                    }
                }
            }
            /// <summary>
            /// Gets the location of a value in a Chart.
            /// </summary>
            public float getValueLocation(double value) {
                if (_axises.Count == 0) return 0f;
                if (_axises.GetType() == typeof(AxisCollection)) return 0f;
                ValueAxisCollection vac = (ValueAxisCollection)_axises;
                if (value < vac[0].Value || value > vac[vac.Count - 1].Value) return 0f;
                int i = 0;
                while (i < vac.Count) {
                    if (vac[i].Value > value) { 
                        float result = getAxisLocation(i - 1);
                        double dValue = value - vac[i - 1].Value;
                        result -= (float)(dValue * getAxisRange() / (vac[i].Value - vac[i - 1].Value));
                        return result;
                    }
                    i++;
                }
                return 0f;
            }
            /// <summary>
            /// Gets the maximum size of area needed by axis.
            /// </summary>
            public float getMaxSize(Graphics g) {
                if (_axises.Count == 0) return 0f;
                if (_axises.GetType() == typeof(AxisCollection)) {
                    AxisCollection ac = (AxisCollection)_axises;
                    float w = 0f;
                    foreach (Axis ax in ac) {
                        SizeF sz = g.MeasureString(ax.Text, _owner._chartFont);
                        if (w < sz.Width) w = sz.Width;
                    }
                    return (float)(w + _owner._chartFont.Height + 2);
                } else {
                    ValueAxisCollection vac = (ValueAxisCollection)_axises;
                    float w = 0f;
                    foreach (ValueAxis vax in vac) {
                        SizeF sz = g.MeasureString(vax.Text, _owner._chartFont);
                        if (w < sz.Width) w = sz.Width;
                    }
                    return (float)(w + _owner._chartFont.Height + 2);
                }
            }
            /// <summary>
            /// Draw the axis host to a specified graphics object.
            /// </summary>
            public void draw(Graphics g) {
                if (_axises.Count == 0) return;
                Boolean isCategory = _axises.GetType() == typeof(AxisCollection);
                float range = getAxisRange();
                RectangleF rectTxt;
                Brush curBrush = _owner.Enabled ? Brushes.Black : Brushes.Gray;
                Pen curPen = _owner.Enabled ? Pens.Black : Pens.Gray;
                if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
                    int i = 0;
                    if (isCategory) { 
                        while(i<=_axises.Count) {
                            g.DrawLine(curPen, _rect.X + (i * range), _rect.Y, _rect.X + (i * range), _rect.Y + LINE_LENGTH);
                            i++;
                        }
                        AxisCollection ac = (AxisCollection)_axises;
                        i = 0;
                        while (i < ac.Count) {
                            if (ac[i].Text != "") { 
                                rectTxt = new RectangleF(_rect.X + (i * range), _rect.Y, range, _rect.Height);
                                g.DrawString(ac[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._vStrFormat);
                            }
                            i++;
                        }
                    } else {
                        while (i < _axises.Count) {
                            g.DrawLine(curPen, _rect.X + (i * range), _rect.Y, _rect.X + (i * range), _rect.Y + LINE_LENGTH);
                            i++;
                        }
                        ValueAxisCollection vAc = (ValueAxisCollection)_axises;
                        i = 0;
                        while (i < vAc.Count) {
                            if (vAc[i].Text != "") { 
                                rectTxt = new RectangleF(_rect.X + (i * range) - (range / 2), _rect.Y, range, _rect.Height);
                                g.DrawString(vAc[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._vStrFormat);
                            }
                            i++;
                        }
                    }
                    if (_drawTitle && _title != "") { 
                        rectTxt = new RectangleF(_rect.X, _rect.Bottom - _owner._chartFont.Height, _rect.Width, _owner._chartFont.Height);
                        g.DrawString(_title, _owner._chartFont, curBrush, rectTxt, _owner._hCenterFormat);
                    }
                } else {
                    int i = 0;
                    if (_rect.X == 1) {
                        if (isCategory) {
                            while (i <= _axises.Count) {
                                g.DrawLine(curPen, _rect.Right, _rect.Bottom - (i * range), _rect.Right - LINE_LENGTH, _rect.Bottom - (i * range));
                                i++;
                            }
                            AxisCollection ac = (AxisCollection)_axises;
                            i = 0;
                            while (i < ac.Count) {
                                if (ac[i].Text != "") { 
                                    rectTxt = new RectangleF(_rect.X, _rect.Bottom, _rect.Width - LINE_LENGTH, range);
                                    g.DrawString(ac[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._hRightFormat);
                                }
                                i++;
                            }
                        } else {
                            while (i < _axises.Count) {
                                g.DrawLine(curPen, _rect.Right, _rect.Bottom - (i * range), _rect.Right - LINE_LENGTH, _rect.Bottom - (i * range));
                                i++;
                            }
                            ValueAxisCollection vAc = (ValueAxisCollection)_axises;
                            i = 0;
                            while (i < vAc.Count) {
                                if (vAc[i].Text != "") { 
                                    rectTxt = new RectangleF(_rect.X, _rect.Bottom - ((i * range) + (range / 2)), _rect.Width - LINE_LENGTH, range);
                                    g.DrawString(vAc[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._hRightFormat);
                                }
                                i++;
                            }
                        }
                        if (_drawTitle && _title != "") { 
                            rectTxt = new RectangleF(_rect.X, _rect.Y, _owner._chartFont.Height, _rect.Height);
                            g.DrawString(_title, _owner._chartFont, curBrush, rectTxt, _owner._vCenterFormat);
                        }
                    } else {
                        if (isCategory) {
                            while (i <= _axises.Count) {
                                g.DrawLine(curPen, _rect.X, _rect.Bottom - (i * range), _rect.X + LINE_LENGTH, _rect.Bottom - (i * range));
                                i++;
                            }
                            AxisCollection ac = (AxisCollection)_axises;
                            i = 0;
                            while (i < ac.Count) {
                                if (ac[i].Text != "") { 
                                    rectTxt = new RectangleF(_rect.X + LINE_LENGTH, _rect.Bottom, _rect.Width - LINE_LENGTH, range);
                                    g.DrawString(ac[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._hLeftFormat);
                                }
                                i++;
                            }
                        } else {
                            while (i < _axises.Count) {
                                g.DrawLine(curPen, _rect.X, _rect.Bottom - (i * range), _rect.X + LINE_LENGTH, _rect.Bottom - (i * range));
                                i++;
                            }
                            ValueAxisCollection vAc = (ValueAxisCollection)_axises;
                            i = 0;
                            while (i < vAc.Count) {
                                if (vAc[i].Text != "") { 
                                    rectTxt = new RectangleF(_rect.X + LINE_LENGTH, _rect.Bottom - ((i * range) + (range / 2)), _rect.Width - LINE_LENGTH, range);
                                    g.DrawString(vAc[i].Text, _owner._chartFont, curBrush, rectTxt, _owner._hLeftFormat);
                                }
                                i++;
                            }
                        }
                        if (_drawTitle && _title != "") { 
                            rectTxt = new RectangleF(_rect.Right - _owner._chartFont.Height, _rect.Y, _owner._chartFont.Height, _rect.Height);
                            g.DrawString(_title, _owner._chartFont, curBrush, rectTxt, _owner._vCenterFormat);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Class to handle visualization of a ChartItem in a Chart.
        /// </summary>
        private class ItemHost {
            ChartItem _item;
            Chart _owner;
            SolidBrush _chartBrush;
            SolidBrush _chartShadowBrush = new SolidBrush(System.Drawing.Color.FromArgb(32, 0, 0, 0));
            Pen _chartPen;
            Pen _chartShadowPen = new Pen(System.Drawing.Color.FromArgb(32, 0, 0, 0), 2);
            List<ValueHost> _hosts = new List<ValueHost>();
            /// <summary>
            /// Class to handle a value of a ChartItem.
            /// </summary>
            public class ValueHost {
                ItemHost _owner;
                ChartItem.ChartValue _value;
                RectangleF _rect;
                PointF _centerPoint;
                Boolean _onHover = false;
                #region Constructor
                public ValueHost(ItemHost owner, ChartItem.ChartValue value) {
                    _owner = owner;
                    _value = value;
                }
                #endregion
                #region Public Properties
                public PointF CenterPoint {
                    get { return _centerPoint; }
                    set { _centerPoint = value; }
                }
                public RectangleF Rectangle {
                    get { return _rect; }
                    set { _rect = value; }
                }
                public Boolean OnHover {
                    get { return _onHover; }
                    set { _onHover = value; }
                }
                public ChartItem.ChartValue Value { get { return _value; } }
                public ItemHost Owner { get { return _owner; } }
                #endregion
                #region Public Methods
                /// <summary>
                /// Determine the mouse pointer is moved over the value area of a chart item.
                /// </summary>
                public Boolean mouseMove(Point p) {
                    if (_rect.Contains(p)) {
                        if (!_onHover) {
                            if (_owner._owner._hoverValue != null) _owner._owner._hoverValue._onHover = false;
                            _onHover = true;
                            _owner._owner._hoverValue = this;
                            return true;
                        }
                    } else {
                        if (_onHover) {
                            _onHover = false;
                            if (_owner._owner._hoverValue == this) _owner._owner._hoverValue = null;
                            return true;
                        }
                    }
                    return false;
                }
                /// <summary>
                /// Draw the value of a chart item to the specified Graphics object.
                /// </summary>
                public void draw(Graphics g) {
                    if (_centerPoint.X == 0 || _centerPoint.Y == 0) return;
                    PointF[] pts;
                    switch (_owner._item.Type) { 
                        case ChartType.Bar:
                        case ChartType.BarLine:
                            LinearGradientBrush barGlow=new LinearGradientBrush(_rect, 
                                Color.Black, Color.White, LinearGradientMode.Horizontal);
                            barGlow.InterpolationColors = Renderer.Drawing.BarGlow;
                            g.FillRectangle(_owner._chartBrush, _rect);
                            g.FillRectangle(barGlow, _rect);
                            barGlow.Dispose();
                            g.DrawRectangle(Pens.Black, new Rectangle((int)_rect.X,(int)_rect.Y ,(int)_rect.Width,(int)_rect.Height));
                            break;
                        case ChartType.LineDot:
                            g.FillEllipse(_owner._chartBrush, _rect);
                            g.DrawEllipse(Pens.Black, _rect);
                            break;
                        case ChartType.LineSquare:
                            g.FillRectangle(_owner._chartBrush, _rect);
                            g.DrawRectangle(Pens.Black, new Rectangle((int)_rect.X, (int)_rect.Y, (int)_rect.Width, (int)_rect.Height));
                            break;
                        case ChartType.LineStar:
                            pts = Ai.Renderer.Drawing.getStarPoints(new Point((int)_centerPoint.X, (int)_centerPoint.Y), 4f);
                            g.FillPolygon(_owner._chartBrush, pts);
                            g.DrawLines(Pens.Black, pts);
                            break;
                        case ChartType.LineTriangle:
                            pts = Ai.Renderer.Drawing.getTrianglePoints(new Point((int)_centerPoint.X, (int)_centerPoint.Y), 4f);
                            g.FillPolygon(_owner._chartBrush, pts);
                            g.DrawLines(Pens.Black, pts);
                            break;
                    }
                    if (_onHover) {
                        string str = _owner._item.Name + " : " + _value.Value.ToString("F2", Renderer.Drawing.en_us_ci);
                        SizeF sz = g.MeasureString(str, _owner._owner._chartFont);
                        RectangleF rectStr = new RectangleF((float)_centerPoint.X - (sz.Width / 2),
                            (float)_centerPoint.Y - (sz.Height / 2), sz.Width, sz.Height);
                        g.FillRectangle(Brushes.White, rectStr);
                        g.DrawRectangle(Pens.Black, new Rectangle((int)rectStr.X, (int)rectStr.Y, (int)rectStr.Width, (int)rectStr.Height));
                        g.DrawString(str, _owner._owner._chartFont, Brushes.Black, rectStr);
                    }
                }
                /// <summary>
                /// Draw the shadow of a chart item to the specified Graphics object.
                /// </summary>
                public void drawShadow(Graphics g) {
                    if (_centerPoint.X == 0 || _centerPoint.Y == 0) return;
                    RectangleF rectSdw = _rect;
                    PointF ctrSdw = _centerPoint;
                    PointF[] pts;
                    rectSdw.X += 2;
                    rectSdw.Y += 2;
                    ctrSdw.X += 2;
                    ctrSdw.Y += 2;
                    switch (_owner._item.Type) { 
                        case ChartType.Bar:
                        case ChartType.BarLine:
                            rectSdw.Y -= 2;
                            g.FillRectangle(_owner._chartShadowBrush, rectSdw);
                            break;
                        case ChartType.LineDot:
                            g.FillEllipse(_owner._chartShadowBrush, rectSdw);
                            break;
                        case ChartType.LineSquare:
                            g.FillRectangle(_owner._chartShadowBrush, rectSdw);
                            break;
                        case ChartType.LineStar:
                            pts = Ai.Renderer.Drawing.getStarPoints(new Point((int)ctrSdw.X, (int)ctrSdw.Y), 4f);
                            g.FillPolygon(_owner._chartShadowBrush, pts);
                            break;
                        case ChartType.LineTriangle:
                            pts = Ai.Renderer.Drawing.getTrianglePoints(new Point((int)ctrSdw.X, (int)ctrSdw.Y), 4f);
                            g.FillPolygon(_owner._chartShadowBrush, pts);
                            break;
                    }
                    if (_onHover) {
                        string str = _owner._item.Name + " : " + _value.Value.ToString("F2", Renderer.Drawing.en_us_ci);
                        SizeF sz = g.MeasureString(str, _owner._owner._chartFont);
                        RectangleF rectStr = new RectangleF((float)(ctrSdw.X - (sz.Width / 2)),
                            (float)(ctrSdw.Y - (sz.Height / 2)), sz.Width, sz.Height);
                        g.FillRectangle(_owner._chartShadowBrush, new Rectangle((int)rectStr.X, (int)rectStr.Y, (int)rectStr.Width, (int)rectStr.Height));
                    }
                }
                /// <summary>
                /// Draw the reference line of a chart item to the specified Graphics object.
                /// </summary>
                public void drawLineRef(Graphics g) {
                    PointF p2 = new PointF(0F, _centerPoint.Y);
                    if (_owner._item.Reference == AxisReference.FirstAxis) p2.X = (float)_owner._owner._catHost.Location.X;
                    else p2.X = (float)_owner._owner._secHost.Location.X;
                    g.DrawLine(_owner._owner._dottedPen, _centerPoint, p2);
                }
                #endregion
            }
            public ItemHost(ChartItem item, Chart owner) { 
                _item = item;
                _owner = owner;
                _chartBrush = new SolidBrush(_item.Color);
                _chartPen = new Pen(_item.Color, 2);
                populateHosts();
                _item.ChartTypeChanged += item_ChartTypeChanged;
                _item.ColorChanged += item_ColorChanged;
                _item.NameChanged += item_NameChanged;
                _item.ReferenceChanged += item_ReferenceChanged;
                _item.ValueAdded += item_ValueAdded;
                _item.ValueChanged += item_ValueChanged;
                _item.ValueRemoved += item_ValueRemoved;
                _item.VisibleChanged += item_VisibleChanged;
            }
            public ChartItem Item { get { return _item; } }
            #region Public Methods
            public void relocateHosts() {
                int i = 0;
                int barCount = 0;
                int barIndex = 0;
                if (_item.Type == ChartType.Bar || _item.Type == ChartType.BarLine) {
                    foreach (ChartItem ci in _owner._items) {
                        if (ci.Type == ChartType.Bar || ci.Type == ChartType.BarLine) {
                            barCount += 1;
                            if (ci == _item) barCount -= 1;
                        }
                    }
                }
                while (i < _hosts.Count) {
                    ValueHost h = _hosts[i];
                    PointF p = new PointF(0, 0);
                    p.X = _owner._catHost.getAxisLocation(i);
                    if (_item.Reference == AxisReference.FirstAxis) p.Y = _owner._priHost.getValueLocation(h.Value.Value);
                    else p.Y = _owner._secHost.getValueLocation(h.Value.Value);
                    h.CenterPoint = p;
                    if (_item.Type == ChartType.Bar || _item.Type == ChartType.BarLine) {
                        int rWidth = (int)((_owner._catHost.getAxisRange() / barCount) - 4);
                        if (rWidth > MAX_BAR_WIDTH) rWidth = MAX_BAR_WIDTH;
                        p.X -= (rWidth * barCount / 2);
                        p.X += (rWidth * barIndex) + (rWidth / 2);
                        h.CenterPoint = p;
                        h.Rectangle = new RectangleF(p.X - (rWidth / 2), p.Y, rWidth, _owner._catHost.Location.Y - p.Y);
                    } else {
                        h.Rectangle = new RectangleF(p.X - 4, p.Y - 4, 9, 9);
                    }
                    i++;
                }
            }
            public void removeHandlers() { 
                _item.ChartTypeChanged -= item_ChartTypeChanged;
                _item.ColorChanged -= item_ColorChanged;
                _item.NameChanged -= item_NameChanged;
                _item.ReferenceChanged -= item_ReferenceChanged;
                _item.ValueAdded -= item_ValueAdded;
                _item.ValueChanged -= item_ValueChanged;
                _item.ValueRemoved -= item_ValueRemoved;
                _item.VisibleChanged -= item_VisibleChanged;
            }
            public void drawLineRef(Graphics g) {
                if (_item.DrawLineRef) {
                    foreach (ValueHost vh in _hosts) vh.drawLineRef(g);
                }
            }
            public void draw(Graphics g) {
                if (_item.Type != ChartType.Bar) {
                    int i = 0;
                    while (i < _hosts.Count - 1) {
                        PointF p1 = _hosts[i].CenterPoint;
                        PointF p2 = _hosts[i + 1].CenterPoint;
                        p1.X += 2;
                        p1.Y += 2;
                        p2.X += 2;
                        p2.Y += 2;
                        g.DrawLine(_chartShadowPen, p1, p2);
                    }
                }
                foreach (ValueHost vh in _hosts) vh.drawShadow(g);
                if (_item.Type != ChartType.Bar) {
                    int i = 0;
                    while (i < _hosts.Count - 1) {
                        PointF p1 = _hosts[i].CenterPoint;
                        PointF p2 = _hosts[i + 1].CenterPoint;
                        p1.X += 2;
                        p1.Y += 2;
                        p2.X += 2;
                        p2.Y += 2;
                        g.DrawLine(_chartPen, p1, p2);
                    }
                }
                foreach (ValueHost vh in _hosts) vh.draw(g);
            }
            public Boolean mouseMove(Point p) {
                Boolean changed = false;
                foreach (ValueHost vh in _hosts) changed = changed || vh.mouseMove(p);
                return changed;
            }
            #endregion
            #region Private Methods
            private void populateHosts() {
                _hosts.Clear();
                foreach (ChartItem.ChartValue cv in _item.Values) {
                    ValueHost vh = new ValueHost(this, cv);
                    _hosts.Add(vh);
                }
                relocateHosts();
            }
            private void item_ValueChanged(object sender, EventArgs e) {
                relocateHosts();
                if (_item.Visible) _owner.Invalidate();
            }
            private void item_ValueAdded(object sender, EventArgs e) {
                populateHosts();
                if (_item.Visible) _owner.Invalidate();
            }
            private void item_ValueRemoved(object sender, EventArgs e) {
                populateHosts();
                if (_item.Visible) _owner.Invalidate();
            }
            private void item_NameChanged(object sender, EventArgs e) { if (_item.Visible) _owner.Invalidate(); }
            private void item_ColorChanged(object sender, EventArgs e) { if (_item.Visible) _owner.Invalidate(); }
            private void item_VisibleChanged(object sender, EventArgs e) { 
                _owner.reloadHeader();
                _owner.Invalidate();
            }
            private void item_ReferenceChanged(object sender, EventArgs e) {
                relocateHosts();
                if (_item.Visible) _owner.Invalidate();
            }
            private void item_ChartTypeChanged(object sender, EventArgs e) {
                relocateHosts();
                if (_item.Visible) _owner.Invalidate();
            }
            #endregion
        }
        /// <summary>
        /// Class to handle visualization the header of a Chart.
        /// </summary>
        private class ChartHeader {
            Chart _owner;
            List<RectangleF> _rects = new List<RectangleF>();
            RectangleF _rect;
            public ChartHeader(Chart owner) { _owner = owner; }
            public RectangleF Bound { get { return _rect; } }
            #region Public Methods
            public void measureRectangle(Graphics g) {
                SizeF sz;
                float nmWidth = 0f;
                _rects.Clear();
                _rect = new RectangleF(1, 1, _owner.Width - 2, _owner.Font.Height);
                sz = g.MeasureString(_owner.Text, _owner.Font, (int)_rect.Width);
                _rect.Height = sz.Height;
                _rects.Add(new RectangleF(1, 1, _owner.Width - 2, sz.Height));
                foreach (ChartItem ci in _owner._items) {
                    if (ci.Name != "") {
                        sz = g.MeasureString(ci.Name, _owner._chartFont);
                        if (nmWidth < sz.Width + 40) nmWidth = sz.Width + 40;
                    }
                }
                float x = 1f;
                float y = _rect.Bottom;
                foreach (ChartItem ci in _owner._items) {
                    RectangleF rectItem = new RectangleF(x, y, nmWidth, _owner._chartFont.Height);
                    _rects.Add(rectItem);
                    x += nmWidth;
                    if (x + nmWidth > _rect.Width) { 
                        x = 1f;
                        y += _owner._chartFont.Height;
                        _rect.Height += _owner._chartFont.Height;
                    }
                }
            }
            public void draw(Graphics g) {
                g.DrawString(_owner.Text, _owner.Font, _owner.Enabled ? Brushes.Black : Brushes.Gray, 
                    _rects[0], _owner._hCenterFormat);
                int i = 0;
                while (i < _owner._items.Count) {
                    drawItemLegend(g, _owner._items[i], _rects[i + 1]);
                    i++;
                }
            }
            #endregion
            #region Private Methods
            private void drawItemLegend(Graphics g, ChartItem item, RectangleF rect) {
                RectangleF rectTxt, rectImg;
                SolidBrush cBrush = new SolidBrush(item.Color);
                PointF[] pts;
                rectTxt = new RectangleF(rect.X + 35, rect.Y, rect.Width - 38, rect.Height);
                rectImg = new RectangleF(rect.X, rect.Y, 30, rect.Height);
                g.DrawString(item.Name, _owner._chartFont, _owner.Enabled ? Brushes.Black : Brushes.Gray, 
                    rectTxt, _owner._hLeftFormat);
                switch (item.Type) { 
                    case ChartType.Bar:
                    case ChartType.BarLine:
                        g.FillRectangle(cBrush, rectImg.X, rectImg.Y + 2, rectImg.Width, rectImg.Height - 4);
                        break;
                    default:
                        PointF cPoint = new PointF((float)(rectImg.X + (rectImg.Width / 2)), (float)(rectImg.Y + (rectImg.Height / 2)));
                        g.FillRectangle(cBrush, rectImg.X, rectImg.Y + 5, rectImg.Width, rectImg.Height - 10);
                        switch (item.Type) { 
                            case ChartType.LineDot:
                                RectangleF rectDot = new RectangleF(cPoint.X - (rectImg.Height / 2), rectImg.Y, rectImg.Height, rectImg.Height);
                                g.FillEllipse(cBrush, rectDot);
                                g.DrawEllipse(Pens.Black, new Rectangle((int)rectDot.X, (int)rectDot.Y, (int)rectDot.Width, (int)rectDot.Height));
                                break;
                            case ChartType.LineSquare:
                                RectangleF rectSqr = new RectangleF(cPoint.X - (rectImg.Height / 2), rectImg.Y, rectImg.Height, rectImg.Height);
                                g.FillRectangle(cBrush, rectSqr);
                                g.DrawRectangle(Pens.Black, new Rectangle((int)rectSqr.X, (int)rectSqr.Y, (int)rectSqr.Width, (int)rectSqr.Height));
                                break;
                            case ChartType.LineStar:
                                pts = Ai.Renderer.Drawing.getStarPoints(new Point((int)cPoint.X,(int)cPoint.Y), (float)(rectImg.Height / 2));
                                g.FillPolygon(cBrush, pts);
                                g.DrawPolygon(Pens.Black, pts);
                                break;
                            case ChartType.LineTriangle:
                                pts = Ai.Renderer.Drawing.getTrianglePoints(new Point((int)cPoint.X, (int)cPoint.Y), (float)(rectImg.Height / 2));
                                g.FillPolygon(cBrush, pts);
                                g.DrawPolygon(Pens.Black, pts);
                                break;
                        }
                        break;
                }
                cBrush.Dispose();
            }
            #endregion
        }
        #endregion
        #region Constructor - Destructor
        public Chart() {
            // Initializing control styles.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            base.BackColor = Color.White;
            // Dotted pen
            _dottedPen.DashPattern = new Single[] { 1.0F, 3.0F, 1.0F, 3.0F };
            // Initializing string format
            _hLeftFormat.LineAlignment = StringAlignment.Center;
            _hRightFormat.LineAlignment = StringAlignment.Center;
            _hCenterFormat.LineAlignment = StringAlignment.Center;
            _hLeftFormat.Alignment = StringAlignment.Near;
            _hRightFormat.Alignment = StringAlignment.Far;
            _hCenterFormat.Alignment = StringAlignment.Center;
            _vStrFormat.Alignment = StringAlignment.Near;
            _vCenterFormat.Alignment = StringAlignment.Center;
            _vStrFormat.LineAlignment = StringAlignment.Center;
            _vCenterFormat.LineAlignment = StringAlignment.Center;
            _vStrFormat.FormatFlags = _vStrFormat.FormatFlags | StringFormatFlags.DirectionVertical | StringFormatFlags.NoWrap;
            _vCenterFormat.FormatFlags = _vCenterFormat.FormatFlags | StringFormatFlags.DirectionVertical | StringFormatFlags.NoWrap;
            _hLeftFormat.FormatFlags = _hLeftFormat.FormatFlags | StringFormatFlags.NoWrap;
            _hRightFormat.FormatFlags = _hRightFormat.FormatFlags | StringFormatFlags.NoWrap;
            _hCenterFormat.FormatFlags = _hCenterFormat.FormatFlags | StringFormatFlags.NoWrap;
            _hLeftFormat.Trimming = StringTrimming.EllipsisCharacter;
            _hRightFormat.Trimming = StringTrimming.EllipsisCharacter;
            _hCenterFormat.Trimming = StringTrimming.EllipsisCharacter;
            _vStrFormat.Trimming = StringTrimming.EllipsisCharacter;
            _vCenterFormat.Trimming = StringTrimming.EllipsisCharacter;
            // Initializing collections
            _items = new ChartItemCollection(this);
            _catAxis = new AxisCollection(this);
            _priAxis = new ValueAxisCollection(this);
            _secAxis = new ValueAxisCollection(this);
            // Initializing hosts
            _catHost = new AxisHost(this, _catAxis);
            _catHost.Orientation = Orientation.Horizontal;
            _priHost = new AxisHost(this, _priAxis);
            _priHost.Orientation = Orientation.Vertical;
            _secHost = new AxisHost(this, _secAxis);
            _secHost.Orientation = Orientation.Vertical;
            _header = new ChartHeader(this);
            _itemHosts = new List<ItemHost>();
            // Setting up tooltip
            _tooltip = new ToolTip(this);
            _tooltip.AnimationSpeed = 20;
            // Attaching events.
            // Chart's events
            this.EnabledChanged += Chart_EnabledChanged;
            this.FontChanged += Chart_FontChanged;
            this.MouseLeave += Chart_MouseLeave;
            this.MouseMove += Chart_MouseMove;
            this.Paint += Chart_Paint;
            this.Resize += Chart_Resize;
            this.TextChanged += Chart_TextChanged;
            // Item Collection's events.
            _items.AfterClear += _items_AfterClear;
            _items.AfterInsert += _items_AfterInsert;
            _items.AfterRemove += _items_AfterRemove;
            _items.Clearing += _items_Clearing;
            // Category Axis's events
            _catAxis.AfterClear += _catAxis_AfterClear;
            _catAxis.AfterInsert += _catAxis_AfterInsert;
            _catAxis.Clearing += _catAxis_Clearing;
            _catAxis.Removing += _catAxis_Removing;
            // Primary Axis's events
            _priAxis.AfterClear += _priAxis_AfterClear;
            _priAxis.AfterInsert += _priAxis_AfterInsert;
            _priAxis.AfterRemove += _priAxis_AfterRemove;
            _priAxis.Clearing += _priAxis_Clearing;
            _priAxis.Removing += _priAxis_Removing;
            // Secondary Axis's events
            _secAxis.AfterClear += _secAxis_AfterClear;
            _secAxis.AfterInsert += _secAxis_AfterInsert;
            _secAxis.AfterRemove += _secAxis_AfterRemove;
            _secAxis.Clearing += _secAxis_Clearing;
            _secAxis.Removing += _secAxis_Removing;
            base.Size = new Size(100, 100);
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                // Detaching events.
                // Chart's events
                this.EnabledChanged -= Chart_EnabledChanged;
                this.FontChanged -= Chart_FontChanged;
                this.MouseLeave -= Chart_MouseLeave;
                this.MouseMove -= Chart_MouseMove;
                this.Paint -= Chart_Paint;
                this.Resize -= Chart_Resize;
                this.TextChanged -= Chart_TextChanged;
                // Item Collection's events.
                _items.AfterClear -= _items_AfterClear;
                _items.AfterInsert -= _items_AfterInsert;
                _items.AfterRemove -= _items_AfterRemove;
                _items.Clearing -= _items_Clearing;
                // Category Axis's events
                _catAxis.AfterClear -= _catAxis_AfterClear;
                _catAxis.AfterInsert -= _catAxis_AfterInsert;
                _catAxis.Clearing -= _catAxis_Clearing;
                _catAxis.Removing -= _catAxis_Removing;
                // Primary Axis's events
                _priAxis.AfterClear -= _priAxis_AfterClear;
                _priAxis.AfterInsert -= _priAxis_AfterInsert;
                _priAxis.AfterRemove -= _priAxis_AfterRemove;
                _priAxis.Clearing -= _priAxis_Clearing;
                _priAxis.Removing -= _priAxis_Removing;
                // Secondary Axis's events
                _secAxis.AfterClear -= _secAxis_AfterClear;
                _secAxis.AfterInsert -= _secAxis_AfterInsert;
                _secAxis.AfterRemove -= _secAxis_AfterRemove;
                _secAxis.Clearing -= _secAxis_Clearing;
                _secAxis.Removing -= _secAxis_Removing;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether primary axis's grid lines appear in the chart.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether primary axis's grid lines appear in the chart.")]
        public bool PrimaryGrid {
            get { return _priHost.DrawGrid; }
            set {
                if (_priHost.DrawGrid != value) {
                    _priHost.DrawGrid = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether secondary axis's grid lines appear in the chart.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether secondary axis's grid lines appear in the chart.")]
        public bool SecondaryGrid {
            get { return _secHost.DrawGrid; }
            set {
                if (_secHost.DrawGrid != value) {
                    _secHost.DrawGrid = value;
                    this.Invalidate();
                }
            }
        }
        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor {
            get { return base.BackColor; }
            set {
                if (base.BackColor != value) {
                    base.BackColor = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the font of the chart displayed by the control.
        /// </summary>
        [Description("Gets or sets the font of the chart displayed by the control.")]
        public Font ChartFont {
            get { return _chartFont; }
            set {
                if (_chartFont != value) {
                    _chartFont = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets the collection of ChartItem that are assigned to the Chart control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of ChartItem that are assigned to the Chart control.")]
        public ChartItemCollection Items { get { return _items; } }
        /// <summary>
        /// Gets the collection of Axis as the category axis that are assigned to the Chart control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of Axis as the category axis that are assigned to the Chart control.")]
        public AxisCollection CategoryAxis { get { return _catAxis; } }
        /// <summary>
        /// Gets the collection of ValueAxis as the primary axis that are assigned to the Chart control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of ValueAxis as the primary axis that are assigned to the Chart control.")]
        public ValueAxisCollection PrimaryAxis { get { return _priAxis; } }
        /// <summary>
        /// Gets the collection of ValueAxis as the secondary axis that are assigned to the Chart control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of ValueAxis as the secondary axis that are assigned to the Chart control.")]
        public ValueAxisCollection SecondaryAxis { get { return _secAxis; } }
        /// <summary>
        /// Gets or sets the text displayed at the category axis in the Chart.
        /// </summary>
        [DefaultValue(""), Description("Gets or sets the text displayed at the category axis in the Chart.")]
        public string CategoryTitle {
            get { return _catHost.Title; }
            set {
                if (_catHost.Title != value) {
                    _catHost.Title = value;
                    reloadAll();
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the text displayed at the primary axis in the Chart.
        /// </summary>
        [DefaultValue(""), Description("Gets or sets the text displayed at the primary axis in the Chart.")]
        public string PrimaryTitle {
            get { return _priHost.Title; }
            set {
                if (_priHost.Title != value) {
                    _priHost.Title = value;
                    reloadAll();
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the text displayed at the secondary axis in the Chart.
        /// </summary>
        [DefaultValue(""), Description("Gets or sets the text displayed at the secondary axis in the Chart.")]
        public string SecondaryTitle {
            get { return _secHost.Title; }
            set {
                if (_secHost.Title != value) {
                    _secHost.Title = value;
                    reloadAll();
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether category title appears.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether category title appears.")]
        public bool ShowCategoryTitle {
            get { return _catHost.DrawTitle; }
            set {
                if (_catHost.DrawTitle != value) {
                    _catHost.DrawTitle = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether primary title appears.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether primary title appears.")]
        public bool ShowPrimaryTitle {
            get { return _priHost.DrawTitle; }
            set {
                if (_priHost.DrawTitle != value) {
                    _priHost.DrawTitle = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether secondary title appears.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether secondary title appears.")]
        public bool ShowSecondaryTitle {
            get { return _secHost.DrawTitle; }
            set {
                if (_secHost.DrawTitle != value) {
                    _secHost.DrawTitle = value;
                    this.Invalidate();
                }
            }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Provide a function to render the chart into a bitmap.
        /// </summary>
        public Bitmap toBitmap() {
            Bitmap aBitmap = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(aBitmap);
            draw(g);
            g.Dispose();
            return aBitmap;
        }
        #endregion
        #region Private Methods
        private void reloadHeader() {
            float hdrHeight = _header.Bound.Height;
            Graphics g = this.CreateGraphics();
            float dHeight;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _header.measureRectangle(g);
            dHeight = hdrHeight - _header.Bound.Height;
            _priHost.Size = new SizeF(_priHost.Size.Width, _priHost.Size.Height + dHeight);
            _priHost.Location = new PointF(_priHost.Location.X, _priHost.Location.Y - dHeight);
            _secHost.Size = new SizeF(_secHost.Size.Width, _priHost.Size.Height);
            _secHost.Location = new PointF(_secHost.Location.X, _priHost.Location.Y);
            _chartArea = new RectangleF(_catHost.Location.X, _priHost.Location.Y, _catHost.Size.Width, _priHost.Size.Height);
            foreach (ItemHost ih in _itemHosts) ih.relocateHosts();
            g.Dispose();
            this.Invalidate();
        }
        private void reloadAll() {
            Graphics g = this.CreateGraphics();
            float priWidth = 0f;
            float secWidth = 0f;
            float catHeight = 0f;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _header.measureRectangle(g);
            priWidth = _priHost.getMaxSize(g);
            secWidth = _secHost.getMaxSize(g);
            catHeight = _catHost.getMaxSize(g);
            _catHost.Location = new PointF(priWidth + 1, this.Height - (catHeight - 2));
            _catHost.Size = new SizeF(this.Width - (priWidth + secWidth + 2), catHeight);
            _priHost.Location = new PointF(1, _header.Bound.Bottom + 10);
            _priHost.Size = new SizeF(priWidth, _catHost.Location.Y - _priHost.Location.Y);
            _secHost.Location = new PointF(this.Width - (secWidth + 1), _header.Bound.Bottom + 10);
            _secHost.Size = new SizeF(secWidth, _priHost.Size.Height);
            _chartArea = new RectangleF(_catHost.Location.X, _priHost.Location.Y, _catHost.Size.Width, _priHost.Size.Height);
            foreach (ItemHost ih in _itemHosts) ih.relocateHosts();
            g.Dispose();
        }
        private void draw(Graphics g) {
            Pen lPen = this.Enabled ? Pens.Black : Pens.Gray;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(this.BackColor);
            _header.draw(g);
            _priHost.draw(g);
            _secHost.draw(g);
            _catHost.draw(g);
            if (_priHost.DrawGrid || _secHost.DrawGrid) {
                int i = 0;
                if (_priHost.DrawGrid) {
                    while (i < _priAxis.Count) {
                        Pen gPen = new Pen(_priAxis[i].Color);
                        float y = _priHost.getAxisLocation(i);
                        g.DrawLine(gPen, _catHost.Location.X, y, _secHost.Location.X, y);
                        gPen.Dispose();
                        i++;
                    }
                }
                if (_secHost.DrawGrid) {
                    i = 0;
                    while (i < _secAxis.Count) {
                        Pen gPen = new Pen(_secAxis[i].Color);
                        float y = _secHost.getAxisLocation(i);
                        g.DrawLine(gPen, _catHost.Location.X, y, _secHost.Location.X, y);
                        gPen.Dispose();
                        i++;
                    }
                }
            }
            g.DrawLine(lPen, _catHost.Location.X, _priHost.Location.Y, _catHost.Location.X, _catHost.Location.Y);
            g.DrawLine(lPen, _catHost.Location.X, _catHost.Location.Y, _secHost.Location.X, _catHost.Location.Y);
            if (_secAxis.Count > 0) g.DrawLine(lPen, _secHost.Location.X, _secHost.Location.Y, _secHost.Location.X, _catHost.Location.Y);
            foreach (ItemHost ih in _itemHosts) ih.drawLineRef(g);
            foreach (ItemHost ih in _itemHosts) ih.draw(g);
            if (_hoverValue != null) { 
                _hoverValue.drawShadow(g);
                _hoverValue.draw(g);
            }
            g.DrawRectangle(Pens.Gray, 0, 0, this.Width - 1, this.Height - 1);
        }
        #endregion
        #region Event Handlers
        private void Chart_EnabledChanged(object sender, EventArgs e) {
            if (_hoverValue != null) {
                _hoverValue.OnHover = false;
                _hoverValue = null;
            }
            this.Invalidate();
        }
        private void Chart_FontChanged(object sender, EventArgs e) { reloadHeader(); }
        private void Chart_MouseLeave(object sender, EventArgs e) {
            if (_hoverValue != null) {
                _hoverValue.OnHover = false;
                _hoverValue = null;
                this.Invalidate();
            }
        }
        private void Chart_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button != System.Windows.Forms.MouseButtons.None) return;
            bool changed = false;
            foreach (ItemHost ih in _itemHosts) changed = changed || ih.mouseMove(e.Location);
            if (changed) this.Invalidate();
        }
        private void Chart_Paint(object sender, PaintEventArgs e) { draw(e.Graphics); }
        private void Chart_Resize(object sender, EventArgs e) { reloadAll(); }
        private void Chart_TextChanged(object sender, EventArgs e) { reloadHeader(); }
        private void _items_AfterClear(object sender, CollectionEventArgs e) {
            _itemHosts.Clear();
            reloadHeader();
        }
        private void _items_AfterInsert(object sender, CollectionEventArgs e) {
            ChartItem newItem = (ChartItem)e.Item;
            ItemHost ih = new ItemHost(newItem, this);
            _itemHosts.Insert(e.Index, ih);
            reloadHeader();
        }
        private void _items_AfterRemove(object sender, CollectionEventArgs e) { reloadHeader(); }
        private void _items_Clearing(object sender, CollectionEventArgs e) {
            foreach (ItemHost ih in _itemHosts) ih.removeHandlers();
        }
        private void _items_Removing(object sender, CollectionEventArgs e) {
            ChartItem remItem = (ChartItem)e.Item;
            int i = 0;
            while (i < _itemHosts.Count) {
                if (_itemHosts[i].Item == remItem) {
                    _itemHosts[i].removeHandlers();
                    _itemHosts.RemoveAt(i);
                    return;
                }
                i++;
            }
        }
        private void _catAxis_AfterClear(object sender, CollectionEventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void _catAxis_AfterInsert(object sender, CollectionEventArgs e) {
            Axis ax = (Axis)e.Item;
            ax.TextChanged += axis_TextChanged;
            ax.ColorChanged += axis_ColorChanged;
            reloadAll();
            this.Invalidate();
        }
        private void _catAxis_Clearing(object sender, CollectionEventArgs e) {
            foreach (Axis ax in _catAxis) {
                ax.TextChanged -= axis_TextChanged;
                ax.ColorChanged -= axis_ColorChanged;
            }
        }
        private void _catAxis_Removing(object sender, CollectionEventArgs e) {
            Axis ax = (Axis)e.Item;
            ax.TextChanged -= axis_TextChanged;
            ax.ColorChanged -= axis_ColorChanged;
        }
        private void _priAxis_AfterClear(object sender, CollectionEventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void _priAxis_AfterInsert(object sender, CollectionEventArgs e) {
            ValueAxis vAx = (ValueAxis)e.Item;
            vAx.ValueChanged += axis_ValueChanged;
            vAx.TextChanged += axis_TextChanged;
            vAx.ColorChanged += axis_ColorChanged;
            reloadAll();
            this.Invalidate();
        }
        private void _priAxis_AfterRemove(object sender, CollectionEventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void _priAxis_Clearing(object sender, CollectionEventArgs e) {
            foreach (ValueAxis vAx in _priAxis) {
                vAx.ValueChanged -= axis_ValueChanged;
                vAx.TextChanged -= axis_TextChanged;
                vAx.ColorChanged -= axis_ColorChanged;
            }
        }
        private void _priAxis_Removing(object sender, CollectionEventArgs e) {
            ValueAxis vAx = (ValueAxis)e.Item;
            vAx.ValueChanged -= axis_ValueChanged;
            vAx.TextChanged -= axis_TextChanged;
            vAx.ColorChanged -= axis_ColorChanged;
        }
        private void _secAxis_AfterClear(object sender, CollectionEventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void _secAxis_AfterInsert(object sender, CollectionEventArgs e) {
            ValueAxis vAx = (ValueAxis)e.Item;
            vAx.ValueChanged += axis_ValueChanged;
            vAx.TextChanged += axis_TextChanged;
            vAx.ColorChanged += axis_ColorChanged;
            reloadAll();
            this.Invalidate();
        }
        private void _secAxis_AfterRemove(object sender, CollectionEventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void _secAxis_Clearing(object sender, CollectionEventArgs e) {
            foreach (ValueAxis vAx in _secAxis) {
                vAx.ValueChanged -= axis_ValueChanged;
                vAx.TextChanged -= axis_TextChanged;
                vAx.ColorChanged -= axis_ColorChanged;
            }
        }
        private void _secAxis_Removing(object sender, CollectionEventArgs e) {
            ValueAxis vAx = (ValueAxis)e.Item;
            vAx.ValueChanged -= axis_ValueChanged;
            vAx.TextChanged -= axis_TextChanged;
            vAx.ColorChanged -= axis_ColorChanged;
        }
        private void axis_ValueChanged(object sender, EventArgs e) {
            foreach (ItemHost ih in _itemHosts) ih.relocateHosts();
            this.Invalidate();
        }
        private void axis_TextChanged(object sender, EventArgs e) {
            reloadAll();
            this.Invalidate();
        }
        private void axis_ColorChanged(object sender, EventArgs e) { this.Invalidate(); }
        #endregion
    }
}