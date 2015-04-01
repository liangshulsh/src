// Ai Software Control Library.
using System;
using System.ComponentModel;
using System.Collections;

namespace Ai.Control {
	/// <summary>
	/// Class to represent a collection of ListViewGroup object.
	/// </summary>
	public class ListViewGroupCollection : CollectionBase {
		internal ListView _owner;
		public ListViewGroupCollection(ListView owner) : base() { _owner = owner; }
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
		/// Gets a ListViewGroup object in the collection specified by its index.
		/// </summary>
		[Description("Gets a ListViewGroup object in the collection specified by its index.")]
		public ListViewGroup this[int index] {
			get {
				if (index >= 0 && index < List.Count) return (ListViewGroup)List[index];
				return null;
			}
		}
		/// <summary>
		/// Gets the index of a ListViewGroup object in the collection.
		/// </summary>
		[Description("Gets the index of a ListViewGroup object in the collection.")]
		public int IndexOf(ListViewGroup group) { return List.IndexOf(group); }
        /// <summary>
        /// Add a ListViewGroup object to the collection.
        /// </summary>
        [Description("Add a ListViewGroup object to the collection.")]
        public ListViewGroup Add(ListViewGroup group) {
            if (!List.Contains(group)) {
                group.setOwner(_owner);
                int index = List.Add(group);
                return (ListViewGroup)List[index];
            }
            return group;
        }
        /// <summary>
        /// Add a ListViewGroup object to the collection by providing its text.
        /// </summary>
        [Description("Add a ListViewGroup object to the collection by providing its text.")]
        public ListViewGroup Add(string text) {
            ListViewGroup newGroup = new ListViewGroup(_owner);
            newGroup.Text = text;
            return this.Add(newGroup);
        }
        /// <summary>
        /// Add a ListViewGroup collection to the collection.
        /// </summary>
        [Description("Add a ListViewGroup collection to the collection."),
            System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void AddRange(ListViewGroupCollection groups) {
            foreach (ListViewGroup grp in groups) this.Add(grp);
        }
        /// <summary>
        /// Insert a ListViewGroup object to the collection at specified index.
        /// </summary>
        [Description("Insert a ListViewGroup object to the collection at specified index.")]
        public void Insert(int index, ListViewGroup group) {
            group.setOwner(_owner);
            List.Insert(index, group);
        }
        /// <summary>
        /// Remove a ListViewGroup object from the collection.
        /// </summary>
        [Description("Remove a ListViewGroup object from the collection.")]
        public void Remove(ListViewGroup group) {
            if (List.Contains(group)) List.Remove(group);
        }
        /// <summary>
        /// Determine whether a ListViewGroup object exist in the collection.
        /// </summary>
        /// <returns></returns>
        [Description("Determine whether a ListViewGroup object exist in the collection.")]
        public bool Contains(ListViewGroup group) { return List.Contains(group); }
        private bool containsName(string name) {
            foreach (ListViewGroup grp in List) {
                if (string.Compare(grp.Name, name, true) == 0) return true;
            }
            return false;
        }
        protected override void OnValidate(object value) {
            if (!typeof(ListViewGroup).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.ListViewGroup", "value");
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
	}
}