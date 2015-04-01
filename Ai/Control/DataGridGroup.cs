// Ai Software Control Library.
// Created by : Burhanudin Ashari (red_moon@CodeProject) @ December 10, 2011.
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Represent a group of DataGridItem objects in DataGrid.
    /// </summary>
    public class DataGridGroup {
        /// <summary>
        /// Specifies how the summarize of a group will be calculated.
        /// </summary>
        public enum GroupSummarizeMode { 
            /// <summary>
            /// No summarize.
            /// </summary>
            None,
            /// <summary>
            /// The number of elements contained in a group.
            /// </summary>
            Count,
            /// <summary>
            /// The sum of elements contained in a group.
            /// </summary>
            Sum,
            /// <summary>
            /// The average of elements contained in a group.
            /// </summary>
            Average,
            /// <summary>
            /// Use text as summarize.
            /// </summary>
            Text,
            /// <summary>
            /// Use custom calculating for the group.
            /// </summary>
            Custom
        }
        #region Public Classes
        /// <summary>
        /// Represent the summarize of elements contained within group in a column.
        /// </summary>
        public class DataGridGroupSummarize {
            GroupSummarizeMode _mode = GroupSummarizeMode.None;
            bool _customPaint = false;
            string _text = "";
            internal DataGridGroup _owner = null;
            public DataGridGroupSummarize() { _owner = new DataGridGroup(); }
            public DataGridGroupSummarize(DataGridGroup owner) { _owner = owner; }
            /// <summary>
            /// Gets of sets the text displayed as summarize of the group if the mode sets to GroupSummarizeMode.Text.
            /// </summary>
            [DefaultValue(""), Description("Gets of sets the text displayed as summarize of the group if the mode sets to GroupSummarizeMode.Text.")]
            public string Text {
                get { return _text; }
                set {
                    if (_text != value) {
                        _text = value;
                        if (_mode == GroupSummarizeMode.Text) { 
                            // TO DO : refresh data grid appearance response to this changes.
                        }
                    }
                }
            }
            /// <summary>
            /// Gets or sets GroupSummarizeMode to calculate the summarize of a group.
            /// </summary>
            [DefaultValue(typeof(GroupSummarizeMode), "None"), Description("Gets or sets GroupSummarizeMode to calculate the summarize of a group.")]
            public GroupSummarizeMode Mode {
                get { return _mode; }
                set {
                    if (_mode != value) {
                        _mode = value;
                        // TO DO : refresh data grid responding to this changes.
                    }
                }
            }
            /// <summary>
            /// Gets or sets a value indicating the summarize of a group should be drawn by your code.
            /// </summary>
            [DefaultValue(false), Description("Gets or sets a value indicating the summarize of a group should be drawn by your code.")]
            public bool UseCustomPaint {
                get { return _customPaint; }
                set {
                    if (_customPaint != value) {
                        _customPaint = value;
                        // TO DO : refresh data grid responding to this changes.
                    }
                }
            }
        }
        /// <summary>
        /// Represent a collection of DataGridGroupSummarize in a DataGridGroup.
        /// </summary>
        public class DataGridGroupSummarizeCollection : CollectionBase {
            internal DataGridGroup _owner = null;
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
            public DataGridGroupSummarizeCollection(DataGridGroup owner) : base() { _owner = owner; }
            #endregion
            #region Public Members
            /// <summary>
            /// Gets a DataGridGroupSummarize object in the collection specified by its index.
            /// </summary>
            /// <param name="index">The index of the item in the collection to get.</param>
            /// <returns>A DataGridGroupSummarize object located at the specified index within the collection.</returns>
            public DataGridGroupSummarize this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (DataGridGroupSummarize)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Returns the index within the collection of the specified item.
            /// </summary>
            /// <param name="item">A DataGridGroupSummarize object representing the item to locate in the collection.</param>
            /// <returns>The zero-based index where the item is located within the collection; otherwise, negative one (-1).</returns>
            public int IndexOf(DataGridGroupSummarize item) { return List.IndexOf(item); }
            /// <summary>
            /// Adds a DataGridGroupSummarize to the list of items for a DataGrid.
            /// </summary>
            public DataGridGroupSummarize Add(DataGridGroupSummarize item) {
                item._owner = _owner;
                int index = List.Add(item);
                return (DataGridGroupSummarize)List[index];
            }
            /// <summary>
            /// Adds the elements of the specified collection to the end of the collection.
            /// </summary>
            /// <param name="items"></param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(DataGridGroupSummarizeCollection items) {
                foreach (DataGridGroupSummarize di in items) this.Add(di);
            }
            /// <summary>
            /// Inserts an element into the collection at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which item should be inserted.</param>
            /// <param name="item">The DataGridGroupSummarize object to insert.</param>
            public void Insert(int index, DataGridGroupSummarize item) {
                if (item != null) {
                    item._owner = _owner;
                    List.Insert(index, item);
                }
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the collection.
            /// </summary>
            /// <param name="item">The object to remove from the collection.</param>
            /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
            public bool Remove(DataGridGroupSummarize item) {
                if (List.Contains(item)) {
                    List.Remove(item);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Determines whether an element is in the collection.
            /// </summary>
            /// <param name="item">The object to locate in the collection.</param>
            /// <returns>true if item is found in the collection; otherwise, false</returns>
            public bool Contains(DataGridGroupSummarize item) { return List.Contains(item); }
            #endregion
            #region Overiden Methods
            protected override void OnValidate(object value) {
                if (!typeof(DataGridGroupSummarize).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must DataGridGroupSummarize", "value");
            }
            protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
            protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
            protected override void OnInsert(int index, object value) { if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value)); }
            protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
            protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
            protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null)AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
            protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            #endregion
        }
        #endregion
        #region Fields
        internal DataGrid _owner = null;
        bool _isExpanded = true;
        DataGridHeader _header = null;
        DataGridGroupSummarizeCollection _summarizes;
        #endregion
        #region Constructor
        public DataGridGroup() {
            _owner = new DataGrid();
            _summarizes = new DataGridGroupSummarizeCollection(this);
        }
        public DataGridGroup(DataGrid owner) {
            _owner = owner;
            _summarizes = new DataGridGroupSummarizeCollection(this);
        }
        public DataGridGroup(DataGrid owner, DataGridHeader header) {
            _owner = owner;
            _header = header;
            _summarizes = new DataGridGroupSummarizeCollection(this);
        }
        #endregion
        #region Public Members
        /// <summary>
        /// Gets a value indicating that the DataGridGroup currently expanded.
        /// </summary>
        [Browsable(false), Description("Gets a value indicating that the DataGridGroup currently expanded.")]
        public bool IsExpanded {
            get { return _isExpanded; }
        }
        /// <summary>
        /// Gets a DataGrid object that owns the DataGridGroup.
        /// </summary>
        [Browsable(false), Description("Gets a DataGrid object that owns the DataGridGroup.")]
        public DataGrid Owner {
            get { return _owner; }
        }
        /// <summary>
        /// Gets a DataGridHeader object that corresponds with the group.
        /// </summary>
        [Browsable(false), Description("Gets or Sets a DataGridHeader object that corresponds with the group.")]
        public DataGridHeader Header {
            get { return _header; }
            set {
                if (value != _header) {
                    if (value != null) {
                        // The owner of both header and this group must be same.
                        if (_owner == value._owner) {
                            _header = value;
                            // Tells the owner if the header used in this group has been changed.
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets a DataGridGroupSummarizeCollection object represent the collection of DataGridGroupSummarize.
        /// </summary>
        [Browsable(false), Description("Gets a DataGridGroupSummarizeCollection object represent the collection of DataGridGroupSummarize.")]
        public DataGridGroupSummarizeCollection Summarizes { get { return _summarizes; } }
        /// <summary>
        /// Expands the datagrid group.
        /// </summary>
        public void Expand() { 
            // TO DO : Command the datagrid to visually expand this group.
        }
        /// <summary>
        /// Collapses the datagrid group.
        /// </summary>
        public void Collapse() { 
            // TO DO : Command the datagrid to visually collapse this group.
        }
        #endregion
    }
}