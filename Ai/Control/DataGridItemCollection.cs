// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Ai.Control {
    public class DataGridItemCollection : CollectionBase {
        internal DataGrid _owner = null;
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
        public DataGridItemCollection(DataGrid owner) : base() { _owner = owner; }
        #endregion
        #region Public Members
        /// <summary>
        /// Gets a DataGridItem object in the collection specified by its index.
        /// </summary>
        /// <param name="index">The index of the item in the collection to get.</param>
        /// <returns>A DataGridItem object located at the specified index within the collection.</returns>
        public DataGridItem this[int index] {
            get {
                if (index >= 0 && index < List.Count) return (DataGridItem)List[index];
                return null;
            }
        }
        /// <summary>
        /// Returns the index within the collection of the specified item.
        /// </summary>
        /// <param name="item">A DataGridItem object representing the item to locate in the collection.</param>
        /// <returns>The zero-based index where the item is located within the collection; otherwise, negative one (-1).</returns>
        public int IndexOf(DataGridItem item) { return List.IndexOf(item); }
        /// <summary>
        /// Adds a DataGridItem to the list of items for a DataGrid.
        /// </summary>
        public DataGridItem Add(DataGridItem item) {
            item._owner = _owner;
            int index = List.Add(item);
            return (DataGridItem)List[index];
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the collection.
        /// </summary>
        /// <param name="items"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void AddRange(DataGridItemCollection items) {
            foreach (DataGridItem di in items) this.Add(di);
        }
        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The DataGridItem object to insert.</param>
        public void Insert(int index, DataGridItem item) {
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
        public bool Remove(DataGridItem item) {
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
        public bool Contains(DataGridItem item) { return List.Contains(item); }
        /// <summary>
        /// Determines whether the collection contains a specific key.
        /// </summary>
        /// <param name="key">The name of the DataGridItem to locate in the collection.</param>
        /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey(string key) {
            foreach (DataGridItem di in List) {
                if (di.Name == key) return true;
            }
            return false;
        }
        public DataGridItem[] Find(string key) {
            List<DataGridItem> result = new List<DataGridItem>();
            foreach (DataGridItem di in List) {
                if (di.Name == key) result.Add(di);
            }
            return result.ToArray();
        }
        #endregion
        #region Overiden Methods
        protected override void OnValidate(object value) {
            if (!typeof(DataGridItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.DataGridItem", "value");
        }
        protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
        protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
        protected override void OnInsert(int index, object value) {
            /*if (_owner.IsDesignMode) {
                DataGridItem item = (DataGridItem)value;
                item.Font = _owner.Font;
            }*/
            if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
        }
        protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
        protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
        protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null)AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
        protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
        protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
        #endregion
    }
}