// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Ai.Control {
	public sealed class TreeNodeCollection : CollectionBase {
		internal TreeNode _parent;
		internal MultiColumnTree _owner;
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
		public TreeNodeCollection(TreeNode parent, MultiColumnTree owner) : base() {
			_owner = owner;
			_parent = parent;
		}
		public TreeNode this[int index] {
			get {
				if (index < 0 || index >= List.Count) return null;
				else return (TreeNode)List[index];
			}
		}
		public int IndexOf(TreeNode node) { return List.IndexOf(node); }
		public TreeNode Add(TreeNode node) {
			node._parent = _parent;
			node._owner = _owner;
			node.setParentNOwner();
			int index = List.Add(node);
			return (TreeNode)List[index];
		}
		public TreeNode Add(string text) {
			TreeNode node = new TreeNode(text);
			return this.Add(node);
		}
		public TreeNode Add(string text, TreeNodeCollection nodes) {
			TreeNode node = new TreeNode(text, nodes);
			return this.Add(node);
		}
		public TreeNode Add(string text, Image img, Image expImg) {
			TreeNode node = new TreeNode(text, img, expImg);
			return this.Add(node);
		}
		public TreeNode Add(string text, Image img, Image expImg, TreeNodeCollection nodes) {
			TreeNode node = new TreeNode(text, img, expImg, nodes);
			return this.Add(node);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public void AddRange(TreeNodeCollection nodes) {
			foreach (TreeNode tn in nodes) this.Add(tn);
		}
		public void Insert(int index, TreeNode node) {
			node._parent = _parent;
			node._owner = _owner;
			node.setParentNOwner();
			List.Insert(index, node);
		}
		public void Remove(TreeNode node) {
			if (List.Contains(node)) List.Remove(node);
		}
		public bool Contains(TreeNode node) { return List.Contains(node); }
		public bool ContainsKey(string key) {
			foreach (TreeNode tn in List) {
				if (tn.Name == key) return true;
			}
			return false;
		}
		public TreeNode[] Find(string key, bool searchAllChildren) {
			List<TreeNode> result = new List<TreeNode>();
			foreach (TreeNode tn in List) {
				if (tn.Name == key) result.Add(tn);
				if (searchAllChildren) {
					if (tn.Nodes.Count > 0) result.AddRange(tn.Nodes.Find(key, searchAllChildren));
				}
			}
			return result.ToArray();
		}
		protected override void OnValidate(object value) {
			if (!typeof(TreeNode).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.TreeNode", "value");
		}
        protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
        protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
		protected override void OnInsert(int index, object value) {
			if (_owner.IsDesignMode) {
				TreeNode node = (TreeNode)value;
				node.NodeFont = _owner.Font;
			}
            if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
		}
        protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
        protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
        protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null)AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
        protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
        protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
	}
}