using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
// Created by : Burhanudin Ashari (red_moon@CodeProject) @ August 04 - August 31, 2010.
// The hardest part on programming is when your code doesn't work as your expectations.
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
/// <summary>
/// Represent a control to displays a collection of items that can be displayed using one of five different views.
/// </summary>
/// <remarks>
/// This control allows you to display a list of items with item text, and optionally, additional information of an item (subitem), and images displayed in different views.
/// There is an additional view mode, Preview, it's like Filmstrip in XP File Explorer.
/// </remarks>
[DefaultProperty("Items"), DefaultEvent("SelectedIndexChanged")]
public class ListView : Windows.Forms.Control
{
	#region "Public Events."
	/// <summary>
	/// Occurs when selected item of a ListViewChanged.
	/// </summary>
	public event SelectedIndexChangedEventHandler SelectedIndexChanged;
	public delegate void SelectedIndexChangedEventHandler(object sender, EventArgs e);
	/// <summary>
	/// Occurs when column header has been reordered.
	/// </summary>
	public event ColumnOrderChangedEventHandler ColumnOrderChanged;
	public delegate void ColumnOrderChangedEventHandler(object sender, ColumnEventArgs e);
	/// <summary>
	/// Occurs when filter parameter of a column has been changed.
	/// </summary>
	public event ColumnFilterChangedEventHandler ColumnFilterChanged;
	public delegate void ColumnFilterChangedEventHandler(object sender, ColumnEventArgs e);
	/// <summary>
	/// Occurs when custom filter of a column is choosen.
	/// </summary>
	public event ColumnCustomFilterEventHandler ColumnCustomFilter;
	public delegate void ColumnCustomFilterEventHandler(object sender, ColumnCustomFilterEventArgs e);
	/// <summary>
	/// Occurs when the width of a column has been changed.
	/// </summary>
	public event ColumnSizeChangedEventHandler ColumnSizeChanged;
	public delegate void ColumnSizeChangedEventHandler(object sender, ColumnEventArgs e);
	/// <summary>
	/// Occurs when background of a column in ListView need to paint.
	/// </summary>
	public event ColumnBackgroundPaintEventHandler ColumnBackgroundPaint;
	public delegate void ColumnBackgroundPaintEventHandler(object sender, ColumnBackgroundPaintEventArgs e);
	/// <summary>
	/// Occurs when view mode of the ListView has been changed.
	/// </summary>
	public event ViewChangedEventHandler ViewChanged;
	public delegate void ViewChangedEventHandler(object sender, EventArgs e);
	/// <summary>
	/// Occurs when a ListViewItem object has been added to ListView.
	/// </summary>
	public event ItemAddedEventHandler ItemAdded;
	public delegate void ItemAddedEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs when a ListViewItem object has been removed from ListView.
	/// </summary>
	public event ItemRemovedEventHandler ItemRemoved;
	public delegate void ItemRemovedEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs when all of ListViewItem object in ListView has been cleared.
	/// </summary>
	public event ItemsClearEventHandler ItemsClear;
	public delegate void ItemsClearEventHandler(object sender, EventArgs e);
	/// <summary>
	/// Occurs when mouse pointer is hover on a ListViewItem object.
	/// </summary>
	public event ItemHoverEventHandler ItemHover;
	public delegate void ItemHoverEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs before checked state of a ListViewItem changed.
	/// </summary>
	public event ItemBeforeCheckEventHandler ItemBeforeCheck;
	public delegate void ItemBeforeCheckEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs after checked state of a ListViewItem changed.
	/// </summary>
	public event ItemAfterCheckEventHandler ItemAfterCheck;
	public delegate void ItemAfterCheckEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs when the user starts editing the label of an item.
	/// </summary>
	public event BeforeLabelEditEventHandler BeforeLabelEdit;
	public delegate void BeforeLabelEditEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs when the label for an item is edited by the user.
	/// </summary>
	public event AfterLabelEditEventHandler AfterLabelEdit;
	public delegate void AfterLabelEditEventHandler(object sender, ItemEventArgs e);
	/// <summary>
	/// Occurs before the default group is collapsed.
	/// </summary>
	public event DefaultGroupBeforeCollapseEventHandler DefaultGroupBeforeCollapse;
	public delegate void DefaultGroupBeforeCollapseEventHandler(object sender, GroupEventArgs e);
	/// <summary>
	/// Occurs after the default group is collapsed.
	/// </summary>
	public event DefaultGroupAfterCollapeEventHandler DefaultGroupAfterCollape;
	public delegate void DefaultGroupAfterCollapeEventHandler(object sender, GroupEventArgs e);
	/// <summary>
	/// Occurs before the default group is expanded.
	/// </summary>
	public event DefaultGroupBeforeExpandEventHandler DefaultGroupBeforeExpand;
	public delegate void DefaultGroupBeforeExpandEventHandler(object sender, GroupEventArgs e);
	/// <summary>
	/// Occurs after the default group is expanded.
	/// </summary>
	public event DefaultGroupAfterExpandEventHandler DefaultGroupAfterExpand;
	public delegate void DefaultGroupAfterExpandEventHandler(object sender, GroupEventArgs e);
	/// <summary>
	/// Occurs before checked state of the default group is changed.
	/// </summary>
	public event DefaultGroupBeforeCheckEventHandler DefaultGroupBeforeCheck;
	public delegate void DefaultGroupBeforeCheckEventHandler(object sender, GroupEventArgs e);
	/// <summary>
	/// Occurs after checked state of the default group is changed.
	/// </summary>
	public event DefaultGroupAfterCheckEventHandler DefaultGroupAfterCheck;
	public delegate void DefaultGroupAfterCheckEventHandler(object sender, GroupEventArgs e);
	#endregion
	#region "Public Classes."
	/// <summary>
	/// Represent a collection of ListViewItem objects in a ListView.
	/// </summary>
	[Description("Represent a collection of ListViewItem object in a ListView.")]
	public class ListViewItemCollection : CollectionBase
	{
			#region "Constructor"
		internal ListView _owner;
		public ListViewItemCollection(ListView owner) : base()
		{
			_owner = owner;
		}
		#endregion
		#region "Public Events"
		public event ClearingEventHandler Clearing;
		public delegate void ClearingEventHandler(object sender, CollectionEventArgs e);
		public event AfterClearEventHandler AfterClear;
		public delegate void AfterClearEventHandler(object sender, CollectionEventArgs e);
		public event InsertingEventHandler Inserting;
		public delegate void InsertingEventHandler(object sender, CollectionEventArgs e);
		public event AfterInsertEventHandler AfterInsert;
		public delegate void AfterInsertEventHandler(object sender, CollectionEventArgs e);
		public event RemovingEventHandler Removing;
		public delegate void RemovingEventHandler(object sender, CollectionEventArgs e);
		public event AfterRemoveEventHandler AfterRemove;
		public delegate void AfterRemoveEventHandler(object sender, CollectionEventArgs e);
		public event SettingEventHandler Setting;
		public delegate void SettingEventHandler(object sender, CollectionEventArgs e);
		public event AfterSetEventHandler AfterSet;
		public delegate void AfterSetEventHandler(object sender, CollectionEventArgs e);
		#endregion
		#region "Public Properties"
		[Description("Gets a ListViewItem object in the collection specified by its index.")]
		public ListViewItem this[int index] {
			get {
				if (index >= 0 & index < List.Count)
					return (ListViewItem)List[index];
				return null;
			}
		}
		[Description("Gets the index of a ListViewItem object in the collection.")]
		public int IndexOf {
			get { return this.List.IndexOf(item); }
		}
		#endregion
		#region "Public Methods"
		[Description("Add a ListViewItem object to the collection.")]
		public ListViewItem Add(ListViewItem item)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(item)) {
				item._owner = _owner;
				int index = List.Add(item);
				return (ListViewItem)List[index];
			}
			return item;
		}
		[Description("Add a ListViewItem object to the collection by providing its text.")]
		public ListViewItem Add(string text)
		{
			ListViewItem anItem = new ListViewItem(_owner);
			anItem.Text = text;
			return this.Add(anItem);
		}
		[Description("Add a ListViewItem collection to the collection.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public void AddRange(ListViewItemCollection items)
		{
			foreach (ListViewItem item in items) {
				this.Add(item);
			}
		}
		[Description("Insert a ListViewItem object to the collection at specified index.")]
		public void Insert(int index, ListViewItem item)
		{
			item._owner = _owner;
			List.Insert(index, item);
		}
		[Description("Remove a ListViewItem object from the collection.")]
		public void Remove(ListViewItem item)
		{
			if (List.Contains(item))
				List.Remove(item);
		}
		[Description("Determine whether a ListViewItem object exist in the collection.")]
		public bool Contains(ListViewItem item)
		{
			return List.Contains(item);
		}
		#endregion
		#region "Private Methods"
		private bool containsName(string name)
		{
			foreach (ListViewItem lvi in List) {
				if (string.Compare(lvi.Name, name, true) == 0)
					return true;
			}
			return false;
		}
		#endregion
		#region "Protected Overriden Methods"
		[Description("Performs additional custom processes when validating a value.")]
		protected override void OnValidate(object value)
		{
			if (!typeof(ListViewItem).IsAssignableFrom(value.GetType())) {
				throw new ArgumentException("Value must ListViewItem", "value");
			}
		}
		protected override void OnClear()
		{
			if (Clearing != null) {
				Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
			}
		}
		protected override void OnClearComplete()
		{
			if (AfterClear != null) {
				AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
			}
		}
		protected override void OnInsert(int index, object value)
		{
			if (_owner.DesignMode) {
				ListViewItem item = (ListViewItem)value;
				//Dim i As Integer = 0
				//Dim find As Boolean = False
				//While i < List.Count And Not find
				//    find = containsName("ListViewItem" & CStr(i))
				//    If Not find Then i += 1
				//End While
				//item.Name = "ListViewItem" & CStr(i)
				item.Font = _owner.Font;
			}
			if (Inserting != null) {
				Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
			}
		}
		protected override void OnInsertComplete(int index, object value)
		{
			if (AfterInsert != null) {
				AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
			}
		}
		protected override void OnRemove(int index, object value)
		{
			if (Removing != null) {
				Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
			}
		}
		protected override void OnRemoveComplete(int index, object value)
		{
			if (AfterRemove != null) {
				AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
			}
		}
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (Setting != null) {
				Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (AfterSet != null) {
				AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		#endregion
	}
	/// <summary>
	/// Represent a collection of frozen ListViewItem objects in a ListView.
	/// </summary>
	public class FrozenListViewItemCollection : CollectionBase
	{
			#region "Constructor"
		internal ListView _owner;
		public FrozenListViewItemCollection(ListView owner) : base()
		{
			_owner = owner;
		}
		#endregion
		#region "Public Events"
		public event ClearingEventHandler Clearing;
		public delegate void ClearingEventHandler(object sender, CollectionEventArgs e);
		public event AfterClearEventHandler AfterClear;
		public delegate void AfterClearEventHandler(object sender, CollectionEventArgs e);
		public event InsertingEventHandler Inserting;
		public delegate void InsertingEventHandler(object sender, CollectionEventArgs e);
		public event AfterInsertEventHandler AfterInsert;
		public delegate void AfterInsertEventHandler(object sender, CollectionEventArgs e);
		public event RemovingEventHandler Removing;
		public delegate void RemovingEventHandler(object sender, CollectionEventArgs e);
		public event AfterRemoveEventHandler AfterRemove;
		public delegate void AfterRemoveEventHandler(object sender, CollectionEventArgs e);
		public event SettingEventHandler Setting;
		public delegate void SettingEventHandler(object sender, CollectionEventArgs e);
		public event AfterSetEventHandler AfterSet;
		public delegate void AfterSetEventHandler(object sender, CollectionEventArgs e);
		#endregion
		#region "Public Properties"
		[Description("Gets a ListViewItem object in the collection specified by its index.")]
		public ListViewItem this[int index] {
			get {
				if (index >= 0 & index < List.Count)
					return (ListViewItem)List[index];
				return null;
			}
		}
		[Description("Gets the index of a ListViewItem object in the collection.")]
		public int IndexOf {
			get { return this.List.IndexOf(item); }
		}
		#endregion
		#region "Public Methods"
		[Description("Add a ListViewItem object to the collection.")]
		public ListViewItem Add(ListViewItem item)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(item)) {
				// Only adding the existing item in ListView.
				if (_owner._items.Contains(item)) {
					int index = List.Add(item);
					return (ListViewItem)List[index];
				}
			}
			return item;
		}
		[Description("Add a ListViewItem collection to the collection.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public void AddRange(ListViewItemCollection items)
		{
			foreach (ListViewItem item in items) {
				this.Add(item);
			}
		}
		[Description("Insert a ListViewItem object to the collection at specified index.")]
		public void Insert(int index, ListViewItem item)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(item)) {
				// Only adding the existing item in ListView.
				if (_owner._items.Contains(item)) {
					List.Insert(index, item);
				}
			}
		}
		[Description("Remove a ListViewItem object from the collection.")]
		public void Remove(ListViewItem item)
		{
			if (List.Contains(item))
				List.Remove(item);
		}
		[Description("Determine whether a ListViewItem object exist in the collection.")]
		public bool Contains(ListViewItem item)
		{
			return List.Contains(item);
		}
		#endregion
		#region "Protected Overriden Methods"
		[Description("Performs additional custom processes when validating a value.")]
		protected override void OnValidate(object value)
		{
			if (!typeof(ListViewItem).IsAssignableFrom(value.GetType())) {
				throw new ArgumentException("Value must ListViewItem", "value");
			}
		}
		protected override void OnClear()
		{
			if (Clearing != null) {
				Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
			}
		}
		protected override void OnClearComplete()
		{
			if (AfterClear != null) {
				AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
			}
		}
		protected override void OnInsert(int index, object value)
		{
			if (Inserting != null) {
				Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
			}
		}
		protected override void OnInsertComplete(int index, object value)
		{
			if (AfterInsert != null) {
				AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
			}
		}
		protected override void OnRemove(int index, object value)
		{
			if (Removing != null) {
				Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
			}
		}
		protected override void OnRemoveComplete(int index, object value)
		{
			if (AfterRemove != null) {
				AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
			}
		}
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (Setting != null) {
				Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (AfterSet != null) {
				AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		#endregion
	}
	/// <summary>
	/// Represent a collection of Checked ListViewItem objects in a ListView or ListViewGroup.
	/// </summary>
	[Description("Represent a collection of Checked ListViewItem objects in a ListView or ListViewGroup.")]
	public class CheckedListViewItemCollection : CollectionBase
	{
			#region "Constructor"
		internal ListView _owner;
		public CheckedListViewItemCollection(ListView owner) : base()
		{
			_owner = owner;
		}
		#endregion
		#region "Public Events"
		public event ClearingEventHandler Clearing;
		public delegate void ClearingEventHandler(object sender, CollectionEventArgs e);
		public event AfterClearEventHandler AfterClear;
		public delegate void AfterClearEventHandler(object sender, CollectionEventArgs e);
		public event InsertingEventHandler Inserting;
		public delegate void InsertingEventHandler(object sender, CollectionEventArgs e);
		public event AfterInsertEventHandler AfterInsert;
		public delegate void AfterInsertEventHandler(object sender, CollectionEventArgs e);
		public event RemovingEventHandler Removing;
		public delegate void RemovingEventHandler(object sender, CollectionEventArgs e);
		public event AfterRemoveEventHandler AfterRemove;
		public delegate void AfterRemoveEventHandler(object sender, CollectionEventArgs e);
		public event SettingEventHandler Setting;
		public delegate void SettingEventHandler(object sender, CollectionEventArgs e);
		public event AfterSetEventHandler AfterSet;
		public delegate void AfterSetEventHandler(object sender, CollectionEventArgs e);
		#endregion
		#region "Public Properties"
		[Description("Gets a ListViewItem object in the collection specified by its index.")]
		public ListViewItem this[int index] {
			get {
				if (index >= 0 & index < List.Count)
					return (ListViewItem)List[index];
				return null;
			}
		}
		[Description("Gets the index of a ListViewItem object in the collection.")]
		public int IndexOf {
			get { return this.List.IndexOf(item); }
		}
		#endregion
		#region "Public Methods"
		[Description("Add a ListViewItem object to the collection.")]
		internal ListViewItem Add(ListViewItem item)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(item)) {
				item._owner = _owner;
				int index = List.Add(item);
				return (ListViewItem)List[index];
			}
			return item;
		}
		[Description("Add a ListViewItem collection to the collection.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		internal void AddRange(ListViewItemCollection items)
		{
			foreach (ListViewItem item in items) {
				this.Add(item);
			}
		}
		[Description("Remove a ListViewItem object from the collection.")]
		public void Remove(ListViewItem item)
		{
			if (List.Contains(item))
				List.Remove(item);
		}
		[Description("Determine whether a ListViewItem object exist in the collection.")]
		public bool Contains(ListViewItem item)
		{
			return List.Contains(item);
		}
		#endregion
		#region "Protected Overriden Methods"
		[Description("Performs additional custom processes when validating a value.")]
		protected override void OnValidate(object value)
		{
			if (!typeof(ListViewItem).IsAssignableFrom(value.GetType())) {
				throw new ArgumentException("Value must ListViewItem", "value");
			}
		}
		protected override void OnClear()
		{
			if (Clearing != null) {
				Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
			}
		}
		protected override void OnClearComplete()
		{
			if (AfterClear != null) {
				AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
			}
		}
		protected override void OnInsert(int index, object value)
		{
			if (Inserting != null) {
				Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
			}
		}
		protected override void OnInsertComplete(int index, object value)
		{
			if (AfterInsert != null) {
				AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
			}
		}
		protected override void OnRemove(int index, object value)
		{
			if (Removing != null) {
				Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
			}
		}
		protected override void OnRemoveComplete(int index, object value)
		{
			if (AfterRemove != null) {
				AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
			}
		}
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (Setting != null) {
				Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (AfterSet != null) {
				AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		#endregion
	}
	/// <summary>
	/// Represent a collection of ColumnHeader objects in a ListView.
	/// </summary>
	[Description("Represent a collection of ColumnHeader objects.")]
	public class ColumnHeaderCollection : CollectionBase
	{
			#region "Constructor"
		internal ListView _owner;
		public ColumnHeaderCollection(ListView owner) : base()
		{
			_owner = owner;
		}
		#endregion
		#region "Public Events"
		public event ClearingEventHandler Clearing;
		public delegate void ClearingEventHandler(object sender, CollectionEventArgs e);
		public event AfterClearEventHandler AfterClear;
		public delegate void AfterClearEventHandler(object sender, CollectionEventArgs e);
		public event InsertingEventHandler Inserting;
		public delegate void InsertingEventHandler(object sender, CollectionEventArgs e);
		public event AfterInsertEventHandler AfterInsert;
		public delegate void AfterInsertEventHandler(object sender, CollectionEventArgs e);
		public event RemovingEventHandler Removing;
		public delegate void RemovingEventHandler(object sender, CollectionEventArgs e);
		public event AfterRemoveEventHandler AfterRemove;
		public delegate void AfterRemoveEventHandler(object sender, CollectionEventArgs e);
		public event SettingEventHandler Setting;
		public delegate void SettingEventHandler(object sender, CollectionEventArgs e);
		public event AfterSetEventHandler AfterSet;
		public delegate void AfterSetEventHandler(object sender, CollectionEventArgs e);
		#endregion
		#region "Public Properties"
		[Description("Gets a ColumnHeader object in the collection specified by its index.")]
		public ColumnHeader this[int index] {
			get {
				if (index >= 0 & index < List.Count)
					return (ColumnHeader)List[index];
				return null;
			}
		}
		[Description("Gets the index of a ColumnHeader object in the collection.")]
		public int IndexOf {
			get { return this.List.IndexOf(item); }
		}
		#endregion
		#region "Public Methods"
		[Description("Add a ColumnHeader object to the collection.")]
		public ColumnHeader Add(ColumnHeader header)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(header)) {
				header._owner = _owner;
				header.EnableFilteringChanged += columnVisibilityChanged;
				header.EnableFrozenChanged += columnVisibilityChanged;
				header.EnableHiddenChanged += columnVisibilityChanged;
				header.EnableSortingChanged += columnVisibilityChanged;
				header.FrozenChanged += columnVisibilityChanged;
				header.ImageChanged += columnVisibilityChanged;
				header.SizeTypeChanged += columnVisibilityChanged;
				header.SortOrderChanged += columnSortOrderChanged;
				header.TextAlignChanged += columnAppearanceChanged;
				header.TextChanged += columnAppearanceChanged;
				header.VisibleChanged += columnVisibilityChanged;
				header.WidthChanged += columnVisibilityChanged;
				header.MaximumValueChanged += columnItemsRelatedValueChanged;
				header.MinimumValueChanged += columnItemsRelatedValueChanged;
				header.FormatChanged += columnItemsRelatedValueChanged;
				header.ColumnAlignChanged += columnItemsRelatedValueChanged;
				header.CustomFormatChanged += columnCustomFormatChanged;
				int index = List.Add(header);
				return (ColumnHeader)List[index];
			}
			return header;
		}
		[Description("Add a ColumnHeader object to the collection by providing its text.")]
		public ColumnHeader Add(string text)
		{
			ColumnHeader aHeader = new ColumnHeader(_owner);
			aHeader.Text = text;
			return this.Add(aHeader);
		}
		[Description("Add a ColumnHeader collection to the collection.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public void AddRange(ColumnHeaderCollection columns)
		{
			foreach (ColumnHeader column in columns) {
				this.Add(column);
			}
		}
		[Description("Insert a ColumnHeader object to the collection at specified index.")]
		public void Insert(int index, ColumnHeader header)
		{
			header._owner = _owner;
			header.EnableFilteringChanged += columnVisibilityChanged;
			header.EnableFrozenChanged += columnVisibilityChanged;
			header.EnableHiddenChanged += columnVisibilityChanged;
			header.EnableSortingChanged += columnVisibilityChanged;
			header.FrozenChanged += columnVisibilityChanged;
			header.ImageChanged += columnVisibilityChanged;
			header.SizeTypeChanged += columnVisibilityChanged;
			header.SortOrderChanged += columnSortOrderChanged;
			header.TextAlignChanged += columnAppearanceChanged;
			header.TextChanged += columnAppearanceChanged;
			header.VisibleChanged += columnVisibilityChanged;
			header.WidthChanged += columnVisibilityChanged;
			header.MaximumValueChanged += columnItemsRelatedValueChanged;
			header.MinimumValueChanged += columnItemsRelatedValueChanged;
			header.FormatChanged += columnItemsRelatedValueChanged;
			header.ColumnAlignChanged += columnItemsRelatedValueChanged;
			header.CustomFormatChanged += columnCustomFormatChanged;
			List.Insert(index, header);
		}
		[Description("Remove a ColumnHeader object from the collection.")]
		public void Remove(ColumnHeader header)
		{
			if (!List.Contains(header))
				return;
			header.EnableFilteringChanged -= columnVisibilityChanged;
			header.EnableFrozenChanged -= columnVisibilityChanged;
			header.EnableHiddenChanged -= columnVisibilityChanged;
			header.EnableSortingChanged -= columnVisibilityChanged;
			header.FrozenChanged -= columnVisibilityChanged;
			header.ImageChanged -= columnVisibilityChanged;
			header.SizeTypeChanged -= columnVisibilityChanged;
			header.SortOrderChanged -= columnSortOrderChanged;
			header.TextAlignChanged -= columnAppearanceChanged;
			header.TextChanged -= columnAppearanceChanged;
			header.VisibleChanged -= columnVisibilityChanged;
			header.WidthChanged -= columnVisibilityChanged;
			header.MaximumValueChanged -= columnItemsRelatedValueChanged;
			header.MinimumValueChanged -= columnItemsRelatedValueChanged;
			header.FormatChanged -= columnItemsRelatedValueChanged;
			header.ColumnAlignChanged -= columnItemsRelatedValueChanged;
			header.CustomFormatChanged -= columnCustomFormatChanged;
			_owner._columnControl.removeHost(header);
			_owner._columnControl.Invalidate();
			List.Remove(header);
		}
		[Description("Determine whether a ColumnHeader object exist in the collection.")]
		public bool Contains(ColumnHeader header)
		{
			return List.Contains(header);
		}
		#endregion
		#region "Private Methods"
		private bool containsName(string name)
		{
			foreach (ColumnHeader ch in List) {
				if (string.Compare(ch.Name, name, true) == 0)
					return true;
			}
			return false;
		}
		#endregion
		#region "Protected Overriden Methods"
		[Description("Performs additional custom processes when validating a value.")]
		protected override void OnValidate(object value)
		{
			if (!typeof(ColumnHeader).IsAssignableFrom(value.GetType())) {
				throw new ArgumentException("Value must ColumnHeader", "value");
			}
		}
		protected override void OnClear()
		{
			foreach (ColumnHeader header in List) {
				header.EnableFilteringChanged -= columnVisibilityChanged;
				header.EnableFrozenChanged -= columnVisibilityChanged;
				header.EnableHiddenChanged -= columnVisibilityChanged;
				header.EnableSortingChanged -= columnVisibilityChanged;
				header.FrozenChanged -= columnVisibilityChanged;
				header.ImageChanged -= columnVisibilityChanged;
				header.SizeTypeChanged -= columnVisibilityChanged;
				header.SortOrderChanged -= columnSortOrderChanged;
				header.TextAlignChanged -= columnAppearanceChanged;
				header.TextChanged -= columnAppearanceChanged;
				header.VisibleChanged -= columnVisibilityChanged;
				header.WidthChanged -= columnVisibilityChanged;
				header.MaximumValueChanged -= columnItemsRelatedValueChanged;
				header.MinimumValueChanged -= columnItemsRelatedValueChanged;
				header.FormatChanged -= columnItemsRelatedValueChanged;
				header.ColumnAlignChanged -= columnItemsRelatedValueChanged;
				header.CustomFormatChanged -= columnCustomFormatChanged;
			}
			if (Clearing != null) {
				Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear));
			}
		}
		protected override void OnClearComplete()
		{
			_owner._columnControl.clearHosts();
			_owner._columnControl.Invalidate();
			if (AfterClear != null) {
				AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete));
			}
		}
		protected override void OnInsert(int index, object value)
		{
			//If _owner.DesignMode Then
			//    Dim column As ColumnHeader = DirectCast(value, ColumnHeader)
			//    Dim i As Integer = 0
			//    Dim find As Boolean = False
			//    While i < List.Count And Not find
			//        find = containsName("ColumnHeader" & CStr(i))
			//        If Not find Then i += 1
			//    End While
			//    column.Name = "ColumnHeader" & CStr(i)
			//End If
			if (Inserting != null) {
				Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
			}
		}
		protected override void OnInsertComplete(int index, object value)
		{
			_owner._columnControl.addHost(value);
			_owner._columnControl.Invalidate();
			if (AfterInsert != null) {
				AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value));
			}
		}
		protected override void OnRemove(int index, object value)
		{
			if (Removing != null) {
				Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value));
			}
		}
		protected override void OnRemoveComplete(int index, object value)
		{
			if (AfterRemove != null) {
				AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value));
			}
		}
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (Setting != null) {
				Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (AfterSet != null) {
				AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue));
			}
		}
		#endregion
		#region "Column event handlers"
		private void columnAppearanceChanged(object sender, EventArgs e)
		{
			_owner._columnControl.Invalidate();
		}
		private void columnVisibilityChanged(object sender, EventArgs e)
		{
			_owner._columnControl.relocateHosts((_owner._hScroll.Visible ? -_owner._hScroll.Value : 0));
			_owner._columnControl.Invalidate();
			if (_owner._view == Control.View.Details) {
				_owner.measureAll();
				_owner.relocateAll();
				_owner.Invalidate();
			}
		}
		private void columnSortOrderChanged(object sender, EventArgs e)
		{
			ColumnHeader ch = sender;
			if (ch.EnableSorting) {
				if (ch.SortOrder != SortOrder.None) {
					foreach (ColumnHeader otherCH in List) {
						if (!object.ReferenceEquals(otherCH, ch))
							otherCH.SortOrder = SortOrder.None;
					}
					_owner._columnRef = _owner._columns.IndexOf[ch];
					_owner.sortAll();
					_owner.relocateAll();
					if (_owner._view == Control.View.Preview)
						_owner.drawPreviewHosts();
					_owner.Invalidate();
				}
			}
		}
		private void columnItemsRelatedValueChanged(object sender, EventArgs e)
		{
			if (_owner._view == View.Details | _owner._view == View.Tile) {
				_owner.Invalidate();
			}
		}
		private void columnCustomFormatChanged(object sender, EventArgs e)
		{
			ColumnHeader ch = (ColumnHeader)sender;
			if (ch.Format == ColumnFormat.Custom | ch.Format == ColumnFormat.CustomDateTime) {
				if (_owner._view == View.Details | _owner._view == View.Tile) {
					_owner.Invalidate();
				}
			}
		}
		#endregion
	}
	#endregion
	#region "Private Constants."
	/// <summary>
	/// Maximum size allowed to display a thumbnail image in a ListView.
	/// </summary>
	const int _maxThumbnailSize = 256;
	/// <summary>
	/// Minimum size allowed to display a thumbnail image in a ListView.
	/// </summary>
	const int _minThumbnailSize = 100;
	/// <summary>
	/// Maximum size allowed to display an icon image in a ListView.
	/// </summary>
	const int _maxIconSize = 80;
	/// <summary>
	/// Minimum size allowed to display an icon image in a ListView.
	/// </summary>
	const int _minIconSize = 32;
	/// <summary>
	/// Width of each column used to display items on List view mode.
	/// </summary>
	const int _listColumnWidth = 400;
	/// <summary>
	/// Width of each column used to display items on Tile view mode.
	/// </summary>
	const int _tileColumnWidth = 300;
	/// <summary>
	/// Height of each item displayed on Tile view mode.
	/// </summary>
	const int _tileHeight = 64;
	/// <summary>
	/// Size of each item displayed on Preview view mode.
	/// </summary>
	const int _slideItemSize = 80;
	/// <summary>
	/// Margin size of each subitem from its bounding rectangle.
	/// </summary>
	const int _subItemMargin = 3;
	/// <summary>
	/// Maximum height of text displayed when item is selected, on Icon and Thumbnail view mode.
	/// </summary>
		#endregion
	const int _maxTextHeight = 40;
	#region "Members."
	// Components
	private CheckedListViewItemCollection withEventsField__checkedItems;
	public CheckedListViewItemCollection _checkedItems {
		get { return withEventsField__checkedItems; }
		set {
			if (withEventsField__checkedItems != null) {
				withEventsField__checkedItems.AfterClear -= _checkedItems_AfterClear;
				withEventsField__checkedItems.AfterRemove -= _checkedItems_AfterRemove;
			}
			withEventsField__checkedItems = value;
			if (withEventsField__checkedItems != null) {
				withEventsField__checkedItems.AfterClear += _checkedItems_AfterClear;
				withEventsField__checkedItems.AfterRemove += _checkedItems_AfterRemove;
			}
		}
	}
	private ListViewItemCollection withEventsField__items;
	public ListViewItemCollection _items {
		get { return withEventsField__items; }
		set {
			if (withEventsField__items != null) {
				withEventsField__items.AfterClear -= _items_AfterClear;
				withEventsField__items.AfterInsert -= _items_AfterInsert;
				withEventsField__items.AfterRemove -= _items_AfterRemove;
			}
			withEventsField__items = value;
			if (withEventsField__items != null) {
				withEventsField__items.AfterClear += _items_AfterClear;
				withEventsField__items.AfterInsert += _items_AfterInsert;
				withEventsField__items.AfterRemove += _items_AfterRemove;
			}
		}
	}
	private ListViewGroupCollection withEventsField__groups;
	public ListViewGroupCollection _groups {
		get { return withEventsField__groups; }
		set {
			if (withEventsField__groups != null) {
				withEventsField__groups.AfterClear -= _groups_AfterClear;
				withEventsField__groups.AfterInsert -= _groups_AfterInsert;
				withEventsField__groups.AfterRemove -= _groups_AfterRemove;
			}
			withEventsField__groups = value;
			if (withEventsField__groups != null) {
				withEventsField__groups.AfterClear += _groups_AfterClear;
				withEventsField__groups.AfterInsert += _groups_AfterInsert;
				withEventsField__groups.AfterRemove += _groups_AfterRemove;
			}
		}
	}
	private ColumnHeaderCollection withEventsField__columns;
	public ColumnHeaderCollection _columns {
		get { return withEventsField__columns; }
		set {
			if (withEventsField__columns != null) {
				withEventsField__columns.AfterClear -= _columns_AfterClear;
				withEventsField__columns.AfterInsert -= _columns_AfterInsert;
				withEventsField__columns.AfterRemove -= _columns_AfterRemove;
			}
			withEventsField__columns = value;
			if (withEventsField__columns != null) {
				withEventsField__columns.AfterClear += _columns_AfterClear;
				withEventsField__columns.AfterInsert += _columns_AfterInsert;
				withEventsField__columns.AfterRemove += _columns_AfterRemove;
			}
		}
	}
	private FrozenListViewItemCollection withEventsField__frozenItems;
	public FrozenListViewItemCollection _frozenItems {
		get { return withEventsField__frozenItems; }
		set {
			if (withEventsField__frozenItems != null) {
				withEventsField__frozenItems.AfterClear -= _frozenItems_AfterClear;
				withEventsField__frozenItems.AfterInsert -= _frozenItems_AfterInsert;
				withEventsField__frozenItems.AfterRemove -= _frozenItems_AfterRemove;
				withEventsField__frozenItems.Clearing -= _frozenItems_Clearing;
				withEventsField__frozenItems.Inserting -= _frozenItems_Inserting;
				withEventsField__frozenItems.Removing -= _frozenItems_Removing;
			}
			withEventsField__frozenItems = value;
			if (withEventsField__frozenItems != null) {
				withEventsField__frozenItems.AfterClear += _frozenItems_AfterClear;
				withEventsField__frozenItems.AfterInsert += _frozenItems_AfterInsert;
				withEventsField__frozenItems.AfterRemove += _frozenItems_AfterRemove;
				withEventsField__frozenItems.Clearing += _frozenItems_Clearing;
				withEventsField__frozenItems.Inserting += _frozenItems_Inserting;
				withEventsField__frozenItems.Removing += _frozenItems_Removing;
			}
		}
	}
	private VScrollBar withEventsField__vScroll = new VScrollBar();
	public VScrollBar _vScroll {
		get { return withEventsField__vScroll; }
		set {
			if (withEventsField__vScroll != null) {
				withEventsField__vScroll.ValueChanged -= _vScroll_ValueChanged;
			}
			withEventsField__vScroll = value;
			if (withEventsField__vScroll != null) {
				withEventsField__vScroll.ValueChanged += _vScroll_ValueChanged;
			}
		}
	}
	private HScrollBar withEventsField__hScroll = new HScrollBar();
	public HScrollBar _hScroll {
		get { return withEventsField__hScroll; }
		set {
			if (withEventsField__hScroll != null) {
				withEventsField__hScroll.ValueChanged -= _hScroll_ValueChanged;
			}
			withEventsField__hScroll = value;
			if (withEventsField__hScroll != null) {
				withEventsField__hScroll.ValueChanged += _hScroll_ValueChanged;
			}
		}
	}
	// Properties
	bool _checkBoxes = false;
	bool _rowNumbers = false;
	bool _showColumnOptions = true;
	Control.View _view = Control.View.Details;
	Control.SlideLocation _slideLocation = Control.SlideLocation.Bottom;
	int _thumbnailSize = 100;
	int _iconSize = 32;
	bool _allowMultiline = false;
	CultureInfo _ci = Renderer.Drawing.en_us_ci;
	bool _showGroups = true;
	bool _fullRowSelect = false;
	bool _labelEdit = false;
	System.Drawing.Color _previewBackColor = Color.Black;
	bool _itemTooltip = true;
	RowsDock _frozenRowsDock = RowsDock.Bottom;
	// Internal use
	private ColumnHeaderControl withEventsField__columnControl;
	public ColumnHeaderControl _columnControl {
		get { return withEventsField__columnControl; }
		set {
			if (withEventsField__columnControl != null) {
				withEventsField__columnControl.AfterColumnCustomFilter -= _columnControl_AfterColumnCustomFilter;
				withEventsField__columnControl.AfterColumnFilter -= _columnControl_AfterColumnFilter;
				withEventsField__columnControl.AfterColumnResize -= _columnControl_AfterColumnResize;
				withEventsField__columnControl.CheckedChanged -= _columnControl_CheckedChanged;
				withEventsField__columnControl.ColumnOrderChanged -= _columnControl_ColumnOrderChanged;
			}
			withEventsField__columnControl = value;
			if (withEventsField__columnControl != null) {
				withEventsField__columnControl.AfterColumnCustomFilter += _columnControl_AfterColumnCustomFilter;
				withEventsField__columnControl.AfterColumnFilter += _columnControl_AfterColumnFilter;
				withEventsField__columnControl.AfterColumnResize += _columnControl_AfterColumnResize;
				withEventsField__columnControl.CheckedChanged += _columnControl_CheckedChanged;
				withEventsField__columnControl.ColumnOrderChanged += _columnControl_ColumnOrderChanged;
			}
		}
		// Custom Control to handle all column header operations.
	}
		// Font object used to draw ListViewGroup text.
	Font _groupFont;
		// Index of a ColumnHeader that perform sort operation.
	int _columnRef = -1;
		// Indicating whether an event is called from internal process, to avoid multiple calls of an operation on an event.
	bool _internalThread = false;
		// An area to draw ListViewItem and ListViewGroup.
	Rectangle _clientArea;
	private ListViewGroup withEventsField__defaultGroup;
	public ListViewGroup _defaultGroup {
		get { return withEventsField__defaultGroup; }
		set {
			if (withEventsField__defaultGroup != null) {
				withEventsField__defaultGroup.AfterCheck -= _defaultGroup_AfterCheck;
				withEventsField__defaultGroup.AfterCollape -= _defaultGroup_AfterCollape;
				withEventsField__defaultGroup.AfterExpand -= _defaultGroup_AfterExpand;
				withEventsField__defaultGroup.BeforeCheck -= _defaultGroup_BeforeCheck;
				withEventsField__defaultGroup.BeforeCollapse -= _defaultGroup_BeforeCollapse;
				withEventsField__defaultGroup.BeforeExpand -= _defaultGroup_BeforeExpand;
			}
			withEventsField__defaultGroup = value;
			if (withEventsField__defaultGroup != null) {
				withEventsField__defaultGroup.AfterCheck += _defaultGroup_AfterCheck;
				withEventsField__defaultGroup.AfterCollape += _defaultGroup_AfterCollape;
				withEventsField__defaultGroup.AfterExpand += _defaultGroup_AfterExpand;
				withEventsField__defaultGroup.BeforeCheck += _defaultGroup_BeforeCheck;
				withEventsField__defaultGroup.BeforeCollapse += _defaultGroup_BeforeCollapse;
				withEventsField__defaultGroup.BeforeExpand += _defaultGroup_BeforeExpand;
			}
		}
		// Represent the default group of the ListView
	}
	private ToolTip withEventsField__tooltip;
	public ToolTip _tooltip {
		get { return withEventsField__tooltip; }
		set {
			if (withEventsField__tooltip != null) {
				withEventsField__tooltip.Draw -= _tooltip_Draw;
				withEventsField__tooltip.Popup -= _tooltip_Popup;
			}
			withEventsField__tooltip = value;
			if (withEventsField__tooltip != null) {
				withEventsField__tooltip.Draw += _tooltip_Draw;
				withEventsField__tooltip.Popup += _tooltip_Popup;
			}
		}
		// ToolTip object to show Item tooltip.
	}
	private ItemSlider withEventsField__slider;
	public ItemSlider _slider {
		get { return withEventsField__slider; }
		set {
			if (withEventsField__slider != null) {
				withEventsField__slider.ValueChanged -= _slider_ValueChanged;
			}
			withEventsField__slider = value;
			if (withEventsField__slider != null) {
				withEventsField__slider.ValueChanged += _slider_ValueChanged;
			}
		}
		// Represent slider for items on Preview views.
	}
		// A color blend to draw line separator of each column in ListView.
	ColorBlend _linePenBlend;
		// Indicating whether the mouse pointer is inside the client area (_clientArea) of the ListView.
	bool _mouseOnClientArea = false;
		// The index of current selected ListViewItem object in _items collection.
	int _selectedIndex = -1;
		// Indicating when a tooltip need to be shown.
	bool _needToolTip = false;
	private TextBoxLabelEditor withEventsField__txtEditor;
	public TextBoxLabelEditor _txtEditor {
		get { return withEventsField__txtEditor; }
		set {
			if (withEventsField__txtEditor != null) {
				withEventsField__txtEditor.KeyDown -= _txtEditor_KeyDown;
				withEventsField__txtEditor.LostFocus -= _txtEditor_LostFocus;
			}
			withEventsField__txtEditor = value;
			if (withEventsField__txtEditor != null) {
				withEventsField__txtEditor.KeyDown += _txtEditor_KeyDown;
				withEventsField__txtEditor.LostFocus += _txtEditor_LostFocus;
			}
		}
		// TextBox control to perform label editng operation.
	}
		// A ListViewItemHost object that performs label editing operation.
	ListViewItemHost _currentEditedHost = null;
	Matrix _normalMatrix;
	Matrix _mirrorMatrix;
	Bitmap _slideBgImage = null;
	List<ListViewItemHost> _frozenHosts = new List<ListViewItemHost>();
	Rectangle _frozenArea;
		// Hold the last value of the VScrollBar
	int _lastVScrollValue = 0;
	// Item Host
		// A list of ListViewItemHost.  All item host is stored here.
	List<ListViewItemHost> _itemHosts = new List<ListViewItemHost>();
		// Host of selected item.
	ListViewItemHost _selectedHost = null;
	// Group Host
		// A list of ListViewGroupHost.  All group host is stored here.
	List<ListViewGroupHost> _groupHosts = new List<ListViewGroupHost>();
		// A ListViewGroupHost to control default group.
	ListViewGroupHost _defaultGroupHost;
	// For text measuring purposes
		// A bitmap to create a Graphics object.
	Bitmap _gBmp = new Bitmap(1, 1);
		// A Graphics object to measure text, to support text formating in measurement.
	Graphics _gObj = Graphics.FromImage(_gBmp);
	// ToolTip
		// A tooltip text needed to be shown.
	string _currentToolTip = "";
		// A tooltip title needed to be shown.
	string _currentToolTipTitle = "";
		// A tooltip image needed to be shown.
	Image _currentToolTipImage = null;
		// An area that must be avoided by the tooltip.
	Rectangle _currentToolTipRect;
		// A ListViewItemHost that need the tooltip to be shown.
	ListViewItemHost _tooltipCaller = null;
	#endregion
	#region "Core Engines.  Classes to handle all objects, key and mouse event handlers, and as a visual representation of an object associated with ListView."
	/// <summary>
	/// Class to hosts all columns and handles all of the operations.
	/// </summary>
	private class ColumnHeaderControl : Windows.Forms.Control
	{
		/// <summary>
		/// Host for each ColumnHeader in ColumnHeaderControl.
		/// </summary>
		private class ColumnHost
		{
			ColumnHeader _column;
			ColumnHeaderControl _owner;
			Rectangle _rect;
			bool _selected = false;
			bool _onHover = false;
			bool _onHoverSplit = false;
			bool _onMouseDown = false;
			bool _onMouseDownSplit = false;
			ColumnFilterHandle _filterHandler;
			public ColumnHost(ColumnHeader column, ColumnHeaderControl owner)
			{
				_column = column;
				_owner = owner;
				_filterHandler = new ColumnFilterHandle(_column);
			}
			/// <summary>
			/// Draw ColumnHeader object in the ColumnHeaderControl.
			/// </summary>
			/// <param name="g">Graphics object to draw the column.</param>
			public void draw(Graphics g)
			{
				Rectangle txtColRect = _rect;
				StringFormat txtColFormat = new StringFormat();
				Rectangle imgColRect = _rect;
				int sortSignLeft = _rect.Right - 8;
				if (_column.Image != null) {
					imgColRect.X = txtColRect.X;
					imgColRect.Size = Renderer.Drawing.scaleImage(_column.Image, 16);
					imgColRect.Y = (_rect.Height - imgColRect.Height) / 2;
					txtColRect.X = txtColRect.X + 18;
					txtColRect.Width = txtColRect.Width - 18;
				}
				if (_column.EnableFiltering) {
					txtColRect.Width = txtColRect.Width - 11;
					sortSignLeft = _rect.Right - 18;
				}
				if (_column.SortOrder != SortOrder.None & _column.EnableSorting) {
					txtColRect.Width = txtColRect.Width - 9;
				}
				txtColFormat.LineAlignment = StringAlignment.Center;
				switch (_column.TextAlign) {
					case HorizontalAlignment.Center:
						txtColFormat.Alignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Left:
						txtColFormat.Alignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Right:
						txtColFormat.Alignment = StringAlignment.Far;
						break;
				}
				txtColFormat.FormatFlags = txtColFormat.FormatFlags | StringFormatFlags.NoWrap;
				txtColFormat.Trimming = StringTrimming.EllipsisCharacter;
				LinearGradientBrush bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
				bgBrush.InterpolationColors = Renderer.Column.NormalBlend;
				g.FillRectangle(bgBrush, _rect);
				bgBrush.Dispose();
				bgBrush = null;
				if (_owner.Enabled) {
					LinearGradientBrush splitBrush = null;
					Rectangle splitRect = new Rectangle(_rect.Right - 10, _rect.Y, 10, _rect.Height);
					Pen linePen = Renderer.Column.NormalBorderPen;
					if (_onHover) {
						bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
						if (_column.EnableFiltering) {
							splitBrush = new LinearGradientBrush(splitRect, Color.Black, Color.White, LinearGradientMode.Vertical);
							if (_onMouseDownSplit) {
								splitBrush.InterpolationColors = Renderer.Column.PressedBlend;
							} else {
								if (_onHoverSplit) {
									splitBrush.InterpolationColors = Renderer.Column.HLitedDropDownBlend;
								} else {
									splitBrush.InterpolationColors = Renderer.Column.HLitedBlend;
								}
							}
						}
						if (_onMouseDown) {
							bgBrush.InterpolationColors = Renderer.Column.PressedBlend;
						} else {
							bgBrush.InterpolationColors = Renderer.Column.HLitedBlend;
						}
					} else {
						if (_selected) {
							bgBrush = new LinearGradientBrush(_rect, Color.Black, Color.White, LinearGradientMode.Vertical);
							bgBrush.InterpolationColors = Renderer.Column.SelectedBlend;
						}
					}
					if (bgBrush != null) {
						linePen = Renderer.Column.ActiveBorderPen;
						g.FillRectangle(bgBrush, _rect);
						bgBrush.Dispose();
					}
					if (splitBrush != null) {
						g.FillRectangle(splitBrush, splitRect);
						splitBrush.Dispose();
					}
					if (_column.EnableFiltering) {
						g.DrawLine(linePen, _rect.Right - 10, _rect.Y + 1, _rect.Right - 10, _rect.Bottom - 2);
						Renderer.Drawing.drawTriangle(g, _rect.Right - 8, _rect.Y + Convert.ToInt32((_rect.Height - 6) / 2), Color.FromArgb(21, 66, 139), Color.White, Renderer.Drawing.TriangleDirection.Down);
					}
				} else {
					if (_column.EnableFiltering) {
						g.DrawLine(Pens.Gray, _rect.Right - 10, _rect.Y + 1, _rect.Right - 10, _rect.Bottom - 2);
						Renderer.Drawing.drawTriangle(g, _rect.Right - 8, _rect.Y + Convert.ToInt32((_rect.Height - 6) / 2), Color.Gray, Color.White, Renderer.Drawing.TriangleDirection.Down);
					}
				}
				if (_column.Image != null) {
					if (_owner.Enabled) {
						g.DrawImage(_column.Image, imgColRect);
					} else {
						Renderer.Drawing.grayscaledImage(_column.Image, imgColRect, g);
					}
				}
				g.DrawString(_column.Text, _owner.Font, (_owner.Enabled ? Renderer.Column.TextBrush : Renderer.Drawing.DisabledTextBrush), txtColRect, txtColFormat);
				if (_column.EnableSorting) {
					switch (_column.SortOrder) {
						case SortOrder.Ascending:
							Renderer.Drawing.drawTriangle(g, sortSignLeft, _rect.Y + Convert.ToInt32((_rect.Height - 6) / 2), (_owner.Enabled ? Color.FromArgb(21, 66, 139) : Color.Gray), Color.White, Renderer.Drawing.TriangleDirection.Up);
							break;
						case SortOrder.Descending:
							Renderer.Drawing.drawTriangle(g, sortSignLeft, _rect.Y + Convert.ToInt32((_rect.Height - 6) / 2), (_owner.Enabled ? Color.FromArgb(21, 66, 139) : Color.Gray), Color.White, Renderer.Drawing.TriangleDirection.Down);
							break;
					}
				}
				if (_onMouseDown)
					Renderer.Column.drawPressedShadow(g, _rect);
			}
			/// <summary>
			/// Draw ColumnHeader object when moved.
			/// </summary>
			/// <param name="g">Graphics object to draw the column.</param>
			/// <param name="rect">Area where the column must be drawn.</param>
			/// <param name="canDrop">Determine whether the column can be dropped on a location.</param>
			public void drawMoved(Graphics g, Rectangle rect, bool canDrop = true)
			{
				Rectangle txtColRect = rect;
				StringFormat txtColFormat = new StringFormat();
				SolidBrush txtBrush = new SolidBrush(Color.FromArgb(191, 127, 127, 127));
				Pen borderPen = new Pen(Color.FromArgb(191, 127, 127, 127), 2);
				if (_column.Image != null) {
					txtColRect.X = txtColRect.X + 18;
					txtColRect.Width = txtColRect.Width - 18;
				}
				if (_column.EnableFiltering) {
					txtColRect.Width = txtColRect.Width - 11;
				}
				if (_column.SortOrder != SortOrder.None & _column.EnableSorting) {
					txtColRect.Width = txtColRect.Width - 9;
				}
				txtColFormat.LineAlignment = StringAlignment.Center;
				switch (_column.TextAlign) {
					case HorizontalAlignment.Center:
						txtColFormat.Alignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Left:
						txtColFormat.Alignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Right:
						txtColFormat.Alignment = StringAlignment.Far;
						break;
				}
				txtColFormat.FormatFlags = txtColFormat.FormatFlags | StringFormatFlags.NoWrap;
				txtColFormat.Trimming = StringTrimming.EllipsisCharacter;
				g.DrawString(_column.Text, _owner.Font, txtBrush, txtColRect, txtColFormat);
				g.DrawRectangle(borderPen, rect);
				if (!canDrop) {
					Rectangle rectSign = new Rectangle(rect.X + 2, rect.Y + 2, rect.Height - 5, rect.Height - 5);
					g.FillEllipse(new SolidBrush(Color.FromArgb(191, 255, 0, 0)), rectSign);
					g.DrawEllipse(new Pen(Color.FromArgb(191, 0, 0, 0)), rectSign);
					g.DrawLine(new Pen(Color.White, 3), rectSign.X + 2, rectSign.Y + Convert.ToInt32(rectSign.Height / 2), rectSign.Right - 3, rectSign.Y + Convert.ToInt32(rectSign.Height / 2));
				}
			}
			/// <summary>
			/// Gets a value indicating a column header need a tooltip to be shown.
			/// </summary>
			/// <returns>True if the column need tooltip.</returns>
			private bool needToolTip()
			{
				Rectangle txtColRect = _rect;
				if (_column.Image != null) {
					txtColRect.X = txtColRect.X + 18;
					txtColRect.Width = txtColRect.Width - 18;
				}
				if (_column.EnableFiltering) {
					txtColRect.Width = txtColRect.Width - 11;
				}
				if (_column.SortOrder != SortOrder.None & _column.EnableSorting) {
					txtColRect.Width = txtColRect.Width - 9;
				}
				return TextRenderer.MeasureText(_column.Text, _owner.Font).Width > txtColRect.Width;
			}
			/// <summary>
			/// Test whether mouse pointer is moved over the column.
			/// </summary>
			/// <returns>True if the state is changed and need to change the appearance.</returns>
			public bool mouseMove(MouseEventArgs e)
			{
				if (_owner._owner._showColumnOptions) {
					if (_owner._optRect.Contains(e.Location)) {
						if (_onHover | _onHoverSplit) {
							_onHover = false;
							_onHoverSplit = false;
							return true;
						}
						return false;
					}
				}
				if (e.X <= _owner._frozenRight & !_column.Frozen) {
					if (_onHover | _onHoverSplit) {
						_onHover = false;
						_onHoverSplit = false;
						return true;
					}
					return false;
				}
				bool stateChanged = false;
				Rectangle rectSplit = new Rectangle(0, 0, 0, 0);
				if (_column.EnableFiltering)
					rectSplit = new Rectangle(_rect.Right - 10, _rect.Y, 10, _rect.Height);
				if (_rect.Contains(e.Location)) {
					if (rectSplit.Contains(e.Location)) {
						if (!_onHoverSplit) {
							_onHoverSplit = true;
							stateChanged = true;
						}
					} else {
						if (_onHoverSplit) {
							_onHoverSplit = false;
							stateChanged = true;
						}
					}
					if (!_onHover) {
						_onHover = true;
						_owner._currentToolTipRect = _rect;
						if (Renderer.ToolTip.containsToolTip(_column.ToolTipTitle, _column.ToolTip, _column.ToolTipImage)) {
							_owner._currentToolTip = _column.ToolTip;
							_owner._currentToolTipTitle = _column.ToolTipTitle;
							_owner._currentToolTipImage = _column.ToolTipImage;
						} else {
							if (needToolTip())
								_owner._currentToolTip = _column.Text;
						}
						stateChanged = true;
					}
				} else {
					if (_onHover | _onHoverSplit) {
						_onHover = false;
						_onHoverSplit = false;
						stateChanged = true;
					}
				}
				return stateChanged;
			}
			/// <summary>
			/// Test whether the mouse left button is pressed over the column, whether it pressed on the column, or on the column' split.
			/// </summary>
			public bool mouseDown()
			{
				if (_onHover | _onHoverSplit) {
					if (_owner._selectedHost != null)
						_owner._selectedHost._selected = false;
					_selected = true;
					_owner._selectedHost = this;
					if (_onHoverSplit) {
						_onMouseDownSplit = true;
						_onMouseDown = false;
						Rectangle rectSplit = new Rectangle(0, 0, 0, 0);
						rectSplit = new Rectangle(_rect.Right - 10, _rect.Y, 10, _rect.Height);
						_owner.showFilterPopup(new FilterChooser(_filterHandler, _owner._toolStrip, Renderer.ToolTip.TextFont, _owner._owner._ci), rectSplit);
					} else {
						_onMouseDown = true;
						_onMouseDownSplit = false;
						return true;
					}
				} else {
					if (_onMouseDown | _onMouseDownSplit) {
						_onMouseDown = false;
						_onMouseDownSplit = false;
						return true;
					}
				}
				return false;
			}
			/// <summary>
			/// Test whether the mouse left button is released over the column.
			/// </summary>
			public bool mouseUp()
			{
				if (_onMouseDown | _onMouseDownSplit) {
					_onMouseDown = false;
					_onMouseDownSplit = false;
					return true;
				}
				return false;
			}
			/// <summary>
			/// Test whether the mouse leaving the column.
			/// </summary>
			public bool mouseLeave()
			{
				if (_onHover | _onHoverSplit | _onMouseDown | _onMouseDownSplit) {
					_onHover = false;
					_onHoverSplit = false;
					_onMouseDown = false;
					_onMouseDownSplit = false;
					return true;
				}
				return false;
			}
			/// <summary>
			/// Add a ListViewItem or its subitem to the filter parameter, based on the column and the associated subitems.
			/// </summary>
			/// <param name="item">ListViewItem object to be added.</param>
			public void addItemToFilter(ListViewItem item)
			{
				if (_column.EnableFiltering) {
					int columnIndex = _owner._owner._columns.IndexOf[_column];
					if (item.SubItems[columnIndex] != null) {
						_filterHandler.addFilter(item.SubItems[columnIndex].Value);
					}
				}
			}
			/// <summary>
			/// Clear all filter parameter on the column.
			/// </summary>
			public void clearFilter()
			{
				_filterHandler.Items.Clear();
			}
			/// <summary>
			/// Determine whether the column has been selected.
			/// </summary>
			public bool Selected {
				get { return _selected; }
				set { _selected = value; }
			}
			/// <summary>
			/// The bounding rectangle of the column in the ColumnHeaderControl.
			/// </summary>
			public Rectangle Bounds {
				get { return _rect; }
				set { _rect = value; }
			}
			/// <summary>
			/// Determine the width of the host based on the column's attributes.
			/// </summary>
			public int Width {
				get { return _rect.Width; }
				set {
					if (_rect.Width != value) {
						if (value >= MinResize) {
							_rect.Width = value;
						} else {
							_rect.Width = MinResize;
						}
					}
				}
			}
			/// <summary>
			/// Determine the height of the host based on the ColumnHeaderControl's height.
			/// </summary>
			public int Height {
				get { return _rect.Height; }
				set { _rect.Height = value; }
			}
			/// <summary>
			/// Determine x location of the host in the ColumnHeaderControl.
			/// </summary>
			public int X {
				get { return _rect.X; }
				set { _rect.X = value; }
			}
			/// <summary>
			/// Gets the rightmost location of the host.
			/// </summary>
			public int Right {
				get { return _rect.Right; }
			}
			/// <summary>
			/// Gets the leftmost location of the host.
			/// </summary>
			public int Left {
				get { return _rect.Left; }
			}
			/// <summary>
			/// Gets a value indicating the column has been pressed.
			/// </summary>
			public bool OnMouseDown {
				get { return _onMouseDown; }
			}
			/// <summary>
			/// Gets a value indicating the split column has been pressed.
			/// </summary>
			public bool OnMouseDownSplit {
				get { return _onMouseDownSplit; }
			}
			/// <summary>
			/// Gets a value indicating the mouse pointer moved over the column.
			/// </summary>
			public bool OnHover {
				get { return _onHover; }
			}
			/// <summary>
			/// Gets a value indicating the mouse pointer moved over the column's split.
			/// </summary>
			public bool OnHoverSplit {
				get { return _onHoverSplit; }
			}
			/// <summary>
			/// Gets a ColumnHeader object contained within the host.
			/// </summary>
			public ColumnHeader Column {
				get { return _column; }
			}
			/// <summary>
			/// Minimum size allowed for this column.
			/// </summary>
			public int MinResize {
				get {
					int minWidth = 10;
					if (_column.Image != null)
						minWidth = minWidth + 18;
					if (_column.EnableFiltering & _column.SortOrder != SortOrder.None)
						minWidth = minWidth + 10;
					if (_column.EnableFiltering)
						minWidth = minWidth + 10;
					if (minWidth < 25)
						minWidth = 25;
					return minWidth;
				}
			}
			/// <summary>
			/// Maximum width allowed for this column.
			/// </summary>
			public int MaxResize {
				get {
					int maxWidth = _owner.Width - (_rect.Left + 5);
					if (_owner._owner._vScroll.Visible) {
						maxWidth = maxWidth - _owner._owner._vScroll.Width;
					} else {
						if (_owner._owner._showColumnOptions)
							maxWidth = maxWidth - 10;
					}
					return maxWidth;
				}
			}
			/// <summary>
			/// Gets the filter handle contained in the host.
			/// </summary>
			public ColumnFilterHandle FilterHandler {
				get { return _filterHandler; }
			}
		}
		/// <summary>
		/// Control to display header options in the popup window.
		/// </summary>
		/// <remarks>This control contains two sections, first for column visibility, and second for column frozen.</remarks>
		private class OptionControl : Windows.Forms.Control
		{
			/// <summary>
			/// Host to control item options operations.
			/// </summary>
			private class ItemHost
			{
				Rectangle _rect;
				ColumnHeader _column;
				bool _checked = false;
				int _displayFor = 0;
				bool _onHover = false;
				OptionControl _owner;
				public ItemHost(ColumnHeader column, int displayFor, OptionControl owner)
				{
					_column = column;
					_displayFor = displayFor;
					_owner = owner;
					if (_displayFor == 0) {
						_checked = _column.Visible;
					} else {
						_checked = _column.Frozen;
					}
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
				public ColumnHeader Column {
					get { return _column; }
				}
				/// <summary>
				/// Gets left location of the host.
				/// </summary>
				public int Left {
					get { return _rect.X; }
				}
				/// <summary>
				/// Gets top location of the host.
				/// </summary>
				public int Top {
					get { return _rect.Y; }
				}
				/// <summary>
				/// Gets the rightmost location of the host.
				/// </summary>
				public int Right {
					get { return _rect.Right; }
				}
				/// <summary>
				/// Gets the bottommost location of the host.
				/// </summary>
				public int Bottom {
					get { return _rect.Bottom; }
				}
				/// <summary>
				/// Gets a display functions of the host.
				/// </summary>
				/// <remarks>Returns 0 for column visibility, and 1 for column frozen.</remarks>
				public int DisplayFor {
					get { return _displayFor; }
				}
				/// <summary>
				/// Test whether the mouse pointer is moved over the host.
				/// </summary>
				public bool mouseMove(MouseEventArgs e)
				{
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
				public bool mouseDown()
				{
					if (_onHover) {
						_checked = !_checked;
						return true;
					}
					return false;
				}
				/// <summary>
				/// Test whether the mouse pointer is leave the host.
				/// </summary>
				public bool mouseLeave()
				{
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
				public void draw(Graphics g)
				{
					Rectangle chkRect = default(Rectangle);
					Rectangle txtRect = default(Rectangle);
					chkRect = new Rectangle(_rect.X, _rect.Y, 22, _rect.Height);
					txtRect = new Rectangle(_rect.X + 22, _rect.Y, _rect.Width - 22, _rect.Height);
					if (_onHover) {
						Renderer.Button.draw(g, _rect, , 2, , , , true);
					}
					if (_checked) {
						Renderer.CheckBox.drawCheck(g, chkRect, CheckState.Checked);
					}
					g.DrawString(_column.Text, _owner.Font, Renderer.Drawing.NormalTextBrush, txtRect, _owner.txtFormat);
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
			public OptionControl(ColumnHeaderControl owner, Font font)
			{
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
				foreach (ColumnHost ch in _owner._columnHosts) {
					itemWidth = 0;
					if (ch.Column.EnableHidden) {
						iHost = new ItemHost(ch.Column, 0, this);
						_itemsVisibility.Add(iHost);
						itemWidth = TextRenderer.MeasureText(ch.Column.Text, this.Font).Width + 5;
					}
					if (ch.Column.EnableFrozen) {
						iHost = new ItemHost(ch.Column, 1, this);
						_itemsFreeze.Add(iHost);
						itemWidth = TextRenderer.MeasureText(ch.Column.Text, this.Font).Width + 5;
					}
					if (maxWidth < itemWidth)
						maxWidth = itemWidth;
				}
				maxWidth = maxWidth + 22;
				_vscVisibility.Visible = false;
				_vscFreeze.Visible = false;
				if (_itemsVisibility.Count + _itemsFreeze.Count > 20) {
					_vscVisibility.Visible = _itemsVisibility.Count > 10;
					if (_vscVisibility.Visible)
						_vscVisibility.Maximum = _itemsVisibility.Count - 10;
					_vscFreeze.Visible = _itemsFreeze.Count > 10;
					if (_vscFreeze.Visible)
						_vscFreeze.Maximum = _itemsFreeze.Count - 10;
				}
				if (_vscVisibility.Visible | _vscFreeze.Visible) {
					if (maxWidth + _vscVisibility.Width + 4 < 110)
						maxWidth = 110 - (_vscVisibility.Width + 4);
				} else {
					if (maxWidth < 110)
						maxWidth = 110;
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
						i = i + 1;
					}
					_vscVisibility.Height = max * 20;
					y = y + 1;
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
						i = i + 1;
					}
					_vscFreeze.Height = max * 20;
					y = y + 1;
				}
				if (_vscVisibility.Visible | _vscFreeze.Visible) {
					this.Width = _vscVisibility.Right;
				} else {
					this.Width = maxWidth + 4;
				}
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
			private void vScroll_ValueChanged(object sender, EventArgs e)
			{
				VScrollBar vsc = (VScrollBar)sender;
				int y = vsc.Top;
				int i = 0;
				if (object.ReferenceEquals(sender, _vscVisibility)) {
					while (i < 10) {
						_itemsVisibility[i + vsc.Value].Y = y;
						y = _itemsVisibility[i + vsc.Value].Bottom;
						i = i + 1;
					}
				} else {
					while (i < 10) {
						_itemsFreeze[i + vsc.Value].Y = y;
						y = _itemsFreeze[i + vsc.Value].Bottom;
						i = i + 1;
					}
				}
				this.Invalidate();
			}
			/// <summary>
			/// Change the check state of each section.
			/// </summary>
			private void checkCheckedState()
			{
				int chkCount = 0;
				foreach (ItemHost ih in _itemsVisibility) {
					if (ih.Checked)
						chkCount = chkCount + 1;
				}
				if (chkCount == 0) {
					_chkVisibilityState = CheckState.Unchecked;
				} else if (chkCount == _itemsVisibility.Count) {
					_chkVisibilityState = CheckState.Checked;
				} else {
					_chkVisibilityState = CheckState.Indeterminate;
				}
				chkCount = 0;
				foreach (ItemHost ih in _itemsFreeze) {
					if (ih.Checked)
						chkCount = chkCount + 1;
				}
				if (chkCount == 0) {
					_chkFreezeState = CheckState.Unchecked;
				} else if (chkCount == _itemsFreeze.Count) {
					_chkFreezeState = CheckState.Checked;
				} else {
					_chkFreezeState = CheckState.Indeterminate;
				}
			}
			private void OptionControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Left) {
					bool changed = false;
					int i = 0;
					if (_chkVisibilityHover) {
						if (_chkVisibilityState == CheckState.Indeterminate | _chkVisibilityState == CheckState.Unchecked) {
							_chkVisibilityState = CheckState.Checked;
							foreach (ItemHost ih in _itemsVisibility) {
								ih.Checked = true;
							}
						} else {
							_chkVisibilityState = CheckState.Unchecked;
							foreach (ItemHost ih in _itemsVisibility) {
								ih.Checked = false;
							}
						}
						changed = true;
					}
					if (!changed) {
						if (_vscVisibility.Visible) {
							i = 0;
							while (i < 10) {
								changed = changed | _itemsVisibility[i + _vscVisibility.Visible].mouseDown();
								i = i + 1;
							}
						} else {
							foreach (ItemHost ih in _itemsVisibility) {
								changed = changed | ih.mouseDown();
							}
						}
					}
					if (!changed) {
						if (_chkFreezeHover) {
							if (_chkFreezeState == CheckState.Indeterminate | _chkFreezeState == CheckState.Unchecked) {
								_chkFreezeState = CheckState.Checked;
								foreach (ItemHost ih in _itemsFreeze) {
									ih.Checked = true;
								}
							} else {
								_chkFreezeState = CheckState.Unchecked;
								foreach (ItemHost ih in _itemsFreeze) {
									ih.Checked = false;
								}
							}
							changed = true;
						}
					}
					if (!changed) {
						if (_vscFreeze.Visible) {
							i = 0;
							while (i < 10) {
								changed = changed | _itemsFreeze[i + _vscFreeze.Visible].mouseDown();
								i = i + 1;
							}
						} else {
							foreach (ItemHost ih in _itemsFreeze) {
								changed = changed | ih.mouseDown();
							}
						}
					}
					checkCheckedState();
					if (!changed) {
						if (_btnHover) {
							// Applying all changes and close the popup window.
							foreach (ItemHost ih in _itemsVisibility) {
								ih.Column.Visible = ih.Checked;
							}
							foreach (ItemHost ih in _itemsFreeze) {
								ih.Column.Frozen = ih.Checked;
							}
							_owner._toolStrip.Close();
							return;
						}
					}
					if (changed)
						this.Invalidate();
				}
			}
			private void OptionControl_MouseLeave(object sender, System.EventArgs e)
			{
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
				foreach (ItemHost ih in _itemsVisibility) {
					changed = changed | ih.mouseLeave();
				}
				foreach (ItemHost ih in _itemsFreeze) {
					changed = changed | ih.mouseLeave();
				}
				if (changed)
					this.Invalidate();
			}
			private void OptionControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
			{
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
						changed = changed | _itemsVisibility[i + _vscVisibility.Visible].mouseMove(e);
						i = i + 1;
					}
				} else {
					foreach (ItemHost ih in _itemsVisibility) {
						changed = changed | ih.mouseMove(e);
					}
				}
				if (_vscFreeze.Visible) {
					i = 0;
					while (i < 10) {
						changed = changed | _itemsFreeze[i + _vscFreeze.Visible].mouseMove(e);
						i = i + 1;
					}
				} else {
					foreach (ItemHost ih in _itemsFreeze) {
						changed = changed | ih.mouseMove(e);
					}
				}
				if (changed)
					this.Invalidate();
			}
			private void OptionControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				if (_hoverHost == null)
					return;
				if (_hoverHost.DisplayFor == 0) {
					if (_vscVisibility.Visible) {
						if (e.Delta < 0) {
							if (_vscVisibility.Value > 0)
								_vscVisibility.Value -= 1;
						} else {
							if (_vscVisibility.Value < _vscVisibility.Maximum)
								_vscVisibility.Value += 1;
						}
					}
				} else {
					if (_vscFreeze.Visible) {
						if (e.Delta < 0) {
							if (_vscFreeze.Value > 0)
								_vscFreeze.Value -= 1;
						} else {
							if (_vscFreeze.Value < _vscFreeze.Maximum)
								_vscFreeze.Value += 1;
						}
					}
				}
			}
			private void OptionControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
			{
				StringFormat btnFormat = new StringFormat();
				int i = 0;
				btnFormat.Alignment = StringAlignment.Center;
				btnFormat.LineAlignment = StringAlignment.Center;
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				// Check area
				e.Graphics.Clear(Renderer.Popup.BackgroundBrush.Color);
				e.Graphics.FillRectangle(Renderer.Popup.PlacementBrush, new Rectangle(0, 0, 22, this.Height));
				e.Graphics.DrawLine(Renderer.Popup.SeparatorPen, 22, 0, 22, this.Height);
				if (_itemsVisibility.Count > 0) {
					e.Graphics.FillRectangle(Renderer.Popup.SeparatorBrush, new Rectangle(0, _chkVisibilityRect.Y, this.Width, _chkVisibilityRect.Height));
					e.Graphics.DrawLine(Renderer.Popup.SeparatorPen, 0, _chkVisibilityRect.Bottom, this.Width, _chkVisibilityRect.Bottom);
					Renderer.CheckBox.drawCheckBox(e.Graphics, _chkVisibilityRect, _chkVisibilityState, , , _chkVisibilityHover);
					e.Graphics.DrawString("Visible", this.Font, Renderer.Drawing.NormalTextBrush, _txtVisibilityRect, txtFormat);
					if (_vscVisibility.Visible) {
						while (i < 10) {
							_itemsVisibility[i + _vscVisibility.Value].draw(e.Graphics);
							i = i + 1;
						}
					} else {
						foreach (ItemHost ih in _itemsVisibility) {
							ih.draw(e.Graphics);
						}
					}
				}
				if (_itemsFreeze.Count > 0) {
					e.Graphics.FillRectangle(Renderer.Popup.SeparatorBrush, new Rectangle(0, _chkFreezeRect.Y, this.Width, _chkFreezeRect.Height));
					e.Graphics.DrawLine(Renderer.Popup.SeparatorPen, 0, _chkFreezeRect.Bottom, this.Width, _chkFreezeRect.Bottom);
					Renderer.CheckBox.drawCheckBox(e.Graphics, _chkFreezeRect, _chkFreezeState, , , _chkFreezeHover);
					e.Graphics.DrawString("Freeze", this.Font, Renderer.Drawing.NormalTextBrush, _txtFreezeRect, txtFormat);
					if (_vscFreeze.Visible) {
						i = 0;
						while (i < 10) {
							_itemsFreeze[i + _vscFreeze.Value].draw(e.Graphics);
							i = i + 1;
						}
					} else {
						foreach (ItemHost ih in _itemsFreeze) {
							ih.draw(e.Graphics);
						}
					}
				}
				e.Graphics.FillRectangle(Renderer.Popup.BackgroundBrush, new Rectangle(0, _btnRect.Y - 2, this.Width, this.Height - (_btnRect.Y - 2)));
				e.Graphics.DrawLine(Renderer.Popup.SeparatorPen, 0, _btnRect.Y - 2, this.Width, _btnRect.Y - 2);
				Renderer.Button.draw(e.Graphics, _btnRect, , , , , , _btnHover);
				e.Graphics.DrawString("OK", this.Font, Renderer.Drawing.NormalTextBrush, _btnRect, btnFormat);
				btnFormat.Dispose();
			}
		}
		// CheckBox control
		Rectangle _chkRect;
		bool _chkHover = false;
		System.Windows.Forms.CheckState _chkState = System.Windows.Forms.CheckState.Unchecked;
		// Internal use
		ListView _owner;
		List<ColumnHost> _columnHosts;
		ColumnHost _selectedHost = null;
		private ToolStripDropDown withEventsField__toolStrip;
		public ToolStripDropDown _toolStrip {
			get { return withEventsField__toolStrip; }
			set {
				if (withEventsField__toolStrip != null) {
					withEventsField__toolStrip.Closed -= _toolStrip_Closed;
				}
				withEventsField__toolStrip = value;
				if (withEventsField__toolStrip != null) {
					withEventsField__toolStrip.Closed += _toolStrip_Closed;
				}
			}
		}
		private ToolTip withEventsField__toolTip;
		public ToolTip _toolTip {
			get { return withEventsField__toolTip; }
			set {
				if (withEventsField__toolTip != null) {
					withEventsField__toolTip.Draw -= _toolTip_Draw;
					withEventsField__toolTip.Popup -= _toolTip_Popup;
				}
				withEventsField__toolTip = value;
				if (withEventsField__toolTip != null) {
					withEventsField__toolTip.Draw += _toolTip_Draw;
					withEventsField__toolTip.Popup += _toolTip_Popup;
				}
			}
		}
		int _frozenRight = 0;
		FilterChooser _shownChooser = null;
		// ToolTip
		string _currentToolTip = "";
		string _currentToolTipTitle = "";
		Image _currentToolTipImage = null;
		Rectangle _currentToolTipRect;
		// Column resize
		bool _onColumnResize = false;
		ColumnHost _resizeHost = null;
		int _resizeStartX = -1;
		int _resizeCurrentX = -1;
		// Column reorder
		ColumnHost _reorderHost = null;
		ColumnHost _reorderTarget = null;
		Point _reorderStart;
		Point _reorderCurrent;
		bool _onColumnReorder = false;
		// Column option button
		Rectangle _optRect;
		bool _optHover = false;
		bool _optOnDown = false;
		// Resume painting
		bool _resumePainting = true;
		[Description("Occurs when the column order has been changed.")]
		public event ColumnOrderChangedEventHandler ColumnOrderChanged;
		public delegate void ColumnOrderChangedEventHandler(object sender, ColumnEventArgs e);
		[Description("Occurs when the column filter has been changed.")]
		public event AfterColumnFilterEventHandler AfterColumnFilter;
		public delegate void AfterColumnFilterEventHandler(object sender, ColumnEventArgs e);
		[Description("Occurs when the column custom filter is choosen.")]
		public event AfterColumnCustomFilterEventHandler AfterColumnCustomFilter;
		public delegate void AfterColumnCustomFilterEventHandler(object sender, ColumnEventArgs e);
		[Description("Occurs when the CheckBox checked status has been changed.")]
		public event CheckedChangedEventHandler CheckedChanged;
		public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
		[Description("Occurs when the ColumnHeader width has been changed.")]
		public event AfterColumnResizeEventHandler AfterColumnResize;
		public delegate void AfterColumnResizeEventHandler(object sender, ColumnEventArgs e);
		public ColumnHeaderControl(ListView owner, Font font)
		{
			Resize += ColumnHeaderControl_Resize;
			Paint += ColumnHeaderControl_Paint;
			MouseUp += ColumnHeaderControl_MouseUp;
			MouseMove += ColumnHeaderControl_MouseMove;
			MouseLeave += ColumnHeaderControl_MouseLeave;
			MouseDown += ColumnHeaderControl_MouseDown;
			EnabledChanged += ColumnHeaderControl_EnabledChanged;
			DoubleClick += ColumnHeaderControl_DoubleClick;
			_owner = owner;
			_columnHosts = new List<ColumnHost>();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.Dock = DockStyle.Top;
			this.Font = font;
			this.Height = this.Font.Height + 8;
			_chkRect = new Rectangle(3, Convert.ToInt32((this.Height - 16) / 2), 14, 14);
			_toolTip = new ToolTip(this);
			_toolTip.AnimationSpeed = 20;
			_toolStrip = new ToolStripDropDown(this);
			_toolStrip.SizingGrip = ToolStripDropDown.SizingGripMode.BottomRight;
		}
		/// <summary>
		/// Create a new host for new ColumnHeader an added it to the collection.
		/// </summary>
		/// <param name="aColumn">A ColumnHeader object to be added.</param>
		public void addHost(ColumnHeader aColumn)
		{
			ColumnHost aHost = new ColumnHost(aColumn, this);
			_columnHosts.Add(aHost);
			aColumn._displayIndex = _columnHosts.IndexOf(aHost);
			relocateHosts();
		}
		/// <summary>
		/// Remove a host from collection based on the column contained.
		/// </summary>
		/// <param name="aColumn">A ColumnHeader object contained within the host.</param>
		/// <remarks></remarks>
		public void removeHost(ColumnHeader aColumn)
		{
			int i = 0;
			if (object.ReferenceEquals(_selectedHost.Column, aColumn))
				_selectedHost = null;
			while (i < _columnHosts.Count) {
				if (object.ReferenceEquals(_columnHosts[i].Column, aColumn)) {
					_columnHosts.RemoveAt(i);
				} else {
					i = i + 1;
				}
			}
			foreach (ColumnHost ch in _columnHosts) {
				ch.Column._displayIndex = _columnHosts.IndexOf(ch);
			}
			relocateHosts();
		}
		/// <summary>
		/// Clear all existing hosts.
		/// </summary>
		public void clearHosts()
		{
			_columnHosts.Clear();
			_selectedHost = null;
		}
		/// <summary>
		/// Initialize the bounding rectangle for each column host.
		/// </summary>
		/// <param name="startX">Optional, determine the starting x location of the host of unfrozen columns..</param>
		public void relocateHosts(int startX = 0)
		{
			int availWidth = this.Width - 5;
			int spaceLeft = 1;
			int frozenWidth = 0;
			if (_owner._view == Control.View.Details & _owner._rowNumbers) {
				int numbersWidth = _owner._gObj.MeasureString(Convert.ToString(_owner._items.Count), _owner.Font).Width + 2;
				spaceLeft += numbersWidth;
				availWidth -= numbersWidth;
			}
			if (_owner._checkBoxes) {
				spaceLeft += 22;
				availWidth -= 22;
			}
			if (_owner._view != Control.View.Preview)
				availWidth -= _owner._vScroll.Width;
			// No need to create if no space available.
			if (availWidth > 0) {
				// Create rectangle for frozen columns first.
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Frozen & ch.Column.Visible) {
						switch (ch.Column.SizeType) {
							case ColumnSizeType.Fixed:
								ch.Width = ch.Column.Width;
								break;
							case ColumnSizeType.Auto:
								ch.Width = _owner.getSubItemMaxWidth(ch.Column);
								break;
							case ColumnSizeType.Percentage:
								ch.Width = ch.Column.Width * availWidth / 100;
								break;
						}
						ch.Height = this.Height;
						frozenWidth = ch.Width + 2;
					}
				}
				availWidth -= frozenWidth;
				// Create reactangle for the rest of the visible columns.
				int fillSizeCount = 0;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & !ch.Column.Frozen) {
						switch (ch.Column.SizeType) {
							case ColumnSizeType.Fixed:
								ch.Width = ch.Column.Width;
								break;
							case ColumnSizeType.Auto:
								ch.Width = _owner.getSubItemMaxWidth(ch.Column);
								break;
							case ColumnSizeType.Fill:
								fillSizeCount = fillSizeCount + 1;
								break;
							case ColumnSizeType.Percentage:
								ch.Width = ch.Column.Width * availWidth / 100;
								break;
						}
						ch.Height = this.Height;
						if (ch.Column.SizeType != ColumnSizeType.Fill)
							availWidth = availWidth - (ch.Width + 2);
					}
				}
				if (fillSizeCount > 0) {
					foreach (ColumnHost ch in _columnHosts) {
						if (ch.Column.Visible & ch.Column.SizeType == ColumnSizeType.Fill)
							ch.Width = (availWidth + (2 * fillSizeCount)) / fillSizeCount;
					}
				}
			}
			_optRect = new Rectangle(this.Right - 10, 0, 10, this.Height);
			moveColumns(startX);
		}
		/// <summary>
		/// Move x location of each host of unfrozen columns.
		/// </summary>
		/// <param name="xScroll"></param>
		/// <remarks></remarks>
		public void moveColumns(int xScroll)
		{
			int startX = 1;
			if (_owner._checkBoxes) {
				startX += 22;
			}
			if (_owner._view == Control.View.Details & _owner._rowNumbers) {
				int numbersWidth = _owner._gObj.MeasureString(Convert.ToString(_owner._items.Count), _owner.Font).Width + 2;
				startX += numbersWidth;
			}
			// Move frezed columns
			foreach (ColumnHost ch in _columnHosts) {
				if (ch.Column.Frozen & ch.Column.Visible) {
					ch.X = startX;
					startX = ch.Right + 2;
				}
			}
			_frozenRight = startX;
			// Move the rest of the columns
			startX = _frozenRight;
			startX = startX + xScroll;
			foreach (ColumnHost ch in _columnHosts) {
				if (ch.Column.Visible & !ch.Column.Frozen) {
					ch.X = startX;
					startX = ch.Right + 2;
				}
			}
		}
		/// <summary>
		/// Determine the check state of the checkbox displayed in the ColumnHeaderControl.
		/// </summary>
		/// <remarks>This checkbox represent the Checked state of all the items in ListView.</remarks>
		public Windows.Forms.CheckState CheckState {
			get { return _chkState; }
			set {
				if (_chkState != value) {
					_chkState = value;
					if (_owner._checkBoxes)
						this.Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets a value indicating column resize operation is performed.
		/// </summary>
		[Description("Gets a value indicating column resize operation is performed.")]
		public bool OnColumnResize {
			get { return _onColumnResize; }
		}
		/// <summary>
		/// Gets current column resize position.
		/// </summary>
		[Description("Gets current column resize position.")]
		public int ResizeCurrentX {
			get { return _resizeCurrentX; }
		}
		/// <summary>
		/// Gets total width of the visible columns.
		/// </summary>
		[Description("Gets total width of the visible columns.")]
		public int ColumnsWidth {
			get {
				int result = 0;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible)
						result = result + ch.Width + 2;
				}
				return result;
			}
		}
		/// <summary>
		/// Gets a ColumnHeader specified by its displayed index, and Frozen property is ignored.
		/// </summary>
		/// <param name="index">Displayed index of a ColumnHeader object.</param>
		/// <remarks>The displayed index and the actual display of the columns can be different.
		/// The frozen columns will be shown first.</remarks>
		[Description("Gets a ColumnHeader specified by its displayed index, and Frozen property is ignored.  " + "However, Frozen columns is displayed first than the others.")]
		public ColumnHeader ColumnAt {
			get {
				if (index >= 0 & index < _columnHosts.Count)
					return _columnHosts[index].Column;
				return null;
			}
		}
		/// <summary>
		/// Gets the bounding rectangle of a ColumnHeader.
		/// </summary>
		/// <param name="column">A ColumnHeader object whose the bounding rectangle you want to return.</param>
		[Description("Gets the bounding rectangle of a ColumnHeader.")]
		public Rectangle ColumnRectangle {
			get {
				foreach (ColumnHost ch in _columnHosts) {
					if (object.ReferenceEquals(ch.Column, column))
						return ch.Bounds;
				}
				return new Rectangle(0, 0, 0, 0);
			}
		}
		/// <summary>
		/// Gets total width of all unfrozen and visible columns.
		/// </summary>
		[Description("Gets total width of all unfrozen and visible columns.")]
		public int UnFrozenWidth {
			get {
				int result = 0;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & !ch.Column.Frozen)
						result = result + ch.Width + 2;
				}
				return result;
			}
		}
		/// <summary>
		/// Gets total width of all frozen and visible columns, and reserved area.
		/// </summary>
		[Description("Gets total width of all frozen and visible columns, and reserved area.")]
		public int FrozenWidth {
			get {
				int result = 0;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & ch.Column.Frozen)
						result = result + ch.Width + 2;
				}
				return result;
			}
		}
		/// <summary>
		/// Gets all frozen and visible columns in the collection, sorted by its display index.
		/// </summary>
		[Description("Gets all frozen and visible columns in the collection, sorted by its display index.")]
		public List<ColumnHeader> FrozenColumns {
			get {
				List<ColumnHeader> result = new List<ColumnHeader>();
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & ch.Column.Frozen)
						result.Add(ch.Column);
				}
				return result;
			}
		}
		/// <summary>
		/// Gets all unfrozen and visible columns in the collection, sorted by its display index.
		/// </summary>
		[Description("Gets all unfrozen and visible columns in the collection, sorted by its display index.")]
		public List<ColumnHeader> UnFrozenColumns {
			get {
				List<ColumnHeader> result = new List<ColumnHeader>();
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & !ch.Column.Frozen)
						result.Add(ch.Column);
				}
				return result;
			}
		}
		/// <summary>
		/// Gets an area used in the ColumnHeaderControl to display all visible columns.
		/// </summary>
		[Description("Gets an area used in the ColumnHeaderControl to display all visible columns.")]
		public Rectangle DisplayedRectangle {
			get {
				int leftMost = 0;
				int rightMost = 0;
				bool first = true;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible) {
						if (first) {
							leftMost = ch.Bounds.X;
							first = false;
						} else {
							if (leftMost > ch.Bounds.X)
								leftMost = ch.Bounds.X;
						}
						if (rightMost < ch.Bounds.Right + 2)
							rightMost = ch.Bounds.Right + 2;
					}
				}
				return new Rectangle(leftMost, 0, rightMost - leftMost, this.Height);
			}
		}
		/// <summary>
		/// Gets an area used in the ColumnHeaderControl to display all frozen and visible columns.
		/// </summary>
		[Description("Gets an area used in the ColumnHeaderControl to display all frozen and visible columns.")]
		public Rectangle FrozenRectangle {
			get {
				int leftMost = -1;
				int rightMost = 0;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & ch.Column.Frozen) {
						if (leftMost == -1) {
							leftMost = ch.Bounds.X;
						} else {
							if (leftMost > ch.Bounds.X)
								leftMost = ch.Bounds.X;
						}
						if (rightMost < ch.Bounds.Right + 2)
							rightMost = ch.Bounds.Right + 2;
					}
				}
				return new Rectangle(leftMost, 0, rightMost - leftMost, this.Height);
			}
		}
		/// <summary>
		/// Gets an area used in the ColumnHeaderControl to display all visible and unfrozen columns.
		/// </summary>
		[Description("Gets an area used in the ColumnHeaderControl to display all visible and unfrozen columns.")]
		public Rectangle UnFrozenRectangle {
			get {
				int leftMost = 0;
				int rightMost = 0;
				bool first = true;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible & !ch.Column.Frozen) {
						if (first) {
							leftMost = ch.Bounds.X;
							first = false;
						} else {
							if (leftMost > ch.Bounds.X)
								leftMost = ch.Bounds.X;
						}
						if (rightMost < ch.Bounds.Right + 2)
							rightMost = ch.Bounds.Right + 2;
					}
				}
				return new Rectangle(leftMost, 0, rightMost - leftMost, this.Height);
			}
		}
		/// <summary>
		/// Gets a list of ColumnFilterHandle objects of all ColumnHeader, sorted by ColumnHeader in ListView columns.
		/// </summary>
		public List<ColumnFilterHandle> FilterHandlers {
			get {
				List<ColumnFilterHandle> result = new List<ColumnFilterHandle>();
				foreach (ColumnHeader col in _owner._columns) {
					foreach (ColumnHost ch in _columnHosts) {
						if (object.ReferenceEquals(ch.Column, col)) {
							result.Add(ch.FilterHandler);
							break; // TODO: might not be correct. Was : Exit For
						}
					}
				}
				return result;
			}
		}
		/// <summary>
		/// Gets a ColumnFilterHandle object specified by its ColumnHeader.
		/// </summary>
		[Description("Gets a ColumnFilterHandle object specified by its ColumnHeader.")]
		public ColumnFilterHandle FilterHandler {
			get {
				foreach (ColumnHost ch in _columnHosts) {
					if (object.ReferenceEquals(ch.Column, column))
						return ch.FilterHandler;
				}
				return null;
			}
		}
		// Filter column related
		/// <summary>
		/// Reload filter parameters of a ColumnFilterHandle on specified ColumnHeader using a list of all parameter.
		/// </summary>
		public void reloadFilter(int columnIndex, List<object> objs)
		{
			foreach (ColumnHost ch in _columnHosts) {
				if (_owner.Columns.IndexOf[ch.Column] == columnIndex) {
					ch.FilterHandler.reloadFilter(objs);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		/// <summary>
		/// Add a filter parameter to a ColumnFilterHandle on specified ColumnHeader.
		/// </summary>
		public void addFilter(int columnIndex, object obj)
		{
			foreach (ColumnHost ch in _columnHosts) {
				if (_owner.Columns.IndexOf[ch.Column] == columnIndex) {
					ch.FilterHandler.addFilter(obj);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		/// <summary>
		/// Clear all filter parameters on all columns.
		/// </summary>
		public void clearFilters()
		{
			foreach (ColumnHost ch in _columnHosts) {
				ch.FilterHandler.Items.Clear();
			}
		}
		/// <summary>
		/// Gets displayed index of the specified column.
		/// </summary>
		public int getDisplayedIndex(ColumnHeader column)
		{
			if (column == null)
				return -1;
			int index = 0;
			// Search on frozen columns first.
			foreach (ColumnHost colHost in _columnHosts) {
				if (colHost.Column.Visible & colHost.Column.Frozen) {
					if (object.ReferenceEquals(colHost.Column, column))
						return index;
					index += 1;
				}
			}
			// Search on unfrozen columns.
			foreach (ColumnHost colHost in _columnHosts) {
				if (colHost.Column.Visible & !colHost.Column.Frozen) {
					if (object.ReferenceEquals(colHost.Column, column))
						return index;
					index += 1;
				}
			}
			return -1;
		}
		/// <summary>
		/// Swap the position of two ColumnHost in collection.
		/// </summary>
		private void swapHost(ColumnHost host1, ColumnHost host2)
		{
			ColumnHost tmpHost = host1;
			int tmpX = 0;
			host1 = host2;
			host2 = tmpHost;
			tmpX = host1.X;
			host1.X = host2.X;
			host2.X = tmpX;
			host1.Column._displayIndex = _columnHosts.IndexOf(host1);
			host2.Column._displayIndex = _columnHosts.IndexOf(host2);
		}
		/// <summary>
		/// Show filter options window in a popup window.
		/// </summary>
		private void showFilterPopup(FilterChooser chooser, Rectangle dropdownRect)
		{
			ToolStripControlHost anHost = null;
			Point scrLoc = default(Point);
			chooser.Width = 200;
			anHost = new ToolStripControlHost(chooser);
			anHost.Padding = new Padding(0);
			_toolStrip.Items.Clear();
			_toolStrip.Items.Add(anHost);
			scrLoc = this.PointToScreen(new Point(dropdownRect.Right - 200, this.Height + 2));
			if (scrLoc.X < 0)
				scrLoc = this.PointToScreen(new Point(dropdownRect.X, this.Height + 2));
			if (scrLoc.Y + chooser.Height + 5 > Screen.PrimaryScreen.WorkingArea.Height)
				scrLoc.Y = scrLoc.Y - (chooser.Height + 5 + this.Height);
			_resumePainting = false;
			// Painting the column control before the dropdown window is shown.
			PaintEventArgs pe = null;
			pe = new PaintEventArgs(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
			this.InvokePaint(this, pe);
			pe.Dispose();
			_shownChooser = chooser;
			_toolStrip.Show(scrLoc);
		}
		private void ColumnHeaderControl_DoubleClick(object sender, System.EventArgs e)
		{
			_toolTip.Hide();
			if (this.Cursor == Cursors.VSplit) {
				if (_resizeHost != null) {
					int maxWidth = _owner.getSubItemMaxWidth(_resizeHost.Column);
					if (maxWidth < _resizeHost.MinResize)
						maxWidth = _resizeHost.MinResize;
					_resizeHost.Column.SizeType = ColumnSizeType.Fixed;
					_resizeHost.Column.Width = maxWidth;
				}
			}
		}
		private void ColumnHeaderControl_EnabledChanged(object sender, System.EventArgs e)
		{
			this.Invalidate();
		}
		private void ColumnHeaderControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_toolTip.Hide();
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				if (_optHover) {
					_optOnDown = true;
					this.Invalidate();
					_resumePainting = false;
					// Show columns options
					OptionControl optCtrl = new OptionControl(this, Renderer.ToolTip.TextFont);
					ToolStripControlHost anHost = new ToolStripControlHost(optCtrl);
					Point scrPoint = this.PointToScreen(new Point(_optRect.X, _optRect.Bottom + 1));
					if (scrPoint.X + optCtrl.Width + 6 > Screen.PrimaryScreen.WorkingArea.Width)
						scrPoint.X = scrPoint.X - (optCtrl.Width - 4);
					if (scrPoint.Y + optCtrl.Height + 6 > Screen.PrimaryScreen.WorkingArea.Height)
						scrPoint.Y = scrPoint.Y - (optCtrl.Height + this.Height + 8);
					_toolStrip.Items.Clear();
					_toolStrip.Items.Add(anHost);
					_toolStrip.Show(scrPoint);
					return;
				}
				if (_chkHover) {
					if (_chkState == System.Windows.Forms.CheckState.Indeterminate | _chkState == System.Windows.Forms.CheckState.Unchecked) {
						_chkState = System.Windows.Forms.CheckState.Checked;
					} else {
						_chkState = System.Windows.Forms.CheckState.Unchecked;
					}
					if (_resumePainting)
						this.Invalidate();
					if (CheckedChanged != null) {
						CheckedChanged(this, new EventArgs());
					}
					return;
				}
				if (this.Cursor == Cursors.VSplit) {
					// Start column resizing operation
					_onColumnResize = true;
					_resizeStartX = e.X;
					_resizeCurrentX = e.X;
					if (_resumePainting)
						_owner.Invalidate(true);
					return;
				}
				if (_reorderHost != null) {
					// Start column reordering operation
					_onColumnReorder = true;
					_reorderStart = e.Location;
					_reorderCurrent = e.Location;
					_reorderHost.mouseDown();
					if (_resumePainting)
						this.Invalidate();
					return;
				}
				bool changed = false;
				foreach (ColumnHost columnHost in _columnHosts) {
					if (columnHost.Column.Visible)
						changed = changed | columnHost.mouseDown();
				}
			}
		}
		private void ColumnHeaderControl_MouseLeave(object sender, System.EventArgs e)
		{
			bool changed = false;
			_toolTip.Hide();
			if (_optHover) {
				_optHover = false;
				changed = true;
			}
			if (_chkHover) {
				_chkHover = false;
				changed = true;
			}
			foreach (ColumnHost ch in _columnHosts) {
				if (ch.Column.Visible)
					changed = changed | ch.mouseLeave();
			}
			if (!_onColumnResize) {
				this.Cursor = Cursors.Default;
				_resizeHost = null;
			}
			if (changed & _resumePainting)
				this.Invalidate();
		}
		private void ColumnHeaderControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.None) {
				bool changed = false;
				this.Cursor = Cursors.Default;
				_resizeHost = null;
				_reorderHost = null;
				if (_owner._showColumnOptions) {
					if (_optRect.Contains(e.Location)) {
						if (!_optHover) {
							_optHover = true;
							_currentToolTip = "Show column options.";
							_currentToolTipRect = _optRect;
							changed = true;
						}
					} else {
						if (_optHover) {
							_optHover = false;
							changed = true;
						}
					}
				}
				if (_owner._checkBoxes) {
					if (_chkRect.Contains(e.Location)) {
						if (!_chkHover) {
							_chkHover = true;
							if (_chkState == CheckState.Unchecked | _chkState == CheckState.Indeterminate) {
								_currentToolTip = "Check all items.";
							} else {
								_currentToolTip = "Uncheck all items.";
							}
							_currentToolTipRect = _chkRect;
							changed = true;
						}
					} else {
						if (_chkHover) {
							_chkHover = false;
							changed = true;
						}
					}
				}
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible)
						changed = changed | ch.mouseMove(e);
					if (ch.OnHover & !ch.OnHoverSplit) {
						if (!ch.Column.Frozen)
							_reorderHost = ch;
					}
				}
				_reorderTarget = _reorderHost;
				Rectangle rectColResize = default(Rectangle);
				rectColResize.Size = new Size(2, this.Height);
				rectColResize.Y = 0;
				_onColumnResize = false;
				foreach (ColumnHost ch in _columnHosts) {
					if (ch.Column.Visible) {
						rectColResize.X = ch.Right;
						if (rectColResize.Contains(e.Location)) {
							this.Cursor = Cursors.VSplit;
							_resizeHost = ch;
							break; // TODO: might not be correct. Was : Exit For
						}
					}
				}
				if (changed & Renderer.ToolTip.containsToolTip(_currentToolTipTitle, _currentToolTip, _currentToolTipImage)) {
					_toolTip.Show(this, _currentToolTipRect);
				} else {
					if (changed)
						_toolTip.Hide();
				}
				if (changed & _resumePainting)
					this.Invalidate();
			} else if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				if (this.Cursor == Cursors.VSplit) {
					// Performing resize operation.
					bool changed = false;
					foreach (ColumnHost ch in _columnHosts) {
						if (ch.Column.Visible)
							changed = changed | ch.mouseMove(e);
					}
					int dx = e.X - _resizeStartX;
					int lastX = _resizeCurrentX;
					if (_resizeHost.Width + dx > _resizeHost.MinResize & _resizeHost.Width + dx < _resizeHost.MaxResize) {
						_resizeCurrentX = e.X;
					} else {
						if (_resizeHost.Width + dx < _resizeHost.MinResize) {
							_resizeCurrentX = _resizeStartX - (_resizeHost.Width - _resizeHost.MinResize);
						} else {
							_resizeCurrentX = _resizeStartX + (_resizeHost.MaxResize - _resizeHost.Width);
						}
					}
					changed = changed | (lastX - _resizeCurrentX != 0);
					if (_resumePainting & changed)
						_owner.Invalidate(true);
				} else if (_onColumnReorder) {
					// Performing column reorder operation.
					_reorderTarget = null;
					Rectangle rBound = default(Rectangle);
					foreach (ColumnHost ch in _columnHosts) {
						if (ch.Column.Visible)
							ch.mouseMove(e);
						rBound = ch.Bounds;
						rBound.Width = rBound.Width + 2;
						if (rBound.Contains(e.Location))
							_reorderTarget = ch;
					}
					if (_reorderTarget == null) {
						Point mPoint = new Point(e.X, 10);
						ColumnHost leftHost = null;
						ColumnHost rightHost = null;
						foreach (ColumnHost ch in _columnHosts) {
							if (ch.Column.Visible) {
								if (!ch.Column.Frozen) {
									if (leftHost == null)
										leftHost = ch;
									rightHost = ch;
								}
								rBound = ch.Bounds;
								rBound.Width = rBound.Width + 2;
								if (rBound.Contains(mPoint)) {
									_reorderTarget = ch;
									break; // TODO: might not be correct. Was : Exit For
								}
							}
						}
						if (_reorderTarget == null) {
							if (e.X < leftHost.X)
								_reorderTarget = leftHost;
							if (e.X > rightHost.Right)
								_reorderTarget = rightHost;
						}
					}
					_reorderCurrent = e.Location;
					if (_resumePainting)
						this.Invalidate();
				}
			}
		}
		private void ColumnHeaderControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_onColumnResize) {
				// End of resize operation
				int dx = _resizeCurrentX - _resizeStartX;
				_resizeHost.Column.Width = _resizeHost.Bounds.Width + dx;
				_resizeHost.Column.SizeType = ColumnSizeType.Fixed;
				this.Cursor = Cursors.Default;
				_onColumnResize = false;
				if (_resumePainting)
					_owner.Invalidate(true);
				if (AfterColumnResize != null) {
					AfterColumnResize(this, new ColumnEventArgs(_resizeHost.Column));
				}
				_resizeHost = null;
				return;
			}
			if (_onColumnReorder) {
				// End of reorder operation
				bool colOrderChanged = false;
				_onColumnReorder = false;
				if (_reorderStart != _reorderCurrent) {
					if (!object.ReferenceEquals(_reorderHost, _reorderTarget)) {
						if (!_reorderTarget.Column.Frozen) {
							swapHost(_reorderHost, _reorderTarget);
							colOrderChanged = true;
						}
					}
				} else {
					if (_reorderHost.Column.EnableSorting) {
						switch (_reorderHost.Column.SortOrder) {
							case SortOrder.Descending:
							case SortOrder.None:
								_reorderHost.Column.SortOrder = SortOrder.Ascending;
								break;
							case SortOrder.Ascending:
								_reorderHost.Column.SortOrder = SortOrder.Descending;
								break;
						}
					}
				}
				this.Invalidate();
				if (colOrderChanged)
					if (ColumnOrderChanged != null) {
						ColumnOrderChanged(this, new ColumnEventArgs(_reorderHost.Column));
					}
				_reorderHost = null;
				_reorderTarget = null;
				return;
			}
			bool changed = false;
			foreach (ColumnHost ch in _columnHosts) {
				changed = changed | ch.mouseUp();
			}
			if (changed & _resumePainting)
				this.Invalidate();
		}
		private void ColumnHeaderControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			int xSeparator = 0;
			Rectangle rectHeader = new Rectangle(0, 0, this.Width, this.Height);
			LinearGradientBrush bgBrush = new LinearGradientBrush(rectHeader, Color.Black, Color.White, LinearGradientMode.Vertical);
			bgBrush.InterpolationColors = Renderer.Column.NormalBlend;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			e.Graphics.FillRectangle(bgBrush, rectHeader);
			bgBrush.Dispose();
			// Draw unfrozen columns first
			foreach (ColumnHost ch in _columnHosts) {
				if (ch.Column.Visible & !ch.Column.Frozen) {
					xSeparator = ch.Right;
					ch.draw(e.Graphics);
					e.Graphics.DrawLine(Renderer.Column.NormalBorderPen, xSeparator, 4, xSeparator, this.Height - 5);
					e.Graphics.DrawLine(Pens.White, xSeparator + 1, 4, xSeparator + 1, this.Height - 5);
				}
			}
			// Draw frozen columns
			foreach (ColumnHost ch in _columnHosts) {
				if (ch.Column.Visible & ch.Column.Frozen) {
					xSeparator = ch.Right;
					ch.draw(e.Graphics);
					e.Graphics.DrawLine(Renderer.Column.NormalBorderPen, xSeparator, 4, xSeparator, this.Height - 5);
					e.Graphics.DrawLine(Pens.White, xSeparator + 1, 4, xSeparator + 1, this.Height - 5);
				}
			}
			if (_owner._showColumnOptions) {
				Pen borderPen = Renderer.Column.NormalBorderPen;
				LinearGradientBrush bgOpt = new LinearGradientBrush(_optRect, Color.Black, Color.White, LinearGradientMode.Vertical);
				if (_optHover) {
					if (_optOnDown) {
						bgOpt.InterpolationColors = Renderer.Column.PressedBlend;
					} else {
						bgOpt.InterpolationColors = Renderer.Column.HLitedBlend;
					}
					borderPen = Renderer.Column.ActiveBorderPen;
				} else {
					bgOpt.InterpolationColors = Renderer.Column.NormalBlend;
				}
				e.Graphics.FillRectangle(bgOpt, _optRect);
				bgOpt.Dispose();
				e.Graphics.DrawLine(borderPen, _optRect.X, 0, _optRect.X, this.Height);
				e.Graphics.DrawLine(Pens.White, _optRect.X + 1, 0, _optRect.X + 1, this.Height);
				Renderer.Drawing.drawTriangle(e.Graphics, _optRect, (this.Enabled ? Color.FromArgb(21, 66, 139) : System.Drawing.Color.Gray), Color.White, Renderer.Drawing.TriangleDirection.Down);
				borderPen.Dispose();
			}
			if (_owner._checkBoxes | (_owner._rowNumbers & _owner._view == Control.View.Details)) {
				int reservedWidth = 0;
				if (_owner._checkBoxes)
					reservedWidth += 22;
				if (_owner._rowNumbers & _owner._view == Control.View.Details)
					reservedWidth += e.Graphics.MeasureString(Convert.ToString(_owner._items.Count), _owner.Font).Width + 2;
				Rectangle chkArea = new Rectangle(0, 0, reservedWidth, this.Height);
				Pen borderPen = Renderer.Column.NormalBorderPen;
				LinearGradientBrush bgChk = new LinearGradientBrush(chkArea, Color.Black, Color.White, LinearGradientMode.Vertical);
				bgChk.InterpolationColors = Renderer.Column.NormalBlend;
				e.Graphics.FillRectangle(bgChk, chkArea);
				bgChk.Dispose();
				e.Graphics.DrawLine(borderPen, chkArea.Right - 1, 0, chkArea.Right - 1, this.Height);
				e.Graphics.DrawLine(Pens.White, chkArea.Right, 0, chkArea.Right, this.Height);
				Renderer.CheckBox.drawCheckBox(e.Graphics, _chkRect, _chkState, , this.Enabled, _chkHover);
				borderPen.Dispose();
			}
			if (_onColumnResize) {
				e.Graphics.DrawLine(Pens.Black, _resizeCurrentX, 0, _resizeCurrentX, this.Height);
			}
			if (_onColumnReorder) {
				Rectangle rectMark = _reorderHost.Bounds;
				rectMark.X = rectMark.X + (_reorderCurrent.X - _reorderStart.X);
				_reorderHost.drawMoved(e.Graphics, rectMark, (_reorderTarget.Column.Frozen ? false : true));
			}
			e.Graphics.DrawRectangle(Renderer.Column.NormalBorderPen, 0, 0, this.Width - 1, this.Height - 1);
		}
		private void ColumnHeaderControl_Resize(object sender, System.EventArgs e)
		{
			_optRect = new Rectangle(this.Right - 10, 0, 10, this.Height);
			relocateHosts();
		}
		private void _toolStrip_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e)
		{
			_optOnDown = false;
			_resumePainting = true;
			this.Invalidate();
			if (_shownChooser != null) {
				if (_shownChooser.Result != FilterChooserResult.Cancel) {
					if (_shownChooser.Result == FilterChooserResult.OK) {
						if (AfterColumnFilter != null) {
							AfterColumnFilter(this, new ColumnEventArgs(_shownChooser.FilterHandle.Column));
						}
					} else {
						if (AfterColumnCustomFilter != null) {
							AfterColumnCustomFilter(this, new ColumnEventArgs(_shownChooser.FilterHandle.Column));
						}
					}
				}
				_shownChooser.Dispose();
			}
		}
		private void _toolTip_Draw(object sender, DrawEventArgs e)
		{
			Renderer.ToolTip.drawToolTip(_currentToolTipTitle, _currentToolTip, _currentToolTipImage, e.Graphics, e.Rectangle);
			_currentToolTipTitle = "";
			_currentToolTip = "";
			_currentToolTipImage = null;
		}
		private void _toolTip_Popup(object sender, PopupEventArgs e)
		{
			e.Size = Renderer.ToolTip.measureSize(_currentToolTipTitle, _currentToolTip, _currentToolTipImage);
		}
	}
	/// <summary>
	/// Class to host a ListViewItem object and handles all of its operations, a visual representation on a ListViewItem.
	/// </summary>
	[Description("Class to host a ListViewItem object and handles all of its operations.")]
	private class ListViewItemHost
	{
		/// <summary>
		/// Class to host a ListViewSubItem, a visual representation of a ListViewSubItem, used in Details view.
		/// </summary>
		[Description("Class to host a ListViewSubItem, used in Details view.")]
		private class ListViewSubItemHost
		{
			System.Windows.Forms.ListViewItem.ListViewSubItem _subItem;
			ListViewItemHost _owner;
			Size _displaySize;
			Point _location;
			SizeF _originalSize;
			Rectangle _displayedRect;
			bool _onHover = false;
			bool _isDisplayed = true;
			public ListViewSubItemHost(ListViewItemHost owner, System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
			{
				_owner = owner;
				_subItem = subitem;
			}
			/// <summary>
			/// Gets the display string of a ListViewSubItem value.
			/// </summary>
			[Description("Gets the display string of a ListViewSubItem value.")]
			public string getSubItemString()
			{
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				if (index == 0)
					return _owner._item.Text;
				return _owner._owner.getValueString(_subItem.Value, index);
			}
			/// <summary>
			/// Measure the original size of the ListViewSubItem value.
			/// </summary>
			[Description("Measure the original size of the ListViewSubItem value.")]
			public void measureOriginal()
			{
				string strSubitem = getSubItemString();
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Font font = null;
					if (_owner._item.UseItemStyleForSubItems) {
						font = _owner._item.Font;
					} else {
						font = _subItem.Font;
					}
					_originalSize = _owner._owner._gObj.MeasureString(strSubitem, font);
					if (_originalSize.Width == 0)
						_originalSize.Width = 5;
					if (_originalSize.Height == 0)
						_originalSize.Height = font.Height;
					_originalSize.Width += ListView._subItemMargin * 2;
					_originalSize.Width += 1;
					_originalSize.Height += ListView._subItemMargin * 2;
					if (index == 0 & _owner._item.SmallImage != null)
						_originalSize.Width += 20;
					if (aColumn.Format == ColumnFormat.Check)
						_originalSize.Height = 14 + (2 * ListView._subItemMargin);
				} else {
					_originalSize = new SizeF(0, 0);
				}
			}
			/// <summary>
			/// Measure the displayed size of the ListViewSubItem value.
			/// </summary>
			[Description("Measure the displayed size of the ListViewSubItem value.")]
			public void measureDisplay()
			{
				string strSubitem = getSubItemString();
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Rectangle colRect = _owner._owner._columnControl.ColumnRectangle[aColumn];
					if (index == 0) {
						if (colRect.Width <= Math.Ceiling(_originalSize.Width)) {
							_displaySize = new Size(colRect.Width, _originalSize.Height);
						} else {
							if (aColumn.ColumnAlign != HorizontalAlignment.Left | aColumn.Format == ColumnFormat.Bar | aColumn.Format == ColumnFormat.Check) {
								_displaySize = new Size(colRect.Width, _originalSize.Height);
							} else {
								_displaySize = new Size(Math.Ceiling(_originalSize.Width) + 1, _originalSize.Height);
							}
						}
					} else {
						if (colRect.Width <= _originalSize.Width) {
							_displaySize = new Size(colRect.Width, _originalSize.Height);
						} else {
							if (aColumn.ColumnAlign != HorizontalAlignment.Left | aColumn.Format == ColumnFormat.Bar) {
								_displaySize = new Size(colRect.Width, _originalSize.Height);
							} else {
								_displaySize = new Size(_originalSize.Width, _originalSize.Height);
							}
						}
					}
					_displaySize.Height += 1;
					if (!_owner._owner._allowMultiline) {
						if (strSubitem.IndexOf(Constants.vbCr) > -1) {
							Font font = _subItem.Font;
							if (_owner._item.UseItemStyleForSubItems)
								font = _owner._item.Font;
							_displaySize.Height = font.Height + (2 * ListView._subItemMargin);
						}
					}
					_displayedRect.X = colRect.X;
					_displayedRect.Width = _displaySize.Width;
					_displayedRect.Height = _displaySize.Height;
					_isDisplayed = aColumn.Visible;
				} else {
					_displaySize = new Size(0, 0);
					_isDisplayed = false;
				}
			}
			/// <summary>
			/// Move x location of displayed rectangle based on the related column.
			/// </summary>
			[Description("Move x location of displayed rectangle based on the related column.")]
			public void moveX()
			{
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Rectangle colRect = _owner._owner._columnControl.ColumnRectangle[aColumn];
					_displayedRect.X = colRect.X;
				}
			}
			/// <summary>
			/// Determine whether mouse pointer is hover on this ListViewSubItem.
			/// </summary>
			[Description("Determine whether mouse pointer is hover on this ListViewSubItem.")]
			public bool OnHover {
				get { return _onHover; }
				set { _onHover = value; }
			}
			/// <summary>
			/// Determine y location of the host.
			/// </summary>
			public int Y {
				get { return _displayedRect.Y; }
				set { _displayedRect.Y = value; }
			}
			/// <summary>
			/// Gets the original size of ListViewSubItem.
			/// </summary>
			[Description("Gets the original size of ListViewSubItem.")]
			public SizeF OriginalSize {
				get { return _originalSize; }
			}
			/// <summary>
			/// Gets the displayed size of ListViewSubItem.
			/// </summary>
			[Description("Gets the displayed size of ListViewSubItem.")]
			public Size DisplayedSize {
				get { return _displaySize; }
			}
			/// <summary>
			/// Gets the displayed rectangle of ListViewSubItem.
			/// </summary>
			[Description("Gets the displayed rectangle of ListViewSubItem.")]
			public Rectangle Bounds {
				get { return _displayedRect; }
			}
			/// <summary>
			/// Draw ListViewSubItem object on specified graphics object, and optionally draw its background.
			/// </summary>
			[Description("Draw ListViewSubItem object on specified graphics object, and optionally draw its background.")]
			public void draw(Graphics g, bool drawBackground = false, bool selected = false)
			{
				string strSubitem = getSubItemString();
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn == null)
					return;
				if (!aColumn.Visible)
					return;
				Rectangle rect = _owner._owner._columnControl.ColumnRectangle[aColumn];
				rect.Y = _displayedRect.Y;
				rect.Height = _owner._size.Height;
				if (rect.X > _owner._owner._clientArea.Right | rect.Y > _owner._owner._clientArea.Bottom)
					return;
				if (rect.Right < _owner._owner._clientArea.X | rect.Bottom < _owner._owner._clientArea.Y)
					return;
				if (drawBackground) {
					if (_onHover | selected) {
						Rectangle rectBg = _displayedRect;
						rectBg.Height = _owner._size.Height;
						LinearGradientBrush hoverBrush = new LinearGradientBrush(rectBg, System.Drawing.Color.Black, System.Drawing.Color.White, LinearGradientMode.Vertical);
						GraphicsPath hoverPath = Renderer.Drawing.roundedRectangle(rectBg, 2, 2, 2, 2);
						Pen borderPen = null;
						if (selected) {
							if (_onHover) {
								borderPen = Renderer.ListItem.SelectedHLiteBorderPen;
								hoverBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
							} else {
								if (_owner._owner.Focused) {
									borderPen = Renderer.ListItem.SelectedBorderPen;
									hoverBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
								} else {
									borderPen = Renderer.ListItem.SelectedBlurBorderPen;
									hoverBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
								}
							}
						} else {
							borderPen = Renderer.ListItem.HLitedBorderPen;
							hoverBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
						}
						g.FillPath(hoverBrush, hoverPath);
						g.DrawPath(borderPen, hoverPath);
						borderPen.Dispose();
						hoverPath.Dispose();
						hoverBrush.Dispose();
					} else {
						if (_owner._item.UseItemStyleForSubItems) {
							g.FillRectangle(new SolidBrush(_owner._item.BackColor), rect);
						} else {
							g.FillRectangle(new SolidBrush(_subItem.BackColor), rect);
						}
					}
				}
				if (aColumn.Format == Control.ColumnFormat.Check) {
					Rectangle rectCheck = new Rectangle(0, 0, 14, 14);
					Rectangle rectValue = _displayedRect;
					bool checkValue = Convert.ToBoolean(_subItem.Value);
					rectValue.Height = _owner._size.Height;
					rectCheck.Y = rectValue.Y + ((rectValue.Height - 14) / 2);
					switch (aColumn.ColumnAlign) {
						case HorizontalAlignment.Center:
							rectCheck.X = rectValue.X + ((rectValue.Width - 14) / 2);
							break;
						case HorizontalAlignment.Left:
							rectCheck.X = rectValue.X + ListView._subItemMargin;
							break;
						case HorizontalAlignment.Right:
							rectCheck.X = rectValue.Right - (ListView._subItemMargin + 1);
							break;
					}
					if (checkValue) {
						Renderer.CheckBox.drawCheck(g, rectCheck, CheckState.Checked, , _owner._owner.Enabled);
					}
				} else {
					System.Drawing.Color color = (_owner._owner.Enabled ? (_owner._item.UseItemStyleForSubItems ? _owner._item.Color : _subItem.Color) : System.Drawing.Color.Gray);
					Font font = (_owner._item.UseItemStyleForSubItems ? _owner._item.Font : _subItem.Font);
					StringFormat strFormat = new StringFormat();
					ColumnFormat columnFormat = (index == 0 ? Control.ColumnFormat.None : aColumn.Format);
					Rectangle rectValue = _displayedRect;
					rectValue.Height = _owner._size.Height;
					if (index == 0 & _owner._item.SmallImage != null) {
						Rectangle rectImg = new Rectangle(rectValue.Location, new Size(20, rectValue.Height));
						rectImg = Renderer.Drawing.getImageRectangle(_owner._item.SmallImage, rectImg, 16);
						if (_owner._owner.Enabled) {
							g.DrawImage(_owner._item.SmallImage, rectImg);
						} else {
							Renderer.Drawing.grayscaledImage(_owner._item.SmallImage, rectImg, g);
						}
						rectValue.X += 20;
						rectValue.Width -= 20;
					}
					rectValue.X += ListView._subItemMargin;
					rectValue.Width -= ListView._subItemMargin * 2;
					strFormat.Trimming = StringTrimming.EllipsisCharacter;
					strFormat.LineAlignment = StringAlignment.Center;
					switch (aColumn.ColumnAlign) {
						case HorizontalAlignment.Center:
							strFormat.Alignment = StringAlignment.Center;
							break;
						case HorizontalAlignment.Left:
							strFormat.Alignment = StringAlignment.Near;
							break;
						case HorizontalAlignment.Right:
							strFormat.Alignment = StringAlignment.Far;
							break;
					}
					strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.NoWrap;
					if (!_owner._owner._allowMultiline)
						strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
					if (columnFormat == Control.ColumnFormat.Bar)
						_owner.drawBar(g, new Rectangle(rectValue.X, rectValue.Y + 2, rectValue.Width, rectValue.Height - 4), _subItem, aColumn);
					if (columnFormat != Control.ColumnFormat.Bar | _subItem.PrintValueOnBar)
						g.DrawString(strSubitem, font, new SolidBrush(color), rectValue, strFormat);
					strFormat.Dispose();
				}
			}
			/// <summary>
			/// Draw ListViewSubItem object on specified graphics object, bounding rectangle, and format.
			/// </summary>
			[Description("Draw ListViewSubItem object on specified graphics object, bounding rectangle, and format.")]
			public void draw(Graphics g, Rectangle rect, StringFormat format)
			{
				string strSubitem = getSubItemString();
				int index = _owner._item.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn == null)
					return;
				System.Drawing.Color color = (_owner._owner.Enabled ? (_owner._item.UseItemStyleForSubItems ? _owner._item.Color : _subItem.Color) : System.Drawing.Color.Gray);
				Font font = (_owner._item.UseItemStyleForSubItems ? _owner._item.Font : _subItem.Font);
				ColumnFormat columnFormat = (index == 0 ? Control.ColumnFormat.None : aColumn.Format);
				if (columnFormat == Control.ColumnFormat.Bar)
					_owner.drawBar(g, new Rectangle(rect.X, rect.Y, rect.Width - 5, rect.Height), _subItem, aColumn);
				if (columnFormat != Control.ColumnFormat.Bar | _subItem.PrintValueOnBar)
					g.DrawString(strSubitem, font, new SolidBrush(color), rect, format);
			}
			// Mouse event
			/// <summary>
			/// Test whether the mouse pointer moves over the host.
			/// </summary>
			[Description("Test whether the mouse pointer moves over the host.")]
			public bool mouseMove(MouseEventArgs e)
			{
				if (!_isDisplayed)
					return false;
				Rectangle rectSubItem = _displayedRect;
				rectSubItem.Height = _owner._size.Height;
				if (rectSubItem.Contains(e.Location) & e.X > _owner._owner._clientArea.X & e.Y > _owner._owner._clientArea.Y) {
					if (!_onHover) {
						_onHover = true;
						return true;
					}
				} else {
					if (_onHover) {
						_onHover = false;
						return true;
					}
				}
				return false;
			}
			/// <summary>
			/// Test whether the mouse pointer leaving the host.
			/// </summary>
			public bool mouseLeave()
			{
				if (_onHover) {
					_onHover = false;
					return true;
				}
				return false;
			}
		}
		ListView _owner;
		ListViewItem _item;
		Point _location;
		Size _size;
		bool _onMouseDown = false;
		bool _selected = false;
		bool _onHover = false;
		bool _visible = true;
		bool _frozen = false;
		// Checkbox
		bool _onHoverCheck = false;
		Rectangle _checkRect;
		// SubItem hosts
		List<ListViewSubItemHost> _subItemHosts = new List<ListViewSubItemHost>();
		ListViewGroupHost _groupHost = null;
		public ListViewItemHost(ListViewItem item, ListView owner)
		{
			_owner = owner;
			_item = item;
			if (_item.Checked)
				_owner._checkedItems.Add(_item);
			if (_item.Group != null) {
				_item.Group._checkedItems.Add(_item);
			}
			refreshSubItem();
		}
		// Sub item
		/// <summary>
		/// Refresh ListViewSubItemHost contained in this host.
		/// </summary>
		[Description("Refresh ListViewSubItemHost contained in this host.")]
		public void refreshSubItem()
		{
			_subItemHosts.Clear();
			int i = 0;
			while (i <= _item.SubItems.Count) {
				ListViewSubItemHost aHost = new ListViewSubItemHost(this, _item.SubItems[i]);
				aHost.measureOriginal();
				_subItemHosts.Add(aHost);
				i = i + 1;
			}
		}
		/// <summary>
		/// Relocate all available ListViewSubItemHost.
		/// </summary>
		public void relocateSubItems()
		{
			foreach (ListViewSubItemHost lvsiHost in _subItemHosts) {
				lvsiHost.moveX();
			}
		}
		// Drawing
		/// <summary>
		/// Draw bar chart of a ListViewSubItem with associated column header.
		/// </summary>
		[Description("Draw bar chart of a ListViewSubItem with associated column header.")]
		private void drawBar(Graphics g, Rectangle rect, System.Windows.Forms.ListViewItem.ListViewSubItem subItem, ColumnHeader column)
		{
			if (column.MaximumValue > column.MinimumValue) {
				// Preparing bar area
				Rectangle rectBar = rect;
				GraphicsPath pathBar = null;
				rectBar.Y = rectBar.Y + 1;
				rectBar.Height = rectBar.Height - 3;
				pathBar = Renderer.Drawing.roundedRectangle(rectBar, 2, 2, 2, 2);
				// Convert subitem's value to Double
				try {
					double subItemValue = Convert.ToDouble(subItem.Value);
					double columnRange = column.MaximumValue - column.MinimumValue;
					Rectangle rectValue = rectBar;
					double valueRange = subItemValue - column.MinimumValue;
					if (valueRange >= 0 & valueRange <= columnRange) {
						rectValue.Width = Math.Ceiling(valueRange * rectBar.Width / columnRange);
						Region valueRegion = new Region(pathBar);
						valueRegion.Intersect(rectValue);
						LinearGradientBrush valueGlowBrush = new LinearGradientBrush(rectValue, Color.Black, Color.White, LinearGradientMode.Vertical);
						valueGlowBrush.InterpolationColors = Renderer.Drawing.BarGlow;
						if (_owner.Enabled) {
							g.FillRegion(new SolidBrush(subItem.Color), valueRegion);
						} else {
							g.FillRegion(Brushes.Gray, valueRegion);
						}
						g.FillRegion(valueGlowBrush, valueRegion);
						valueRegion.Dispose();
						valueGlowBrush.Dispose();
					}
				} catch (Exception ex) {
				}
				g.DrawPath(Pens.Black, pathBar);
				pathBar.Dispose();
			}
		}
		/// <summary>
		/// Draw ListViewSubItems that associated with all unfrozen columns.
		/// </summary>
		[Description("Draw ListViewSubItems that associated with all unfrozen columns.")]
		public void drawUnFrozen(Graphics g, int frozenCount)
		{
			if (!_visible)
				return;
			if (_location.X > _owner.Width | _location.Y > _owner.Height)
				return;
			if (this.Bottom < 0 | this.Right < 0)
				return;
			if (_owner._showGroups & _groupHost.Group.IsCollapsed)
				return;
			if (_owner._fullRowSelect) {
				if (_selected | _onHover) {
					Rectangle unfrozenRect = _owner._columnControl.UnFrozenRectangle;
					unfrozenRect.Y = _location.Y;
					unfrozenRect.Height = _size.Height;
					GraphicsPath unfrozenPath = Renderer.Drawing.roundedRectangle(unfrozenRect, (frozenCount > 0 ? 0 : 2), 2, (frozenCount > 0 ? 0 : 2), 2);
					LinearGradientBrush unfrozenBrush = new LinearGradientBrush(unfrozenRect, Color.Black, Color.White, LinearGradientMode.Vertical);
					Pen unfrozenPen = null;
					if (_onMouseDown) {
						unfrozenBrush.InterpolationColors = Renderer.ListItem.PressedBlend;
						unfrozenPen = Renderer.ListItem.PressedBorderPen;
					} else {
						if (_selected) {
							if (_onHover) {
								unfrozenBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
								unfrozenPen = Renderer.ListItem.SelectedHLiteBorderPen;
							} else {
								if (_owner.Focused) {
									unfrozenBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
									unfrozenPen = Renderer.ListItem.SelectedBorderPen;
								} else {
									unfrozenBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
									unfrozenPen = Renderer.ListItem.SelectedBlurBorderPen;
								}
							}
						} else {
							unfrozenBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
							unfrozenPen = Renderer.ListItem.HLitedBorderPen;
						}
					}
					g.FillPath(unfrozenBrush, unfrozenPath);
					g.DrawPath(unfrozenPen, unfrozenPath);
					unfrozenBrush.Dispose();
					unfrozenPen.Dispose();
				}
			}
			bool first = frozenCount == 0;
			foreach (ColumnHeader ch in _owner._columnControl.UnFrozenColumns) {
				int colIndex = _owner._columns.IndexOf[ch];
				if (colIndex >= 0 & colIndex < _subItemHosts.Count) {
					_subItemHosts[colIndex].draw(g, !_owner._fullRowSelect, first & _selected);
					first = false;
				}
			}
		}
		/// <summary>
		/// Draw ListViewSubItems that associated with all frozen columns.
		/// </summary>
		[Description("Draw ListViewSubItems that associated with all frozen columns.")]
		public void drawFrozen(Graphics g, int unfrozenCount)
		{
			if (!_visible)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			if (this.Bottom < 0 | this.Right < 0)
				return;
			if (_owner._showGroups & _groupHost.Group.IsCollapsed)
				return;
			if (_owner._fullRowSelect) {
				if (_selected | _onHover) {
					Rectangle frozenRect = _owner._columnControl.FrozenRectangle;
					frozenRect.Y = _location.Y;
					frozenRect.Height = _size.Height;
					frozenRect.Width -= 1;
					GraphicsPath frozenPath = Renderer.Drawing.roundedRectangle(frozenRect, 2, (unfrozenCount > 0 ? 0 : 2), 2, (unfrozenCount > 0 ? 0 : 2));
					LinearGradientBrush frozenBrush = new LinearGradientBrush(frozenRect, Color.Black, Color.White, LinearGradientMode.Vertical);
					Pen frozenPen = null;
					if (_onMouseDown) {
						frozenBrush.InterpolationColors = Renderer.ListItem.PressedBlend;
						frozenPen = Renderer.ListItem.PressedBorderPen;
					} else {
						if (_selected) {
							if (_onHover) {
								frozenBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
								frozenPen = Renderer.ListItem.SelectedHLiteBorderPen;
							} else {
								if (_owner.Focused) {
									frozenBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
									frozenPen = Renderer.ListItem.SelectedBorderPen;
								} else {
									frozenBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
									frozenPen = Renderer.ListItem.SelectedBlurBorderPen;
								}
							}
						} else {
							frozenBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
							frozenPen = Renderer.ListItem.HLitedBorderPen;
						}
					}
					g.FillPath(frozenBrush, frozenPath);
					g.DrawPath(frozenPen, frozenPath);
					frozenBrush.Dispose();
					frozenPen.Dispose();
				}
			}
			bool first = true;
			foreach (ColumnHeader ch in _owner._columnControl.FrozenColumns) {
				int colIndex = _owner._columns.IndexOf[ch];
				if (colIndex >= 0 & colIndex < _subItemHosts.Count) {
					_subItemHosts[colIndex].draw(g, !_owner._fullRowSelect, first & _selected);
					first = false;
				}
			}
		}
		/// <summary>
		/// Draw ListViewItem on List view mode.
		/// </summary>
		[Description("Draw ListViewItem on List view mode.")]
		public void drawList(Graphics g, bool drawCheckBox = false)
		{
			if (!_visible)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			Rectangle itemRect = new Rectangle(_location, _size);
			if (itemRect.Right < _owner._clientArea.X | itemRect.Bottom < _owner._clientArea.Y)
				return;
			Rectangle textRect = itemRect;
			Rectangle imgRect = new Rectangle(0, 0, 0, 0);
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Near;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.NoWrap;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			if (_owner._checkBoxes) {
				textRect.X += 22;
				textRect.Width -= 22;
			}
			imgRect = textRect;
			imgRect.Width = 20;
			imgRect = Renderer.Drawing.getImageRectangle(_item.SmallImage, imgRect, 16);
			textRect.X += 20;
			textRect.Width -= 20;
			if (_selected | _onHover) {
				GraphicsPath itemPath = Renderer.Drawing.roundedRectangle(itemRect, 2, 2, 2, 2);
				LinearGradientBrush itemBrush = new LinearGradientBrush(itemRect, Color.Black, Color.White, LinearGradientMode.Vertical);
				Pen itemPen = null;
				if (_onMouseDown) {
					itemBrush.InterpolationColors = Renderer.ListItem.PressedBlend;
					itemPen = Renderer.ListItem.PressedBorderPen;
				} else {
					if (_selected) {
						if (_onHover) {
							itemBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
							itemPen = Renderer.ListItem.SelectedHLiteBorderPen;
						} else {
							if (_owner.Focused) {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
								itemPen = Renderer.ListItem.SelectedBorderPen;
							} else {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
								itemPen = Renderer.ListItem.SelectedBlurBorderPen;
							}
						}
					} else {
						itemBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
						itemPen = Renderer.ListItem.HLitedBorderPen;
					}
				}
				g.FillPath(itemBrush, itemPath);
				g.DrawPath(itemPen, itemPath);
				itemBrush.Dispose();
				itemPen.Dispose();
			} else {
				g.FillRectangle(new SolidBrush(_item.BackColor), itemRect);
			}
			if ((_onHover | _selected | _item.Checked) & _owner._checkBoxes)
				Renderer.CheckBox.drawCheckBox(g, _checkRect, (_item.Checked ? CheckState.Checked : CheckState.Unchecked), , _owner.Enabled, _onHoverCheck);
			if (_item.SmallImage != null) {
				if (_owner.Enabled) {
					g.DrawImage(_item.SmallImage, imgRect);
				} else {
					Renderer.Drawing.grayscaledImage(_item.SmallImage, imgRect, g);
				}
			}
			g.DrawString(_item.Text, _item.Font, (_owner.Enabled ? new SolidBrush(_item.Color) : Brushes.Gray), textRect, strFormat);
			strFormat.Dispose();
		}
		/// <summary>
		/// Draw ListViewItem on Tile view mode.
		/// </summary>
		[Description("Draw ListViewItem on Tile view mode.")]
		public void drawTile(Graphics g, bool drawCheckBox = false)
		{
			if (!_visible)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			Rectangle itemRect = new Rectangle(_location, _size);
			if (itemRect.Right < _owner._clientArea.X | itemRect.Bottom < _owner._clientArea.Y)
				return;
			Rectangle textRect = itemRect;
			Rectangle imgRect = new Rectangle(0, 0, 0, 0);
			//If _item.TileImage IsNot Nothing Then
			//    imgRect = textRect
			//    imgRect.Width = _tileHeight
			//    imgRect = Renderer.Drawing.getImageRectangle(_item.TileImage, imgRect, _tileHeight - 16)
			//    textRect.X += _tileHeight
			//    textRect.Width -= _tileHeight
			//End If
			imgRect = textRect;
			imgRect.Width = ListView._tileHeight;
			imgRect = Renderer.Drawing.getImageRectangle(_item.TileImage, imgRect, ListView._tileHeight - 16);
			textRect.X += ListView._tileHeight;
			textRect.Width -= ListView._tileHeight;
			if (_selected | _onHover) {
				GraphicsPath itemPath = Renderer.Drawing.roundedRectangle(itemRect, 2, 2, 2, 2);
				LinearGradientBrush itemBrush = new LinearGradientBrush(itemRect, Color.Black, Color.White, LinearGradientMode.Vertical);
				Pen itemPen = null;
				if (_onMouseDown) {
					itemBrush.InterpolationColors = Renderer.ListItem.PressedBlend;
					itemPen = Renderer.ListItem.PressedBorderPen;
				} else {
					if (_selected) {
						if (_onHover) {
							itemBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
							itemPen = Renderer.ListItem.SelectedHLiteBorderPen;
						} else {
							if (_owner.Focused) {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
								itemPen = Renderer.ListItem.SelectedBorderPen;
							} else {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
								itemPen = Renderer.ListItem.SelectedBlurBorderPen;
							}
						}
					} else {
						itemBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
						itemPen = Renderer.ListItem.HLitedBorderPen;
					}
				}
				g.FillPath(itemBrush, itemPath);
				g.DrawPath(itemPen, itemPath);
				itemBrush.Dispose();
				itemPen.Dispose();
			} else {
				g.FillRectangle(new SolidBrush(_item.BackColor), itemRect);
			}
			// Drawing the image
			if (_item.TileImage != null) {
				if (_owner.Enabled) {
					g.DrawImage(_item.TileImage, imgRect);
				} else {
					Renderer.Drawing.grayscaledImage(_item.TileImage, imgRect, g);
				}
			}
			int maxIndex = 0;
			int heightUsed = 0;
			StringFormat strFormat = new StringFormat();
			Font font = null;
			SizeF strSize = default(SizeF);
			string strSubItem = "";
			int linesCount = 0;
			int itemLines = 0;
			int charFitted = 0;
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Near;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			while (maxIndex < _subItemHosts.Count & maxIndex < _owner._columns.Count & heightUsed < _size.Height & linesCount < 3) {
				if (_item.UseItemStyleForSubItems) {
					font = _item.Font;
				} else {
					font = _item.SubItems[maxIndex].Font;
				}
				if (maxIndex == 0) {
					strSubItem = _item.Text;
				} else {
					strSubItem = _owner.getValueString(_item.SubItems[maxIndex].Value, maxIndex);
				}
				if (!string.IsNullOrEmpty(strSubItem)) {
					strSize = _owner._gObj.MeasureString(strSubItem, font, new SizeF(textRect.Width, font.Height * (maxIndex == 0 ? 2 : 1)), strFormat, out charFitted, out itemLines);
					if (heightUsed + strSize.Height > _size.Height)
						break; // TODO: might not be correct. Was : Exit While
					heightUsed += strSize.Height;
					linesCount += itemLines;
				}
				maxIndex += 1;
				if (maxIndex > 0)
					strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.NoWrap;
			}
			maxIndex -= 1;
			if (maxIndex < 0)
				maxIndex = 0;
			// At least, item's text will be drawn, if min 1 column exists.
			// Drawing ListViewSubItem
			strFormat.Dispose();
			// Recreating string format
			strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Near;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			int i = 0;
			int y = _location.Y + ((_size.Height - heightUsed) / 2);
			while (i <= maxIndex) {
				if (_item.UseItemStyleForSubItems) {
					font = _item.Font;
				} else {
					font = _item.SubItems[maxIndex].Font;
				}
				if (i == 0) {
					strSubItem = _item.Text;
				} else {
					strSubItem = _owner.getValueString(_item.SubItems[i].Value, i);
				}
				if (!string.IsNullOrEmpty(strSubItem)) {
					strSize = _owner._gObj.MeasureString(strSubItem, font, new SizeF(textRect.Width, font.Height * (i == 0 ? 2 : 1)), strFormat);
					_subItemHosts[i].draw(g, new Rectangle(textRect.X, y, textRect.Width, strSize.Height), strFormat);
					y += strSize.Height;
				}
				i += 1;
				if (i > 0)
					strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.NoWrap;
			}
			strFormat.Dispose();
			if ((_onHover | _selected | _item.Checked) & _owner._checkBoxes)
				Renderer.CheckBox.drawCheckBox(g, _checkRect, (_item.Checked ? CheckState.Checked : CheckState.Unchecked), , _owner.Enabled, _onHoverCheck);
		}
		/// <summary>
		/// Draw ListViewItem on Icon or Thumbnail view mode.
		/// </summary>
		[Description("Draw ListViewItem on Icon or Thumbnail view mode.")]
		public void drawIconThumbnail(Graphics g, bool drawCheckBox = false)
		{
			if (!_visible)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			Rectangle itemRect = new Rectangle(_location, _size);
			int imgSize = (_owner._view == Control.View.Icon ? _owner._iconSize : _owner._thumbnailSize);
			if (_selected) {
				SizeF textSize = getTextSize(_size.Width - (2 * ListView._subItemMargin), ListView._maxTextHeight);
				if (imgSize + 10 + ListView._subItemMargin + textSize.Height > _size.Height)
					itemRect.Height = imgSize + 10 + ListView._subItemMargin + textSize.Height;
			}
			if (itemRect.Right < _owner._clientArea.X | itemRect.Bottom < _owner._clientArea.Y)
				return;
			Rectangle textRect = new Rectangle(itemRect.X + ListView._subItemMargin, _location.Y + imgSize + 10, itemRect.Width - (2 * ListView._subItemMargin), itemRect.Height - (imgSize + 10 + ListView._subItemMargin));
			Rectangle imgRect = new Rectangle(itemRect.X + 10, itemRect.Y + 5, imgSize, imgSize);
			Image img = null;
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			if (_owner._view == Control.View.Icon) {
				img = _item.TileImage;
			} else {
				img = _item.ThumbnailImage;
			}
			if (img != null) {
				imgRect = Renderer.Drawing.getImageRectangle(img, imgRect, imgSize);
			}
			if (_selected | _onHover) {
				GraphicsPath itemPath = Renderer.Drawing.roundedRectangle(itemRect, 2, 2, 2, 2);
				LinearGradientBrush itemBrush = new LinearGradientBrush(itemRect, Color.Black, Color.White, LinearGradientMode.Vertical);
				Pen itemPen = null;
				if (_onMouseDown) {
					itemBrush.InterpolationColors = Renderer.ListItem.PressedBlend;
					itemPen = Renderer.ListItem.PressedBorderPen;
				} else {
					if (_selected) {
						if (_onHover) {
							itemBrush.InterpolationColors = Renderer.ListItem.SelectedHLiteBlend;
							itemPen = Renderer.ListItem.SelectedHLiteBorderPen;
						} else {
							if (_owner.Focused) {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlend;
								itemPen = Renderer.ListItem.SelectedBorderPen;
							} else {
								itemBrush.InterpolationColors = Renderer.ListItem.SelectedBlurBlend;
								itemPen = Renderer.ListItem.SelectedBlurBorderPen;
							}
						}
					} else {
						itemBrush.InterpolationColors = Renderer.ListItem.HLitedBlend;
						itemPen = Renderer.ListItem.HLitedBorderPen;
					}
				}
				g.FillPath(itemBrush, itemPath);
				g.DrawPath(itemPen, itemPath);
				itemBrush.Dispose();
				itemPen.Dispose();
			} else {
				g.FillRectangle(new SolidBrush(_item.BackColor), itemRect);
			}
			if (img != null) {
				if (_owner.Enabled) {
					g.DrawImage(img, imgRect);
				} else {
					Renderer.Drawing.grayscaledImage(img, imgRect, g);
				}
			}
			g.DrawString(_item.Text, _item.Font, (_owner.Enabled ? new SolidBrush(_item.Color) : Brushes.Gray), textRect, strFormat);
			if ((_onHover | _selected | _item.Checked) & _owner._checkBoxes)
				Renderer.CheckBox.drawCheckBox(g, _checkRect, (_item.Checked ? CheckState.Checked : CheckState.Unchecked), , _owner.Enabled, _onHoverCheck);
		}
		/// <summary>
		/// Draw ListViewItem on Preview view mode.
		/// </summary>
		[Description("Draw ListViewItem on Preview view mode.")]
		public void drawPreview(Graphics g, bool drawCheckBox = false)
		{
			if (!_visible)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			Rectangle itemRect = new Rectangle(_location, _size);
			int imgSize = 48;
			if (itemRect.Right < _owner._clientArea.X | itemRect.Bottom < _owner._clientArea.Y)
				return;
			Rectangle textRect = new Rectangle(itemRect.X + ListView._subItemMargin, _location.Y + imgSize + 4 + ListView._subItemMargin, itemRect.Width - (2 * ListView._subItemMargin), itemRect.Height - (imgSize + 4 + ListView._subItemMargin));
			Rectangle imgRect = new Rectangle(itemRect.X + 16, itemRect.Y + 2, imgSize, imgSize);
			Image img = _item.PreviewImage;
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			if (img != null)
				imgRect = Renderer.Drawing.getImageRectangle(img, imgRect, imgSize);
			if (_selected | _onHover) {
				Renderer.Button.draw(g, itemRect, , 2, _owner.Enabled, , _selected, _onHover, false);
			}
			if (img != null) {
				if (_owner.Enabled) {
					g.DrawImage(img, imgRect);
				} else {
					Renderer.Drawing.grayscaledImage(img, imgRect, g);
				}
			}
			g.DrawString(_item.Text, _item.Font, (_owner.Enabled ? (_selected | _onHover ? Renderer.Column.TextBrush : Brushes.White) : Brushes.Gray), textRect, strFormat);
			if ((_onHover | _selected | _item.Checked) & _owner._checkBoxes)
				Renderer.CheckBox.drawCheckBox(g, _checkRect, (_item.Checked ? CheckState.Checked : CheckState.Unchecked), , _owner.Enabled, _onHoverCheck);
		}
		/// <summary>
		/// Draw the ListViewItem checkbox on specified Graphics object.
		/// </summary>
		public void drawCheckBox(Graphics g)
		{
			Renderer.CheckBox.drawCheckBox(g, _checkRect, (_item.Checked ? System.Windows.Forms.CheckState.Checked : System.Windows.Forms.CheckState.Unchecked), , _owner.Enabled, _onHoverCheck);
		}
		/// <summary>
		/// Draw the preview image in the center of the client area.
		/// </summary>
		public void drawPreviewImage(Graphics g, Rectangle rect, int degree, bool drawText = false)
		{
			Rectangle mirrorRect = new Rectangle(rect.X, -(rect.Y + (rect.Height * 2)) + 2, rect.Width, rect.Height);
			GraphicsPath path = null;
			Rectangle imageRect = default(Rectangle);
			Image img = _item.PreviewImage;
			StringFormat strFormat = new StringFormat();
			int rounded = 5 * Math.Sin(degree * Math.PI / 180);
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			// Drawing the shadow
			drawRectangleShadow(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height - 5), Color.Black, g, 5 * Math.Sin(degree * Math.PI / 180));
			// Drawing original image
			// Drawing image's background
			path = Renderer.Drawing.roundedRectangle(rect, rounded, rounded, rounded, rounded);
			g.FillPath(Brushes.LightGray, path);
			g.DrawPath(Pens.White, path);
			// Draw the image, or notification if no image available
			if (img != null) {
				imageRect = Renderer.Drawing.getImageRectangle(img, rect, rect.Width - (10 * Math.Sin(degree * Math.PI / 180)), rect.Height - (10 * Math.Sin(degree * Math.PI / 180)));
				if (_owner.Enabled) {
					g.DrawImage(img, imageRect);
				} else {
					Renderer.Drawing.grayscaledImage(img, imageRect, g);
				}
			} else {
				if (_owner.Enabled) {
					g.DrawString("No Preview Available", _item.Font, Brushes.Black, rect, strFormat);
				} else {
					g.DrawString("No Preview Available", _item.Font, Brushes.Gray, rect, strFormat);
				}
			}
			// Darken effect
			g.FillPath(new SolidBrush(Color.FromArgb(255 - (128 + (127 * Math.Sin(degree * Math.PI / 180))), 0, 0, 0)), path);
			g.DrawPath(new Pen(Color.FromArgb(255 - (128 + (127 * Math.Sin(degree * Math.PI / 180))), 0, 0, 0)), path);
			path.Dispose();
			// Reflecting to X axis
			g.Transform = _owner._mirrorMatrix;
			// Drawing mirror image, using mirror rectangle as origin
			// Creating mirror effect
			LinearGradientBrush mirrorBrush = new LinearGradientBrush(mirrorRect, Color.White, Color.Black, LinearGradientMode.Vertical);
			ColorBlend mirrorBlend = new ColorBlend();
			Color[] mirrorColors = new Color[3];
			float[] mirrorPos = new float[3];
			mirrorColors[0] = Color.Black;
			mirrorColors[1] = Color.Black;
			mirrorColors[2] = Color.FromArgb(31, 0, 0, 0);
			mirrorPos[0] = 0f;
			mirrorPos[1] = 0.4f;
			mirrorPos[2] = 1f;
			mirrorBlend.Colors = mirrorColors;
			mirrorBlend.Positions = mirrorPos;
			mirrorBrush.InterpolationColors = mirrorBlend;
			drawRectangleShadow(new Rectangle(mirrorRect.X, mirrorRect.Y, mirrorRect.Width, mirrorRect.Height - 5), Color.Black, g, 5 * Math.Sin(degree * Math.PI / 180));
			// Drawing image's background
			path = Renderer.Drawing.roundedRectangle(mirrorRect, rounded, rounded, rounded, rounded);
			g.FillPath(Brushes.Gray, path);
			g.DrawPath(Pens.White, path);
			// Draw the image, or notification if no image available
			if (img != null) {
				imageRect = Renderer.Drawing.getImageRectangle(img, mirrorRect, rect.Width - (10 * Math.Sin(degree * Math.PI / 180)), rect.Height - (10 * Math.Sin(degree * Math.PI / 180)));
				if (_owner.Enabled) {
					g.DrawImage(img, imageRect);
				} else {
					Renderer.Drawing.grayscaledImage(img, imageRect, g);
				}
			} else {
				if (_owner.Enabled) {
					g.DrawString("No Preview Available", _item.Font, Brushes.Black, mirrorRect, strFormat);
				} else {
					g.DrawString("No Preview Available", _item.Font, Brushes.Gray, mirrorRect, strFormat);
				}
			}
			// Darken effect
			g.FillPath(new SolidBrush(Color.FromArgb(255 - (128 + (127 * Math.Sin(degree * Math.PI / 180))), 0, 0, 0)), path);
			g.DrawPath(new Pen(Color.FromArgb(255 - (128 + (127 * Math.Sin(degree * Math.PI / 180))), 0, 0, 0)), path);
			// Darken mirror effect
			g.FillPath(mirrorBrush, path);
			g.DrawPath(new Pen(mirrorBrush, 2), path);
			path.Dispose();
			mirrorBrush.Dispose();
			// Returning graphics transformation
			g.Transform = _owner._normalMatrix;
			// Releasing resource
			if (drawText) {
				if (!_owner._allowMultiline)
					strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
				Rectangle txtRect = new Rectangle(rect.X, rect.Bottom - 8, rect.Width, _item.Font.Height * 2);
				SizeF txtSize = g.MeasureString(_item.Text, _item.Font, txtRect.Size, strFormat);
				txtRect.Height = txtSize.Height + 4;
				txtRect.Width = txtSize.Width + 4;
				txtRect.X = rect.X + ((rect.Width - txtRect.Width) / 2);
				txtRect.Y = rect.Bottom + 5 - txtRect.Height;
				drawRectangleShadow(txtRect, Color.White, g, 2, 10);
				GraphicsPath txtPath = Renderer.Drawing.roundedRectangle(txtRect, 2, 2, 2, 2);
				g.FillPath(new SolidBrush(Color.FromArgb(127, 127, 127, 127)), txtPath);
				g.DrawPath(new Pen(Color.FromArgb(127, 255, 255, 255)), txtPath);
				g.DrawString(_item.Text, _item.Font, Brushes.White, txtRect, strFormat);
				txtPath.Dispose();
			}
			strFormat.Dispose();
		}
		/// <summary>
		/// Draw the shadow effect of a rectangle.
		/// </summary>
		private void drawRectangleShadow(Rectangle rect, Color color, Graphics g, int rounded = 3, int maxStep = 20)
		{
			if (maxStep <= 2)
				return;
			int i = 0;
			GraphicsPath rPath = Renderer.Drawing.roundedRectangle(rect, rounded, rounded, rounded, rounded);
			i = 2;
			while (i < maxStep) {
				Color pColor = System.Drawing.Color.FromArgb((maxStep + 2) - i, color.R, color.G, color.B);
				Pen pRect = new Pen(pColor, i);
				g.DrawPath(pRect, rPath);
				pRect.Dispose();
				i += 1;
			}
			rPath.Dispose();
		}
		// Item measurement
		/// <summary>
		/// Measure the size of the ListViewItemHost in defferent views.
		/// </summary>
		[Description("Measure the size of the ListViewItemHost in defferent views.")]
		public void measureSize()
		{
			switch (_owner._view) {
				case Control.View.Details:
					int x = 0;
					int right = 0;
					int height = 0;
					bool first = true;
					foreach (ListViewSubItemHost siHost in _subItemHosts) {
						siHost.measureOriginal();
						siHost.measureDisplay();
						if (first) {
							x = siHost.Bounds.X;
							right = siHost.Bounds.Right;
							height = siHost.Bounds.Height;
							first = false;
						} else {
							if (siHost.Bounds.X < x)
								x = siHost.Bounds.X;
							if (right < siHost.Bounds.Right)
								right = siHost.Bounds.Right;
							if (height < siHost.Bounds.Height)
								height = siHost.Bounds.Height;
						}
					}

					_location.X = x;
					_size.Width = right - x;
					_size.Height = height;
					break;
				case Control.View.Icon:
					_size = new Size(_owner._iconSize + 20, _owner._iconSize + 40);
					break;
				case Control.View.List:
					_subItemHosts[0].measureOriginal();
					_subItemHosts[0].measureDisplay();
					_size = _subItemHosts[0].OriginalSize.ToSize();
					if (_owner._checkBoxes)
						_size.Width += 22;
					if (_size.Width > ListView._listColumnWidth - 20)
						_size.Width = ListView._listColumnWidth - 20;
					break;
				case View.Preview:
					_size = new Size(ListView._slideItemSize, ListView._slideItemSize);
					break;
				case View.Thumbnail:
					_size = new Size(_owner._thumbnailSize + 20, _owner._thumbnailSize + 40);
					break;
				case View.Tile:
					_size = new Size(ListView._tileColumnWidth - 20, ListView._tileHeight);
					break;
			}
		}
		/// <summary>
		/// Gets the bounding rectangle of a ListViewSubItem host specified by its index.
		/// </summary>
		/// <param name="index">Index of a ListViewSubItem object in ListViewItem.SubItems collection.</param>
		[Description("Gets the bounding rectangle of a ListViewSubItem host specified by its index.")]
		public Rectangle getSubItemRectangle(int index)
		{
			if (index >= 0 & index < _subItemHosts.Count) {
				return _subItemHosts[index].Bounds;
			}
			return new Rectangle(0, 0, 0, 0);
		}
		/// <summary>
		/// Gets the display string of a ListViewSubItem object specified by its index.
		/// </summary>
		/// <param name="index">Index of a ListViewSubItem object in ListViewItem.SubItems collection.</param>
		[Description("Gets the display string of a ListViewSubItem object specified by its index.")]
		public string getSubItemString(int index)
		{
			if (index >= 0 & index < _subItemHosts.Count) {
				return _subItemHosts[index].getSubItemString();
			}
			return "";
		}
		/// <summary>
		/// Gets the size of displayed string from a ListViewSubItem object specified by its index.
		/// </summary>
		/// <param name="index">Index of a ListViewSubItem object in ListViewItem.SubItems collection.</param>
		[Description("Gets the size of displayed string from a ListViewSubItem object specified by its index.")]
		public SizeF getSubItemSize(int index)
		{
			if (index >= 0 & index < _subItemHosts.Count) {
				return _subItemHosts[index].OriginalSize;
			}
			return new SizeF(0, 0);
		}
		/// <summary>
		/// Gets the size of the item's text, with maximum width and height allowed.
		/// </summary>
		/// <param name="maxWidth">Maximum width allowed of the item's text size.</param>
		/// <param name="maxHeight">Maximum height allowed of the item's text size.</param>
		[Description("Gets the size of the item's text, with maximum width and height allowed.")]
		public SizeF getTextSize(int maxWidth, int maxHeight)
		{
			SizeF result = default(SizeF);
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			result = _owner._gObj.MeasureString(_item.Text, _item.Font, maxWidth, strFormat);
			if (_item.SmallImage != null)
				result.Width += 20;
			if (result.Height > maxHeight)
				result.Height = maxHeight;
			return result;
		}
		/// <summary>
		/// Gets the bounding rectangle of the ListViewItem text.
		/// </summary>
		public Rectangle getTextRectangle()
		{
			Rectangle txtRect = new Rectangle(0, 0, 0, 0);
			txtRect.Size = _subItemHosts[0].OriginalSize.ToSize();
			if (txtRect.Height == 0) {
				txtRect.Height = _item.Font.Height;
				txtRect.Width = 10;
			}
			switch (_owner._view) {
				case Control.View.Details:
				case Control.View.List:
					txtRect.Y = _location.Y;
					txtRect.X = _subItemHosts[0].Bounds.X + (_item.SmallImage != null ? 20 : 0);
					break;
				case Control.View.Icon:
				case Control.View.Preview:
				case Control.View.Thumbnail:
					txtRect.Y = _location.Y + _size.Height - 30;
					txtRect.X = _location.X + ((_size.Width - txtRect.Width) / 2);
					break;
				case Control.View.Tile:
					txtRect.Y = _location.Y + ((_size.Height - txtRect.Height) / 2);
					txtRect.X = _location.X + ListView._tileHeight;
					break;
			}
			return txtRect;
		}
		/// <summary>
		/// Gets a value indicating an item need tooltip to show the full text.
		/// </summary>
		public bool needToolTip(Rectangle rect)
		{
			StringFormat strFormat = new StringFormat();
			Rectangle textRect = new Rectangle(0, 0, 0, 0);
			SizeF strSize = new SizeF(0, 0);
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			switch (_owner._view) {
				case Control.View.Icon:
				case Control.View.Thumbnail:
					int imgSize = (_owner._view == Control.View.Icon ? _owner._iconSize : _owner._thumbnailSize);
					textRect = new Rectangle(rect.X + ListView._subItemMargin, _location.Y + imgSize + 10, rect.Width - (2 * ListView._subItemMargin), rect.Height - (imgSize + 10 + ListView._subItemMargin));
					strSize = _owner._gObj.MeasureString(_item.Text, _item.Font, textRect.Width, strFormat);
					break;
				case Control.View.Preview:
					int imgSize = 48;
					textRect = new Rectangle(rect.X + ListView._subItemMargin, _location.Y + imgSize + 4 + ListView._subItemMargin, rect.Width - (2 * ListView._subItemMargin), rect.Height - (imgSize + 4 + ListView._subItemMargin));
					strSize = _owner._gObj.MeasureString(_item.Text, _item.Font, textRect.Width, strFormat);
					break;
				case Control.View.List:
					strFormat.Alignment = StringAlignment.Near;
					textRect = rect;
					if (_owner._checkBoxes) {
						textRect.X += 22;
						textRect.Width -= 22;
					}
					textRect.X += 20;
					textRect.Width -= 20;
					strSize = _owner._gObj.MeasureString(_item.Text, _item.Font, textRect.Width, strFormat);
					break;
				case Control.View.Tile:
					strFormat.Alignment = StringAlignment.Near;
					textRect = rect;
					textRect.X += ListView._tileHeight;
					textRect.Width -= ListView._tileHeight;
					textRect.Height = _item.Font.Height * 2;
					strSize = _owner._gObj.MeasureString(_item.Text, _item.Font, textRect.Width, strFormat);
					break;
			}
			return Math.Floor(strSize.Width) > textRect.Width | Math.Floor(strSize.Height) > textRect.Height;
		}
		// Properties
		/// <summary>
		/// Determine x location of the ListViewItemHost.
		/// </summary>
		public int X {
			get { return _location.X; }
			set {
				_location.X = value;
				if (_owner._view == View.Details) {
					_checkRect = new Rectangle(4, _location.Y + ((_size.Height - 14) / 2), 14, 14);
				} else {
					_checkRect = new Rectangle(_location.X + 4, _location.Y + 4, 14, 14);
				}
				relocateSubItems();
			}
		}
		/// <summary>
		/// Determine y location of the ListViewItemHost.
		/// </summary>
		public int Y {
			get { return _location.Y; }
			set {
				_location.Y = value;
				foreach (ListViewSubItemHost siHost in _subItemHosts) {
					siHost.Y = _location.Y;
				}
				if (_owner._view == View.Details) {
					_checkRect = new Rectangle(4, _location.Y + ((_size.Height - 14) / 2), 14, 14);
				} else {
					_checkRect = new Rectangle(_location.X + 4, _location.Y + 4, 14, 14);
				}
			}
		}
		/// <summary>
		/// Determine the location of the ListViewItemHost.
		/// </summary>
		[Description("Determine the locatoin of the host in ListView." + "This location can be determined by ListView itself, or by the GroupHost where this item is attached.")]
		public Point Location {
			get { return _location; }
			set {
				_location = value;
				foreach (ListViewSubItemHost siHost in _subItemHosts) {
					siHost.Y = _location.Y;
				}
				if (_owner._view == View.Details) {
					_checkRect = new Rectangle(4, _location.Y + ((_size.Height - 14) / 2), 14, 14);
				} else {
					_checkRect = new Rectangle(_location.X + 4, _location.Y + 4, 14, 14);
				}
			}
		}
		/// <summary>
		/// Determine the visibility of the ListViewItemHost.
		/// </summary>
		[Description("Determine the visibility of the host.")]
		public bool Visible {
			get { return _visible; }
			set {
				_visible = value;
				if (!_visible) {
					_onMouseDown = false;
					if (object.ReferenceEquals(_owner._selectedHost, this))
						_owner._selectedHost = null;
					_selected = false;
					_onHover = false;
					_onHoverCheck = false;
				}
			}
		}
		/// <summary>
		/// Determine whether the ListViewSubItemHost is selected.
		/// </summary>
		public bool Selected {
			get { return _selected; }
			set { _selected = value; }
		}
		/// <summary>
		/// Determine GroupHost object where the ListViewItemHost object to be placed.
		/// </summary>
		[Description("Determine a LsitViewGroupHost contained this ListViewItemHost.")]
		public ListViewGroupHost GroupHost {
			get { return _groupHost; }
			set { _groupHost = value; }
		}
		/// <summary>
		/// Determine whether the ListViewSubItemHost is frozen.
		/// </summary>
		public bool Frozen {
			get { return _frozen; }
			set { _frozen = value; }
		}
		/// <summary>
		/// Gets a value indicating the ListViewItemHost is visible in the client area of ListView control.
		/// </summary>
		public bool IsVisible {
			get {
				if (!_visible)
					return false;
				if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
					return false;
				Rectangle rect = Bounds;
				if (rect.Right < _owner._clientArea.X | rect.Bottom < _owner._clientArea.Y)
					return false;
				return true;
			}
		}
		/// <summary>
		/// Gets the rightmost location of the ListViewItemHost.
		/// </summary>
		public int Right {
			get { return _location.X + _size.Width; }
		}
		/// <summary>
		/// Gets the bottommost location of the ListViewItemHost.
		/// </summary>
		public int Bottom {
			get { return _location.Y + _size.Height; }
		}
		/// <summary>
		/// Gets the bounding rectangle of the ListViewItemHost.
		/// </summary>
		[Description("Gets the bounding rectangle of the ListViewItemHost.  It can be different when the item is selected.")]
		public Rectangle Bounds {
			get {
				Rectangle rectArea = default(Rectangle);
				rectArea = new Rectangle(_location, _size);
				if (_selected) {
					if (_owner._view == Control.View.Icon | _owner._view == Control.View.Thumbnail) {
						int imgSize = (_owner._view == Control.View.Icon ? _owner._iconSize : _owner._thumbnailSize);
						SizeF textSize = getTextSize(_size.Width - (2 * ListView._subItemMargin), ListView._maxTextHeight);
						if (imgSize + 10 + ListView._subItemMargin + textSize.Height > _size.Height)
							rectArea.Height = imgSize + 10 + ListView._subItemMargin + textSize.Height;
					}
				}
				return rectArea;
			}
		}
		/// <summary>
		/// Gets the ListViewItem object contained within ListViewItemHost.
		/// </summary>
		[Description("Gets ListViewItem object contained in the ListViewItemHost.")]
		public ListViewItem Item {
			get { return _item; }
		}
		/// <summary>
		/// Gets a value indicating the mouse is pressed on the ListViewItemHost.
		/// </summary>
		[Description("Gets a value indicating the host is pressed.")]
		public bool OnMouseDown {
			get { return _onMouseDown; }
		}
		/// <summary>
		/// Gets a value indicating the mouse pointer is moved over the ListViewItemHost.
		/// </summary>
		[Description("Gets a value indicating the mouse pointer is moved over the host.")]
		public bool OnHover {
			get { return _onHover; }
		}
		/// <summary>
		/// Gets a value indicating the row index of a ListViewItem.
		/// </summary>
		public int RowIndex {
			get {
				int index = 0;
				foreach (ListViewGroupHost lvgHost in _owner._groupHosts) {
					if (!object.ReferenceEquals(lvgHost, _groupHost)) {
						index += lvgHost.VisibleItemsCount;
					} else {
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				foreach (ListViewItemHost lviHost in _groupHost.ItemHosts) {
					if (!object.ReferenceEquals(lviHost, this)) {
						if (lviHost.Visible & !lviHost.Frozen)
							index += 1;
					} else {
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				return index + 1;
			}
		}
		// Mouse event handlers
		/// <summary>
		/// Test whether the mouse pointer moves over the host.
		/// </summary>
		[Description("Test whether the mouse pointer moves over the host.")]
		public bool mouseMove(MouseEventArgs e)
		{
			if (!_visible)
				return false;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return false;
			if (_owner._view == Control.View.Details) {
				// Special case on Details views.
				bool stateChanged = false;
				if (_owner._fullRowSelect) {
					Rectangle itemRect = Bounds;
					if (itemRect.Contains(e.Location) & e.X > _owner._clientArea.X & e.Y > _owner._clientArea.Y) {
						if (!_onHover) {
							_owner.invokeItemHover(_item);
							_onHover = true;
							if (_owner._itemTooltip) {
								if (Renderer.ToolTip.containsToolTip(_item.ToolTipTitle, _item.ToolTip, _item.ToolTipImage)) {
									_owner._needToolTip = true;
									_owner._currentToolTip = _item.ToolTip;
									_owner._currentToolTipTitle = _item.ToolTipTitle;
									_owner._currentToolTipImage = _item.ToolTipImage;
									_owner._currentToolTipRect = itemRect;
									_owner._tooltipCaller = this;
								}
							} else {
								// Gets the first displayed subitem
								List<ColumnHeader> columns = _owner._columnControl.FrozenColumns;
								if (columns.Count == 0)
									columns = _owner._columnControl.UnFrozenColumns;
								if (columns.Count > 0) {
									int firstIndex = _owner._columns.IndexOf[columns[0]];
									if (firstIndex == 0) {
										ListViewSubItemHost aHost = _subItemHosts[firstIndex];
										if (Math.Floor(aHost.OriginalSize.Width) > aHost.DisplayedSize.Width | Math.Floor(aHost.OriginalSize.Height) > aHost.DisplayedSize.Height) {
											_owner._needToolTip = true;
											_owner._currentToolTip = aHost.getSubItemString();
											_owner._currentToolTipRect = aHost.Bounds;
											_owner._tooltipCaller = this;
										}
									}
								}
							}
							stateChanged = true;
						}
					} else {
						if (_onHover) {
							_onHover = false;
							if (object.ReferenceEquals(_owner._tooltipCaller, this))
								_owner._tooltip.Hide();
							stateChanged = true;
						}
					}
					if (_owner._checkBoxes & !_onHover) {
						if (_checkRect.Contains(e.Location)) {
							if (!_onHoverCheck) {
								_onHoverCheck = true;
								stateChanged = true;
							}
						} else {
							if (_onHoverCheck) {
								_onHoverCheck = false;
								stateChanged = true;
							}
						}
					}
				} else {
					ListViewSubItemHost subitemHover = null;
					List<ColumnHeader> columns = null;
					int colIndex = 0;
					int hoverIndex = -1;
					// Test on unfrozen columns first
					columns = _owner._columnControl.UnFrozenColumns;
					foreach (ColumnHeader ch in columns) {
						colIndex = _owner._columns.IndexOf[ch];
						if (colIndex >= 0 & colIndex < _subItemHosts.Count) {
							stateChanged = stateChanged | _subItemHosts[colIndex].mouseMove(e);
							if (_subItemHosts[colIndex].OnHover) {
								subitemHover = _subItemHosts[colIndex];
								hoverIndex = colIndex;
							}
						}
					}
					// Test on frozen columns.
					columns = _owner._columnControl.FrozenColumns;
					if (columns.Count > 0) {
						Rectangle rectFrozen = _owner._columnControl.FrozenRectangle;
						rectFrozen.Y = 0;
						rectFrozen.Height = _owner.Height;
						if (rectFrozen.Contains(e.Location)) {
							if (subitemHover != null)
								subitemHover.OnHover = false;
							subitemHover = null;
							hoverIndex = -1;
						}
					}
					foreach (ColumnHeader ch in columns) {
						colIndex = _owner._columns.IndexOf[ch];
						if (colIndex >= 0 & colIndex < _subItemHosts.Count) {
							stateChanged = stateChanged | _subItemHosts[colIndex].mouseMove(e);
							if (_subItemHosts[colIndex].OnHover) {
								if (subitemHover != null)
									subitemHover.OnHover = false;
								subitemHover = _subItemHosts[colIndex];
								hoverIndex = colIndex;
							}
						}
					}
					if (subitemHover != null & hoverIndex == 0) {
						if (!_onHover) {
							_owner.invokeItemHover(_item);
							if (_owner._itemTooltip) {
								if (Renderer.ToolTip.containsToolTip(_item.ToolTipTitle, _item.ToolTip, _item.ToolTipImage)) {
									_owner._needToolTip = true;
									_owner._currentToolTip = _item.ToolTip;
									_owner._currentToolTipTitle = _item.ToolTipTitle;
									_owner._currentToolTipImage = _item.ToolTipImage;
									_owner._currentToolTipRect = subitemHover.Bounds;
									_owner._tooltipCaller = this;
								}
							} else {
								if (Math.Floor(subitemHover.OriginalSize.Width) > subitemHover.DisplayedSize.Width | Math.Floor(subitemHover.OriginalSize.Height) > subitemHover.DisplayedSize.Height) {
									_owner._needToolTip = true;
									_owner._currentToolTip = subitemHover.getSubItemString();
									_owner._currentToolTipRect = subitemHover.Bounds;
									_owner._tooltipCaller = this;
								}
							}
						}
						_onHover = true;
					} else {
						if (object.ReferenceEquals(_owner._tooltipCaller, this))
							_owner._tooltip.Hide();
						_onHover = false;
					}
					if (_owner._checkBoxes & !_onHover) {
						if (_checkRect.Contains(e.Location)) {
							if (!_onHoverCheck) {
								_onHoverCheck = true;
								stateChanged = true;
							}
						} else {
							if (_onHoverCheck) {
								_onHoverCheck = false;
								stateChanged = true;
							}
						}
					}
				}
				return stateChanged;
			} else {
				bool stateChanged = false;
				Rectangle itemRect = Bounds;
				if (itemRect.Contains(e.Location) & e.X > _owner._clientArea.X & e.Y > _owner._clientArea.Y) {
					bool mouseOnArea = true;
					if (!object.ReferenceEquals(_owner._selectedHost, this)) {
						if (_owner._selectedHost != null) {
							if (_owner._selectedHost.IsVisible) {
								if (_owner._selectedHost.Bounds.Contains(e.Location))
									mouseOnArea = false;
							}
						}
					}
					if (mouseOnArea) {
						if (!_onHover) {
							_owner.invokeItemHover(_item);
							if (_owner._itemTooltip) {
								if (Renderer.ToolTip.containsToolTip(_item.ToolTipTitle, _item.ToolTip, _item.ToolTipImage)) {
									_owner._needToolTip = true;
									_owner._currentToolTip = _item.ToolTip;
									_owner._currentToolTipTitle = _item.ToolTipTitle;
									_owner._currentToolTipImage = _item.ToolTipImage;
									_owner._currentToolTipRect = itemRect;
									_owner._tooltipCaller = this;
								}
							} else {
								if (needToolTip(itemRect)) {
									_owner._needToolTip = true;
									_owner._currentToolTip = _item.Text;
									_owner._currentToolTipRect = itemRect;
									_owner._tooltipCaller = this;
								}
							}
							_onHover = true;
							stateChanged = true;
						}
						if (_owner._checkBoxes) {
							if (_checkRect.Contains(e.Location)) {
								if (!_onHoverCheck) {
									_onHoverCheck = true;
									stateChanged = true;
								}
							} else {
								if (_onHoverCheck) {
									_onHoverCheck = false;
									stateChanged = true;
								}
							}
						}
					} else {
						if (_onHover) {
							if (object.ReferenceEquals(_owner._tooltipCaller, this))
								_owner._tooltip.Hide();
							_onHoverCheck = false;
							_onHover = false;
							stateChanged = true;
						}
					}
				} else {
					if (_onHover) {
						if (object.ReferenceEquals(_owner._tooltipCaller, this))
							_owner._tooltip.Hide();
						_onHoverCheck = false;
						_onHover = false;
						stateChanged = true;
					}
				}
				return stateChanged;
			}
		}
		/// <summary>
		/// Test whether the mouse is pressed over the host.
		/// </summary>
		[Description("Test whether the mouse is pressed over the host.")]
		public bool mouseDown(MouseButtons button = System.Windows.Forms.MouseButtons.Left)
		{
			if (_onHover) {
				if (!_onHoverCheck) {
					if (!object.ReferenceEquals(_owner._selectedHost, this)) {
						_owner.setSelectedHost(this);
					} else {
						if (button == System.Windows.Forms.MouseButtons.Left) {
							if (_owner._labelEdit)
								_owner.showTextBoxEditor(this);
						}
					}
					if (!_onMouseDown) {
						_onMouseDown = true;
						return true;
					}
				}
			} else {
				if (_onMouseDown) {
					_onMouseDown = false;
					return true;
				}
			}
			if (_onHoverCheck) {
				_item.Checked = !_item.Checked;
				_onMouseDown = false;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Test whether the mouse is released over the host.
		/// </summary>
		[Description("Test whether the mouse is released over the host.")]
		public bool mouseUp()
		{
			if (_onMouseDown) {
				_onMouseDown = false;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Test whether the mouse pointer is leaving the host.
		/// </summary>
		public bool mouseLeave()
		{
			bool stateChanged = false;
			if (_owner._view == Control.View.Details) {
				foreach (ListViewSubItemHost lvsiHost in _subItemHosts) {
					stateChanged = stateChanged | lvsiHost.mouseLeave();
				}
			}
			if (_onHover | _onHoverCheck) {
				_onHover = false;
				_onHoverCheck = false;
				stateChanged = true;
			}
			return stateChanged;
		}
	}
	/// <summary>
	/// Class to host a ListViewGroup object and handles all of its operations, a visual representation of a ListViewGroup.
	/// </summary>
	[Description("Class to host a ListViewGroup object and handles all of its operations.")]
	private class ListViewGroupHost
	{
		ListViewGroup _group;
		ListView _owner;
		Point _location;
		Size _headerSize;
		Size _groupSize;
		List<ListViewItemHost> _itemHosts = new List<ListViewItemHost>();
		// CheckBox and DropDown
		Rectangle _chkRect;
		bool _chkHover = false;
		Rectangle _ddRect;
		bool _ddHover = false;
		public ListViewGroupHost(ListView owner, ListViewGroup @group)
		{
			_owner = owner;
			_group = @group;
		}
		/// <summary>
		/// Relocate all ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		[Description("Measure size used by the group, and relocate all contained items, using current location.")]
		public void relocate()
		{
			if (_itemHosts.Count == 0)
				return;
			int itemsY = _location.Y;
			int itemsX = _location.X;
			int maxWidth = _owner.Width - (_owner._vScroll.Width + 2);
			switch (_owner._view) {
				case Control.View.Details:
					itemsY = _location.Y + _headerSize.Height + 1;
					_location.X = _itemHosts[0].X;
					_groupSize.Width = _owner._columnControl.DisplayedRectangle.Width;
					foreach (ListViewItemHost lviHost in _itemHosts) {
						lviHost.relocateSubItems();
						if (lviHost.Visible & !lviHost.Frozen) {
							lviHost.Y = itemsY;
							itemsY += lviHost.Bounds.Height + 1;
						}
					}

					break;
				case Control.View.Icon:
				case Control.View.Thumbnail:
				case Control.View.Tile:
				case Control.View.List:
					itemsY = _location.Y + _headerSize.Height + 1;
					int xRange = 0;
					int yRange = 0;
					switch (_owner._view) {
						case View.Icon:
							xRange = _owner._iconSize + 30;
							yRange = _owner._iconSize + 50;
							break;
						case View.List:
							xRange = ListView._listColumnWidth;
							break;
						case View.Thumbnail:
							xRange = _owner._thumbnailSize + 30;
							yRange = _owner._thumbnailSize + 50;
							break;
						case View.Tile:
							xRange = ListView._tileColumnWidth;
							yRange = ListView._tileHeight + 5;
							break;
					}
					foreach (ListViewItemHost lviHost in _itemHosts) {
						if (lviHost.Visible) {
							lviHost.X = itemsX;
							lviHost.Y = itemsY;
							itemsX = itemsX + xRange;
							if (_owner._view == View.List) {
								if (yRange < lviHost.Bounds.Height)
									yRange = lviHost.Bounds.Height;
							}
							if (itemsX + xRange > maxWidth) {
								if (_owner._view == View.List)
									yRange += 1;
								itemsY = itemsY + yRange;
								_groupSize.Width = itemsX;
								itemsX = _location.X;
								if (_owner._view == View.List)
									yRange = 0;
							}
						}
					}

					break;
				case Control.View.Preview:
					switch (_owner._slideLocation) {
						case SlideLocation.Bottom:
							_location.Y = _owner.Height - (ListView._slideItemSize + 1);
							_headerSize = new Size(_owner._groupFont.Height, ListView._slideItemSize);
							break;
						case SlideLocation.Left:
							_location.X = 1;
							_headerSize = new Size(ListView._slideItemSize, _owner._groupFont.Height);
							break;
						case SlideLocation.Right:
							_location.X = _owner.Width - (ListView._slideItemSize + 1);
							_headerSize = new Size(ListView._slideItemSize, _owner._groupFont.Height);
							break;
						case SlideLocation.Top:
							_location.Y = _owner._columnControl.Bottom + 1;
							_headerSize = new Size(_owner._groupFont.Height, ListView._slideItemSize);
							break;
					}
					if (_owner._slideLocation == Control.SlideLocation.Top | _owner._slideLocation == Control.SlideLocation.Bottom) {
						itemsX = _location.X + _headerSize.Width + 1;
						foreach (ListViewItemHost lviHost in _itemHosts) {
							lviHost.X = itemsX;
							lviHost.Y = itemsY;
							if (lviHost.Visible)
								itemsX += lviHost.Bounds.Width + 5;
						}
					} else {
						itemsY = _location.Y + _headerSize.Height + 1;
						foreach (ListViewItemHost lviHost in _itemHosts) {
							lviHost.Y = itemsY;
							lviHost.X = itemsX;
							if (lviHost.Visible)
								itemsY += lviHost.Bounds.Height + 5;
						}
					}
					break;
			}
		}
		/// <summary>
		/// Measure the size of ListViewGroupHost and all ListViewItemHost contained in it.
		/// </summary>
		public void measureSize()
		{
			foreach (ListViewItemHost lviHost in _itemHosts) {
				lviHost.measureSize();
			}
			if (_itemHosts.Count == 0) {
				_groupSize.Width = 0;
				_groupSize.Height = 0;
				return;
			}
			int itemsHeight = 0;
			int itemsWidth = 0;
			int maxWidth = _owner.Width - (_owner._vScroll.Width + 2);
			switch (_owner._view) {
				case Control.View.Details:
					_location.X = _owner._columnControl.DisplayedRectangle.X;
					Rectangle frozenRect = _owner._columnControl.FrozenRectangle;
					if (frozenRect.X > -1)
						_location.X = frozenRect.X;
					_groupSize.Width = _owner._columnControl.DisplayedRectangle.Width;
					_ddRect = new Rectangle(_location.X + 2, _location.Y + ((_headerSize.Height - 10) / 2), 10, 10);
					_chkRect = new Rectangle(_ddRect.Right + 5, _location.Y + ((_headerSize.Height - 22) / 2), 22, 22);
					foreach (ListViewItemHost lviHost in _itemHosts) {
						if (lviHost.Visible & !lviHost.Frozen) {
							itemsHeight += lviHost.Bounds.Height + 1;
						}
					}

					_headerSize.Width = _groupSize.Width;
					_groupSize.Height = itemsHeight;
					if (_groupSize.Height > 0) {
						_headerSize.Height = _owner._groupFont.Height;
						if (_headerSize.Height < 22)
							_headerSize.Height = 22;
						_groupSize.Height += _headerSize.Height;
					}
					break;
				case Control.View.Icon:
				case Control.View.Thumbnail:
				case Control.View.Tile:
				case Control.View.List:
					int xRange = 0;
					int yRange = 0;
					switch (_owner._view) {
						case View.Icon:
							xRange = _owner._iconSize + 30;
							yRange = _owner._iconSize + 50;
							break;
						case View.List:
							xRange = ListView._listColumnWidth;
							break;
						case View.Thumbnail:
							xRange = _owner._thumbnailSize + 30;
							yRange = _owner._thumbnailSize + 50;
							break;
						case View.Tile:
							xRange = ListView._tileColumnWidth;
							yRange = ListView._tileHeight + 5;
							break;
					}
					foreach (ListViewItemHost lviHost in _itemHosts) {
						if (lviHost.Visible) {
							itemsWidth = itemsWidth + xRange;
							if (_owner._view == View.List) {
								if (yRange < lviHost.Bounds.Height)
									yRange = lviHost.Bounds.Height;
							}
							if (itemsWidth + xRange > maxWidth) {
								if (_owner._view == View.List)
									yRange += 1;
								itemsHeight = itemsHeight + yRange;
								_groupSize.Width = itemsWidth;
								itemsWidth = 0;
								if (_owner._view == View.List)
									yRange = 0;
							}
						}
					}

					if (itemsWidth > 0)
						itemsHeight += yRange;
					if (_groupSize.Width < maxWidth)
						_groupSize.Width = maxWidth;
					if (itemsHeight > 0)
						_groupSize.Height = itemsHeight;
					_headerSize.Width = _groupSize.Width;
					if (_groupSize.Height > 0) {
						_headerSize.Height = _owner._groupFont.Height;
						if (_headerSize.Height < 22)
							_headerSize.Height = 22;
						_groupSize.Height += _headerSize.Height;
					}
					break;
				case Control.View.Preview:
					switch (_owner._slideLocation) {
						case SlideLocation.Bottom:
							_location.Y = _owner.Height - (ListView._slideItemSize + 1);
							_headerSize = new Size(_owner._groupFont.Height, ListView._slideItemSize);
							break;
						case SlideLocation.Left:
							_location.X = 1;
							_headerSize = new Size(ListView._slideItemSize, _owner._groupFont.Height);
							break;
						case SlideLocation.Right:
							_location.X = _owner.Width - (ListView._slideItemSize + 1);
							_headerSize = new Size(ListView._slideItemSize, _owner._groupFont.Height);
							break;
						case SlideLocation.Top:
							_location.Y = _owner._columnControl.Bottom + 1;
							_headerSize = new Size(_owner._groupFont.Height, ListView._slideItemSize);
							break;
					}
					if (_owner._slideLocation == Control.SlideLocation.Bottom | _owner._slideLocation == Control.SlideLocation.Top) {
						_ddRect = new Rectangle(_location.X + ((_headerSize.Width - 10) / 2), _location.Y + 2, 10, 10);
						_chkRect = new Rectangle(_location.X + ((_headerSize.Width - 22) / 2), _ddRect.Bottom + 2, 22, 22);
					} else {
						_ddRect = new Rectangle(_location.X + 2, _location.Y + ((_headerSize.Height - 10) / 2), 10, 10);
						_chkRect = new Rectangle(_ddRect.Right + 5, _location.Y + ((_headerSize.Height - 22) / 2), 22, 22);
					}
					if (_owner._slideLocation == Control.SlideLocation.Top | _owner._slideLocation == Control.SlideLocation.Bottom) {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible)
								itemsWidth += lviHost.Bounds.Width + 5;
						}
						if (itemsWidth > 0) {
							_groupSize.Height = _headerSize.Height;
							_groupSize.Width = itemsWidth + _headerSize.Width;
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible)
								itemsHeight += lviHost.Bounds.Height + 5;
						}
						if (itemsHeight > 0) {
							_groupSize.Width = _headerSize.Width;
							_groupSize.Height = itemsHeight + _headerSize.Height;
						}
					}
					break;
			}
		}
		// Item Hosts
		/// <summary>
		/// Sort all ListViewItemHost contained in ListViewGroupHost based on current applied sort order.
		/// </summary>
		[Description("Sort all ListViewItem contained within ListViewGroup.")]
		public void sortItems()
		{
			_itemHosts.Sort(_owner.itemHostComparer);
		}
		/// <summary>
		/// Filter all ListViewItemHost contained in ListViewGroupHost based on current filter parameters.
		/// </summary>
		[Description("Filter all ListViewItem contained within ListViewGroup.")]
		public void filterItems()
		{
			List<ColumnFilterHandle> handlers = _owner._columnControl.FilterHandlers;
			foreach (ListViewItemHost ih in _itemHosts) {
				ih.Visible = _owner.filterItem(ih.Item, handlers);
			}
		}
		/// <summary>
		/// Remove a ListViewItemHost from ListViewGroupHost's collection specified by the ListViewItem.
		/// </summary>
		/// <param name="item">Item to be removed.</param>
		[Description("Remove a ListViewItemHost specified by its ListViewItem object.")]
		public void removeHost(ListViewItem item)
		{
			int index = -1;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item)) {
					index = _itemHosts.IndexOf(lviHost);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
			if (index > -1)
				_itemHosts.RemoveAt(index);
		}
		// Check Box
		/// <summary>
		/// Check the CheckState of the ListViewItemGroupHost.
		/// </summary>
		[Description("Change CheckState of ListViewGroup based on Checked property of ListViewItem contained in this ListViewGroup.")]
		public void checkCheckedState()
		{
			int checkedCount = 0;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (lviHost.Item.Checked)
					checkedCount += 1;
			}
			if (checkedCount == _itemHosts.Count) {
				_group._checkState = CheckState.Checked;
			} else {
				if (checkedCount == 0) {
					_group._checkState = CheckState.Unchecked;
				} else {
					_group._checkState = CheckState.Indeterminate;
				}
			}
		}
		// Public properties
		/// <summary>
		/// Determine x location of the LsitViewGroupHost.
		/// </summary>
		[Description("Determine X location of the host.")]
		public int X {
			get { return _location.X; }
			set {
				int dx = value - _location.X;
				if (dx != 0) {
					foreach (ListViewItemHost ih in _itemHosts) {
						ih.X = ih.X + dx;
						ih.relocateSubItems();
					}
					_location.X = value;
					if (_owner._view == Control.View.Preview & (_owner._slideLocation == Control.SlideLocation.Bottom | _owner._slideLocation == Control.SlideLocation.Top)) {
						_ddRect = new Rectangle(_location.X + ((_headerSize.Width - 10) / 2), _location.Y + 2, 10, 10);
						_chkRect = new Rectangle(_location.X + ((_headerSize.Width - 22) / 2), _ddRect.Bottom + 2, 22, 22);
					} else {
						_ddRect = new Rectangle(_location.X + 2, _location.Y + ((_headerSize.Height - 10) / 2), 10, 10);
						_chkRect = new Rectangle(_ddRect.Right + 5, _location.Y + ((_headerSize.Height - 22) / 2), 22, 22);
					}
				}
			}
		}
		/// <summary>
		/// Determine y location of the ListViewGroupHost.
		/// </summary>
		[Description("Determine Y location of the host.")]
		public int Y {
			get { return _location.Y; }
			set {
				int dy = value - _location.Y;
				if (dy != 0) {
					foreach (ListViewItemHost ih in _itemHosts) {
						if (_owner._view == Control.View.Details) {
							if (!ih.Frozen) {
								ih.Y += dy;
								ih.relocateSubItems();
							}
						} else {
							ih.Y += dy;
							ih.relocateSubItems();
						}
					}
					_location.Y = value;
					if (_owner._view == Control.View.Preview & (_owner._slideLocation == Control.SlideLocation.Bottom | _owner._slideLocation == Control.SlideLocation.Top)) {
						_ddRect = new Rectangle(_location.X + ((_headerSize.Width - 10) / 2), _location.Y + 2, 10, 10);
						_chkRect = new Rectangle(_location.X + ((_headerSize.Width - 22) / 2), _ddRect.Bottom + 2, 22, 22);
					} else {
						_ddRect = new Rectangle(_location.X + 2, _location.Y + ((_headerSize.Height - 10) / 2), 10, 10);
						_chkRect = new Rectangle(_ddRect.Right + 5, _location.Y + ((_headerSize.Height - 22) / 2), 22, 22);
					}
				}
			}
		}
		/// <summary>
		/// Gets a value indicating the size of the ListViewGroupHost.
		/// </summary>
		public Size GroupSize {
			get {
				if (_group.IsCollapsed) {
					return _headerSize;
				} else {
					return _groupSize;
				}
			}
		}
		/// <summary>
		/// Gets a list of all visible ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		public List<ListViewItemHost> VisibleItemHosts {
			get {
				List<ListViewItemHost> result = new List<ListViewItemHost>();
				foreach (ListViewItemHost lviHost in _itemHosts) {
					if (lviHost.Visible & !lviHost.Frozen)
						result.Add(lviHost);
				}
				return result;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the ListViewGroupHost is visible.
		/// </summary>
		[Description("Gets a value indicating ListViewGroupHost is visible.")]
		public bool IsVisible {
			get { return _itemHosts.Count > 0 & _groupSize.Width > 0 & _groupSize.Height > 0; }
		}
		/// <summary>
		/// Gets a list of all ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		[Description("Determine the ListViewItemHost contained within ListViewGroupHost.")]
		public List<ListViewItemHost> ItemHosts {
			get { return _itemHosts; }
		}
		/// <summary>
		/// Gets a ListViewGroup object contained in ListViewGroupHost.
		/// </summary>
		[Description("Gets ListViewGroup object contained within ListViewGroupHost.")]
		public ListViewGroup Group {
			get { return _group; }
		}
		/// <summary>
		/// Gets the bounding rectangle of the ListViewGroupHost.
		/// </summary>
		[Description("Gets bounding rectangle of ListViewGroupHost.")]
		public Rectangle Bounds {
			get {
				if (_groupSize.Width == 0 | _groupSize.Height == 0) {
					return new Rectangle(_location, _groupSize);
				} else {
					if (_group.IsCollapsed) {
						return new Rectangle(_location, _headerSize);
					} else {
						return new Rectangle(_location, _groupSize);
					}
				}
			}
		}
		/// <summary>
		/// Gets the rightmost location of the ListViewGroupHost.
		/// </summary>
		[Description("Gets the right position of ListViewGroupHost.")]
		public int Right {
			get {
				if (_groupSize.Width == 0) {
					return _location.X;
				} else {
					if (_group.IsCollapsed) {
						return _location.X + _headerSize.Width;
					} else {
						return _location.X + _groupSize.Width;
					}
				}
			}
		}
		/// <summary>
		/// Gets the bottommost location of the ListViewGroupHost.
		/// </summary>
		public int Bottom {
			get {
				if (_groupSize.Height == 0) {
					return _location.Y;
				} else {
					if (_group.IsCollapsed) {
						return _location.Y + _headerSize.Height;
					} else {
						return _location.Y + _groupSize.Height;
					}
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether a ListViewItem object is visible.
		/// </summary>
		/// <param name="item">Item to check.</param>
		public bool ItemIsVisible {
			get {
				foreach (ListViewItemHost ih in _itemHosts) {
					if (object.ReferenceEquals(ih.Item, item))
						return ih.Visible;
				}
				return false;
			}
		}
		/// <summary>
		/// Gets the number of all visible ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		public int VisibleItemsCount {
			get {
				int result = 0;
				foreach (ListViewItemHost lviHost in _itemHosts) {
					if (lviHost.Visible & !lviHost.Frozen)
						result += 1;
				}
				return result;
			}
		}
		// Drawing
		/// <summary>
		/// Draw a ListViewGroup on a specified Graphics object, and optionally draw the CheckBox.
		/// </summary>
		/// <remarks>This draw method will draw all ListViewItem contained in a group in all view mode, except Details view mode.</remarks>
		public void draw(Graphics g, bool drawCheckBox = false)
		{
			if (VisibleItemsCount == 0)
				return;
			if (!_owner._showGroups)
				return;
			if (_groupSize.Width == 0 | _groupSize.Height == 0)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			if (this.Right < 0 | this.Bottom < 0)
				return;
			Rectangle txtRect = new Rectangle(_location, new Size(0, 0));
			StringFormat txtFormat = new StringFormat();
			int visibleCount = 0;
			txtFormat.FormatFlags = txtFormat.FormatFlags | StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
			txtFormat.Trimming = StringTrimming.EllipsisCharacter;
			foreach (ListViewItemHost ih in _itemHosts) {
				if (ih.Visible & !ih.Frozen)
					visibleCount = visibleCount + 1;
			}
			if (_owner._view == View.Preview & (_owner._slideLocation == SlideLocation.Bottom | _owner._slideLocation == SlideLocation.Top)) {
				txtFormat.FormatFlags = txtFormat.FormatFlags | StringFormatFlags.DirectionVertical;
				txtFormat.Alignment = StringAlignment.Center;
				switch (_group.TextAlign) {
					case HorizontalAlignment.Left:
						txtFormat.LineAlignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Center:
						txtFormat.LineAlignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						txtFormat.LineAlignment = StringAlignment.Far;
						break;
				}
				txtRect.Y = _ddRect.Bottom + 2;
				if (_owner._checkBoxes)
					txtRect.Y = _chkRect.Bottom + 2;
				txtRect.Height = _headerSize.Height - ((txtRect.Y - _location.Y) + 2);
				txtRect.Width = _headerSize.Width;
				// Draw drop down
				if (_owner.Enabled) {
					Renderer.Drawing.drawTriangle(g, _ddRect, (_ddHover ? System.Drawing.Color.Gold : System.Drawing.Color.White), Color.Black, (_group.IsCollapsed ? Renderer.Drawing.TriangleDirection.Down : Renderer.Drawing.TriangleDirection.DownRight));
				} else {
					Renderer.Drawing.drawTriangle(g, _ddRect, Color.Gray, Color.Black, (_group.IsCollapsed ? Renderer.Drawing.TriangleDirection.Down : Renderer.Drawing.TriangleDirection.DownRight));
				}
				// Draw checkbox
				if (_owner._checkBoxes & drawCheckBox) {
					Renderer.CheckBox.drawCheckBox(g, _chkRect, _group._checkState, , _owner.Enabled, _chkHover);
				}
				// Draw header and underline
				g.DrawString(_group.Text + " (" + Convert.ToString(visibleCount) + ")", _owner._groupFont, (_owner.Enabled ? new SolidBrush(System.Drawing.Color.White) : new SolidBrush(System.Drawing.Color.Gray)), txtRect, txtFormat);
				g.DrawLine((_owner.Enabled ? Pens.White : Pens.Gray), _location.X + _headerSize.Width, _location.Y + 2, _location.X + _headerSize.Width, _location.Y + _headerSize.Height - 3);
			} else {
				txtFormat.LineAlignment = StringAlignment.Center;
				switch (_group.TextAlign) {
					case HorizontalAlignment.Left:
						txtFormat.Alignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Center:
						txtFormat.Alignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						txtFormat.Alignment = StringAlignment.Far;
						break;
				}
				txtRect.X = _ddRect.Right + 2;
				if (_owner._checkBoxes)
					txtRect.X = _chkRect.Right + 2;
				txtRect.Width = _headerSize.Width - ((txtRect.X - _location.X) + 2);
				txtRect.Height = _headerSize.Height;
				// Draw drop down
				if (_owner.Enabled) {
					if (_owner._view == Control.View.Preview) {
						Renderer.Drawing.drawTriangle(g, _ddRect, (_ddHover ? System.Drawing.Color.Gold : System.Drawing.Color.White), Color.Black, (_group.IsCollapsed ? Renderer.Drawing.TriangleDirection.Right : Renderer.Drawing.TriangleDirection.DownRight));
					} else {
						Renderer.Drawing.drawTriangle(g, _ddRect, (_ddHover ? System.Drawing.Color.Gold : System.Drawing.Color.Black), Color.White, (_group.IsCollapsed ? Renderer.Drawing.TriangleDirection.Right : Renderer.Drawing.TriangleDirection.DownRight));
					}
				} else {
					Renderer.Drawing.drawTriangle(g, _ddRect, Color.Gray, (_owner._view == Control.View.Preview ? Color.Black : Color.White), (_group.IsCollapsed ? Renderer.Drawing.TriangleDirection.Right : Renderer.Drawing.TriangleDirection.DownRight));
				}
				// Draw checkbox
				if (_owner._checkBoxes & (drawCheckBox | _group._checkState != CheckState.Unchecked)) {
					Renderer.CheckBox.drawCheckBox(g, _chkRect, _group._checkState, , _owner.Enabled, _chkHover);
				}
				// Draw header and underline
				if (_owner._view == Control.View.Preview) {
					g.DrawString(_group.Text + " (" + Convert.ToString(visibleCount) + ")", _owner._groupFont, (_owner.Enabled ? new SolidBrush(System.Drawing.Color.White) : new SolidBrush(System.Drawing.Color.Gray)), txtRect, txtFormat);
					g.DrawLine((_owner.Enabled ? Pens.White : Pens.Gray), _location.X, _location.Y + _headerSize.Height, _location.X + _headerSize.Width, _location.Y + _headerSize.Height);
				} else {
					g.DrawString(_group.Text + " (" + Convert.ToString(visibleCount) + ")", _owner._groupFont, (_owner.Enabled ? new SolidBrush(System.Drawing.Color.Black) : new SolidBrush(System.Drawing.Color.Gray)), txtRect, txtFormat);
					g.DrawLine((_owner.Enabled ? Pens.Black : Pens.Gray), _location.X, _location.Y + _headerSize.Height, _location.X + _headerSize.Width, _location.Y + _headerSize.Height);
				}
			}
			// Perform item drawing when ListView.View <> Details
			if (_owner._view != View.Details) {
				if (!_group.IsCollapsed) {
					foreach (ListViewItemHost ih in _itemHosts) {
						switch (_owner._view) {
							case View.List:
								ih.drawList(g, drawCheckBox);
								break;
							case View.Tile:
								ih.drawTile(g, drawCheckBox);
								break;
							case Control.View.Preview:
								ih.drawPreview(g, drawCheckBox);
								break;
							case Control.View.Icon:
							case Control.View.Thumbnail:
								ih.drawIconThumbnail(g, drawCheckBox);
								break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Draw the checkbox of all ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		public void drawItemsCheckBox(Graphics g)
		{
			if (!_owner._showGroups)
				return;
			if (_groupSize.Width == 0 | _groupSize.Height == 0)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			if (this.Right < 0 | this.Bottom < 0)
				return;
			if (_group.IsCollapsed)
				return;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (lviHost.IsVisible)
					lviHost.drawCheckBox(g);
			}
		}
		/// <summary>
		/// Draw the row number of all ListViewItemHost contained in ListViewGroupHost.
		/// </summary>
		public void drawItemsRowNumber(Graphics g, StringFormat rowFormat, int x, int width, Brush rowBrush)
		{
			if (!_owner._showGroups)
				return;
			if (_groupSize.Width == 0 | _groupSize.Height == 0)
				return;
			if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
				return;
			if (this.Right < 0 | this.Bottom < 0)
				return;
			if (_group.IsCollapsed)
				return;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (lviHost.IsVisible & !lviHost.Frozen) {
					Rectangle rectRowNumber = new Rectangle(x, 0, width, 0);
					rectRowNumber.Y = lviHost.Y;
					rectRowNumber.Height = lviHost.Bounds.Height;
					g.DrawString(Convert.ToString(lviHost.RowIndex), _owner.Font, rowBrush, rectRowNumber, rowFormat);
				}
			}
		}
		// Mouse event handler
		/// <summary>
		/// Test whether the mouse pointer is moved over a ListViewGroupHost.
		/// </summary>
		public bool mouseMove(MouseEventArgs e)
		{
			if (!_owner._showGroups)
				return false;
			if (_groupSize.Width == 0 | _groupSize.Height == 0)
				return false;
			if (_location.X > _owner.Width | _location.Y > _owner.Height)
				return false;
			if (this.Bottom < 0 | this.Right < 0)
				return false;
			bool stateChanged = false;
			if (_ddRect.Contains(e.Location)) {
				if (!_ddHover) {
					_ddHover = true;
					stateChanged = true;
				}
			} else {
				if (_ddHover) {
					_ddHover = false;
					stateChanged = true;
				}
			}
			if (_owner._checkBoxes) {
				if (_chkRect.Contains(e.Location)) {
					if (!_chkHover) {
						_chkHover = true;
						stateChanged = true;
					}
				} else {
					if (_chkHover) {
						_chkHover = false;
						stateChanged = true;
					}
				}
			}
			if (!_group.IsCollapsed) {
				foreach (ListViewItemHost ih in _itemHosts) {
					if (ih.Visible)
						stateChanged = stateChanged | ih.mouseMove(e);
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the left button mouse is pressed over a ListViewGroupHost.
		/// </summary>
		public bool mouseDown(MouseButtons button = System.Windows.Forms.MouseButtons.Left)
		{
			bool stateChanged = false;
			if (_ddHover | _chkHover) {
				if (_ddHover) {
					// Change collapsed state
					_owner._internalThread = true;
					if (_group.IsCollapsed) {
						_group.expand();
					} else {
						_group.collapse();
					}
					_owner._internalThread = false;
				} else {
					// Change checked state
					_owner._internalThread = true;
					if (_group._checkState == CheckState.Checked) {
						_group.Checked = false;
						if (!_group.cancelStatus) {
							foreach (ListViewItemHost lviHost in _itemHosts) {
								lviHost.Item.setChecked(false);
							}
							_group._checkState = CheckState.Unchecked;
							_group.CheckedItems.Clear();
						}
					} else {
						_group.Checked = true;
						if (!_group.cancelStatus) {
							foreach (ListViewItemHost lviHost in _itemHosts) {
								if (!lviHost.Item.Checked) {
									lviHost.Item.setChecked(true);
									_group.CheckedItems.Add(lviHost.Item);
								}
							}
							_group._checkState = CheckState.Checked;
						}
					}
					_owner.changeColumnCheckState();
					_owner._internalThread = false;
				}
				stateChanged = true;
			}
			if (!_group.IsCollapsed) {
				foreach (ListViewItemHost ih in _itemHosts) {
					if (ih.Visible)
						stateChanged = stateChanged | ih.mouseDown(button);
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the mouse pointer is leaving the ListViewGroupHost.
		/// </summary>
		public bool mouseLeave()
		{
			bool stateChanged = false;
			if (_ddHover | _chkHover) {
				_ddHover = false;
				_chkHover = false;
				stateChanged = true;
			}
			if (!_group.IsCollapsed) {
				foreach (ListViewItemHost ih in _itemHosts) {
					if (ih.Visible)
						stateChanged = stateChanged | ih.mouseLeave();
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the left button mouse is released.
		/// </summary>
		public bool mouseUp()
		{
			bool stateChanged = false;
			if (!_group.IsCollapsed) {
				foreach (ListViewItemHost ih in _itemHosts) {
					if (ih.Visible)
						stateChanged = stateChanged | ih.mouseUp();
				}
			}
			return stateChanged;
		}
	}
	/// <summary>
	/// Class to represent a slider object.
	/// </summary>
	/// <remarks>This slider is used when the view property is set to Preview.</remarks>
	private class ItemSlider
	{
		ListView _owner;
		float _value = 0;
		System.Windows.Forms.Orientation _orientation = Orientation.Horizontal;
		Point _location = new Point(0, 0);
		int _width = 100;
		int _height = 5;
		bool _visible = true;
		float _max = 100;
		Rectangle _slideRect = new Rectangle(0, 0, 5, 5);
		bool _hoverSlide = false;
		bool _interceptMouseDown = false;
		Point _boxPoint;
			// Determine whether the object need to repaint.
		bool _changed = false;
		public event ValueChangedEventHandler ValueChanged;
		public delegate void ValueChangedEventHandler(object sender, EventArgs e);
		public ItemSlider(ListView owner)
		{
			_owner = owner;
		}
		/// <summary>
		/// Determine the orientation of the slider.
		/// </summary>
		public Windows.Forms.Orientation Orientation {
			get { return _orientation; }
			set {
				if (_orientation != value) {
					_orientation = value;
					if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
						_height = 5;
						_slideRect.X = _location.X + (_width * _value / _max) - 3;
						_slideRect.Y = _location.Y;
					} else {
						_width = 5;
						_slideRect.X = _location.X;
						_slideRect.Y = _location.Y + (_height * _value / _max) - 3;
					}
				}
			}
		}
		/// <summary>
		/// Determine the current value of the slider.
		/// </summary>
		public float Value {
			get { return _value; }
			set {
				if (_value != value) {
					if (value >= 0 & value <= _max) {
						_value = value;
						if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
							_slideRect.X = _location.X + (_width * _value / _max) - 3;
						} else {
							_slideRect.Y = _location.Y + (_height * _value / _max) - 3;
						}
						_changed = true;
						if (ValueChanged != null) {
							ValueChanged(this, new EventArgs());
						}
					}
				}
			}
		}
		/// <summary>
		/// Determine the top left position of the slider.
		/// </summary>
		public Point Location {
			get { return _location; }
			set {
				if (_location != value) {
					_location = value;
					if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
						_slideRect.X = _location.X + (_width * _value / _max) - 3;
						_slideRect.Y = _location.Y;
					} else {
						_slideRect.X = _location.X;
						_slideRect.Y = _location.Y + (_height * _value / _max) - 3;
					}
					_changed = true;
				}
			}
		}
		/// <summary>
		/// Determine whether the slider is visible.
		/// </summary>
		public bool Visible {
			get { return _visible; }
			set {
				if (_visible != value) {
					_visible = value;
					if (!_visible) {
						_hoverSlide = false;
						_interceptMouseDown = false;
					}
					_changed = true;
				}
			}
		}
		/// <summary>
		/// Determine the width of the slider.
		/// </summary>
		public int Width {
			get { return _width; }
			set {
				if (_orientation == System.Windows.Forms.Orientation.Vertical)
					return;
				if (_width == value)
					return;
				if (value > 20) {
					_width = value;
					_slideRect.X = _location.X + (_width * _value / _max) - 3;
					_changed = true;
				}
			}
		}
		/// <summary>
		/// Determine the height of the slider.
		/// </summary>
		public int Height {
			get { return _height; }
			set {
				if (_orientation == System.Windows.Forms.Orientation.Horizontal)
					return;
				if (_height == value)
					return;
				if (value > 20) {
					_height = value;
					_slideRect.Y = _location.Y + (_height * _value / _max) - 3;
					_changed = true;
				}
			}
		}
		/// <summary>
		/// Determine whether the slider need the change its visual representation.
		/// </summary>
		public bool Changed {
			get { return _changed; }
			set { _changed = value; }
		}
		/// <summary>
		/// Determine the maximum value of the slider.
		/// </summary>
		public int Maximum {
			get { return _max; }
			set {
				if (value <= 0)
					return;
				if (_max != value) {
					_max = value;
					_changed = true;
					if (_value > _max) {
						this.Value = _max;
					} else {
						if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
							_slideRect.X = _location.X + (_width * _value / _max) - 3;
						} else {
							_slideRect.Y = _location.Y + (_height * _value / _max) - 3;
						}
					}
				}
			}
		}
		/// <summary>
		/// Gets the rightmost location of the slider.
		/// </summary>
		public int Right {
			get { return _location.X + _width; }
		}
		/// <summary>
		/// Gets the bottommost location of the slider.
		/// </summary>
		public int Bottom {
			get { return _location.Y + _height; }
		}
		/// <summary>
		/// Gets a value indicating the slider is reacted to the mouse event, even if the mouse is outside of the slider.
		/// </summary>
		public bool InterceptMouseDown {
			get { return _interceptMouseDown; }
		}
		/// <summary>
		/// Gets the bounding rectangle of the slider.
		/// </summary>
		public Rectangle Bounds {
			get { return new Rectangle(_location.X, _location.Y, _width, _height); }
		}
		/// <summary>
		/// Draw the slider button object on specified Graphics object.
		/// </summary>
		private void drawSlider(Graphics g)
		{
			LinearGradientBrush sliderBrush = new LinearGradientBrush(_slideRect, Color.Black, Color.White, LinearGradientMode.Vertical);
			Pen borderPen = null;
			if (_visible) {
				if (_hoverSlide) {
					sliderBrush.InterpolationColors = Ai.Renderer.Button.HLitedBlend;
					borderPen = Ai.Renderer.Button.HLitedBorderPen;
				} else {
					sliderBrush.InterpolationColors = Ai.Renderer.Button.NormalBlend;
					borderPen = Ai.Renderer.Button.NormalBorderPen;
				}
			} else {
				sliderBrush.InterpolationColors = Ai.Renderer.Button.DisabledBlend;
				borderPen = Ai.Renderer.Button.DisabledBorderPen;
			}
			g.FillEllipse(sliderBrush, _slideRect);
			g.DrawEllipse(borderPen, _slideRect);
			sliderBrush.Dispose();
			borderPen.Dispose();
		}
		/// <summary>
		/// Draw the whole slider object on a specified Graphics object.
		/// </summary>
		public void draw(Graphics g)
		{
			if (!_visible)
				return;
			if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
				if (_owner.Enabled) {
					g.DrawLine(Pens.White, _location.X, _location.Y + 2, _location.X + _width, _location.Y + 2);
				} else {
					g.DrawLine(Pens.Gray, _location.X, _location.Y + 2, _location.X + _width, _location.Y + 2);
				}
			} else {
				if (_owner.Enabled) {
					g.DrawLine(Pens.White, _location.X + 2, _location.Y, _location.X + 2, _location.Y + _height);
				} else {
					g.DrawLine(Pens.Gray, _location.X + 2, _location.Y, _location.X + 2, _location.Y + _height);
				}
			}
			drawSlider(g);
			_changed = false;
		}
		/// <summary>
		/// Test whether the mouse pointer is moved over the slider.
		/// </summary>
		public void mouseMove(Point p)
		{
			if (_visible) {
				if (!_interceptMouseDown) {
					if (_slideRect.Contains(p)) {
						if (!_hoverSlide) {
							_hoverSlide = true;
							_changed = true;
						}
					} else {
						Rectangle rectArea = new Rectangle(_location, new Size(_width, _height));
						if (rectArea.Contains(p)) {
							_boxPoint = new Point(p.X - _location.X, p.Y - _location.Y);
						} else {
							_boxPoint = new Point(0, 0);
						}
						if (_hoverSlide) {
							_hoverSlide = false;
							_changed = true;
						}
					}
				} else {
					float _newValue = 0;
					if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
						_newValue = (p.X - _location.X) * _max / _width;
					} else {
						_newValue = (p.Y - _location.Y) * _max / _height;
					}
					if (_newValue < 0)
						_newValue = 0;
					if (_newValue > _max)
						_newValue = _max;
					Value = _newValue;
				}
			}
		}
		/// <summary>
		/// Test whether the mouse left button is pressed over the slider.
		/// </summary>
		public void mouseDown()
		{
			if (_visible) {
				if (_hoverSlide) {
					_interceptMouseDown = true;
				} else {
					if (_boxPoint.X > 0 | _boxPoint.Y > 0) {
						float _newValue = 0;
						if (_orientation == System.Windows.Forms.Orientation.Horizontal) {
							_newValue = _boxPoint.X * _max / _width;
						} else {
							_newValue = _boxPoint.Y * _max / _height;
						}
						if (_newValue < 0)
							_newValue = 0;
						if (_newValue > _max)
							_newValue = _max;
						Value = _newValue;
					}
				}
			}
		}
		/// <summary>
		/// Test whether the mouse pointer is leaving the slider.
		/// </summary>
		public void mouseLeave()
		{
			if (_visible) {
				_boxPoint = new Point(0, 0);
				if (_hoverSlide) {
					_hoverSlide = false;
					_changed = true;
				}
			}
		}
		/// <summary>
		/// Test whether the mouse left button is released over the slider.
		/// </summary>
		public void mouseUp()
		{
			_interceptMouseDown = false;
		}
		/// <summary>
		/// Increase the current value of the slider.
		/// </summary>
		public void increaseValue()
		{
			float newValue = _value + 1;
			if (newValue > _max)
				newValue = _max;
			Value = newValue;
		}
		/// <summary>
		/// Decrease the current value of the slider.
		/// </summary>
		public void decreaseValue()
		{
			float newValue = _value - 1;
			if (newValue < 0)
				newValue = 0;
			Value = newValue;
		}
	}
	/// <summary>
	/// TextBox for label editing.
	/// </summary>
	private class TextBoxLabelEditor : TextBox
	{
		public TextBoxLabelEditor() : base()
		{
		}
		protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					return true;
				case Keys.Return:
					if (!Multiline)
						return true;
					break;
				case Keys.Control | Keys.Return:
					if (Multiline)
						return true;
					break;
			}
			return base.IsInputKey(keyData);
		}
	}
	#endregion
	#region "Private Routines."
	/// <summary>
	/// Gets the maximum width of subitem in all of the items specified by its index.
	/// </summary>
	[Description("Gets the maximum width of subitem in all of the items specified by its index.")]
	private int getSubItemMaxWidth(int index)
	{
		int maxWidth = 0;
		SizeF iSize = default(SizeF);
		foreach (ListViewItemHost ih in _itemHosts) {
			iSize = ih.getSubItemSize(index);
			if (maxWidth < iSize.Width)
				maxWidth = Math.Ceiling(iSize.Width);
		}
		return maxWidth;
	}
	/// <summary>
	/// Gets the maximum width of subitem in all of the items specified by its column.
	/// </summary>
	[Description("Gets the maximum width of subitem in all of the items specified by its column.")]
	private int getSubItemMaxWidth(ColumnHeader column)
	{
		return getSubItemMaxWidth(_columns.IndexOf[column]);
	}
	/// <summary>
	/// Compares two ListViewItemHosts for equivalence.  Depending on the column sort order.
	/// </summary>
	[Description("Compares two ListViewItemHosts for equivalence.  Depending on the column sort order.")]
	private int itemHostComparer(ListViewItemHost host1, ListViewItemHost host2)
	{
		if (_columnRef == -1)
			return 0;
		// Check whether both hosts is visible
		if (host1.Visible & host2.Visible) {
			List<int> colsToCompare = new List<int>();
			int i = 0;
			int result = 0;
			int currentColumn = 0;
			// Create a sequence of column indexes to perform comparison
			// Add sorted column index to the first sequence
			colsToCompare.Add(_columnRef);
			i = 0;
			while (i < _columns.Count) {
				if (i != _columnRef)
					colsToCompare.Add(i);
				i = i + 1;
			}
			i = 0;
			while (i < colsToCompare.Count & result == 0) {
				currentColumn = colsToCompare[i];
				try {
					object vNode1 = null;
					object vNode2 = null;
					Comparer objCompare = new Comparer(_ci);
					vNode1 = host1.Item.SubItems[currentColumn].Value;
					vNode2 = host2.Item.SubItems[currentColumn].Value;
					if (_columns[currentColumn].SortOrder == SortOrder.Ascending) {
						result = objCompare.Compare(vNode1, vNode2);
					} else {
						result = -objCompare.Compare(vNode1, vNode2);
					}
				} catch (Exception ex) {
					result = 0;
				}
				i = i + 1;
			}
			return result;
		} else {
			if (host1.Visible) {
				if (_columns[_columnRef].SortOrder == SortOrder.Ascending) {
					return 1;
				} else {
					return -1;
				}
			} else if (host2.Visible) {
				if (_columns[_columnRef].SortOrder == SortOrder.Ascending) {
					return -1;
				} else {
					return 1;
				}
			}
			return 0;
		}
	}
	/// <summary>
	/// Compares the Text of two ListViewGroupHosts for equivalence.
	/// </summary>
	[Description("Compares the Text of two ListViewGroupHosts for equivalence.")]
	private int groupHostComparer(ListViewGroupHost host1, ListViewGroupHost host2)
	{
		if (_columnRef == -1)
			return 0;
		if (_columns[_columnRef].SortOrder == SortOrder.Ascending) {
			return string.Compare(host1.Group.Text, host2.Group.Text, true);
		} else {
			return -string.Compare(host1.Group.Text, host2.Group.Text, true);
		}
	}
	/// <summary>
	/// Filter a ListViewItem based on existing filter parameters on each columns.
	/// </summary>
	[Description("Filter a ListViewItem based on existing filter parameters on each columns.")]
	private bool filterItem(ListViewItem item)
	{
		List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
		int i = 0;
		bool result = true;
		while (i <= item.SubItems.Count & i < handlers.Count) {
			result = result & handlers[i].filterValue(item.SubItems[i].Value);
			i = i + 1;
		}
		return result;
	}
	/// <summary>
	/// Filter a ListViewItem based on specified filter parameters.
	/// </summary>
	[Description("Filter a ListViewItem based on specified filter parameters.")]
	private bool filterItem(ListViewItem item, List<ColumnFilterHandle> handlers)
	{
		int i = 0;
		bool result = true;
		while (i <= item.SubItems.Count & i < handlers.Count) {
			result = result & handlers[i].filterValue(item.SubItems[i].Value);
			i = i + 1;
		}
		return result;
	}
	/// <summary>
	/// Change the check state of the checkbox that appears at column header.
	/// </summary>
	[Description("Change the check state of the checkbox that appears at column header.")]
	private void changeColumnCheckState()
	{
		int checkedCount = 0;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (lviHost.Item.Checked)
				checkedCount += 1;
		}
		if (checkedCount == _itemHosts.Count) {
			_columnControl.CheckState = CheckState.Checked;
		} else {
			if (checkedCount == 0) {
				_columnControl.CheckState = CheckState.Unchecked;
			} else {
				_columnControl.CheckState = CheckState.Indeterminate;
			}
		}
	}
	/// <summary>
	/// Sort all existing hosts.
	/// </summary>
	[Description("Sort all existing hosts.")]
	private void sortAll()
	{
		if (_columnRef == -1)
			return;
		if (_showGroups) {
			_groupHosts.Sort(groupHostComparer);
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				lvgHost.sortItems();
			}
			_defaultGroupHost.sortItems();
		} else {
			_itemHosts.Sort(itemHostComparer);
		}
	}
	/// <summary>
	/// Measure all existing hosts, and the client area used to display all hosts.
	/// </summary>
	private void measureAll()
	{
		if (_view == Control.View.Preview) {
			_vScroll.Visible = false;
			_hScroll.Visible = false;
		} else {
			_slider.Visible = false;
		}
		if (_itemHosts.Count == 0) {
			if (_view == Control.View.Details) {
				_clientArea = new Rectangle(1, _columnControl.Bottom + 1, this.Width - (_vScroll.Width + 2), this.Height - (_columnControl.Bottom + 2));
				if (_checkBoxes | _rowNumbers) {
					if (_checkBoxes) {
						_clientArea.X = 23;
						_clientArea.Width -= 22;
					}
					if (_rowNumbers) {
						_clientArea.X += _gObj.MeasureString("0", Font).Width + 2;
						_clientArea.Width -= (_gObj.MeasureString("0", Font).Width + 2);
					}
				}
				_hScroll.Left = 1;
				_hScroll.Top = this.Height - (_hScroll.Height + 1);
				_hScroll.Width = this.Width - 2;
				int columnsWidth = _columnControl.ColumnsWidth;
				if (columnsWidth > this.Width - (_vScroll.Width + _clientArea.X)) {
					_hScroll.Visible = true;
					_hScroll.SmallChange = _clientArea.Width / 20;
					_hScroll.LargeChange = _clientArea.Width / 10;
					_hScroll.Maximum = (columnsWidth - _clientArea.Width) + _hScroll.LargeChange;
				} else {
					_hScroll.Visible = false;
				}
			} else {
				_clientArea = new Rectangle(1, _columnControl.Bottom + 1, this.Width - 2, this.Height - (_columnControl.Bottom + 2));
			}
			return;
		}
		int itemsHeight = 0;
		int itemsWidth = 0;
		if (_view == Control.View.Details)
			itemsWidth = _columnControl.ColumnsWidth;
		if (_showGroups) {
			if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top)) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.measureSize();
					if (lvgHost.IsVisible)
						itemsWidth += lvgHost.GroupSize.Width + 5;
				}
				_defaultGroupHost.measureSize();
				if (_defaultGroupHost.IsVisible)
					itemsWidth += _defaultGroupHost.GroupSize.Width;
				itemsHeight = _slideItemSize + 5;
			} else {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.measureSize();
					if (lvgHost.IsVisible) {
						if (itemsWidth < lvgHost.GroupSize.Width)
							itemsWidth = lvgHost.GroupSize.Width;
						itemsHeight += lvgHost.GroupSize.Height + 5;
					}
				}
				_defaultGroupHost.measureSize();
				if (_defaultGroupHost.IsVisible) {
					if (itemsWidth < _defaultGroupHost.GroupSize.Width)
						itemsWidth = _defaultGroupHost.GroupSize.Width;
					itemsHeight += _defaultGroupHost.GroupSize.Height;
				}
				if (_view == Control.View.Preview)
					itemsWidth = _slideItemSize + 5;
			}
			// Calculating the frozen items if _view = Details
			if (_view == Control.View.Details) {
				foreach (ListViewItemHost lviHost in _frozenHosts) {
					lviHost.measureSize();
					if (lviHost.Visible)
						itemsHeight += lviHost.Bounds.Height + 1;
				}
			}
		} else {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				lviHost.measureSize();
			}
			int maxWidth = Width - (_vScroll.Width + 2);
			switch (_view) {
				case Control.View.Details:
					foreach (ListViewItemHost lviHost in _itemHosts) {
						if (lviHost.Visible) {
							itemsHeight += lviHost.Bounds.Height + 1;
						}
					}

					break;
				case Control.View.Icon:
				case Control.View.Thumbnail:
				case Control.View.Tile:
				case Control.View.List:
					int xRange = 0;
					int yRange = 0;
					switch (_view) {
						case View.Icon:
							xRange = _iconSize + 30;
							yRange = _iconSize + 50;
							break;
						case View.List:
							xRange = _listColumnWidth;
							break;
						case View.Thumbnail:
							xRange = _thumbnailSize + 30;
							yRange = _thumbnailSize + 50;
							break;
						case View.Tile:
							xRange = _tileColumnWidth;
							yRange = _tileHeight + 5;
							break;
					}
					int rowWidth = 0;
					foreach (ListViewItemHost ih in _itemHosts) {
						if (ih.Visible) {
							rowWidth = rowWidth + xRange;
							if (_view == View.List) {
								if (yRange < ih.Bounds.Height)
									yRange = ih.Bounds.Height;
							}
							if (rowWidth + xRange > maxWidth) {
								if (_view == View.List)
									yRange += 1;
								itemsHeight = itemsHeight + yRange;
								itemsWidth = rowWidth;
								rowWidth = 0;
								if (_view == View.List)
									yRange = 0;
							}
						}
					}

					if (rowWidth > 0)
						itemsHeight += yRange;
					break;
				case Control.View.Preview:
					if (_slideLocation == Control.SlideLocation.Top | _slideLocation == Control.SlideLocation.Bottom) {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible)
								itemsWidth += lviHost.Bounds.Width + 5;
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible)
								itemsHeight += lviHost.Bounds.Height + 5;
						}
					}
					break;
			}
		}
		_clientArea.X = 1;
		_clientArea.Y = _columnControl.Bottom + 1;
		_frozenArea = new Rectangle(0, 0, 0, 0);
		int heightUsed = _clientArea.Y;
		switch (_view) {
			case Control.View.Details:
				if (_checkBoxes)
					_clientArea.X += 22;
				if (_rowNumbers)
					_clientArea.X += _gObj.MeasureString(Convert.ToString(_items.Count), Font).Width + 2;
				_clientArea.Width = this.Width - (_vScroll.Width + _clientArea.X);
				if (itemsWidth > this.Width - (_vScroll.Width + _clientArea.X)) {
					_hScroll.Visible = true;
					_hScroll.SmallChange = _clientArea.Width / 20;
					_hScroll.LargeChange = _clientArea.Width / 10;
					_hScroll.Maximum = (itemsWidth - _clientArea.Width) + _hScroll.LargeChange;
					heightUsed += _hScroll.Height;
				} else {
					_hScroll.Visible = false;
				}
				_clientArea.Height = this.Height - heightUsed;
				if (itemsHeight > _clientArea.Height) {
					_vScroll.Visible = true;
					_vScroll.SmallChange = _clientArea.Height / 20;
					_vScroll.LargeChange = _clientArea.Height / 10;
					_vScroll.Maximum = (itemsHeight - _clientArea.Height) + _vScroll.LargeChange;
				} else {
					_vScroll.Visible = false;
				}
				// Calculating frozen rows
				_frozenArea.X = _clientArea.X;
				_frozenArea.Width = _clientArea.Width;
				foreach (ListViewItemHost lviHost in _frozenHosts) {
					if (lviHost.Visible)
						_frozenArea.Height += lviHost.Bounds.Height + 1;
				}

				if (_frozenRowsDock == RowsDock.Top) {
					_frozenArea.Y = _clientArea.Y - 1;
				} else {
					_frozenArea.Y = _clientArea.Bottom - _frozenArea.Height;
				}
				break;
			case Control.View.Preview:
				_vScroll.Visible = false;
				_hScroll.Visible = false;
				_clientArea.Width = this.Width - 2;
				_clientArea.Height = this.Height - (heightUsed + 1);
				if (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top) {
					_slider.Orientation = Orientation.Horizontal;
					if (_slideLocation == Control.SlideLocation.Bottom) {
						_slider.Location = new Point(10, this.Height - (itemsHeight + _slider.Height + 1));
					} else {
						_slider.Location = new Point(10, _clientArea.Y + itemsHeight);
					}
					_slider.Width = _clientArea.Width - 20;
					_slider.Visible = itemsWidth > _clientArea.Width;
					if (_slider.Visible)
						_slider.Maximum = itemsWidth - _clientArea.Width;
				} else {
					_slider.Orientation = Orientation.Vertical;
					if (_slideLocation == Control.SlideLocation.Left) {
						_slider.Location = new Point(itemsWidth + 1, _clientArea.Y + 10);
					} else {
						_slider.Location = new Point(this.Width - (itemsWidth + _slider.Width + 1), _clientArea.Y + 10);
					}
					_slider.Height = _clientArea.Height - 20;
					_slider.Visible = itemsHeight > _clientArea.Height;
					if (_slider.Visible)
						_slider.Maximum = itemsHeight - _clientArea.Height;
				}
				break;
			default:
				_clientArea.Width = this.Width - (_vScroll.Width + _clientArea.X);
				if (itemsWidth > this.Width - (_vScroll.Width + _clientArea.X)) {
					_hScroll.Visible = true;
					_hScroll.SmallChange = _clientArea.Width / 20;
					_hScroll.LargeChange = _clientArea.Width / 10;
					_hScroll.Maximum = (itemsWidth - _clientArea.Width) + _hScroll.LargeChange;
					heightUsed += _hScroll.Height;
				} else {
					_hScroll.Visible = false;
				}
				_clientArea.Height = this.Height - heightUsed;
				if (itemsHeight > _clientArea.Height) {
					_vScroll.Visible = true;
					_vScroll.SmallChange = _clientArea.Height / 20;
					_vScroll.LargeChange = _clientArea.Height / 10;
					_vScroll.Maximum = (itemsHeight - _clientArea.Height) + _vScroll.LargeChange;
				} else {
					_vScroll.Visible = false;
				}
				break;
		}
		_vScroll.Top = _clientArea.Y;
		_vScroll.Left = this.Width - (_vScroll.Width + 1);
		_vScroll.Height = _clientArea.Height;
		_hScroll.Left = 1;
		_hScroll.Top = this.Height - (_hScroll.Height + 1);
		_hScroll.Width = (_vScroll.Visible ? this.Width - (_vScroll.Width + 2) : this.Width - 2);
	}
	/// <summary>
	/// Relocate all existing hosts, scrollbars, and slider.
	/// </summary>
	private void relocateAll()
	{
		if (_itemHosts.Count == 0)
			return;
		int x = _clientArea.X;
		int y = _clientArea.Y;
		if (_view == Control.View.Preview) {
			if (_slider.Visible) {
				if (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top) {
					x -= _slider.Value;
				} else {
					y -= _slider.Value;
				}
			}
		} else {
			if (_view != Control.View.Details) {
				if (_hScroll.Visible)
					x -= _hScroll.Value;
			}
			if (_vScroll.Visible)
				y -= _vScroll.Value;
		}
		if (_view == Control.View.Details) {
			// Relocate all frozen items
			if (_frozenRowsDock == RowsDock.Top) {
				_frozenArea.Y = _clientArea.Y - 2;
			} else {
				_frozenArea.Y = _clientArea.Bottom - _frozenArea.Height;
			}
			int frozenY = _frozenArea.Y;
			foreach (ListViewItemHost lviHost in _frozenHosts) {
				lviHost.Y = frozenY;
				if (lviHost.Visible)
					frozenY = lviHost.Bottom + 1;
			}
		}
		if (_showGroups) {
			switch (_view) {
				case Control.View.Details:
					if (_frozenRowsDock == RowsDock.Top)
						y += _frozenArea.Height;
					foreach (ListViewGroupHost lvgHost in _groupHosts) {
						lvgHost.Y = y;
						lvgHost.relocate();
						if (lvgHost.IsVisible)
							y = lvgHost.Bottom + 5;
					}

					_defaultGroupHost.Y = y;
					_defaultGroupHost.relocate();
					break;
				case Control.View.Preview:
					if (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (_slideLocation == Control.SlideLocation.Top) {
								lvgHost.Y = _clientArea.Y;
							} else {
								lvgHost.Y = _clientArea.Bottom - (_slideItemSize + 1);
							}
							lvgHost.X = x;
							lvgHost.relocate();
							if (lvgHost.IsVisible)
								x = lvgHost.Right + 5;
						}
						if (_slideLocation == Control.SlideLocation.Top) {
							_defaultGroupHost.Y = _clientArea.Y;
						} else {
							_defaultGroupHost.Y = _clientArea.Bottom - (_slideItemSize + 1);
						}
						_defaultGroupHost.X = x;
						_defaultGroupHost.relocate();
					} else {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (_slideLocation == Control.SlideLocation.Left) {
								lvgHost.X = _clientArea.X;
							} else {
								lvgHost.X = _clientArea.Right - (_slideItemSize + 1);
							}
							lvgHost.Y = y;
							lvgHost.relocate();
							if (lvgHost.IsVisible)
								y = lvgHost.Bottom + 5;
						}
						if (_slideLocation == Control.SlideLocation.Left) {
							_defaultGroupHost.X = _clientArea.X;
						} else {
							_defaultGroupHost.X = _clientArea.Right - (_slideItemSize + 1);
						}
						_defaultGroupHost.Y = y;
						_defaultGroupHost.relocate();
					}
					break;
				default:
					foreach (ListViewGroupHost lvgHost in _groupHosts) {
						lvgHost.X = x;
						lvgHost.Y = y;
						lvgHost.relocate();
						if (lvgHost.IsVisible)
							y = lvgHost.Bottom + 5;
					}

					_defaultGroupHost.Y = y;
					_defaultGroupHost.X = x;
					_defaultGroupHost.relocate();
					break;
			}
		} else {
			switch (_view) {
				case Control.View.Details:
					if (_frozenRowsDock == RowsDock.Top)
						y += _frozenArea.Height;
					foreach (ListViewItemHost lviHost in _itemHosts) {
						lviHost.Y = y;
						lviHost.relocateSubItems();
						if (lviHost.Visible & !lviHost.Frozen)
							y = lviHost.Bounds.Bottom + 1;
					}

					break;
				case Control.View.Preview:
					if (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top) {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (_slideLocation == Control.SlideLocation.Top) {
								lviHost.Y = _clientArea.Y;
							} else {
								lviHost.Y = _clientArea.Bottom - (_slideItemSize + 1);
							}
							lviHost.X = x;
							if (lviHost.Visible)
								x = lviHost.Right + 5;
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (_slideLocation == Control.SlideLocation.Left) {
								lviHost.X = _clientArea.X;
							} else {
								lviHost.X = _clientArea.Right - (_slideItemSize + 1);
							}
							lviHost.Y = y;
							if (lviHost.Visible)
								y = lviHost.Bottom + 5;
						}
					}
					break;
				default:
					int xRange = 0;
					int yRange = 0;
					switch (_view) {
						case View.Icon:
							xRange = _iconSize + 30;
							yRange = _iconSize + 50;
							break;
						case View.List:
							xRange = _listColumnWidth;
							break;
						case View.Thumbnail:
							xRange = _thumbnailSize + 30;
							yRange = _thumbnailSize + 50;
							break;
						case View.Tile:
							xRange = _tileColumnWidth;
							yRange = _tileHeight + 5;
							break;
					}
					int itemX = x;
					int itemY = y;
					foreach (ListViewItemHost lviHost in _itemHosts) {
						if (lviHost.Visible) {
							lviHost.X = itemX;
							lviHost.Y = itemY;
							itemX = itemX + xRange;
							if (_view == View.List) {
								if (yRange < lviHost.Bounds.Height)
									yRange = lviHost.Bounds.Height;
							}
							if (itemX + xRange > _clientArea.Right) {
								if (_view == View.List)
									yRange += 1;
								itemY = itemY + yRange;
								itemX = x;
								if (_view == View.List)
									yRange = 0;
							}
						}
					}

					break;
			}
		}
	}
	/// <summary>
	/// Gets an upper and nearest ListViewItemHost from specified ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getUpperHost(ListViewItemHost aHost, List<ListViewItemHost> itemHosts, bool searchFromBottom = false)
	{
		if (aHost == null)
			return null;
		if (_itemHosts.Count == 0)
			return null;
		int hostIndex = itemHosts.IndexOf(aHost) - 1;
		if (hostIndex < 0) {
			if (searchFromBottom)
				hostIndex = itemHosts.Count - 1;
		}
		while (hostIndex >= 0) {
			if (itemHosts[hostIndex].Visible) {
				if (itemHosts[hostIndex].X == aHost.X)
					return itemHosts[hostIndex];
			}
			hostIndex -= 1;
		}
		return null;
	}
	/// <summary>
	/// Gets a lower and nearest ListViewItemHost from specified ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getLowerHost(ListViewItemHost aHost, List<ListViewItemHost> itemHosts, bool searchFromTop = false)
	{
		if (aHost == null)
			return null;
		if (_itemHosts.Count == 0)
			return null;
		int hostIndex = itemHosts.IndexOf(aHost) + 1;
		if (hostIndex == 0 & !searchFromTop)
			return null;
		while (hostIndex < itemHosts.Count) {
			if (itemHosts[hostIndex].Visible) {
				if (itemHosts[hostIndex].X == aHost.X)
					return itemHosts[hostIndex];
			}
			hostIndex += 1;
		}
		return null;
	}
	/// <summary>
	/// Gets a left side and nearest ListViewItemHost from specified ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getLeftSideHost(ListViewItemHost aHost, List<ListViewItemHost> itemHosts)
	{
		if (aHost == null)
			return null;
		if (_itemHosts.Count == 0)
			return null;
		int hostIndex = itemHosts.IndexOf(aHost) - 1;
		if (hostIndex < 0)
			hostIndex = itemHosts.Count - 1;
		while (hostIndex >= 0) {
			if (itemHosts[hostIndex].Visible) {
				if (itemHosts[hostIndex].Y == aHost.Y)
					return itemHosts[hostIndex];
			}
			hostIndex -= 1;
		}
		return null;
	}
	/// <summary>
	/// Gets a right side and nearest ListViewItemHost from specified ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getRightSideHost(ListViewItemHost aHost, List<ListViewItemHost> itemHosts)
	{
		if (aHost == null)
			return null;
		if (_itemHosts.Count == 0)
			return null;
		int hostIndex = itemHosts.IndexOf(aHost) + 1;
		while (hostIndex < itemHosts.Count) {
			if (itemHosts[hostIndex].Visible) {
				if (itemHosts[hostIndex].Y == aHost.Y)
					return itemHosts[hostIndex];
			}
			hostIndex += 1;
		}
		return null;
	}
	/// <summary>
	/// Gets the first visible ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getFirstVisibleHost(List<ListViewItemHost> itemHost)
	{
		foreach (ListViewItemHost lviHost in itemHost) {
			if (lviHost.Visible)
				return lviHost;
		}
		return null;
	}
	/// <summary>
	/// Gets the first visible ListViewItemHost in all existing ListViewItemHost.
	/// </summary>
	private ListViewItemHost getFirstVisibleHost(string startsWith = "")
	{
		if (_itemHosts.Count == 0)
			return null;
		if (_showGroups) {
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				if (lvgHost.IsVisible & !lvgHost.Group.IsCollapsed) {
					foreach (ListViewItemHost lviHost in lvgHost.ItemHosts) {
						if (lviHost.Visible & lviHost.Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
							return lviHost;
					}
				}
			}
			if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed) {
				foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
					if (lviHost.Visible & lviHost.Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
						return lviHost;
				}
			}
		} else {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (lviHost.Visible & lviHost.Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
					return lviHost;
			}
		}
		return null;
	}
	/// <summary>
	/// Gets the last visible ListViewItemHost in a list of ListViewItemHost.
	/// </summary>
	private ListViewItemHost getLastVisibleHost(List<ListViewItemHost> itemHost)
	{
		int i = itemHost.Count - 1;
		while (i >= 0) {
			if (itemHost[i].Visible)
				return itemHost[i];
			i -= 1;
		}
		return null;
	}
	/// <summary>
	/// Gets the next visible ListViewItemHost in all existing ListViewItemHost from specified ListViewItemHost.
	/// </summary>
	private ListViewItemHost getNextVisibleHost(ListViewItemHost aHost, string startsWith = "")
	{
		if (_itemHosts.Count == 0)
			return null;
		if (aHost == null)
			return getFirstVisibleHost(startsWith);
		if (_showGroups) {
			int itemIndex = 0;
			if (!object.ReferenceEquals(aHost.GroupHost, _defaultGroupHost)) {
				int groupIndex = _groupHosts.IndexOf(aHost.GroupHost);
				while (groupIndex < _groupHosts.Count) {
					if (!_groupHosts[groupIndex].Group.IsCollapsed) {
						itemIndex = _groupHosts[groupIndex].ItemHosts.IndexOf(aHost) + 1;
						while (itemIndex < _groupHosts[groupIndex].ItemHosts.Count) {
							if (_groupHosts[groupIndex].ItemHosts[itemIndex].Visible & _groupHosts[groupIndex].ItemHosts[itemIndex].Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
								return _groupHosts[groupIndex].ItemHosts[itemIndex];
							itemIndex += 1;
						}
					}
					groupIndex += 1;
				}
			}
			if (_defaultGroup.IsCollapsed)
				return null;
			itemIndex = _defaultGroupHost.ItemHosts.IndexOf(aHost) + 1;
			while (itemIndex < _defaultGroupHost.ItemHosts.Count) {
				if (_defaultGroupHost.ItemHosts[itemIndex].Visible & _defaultGroupHost.ItemHosts[itemIndex].Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
					return _defaultGroupHost.ItemHosts[itemIndex];
				itemIndex += 1;
			}
		} else {
			int nextIndex = _itemHosts.IndexOf(aHost) + 1;
			while (nextIndex < _itemHosts.Count) {
				if (_itemHosts[nextIndex].Visible & _itemHosts[nextIndex].Item.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
					return _itemHosts[nextIndex];
				nextIndex += 1;
			}
		}
		return null;
	}
	/// <summary>
	/// Gets the previous visible ListViewItemHost in all existing ListViewItemHost from specified ListViewItemHost.
	/// </summary>
	private ListViewItemHost getPrevVisibleHost(ListViewItemHost aHost)
	{
		if (_itemHosts.Count == 0)
			return null;
		if (aHost == null)
			return getFirstVisibleHost();
		if (_showGroups) {
			int itemIndex = 0;
			if (object.ReferenceEquals(aHost.GroupHost, _defaultGroupHost)) {
				if (!_defaultGroup.IsCollapsed) {
					itemIndex = _defaultGroupHost.ItemHosts.IndexOf(aHost) - 1;
					while (itemIndex >= 0) {
						if (_defaultGroupHost.ItemHosts[itemIndex].Visible)
							return _defaultGroupHost.ItemHosts[itemIndex];
						itemIndex -= 1;
					}
				}
			}
			int groupIndex = _groupHosts.IndexOf(aHost.GroupHost);
			if (groupIndex == -1)
				groupIndex = _groupHosts.Count - 1;
			while (groupIndex >= 0) {
				if (!_groupHosts[groupIndex].Group.IsCollapsed) {
					itemIndex = _groupHosts[groupIndex].ItemHosts.IndexOf(aHost) - 1;
					if (itemIndex == -2)
						itemIndex = _groupHosts[groupIndex].ItemHosts.Count - 1;
					while (itemIndex >= 0) {
						if (_groupHosts[groupIndex].ItemHosts[itemIndex].Visible)
							return _groupHosts[groupIndex].ItemHosts[itemIndex];
						itemIndex -= 1;
					}
				}
				groupIndex -= 1;
			}
		} else {
			int prevIndex = _itemHosts.IndexOf(aHost) - 1;
			while (prevIndex >= 0) {
				if (_itemHosts[prevIndex].Visible)
					return _itemHosts[prevIndex];
				prevIndex -= 1;
			}
		}
		return null;
	}
	/// <summary>
	/// Sets the selected ListViewItemHost to a specified ListViewItemHost.
	/// </summary>
	private void setSelectedHost(ListViewItemHost host)
	{
		bool selectedHostChanged = false;
		if (_selectedHost == null)
			selectedHostChanged = true;
		if (!object.ReferenceEquals(_selectedHost, host))
			selectedHostChanged = true;
		if (_selectedHost != null)
			_selectedHost.Selected = false;
		_selectedHost = host;
		if (_selectedHost != null) {
			_selectedHost.Selected = true;
			_selectedIndex = _items.IndexOf[_selectedHost.Item];
		} else {
			_selectedIndex = -1;
		}
		if (selectedHostChanged)
			if (SelectedIndexChanged != null) {
				SelectedIndexChanged(this, new EventArgs());
			}
		if (_view == Control.View.Preview)
			drawPreviewHosts();
	}
	/// <summary>
	/// Raise the ItemHover event.
	/// </summary>
	private void invokeItemHover(ListViewItem item)
	{
		if (ItemHover != null) {
			ItemHover(this, new ItemEventArgs(item));
		}
	}
	/// <summary>
	/// Gets a string represent the value of a ListViewSubItem.
	/// </summary>
	private string getValueString(object value, int index)
	{
		if (index < 0 | index >= _columns.Count)
			return "";
		string result = "";
		if (value == null)
			return "";
		ColumnHeader aColumn = _columns[index];
		if (aColumn != null) {
			switch (aColumn.Format) {
				case ColumnFormat.Check:
					result = "";
					break;
				case ColumnFormat.None:
					result = value.ToString();
					break;
				case ColumnFormat.Password:
					result = "12345";
					break;
				case ColumnFormat.Bar:
				case ColumnFormat.Currency:
				case ColumnFormat.Custom:
				case ColumnFormat.Exponential:
				case ColumnFormat.FixedPoint:
				case ColumnFormat.General:
				case ColumnFormat.HexaDecimal:
				case ColumnFormat.Number:
				case ColumnFormat.Percent:
				case ColumnFormat.RoundTrip:
					// Convert to double
					try {
						double dblValue = Convert.ToDouble(value);
						switch (aColumn.Format) {
							case ColumnFormat.Bar:
							case ColumnFormat.Custom:
								result = dblValue.ToString(aColumn.CustomFormat, _ci);
								break;
							case ColumnFormat.Currency:
								result = dblValue.ToString("C", _ci);
								break;
							case ColumnFormat.Exponential:
								result = dblValue.ToString("E", _ci);
								break;
							case ColumnFormat.FixedPoint:
								result = dblValue.ToString("F", _ci);
								break;
							case ColumnFormat.General:
								result = dblValue.ToString("G", _ci);
								break;
							case ColumnFormat.HexaDecimal:
								result = dblValue.ToString("X", _ci);
								break;
							case ColumnFormat.Number:
								result = dblValue.ToString("N", _ci);
								break;
							case ColumnFormat.Percent:
								result = dblValue.ToString("P", _ci);
								break;
							case ColumnFormat.RoundTrip:
								result = dblValue.ToString("R", _ci);
								break;
						}
					} catch (Exception ex) {
					}
					break;
				case ColumnFormat.DecimalNumber:
					// Convert to integer
					try {
						int intValue = Convert.ToInt32(value);
						result = intValue.ToString("D", _ci);
					} catch (Exception ex) {
					}
					break;
				default:
					// Convert to datetime
					try {
						System.DateTime dtValue = Convert.ToDateTime(value);
						switch (aColumn.Format) {
							case ColumnFormat.CustomDateTime:
								result = dtValue.ToString(aColumn.CustomFormat, _ci);
								break;
							case ColumnFormat.ShortDate:
								result = dtValue.ToString("d", _ci);
								break;
							case ColumnFormat.LongDate:
								result = dtValue.ToString("D", _ci);
								break;
							case ColumnFormat.FullDateShortTime:
								result = dtValue.ToString("f", _ci);
								break;
							case ColumnFormat.FullDateLongTime:
								result = dtValue.ToString("F", _ci);
								break;
							case ColumnFormat.GeneralDateShortTime:
								result = dtValue.ToString("g", _ci);
								break;
							case ColumnFormat.GeneralDateLongTime:
								result = dtValue.ToString("G", _ci);
								break;
							case ColumnFormat.RoundTripDateTime:
								result = dtValue.ToString("O", _ci);
								break;
							case ColumnFormat.RFC1123:
								result = dtValue.ToString("R", _ci);
								break;
							case ColumnFormat.SortableDateTime:
								result = dtValue.ToString("s", _ci);
								break;
							case ColumnFormat.ShortTime:
								result = dtValue.ToString("t", _ci);
								break;
							case ColumnFormat.LongTime:
								result = dtValue.ToString("T", _ci);
								break;
							case ColumnFormat.UniversalSortableDateTime:
								result = dtValue.ToString("u", _ci);
								break;
							case ColumnFormat.UniversalFullDateTime:
								result = dtValue.ToString("U", _ci);
								break;
						}
					} catch (Exception ex) {
					}
					break;
			}
		}
		return result;
	}
	/// <summary>
	/// Show a TextBox for label editing on a specified ListViewItemHost.
	/// </summary>
	private void showTextBoxEditor(ListViewItemHost aHost)
	{
		if (aHost == null)
			return;
		if (_columns.Count == 0)
			return;
		if (!_columns[0].Visible)
			return;
		ItemEventArgs beforeLabelEdit = new ItemEventArgs(aHost.Item);
		if (BeforeLabelEdit != null) {
			BeforeLabelEdit(this, beforeLabelEdit);
		}
		if (beforeLabelEdit.Cancel)
			return;
		_currentEditedHost = aHost;
		if (_allowMultiline) {
			_txtEditor.Multiline = true;
			_txtEditor.ScrollBars = ScrollBars.Both;
		} else {
			_txtEditor.Multiline = false;
			_txtEditor.ScrollBars = ScrollBars.None;
		}
		Rectangle txtRect = aHost.getTextRectangle();
		_txtEditor.Location = txtRect.Location;
		_txtEditor.Size = txtRect.Size;
		_txtEditor.Text = aHost.Item.Text;
		_txtEditor.Visible = true;
		_txtEditor.SelectAll();
		_txtEditor.Focus();
	}
	/// <summary>
	/// Gets the bounding rectangle of a preview image.
	/// </summary>
	private Rectangle getRectangle(PointF cPoint, SizeF initSize, int degree, int _maxIncrementWidth, int _maxIncrementHeight)
	{
		SizeF resultSize = new SizeF(0, 0);
		Rectangle resultRect = new Rectangle(0, 0, 0, 0);
		resultSize.Width = initSize.Width + (Math.Sin(degree * Math.PI / 180) * _maxIncrementWidth * Math.Sin(75 * Math.PI / 180));
		resultSize.Height = initSize.Height + (Math.Sin(degree * Math.PI / 180) * _maxIncrementHeight * Math.Sin(75 * Math.PI / 180));
		resultRect.X = cPoint.X - (resultSize.Width / 2);
		resultRect.Y = cPoint.Y - resultSize.Height;
		resultRect.Width = resultSize.Width;
		resultRect.Height = resultSize.Height;
		return resultRect;
	}
	/// <summary>
	/// Drawing the preview images on specified Graphics object.
	/// </summary>
	private void drawPreviewHosts()
	{
		if (_slideBgImage != null)
			_slideBgImage.Dispose();
		if (_itemHosts.Count == 0)
			return;
		_slideBgImage = new Bitmap(this.Width, this.Height - _columnControl.Height);
		Graphics g = Graphics.FromImage(_slideBgImage);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		g.Clear(Color.Black);
		ListViewItemHost frontHost = null;
		ListViewItemHost right1 = null;
		ListViewItemHost right2 = null;
		ListViewItemHost left1 = null;
		ListViewItemHost left2 = null;
		// Selecting five drawn hosts, if possible.
		if (_selectedHost != null) {
			frontHost = _selectedHost;
		} else {
			frontHost = getFirstVisibleHost();
		}
		if (frontHost == null)
			return;
		right1 = getNextVisibleHost(frontHost);
		if (right1 != null)
			right2 = getNextVisibleHost(right1);
		left1 = getPrevVisibleHost(frontHost);
		if (left1 != null)
			left2 = getPrevVisibleHost(left1);
		// Drawing preview
		// Check parameter
		dynamic maxIncWidth = this.Width / 2;
		dynamic maxIncHeight = (this.Height - _columnControl.Height) * 3 / 4;
		// Positioning previews
		PointF centerPoint = new PointF(this.Width / 2, this.Height * 2 / 3);
		SizeF initSize = new Size(80, 60);
		Rectangle rect = default(Rectangle);
		dynamic degree = 30;
		while (degree <= 90) {
			if (degree < 90) {
				PointF outerPoint1 = new PointF(0, 0);
				PointF outerPoint2 = new PointF(0, 0);
				ListViewItemHost rightHost = null;
				ListViewItemHost leftHost = null;
				if (degree == 30) {
					rightHost = right2;
					leftHost = left2;
				} else {
					rightHost = right1;
					leftHost = left1;
				}
				// Right item
				if (rightHost != null) {
					outerPoint1.X = centerPoint.X + (Math.Cos(degree * Math.PI / 180) * (this.Width / 3));
					outerPoint1.Y = centerPoint.Y + (Math.Sin(degree * Math.PI / 180) * (this.Height / 3) * Math.Cos(75 * Math.PI / 180));
					rect = getRectangle(outerPoint1, initSize, degree, maxIncWidth, maxIncHeight);
					rightHost.drawPreviewImage(g, rect, degree);
				}
				// Left item
				if (leftHost != null) {
					outerPoint2.X = centerPoint.X + (Math.Cos((180 - degree) * Math.PI / 180) * (this.Width / 3));
					outerPoint2.Y = centerPoint.Y + (Math.Sin((180 - degree) * Math.PI / 180) * (this.Height / 3) * Math.Cos(75 * Math.PI / 180));
					rect = getRectangle(outerPoint2, initSize, degree, maxIncWidth, maxIncHeight);
					leftHost.drawPreviewImage(g, rect, degree);
				}
			} else {
				PointF outerPoint = new PointF(0, 0);
				outerPoint.X = centerPoint.X + (Math.Cos(degree * Math.PI / 180) * (this.Width / 3));
				outerPoint.Y = centerPoint.Y + (Math.Sin(degree * Math.PI / 180) * (this.Height / 3) * Math.Cos(75 * Math.PI / 180));
				rect = getRectangle(outerPoint, initSize, degree, maxIncWidth, maxIncHeight);
				frontHost.drawPreviewImage(g, rect, degree, true);
			}
			degree += 30;
		}
		g.Dispose();
	}
	#endregion
	#region "Friend Routines."
	// ListViewGroup related.
	internal void _groupTextAlignChanged(ListViewGroup @group)
	{
		if (_showGroups)
			this.Invalidate();
	}
	internal void _groupTextChanged(ListViewGroup @group)
	{
		if (!_showGroups)
			return;
		if (_columnRef > -1) {
			_groupHosts.Sort(groupHostComparer);
			if (_view == View.Preview) {
				if (_slideLocation == SlideLocation.Bottom | _slideLocation == SlideLocation.Top) {
					int x = _slider.Value;
					foreach (ListViewGroupHost gh in _groupHosts) {
						gh.X = x;
						if (gh.IsVisible)
							x = gh.Right + 5;
					}
				} else {
					int y = _slider.Value;
					foreach (ListViewGroupHost gh in _groupHosts) {
						gh.Y = y;
						if (gh.IsVisible)
							y = gh.Bottom + 5;
					}
				}
			} else {
				int y = 0;
				if (_vScroll.Visible) {
					y = -_vScroll.Value;
				} else {
					y = _clientArea.Y;
				}
				foreach (ListViewGroupHost gh in _groupHosts) {
					gh.Y = y;
					if (gh.IsVisible)
						y = gh.Bottom + 5;
				}
			}
		}
		this.Invalidate();
	}
	[Description("Routines to change the appearance of a ListViewGroup when collpased or expanded.")]
	internal void _groupCollapseChanged(ListViewGroup @group)
	{
		if (!_showGroups)
			return;
		measureAll();
		relocateAll();
		if (!_internalThread)
			this.Invalidate();
	}
	[Description("Routines to change the check state of a ListViewGroup when Checked property of ListViewGroup has been changed.")]
	internal void _groupCheckedChanged(ListViewGroup @group)
	{
		if (_internalThread)
			return;
		_internalThread = true;
		int hostIndex = -1;
		foreach (ListViewGroupHost gh in _groupHosts) {
			if (object.ReferenceEquals(gh.Group, @group)) {
				hostIndex = _groupHosts.IndexOf(gh);
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (hostIndex == -1)
			return;
		foreach (ListViewItemHost lviHost in _groupHosts[hostIndex].ItemHosts) {
			if (lviHost.Item.Checked != @group.Checked) {
				lviHost.Item.setChecked(@group.Checked);
				if (@group.Checked) {
					@group.CheckedItems.Add(lviHost.Item);
					_checkedItems.Add(lviHost.Item);
				}
			}
		}
		if (!@group.Checked)
			@group.CheckedItems.Clear();
		@group._checkState = (@group.Checked ? CheckState.Checked : CheckState.Unchecked);
		this.Invalidate();
		changeColumnCheckState();
		_internalThread = false;
	}
	// ListViewItem related.
	internal void invokeItemBeforeCheck(ItemEventArgs e)
	{
		if (ItemBeforeCheck != null) {
			ItemBeforeCheck(this, e);
		}
	}
	internal void _itemTextChanged(ListViewItem item)
	{
		if (_columns.Count == 0)
			return;
		if (_columns[0].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		// Rebuilding filter
		ListViewItemHost itemHost = null;
		List<object> objs = new List<object>();
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item))
				itemHost = lviHost;
			if (!objs.Contains(lviHost.Item.Text))
				objs.Add(lviHost.Item.Text);
		}
		if (itemHost == null)
			return;
		_columnControl.reloadFilter(0, objs);
		itemHost.Visible = filterItem(item);
		sortAll();
		if (_view == Control.View.Details | _view == Control.View.List)
			measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _itemFontChanged(ListViewItem item)
	{
		if (_columns.Count == 0)
			return;
		if (_columns[0].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		ListViewItemHost itemHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item))
				itemHost = lviHost;
		}
		if (itemHost == null)
			return;
		if (_view == Control.View.Details | _view == Control.View.List)
			measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _itemBackColorChanged(ListViewItem item)
	{
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				if (lviHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _itemColorChanged(ListViewItem item)
	{
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				if (lviHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _itemSmallImageChanged(ListViewItem item)
	{
		if (_view == Control.View.Details | _view == Control.View.List) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item)) {
					if (lviHost.IsVisible)
						this.Invalidate();
					return;
				}
			}
		}
	}
	internal void _itemTileImageChanged(ListViewItem item)
	{
		if (_view == Control.View.Tile | _view == Control.View.Icon) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item)) {
					if (lviHost.IsVisible)
						this.Invalidate();
					return;
				}
			}
		}
	}
	internal void _itemThumbnailImageChanged(ListViewItem item)
	{
		if (_view == Control.View.Thumbnail) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item)) {
					if (lviHost.IsVisible)
						this.Invalidate();
					return;
				}
			}
		}
	}
	internal void _itemPreviewImageChanged(ListViewItem item)
	{
		if (_view == Control.View.Preview) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item)) {
					if (lviHost.IsVisible)
						this.Invalidate();
					return;
				}
			}
		}
	}
	[Description("Fired when checked property of ListViewItem has been changed.")]
	internal void _itemCheckedChanged(ListViewItem item)
	{
		if (_internalThread) {
			if (ItemAfterCheck != null) {
				ItemAfterCheck(this, new ItemEventArgs(item));
			}
			return;
		}
		if (item.Group != null) {
			if (item.Checked) {
				item.Group.CheckedItems.Add(item);
				_checkedItems.Add(item);
			} else {
				item.Group.CheckedItems.Remove(item);
			}
		}
		ListViewItemHost itemHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				itemHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (itemHost == null)
			return;
		if (itemHost.GroupHost != null)
			itemHost.GroupHost.checkCheckedState();
		changeColumnCheckState();
		if (itemHost.IsVisible)
			this.Invalidate();
		if (ItemAfterCheck != null) {
			ItemAfterCheck(this, new ItemEventArgs(item));
		}
	}
	internal void _itemUseItemStyleForSubItemsChanged(ListViewItem item)
	{
		_itemFontChanged(item);
	}
	internal void _itemGroupChanged(ListViewItem item)
	{
		if (_internalThread)
			return;
		ListViewItemHost itemHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				itemHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (itemHost == null)
			return;
		ListViewGroupHost groupHost = null;
		if (item.Group == null) {
			groupHost = _defaultGroupHost;
		} else {
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				if (object.ReferenceEquals(lvgHost.Group, item.Group)) {
					groupHost = lvgHost;
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		_defaultGroupHost.ItemHosts.Remove(itemHost);
		itemHost.GroupHost = groupHost;
		groupHost.ItemHosts.Add(itemHost);
		sortAll();
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _itemSubItemsChanged(ListViewItem item)
	{
		ListViewItemHost itemHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				itemHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (itemHost == null)
			return;
		itemHost.refreshSubItem();
		sortAll();
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
	}
	// ListViewSubItem related.
	internal void _subitemValueChanged(System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
	{
		int subItemIndex = subitem.ListViewItem.SubItems.IndexOf(subitem) + 1;
		if (_columns.Count == 0 | subItemIndex >= _columns.Count)
			return;
		if (_columns[subItemIndex].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		// Rebuilding filter
		ListViewItemHost itemHost = null;
		List<object> objs = new List<object>();
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, subitem.ListViewItem))
				itemHost = lviHost;
			if (lviHost.Item.SubItems[subItemIndex] != null) {
				if (!objs.Contains(lviHost.Item.SubItems[subItemIndex].Value))
					objs.Add(lviHost.Item.SubItems[subItemIndex].Value);
			}
		}
		if (itemHost == null)
			return;
		_columnControl.reloadFilter(subItemIndex, objs);
		itemHost.Visible = filterItem(subitem.ListViewItem);
		if (_view == Control.View.Details)
			measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _subitemFontChanged(System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
	{
		if (subitem.ListViewItem.UseItemStyleForSubItems)
			return;
		bool reloadAll = false;
		int subItemIndex = subitem.ListViewItem.SubItems.IndexOf(subitem) + 1;
		if (_columns.Count == 0 | subItemIndex >= _columns.Count)
			return;
		if (_columns[subItemIndex].SizeType == ColumnSizeType.Auto) {
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
			reloadAll = true;
		}
		ListViewItemHost itemHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, subitem.ListViewItem))
				itemHost = lviHost;
		}
		if (itemHost == null)
			return;
		if (!reloadAll) {
			itemHost.measureSize();
		} else {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				lviHost.measureSize();
			}
		}
		if (_view == Control.View.Details) {
			if (itemHost.GroupHost != null & _showGroups) {
				if (!object.ReferenceEquals(itemHost.GroupHost, _defaultGroupHost)) {
					int i = _groupHosts.IndexOf(itemHost.GroupHost) + 1;
					int y = (itemHost.GroupHost.IsVisible ? itemHost.GroupHost.Bottom + 5 : itemHost.GroupHost.Y);
					while (i < _groupHosts.Count) {
						_groupHosts[i].Y = y;
						if (_groupHosts[i].IsVisible)
							y = _groupHosts[i].Bottom + 5;
						i = i + 1;
					}
					_defaultGroupHost.Y = y;
				}
			} else {
				int y = (_vScroll.Visible ? _clientArea.Y - _vScroll.Value : _clientArea.Y);
				foreach (ListViewItemHost ih in _itemHosts) {
					if (ih.Visible) {
						ih.Y = y;
						y = ih.Bottom + 1;
					}
				}
			}
		}
		this.Invalidate();
	}
	internal void _subitemBackColorChanged(System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
	{
		if (subitem.ListViewItem.UseItemStyleForSubItems)
			return;
		_itemBackColorChanged(subitem.ListViewItem);
	}
	internal void _subitemColorChanged(System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
	{
		if (subitem.ListViewItem.UseItemStyleForSubItems)
			return;
		_itemColorChanged(subitem.ListViewItem);
	}
	internal void _subitemPrintValueOnBarChanged(System.Windows.Forms.ListViewItem.ListViewSubItem subitem)
	{
		_itemColorChanged(subitem.ListViewItem);
	}
	// Friend property, to provide design mode to ListViewItem class
	internal bool IsDesignMode {
		get { return DesignMode; }
	}
	#endregion
	#region "Public Routines."
	/// <summary>
	/// Collapse the default group of the ListView.
	/// </summary>
	public void CollapseDefaultGroup()
	{
		_defaultGroup.collapse();
	}
	/// <summary>
	/// Expand the default group of the ListView.
	/// </summary>
	public void ExpandDefaultGroup()
	{
		_defaultGroup.expand();
	}
	/// <summary>
	/// Ensures that the specified item is visible within the control, scrolling the contents of the control if necessary.
	/// </summary>
	public void ensureVisible(int index)
	{
		if (index < 0 | index >= _items.Count)
			return;
		ListViewItem anItem = _items[index];
		ensureVisible(anItem);
	}
	/// <summary>
	/// Ensures that the specified item is visible within the control, scrolling the contents of the control if necessary.
	/// </summary>
	public void ensureVisible(ListViewItem item)
	{
		if (item == null)
			return;
		ListViewItemHost aHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				aHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (aHost == null)
			return;
		if (!aHost.Visible)
			return;
		if (_showGroups)
			aHost.GroupHost.Group.expand();
		int dx = 0;
		int dy = 0;
		if (aHost.X < _clientArea.X | aHost.Right > _clientArea.Right) {
			if (aHost.X < _clientArea.X) {
				dx = aHost.X - _clientArea.X;
			} else {
				dx = aHost.Right - _clientArea.Right;
			}
		}
		if (aHost.Y < _clientArea.Y | aHost.Bottom > _clientArea.Bottom) {
			if (aHost.Y < _clientArea.Y) {
				dy = aHost.Y - _clientArea.Y;
			} else {
				dy = aHost.Bottom - _clientArea.Bottom;
			}
		}
		if (_vScroll.Visible | _hScroll.Visible) {
			_vScroll.Value += dy;
			_hScroll.Value += dx;
		}
		if (_slider.Visible) {
			if (_slider.Orientation == Orientation.Horizontal) {
				_slider.Value += dx;
			} else {
				_slider.Value += dy;
			}
		}
	}
	/// <summary>
	/// Finds the first ListViewItem that begins with the specified text value.
	/// </summary>
	public ListViewItem findItemWithText(string text)
	{
		foreach (ListViewItem lvi in _items) {
			if (lvi.Text.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				return lvi;
		}
		return null;
	}
	/// <summary>
	/// Finds the first ListViewItem that begins with the specified text value or have subitem with specified text.
	/// </summary>
	public ListViewItem findItemWithText(string text, bool includeSubItem)
	{
		foreach (ListViewItem lvi in _items) {
			if (lvi.Text.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				return lvi;
			int i = 1;
			while (i <= lvi.SubItems.Count) {
				string strValue = getValueString(lvi.SubItems[i].Value, i);
				if (strValue.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					return lvi;
				i += 1;
			}
		}
		return null;
	}
	/// <summary>
	/// Finds the first ListViewItem that begins with the specified text value or have subitem with specified text, starting from the specified index.
	/// </summary>
	public ListViewItem findItemWithText(string text, bool includeSubItem, int start)
	{
		if (start < 0 | start >= _items.Count)
			return null;
		dynamic index = start;
		ListViewItem lvi = null;
		while (index < _items.Count) {
			lvi = _items[index];
			if (lvi.Text.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				return lvi;
			int i = 1;
			while (i <= lvi.SubItems.Count) {
				string strValue = getValueString(lvi.SubItems[i].Value, i);
				if (strValue.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					return lvi;
				i += 1;
			}
			index += 1;
		}
		return null;
	}
	/// <summary>
	/// Finds the first ListViewItem that begins with the specified text value or have subitem with specified text, starting from the specified index, and ignoring or honoring their case.
	/// </summary>
	public ListViewItem findItemWithText(string text, bool includeSubItem, int start, bool ignoreCase)
	{
		if (start < 0 | start >= _items.Count)
			return null;
		dynamic index = start;
		ListViewItem lvi = null;
		StringComparison strComparison = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		while (index < _items.Count) {
			lvi = _items[index];
			if (lvi.Text.StartsWith(text, strComparison))
				return lvi;
			int i = 1;
			while (i <= lvi.SubItems.Count) {
				string strValue = getValueString(lvi.SubItems[i].Value, i);
				if (strValue.StartsWith(text, strComparison))
					return lvi;
				i += 1;
			}
			index += 1;
		}
		return null;
	}
	/// <summary>
	/// Retrieves the bounding rectangle for an item within the control.
	/// </summary>
	public Rectangle getItemRect(int index)
	{
		if (index >= 0 & index < _items.Count) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, _items[index]))
					return lviHost.Bounds;
			}
		}
		return new Rectangle(0, 0, 0, 0);
	}
	/// <summary>
	/// Retrieves the bounding rectangle for an item within the control.
	/// </summary>
	public Rectangle getItemRect(ListViewItem item)
	{
		if (item != null) {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, item))
					return lviHost.Bounds;
			}
		}
		return new Rectangle(0, 0, 0, 0);
	}
	/// <summary>
	/// Force the ListView to show a TextBox for label editing on a ListViewItem specified by its index.
	/// </summary>
	public void invokeLabelEdit(int index)
	{
		if (index < 0 | index >= _items.Count)
			return;
		invokeLabelEdit(_items[index]);
	}
	/// <summary>
	/// Force the ListView to show a TextBox for label editing on a specified ListViewItem.
	/// </summary>
	public void invokeLabelEdit(ListViewItem item)
	{
		if (item == null)
			return;
		ListViewItemHost invokedHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, item)) {
				invokedHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (invokedHost == null)
			return;
		if (!invokedHost.Visible)
			return;
		ensureVisible(item);
		showTextBoxEditor(invokedHost);
	}
	/// <summary>
	/// Filter items based on the ListViewSubItem on the specified index using filter values.
	/// </summary>
	/// <remarks>However, filter operation is combined with another filter column parameters, and the EnableFiltering property of the associated ColumnHeader must be set to true.</remarks>
	public void filterItem(int index, object[] values)
	{
		if (index < 0 | index >= _columns.Count)
			return;
		if (values.Length == 0)
			return;
		ColumnFilterHandle filterHandle = _columnControl.FilterHandler[_columns[index]];
		if (filterHandle == null)
			return;
		ColumnFilterItem cfi = default(ColumnFilterItem);
		foreach ( cfi in filterHandle.Items) {
			cfi.Selected = false;
		}
		foreach (object objValue in values) {
			cfi = filterHandle.getFilterItem(objValue);
			if (cfi != null)
				cfi.Selected = true;
		}
		filterHandle.FilterMode = FilterMode.ByValue;
		List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			lviHost.Visible = filterItem(lviHost.Item, handlers);
		}
		measureAll();
		relocateAll();
		this.Invalidate();
		if (ColumnFilterChanged != null) {
			ColumnFilterChanged(this, new ColumnEventArgs(_columns[index]));
		}
	}
	/// <summary>
	/// Filter items based on the ListViewSubItem on the specified index using maximum and minimum value allowed, and the filter mode, for numeric and date time data types.
	/// </summary>
	/// <remarks>However, filter operation is combined with another filter column parameters, and the EnableFiltering property of the associated ColumnHeader must be set to true.</remarks>
	public void filterItem(int index, object maxValue, object minValue, FilterRangeMode mode)
	{
		if (index < 0 | index >= _columns.Count)
			return;
		if (maxValue <= minValue)
			return;
		if (mode != FilterRangeMode.Between | mode != FilterRangeMode.Outside)
			return;
		ColumnFilterHandle filterHandle = _columnControl.FilterHandler[_columns[index]];
		if (filterHandle == null)
			return;
		filterHandle.MaxValueSelected = maxValue;
		filterHandle.MinValueSelected = minValue;
		filterHandle.FilterMode = FilterMode.ByRange;
		filterHandle.RangeMode = mode;
		List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			lviHost.Visible = filterItem(lviHost.Item, handlers);
		}
		measureAll();
		relocateAll();
		this.Invalidate();
		if (ColumnFilterChanged != null) {
			ColumnFilterChanged(this, new ColumnEventArgs(_columns[index]));
		}
	}
	/// <summary>
	/// Filter items based on the ListViewSubItem on the specified index using words parameter, and the filter mode, for string data type.
	/// </summary>
	/// <remarks>However, filter operation is combined with another filter column parameters, and the EnableFiltering property of the associated ColumnHeader must be set to true.</remarks>
	public void filterItem(int index, string words, FilterRangeMode mode)
	{
		if (index < 0 | index >= _columns.Count)
			return;
		if (mode == FilterRangeMode.Between | mode == FilterRangeMode.Outside | mode == FilterRangeMode.Equal)
			return;
		ColumnFilterHandle filterHandle = _columnControl.FilterHandler[_columns[index]];
		if (filterHandle == null)
			return;
		filterHandle.MinValueSelected = words;
		filterHandle.FilterMode = FilterMode.ByRange;
		filterHandle.RangeMode = mode;
		List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			lviHost.Visible = filterItem(lviHost.Item, handlers);
		}
		measureAll();
		relocateAll();
		this.Invalidate();
		if (ColumnFilterChanged != null) {
			ColumnFilterChanged(this, new ColumnEventArgs(_columns[index]));
		}
	}
	#endregion
	#region "Constructor."
	public ListView()
	{
		Resize += ListView_Resize;
		Paint += ListView_Paint;
		MouseWheel += ListView_MouseWheel;
		MouseUp += ListView_MouseUp;
		MouseMove += ListView_MouseMove;
		MouseLeave += ListView_MouseLeave;
		MouseEnter += ListView_MouseEnter;
		MouseDown += ListView_MouseDown;
		LostFocus += ListView_LostFocus;
		KeyPress += ListView_KeyPress;
		KeyDown += ListView_KeyDown;
		GotFocus += ListView_GotFocus;
		FontChanged += ListView_FontChanged;
		EnabledChanged += ListView_EnabledChanged;
		Disposed += ListView_Disposed;
		// Initialize control styles.
		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.SetStyle(ControlStyles.ResizeRedraw, true);
		this.SetStyle(ControlStyles.Selectable, true);
		base.BackColor = Color.White;
		base.Padding = new Padding(1);
		this.SuspendLayout();
		// Setting color blend for column separator
		Color[] _linePenColors = new Color[3];
		float[] _linePenPos = new float[3];
		_linePenColors[0] = Color.FromArgb(0, 158, 187, 221);
		_linePenColors[1] = Color.FromArgb(158, 187, 221);
		_linePenColors[2] = Color.FromArgb(0, 158, 187, 221);
		_linePenPos[0] = 0f;
		_linePenPos[1] = 0.5f;
		_linePenPos[2] = 1f;
		_linePenBlend = new ColorBlend();
		_linePenBlend.Colors = _linePenColors;
		_linePenBlend.Positions = _linePenPos;
		// Initialize all objects
		if (this.Font.FontFamily.IsStyleAvailable(FontStyle.Bold)) {
			_groupFont = new Font(this.Font.FontFamily.Name, this.Font.Size, FontStyle.Bold);
		} else {
			_groupFont = this.Font;
		}
		_columns = new ColumnHeaderCollection(this);
		_columnControl = new ColumnHeaderControl(this, Renderer.ToolTip.TextFont);
		_groups = new ListViewGroupCollection(this);
		_items = new ListViewItemCollection(this);
		_checkedItems = new CheckedListViewItemCollection(this);
		_frozenItems = new FrozenListViewItemCollection(this);
		_defaultGroup = new ListViewGroup(this);
		_slider = new ItemSlider(this);
		_txtEditor = new TextBoxLabelEditor();
		_txtEditor.Visible = false;
		// Matrixes
		_normalMatrix = new Matrix(1, 0, 0, 1, 0, 0);
		_mirrorMatrix = new Matrix(1, 0, 0, -1, 0, 0);
		// Setting up graphics object for text measurement.
		_gObj.SmoothingMode = SmoothingMode.AntiAlias;
		_gObj.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		// Setting up default group and it host
		_defaultGroup.Text = "Default";
		_defaultGroupHost = new ListViewGroupHost(this, _defaultGroup);
		// Setting up tooltip
		_tooltip = new ToolTip(this);
		_tooltip.AnimationSpeed = 20;
		// Adding ColumnControl, TextBoxLabelEditor, and ScrollBars
		this.Controls.Add(_txtEditor);
		this.Controls.Add(_columnControl);
		_vScroll.Top = _columnControl.Bottom + 1;
		_vScroll.Left = _clientArea.Right - _vScroll.Width;
		_vScroll.Visible = false;
		_hScroll.Left = 1;
		_hScroll.Top = _clientArea.Bottom - _hScroll.Height;
		_hScroll.Visible = false;
		this.Controls.Add(_vScroll);
		this.Controls.Add(_hScroll);
		base.Size = new Size(100, 100);
		// Initialize client area
		measureAll();
		this.ResumeLayout();
	}
	protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
	{
		if (_view == Control.View.Details) {
			switch (keyData) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Space:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
				case Keys.F2:
					return true;
				case Keys.Left:
				case Keys.Right:
					return _hScroll.Visible;
				default:
					return base.IsInputKey(keyData);
			}
		} else if (_view == Control.View.Preview) {
			switch (keyData) {
				case Keys.Space:
				case Keys.Home:
				case Keys.End:
				case Keys.F2:
					return true;
				case Keys.Left:
				case Keys.Right:
					return _slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top;
				case Keys.Up:
				case Keys.Down:
					return _slideLocation == Control.SlideLocation.Left | _slideLocation == Control.SlideLocation.Right;
				default:
					return base.IsInputKey(keyData);
			}
		} else {
			switch (keyData) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Space:
				case Keys.Left:
				case Keys.Right:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
				case Keys.F2:
					return true;
				default:
					return base.IsInputKey(keyData);
			}
		}
	}
	#endregion
	#region "Public Properties."
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public new Padding Padding {
		get { return base.Padding; }
		set { base.Padding = value; }
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public new string Text {
		get { return base.Text; }
		set { base.Text = value; }
	}
	[DefaultValue(typeof(Drawing.Color), "White")]
	public new Drawing.Color BackColor {
		get { return base.BackColor; }
		set {
			if (base.BackColor != value) {
				base.BackColor = value;
				if (_view != Control.View.Preview)
					this.Invalidate();
			}
		}
	}
	// Behavior
	[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets the collection of ColumnHeader that are assigned to the ListView control")]
	public ColumnHeaderCollection Columns {
		get { return _columns; }
	}
	[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets the collection of ListViewGroup that are assigned to the ListView control")]
	public ListViewGroupCollection Groups {
		get { return _groups; }
	}
	[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets the collection of ListViewItem that are assigned to the ListView control")]
	public ListViewItemCollection Items {
		get { return _items; }
	}
	[Category("Behavior"), Browsable(false), Description("Gets the collection of checked ListViewItem that are assigned to the ListView control")]
	public CheckedListViewItemCollection CheckedItems {
		get { return _checkedItems; }
	}
	[Category("Behavior"), Browsable(false), Description("Gets the collection of frozen ListViewItem that are assigned to the ListView control")]
	public FrozenListViewItemCollection FrozenItems {
		get { return _frozenItems; }
	}
	[Category("Behavior"), DefaultValue(false), Description("Determine whether the user can edit the labels of items in the control.")]
	public bool LabelEdit {
		get { return _labelEdit; }
		set { _labelEdit = value; }
	}
	[Category("Behavior"), DefaultValue(true), Description("Determine whether items are displayed in groups.")]
	public bool ShowGroups {
		get { return _showGroups; }
		set {
			if (_showGroups != value) {
				_showGroups = value;
				measureAll();
				relocateAll();
				this.Invalidate();
			}
		}
	}
	[Category("Behavior"), Description("Determine culture info used to display values.")]
	public CultureInfo Culture {
		get { return _ci; }
		set {
			if (!object.ReferenceEquals(_ci, value)) {
				if (value == null) {
					_ci = Renderer.Drawing.en_us_ci;
				} else {
					_ci = value;
				}
				_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
				measureAll();
				relocateAll();
				this.Invalidate(true);
			}
		}
	}
	[Category("Behavior"), DefaultValue(true), Description("")]
	public bool ItemToolTip {
		get { return _itemTooltip; }
		set { _itemTooltip = value; }
	}
	// Appearance
	[Category("Appearance"), DefaultValue(false), Description("Determine a check box appears next to each item in the control.")]
	public bool CheckBoxes {
		get { return _checkBoxes; }
		set {
			if (_checkBoxes != value) {
				_checkBoxes = value;
				_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
				measureAll();
				relocateAll();
				this.Invalidate(true);
			}
		}
	}
	[Category("Appearance"), DefaultValue(false), Description("Determine a number appears next to each item in the control.")]
	public bool RowNumbers {
		get { return _rowNumbers; }
		set {
			if (_rowNumbers != value) {
				_rowNumbers = value;
				if (_view == Control.View.Details) {
					_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
					measureAll();
					relocateAll();
					this.Invalidate(true);
				}
			}
		}
	}
	[Category("Appearance"), DefaultValue(true), Description("Determine a dropdown options is shown in the column header.")]
	public bool ShowColumnOptions {
		get { return _showColumnOptions; }
		set {
			if (_showColumnOptions != value) {
				_showColumnOptions = value;
				_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
			}
		}
	}
	[Category("Appearance"), DefaultValue(false), Description("Determine whether clicking an item selects all its subitems.")]
	public bool FullRowSelect {
		get { return _fullRowSelect; }
		set {
			if (_fullRowSelect != value) {
				_fullRowSelect = value;
				this.Invalidate();
			}
		}
	}
	[Category("Appearance"), DefaultValue(typeof(Control.View), "Details"), Description("Determine how items are displayed in the control.")]
	public Control.View View {
		get { return _view; }
		set {
			if (value < 0 | value > Control.View.Preview)
				return;
			if (_view != value) {
				_view = value;
				_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
				measureAll();
				relocateAll();
				if (_view == Control.View.Preview)
					drawPreviewHosts();
				this.Invalidate(true);
				if (ViewChanged != null) {
					ViewChanged(this, new EventArgs());
				}
			}
		}
	}
	[Category("Appearance"), DefaultValue(typeof(Control.SlideLocation), "Bottom"), Description("Determine how items are placed in the control on Preview views.")]
	public Control.SlideLocation SlideLocation {
		get { return _slideLocation; }
		set {
			if (value < Control.SlideLocation.Top | value > Control.SlideLocation.Bottom)
				return;
			if (_slideLocation != value) {
				_slideLocation = value;
				measureAll();
				relocateAll();
				this.Invalidate();
			}
		}
	}
	[Category("Appearance"), DefaultValue(false), Description("Determine whether multiline item's text and subitem's value is allowed.")]
	public bool AllowMultiline {
		get { return _allowMultiline; }
		set {
			if (_allowMultiline != value) {
				_allowMultiline = value;
				measureAll();
				relocateAll();
				this.Invalidate();
			}
		}
	}
	[Category("Appearance"), DefaultValue(32), Description("Determine the size of the images shown in Icon view.")]
	public int IconSize {
		get { return _iconSize; }
		set {
			if (_iconSize != value) {
				if (value >= _minIconSize & value <= _maxIconSize) {
					_iconSize = value;
					if (_view == Control.View.Icon) {
						measureAll();
						relocateAll();
						this.Invalidate();
					}
				}
			}
		}
	}
	[Category("Appearance"), DefaultValue(128), Description("Determine the size of the images shown in Thumbnail view.")]
	public int ThumbnailSize {
		get { return _thumbnailSize; }
		set {
			if (_thumbnailSize != value) {
				if (value >= _minThumbnailSize & value <= _maxThumbnailSize) {
					_thumbnailSize = value;
					if (_view == Control.View.Thumbnail) {
						measureAll();
						relocateAll();
					}
				}
			}
		}
	}
	[DefaultValue(typeof(Drawing.Color), "Black"), Category("Appearance"), Description("")]
	public Drawing.Color PreviewBackColor {
		get { return _previewBackColor; }
		set {
			if (_previewBackColor != value) {
				_previewBackColor = value;
				if (_view == Control.View.Preview)
					this.Invalidate();
			}
		}
	}
	[DefaultValue(typeof(RowsDock), "Bottom"), Category("Appearance"), Description("Determine which item borders are docked to ListView control.")]
	public RowsDock FrozenRowsDock {
		get { return _frozenRowsDock; }
		set {
			if (value != RowsDock.Bottom & value != RowsDock.Top)
				return;
			if (_frozenRowsDock != value) {
				_frozenRowsDock = value;
				if (_view == Control.View.Details) {
					//measureAll()
					relocateAll();
					this.Invalidate();
				}
			}
		}
	}
	// Default Group
	[Category("Default Group"), DefaultValue("Default"), Description("Determine Text of the default group.")]
	public string DefaultGroupText {
		get { return _defaultGroup.Text; }
		set { _defaultGroup.Text = value; }
	}
	[Category("Default Group"), DefaultValue(typeof(HorizontalAlignment), "Left"), Description("Determine TextAlign of the default group.")]
	public HorizontalAlignment DefaultGroupTextAlign {
		get { return _defaultGroup.TextAlign; }
		set { _defaultGroup.TextAlign = value; }
	}
	[Category("Default Group"), DefaultValue(""), TypeConverter(typeof(StringConverter)), Description("Determine object that contains data about the default group.")]
	public object DefaultGroupTag {
		get { return _defaultGroup.Tag; }
		set { _defaultGroup.Tag = value; }
	}
	[Category("Default Group"), DefaultValue(false), Description("Determine a value indicating whether the default group is checked.")]
	public bool DefaultGroupChecked {
		get { return _defaultGroup.Checked; }
		set { _defaultGroup.Checked = value; }
	}
	[Category("Default Group"), DefaultValue(typeof(CheckState), "Unchecked"), Browsable(false), Description("")]
	public CheckState DefaultGroupCheckState {
		get { return _defaultGroup.CheckState; }
	}
	[Category("Default Group"), DefaultValue(false), Description(""), Browsable(false)]
	public bool DefaultGroupIsCollapsed {
		get { return _defaultGroup.IsCollapsed; }
	}
	[Category("Default Group"), Browsable(false), Description("")]
	public List<ListViewItem> DefaultGroupItems {
		get {
			List<ListViewItem> result = new List<ListViewItem>();
			foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
				result.Add(lviHost.Item);
			}
			return result;
		}
	}
	// Item Access
	[Browsable(false)]
	public ListViewItem SelectedItem {
		get {
			if (_selectedHost != null) {
				return _selectedHost.Item;
			} else {
				return null;
			}
		}
		set {
			if (value == null) {
				setSelectedHost(null);
				return;
			}
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (object.ReferenceEquals(lviHost.Item, value)) {
					setSelectedHost(lviHost);
					if (_selectedHost.Visible) {
						if (_showGroups & _selectedHost.GroupHost.Group.IsCollapsed)
							return;
						// Move the selected host into client area
						int dx = 0;
						int dy = 0;
						if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
							if (_selectedHost.X > _clientArea.X) {
								dx = _selectedHost.X - _clientArea.X;
							} else {
								dx = _selectedHost.Right - _clientArea.Right;
							}
						}
						if (_selectedHost.Y < _clientArea.Y | _selectedHost.Bottom > _clientArea.Bottom) {
							if (_selectedHost.Y < _clientArea.Y) {
								dy = _selectedHost.Y - _clientArea.Y;
							} else {
								dy = _selectedHost.Bounds.Bottom - _clientArea.Bottom;
							}
						}
						if (dx != 0 | dy != 0) {
							if (_vScroll.Visible)
								_vScroll.Value += dy;
							if (_hScroll.Visible)
								_hScroll.Value += dx;
							if (_slider.Visible) {
								if (_slider.Orientation == Orientation.Horizontal) {
									_slider.Value += dx;
								} else {
									_slider.Value += dy;
								}
							}
							return;
						}
						this.Invalidate();
					}
					return;
				}
			}
		}
	}
	[Browsable(false)]
	public int SelectedIndex {
		get { return _selectedIndex; }
		set {
			if (_selectedIndex != value) {
				if (value >= -1 | value < _items.Count - 1) {
					ListViewItemHost selHost = null;
					if (value > -1) {
						ListViewItem anItem = _items[value];
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (object.ReferenceEquals(lviHost.Item, anItem)) {
								selHost = lviHost;
								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
					setSelectedHost(selHost);
					int dx = 0;
					int dy = 0;
					if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
						if (_selectedHost.X > _clientArea.X) {
							dx = _selectedHost.X - _clientArea.X;
						} else {
							dx = _selectedHost.Right - _clientArea.Right;
						}
					}
					if (_selectedHost.Y < _clientArea.Y | _selectedHost.Bottom > _clientArea.Bottom) {
						if (_selectedHost.Y < _clientArea.Y) {
							dy = _selectedHost.Y - _clientArea.Y;
						} else {
							dy = _selectedHost.Bounds.Bottom - _clientArea.Bottom;
						}
					}
					if (dx != 0 | dy != 0) {
						if (_vScroll.Visible)
							_vScroll.Value += dy;
						if (_hScroll.Visible)
							_hScroll.Value += dx;
						if (_slider.Visible) {
							if (_slider.Orientation == Orientation.Horizontal) {
								_slider.Value += dx;
							} else {
								_slider.Value += dy;
							}
						}
						return;
					}
					this.Invalidate();
				}
			}
		}
	}
	#endregion
	#region "Event Handlers."
	private void _groups_AfterClear(object sender, CollectionEventArgs e)
	{
		_internalThread = true;
		_groupHosts.Clear();
		foreach (ListViewItemHost lviHost in _itemHosts) {
			lviHost.GroupHost = _defaultGroupHost;
			_defaultGroupHost.ItemHosts.Add(lviHost);
			lviHost.Item.Group = null;
		}
		sortAll();
		measureAll();
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate();
		_internalThread = false;
	}
	private void _groups_AfterInsert(object sender, CollectionEventArgs e)
	{
		ListViewGroup newGroup = (ListViewGroup)e.Item;
		ListViewGroupHost newHost = new ListViewGroupHost(this, newGroup);
		if (newGroup.Items.Count > 0) {
			_internalThread = true;
			foreach (ListViewItem lvi in newGroup.Items) {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					if (object.ReferenceEquals(lviHost.Item, lvi)) {
						lviHost.GroupHost = newHost;
						lvi.Group = newGroup;
						newHost.ItemHosts.Add(lviHost);
						break; // TODO: might not be correct. Was : Exit For
					}
				}
			}
		}
		_groupHosts.Add(newHost);
		sortAll();
		measureAll();
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate();
		_internalThread = false;
	}
	private void _groups_AfterRemove(object sender, CollectionEventArgs e)
	{
		ListViewGroup remGroup = (ListViewGroup)e.Item;
		ListViewGroupHost remHost = null;
		foreach (ListViewGroupHost lvgHost in _groupHosts) {
			if (object.ReferenceEquals(lvgHost.Group, remGroup)) {
				remHost = lvgHost;
			}
		}
		if (remHost == null)
			return;
		_internalThread = true;
		foreach (ListViewItemHost lviHost in remHost.ItemHosts) {
			lviHost.GroupHost = _defaultGroupHost;
			_defaultGroupHost.ItemHosts.Add(lviHost);
			lviHost.Item.Group = null;
		}
		_groupHosts.Remove(remHost);
		sortAll();
		measureAll();
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate();
		_internalThread = false;
	}
	private void _hScroll_ValueChanged(object sender, System.EventArgs e)
	{
		if (!_hScroll.Visible)
			return;
		if (_view == Control.View.Details) {
			_columnControl.moveColumns(-_hScroll.Value);
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.X = _clientArea.X - _hScroll.Value;
				}
				_defaultGroupHost.X = _clientArea.X - _hScroll.Value;
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					lviHost.X = _clientArea.X - _hScroll.Value;
				}
			}
			this.Invalidate(true);
		} else {
			if (_itemHosts.Count == 0)
				return;
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.X = _clientArea.X - _hScroll.Value;
				}
				_defaultGroupHost.X = _clientArea.X - _hScroll.Value;
			} else {
				int lastX = _itemHosts[0].X;
				int dx = (_clientArea.X - _hScroll.Value) - lastX;
				foreach (ListViewItemHost lviHost in _itemHosts) {
					lviHost.X += dx;
				}
			}
			this.Invalidate();
		}
	}
	private void _vScroll_ValueChanged(object sender, System.EventArgs e)
	{
		if (!_vScroll.Visible) {
			_lastVScrollValue = 0;
			return;
		}
		if (_itemHosts.Count == 0) {
			_lastVScrollValue = 0;
			return;
		}
		if (_showGroups) {
			int dy = _lastVScrollValue - _vScroll.Value;
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				lvgHost.Y += dy;
			}
			_defaultGroupHost.Y += dy;
			if (_txtEditor.Visible)
				_txtEditor.Top += dy;
		} else {
			int dy = _lastVScrollValue - _vScroll.Value;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (_view == Control.View.Details) {
					if (!lviHost.Frozen)
						lviHost.Y += dy;
				} else {
					lviHost.Y += dy;
				}
			}
			if (_txtEditor.Visible)
				_txtEditor.Top += dy;
		}
		_lastVScrollValue = _vScroll.Value;
		this.Invalidate();
	}
	private void _items_AfterClear(object sender, CollectionEventArgs e)
	{
		_checkedItems.Clear();
		foreach (ListViewGroupHost lvgHost in _groupHosts) {
			lvgHost.ItemHosts.Clear();
			lvgHost.Group.Items.Clear();
		}
		setSelectedHost(null);
		_defaultGroupHost.ItemHosts.Clear();
		_itemHosts.Clear();
		_columnControl.clearFilters();
		_vScroll.Visible = false;
		_clientArea = new Rectangle(1, _columnControl.Bottom + 1, this.Width - 2, this.Height - (_columnControl.Bottom + 1 + (_hScroll.Visible ? _hScroll.Height + 1 : 0)));
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate(true);
		if (ItemsClear != null) {
			ItemsClear(this, new EventArgs());
		}
	}
	private void _items_AfterInsert(object sender, CollectionEventArgs e)
	{
		ListViewItem newItem = (ListViewItem)e.Item;
		ListViewItemHost newHost = new ListViewItemHost(newItem, this);
		ListViewGroupHost groupHost = _defaultGroupHost;
		if (newItem.Group != null) {
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				if (object.ReferenceEquals(lvgHost.Group, newItem.Group)) {
					groupHost = lvgHost;
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		newHost.GroupHost = groupHost;
		groupHost.ItemHosts.Add(newHost);
		// Adding item text and all its subitems to filter parameters
		int i = 0;
		while (i <= newItem.SubItems.Count & i < _columns.Count) {
			_columnControl.addFilter(i, newItem.SubItems[i].Value);
			i += 1;
		}
		_itemHosts.Add(newHost);
		newHost.Visible = filterItem(newItem);
		sortAll();
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate(true);
		if (ItemAdded != null) {
			ItemAdded(this, new ItemEventArgs(newItem));
		}
	}
	private void _items_AfterRemove(object sender, CollectionEventArgs e)
	{
		ListViewItem remItem = (ListViewItem)e.Item;
		ListViewItemHost remHost = null;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, remItem)) {
				remHost = lviHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (remHost == null)
			return;
		if (object.ReferenceEquals(remHost, _selectedHost))
			setSelectedHost(null);
		ListViewGroupHost groupHost = remHost.GroupHost;
		groupHost.ItemHosts.Remove(remHost);
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate(true);
		if (ItemRemoved != null) {
			ItemRemoved(this, new ItemEventArgs(remItem));
		}
	}
	private void ListView_Disposed(object sender, System.EventArgs e)
	{
		if (_slideBgImage != null)
			_slideBgImage.Dispose();
		if (_gObj != null)
			_gObj.Dispose();
		if (_gBmp != null)
			_gBmp.Dispose();
	}
	private void ListView_EnabledChanged(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void ListView_FontChanged(object sender, System.EventArgs e)
	{
		if (this.Font.FontFamily.IsStyleAvailable(FontStyle.Bold)) {
			_groupFont = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);
		} else {
			_groupFont = this.Font;
		}
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void ListView_GotFocus(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void ListView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		switch (e.KeyCode) {
			case Keys.Up:
				if (_itemHosts.Count == 0)
					return;
				if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Top | _slideLocation == Control.SlideLocation.Bottom))
					return;
				if (_selectedHost == null) {
					// There is no selected item, select the first visible item.
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (lvgHost.IsVisible) {
								// However, all items in a collapsed group isn't visible.
								if (!lvgHost.Group.IsCollapsed) {
									foreach (ListViewItemHost lviHost in lvgHost.ItemHosts) {
										if (lviHost.Visible) {
											setSelectedHost(lviHost);
											break; // TODO: might not be correct. Was : Exit For
										}
									}
								}
							}
							if (_selectedHost != null)
								break; // TODO: might not be correct. Was : Exit For
						}
						if (_selectedHost == null) {
							if (!_defaultGroupHost.IsVisible)
								return;
							if (_defaultGroup.IsCollapsed)
								return;
							foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
								if (lviHost.Visible) {
									setSelectedHost(lviHost);
									break; // TODO: might not be correct. Was : Exit For
								}
							}
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible) {
								setSelectedHost(lviHost);
								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
					if (_selectedHost == null)
						return;
				} else {
					ListViewItemHost upperHost = null;
					if (_showGroups) {
						int groupIndex = _groupHosts.Count - 1;
						if (object.ReferenceEquals(_selectedHost.GroupHost, _defaultGroupHost)) {
							if (!_defaultGroup.IsCollapsed & _defaultGroupHost.IsVisible)
								upperHost = getUpperHost(_selectedHost, _defaultGroupHost.ItemHosts);
						}
						if (upperHost == null) {
							groupIndex = _groupHosts.IndexOf(_selectedHost.GroupHost);
							if (groupIndex == -1) {
								groupIndex = _groupHosts.Count - 1;
							} else {
								if (!_groupHosts[groupIndex].Group.IsCollapsed)
									upperHost = getUpperHost(_selectedHost, _groupHosts[groupIndex].ItemHosts);
								groupIndex -= 1;
							}
							while (upperHost == null & groupIndex >= 0) {
								if (_groupHosts[groupIndex].IsVisible) {
									if (!_groupHosts[groupIndex].Group.IsCollapsed)
										upperHost = getUpperHost(_selectedHost, _groupHosts[groupIndex].ItemHosts, true);
								}
								groupIndex -= 1;
							}
						}
					} else {
						upperHost = getUpperHost(_selectedHost, _itemHosts);
					}
					if (upperHost == null)
						return;
					setSelectedHost(upperHost);
				}
				if (_selectedHost.Y < _clientArea.Y | _selectedHost.Bounds.Bottom > _clientArea.Bottom) {
					int dy = 0;
					if (_selectedHost.Y < _clientArea.Y) {
						dy = _selectedHost.Y - _clientArea.Y;
					} else {
						dy = _selectedHost.Bounds.Bottom - _clientArea.Bottom;
					}
					_vScroll.Value += dy;
				} else {
					this.Invalidate();
				}
				break;
			case Keys.Down:
				if (_itemHosts.Count == 0)
					return;
				if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Top | _slideLocation == Control.SlideLocation.Bottom))
					return;
				if (_selectedHost == null) {
					// There is no selected item, select the first visible item.
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (lvgHost.IsVisible) {
								// However, all items in a collapsed group isn't visible.
								if (!lvgHost.Group.IsCollapsed) {
									foreach (ListViewItemHost lviHost in lvgHost.ItemHosts) {
										if (lviHost.Visible) {
											setSelectedHost(lviHost);
											break; // TODO: might not be correct. Was : Exit For
										}
									}
								}
							}
							if (_selectedHost != null)
								break; // TODO: might not be correct. Was : Exit For
						}
						if (_selectedHost == null) {
							if (!_defaultGroupHost.IsVisible)
								return;
							if (_defaultGroup.IsCollapsed)
								return;
							foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
								if (lviHost.Visible) {
									setSelectedHost(lviHost);
									break; // TODO: might not be correct. Was : Exit For
								}
							}
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible) {
								setSelectedHost(lviHost);
								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
					if (_selectedHost == null)
						return;
				} else {
					ListViewItemHost lowerHost = null;
					if (_showGroups) {
						if (object.ReferenceEquals(_selectedHost.GroupHost, _defaultGroupHost)) {
							if (!_defaultGroup.IsCollapsed & _defaultGroupHost.IsVisible)
								lowerHost = getLowerHost(_selectedHost, _defaultGroupHost.ItemHosts);
						} else {
							int groupIndex = _groupHosts.IndexOf(_selectedHost.GroupHost);
							if (!_groupHosts[groupIndex].Group.IsCollapsed)
								lowerHost = getLowerHost(_selectedHost, _groupHosts[groupIndex].ItemHosts);
							groupIndex += 1;
							while (lowerHost == null & groupIndex < _groupHosts.Count) {
								if (_groupHosts[groupIndex].IsVisible) {
									if (!_groupHosts[groupIndex].Group.IsCollapsed)
										lowerHost = getLowerHost(_selectedHost, _groupHosts[groupIndex].ItemHosts, true);
								}
								groupIndex += 1;
							}
							if (lowerHost == null) {
								if (!_defaultGroup.IsCollapsed & _defaultGroupHost.IsVisible)
									lowerHost = getLowerHost(_selectedHost, _defaultGroupHost.ItemHosts, true);
							}
						}
					} else {
						lowerHost = getLowerHost(_selectedHost, _itemHosts);
					}
					if (lowerHost == null)
						return;
					setSelectedHost(lowerHost);
				}
				if (_selectedHost.Y < _clientArea.Y | _selectedHost.Bounds.Bottom > _clientArea.Bottom) {
					int dy = 0;
					if (_selectedHost.Y < _clientArea.Y) {
						dy = _selectedHost.Y - _clientArea.Y;
					} else {
						dy = _selectedHost.Bounds.Bottom - _clientArea.Bottom;
					}
					_vScroll.Value += dy;
				} else {
					this.Invalidate();
				}
				break;
			case Keys.Left:
				if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Left | _slideLocation == Control.SlideLocation.Right))
					return;
				if (_view == Control.View.Details) {
					if (!_hScroll.Visible)
						return;
					if (_hScroll.Value > 0)
						_hScroll.Value -= 1;
					return;
				}
				if (_selectedHost == null) {
					// There is no selected item, select the first visible item.
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (lvgHost.IsVisible) {
								// However, all items in a collapsed group isn't visible.
								if (!lvgHost.Group.IsCollapsed) {
									foreach (ListViewItemHost lviHost in lvgHost.ItemHosts) {
										if (lviHost.Visible) {
											setSelectedHost(lviHost);
											break; // TODO: might not be correct. Was : Exit For
										}
									}
								}
							}
							if (_selectedHost != null)
								break; // TODO: might not be correct. Was : Exit For
						}
						if (_selectedHost == null) {
							if (!_defaultGroupHost.IsVisible)
								return;
							if (_defaultGroup.IsCollapsed)
								return;
							foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
								if (lviHost.Visible) {
									setSelectedHost(lviHost);
									break; // TODO: might not be correct. Was : Exit For
								}
							}
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible) {
								setSelectedHost(lviHost);
								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
					if (_selectedHost == null)
						return;
				} else {
					ListViewItemHost leftSideHost = null;
					if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top)) {
						if (_showGroups) {
							if (object.ReferenceEquals(_selectedHost.GroupHost, _defaultGroupHost)) {
								if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed)
									leftSideHost = getLeftSideHost(_selectedHost, _defaultGroupHost.ItemHosts);
							} else {
								int groupIndex = _groupHosts.IndexOf(_selectedHost.GroupHost);
								while (leftSideHost == null & groupIndex >= 0) {
									if (_groupHosts[groupIndex].IsVisible & !_groupHosts[groupIndex].Group.IsCollapsed)
										leftSideHost = getLeftSideHost(_selectedHost, _groupHosts[groupIndex].ItemHosts);
									groupIndex -= 1;
								}
							}
						} else {
							leftSideHost = getLeftSideHost(_selectedHost, _itemHosts);
						}
					} else {
						if (_showGroups) {
							if (_selectedHost.GroupHost.IsVisible & !_selectedHost.GroupHost.Group.IsCollapsed)
								leftSideHost = getLeftSideHost(_selectedHost, _selectedHost.GroupHost.ItemHosts);
						} else {
							leftSideHost = getLeftSideHost(_selectedHost, _itemHosts);
						}
					}
					if (leftSideHost == null)
						return;
					setSelectedHost(leftSideHost);
				}
				if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
					int dx = 0;
					if (_selectedHost.X < _clientArea.X) {
						dx = _selectedHost.X - _clientArea.X;
					} else {
						dx = _selectedHost.Bounds.Right - _clientArea.Right;
					}
					if (_view == Control.View.Preview) {
						_slider.Value += dx;
					} else {
						_hScroll.Value += dx;
					}
				} else {
					this.Invalidate();
				}
				break;
			case Keys.Right:
				if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Left | _slideLocation == Control.SlideLocation.Right))
					return;
				if (_view == Control.View.Details) {
					if (!_hScroll.Visible)
						return;
					if (_hScroll.Value < _hScroll.Maximum)
						_hScroll.Value += 1;
					return;
				}
				if (_selectedHost == null) {
					// There is no selected item, select the first visible item.
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							if (lvgHost.IsVisible) {
								// However, all items in a collapsed group isn't visible.
								if (!lvgHost.Group.IsCollapsed) {
									foreach (ListViewItemHost lviHost in lvgHost.ItemHosts) {
										if (lviHost.Visible) {
											setSelectedHost(lviHost);
											break; // TODO: might not be correct. Was : Exit For
										}
									}
								}
							}
							if (_selectedHost != null)
								break; // TODO: might not be correct. Was : Exit For
						}
						if (_selectedHost == null) {
							if (!_defaultGroupHost.IsVisible)
								return;
							if (_defaultGroup.IsCollapsed)
								return;
							foreach (ListViewItemHost lviHost in _defaultGroupHost.ItemHosts) {
								if (lviHost.Visible) {
									setSelectedHost(lviHost);
									break; // TODO: might not be correct. Was : Exit For
								}
							}
						}
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.Visible) {
								setSelectedHost(lviHost);
								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
					if (_selectedHost == null)
						return;
				} else {
					ListViewItemHost rightSideHost = null;
					if (_view == Control.View.Preview & (_slideLocation == Control.SlideLocation.Bottom | _slideLocation == Control.SlideLocation.Top)) {
						if (_showGroups) {
							if (object.ReferenceEquals(_selectedHost.GroupHost, _defaultGroupHost)) {
								if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed)
									rightSideHost = getRightSideHost(_selectedHost, _defaultGroupHost.ItemHosts);
							} else {
								int groupIndex = _groupHosts.IndexOf(_selectedHost.GroupHost);
								while (rightSideHost == null & groupIndex < _groupHosts.Count) {
									if (_groupHosts[groupIndex].IsVisible & !_groupHosts[groupIndex].Group.IsCollapsed)
										rightSideHost = getRightSideHost(_selectedHost, _groupHosts[groupIndex].ItemHosts);
									groupIndex += 1;
								}
								if (rightSideHost == null) {
									if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed)
										rightSideHost = getRightSideHost(_selectedHost, _defaultGroupHost.ItemHosts);
								}
							}
						} else {
							rightSideHost = getRightSideHost(_selectedHost, _itemHosts);
						}
					} else {
						if (_showGroups) {
							if (_selectedHost.GroupHost.IsVisible & !_selectedHost.GroupHost.Group.IsCollapsed)
								rightSideHost = getRightSideHost(_selectedHost, _selectedHost.GroupHost.ItemHosts);
						} else {
							rightSideHost = getRightSideHost(_selectedHost, _itemHosts);
						}
					}
					if (rightSideHost == null)
						return;
					setSelectedHost(rightSideHost);
				}
				if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
					int dx = 0;
					if (_selectedHost.X < _clientArea.X) {
						dx = _selectedHost.X - _clientArea.X;
					} else {
						dx = _selectedHost.Bounds.Right - _clientArea.Right;
					}
					if (_view == Control.View.Preview) {
						_slider.Value += dx;
					} else {
						_hScroll.Value += dx;
					}
				} else {
					this.Invalidate();
				}
				break;
			case Keys.PageUp:
				break;
			case Keys.PageDown:
				break;
			case Keys.Home:
				if (_itemHosts.Count == 0)
					return;
				ListViewItemHost firstHost = null;
				if (_showGroups) {
					foreach (ListViewGroupHost lvgHost in _groupHosts) {
						if (lvgHost.IsVisible & !lvgHost.Group.IsCollapsed) {
							firstHost = getFirstVisibleHost(lvgHost.ItemHosts);
							break; // TODO: might not be correct. Was : Exit For
						}
					}
					if (firstHost == null) {
						if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed)
							firstHost = getFirstVisibleHost(_defaultGroupHost.ItemHosts);
					}
				} else {
					firstHost = getFirstVisibleHost(_itemHosts);
				}
				if (firstHost == null)
					return;
				setSelectedHost(firstHost);
				if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
					int dx = 0;
					if (_selectedHost.X < _clientArea.X) {
						dx = _selectedHost.X - _clientArea.X;
					} else {
						dx = _selectedHost.Bounds.Right - _clientArea.Right;
					}
					if (_view == Control.View.Preview) {
						_slider.Value += dx;
					} else {
						_hScroll.Value += dx;
					}
				} else {
					this.Invalidate();
				}
				break;
			case Keys.End:
				if (_itemHosts.Count == 0)
					return;
				ListViewItemHost lastHost = null;
				if (_showGroups) {
					if (_defaultGroupHost.IsVisible & !_defaultGroup.IsCollapsed)
						lastHost = getLastVisibleHost(_defaultGroupHost.ItemHosts);
					if (lastHost == null) {
						int i = _groupHosts.Count - 1;
						while (i >= 0) {
							if (_groupHosts[i].IsVisible & !_groupHosts[i].Group.IsCollapsed) {
								lastHost = getLastVisibleHost(_defaultGroupHost.ItemHosts);
							}
							i -= 1;
						}
					}
				} else {
					lastHost = getLastVisibleHost(_itemHosts);
				}
				if (lastHost == null)
					return;
				setSelectedHost(lastHost);
				if (_selectedHost.X < _clientArea.X | _selectedHost.Bounds.Right > _clientArea.Right) {
					int dx = 0;
					if (_selectedHost.X < _clientArea.X) {
						dx = _selectedHost.X - _clientArea.X;
					} else {
						dx = _selectedHost.Bounds.Right - _clientArea.Right;
					}
					if (_view == Control.View.Preview) {
						_slider.Value += dx;
					} else {
						_hScroll.Value += dx;
					}
				} else {
					this.Invalidate();
				}
				break;
			case Keys.Space:
				if (!_checkBoxes)
					return;
				if (_selectedHost == null)
					return;
				_selectedHost.Item.Checked = !_selectedHost.Item.Checked;
				break;
			case Keys.F2:
				if (!_labelEdit)
					return;
				if (_selectedHost == null)
					return;
				showTextBoxEditor(_selectedHost);
				break;
		}
	}
	private void ListView_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
	{
		string startsWith = e.KeyChar.ToString();
		ListViewItemHost foundItem = getNextVisibleHost(_selectedHost, startsWith);
		if (foundItem != null) {
			setSelectedHost(foundItem);
			ensureVisible(foundItem.Item);
		}
	}
	private void ListView_LostFocus(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void ListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		_tooltip.Hide();
		this.Focus();
		bool stateChanged = false;
		if (_slider.Visible) {
			_slider.mouseDown();
			stateChanged = stateChanged | _slider.Changed;
		}
		if (!_slider.InterceptMouseDown) {
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					stateChanged = stateChanged | lvgHost.mouseDown();
				}
				stateChanged = stateChanged | _defaultGroupHost.mouseDown();
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					stateChanged = stateChanged | lviHost.mouseDown();
				}
			}
			if (!stateChanged)
				setSelectedHost(null);
		}
		this.Invalidate();
	}
	private void ListView_MouseEnter(object sender, System.EventArgs e)
	{
		bool stateChanged = false;
		if (_vScroll.Visible) {
			Point p = System.Windows.Forms.Control.MousePosition;
			p = this.PointToClient(p);
			if (_clientArea.Contains(p)) {
				if (!_mouseOnClientArea) {
					_mouseOnClientArea = true;
					if (_checkBoxes)
						stateChanged = true;
				}
			}
		} else {
			if (!_mouseOnClientArea) {
				_mouseOnClientArea = true;
				if (_checkBoxes)
					stateChanged = true;
			}
		}
		if (stateChanged)
			this.Invalidate();
	}
	private void ListView_MouseLeave(object sender, System.EventArgs e)
	{
		_tooltip.Hide();
		bool stateChanged = false;
		if (_mouseOnClientArea) {
			_mouseOnClientArea = false;
			if (_checkBoxes)
				stateChanged = true;
		}
		if (_showGroups) {
			foreach (ListViewGroupHost lvgHost in _groupHosts) {
				stateChanged = stateChanged | lvgHost.mouseLeave();
			}
			stateChanged = stateChanged | _defaultGroupHost.mouseLeave();
		} else {
			foreach (ListViewItemHost lviHost in _itemHosts) {
				stateChanged = stateChanged | lviHost.mouseLeave();
			}
		}
		if (_slider.Visible) {
			_slider.mouseLeave();
			stateChanged = stateChanged | _slider.Changed;
		}
		if (stateChanged)
			this.Invalidate();
	}
	private void ListView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		bool stateChanged = false;
		if (_frozenArea.Contains(e.Location)) {
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					stateChanged = stateChanged | lvgHost.mouseLeave();
				}
				stateChanged = stateChanged | _defaultGroupHost.mouseLeave();
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					stateChanged = stateChanged | lviHost.mouseLeave();
				}
			}
			if (stateChanged)
				this.Invalidate();
			return;
		}
		if (_vScroll.Visible) {
			if (_clientArea.Contains(e.Location)) {
				if (!_mouseOnClientArea) {
					_mouseOnClientArea = true;
					if (_checkBoxes)
						stateChanged = true;
				}
			} else {
				if (_mouseOnClientArea) {
					_mouseOnClientArea = false;
					if (_checkBoxes)
						stateChanged = true;
				}
			}
		} else {
			if (!_mouseOnClientArea) {
				_mouseOnClientArea = true;
				if (_checkBoxes)
					stateChanged = true;
			}
		}
		if (!_slider.InterceptMouseDown) {
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					stateChanged = stateChanged | lvgHost.mouseMove(e);
				}
				stateChanged = stateChanged | _defaultGroupHost.mouseMove(e);
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					stateChanged = stateChanged | lviHost.mouseMove(e);
				}
			}
		}
		if (_slider.Visible) {
			_slider.mouseMove(e.Location);
			stateChanged = stateChanged | _slider.Changed;
		}
		if (stateChanged)
			this.Invalidate();
		if (_needToolTip) {
			_tooltip.Show(this, _currentToolTipRect);
			_needToolTip = false;
		}
	}
	private void ListView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		bool stateChanged = false;
		if (!_slider.InterceptMouseDown) {
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					stateChanged = stateChanged | lvgHost.mouseUp();
				}
				stateChanged = stateChanged | _defaultGroupHost.mouseUp();
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					stateChanged = stateChanged | lviHost.mouseUp();
				}
			}
		}
		if (_slider.Visible)
			_slider.mouseUp();
		if (stateChanged)
			this.Invalidate();
	}
	private void ListView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		bool _next = true;
		if (e.Delta > 0) {
			if (_vScroll.Visible & _vScroll.Value > 0) {
				if (_vScroll.Value >= _vScroll.SmallChange) {
					_vScroll.Value -= _vScroll.SmallChange;
				} else {
					_vScroll.Value = 0;
				}
				return;
			}
			if (_hScroll.Visible & _hScroll.Value > 0) {
				if (_hScroll.Value >= _hScroll.SmallChange) {
					_hScroll.Value -= _hScroll.SmallChange;
				} else {
					_hScroll.Value = 0;
				}
				return;
			}
			if (_slider.Visible & _slider.Value > 0) {
				int smallChange = _slider.Maximum / 10;
				if (_slider.Value >= smallChange) {
					_slider.Value -= smallChange;
				} else {
					_slider.Value = 0;
				}
				return;
			}
		} else if (e.Delta < 0) {
			if (_vScroll.Visible & _vScroll.Value < _vScroll.Maximum) {
				if (_vScroll.Value <= _vScroll.Maximum - _vScroll.SmallChange) {
					_vScroll.Value += _vScroll.SmallChange;
				} else {
					_vScroll.Value = _vScroll.Maximum;
				}
				return;
			}
			if (_hScroll.Visible & _hScroll.Value < _hScroll.Maximum) {
				if (_hScroll.Value <= _hScroll.Maximum - _hScroll.SmallChange) {
					_hScroll.Value += _hScroll.SmallChange;
				} else {
					_hScroll.Value = _hScroll.Maximum;
				}
				return;
			}
			if (_slider.Visible & _slider.Value < _slider.Maximum) {
				int smallChange = _slider.Maximum / 10;
				if (_slider.Value <= _slider.Maximum - smallChange) {
					_slider.Value += smallChange;
				} else {
					_slider.Value = _slider.Maximum;
				}
				return;
			}
		}
	}
	private void ListView_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		if (_view == Control.View.Preview) {
			e.Graphics.Clear(Color.Black);
		} else {
			e.Graphics.Clear(Color.White);
		}
		if (_view == Control.View.Details) {
			// Create separator pen for each column.
			LinearGradientBrush lineBrush = new LinearGradientBrush(_clientArea, Color.Black, Color.White, LinearGradientMode.Vertical);
			lineBrush.InterpolationColors = _linePenBlend;
			Pen linePen = new Pen(lineBrush);
			// Paint unfrozen columns
			List<ColumnHeader> unfrozenCols = _columnControl.UnFrozenColumns;
			List<ColumnHeader> frozenCols = _columnControl.FrozenColumns;
			Rectangle colRect = default(Rectangle);
			foreach (ColumnHeader ch in unfrozenCols) {
				colRect = _columnControl.ColumnRectangle[ch];
				colRect.Y = _clientArea.Y;
				colRect.Height = _clientArea.Height;
				ColumnBackgroundPaintEventArgs pe = new ColumnBackgroundPaintEventArgs(ch, _columns.IndexOf[ch], e.Graphics, colRect);
				if (ColumnBackgroundPaint != null) {
					ColumnBackgroundPaint(this, pe);
				}
				e.Graphics.DrawLine(linePen, colRect.Right, colRect.Y, colRect.Right, colRect.Bottom);
			}
			foreach (ListViewItemHost lviHost in _itemHosts) {
				if (!lviHost.Frozen)
					lviHost.drawUnFrozen(e.Graphics, frozenCols.Count);
			}
			if (frozenCols.Count > 0) {
				colRect = _columnControl.FrozenRectangle;
				colRect.Y = 0;
				colRect.Height = this.Height;
				colRect.Width += colRect.X;
				colRect.X = 0;
				e.Graphics.FillRectangle(Brushes.White, colRect);
				foreach (ColumnHeader ch in frozenCols) {
					colRect = _columnControl.ColumnRectangle[ch];
					colRect.Y = _clientArea.Y;
					colRect.Height = _clientArea.Height;
					ColumnBackgroundPaintEventArgs pe = new ColumnBackgroundPaintEventArgs(ch, _columns.IndexOf[ch], e.Graphics, colRect);
					if (ColumnBackgroundPaint != null) {
						ColumnBackgroundPaint(this, pe);
					}
					e.Graphics.DrawLine(linePen, colRect.Right, colRect.Y, colRect.Right, colRect.Bottom);
				}
				foreach (ListViewItemHost lviHost in _itemHosts) {
					if (!lviHost.Frozen)
						lviHost.drawFrozen(e.Graphics, unfrozenCols.Count);
				}
			}
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.draw(e.Graphics, _checkBoxes);
				}
				_defaultGroupHost.draw(e.Graphics, _checkBoxes);
			}
			// Draw frozen items
			if (_frozenArea.Height > 0) {
				// Clip the graphics object
				e.Graphics.SetClip(_frozenArea);
				// Set the background
				Rectangle rectBg = new Rectangle(0, _frozenArea.Y, this.Width, _frozenArea.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(247, 248, 250)), rectBg);
				if (_frozenRowsDock == RowsDock.Top) {
					e.Graphics.DrawLine(Pens.LightGray, 0, _frozenArea.Bottom - 1, this.Width, _frozenArea.Bottom - 1);
				} else {
					e.Graphics.DrawLine(Pens.LightGray, 0, _frozenArea.Y, this.Width, _frozenArea.Y);
				}
				// Draw the items
				foreach (ColumnHeader ch in unfrozenCols) {
					colRect = _columnControl.ColumnRectangle[ch];
					colRect.Y = _clientArea.Y;
					colRect.Height = _clientArea.Height;
					ColumnBackgroundPaintEventArgs pe = new ColumnBackgroundPaintEventArgs(ch, _columns.IndexOf[ch], e.Graphics, colRect);
					if (ColumnBackgroundPaint != null) {
						ColumnBackgroundPaint(this, pe);
					}
					e.Graphics.DrawLine(linePen, colRect.Right, colRect.Y, colRect.Right, colRect.Bottom);
				}
				foreach (ListViewItemHost lviHost in _frozenHosts) {
					lviHost.drawUnFrozen(e.Graphics, frozenCols.Count);
				}
				if (frozenCols.Count > 0) {
					colRect = _columnControl.FrozenRectangle;
					colRect.Y = 0;
					colRect.Height = this.Height;
					colRect.Width += colRect.X;
					colRect.X = 0;
					e.Graphics.FillRectangle(Brushes.White, colRect);
					foreach (ColumnHeader ch in frozenCols) {
						colRect = _columnControl.ColumnRectangle[ch];
						colRect.Y = _clientArea.Y;
						colRect.Height = _clientArea.Height;
						ColumnBackgroundPaintEventArgs pe = new ColumnBackgroundPaintEventArgs(ch, _columns.IndexOf[ch], e.Graphics, colRect);
						if (ColumnBackgroundPaint != null) {
							ColumnBackgroundPaint(this, pe);
						}
						e.Graphics.DrawLine(linePen, colRect.Right, colRect.Y, colRect.Right, colRect.Bottom);
					}
					foreach (ListViewItemHost lviHost in _frozenHosts) {
						lviHost.drawFrozen(e.Graphics, unfrozenCols.Count);
					}
				}
				e.Graphics.ResetClip();
			}
			linePen.Dispose();
			lineBrush.Dispose();
			// Draw the checkboxes and rownumbers, if specified.
			if (_checkBoxes | _rowNumbers) {
				Rectangle aRect = new Rectangle(0, 0, 0, this.Height);
				if (_checkBoxes)
					aRect.Width = 22;
				if (_rowNumbers)
					aRect.Width += e.Graphics.MeasureString(Convert.ToString(_items.Count), Font).Width + 2;
				int rowNumbersX = 1;
				LinearGradientBrush rectBrush = new LinearGradientBrush(aRect, Color.Black, Color.White, LinearGradientMode.Horizontal);
				rectBrush.InterpolationColors = Renderer.Column.NormalBlend;
				e.Graphics.FillRectangle(rectBrush, aRect);
				if (_checkBoxes) {
					rowNumbersX = 23;
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							lvgHost.drawItemsCheckBox(e.Graphics);
						}
						_defaultGroupHost.drawItemsCheckBox(e.Graphics);
					} else {
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.IsVisible)
								lviHost.drawCheckBox(e.Graphics);
						}
					}
				}
				if (_rowNumbers) {
					int rowWidth = _clientArea.X - (rowNumbersX + 1);
					StringFormat rowFormat = new StringFormat();
					rowFormat.LineAlignment = StringAlignment.Center;
					rowFormat.Alignment = StringAlignment.Far;
					if (_showGroups) {
						foreach (ListViewGroupHost lvgHost in _groupHosts) {
							lvgHost.drawItemsRowNumber(e.Graphics, rowFormat, rowNumbersX, rowWidth, (Enabled ? Brushes.Black : Brushes.Gray));
						}
						_defaultGroupHost.drawItemsRowNumber(e.Graphics, rowFormat, rowNumbersX, rowWidth, (Enabled ? Brushes.Black : Brushes.Gray));
					} else {
						Rectangle rowRect = new Rectangle(rowNumbersX, 0, rowWidth, 0);
						foreach (ListViewItemHost lviHost in _itemHosts) {
							if (lviHost.IsVisible) {
								rowRect.Y = lviHost.Y;
								rowRect.Height = lviHost.Bounds.Height;
								e.Graphics.DrawString(Convert.ToString(lviHost.RowIndex), this.Font, (Enabled ? Brushes.Black : Brushes.Gray), rowRect, rowFormat);
							}
						}
					}
					rowFormat.Dispose();
				}
				aRect.Y = _frozenArea.Y;
				aRect.Height = _frozenArea.Height;
				e.Graphics.FillRectangle(rectBrush, aRect);
				rectBrush.Dispose();
				e.Graphics.DrawLine(Renderer.Column.NormalBorderPen, aRect.Right - 1, 0, aRect.Right - 1, this.Height);
			}
		} else {
			if (_view == Control.View.Preview) {
				if (_slideBgImage != null) {
					e.Graphics.DrawImage(_slideBgImage, 0, _columnControl.Bottom);
				}
				Rectangle slideRect = new Rectangle(0, 0, 0, 0);
				LinearGradientBrush slideBrush = null;
				switch (_slideLocation) {
					case Control.SlideLocation.Bottom:
						slideRect = new Rectangle(0, this.Height - (_slideItemSize + 15), this.Width, _slideItemSize + 15);
						slideBrush = new LinearGradientBrush(slideRect, Color.Black, Color.FromArgb(127, 0, 0, 0), 270);
						slideRect.Y += 1;
						break;
					case Control.SlideLocation.Left:
						slideRect = new Rectangle(0, 0, _slideItemSize + 15, this.Height);
						slideBrush = new LinearGradientBrush(slideRect, Color.Black, Color.FromArgb(127, 0, 0, 0), 0);
						slideRect.Width -= 1;
						break;
					case Control.SlideLocation.Right:
						slideRect = new Rectangle(this.Width - (_slideItemSize + 15), 0, _slideItemSize + 15, this.Height);
						slideBrush = new LinearGradientBrush(slideRect, Color.Black, Color.FromArgb(127, 0, 0, 0), 180);
						slideRect.X += 1;
						break;
					case Control.SlideLocation.Top:
						slideRect = new Rectangle(0, _clientArea.Y - 1, this.Width, _slideItemSize + 15);
						slideBrush = new LinearGradientBrush(slideRect, Color.Black, Color.FromArgb(127, 0, 0, 0), 90);
						slideRect.Height -= 1;
						break;
				}
				e.Graphics.FillRectangle(slideBrush, slideRect);
				slideBrush.Dispose();
			}
			if (_showGroups) {
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.draw(e.Graphics, _mouseOnClientArea & _checkBoxes);
				}
				_defaultGroupHost.draw(e.Graphics, _mouseOnClientArea & _checkBoxes);
			} else {
				foreach (ListViewItemHost lviHost in _itemHosts) {
					switch (_view) {
						case Control.View.Icon:
						case Control.View.Thumbnail:
							lviHost.drawIconThumbnail(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.List:
							lviHost.drawList(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.Preview:
							lviHost.drawPreview(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.Tile:
							lviHost.drawTile(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
					}
				}
			}
			if (_selectedHost != null) {
				if (_selectedHost.IsVisible) {
					switch (_view) {
						case Control.View.Icon:
						case Control.View.Thumbnail:
							_selectedHost.drawIconThumbnail(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.List:
							_selectedHost.drawList(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.Preview:
							_selectedHost.drawPreview(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
						case Control.View.Tile:
							_selectedHost.drawTile(e.Graphics, _mouseOnClientArea & _checkBoxes);
							break;
					}
				}
			}
		}
		if (_slider.Visible)
			_slider.draw(e.Graphics);
		if (_hScroll.Visible & _vScroll.Visible) {
			Rectangle aRect = new Rectangle(_vScroll.Left - 1, _hScroll.Top - 1, _vScroll.Width + 1, _hScroll.Height + 1);
			e.Graphics.FillRectangle(Brushes.Gainsboro, aRect);
		}
		_slider.Changed = false;
		e.Graphics.DrawRectangle(Pens.LightGray, 0, 0, this.Width - 1, this.Height - 1);
		if (_columnControl.OnColumnResize)
			e.Graphics.DrawLine(Pens.Black, _columnControl.ResizeCurrentX, 0, _columnControl.ResizeCurrentX, this.Height);
	}
	private void ListView_Resize(object sender, System.EventArgs e)
	{
		_columnControl.relocateHosts();
		measureAll();
		_columnControl.moveColumns((_hScroll.Visible ? -_hScroll.Value : 0));
		relocateAll();
		if (_view == Control.View.Preview)
			drawPreviewHosts();
		this.Invalidate(true);
	}
	private void _checkedItems_AfterClear(object sender, CollectionEventArgs e)
	{
		foreach (ListViewItem item in _items) {
			item.Checked = false;
		}
	}
	private void _checkedItems_AfterRemove(object sender, CollectionEventArgs e)
	{
		ListViewItem item = (ListViewItem)e.Item;
		item.Checked = false;
	}
	private void _columnControl_AfterColumnCustomFilter(object sender, ColumnEventArgs e)
	{
		ColumnCustomFilterEventArgs cEvent = new ColumnCustomFilterEventArgs(e.Column);
		if (ColumnCustomFilter != null) {
			ColumnCustomFilter(this, cEvent);
		}
		if (!cEvent.CancelFilter) {
			List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
			foreach (ListViewItemHost lviHost in _itemHosts) {
				lviHost.Visible = filterItem(lviHost.Item, handlers);
			}
			measureAll();
			relocateAll();
			this.Invalidate();
			if (ColumnFilterChanged != null) {
				ColumnFilterChanged(this, e);
			}
		}
	}
	private void _columnControl_AfterColumnFilter(object sender, ColumnEventArgs e)
	{
		List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			lviHost.Visible = filterItem(lviHost.Item, handlers);
		}
		measureAll();
		relocateAll();
		this.Invalidate();
		if (ColumnFilterChanged != null) {
			ColumnFilterChanged(this, e);
		}
	}
	private void _columnControl_AfterColumnResize(object sender, ColumnEventArgs e)
	{
		if (_view == Control.View.Details)
			measureAll();
		_columnControl.moveColumns((_hScroll.Visible ? -_hScroll.Value : 0));
		relocateAll();
		this.Invalidate();
		if (ColumnSizeChanged != null) {
			ColumnSizeChanged(this, e);
		}
	}
	private void _columnControl_CheckedChanged(object sender, System.EventArgs e)
	{
		_internalThread = true;
		foreach (ListViewItem lvi in _items) {
			lvi.setChecked(_columnControl.CheckState == CheckState.Checked);
			if (lvi.Group != null) {
				if (lvi.Checked) {
					lvi.Group.CheckedItems.Add(lvi);
				} else {
					lvi.Group.CheckedItems.Remove(lvi);
				}
			}
		}
		foreach (ListViewGroupHost lvgHost in _groupHosts) {
			lvgHost.checkCheckedState();
		}
		_defaultGroupHost.checkCheckedState();
		this.Invalidate();
		_internalThread = false;
	}
	private void _columnControl_ColumnOrderChanged(object sender, ColumnEventArgs e)
	{
		if (_view == Control.View.Details) {
			measureAll();
			relocateAll();
			this.Invalidate();
		}
		if (ColumnOrderChanged != null) {
			ColumnOrderChanged(this, e);
		}
	}
	private void _columns_AfterClear(object sender, CollectionEventArgs e)
	{
		_hScroll.Visible = false;
	}
	private void _columns_AfterInsert(object sender, CollectionEventArgs e)
	{
		if (_view == Control.View.Details)
			measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void _columns_AfterRemove(object sender, CollectionEventArgs e)
	{
		if (_view == Control.View.Details)
			measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void _slider_ValueChanged(object sender, System.EventArgs e)
	{
		if (_itemHosts.Count == 0)
			return;
		if (_slider.Orientation == Orientation.Horizontal) {
			if (_showGroups) {
				int lastX = _defaultGroupHost.X;
				if (_groupHosts.Count > 0)
					lastX = _groupHosts[0].X;
				int dx = (_clientArea.X - _slider.Value) - lastX;
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.X += dx;
				}
				_defaultGroupHost.X += dx;
			} else {
				int lastX = _itemHosts[0].X;
				int dx = (_clientArea.X - _slider.Value) - lastX;
				foreach (ListViewItemHost lviHost in _itemHosts) {
					lviHost.X += dx;
				}
			}
		} else {
			if (_showGroups) {
				int lastY = _defaultGroupHost.Y;
				if (_groupHosts.Count > 0)
					lastY = _groupHosts[0].Y;
				int dy = (_clientArea.Y - _slider.Value) - lastY;
				foreach (ListViewGroupHost lvgHost in _groupHosts) {
					lvgHost.Y += dy;
				}
				_defaultGroupHost.Y += dy;
			} else {
				int lastY = _itemHosts[0].Y;
				int dy = (_clientArea.Y - _slider.Value) - lastY;
				foreach (ListViewItemHost lviHost in _itemHosts) {
					lviHost.Y += dy;
				}
			}
		}
		this.Invalidate();
	}
	private void _defaultGroup_AfterCheck(object sender, GroupEventArgs e)
	{
		if (DefaultGroupAfterCheck != null) {
			DefaultGroupAfterCheck(this, e);
		}
	}
	private void _defaultGroup_AfterCollape(object sender, GroupEventArgs e)
	{
		if (DefaultGroupAfterCollape != null) {
			DefaultGroupAfterCollape(this, e);
		}
	}
	private void _defaultGroup_AfterExpand(object sender, GroupEventArgs e)
	{
		if (DefaultGroupAfterExpand != null) {
			DefaultGroupAfterExpand(this, e);
		}
	}
	private void _defaultGroup_BeforeCheck(object sender, GroupEventArgs e)
	{
		if (DefaultGroupBeforeCheck != null) {
			DefaultGroupBeforeCheck(this, e);
		}
	}
	private void _defaultGroup_BeforeCollapse(object sender, GroupEventArgs e)
	{
		if (DefaultGroupBeforeCollapse != null) {
			DefaultGroupBeforeCollapse(this, e);
		}
	}
	private void _defaultGroup_BeforeExpand(object sender, GroupEventArgs e)
	{
		if (DefaultGroupBeforeExpand != null) {
			DefaultGroupBeforeExpand(this, e);
		}
	}
	private void _tooltip_Draw(object sender, DrawEventArgs e)
	{
		Renderer.ToolTip.drawToolTip(_currentToolTipTitle, _currentToolTip, _currentToolTipImage, e.Graphics, e.Rectangle);
		_currentToolTipTitle = "";
		_currentToolTip = "";
		_currentToolTipImage = null;
	}
	private void _tooltip_Popup(object sender, PopupEventArgs e)
	{
		e.Size = Renderer.ToolTip.measureSize(_currentToolTipTitle, _currentToolTip, _currentToolTipImage);
	}
	private void _txtEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		switch (e.KeyData) {
			case Keys.Escape:
				this.Focus();
				break;
			case Keys.Return:
				if (!_txtEditor.Multiline) {
					if (_currentEditedHost != null) {
						if (_currentEditedHost.Item.Text != _txtEditor.Text) {
							_currentEditedHost.Item.Text = _txtEditor.Text;
							if (AfterLabelEdit != null) {
								AfterLabelEdit(this, new ItemEventArgs(_currentEditedHost.Item));
							}
						}
					}
					this.Focus();
				}
				break;
			case Keys.Control | Keys.Return:
				if (_txtEditor.Multiline) {
					if (_currentEditedHost != null) {
						if (_currentEditedHost.Item.Text != _txtEditor.Text) {
							_currentEditedHost.Item.Text = _txtEditor.Text;
							if (AfterLabelEdit != null) {
								AfterLabelEdit(this, new ItemEventArgs(_currentEditedHost.Item));
							}
						}
					}
					this.Focus();
				}
				break;
		}
	}
	private void _txtEditor_LostFocus(object sender, System.EventArgs e)
	{
		_txtEditor.Visible = false;
	}
	private void _frozenItems_AfterClear(object sender, CollectionEventArgs e)
	{
		if (_view == Control.View.Details) {
			measureAll();
			relocateAll();
			this.Invalidate();
		}
	}
	private void _frozenItems_AfterInsert(object sender, CollectionEventArgs e)
	{
		if (_view == Control.View.Details) {
			measureAll();
			relocateAll();
			this.Invalidate();
		}
	}
	private void _frozenItems_AfterRemove(object sender, CollectionEventArgs e)
	{
		if (_view == Control.View.Details) {
			measureAll();
			relocateAll();
			this.Invalidate();
		}
	}
	private void _frozenItems_Clearing(object sender, CollectionEventArgs e)
	{
		foreach (ListViewItemHost lviHost in _itemHosts) {
			foreach (ListViewItem lvi in _frozenItems) {
				if (object.ReferenceEquals(lviHost.Item, lvi)) {
					lviHost.Frozen = false;
				}
			}
		}
		_frozenHosts.Clear();
	}
	private void _frozenItems_Inserting(object sender, CollectionEventArgs e)
	{
		ListViewItem newFrozen = (ListViewItem)e.Item;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, newFrozen)) {
				lviHost.Frozen = true;
				_frozenHosts.Add(lviHost);
				return;
			}
		}
	}
	private void _frozenItems_Removing(object sender, CollectionEventArgs e)
	{
		ListViewItem remFrozen = (ListViewItem)e.Item;
		foreach (ListViewItemHost lviHost in _itemHosts) {
			if (object.ReferenceEquals(lviHost.Item, remFrozen)) {
				lviHost.Frozen = false;
				if (_frozenHosts.Contains(lviHost))
					_frozenHosts.Remove(lviHost);
				return;
			}
		}
	}
	#endregion
}
public enum View
{
	Details = System.Windows.Forms.View.Details,
	List = System.Windows.Forms.View.List,
	Icon = System.Windows.Forms.View.SmallIcon,
	Tile = System.Windows.Forms.View.Tile,
	Thumbnail = System.Windows.Forms.View.LargeIcon,
	[Description("Show full sized image on slide show.")]
	Preview = 5
}
/// <summary>
/// Specifies the position and manner in which the items should be shown in Preview view mode.
/// </summary>
public enum SlideLocation
{
	Top = 0,
	Left = 1,
	Right = 2,
	Bottom = 3
}
/// <summary>
/// Specifies the position and manner in which the frozen items is docked.
/// </summary>
public enum RowsDock
{
	Top = 1,
	Bottom = 0
}