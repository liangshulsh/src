// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
	/// <summary>
	/// Class to represent a group of ListViewItem in the ListView.
	/// </summary>
	/// <remarks>
	/// Each ListViewGroup can be collapsed or expanded, through UI or programmatically.
	/// Each ListViewGroup have a checkbox, if CheckBoxes property of ListView is set to true, to Check or Uncheck all items inside the group.
	/// ListViewGroup always be shown on each view mode.
	/// The properties and methods for default group, can be accessed through ListView object.
	/// </remarks>
	public class ListViewGroup {
		#region Declarations
		ListView.ListViewItemCollection _items;
		internal ListView.CheckedListViewItemCollection _checkedItems;
		string _name = "ListViewGroup";
		string _text = "Group";
		bool _cancelStatus = false;
		HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		bool _isCollapsed = false;
		object _tag = null;
		bool _checked = false;
		bool _innerThread = false;
		internal ListView _owner;
		internal System.Windows.Forms.CheckState _checkState = System.Windows.Forms.CheckState.Unchecked;
		#endregion
		#region Public Events
		/// <summary>
		/// Occurs before the group is collapsed.
		/// </summary>
		public event EventHandler<GroupEventArgs> BeforeCollapse;
		/// <summary>
		/// Occurs after the group is collapsed.
		/// </summary>
		public event EventHandler<GroupEventArgs> AfterCollapse;
		/// <summary>
		/// Occurs before the group is expanded.
		/// </summary>
		public event EventHandler<GroupEventArgs> BeforeExpand;
		/// <summary>
		/// Occurs after the group is expanded.
		/// </summary>
		public event EventHandler<GroupEventArgs> AfterExpand;
		/// <summary>
		/// Occurs before checked state of a ListViewGroup is changed.
		/// </summary>
		public event EventHandler<GroupEventArgs> BeforeCheck;
		/// <summary>
		/// Occurs after checked state of a ListViewGroup is changed.
		/// </summary>
		public event EventHandler<GroupEventArgs> AfterCheck;
		#endregion
		#region Constructor
		/// <summary>
		/// Create an instance of ListViewGroup.
		/// </summary>
		[Description("Create an instance of ListViewGroup.")]
		public ListViewGroup() {
			_owner = new ListView();
			_items = new ListView.ListViewItemCollection(_owner);
			_checkedItems = new ListView.CheckedListViewItemCollection(_owner);
		}
		/// <summary>
		/// Create an instance of ListViewGroup with specified a ListView as its owner.
		/// </summary>
		[Description("Create an instance of ListViewGroup with specified a ListView as its owner.")]
		public ListViewGroup(ListView owner) {
			_owner = owner;
			_items = new ListView.ListViewItemCollection(_owner);
			_checkedItems = new ListView.CheckedListViewItemCollection(_owner);
		}
		#endregion
		#region Public Properties
		/// <summary>
		/// Determine the text displayed in a ListViewGroup.
		/// </summary>
		[DefaultValue("Group"), 
			Description("Determine the text displayed in a ListViewGroup.")]
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;
					_owner._groupTextChanged(this);
				}
			}
		}
		/// <summary>
		/// Specifies how ListViewGroup text will be aligned.
		/// </summary>
		[DefaultValue(typeof(HorizontalAlignment), "Left"), 
			Description("Specifies how ListViewGroup text will be aligned.")]
		public HorizontalAlignment TextAlign {
			get { return _textAlign; }
			set {
				if (_textAlign != value) {
					_textAlign = value;
					_owner._groupTextAlignChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine whether the ListViewGroup and all items is checked.
		/// </summary>
		[DefaultValue(false), 
			Description("Determine whether the ListViewGroup and all items is checked.")]
		public bool Checked {
			get { return _checked; }
			set {
				if (_checked != value) {
					_cancelStatus = false;
					GroupEventArgs beforeCheck = new GroupEventArgs(this);
                    if (BeforeCheck != null) BeforeCheck(this, beforeCheck);
					_cancelStatus = beforeCheck.Cancel;
					if (beforeCheck.Cancel) return;
					_checked = value;
					if (_checked) _checkState = System.Windows.Forms.CheckState.Checked;
					else _checkState = System.Windows.Forms.CheckState.Unchecked;
					_owner._groupCheckedChanged(this);
                    if (AfterCheck != null) AfterCheck(this, new GroupEventArgs(this));
				}
			}
		}
		/// <summary>
		/// Gets a value indicating the CheckBox's state of the ListViewGroup.
		/// </summary>
		[DefaultValue(typeof(System.Windows.Forms.CheckState), "Unchecked"), 
			Description("Gets a value indicating the CheckBox's state of the ListViewGroup."), Browsable(false)]
		public System.Windows.Forms.CheckState CheckState { get { return _checkState; } }
		/// <summary>
		/// Determine an object data associated with ListViewGroup object.
		/// </summary>
		[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
			Description("Determine an object data associated with ListViewGroup object.")]
		public object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
		[DefaultValue("ListViewGroup")]
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		/// <summary>
		/// Gets the collection of Checked ListViewItem objects assigned to the current ListViewGroup.
		/// </summary>
		[Browsable(false), 
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
			Description("Gets the collection of Checked ListViewItem objects assigned to the current ListViewGroup.")]
		public ListView.CheckedListViewItemCollection CheckedItems { get { return _checkedItems; } }
		/// <summary>
		/// Gets the collection of ListViewItem objects assigned to the current ListViewGroup.
		/// </summary>
		[Browsable(false), 
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
			Description("Gets the collection of ListViewItem objects assigned to the current ListViewGroup.")]
		public ListView.ListViewItemCollection Items { get { return _items; } }
		[Browsable(false)]
		public bool IsCollapsed { get { return _isCollapsed; } }
		[Browsable(false)]
		public ListView ListView { get { return _owner; } }
		#endregion
		#region Public Methods
		/// <summary>
		/// Forces the ListViewGroup to hide its items.
		/// </summary>
		[Description("Forces the ListViewGroup to hide its items.")]
		public void collapse() {
			if (!_isCollapsed) {
				_cancelStatus = false;
				GroupEventArgs beforeCollapse = new GroupEventArgs(this);
                if (BeforeCollapse != null) BeforeCollapse(this, beforeCollapse);
				_cancelStatus = beforeCollapse.Cancel;
				if (beforeCollapse.Cancel) return;
				_isCollapsed = true;
				_owner._groupCollapseChanged(this);
                if (AfterCollapse != null) AfterCollapse(this, new GroupEventArgs(this));
			}
		}
		/// <summary>
		/// Forces the ListViewGroup to show its items.
		/// </summary>
		[Description("Forces the ListViewGroup to show its items.")]
		public void expand() {
			if (_isCollapsed) {
				_cancelStatus = false;
				GroupEventArgs beforeExpand = new GroupEventArgs(this);
                if (BeforeExpand != null) BeforeExpand(this, beforeExpand);
				_cancelStatus = beforeExpand.Cancel;
				if (beforeExpand.Cancel) return;
				_isCollapsed = false;
				_owner._groupCollapseChanged(this);
                if (AfterExpand != null) AfterExpand(this, new GroupEventArgs(this));
			}
		}
		#endregion
		#region Internal Methods
		/// <summary>
		/// Sets the owner of the ListViewGroup and all of the ListViewItem in the collection.
		/// </summary>
		[Description("Sets the owner of the ListViewGroup and all of the ListViewItem in the collection.")]
		internal void setOwner(ListView owner) {
			_owner = owner;
			_items._owner = owner;
			_checkedItems._owner = owner;
			foreach (ListViewItem lvi in _items) lvi._owner = owner;
		}
		internal bool cancelStatus() { return _cancelStatus; }
		#endregion
		#region Event Handlers
		private void _items_AfterInsert(object sender, CollectionEventArgs e) {
			ListViewItem anItem = (ListViewItem)e.Item;
			anItem.Group = this;
			anItem._owner = _owner;
			if (anItem.Checked) _checkedItems.Add(anItem);
		}
		private void _items_AfterRemove(object sender, CollectionEventArgs e) {
			// The item's group must be set to nothing.
			ListViewItem anItem = (ListViewItem)e.Item;
			anItem.Group = null;
			_innerThread = true;
			_checkedItems.Remove(anItem);
			_innerThread = false;
		}
		private void _items_Clearing(object sender, CollectionEventArgs e) {
			// All the item's group must be set to nothing before it cleared.
			foreach (ListViewItem anItem in _items) anItem.Group = null;
			_innerThread = true;
			_checkedItems.Clear();
			_innerThread = false;
		}
		private void _checkedItems_AfterRemove(object sender, CollectionEventArgs e) {
			if (_innerThread) return;
			ListViewItem anItem = (ListViewItem)e.Item;
			anItem.Checked = false;
			_owner.CheckedItems.Remove((ListViewItem)e.Item);
		}
		private void _checkedItems_Clearing(object sender, CollectionEventArgs e) {
			if (_innerThread) return;
			foreach (ListViewItem lvi in _checkedItems) {
				lvi.Checked = false;
				_owner.CheckedItems.Remove(lvi);
			}
		}
		#endregion
	}
}