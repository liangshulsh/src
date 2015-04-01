// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Ai.Control {
    /// <summary>
    /// Represent a collection of DataGridGroup in a DataGrid.
    /// </summary>
    public sealed class DataGridGroupCollection : CollectionBase {
        internal DataGrid _owner = null;
        bool internalCalls = false;
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
        #region Internal Events
        /// <summary>
        /// Occurs when a group has been moved along the collection.
        /// </summary>
        internal event EventHandler<EventArgs> GroupMoved;
        /// <summary>
        /// Occurs when two groups has been swapped between them.
        /// </summary>
        internal event EventHandler<EventArgs> GroupSwapped;
        #endregion
        #region Constructor
        public DataGridGroupCollection(DataGrid owner) : base() { _owner = owner; }
        #endregion
        #region Public Members
        /// <summary>
        /// Gets a DataGridGroup object in the collection specified by its index.
        /// </summary>
        /// <param name="index">The index of the item in the collection to get.</param>
        /// <returns>A DataGridGroup object located at the specified index within the collection.</returns>
        public DataGridGroup this[int index] {
            get {
                if (index >= 0 && index < List.Count) return (DataGridGroup)List[index];
                return null;
            }
        }
        /// <summary>
        /// Returns the index within the collection of the specified group.
        /// </summary>
        /// <param name="item">A DataGridGroup object representing the group to locate in the collection.</param>
        /// <returns>The zero-based index where the group is located within the collection; otherwise, negative one (-1).</returns>
        public int IndexOf(DataGridGroup group) { return List.IndexOf(group); }
        /// <summary>
        /// Adds a DataGridGroup to the list of groups for a DataGrid.
        /// </summary>
        public DataGridGroup Add(DataGridGroup group) {
            foreach (DataGridGroup dg in List) {
                if (dg.Header == group.Header) return dg;
            }
            bool found = false;
            foreach (DataGridHeader dh in _owner._headers) {
                if (dh == group.Header) {
                    found = true;
                    break;
                }
            }
            if (found) {
                int index = List.Add(group);
                group._owner = _owner;
                return (DataGridGroup)List[index];
            }
            return null;
        }
        /// <summary>
        /// Adds a DataGridGroup to the list of groups specified by the available header name for a DataGrid.
        /// </summary>
        public DataGridGroup Add(string headerName) {
            foreach (DataGridGroup dg in List) {
                if (dg.Header.Name == headerName) return dg;
            }
            DataGridHeader header = null;
            foreach (DataGridHeader dh in _owner._headers) {
                if (dh.Name == headerName) {
                    header = dh;
                    break;
                }
            }
            if (header != null) {
                DataGridGroup group = new DataGridGroup(_owner, header);
                int index = List.Add(group);
                return (DataGridGroup)List[index];
            }
            return null;
        }
        /// <summary>
        /// Adds a DataGridGroup to the list of groups specified by the available header for a DataGrid.
        /// </summary>
        public DataGridGroup Add(DataGridHeader header) {
            foreach (DataGridGroup dg in List) {
                if (dg.Header == header) return dg;
            }
            bool found = false;
            foreach (DataGridHeader dh in _owner._headers) {
                if (dh == header) {
                    found = true;
                    break;
                }
            }
            if (found) {
                DataGridGroup group = new DataGridGroup(_owner, header);
                int index = List.Add(group);
                return (DataGridGroup)List[index];
            }
            return null;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the collection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void AddRange(DataGridGroupCollection groups) {
            foreach (DataGridGroup dg in groups) this.Add(dg);
        }
        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="group">The DataGridGroup object to insert.</param>
        public void Insert(int index, DataGridGroup group) {
            if (group != null) {
                // Check for existing header in collection.
                foreach (DataGridGroup dg in List) {
                    if (dg.Header == group.Header) return;
                }
                // Check for existing header in owner data grid.
                bool found = false;
                foreach (DataGridHeader dh in _owner._headers) {
                    if (group.Header == dh) {
                        found = true;
                        break;
                    }
                }
                if (found) {
                    group._owner = _owner;
                    List.Insert(index, group);
                }
            }
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="group">The object to remove from the collection.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public bool Remove(DataGridGroup group) {
            if (List.Contains(group)) {
                List.Remove(group);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="group">The object to locate in the collection.</param>
        /// <returns>true if group is found in the collection; otherwise, false</returns>
        public bool Contains(DataGridGroup group) { return List.Contains(group); }
        /// <summary>
        /// Determines whether a data grid group that contains specified data grid header is in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false</returns>
        public bool Contains(DataGridHeader header) {
            foreach (DataGridGroup dg in List) {
                if (dg.Header == header) return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether a data grid group that contains specified header name is in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false</returns>
        public bool Contains(string headerName) {
            foreach (DataGridGroup dg in List) {
                if (dg.Header != null) {
                    if (dg.Header.Name == headerName) return true;
                }
            }
            return false;
        }
        #endregion
        #region Internal Members
        /// <summary>
        /// Moves a DataGridGroup object along this collection.
        /// </summary>
        /// <param name="moveTo">The zero based index where the object will be placed.</param>
        /// <param name="group">The DataGridGroup object to be moved.</param>
        /// <returns>true, if the operation executed successfully, otherwise, false.</returns>
        internal bool moveGroup(int moveTo, DataGridGroup group) {
            bool result = false;
            internalCalls = true;
            if (Contains(group)) {
                if (moveTo >= 0 && moveTo < List.Count) {
                    int groupIdx = List.IndexOf(group);
                    if (groupIdx >= moveTo) groupIdx++;
                    List.Insert(moveTo, group);
                    List.RemoveAt(groupIdx);
                    result = true;
                    if (GroupMoved != null) GroupMoved(this, new EventArgs());
                }
            }
            internalCalls = false;
            return result;
        }
        /// <summary>
        /// Moves a DataGridGroup object to the first index in this collection.
        /// </summary>
        /// <param name="group">A DataGridGroup object to be moved.</param>
        /// <returns>true, if the operation executed successfully, otherwise, false.</returns>
        internal bool moveFirst(DataGridGroup group) {
            return moveGroup(0, group);
        }
        /// <summary>
        /// Moves a DataGridGroup object to the last position in this collection.
        /// </summary>
        /// <param name="group">A DataGridGroup object to be moved.</param>
        /// <returns>true, if the operation executed successfully, otherwise, false.</returns>
        internal bool moveLast(DataGridGroup group) {
            return moveGroup(List.Count - 1, group);
        }
        /// <summary>
        /// Swaps the position between 2 groups in this collection.
        /// </summary>
        /// <param name="group1">The first DataGridGroup object to be swapped.</param>
        /// <param name="group2">The second DataGridGroup object to be swapped.</param>
        /// <returns>true, if the operation executed successfully, otherwise, false.</returns>
        internal bool swapGroup(DataGridGroup group1, DataGridGroup group2) {
            bool result = false;
            internalCalls = true;
            if (Contains(group1) && Contains(group2)) {
                result = true;
                int idx1 = List.IndexOf(group1);
                int idx2 = List.IndexOf(group2);
                List[idx1] = group2;
                List[idx2] = group1;
                if (GroupSwapped != null) GroupSwapped(this, new EventArgs());
            }
            internalCalls = false;
            return result;
        }
        #endregion
        #region Overiden Methods
        protected override void OnValidate(object value) {
            if (!typeof(DataGridGroup).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.DataGridGroup", "value");
        }
        protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
        protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
        protected override void OnInsert(int index, object value) {
            if (Inserting != null && !internalCalls) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
        }
        protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null && !internalCalls) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
        protected override void OnRemove(int index, object value) { if (Removing != null && !internalCalls ) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
        protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null && !internalCalls)AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
        protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null && !internalCalls) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
        protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null && !internalCalls) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
        #endregion
    }
}