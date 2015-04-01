// Ai Software Control Library.
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Class to represent a collection of cells in a row in the DataGrid.
    /// </summary>
    [DefaultProperty("Cells")]
    public class DataGridItem {
        #region Public Classes
        /// <summary>
        /// A class to represent a cell in the DataGrid.
        /// </summary>
        [DefaultProperty("Value")]
        public class DataGridCell {
            #region Members
            internal DataGridItem _owner = null;
            internal System.Drawing.Font _font = null;
            object _value = null;
            object _tag = null;
            System.Drawing.Color _color = System.Drawing.Color.Black;
            System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
            string _name = "DataGridCell";
            bool _printValueOnBar = false;
            // Tooltip
            string _tooltipTitle = "";
            string _tooltip = "";
            Image _tooltipImage = null;
            #endregion
            #region Constructor
            /// <summary>
            /// Create an instance of DataGridCell object.
            /// </summary>
            public DataGridCell() {
                _owner = new DataGridItem();
                _font = _owner._font;
            }
            /// <summary>
            /// Create an instance of DataGridCell object with specified DataGridItem as owner.
            /// </summary>
            public DataGridCell(DataGridItem owner) {
                _owner = owner;
                _font = _owner._font;
            }
            #endregion
            #region Public Properties
            /// <summary>
            /// Determine the name of the DataGridCell object.
            /// </summary>
            [DefaultValue("DataGridCell"), Description("Determine the name of the DataGridCell object.")]
            public string Name {
                get { return _name; }
                set { _name = value; }
            }
            /// <summary>
            /// Determine an object data associated with DataGridCell.
            /// </summary>
            [DefaultValue(""), Description("Determine an object data associated with DataGridCell."),
                TypeConverter(typeof(StringConverter))]
            public object Tag {
                get { return _tag; }
                set { _tag = value; }
            }
            /// <summary>
            /// Determine the value of the DataGridCell.
            /// </summary>
            [DefaultValue(""), TypeConverter(typeof(StringConverter)),
                Description("Determine the value of the DataGridCell.")]
            public object Value {
                get { return _value; }
                set { _value = value; }
            }
            /// <summary>
            /// Determine a Font object to draw the value of the DataGridCell.
            /// </summary>
            [Description("Determine a Font object to draw the value of the DataGridCell.")]
            public System.Drawing.Font Font {
                get { return _font; }
                set { _font = value; }
            }
            /// <summary>
            /// Determine a color used for DataGridCell background.
            /// </summary>
            [DefaultValue(typeof(System.Drawing.Color), "Transparent"),
                Description("Determine a color used for DataGridCell background.")]
            public System.Drawing.Color BackColor {
                get { return _backColor; }
                set { _backColor = value; }
            }
            /// <summary>
            /// Determine the color used to draw the value of the DataGridCell.
            /// </summary>
            [DefaultValue(typeof(System.Drawing.Color), "Black"),
                Description("Determine the color used to draw the value of the DataGridCell.")]
            public System.Drawing.Color Color {
                get { return _color; }
                set { _color = value; }
            }
            [Browsable(false)]
            public DataGridItem DataGridItem { get { return _owner; } }
            [Browsable(false)]
            public DataGrid DataGrid { get { return _owner._owner; } }
            /// <summary>
            /// Draw cell value when column format is Bar
            /// </summary>
            [DefaultValue(false), Description("Draw cell value when column format is Bar")]
            public bool PrintValueOnBar {
                get { return _printValueOnBar; }
                set {
                    if (_printValueOnBar != value) {
                        _printValueOnBar = value;
                        //_owner._owner._subitemPrintValueOnBarChanged(this);
                    }
                }
            }
            /// <summary>
            /// Gets or sets the title of the tooltip.
            /// </summary>
            [DefaultValue(""), Description("Gets or sets the title of the tooltip.")]
            public string ToolTipTitle {
                get { return _tooltipTitle; }
                set { _tooltipTitle = value; }
            }
            /// <summary>
            /// Gets or sets the content of the tooltip.
            /// </summary>
            [DefaultValue(""), EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor),
                typeof(System.Drawing.Design.UITypeEditor)), Description("Gets or sets the content of the tooltip.")]
            public string ToolTip {
                get { return _tooltip; }
                set { _tooltip = value; }
            }
            /// <summary>
            /// Gets or sets the image shown in the tooltip.
            /// </summary>
            [DefaultValue(typeof(System.Drawing.Image), "Null"), Description("Gets or sets the image shown in the tooltip.")]
            public Image ToolTipImage {
                get { return _tooltipImage; }
                set { _tooltipImage = value; }
            }
            #endregion
        }
        /// <summary>
        /// Class to represent a Collection of DataGridCell object.
        /// </summary>
        public class DataGridCellCollection : CollectionBase {
            DataGridItem _owner;
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
            /// <summary>
            /// Create an instance of DataGridCellCollection with a DataGridItem as its owner.
            /// </summary>
            [Description("Create an instance of DataGridCellCollection with a DataGridItem as its owner.")]
            public DataGridCellCollection(DataGridItem owner) : base() {
                _owner = owner;
            }
            #endregion
            #region Public Members
            /// <summary>
            /// Gets a DataGridCell object in the collection specified by its index.
            /// </summary>
            [Description("Gets a DataGridCell object in the collection specified by its index.")]
            public DataGridCell this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (DataGridCell)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets the index of a DataGridCell object in the collection.
            /// </summary>
            [Description("Gets the index of a DataGridCell object in the collection.")]
            public int IndexOf(DataGridCell cell) { return List.IndexOf(cell); }
            /// <summary>
            /// Add a DataGridCell object to the collection.
            /// </summary>
            [Description("Add a DataGridCell object to the collection.")]
            public DataGridCell Add(DataGridCell cell) {
                cell._owner = _owner;
                int index = List.Add(cell);
                return (DataGridCell)List[index];
            }
            /// <summary>
            /// Add a DataGridCell object to the collection by providing its value.
            /// </summary>
            [Description("Add a DataGridCell object to the collection by providing its value.")]
            public DataGridCell Add(object value) {
                DataGridCell aCell = new DataGridCell(_owner);
                aCell.Value = value;
                return this.Add(aCell);
            }
            /// <summary>
            /// Add a DataGridCell collection to the collection.
            /// </summary>
            [Description("Add a DataGridCell collection to the collection."),
                System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(DataGridCellCollection cells) {
                foreach (DataGridCell cell in cells) this.Add(cell);
            }
            /// <summary>
            /// Insert a DataGridCell object to the collection at specified index.
            /// </summary>
            [Description("Insert a DataGridCell object to the collection at specified index.")]
            public void Insert(int index, DataGridCell cell) {
                cell._owner = _owner;
                List.Insert(index, cell);
            }
            /// <summary>
            /// Remove a DataGridCell object from the collection.
            /// </summary>
            [Description("Remove a DataGridCell object from the collection.")]
            public void Remove(DataGridCell cell) {
                if (List.Contains(cell)) List.Remove(cell);
            }
            /// <summary>
            /// Determine whether a DataGridCell object exist in the collection.
            /// </summary>
            [Description("Determine whether a DataGridCell object exist in the collection.")]
            public bool Contains(DataGridCell cell) { return List.Contains(cell); }
            #endregion
            #region Overriden Methods
            protected override void OnValidate(object value) {
                if (!typeof(DataGridCell).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.DataGridCell", "value");
            }
            protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
            protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
            protected override void OnInsert(int index, object value) {
                /*if (_owner._owner.IsDesignMode) {
                    DataGridCell cell = (DataGridCell)value;
                    cell.Font = _owner._owner.Font;
                }*/
                if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
            }
            protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
            protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
            protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null) AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
            protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            #endregion
        }
        #endregion
        #region Members
        internal DataGrid _owner = null;
        internal System.Drawing.Font _font = null;
        string _name = "DataGridItem";
        System.Drawing.Color _color = System.Drawing.Color.Black;
        System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
        object _tag = null;
        bool _useItemStyleForSubItems = true;
        DataGridCellCollection _cells;
        #endregion
        #region Constructor
        /// <summary>
        /// Create an instance of ListViewItem.
        /// </summary>
        [Description("Create an instance of DataGridItem.")]
        public DataGridItem() {
            _owner = new DataGrid();
            _cells = new DataGridCellCollection(this);
            _font = _owner.Font;
            _cells.AfterClear += _cells_AfterClear;
            _cells.AfterInsert += _cells_AfterInsert;
            _cells.AfterRemove += _cells_AfterRemove;
            _cells.AfterSet += _cells_AfterSet;
        }
        /// <summary>
        /// Create an Instance of DataGridItem with specified DataGrid as its owner.
        /// </summary>
        [Description("Create an Instance of DataGridItem with specified DataGrid as its owner.")]
        public DataGridItem(DataGrid owner) {
            _owner = owner;
            _cells = new DataGridCellCollection(this);
            _font = _owner.Font;
            _cells.AfterClear += _cells_AfterClear;
            _cells.AfterInsert += _cells_AfterInsert;
            _cells.AfterRemove += _cells_AfterRemove;
            _cells.AfterSet += _cells_AfterSet;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Determine the font used to draw the Text of a DataGridItem.
        /// </summary>
        [Description("Determine the font used to draw the Text of a DataGridItem.")]
        public System.Drawing.Font Font {
            get {
                if (_font == null) return _owner.Font;
                else return _font;
            }
            set {
                if (_font != value) {
                    _font = value;
                    //_owner._itemFontChanged(this);
                }
            }
        }
        /// <summary>
        /// Determine a color used for DataGridItem background.
        /// </summary>
        [DefaultValue(typeof(System.Drawing.Color), "Transparent"),
            Description("Determine a color used for DataGridItem background.")]
        public System.Drawing.Color BackColor {
            get { return _backColor; }
            set {
                if (_backColor != value) {
                    _backColor = value;
                    //_owner._itemBackColorChanged(this);
                }
            }
        }
        /// <summary>
        /// Determine a color used to draw the Text of a DataGridItem.
        /// </summary>
        [DefaultValue(typeof(System.Drawing.Color), "Black"),
            Description("Determine a color used to draw the Text of a DataGridItem.")]
        public System.Drawing.Color Color {
            get { return _color; }
            set {
                if (_color != value) {
                    _color = value;
                    //_owner._itemColorChanged(this);
                }
            }
        }
        /// <summary>
        /// Determine whether item style is used for all cells.
        /// </summary>
        [DefaultValue(true),
            Description("Determine whether item style is used for all cells.")]
        public bool UseItemStyleForSubItems {
            get { return _useItemStyleForSubItems; }
            set {
                if (_useItemStyleForSubItems != value) {
                    _useItemStyleForSubItems = value;
                    //_owner._itemUseItemStyleForSubItemsChanged(this);
                }
            }
        }
        /// <summary>
        /// Determine the name of a DataGridItem object.
        /// </summary>
        [DefaultValue("DataGridItem"),
            Description("Determine the name of a DataGridItem object.")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Determine an object data associated with DataGridItem object.
        /// </summary>
        [DefaultValue(""), TypeConverter(typeof(StringConverter)),
            Description("Determine an object data associated with DataGridItem object.")]
        public object Tag {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Gets the collection of DataGridCell objects assigned to the current DataGridItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Description("Gets the collection of DataGridCell objects assigned to the current DataGridItem.")]
        public DataGridCellCollection Cells { get { return _cells; } }
        [Browsable(false)]
        public DataGrid DataGrid { get { return _owner; } }
        #endregion
        #region Event Handlers
        private void _cells_AfterClear(object sender, CollectionEventArgs e) { 
            //_owner._itemCellsChanged(this); 
        }
        private void _cells_AfterInsert(object sender, CollectionEventArgs e) { 
            //_owner._itemCellsChanged(this); 
        }
        private void _cells_AfterRemove(object sender, CollectionEventArgs e) { 
            //_owner._itemCellsChanged(this); 
        }
        private void _cells_AfterSet(object sender, CollectionEventArgs e) { 
            //_owner._itemCellsChanged(this); 
        }
        #endregion
    }
}