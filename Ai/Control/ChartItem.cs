// Ai Software Control Library.
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ai.Control {
    public enum ChartType {
        Bar = 0,
        BarLine = 1,
        Line = 2,
        LineDot = 3,
        LineSquare = 4,
        LineTriangle = 5,
        LineStar = 6
    }
    public enum AxisReference {
        FirstAxis = 0,
        SecondAxis = 1
    }
    /// <summary>
    /// Class to represent an Item of a Chart.
    /// </summary>
    public class ChartItem {
        #region Public Events
        /// <summary>
        /// Raised when a value of a ChartItem has been changed.
        /// </summary>
        [Description("Raised when a value of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> ValueChanged;
        /// <summary>
        /// Raised when a value has been added to a ChartItem.
        /// </summary>
        [Description("Raised when a value has been added to a ChartItem.")]
        public event EventHandler<EventArgs> ValueAdded;
        /// <summary>
        /// Raised when a value has been removed from a ChartItem.
        /// </summary>
        [Description("Raised when a value has been removed from a ChartItem.")]
        public event EventHandler<EventArgs> ValueRemoved;
        /// <summary>
        /// Raised whed visibility of a ChartItem has been changed.
        /// </summary>
        [Description("Raised whed visibility of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> VisibleChanged;
        /// <summary>
        /// Raised when the color of a ChartItem has been changed.
        /// </summary>
        [Description("Raised when the color of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> ColorChanged;
        /// <summary>
        /// Raised when the type of a ChartItem has been changed.
        /// </summary>
        [Description("Raised when the type of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> ChartTypeChanged;
        /// <summary>
        /// Raised when the name of a ChartItem has been changed.
        /// </summary>
        [Description("Raised when the name of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> NameChanged;
        /// <summary>
        /// Raised when the referenced axis of a ChartItem has been changed.
        /// </summary>
        [Description("Raised when the referenced axis of a ChartItem has been changed.")]
        public event EventHandler<EventArgs> ReferenceChanged;
        #endregion
        #region Public Classes
        /// <summary>
        /// Represent a value of a ChartItem.
        /// </summary>
        public class ChartValue {
            double _value = 0D;
            string _tooltip = "";
            internal ChartItem _owner;
            public ChartValue() { _owner = new ChartItem(); }
            public ChartValue(double value) {
                _value = value;
                _owner = new ChartItem();
            }
            public ChartValue(ChartItem owner) { _owner = owner; }
            public ChartValue(double value, ChartItem owner) {
                _value = value;
                _owner = owner;
            }
            /// <summary>
            /// Gets or sets a value of a ChartValue.
            /// </summary>
            [DefaultValue(typeof(double),"0D"), Description("Gets or sets a value of a ChartValue.")]
            public double Value {
                get { return _value; }
                set {
                    if (_value != value) {
                        _value = value;
                        _owner.invokeValueChanged(this);
                    }
                }
            }
            /// <summary>
            /// Gets or sets the tooltip text of a value of a ChartItem.
            /// </summary>
            [DefaultValue(""), EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
                typeof(System.Drawing.Design.UITypeEditor)),
            Description("Gets or sets the tooltip text of a value of a ChartItem.")]
            public string ToolTip {
                get { return _tooltip; }
                set { _tooltip = value; }
            }
        }
        public class ChartValueCollection : System.Collections.CollectionBase {
            internal ChartItem _owner;
            public ChartValueCollection(ChartItem owner) : base() {
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
            /// Gets a ChartValue object in the collection specified by its index.
            /// </summary>
            [Description("Gets a ChartValue object in the collection specified by its index.")]
            public ChartValue this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (ChartValue)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a ChartValue object in the collection.
            /// </summary>
            public int IndexOf(ChartValue item) {
                return List.IndexOf(item);
            }
            /// <summary>
            /// Determine whether a ChartValue object exist in the collection.
            /// </summary>
            public Boolean Contains(ChartValue item) { return List.Contains(item); }
            /// <summary>
            /// Add a ChartValue object to the collection.
            /// </summary>
            public ChartValue Add(ChartValue item) {
                // Avoid adding the same item multiple times.
                if (!Contains(item)) {
                    item._owner = _owner;
                    int index = List.Add(item);
                    return (ChartValue)List[index];
                }
                return item;
            }
            /// <summary>
            /// Add a ChartValue object to the collection by providing its value.
            /// </summary>
            public ChartValue Add(double value) {
                ChartValue anItem = new ChartValue(value);
                return this.Add(anItem);
            }
            /// <summary>
            /// Add a ChartValue collection to the collection.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(ChartValueCollection items) {
                foreach (ChartValue i in items) this.Add(i);
            }
            /// <summary>
            /// Insert a ChartValue object to the collection at specified index.
            /// </summary>
            public void Insert(int index, ChartValue item) {
                item._owner = _owner;
                List.Insert(index, item);
            }
            /// <summary>
            /// Remove a ChartValue object from the collection.
            /// </summary>
            public void Remove(ChartValue item) {
                if (List.Contains(item)) List.Remove(item);
            }
            #region Event Handlers
            /// <summary>
            /// Performs additional custom processes when validating a value.
            /// </summary>
            protected override void OnValidate(object value) {
                if (!typeof(ChartValue).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must ChartValue!", "value");
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
        string _name = "ChartItem";
        System.Drawing.Color _color = System.Drawing.Color.Black;
        string _tooltipTitle = "";
        Image _tooltipImage = null;
        ChartType _type = ChartType.Bar;
        Boolean _visible = true;
        AxisReference _reference = AxisReference.FirstAxis;
        Boolean _drawLineRef = false;
        ChartValueCollection _values;
        internal Chart _owner;
        public ChartItem() { 
            _owner = new Chart();
            _values = new ChartValueCollection(this);
            _values.AfterInsert += _values_AfterInsert;
            _values.AfterRemove += _values_AfterRemove;
        }
        public ChartItem(Chart owner) { 
            _owner = owner;
            _values = new ChartValueCollection(this);
            _values.AfterInsert += _values_AfterInsert;
            _values.AfterRemove += _values_AfterRemove;
        }
        private void invokeValueChanged(ChartValue value) { if (_visible) if (ValueChanged != null) ValueChanged(this, new EventArgs()); }
        /// <summary>
        /// Gets or sets the color of a ChartItem.
        /// </summary>
        [DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Gets or sets the color of a ChartItem.")]
        public System.Drawing.Color Color {
            get { return _color; }
            set {
                if (_color != value) {
                    _color = value;
                    if (_visible)
                        if (ColorChanged != null) ColorChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the name of a ChartItem.
        /// </summary>
        [DefaultValue("ChartItem"), Description("Gets or sets the name of a ChartItem.")]
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    if (_visible)
                        if (NameChanged != null) NameChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets how a ChartItem must be drawn in a Chart control.
        /// </summary>
        [DefaultValue(typeof(ChartType),"Bar"), Description("Gets or sets how a ChartItem must be drawn in a Chart control.")]
        public ChartType Type {
            get { return _type; }
            set {
                if (_type != value) {
                    _type = value;
                    if (_visible)
                        if (ChartTypeChanged != null) ChartTypeChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the visibility of a ChartItem.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets the visibility of a ChartItem.")]
        public Boolean Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;
                    if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the referenced axis of a ChartItem.
        /// </summary>
        [DefaultValue(typeof(AxisReference), "FirstAxis"), Description("Gets or sets the referenced axis of a ChartItem.")]
        public AxisReference Reference {
            get { return _reference; }
            set {
                if (_reference != value) {
                    _reference = value;
                    if (_visible)
                        if (ReferenceChanged != null) ReferenceChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the reference line should be drawn in the Chart.
        /// </summary>
        [DefaultValue(false), Description("Gets or sets a value indicating whether the reference line should be drawn in the Chart.")]
        public Boolean DrawLineRef {
            get { return _drawLineRef; }
            set {
                if (_drawLineRef != value) {
                    _drawLineRef = value;
                    if (_visible) _owner.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets the collection of ChartValue that are assigned to the ChartItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Description("Gets the collection of ChartValue that are assigned to the ChartItem.")]
        public ChartValueCollection Values {
            get { return _values; }
        }
        public void _values_AfterInsert(object sender, CollectionEventArgs e) {
            if (_visible)
                if (ValueAdded != null) ValueAdded(this, new EventArgs());
        }
        public void _values_AfterRemove(object sender, CollectionEventArgs e) {
            if (_visible)
                if (ValueRemoved != null) ValueRemoved(this, new EventArgs());
        }
    }
}