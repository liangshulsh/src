using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
// Created by : Burhanudin Ashari (red_moon@CodeProject) @ July 22, 2010.
// Modified by : Burhanudin Ashari (red_moon@CodeProject) @ August 30 - September 05, 2010.
// I'm not live my life for the code, but I'll live and enhance it through the code.
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
/// <summary>
/// Represent a control to displays a collection of TreeNode.
/// </summary>
/// <remarks>
/// This control allows you to display a list of nodes with node text, and optionally, additional information of an item (subitem), and images displayed in when collapsed or expanded.
/// </remarks>
public class MultiColumnTree : Windows.Forms.Control
{
	#region "Private Constants."
	/// <summary>
	/// Margin size of each subitem from its bounding rectangle.
	/// </summary>
	const int _subItemMargin = 3;
	/// <summary>
	/// Minimum value of the Indent property.
	/// </summary>
		#endregion
	const int _mininumIndent = 15;
	#region "Public Classes."
	/// <summary>
	/// Represent a collection of ColumnHeader objects in a MultiColumnTree.
	/// </summary>
	[Description("Represent a collection of ColumnHeader objects.")]
	public class ColumnHeaderCollection : CollectionBase
	{
			#region "Constructor"
		internal MultiColumnTree _owner;
		public ColumnHeaderCollection(MultiColumnTree owner) : base()
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
		#region "Friend Events"
		internal event ColumnFilterChangedEventHandler ColumnFilterChanged;
		internal delegate void ColumnFilterChangedEventHandler(object sender, EventArgs e);
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
				header.EnableFilteringChanged += columnEnableFilterChanged;
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
			_owner.measureAll();
			_owner.relocateAll();
			_owner.Invalidate();
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
					_owner.Invalidate();
				}
			}
		}
		private void columnItemsRelatedValueChanged(object sender, EventArgs e)
		{
			_owner.Invalidate();
		}
		private void columnCustomFormatChanged(object sender, EventArgs e)
		{
			ColumnHeader ch = (ColumnHeader)sender;
			if (ch.Format == ColumnFormat.Custom | ch.Format == ColumnFormat.CustomDateTime) {
				_owner.Invalidate();
			}
		}
		private void columnEnableFilterChanged(object sender, EventArgs e)
		{
			if (ColumnFilterChanged != null) {
				ColumnFilterChanged(sender, e);
			}
		}
		#endregion
	}
	/// <summary>
	/// Represent a collection of Checked TreeNode objects in a MultiColumnTree.
	/// </summary>
	[Description("Represent a collection of Checked TreeNode objects in a MultiColumnTree.")]
	public class CheckedTreeNodeCollection : CollectionBase
	{
			#region "Constructor"
		internal MultiColumnTree _owner;
		public CheckedTreeNodeCollection(MultiColumnTree owner) : base()
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
		[Description("Gets a TreeNode object in the collection specified by its index.")]
		public TreeNode this[int index] {
			get {
				if (index >= 0 & index < List.Count)
					return (TreeNode)List[index];
				return null;
			}
		}
		[Description("Gets the index of a TreeNode object in the collection.")]
		public int IndexOf {
			get { return this.List.IndexOf(node); }
		}
		#endregion
		#region "Public Methods"
		[Description("Add a TreeNode object to the collection.")]
		internal TreeNode Add(TreeNode node)
		{
			// Avoid adding the same item multiple times.
			if (!this.Contains(node)) {
				int index = List.Add(node);
				return (TreeNode)List[index];
			}
			return node;
		}
		[Description("Add a TreeNode collection to the collection.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		internal void AddRange(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				this.Add(node);
			}
		}
		[Description("Remove a TreeNode object from the collection.")]
		public void Remove(TreeNode node)
		{
			if (List.Contains(node)) {
				if (!node.Checked)
					node.Checked = false;
				List.Remove(node);
			}
		}
		[Description("Determine whether a TreeNode object exist in the collection.")]
		public bool Contains(TreeNode node)
		{
			return List.Contains(node);
		}
		#endregion
		#region "Protected Overriden Methods"
		[Description("Performs additional custom processes when validating a value.")]
		protected override void OnValidate(object value)
		{
			if (!typeof(TreeNode).IsAssignableFrom(value.GetType())) {
				throw new ArgumentException("Value must TreeNode", "value");
			}
		}
		protected override void OnClear()
		{
			foreach (TreeNode tn in List) {
				tn.Checked = false;
			}
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
	#endregion
	#region "Public Events."
	/// <summary>
	/// Occurs before tree node check box is checked.
	/// </summary>
	public event BeforeCheckEventHandler BeforeCheck;
	public delegate void BeforeCheckEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs afetr tree node check box is checked.
	/// </summary>
	public event AfterCheckEventHandler AfterCheck;
	public delegate void AfterCheckEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs before tree node is collapsed.
	/// </summary>
	public event BeforeCollapseEventHandler BeforeCollapse;
	public delegate void BeforeCollapseEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs after tree node is collapsed.
	/// </summary>
	public event AfterCollapseEventHandler AfterCollapse;
	public delegate void AfterCollapseEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs before tree node is expanded.
	/// </summary>
	public event BeforeExpandEventHandler BeforeExpand;
	public delegate void BeforeExpandEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs after tree node is expanded.
	/// </summary>
	public event AfterExpandEventHandler AfterExpand;
	public delegate void AfterExpandEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs before tree node label text is edited.
	/// </summary>
	public event BeforeLabelEditEventHandler BeforeLabelEdit;
	public delegate void BeforeLabelEditEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs after tree node label text is edited.
	/// </summary>
	public event AfterLabelEditEventHandler AfterLabelEdit;
	public delegate void AfterLabelEditEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs before tree node is selected.
	/// </summary>
	public event BeforeSelectEventHandler BeforeSelect;
	public delegate void BeforeSelectEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs after tree node is selected.
	/// </summary>
	public event AfterSelectEventHandler AfterSelect;
	public delegate void AfterSelectEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs when mouse pointer is hover a tree node.
	/// </summary>
	public event NodeMouseHoverEventHandler NodeMouseHover;
	public delegate void NodeMouseHoverEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs when mouse pointer is leaving a tree node.
	/// </summary>
	public event NodeMouseLeaveEventHandler NodeMouseLeave;
	public delegate void NodeMouseLeaveEventHandler(object sender, TreeNodeEventArgs e);
	/// <summary>
	/// Occurs when mouse button is pressed over a tree node.
	/// </summary>
	public event NodeMouseDownEventHandler NodeMouseDown;
	public delegate void NodeMouseDownEventHandler(object sender, TreeNodeMouseEventArgs e);
	/// <summary>
	/// Occurs when mouse button is released over a tree node.
	/// </summary>
	public event NodeMouseUpEventHandler NodeMouseUp;
	public delegate void NodeMouseUpEventHandler(object sender, TreeNodeMouseEventArgs e);
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
	/// Occurs when background of a column in MultiColumnTree need to paint.
	/// </summary>
	public event ColumnBackgroundPaintEventHandler ColumnBackgroundPaint;
	public delegate void ColumnBackgroundPaintEventHandler(object sender, ColumnBackgroundPaintEventArgs e);
	/// <summary>
	/// Occurs when selected node has been changed.
	/// </summary>
	public event SelectedNodeChangedEventHandler SelectedNodeChanged;
	public delegate void SelectedNodeChangedEventHandler(object sender, EventArgs e);
	#endregion
	#region "Members."
	// Components
	private TreeNodeCollection withEventsField__nodes;
	public TreeNodeCollection _nodes {
		get { return withEventsField__nodes; }
		set {
			if (withEventsField__nodes != null) {
				withEventsField__nodes.AfterClear -= _nodes_AfterClear;
				withEventsField__nodes.AfterInsert -= _nodes_AfterInsert;
				withEventsField__nodes.AfterRemove -= _nodes_AfterRemove;
			}
			withEventsField__nodes = value;
			if (withEventsField__nodes != null) {
				withEventsField__nodes.AfterClear += _nodes_AfterClear;
				withEventsField__nodes.AfterInsert += _nodes_AfterInsert;
				withEventsField__nodes.AfterRemove += _nodes_AfterRemove;
			}
		}
	}
	CheckedTreeNodeCollection _checkedNodes;
	private ColumnHeaderCollection withEventsField__columns;
	public ColumnHeaderCollection _columns {
		get { return withEventsField__columns; }
		set {
			if (withEventsField__columns != null) {
				withEventsField__columns.AfterClear -= _columns_AfterClear;
				withEventsField__columns.AfterInsert -= _columns_AfterInsert;
				withEventsField__columns.AfterRemove -= _columns_AfterRemove;
				withEventsField__columns.ColumnFilterChanged -= _columns_ColumnFilterChanged;
			}
			withEventsField__columns = value;
			if (withEventsField__columns != null) {
				withEventsField__columns.AfterClear += _columns_AfterClear;
				withEventsField__columns.AfterInsert += _columns_AfterInsert;
				withEventsField__columns.AfterRemove += _columns_AfterRemove;
				withEventsField__columns.ColumnFilterChanged += _columns_ColumnFilterChanged;
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
	bool _showColumnOptions = true;
	bool _allowMultiline = false;
	CultureInfo _ci = Renderer.Drawing.en_us_ci;
	bool _fullRowSelect = false;
	bool _labelEdit = false;
	bool _nodeToolTip = true;
	string _pathSeparator = "\\";
	bool _showRootLines = false;
	bool _showNodeLines = false;
	bool _showImages = true;
	int _indent = 20;
	// Internal use
		// Index of a ColumnHeader that perform sort operation.
	int _columnRef = -1;
		// Indicating whether the MultiColumnTree perform validation on each action.
	bool _performValidation = true;
		// Indicating whether an event is called from internal process, to avoid multiple calls of an operation on an event.
	bool _internalThread = false;
		// An area to draw TreeNode and ListViewGroup.
	Rectangle _clientArea;
		// A color blend to draw line separator of each column in ListView.
	ColorBlend _linePenBlend;
		// Indicating when a tooltip need to be shown.
	bool _needToolTip = false;
		// A TreeNodeHost object that performs label editing operation.
	TreeNodeHost _currentEditedHost = null;
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
	// For text measuring purposes
		// A bitmap to create a Graphics object.
	Bitmap _gBmp = new Bitmap(1, 1);
		// A Graphics object to measure text, to support text formating in measurement.
	Graphics _gObj = Graphics.FromImage(_gBmp);
	// TreeNode Host
		// A list of TreeNodeHost.  All node host is stored here.
	List<TreeNodeHost> _nodeHosts = new List<TreeNodeHost>();
		// Host of selected item.
	TreeNodeHost _selectedHost = null;
	// ToolTip
		// A tooltip text needed to be shown.
	string _currentToolTip = "";
		// A tooltip title needed to be shown.
	string _currentToolTipTitle = "";
		// A tooltip image needed to be shown.
	Image _currentToolTipImage = null;
		// An area that must be avoided by the tooltip.
	Rectangle _currentToolTipRect;
		// A TreeNodeHost that need the tooltip to be shown.
	TreeNodeHost _tooltipCaller = null;
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
			/// Add a TreeNode or its subitem to the filter parameter, based on the column and the associated subitems.
			/// </summary>
			/// <param name="item">TreeNode object to be added.</param>
			public void addItemToFilter(TreeNode item)
			{
				if (_column.EnableFiltering) {
					int columnIndex = _owner._owner._columns.IndexOf[_column];
					if (item.SubItems(columnIndex) != null) {
						_filterHandler.addFilter(item.SubItems(columnIndex).Value);
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
		MultiColumnTree _owner;
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
		public ColumnHeaderControl(MultiColumnTree owner, Font font)
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
			int availWidth = this.Width - (_owner._vScroll.Width + 5);
			int spaceLeft = 1;
			int frozenWidth = 0;
			if (_owner._checkBoxes) {
				spaceLeft += 22;
				availWidth -= 22;
			}
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
		public void moveColumns(int xScroll)
		{
			int startX = 1;
			if (_owner._checkBoxes) {
				startX += 22;
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
			if (_owner._performValidation) {
				foreach (ColumnHost ch in _columnHosts) {
					if (_owner.Columns.IndexOf[ch.Column] == columnIndex & ch.Column.EnableFiltering) {
						ch.FilterHandler.reloadFilter(objs);
						break; // TODO: might not be correct. Was : Exit For
					}
				}
			}
		}
		/// <summary>
		/// Add a filter parameter to a ColumnFilterHandle on specified ColumnHeader.
		/// </summary>
		public void addFilter(int columnIndex, object obj)
		{
			if (_owner._performValidation) {
				foreach (ColumnHost ch in _columnHosts) {
					if (_owner.Columns.IndexOf[ch.Column] == columnIndex & ch.Column.EnableFiltering) {
						ch.FilterHandler.addFilter(obj);
						break; // TODO: might not be correct. Was : Exit For
					}
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
				ch.FilterHandler.Values.Clear();
			}
		}
		/// <summary>
		/// Clear all filter parameters on a specified column.
		/// </summary>
		public void clearFilters(int columnIndex)
		{
			foreach (ColumnHost ch in _columnHosts) {
				if (object.ReferenceEquals(ch.Column, _owner._columns[columnIndex])) {
					ch.FilterHandler.Items.Clear();
					ch.FilterHandler.Values.Clear();
					return;
				}
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
			if (_owner._checkBoxes) {
				int reservedWidth = 0;
				if (_owner._checkBoxes)
					reservedWidth += 22;
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
	/// <summary>
	/// Class to host a TreeNode object and handles all of its operations, a visual representation on a MultiColumnTree.
	/// </summary>
	[Description("Class to host a TreeNode object and handles all of its operations.")]
	private class TreeNodeHost
	{
		/// <summary>
		/// Class to host a TreeNodeSubItem, a visual representation of a TreeNodeSubItem.
		/// </summary>
		[Description("Class to host a TreeNodeSubItem.")]
		private class TreeNodeSubItemHost
		{
			TreeNode.TreeNodeSubItem _subItem;
			TreeNodeHost _owner;
			Size _displaySize;
			Point _location;
			SizeF _originalSize;
			Rectangle _displayedRect;
			Rectangle _checkBoxRect = new Rectangle(0, 0, 14, 14);
			bool _onHover = false;
			bool _onHoverCheck = false;
			bool _isDisplayed = true;
			public TreeNodeSubItemHost(TreeNodeHost owner, TreeNode.TreeNodeSubItem subitem)
			{
				_owner = owner;
				_subItem = subitem;
			}
			/// <summary>
			/// Gets the display string of a TreeNodeSubItem value.
			/// </summary>
			[Description("Gets the display string of a TreeNodeSubItem value.")]
			public string getSubItemString()
			{
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				if (index == 0)
					return _owner._node.Text;
				return _owner._owner.getValueString(_subItem.Value, index);
			}
			/// <summary>
			/// Measure the original size of the TreeNodeSubItem value.
			/// </summary>
			[Description("Measure the original size of the TreeNodeSubItem value.")]
			public void measureOriginal()
			{
				if (!_owner._owner._performValidation)
					return;
				string strSubitem = getSubItemString();
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Font font = null;
					if (_owner._node.UseNodeStyleForSubItems) {
						font = _owner._node.NodeFont;
					} else {
						font = _subItem.Font;
					}
					_originalSize = _owner._owner._gObj.MeasureString(strSubitem, font);
					if (_originalSize.Width == 0)
						_originalSize.Width = 5;
					if (_originalSize.Height == 0)
						_originalSize.Height = font.Height;
					_originalSize.Width += MultiColumnTree._subItemMargin * 2;
					_originalSize.Width += 1;
					_originalSize.Height += MultiColumnTree._subItemMargin * 2;
					if (index == 0) {
						if (_owner._owner._showImages)
							_originalSize.Width += 20;
						if (_owner._owner._checkBoxes)
							_originalSize.Width += 20;
					}
					if (aColumn.Format == ColumnFormat.Check)
						_originalSize.Height = 14 + (2 * MultiColumnTree._subItemMargin);
				} else {
					_originalSize = new SizeF(0, 0);
				}
			}
			/// <summary>
			/// Measure the displayed size of the TreeNodeSubItem value.
			/// </summary>
			[Description("Measure the displayed size of the TreeNodeSubItem value.")]
			public void measureDisplay()
			{
				if (!_owner._owner._performValidation)
					return;
				string strSubitem = getSubItemString();
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Rectangle colRect = _owner._owner._columnControl.ColumnRectangle[aColumn];
					if (_owner._owner._columnControl.getDisplayedIndex(aColumn) == 0 & _owner._owner._checkBoxes) {
						colRect.X -= 22;
						colRect.Width += 22;
					}
					if (index == 0) {
						_displaySize = new Size(_originalSize.Width, _originalSize.Height);
						_displayedRect.X = colRect.X + (_owner._node.Level * _owner._owner._indent) + 12;
						_checkBoxRect.X = _displayedRect.X + 3;
						_owner._dropDownRect.X = _displayedRect.X - 12;
					} else {
						if (colRect.Width <= _originalSize.Width) {
							_displaySize = new Size(colRect.Width, _originalSize.Height);
						} else {
							if (aColumn.ColumnAlign != HorizontalAlignment.Left | aColumn.Format == ColumnFormat.Bar | aColumn.Format == ColumnFormat.Check) {
								_displaySize = new Size(colRect.Width, _originalSize.Height);
							} else {
								_displaySize = new Size(_originalSize.Width, _originalSize.Height);
							}
						}
						_displayedRect.X = colRect.X;
					}
					_displaySize.Height += 1;
					if (!_owner._owner._allowMultiline) {
						if (strSubitem.IndexOf(Constants.vbCr) > -1) {
							Font font = _subItem.Font;
							if (_owner._node.UseNodeStyleForSubItems)
								font = _owner.Node.NodeFont;
							_displaySize.Height = font.Height + (2 * MultiColumnTree._subItemMargin);
						}
					}
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
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Rectangle colRect = _owner._owner._columnControl.ColumnRectangle[aColumn];
					if (_owner._owner._columnControl.getDisplayedIndex(aColumn) == 0 & _owner._owner._checkBoxes) {
						colRect.X -= 22;
						colRect.Width += 22;
					}
					if (index > 0) {
						_displayedRect.X = colRect.X;
					} else {
						_displayedRect.X = colRect.X + (_owner._node.Level * _owner._owner._indent) + 12;
						_checkBoxRect.X = _displayedRect.X + 3;
						_owner._dropDownRect.X = _displayedRect.X - 12;
					}
				}
			}
			/// <summary>
			/// Determine whether mouse pointer is hover on this TreeNodeSubItem.
			/// </summary>
			[Description("Determine whether mouse pointer is hover on this TreeNodeSubItem.")]
			public bool OnHover {
				get { return _onHover; }
				set { _onHover = value; }
			}
			/// <summary>
			/// Determine whether mouse pointer is hover on the checkbox of this TreeNodeSubItem.
			/// </summary>
			public bool OnHoverCheck {
				get { return _onHoverCheck; }
				set { _onHoverCheck = value; }
			}
			/// <summary>
			/// Determine y location of the host.
			/// </summary>
			public int Y {
				get { return _displayedRect.Y; }
				set {
					_displayedRect.Y = value;
					_checkBoxRect.Y = value + ((_owner._size.Height - 14) / 2);
				}
			}
			/// <summary>
			/// Gets the original size of TreeNodeSubItem.
			/// </summary>
			[Description("Gets the original size of TreeNodeSubItem.")]
			public SizeF OriginalSize {
				get { return _originalSize; }
			}
			/// <summary>
			/// Gets the displayed size of TreeNodeSubItem.
			/// </summary>
			[Description("Gets the displayed size of TreeNodeSubItem.")]
			public Size DisplayedSize {
				get { return _displaySize; }
			}
			/// <summary>
			/// Gets the displayed rectangle of TreeNodeSubItem.
			/// </summary>
			[Description("Gets the displayed rectangle of TreeNodeSubItem.")]
			public Rectangle Bounds {
				get { return _displayedRect; }
			}
			/// <summary>
			/// Draw TreeNodeSubItem object on specified graphics object, and optionally draw its background.
			/// </summary>
			[Description("Draw TreeNodeSubItem object on specified graphics object, and optionally draw its background.")]
			public void draw(Graphics g, bool drawBackground = false, bool selected = false)
			{
				if (!_owner._owner._performValidation)
					return;
				string strSubitem = getSubItemString();
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn == null)
					return;
				if (!aColumn.Visible)
					return;
				Rectangle rect = _owner._owner._columnControl.ColumnRectangle[aColumn];
				if (_owner._owner._columnControl.getDisplayedIndex(aColumn) == 0 & _owner._owner._checkBoxes) {
					rect.X -= 22;
					rect.Width += 22;
				}
				rect.Y = _displayedRect.Y;
				rect.Height = _owner._size.Height;
				if (rect.X > _owner._owner._clientArea.Right | rect.Y > _owner._owner._clientArea.Bottom)
					return;
				if (rect.Right < _owner._owner._clientArea.X | rect.Bottom < _owner._owner._clientArea.Y)
					return;
				if (index == 0)
					g.SetClip(rect);
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
						if (_owner._node.UseNodeStyleForSubItems) {
							g.FillRectangle(new SolidBrush(_owner._node.BackColor), rect);
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
							rectCheck.X = rectValue.X + MultiColumnTree._subItemMargin;
							break;
						case HorizontalAlignment.Right:
							rectCheck.X = rectValue.Right - (MultiColumnTree._subItemMargin + 1);
							break;
					}
					if (checkValue) {
						Renderer.CheckBox.drawCheck(g, rectCheck, CheckState.Checked, , _owner._owner.Enabled);
					}
				} else {
					System.Drawing.Color color = (_owner._owner.Enabled ? (_owner._node.UseNodeStyleForSubItems ? _owner._node.Color : _subItem.Color) : System.Drawing.Color.Gray);
					Font font = (_owner._node.UseNodeStyleForSubItems ? _owner._node.NodeFont : _subItem.Font);
					StringFormat strFormat = new StringFormat();
					ColumnFormat columnFormat = (index == 0 ? Control.ColumnFormat.None : aColumn.Format);
					Rectangle rectValue = _displayedRect;
					rectValue.Height = _owner._size.Height;
					if (index == 0) {
						if (_owner._owner._checkBoxes) {
							rectValue.X += 20;
							rectValue.Width -= 20;
							if (_owner._onHover | _owner._node.CheckState != CheckState.Unchecked | _owner._selected)
								Renderer.CheckBox.drawCheckBox(g, _checkBoxRect, _owner._node.CheckState, , _owner._owner.Enabled, _onHoverCheck);
						}
						if (_owner._owner._showImages) {
							Image img = null;
							if (_owner._node.IsExpanded)
								img = _owner._node.ExpandedImage;
							if (img == null)
								img = _owner._node.Image;
							if (img != null) {
								Rectangle rectImg = new Rectangle(rectValue.Location, new Size(20, rectValue.Height));
								rectImg = Renderer.Drawing.getImageRectangle(img, rectImg, 16);
								if (_owner._owner.Enabled) {
									g.DrawImage(img, rectImg);
								} else {
									Renderer.Drawing.grayscaledImage(img, rectImg, g);
								}
							}
							rectValue.X += 20;
							rectValue.Width -= 20;
						}
						if (_owner.getVisibleChildCount() > 0) {
							if (_owner._owner.Enabled) {
								Renderer.Drawing.drawTriangle(g, _owner._dropDownRect, (_owner._onHoverDropDown ? System.Drawing.Color.Gold : System.Drawing.Color.Black), System.Drawing.Color.White, (_owner._node.IsExpanded ? Renderer.Drawing.TriangleDirection.DownRight : Renderer.Drawing.TriangleDirection.Right));
							} else {
								Renderer.Drawing.drawTriangle(g, _owner._dropDownRect, System.Drawing.Color.Gray, System.Drawing.Color.White, (_owner._node.IsExpanded ? Renderer.Drawing.TriangleDirection.DownRight : Renderer.Drawing.TriangleDirection.Right));
							}
						}
					}
					rectValue.X += MultiColumnTree._subItemMargin;
					rectValue.Width -= MultiColumnTree._subItemMargin * 2;
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
				if (index == 0)
					g.ResetClip();
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
					bool stateChanged = false;
					if (!_onHover) {
						_onHover = true;
						stateChanged = true;
					}
					if (_owner._owner._checkBoxes) {
						if (_owner._subItemHosts.IndexOf(this) == 0) {
							if (_checkBoxRect.Contains(e.Location)) {
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
					if (_onHover | _onHoverCheck) {
						_onHover = false;
						_onHoverCheck = false;
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
				if (_onHover | _onHoverCheck) {
					_onHover = false;
					_onHoverCheck = false;
					return true;
				}
				return false;
			}
			// Tooltip
			/// <summary>
			/// Determine whether a tooltip need to be shown on a TreeNodeSubItem object.
			/// </summary>
			public bool needToolTip()
			{
				int index = _owner._node.SubItems.IndexOf(_subItem) + 1;
				if (index > 0)
					return false;
				ColumnHeader aColumn = _owner._owner._columns[index];
				if (aColumn != null) {
					Rectangle colRect = _owner._owner._columnControl.ColumnRectangle[aColumn];
					if (_owner._owner._columnControl.getDisplayedIndex(aColumn) == 0 & _owner._owner._checkBoxes) {
						colRect.X -= 22;
						colRect.Width += 22;
					}
					Rectangle subItemRect = Bounds;
					return subItemRect.Right > colRect.Right | Math.Floor(_originalSize.Height) > _displaySize.Height;
				}
				return false;
			}
			/// <summary>
			/// Gets an area where tooltip must be avoid.
			/// </summary>
			public Rectangle toolTipRect()
			{
				Rectangle rect = _displayedRect;
				if (_owner._owner._showImages) {
					rect.X += 20;
					rect.Width -= 20;
				}
				if (_owner._owner._checkBoxes) {
					rect.X += 20;
					rect.Width -= 20;
				}
				return rect;
			}
		}
		MultiColumnTree _owner;
		TreeNode _node;
		Point _location;
		Size _size;
		bool _onMouseDown = false;
		bool _selected = false;
		bool _onHover = false;
		bool _visible = true;
		// Checkbox
		bool _onHoverDropDown = false;
		Rectangle _dropDownRect = new Rectangle(0, 0, 10, 10);
		// SubItem hosts
		List<TreeNodeSubItemHost> _subItemHosts = new List<TreeNodeSubItemHost>();
		// Child nodes
		Size _childSize;
		List<TreeNodeHost> _childHosts = new List<TreeNodeHost>();
		TreeNodeHost _parentHost = null;
		// Filter operation
		bool _filterPerformed = false;
		public TreeNodeHost(TreeNode node, MultiColumnTree owner, TreeNodeHost parentHost)
		{
			_owner = owner;
			_node = node;
			_parentHost = parentHost;
			// Adding item text and all its subitems to filter parameters
			int i = 0;
			while (i <= _node.SubItems.Count & i < _owner._columns.Count) {
				_owner._columnControl.addFilter(i, _node.SubItems(i).Value);
				i += 1;
			}
			if (_node.Checked)
				_owner._checkedNodes.Add(_node);
			refreshSubItem();
			refreshChildrenHosts();
			_node.NodeAdded += node_NodeAdded;
			_node.NodeRemoved += node_NodeRemoved;
			_node.NodesOnClear += node_NodeRemoved;
		}
		// Child nodes
		/// <summary>
		/// Refresh TreeNodeHost contained in this host.
		/// </summary>
		public void refreshChildrenHosts()
		{
			_childHosts.Clear();
			foreach (TreeNode tn in _node.Nodes) {
				TreeNodeHost tnHost = new TreeNodeHost(tn, _owner, this);
				_childHosts.Add(tnHost);
			}
		}
		/// <summary>
		/// Relocate all TreeNodeHost object contained in this host.
		/// </summary>
		public void relocateChildrenHosts()
		{
			int y = _location.Y + _size.Height;
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.Y = y;
				if (tnHost.Visible) {
					if (tnHost._node.IsExpanded)
						tnHost.relocateChildrenHosts();
					y = tnHost.Bottom;
				}
			}
		}
		/// <summary>
		/// Sort the children host contained in this host.
		/// </summary>
		public void sortChildren(bool relocateChild = true)
		{
			_childHosts.Sort(_owner.nodeHostComparer);
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.sortChildren(relocateChild);
			}
			if (relocateChild)
				relocateChildrenHosts();
		}
		/// <summary>
		/// Filter the children host contained in this host, based on specified filter parameters.
		/// </summary>
		public void filterChildren(List<ColumnFilterHandle> handlers, bool relocateChild = true)
		{
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.Visible = _owner.filterNode(tnHost.Node, handlers);
				if (tnHost.Visible)
					tnHost.filterChildren(handlers, relocateChild);
			}
			if (relocateChild) {
				measureChildren();
				relocateChildrenHosts();
			}
		}
		/// <summary>
		/// Filter the children host contained in this host.
		/// </summary>
		public void filterChildren(bool relocateChild = true)
		{
			List<ColumnFilterHandle> handlers = _owner._columnControl.FilterHandlers;
			filterChildren(handlers, relocateChild);
		}
		/// <summary>
		/// Removes the event handlers of the node and all of the children.
		/// </summary>
		public void removeHandlers()
		{
			_node.NodeAdded -= node_NodeAdded;
			_node.NodeRemoved -= node_NodeRemoved;
			_node.NodesOnClear -= node_NodesOnClear;
			if (_node.Checked)
				_owner._checkedNodes.Remove(_node);
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.removeHandlers();
			}
		}
		/// <summary>
		/// Collect all available TreeNodeSubItem values contained in node.
		/// </summary>
		public void collectSubItemValue(int index, List<object> values)
		{
			if (index >= 0 & index <= _node.SubItems.Count) {
				object subValue = _node.SubItems(index).Value;
				if (subValue != null) {
					if (!values.Contains(subValue))
						values.Add(subValue);
				}
			}
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.collectSubItemValue(index, values);
			}
		}
		/// <summary>
		/// Gets a TreeNodeHost object that contains specified node.
		/// </summary>
		public TreeNodeHost getHost(TreeNode node)
		{
			if (object.ReferenceEquals(node, _node))
				return this;
			TreeNodeHost foundHost = null;
			foreach (TreeNodeHost tnHost in _childHosts) {
				foundHost = tnHost.getHost(node);
				if (foundHost != null)
					return foundHost;
			}
			return null;
		}
		/// <summary>
		/// Gets the number of visible child of a TreeNode.
		/// </summary>
		public int getVisibleChildCount()
		{
			int visibleCount = 0;
			foreach (TreeNodeHost tnHost in _childHosts) {
				if (tnHost.Visible)
					visibleCount += 1;
			}
			return visibleCount;
		}
		/// <summary>
		/// Gets previous TreeNodeHost child of a TreeNodeHost, starting from specified TreeNodeHost.
		/// </summary>
		public TreeNodeHost getPrevHost(TreeNodeHost fromHost)
		{
			int hostIndex = _childHosts.IndexOf(fromHost);
			if (hostIndex == 0) {
				return this;
			} else {
				hostIndex -= 1;
				while (hostIndex >= 0) {
					if (_childHosts[hostIndex].Visible)
						return _childHosts[hostIndex];
					hostIndex += 1;
				}
			}
			return null;
		}
		/// <summary>
		/// Gets the next TreeNodeHost child of a TreeNodeHost, starting from specified TreeNodeHost.
		/// </summary>
		public TreeNodeHost getNextHost(TreeNodeHost @from)
		{
			if (getVisibleChildCount() == 0)
				return null;
			int hostIndex = _childHosts.IndexOf(@from);
			if (object.ReferenceEquals(@from, this))
				hostIndex = 0;
			if (hostIndex == -1)
				return null;
			while (hostIndex < _childHosts.Count) {
				if (_childHosts[hostIndex].Visible)
					return _childHosts[hostIndex];
				hostIndex += 1;
			}
			return null;
		}
		/// <summary>
		/// Gets the next TreeNodeHost child of a TreeNodeHost, starting from specified TreeNodeHost, and NodeText is started with specified character.
		/// </summary>
		public TreeNodeHost getNextHost(TreeNodeHost @from, string startsWith, bool startNextNode = true)
		{
			if (getVisibleChildCount() == 0)
				return null;
			int hostIndex = _childHosts.IndexOf(@from);
			if (object.ReferenceEquals(@from, this)) {
				hostIndex = 0;
				if (!startNextNode) {
					// Checking own node
					if (_node.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
						return this;
					if (getVisibleChildCount() == 0 | !_node.IsExpanded)
						return null;
				}
			}
			while (hostIndex < _childHosts.Count) {
				if (_childHosts[hostIndex].Visible) {
					if (_childHosts[hostIndex]._node.Text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase)) {
						return _childHosts[hostIndex];
					} else {
						if (_childHosts[hostIndex].getVisibleChildCount() > 0 & _childHosts[hostIndex]._node.IsExpanded) {
							TreeNodeHost result = null;
							result = _childHosts[hostIndex].getNextHost(_childHosts[hostIndex], startsWith);
							if (result != null)
								return result;
						}
					}
				}
				hostIndex += 1;
			}
			return null;
		}
		/// <summary>
		/// Gets the last visible of the children host of a TreeNodeHost.
		/// </summary>
		public TreeNodeHost getLastVisibleNode()
		{
			if (_node.IsExpanded & getVisibleChildCount() > 0) {
				int i = _childHosts.Count - 1;
				while (i >= 0) {
					if (_childHosts[i].Visible)
						return _childHosts[i].getLastVisibleNode();
					i -= 1;
				}
			}
			return this;
		}
		// Parent node and host.
		/// <summary>
		/// Gets the top level of the parent.
		/// </summary>
		public TreeNodeHost getTopParentHost()
		{
			if (_parentHost == null)
				return this;
			TreeNodeHost pHost = _parentHost;
			while (pHost != null) {
				if (pHost.ParentHost == null)
					return pHost;
				pHost = pHost.ParentHost;
			}
			return null;
		}
		/// <summary>
		/// Gets a value indicating whether node containing within the host is descendant from specified node.
		/// </summary>
		public bool isDescendantFrom(TreeNode node)
		{
			TreeNode pNode = _node.Parent;
			while (pNode != null) {
				if (object.ReferenceEquals(pNode, node))
					return true;
				pNode = pNode.Parent;
			}
			return false;
		}
		/// <summary>
		/// Gets a TreeNodeHost that contains one of the parental node.
		/// </summary>
		public TreeNodeHost getParentHost(TreeNode node)
		{
			TreeNodeHost pHost = null;
			while (pHost != null) {
				if (object.ReferenceEquals(pHost.Node, node))
					return pHost;
				pHost = pHost.ParentHost;
			}
			return null;
		}
		/// <summary>
		/// Expand all parent in node hierarchy.
		/// </summary>
		public void expandAllParent()
		{
			TreeNode pNode = _node.Parent;
			while (pNode != null) {
				pNode.Expand();
				pNode = pNode.Parent;
			}
		}
		// Sub items
		/// <summary>
		/// Refresh TreeNodeSubItemHost contained in this host.
		/// </summary>
		[Description("Refresh TreeNodeSubItemHost contained in this host.")]
		public void refreshSubItem()
		{
			_subItemHosts.Clear();
			int i = 0;
			while (i <= _node.SubItems.Count) {
				TreeNodeSubItemHost aHost = new TreeNodeSubItemHost(this, _node.SubItems(i));
				aHost.measureOriginal();
				_subItemHosts.Add(aHost);
				i = i + 1;
			}
		}
		/// <summary>
		/// Relocate all available TreeNodeSubItemHost.
		/// </summary>
		public void relocateSubItems()
		{
			foreach (TreeNodeSubItemHost lvsiHost in _subItemHosts) {
				lvsiHost.moveX();
			}
		}
		// Drawing
		/// <summary>
		/// Draw bar chart of a TreeNodeSubItem with associated column header.
		/// </summary>
		[Description("Draw bar chart of a TreeNodeSubItem with associated column header.")]
		private void drawBar(Graphics g, Rectangle rect, TreeNode.TreeNodeSubItem subItem, ColumnHeader column)
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
		/// Draw TreeNodeSubItems that associated with all unfrozen columns.
		/// </summary>
		[Description("Draw ListViewSubItems that associated with all unfrozen columns.")]
		public void drawUnFrozen(Graphics g, int frozenCount)
		{
			if (!_visible)
				return;
			if (_location.X > _owner.Width | _location.Y > _owner.Height)
				return;
			if (Bottom < 0 | Right < 0)
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
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.drawUnFrozen(g, frozenCount);
				}
			}
		}
		/// <summary>
		/// Draw TreeNodeSubItems that associated with all frozen columns.
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
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.drawFrozen(g, unfrozenCount);
				}
			}
		}
		// Item measurement
		/// <summary>
		/// Measure the size of the TreeNodeHost in defferent views.
		/// </summary>
		[Description("Measure the size of the TreeNodeHost in defferent views.")]
		public void measureSize()
		{
			int height = 0;
			foreach (TreeNodeSubItemHost siHost in _subItemHosts) {
				siHost.measureOriginal();
				siHost.measureDisplay();
				if (height < siHost.Bounds.Height)
					height = siHost.Bounds.Height;
			}
			Rectangle colsRect = _owner._columnControl.DisplayedRectangle;
			_location.X = colsRect.X;
			_size.Width = colsRect.Width;
			_size.Height = height;
			_dropDownRect.Y = _location.Y + ((height - 10) / 2);
		}
		/// <summary>
		/// Measure the size used as children's area of the host.
		/// </summary>
		public void measureChildren()
		{
			_childSize.Width = _size.Width;
			if (_node.IsExpanded) {
				int childrenHeight = 0;
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.measureSize();
					tnHost.measureChildren();
					if (tnHost.Visible)
						childrenHeight += tnHost.Bounds.Height;
				}
				_childSize.Height = childrenHeight;
			} else {
				_childSize.Height = 0;
			}
		}
		/// <summary>
		/// Gets the bounding rectangle of a TreeNodeSubItem host specified by its index.
		/// </summary>
		/// <param name="index">Index of a TreeNodeSubItem object in TreeNode.SubItems collection.</param>
		[Description("Gets the bounding rectangle of a TreeNodeSubItem host specified by its index.")]
		public Rectangle getSubItemRectangle(int index)
		{
			if (index >= 0 & index < _subItemHosts.Count) {
				return _subItemHosts[index].Bounds;
			}
			return new Rectangle(0, 0, 0, 0);
		}
		/// <summary>
		/// Gets the display string of a TreeNodeSubItem object specified by its index.
		/// </summary>
		/// <param name="index">Index of a TreeNodeSubItem object in TreeNode.SubItems collection.</param>
		[Description("Gets the display string of a TreeNodeSubItem object specified by its index.")]
		public string getSubItemString(int index)
		{
			if (index >= 0 & index < _subItemHosts.Count) {
				return _subItemHosts[index].getSubItemString();
			}
			return "";
		}
		/// <summary>
		/// Gets the size of displayed string from a TreeNodeSubItem object specified by its index.
		/// </summary>
		/// <param name="index">Index of a TreeNodeSubItem object in TreeNode.SubItems collection.</param>
		[Description("Gets the size of displayed string from a TreeNodeSubItem object specified by its index.")]
		public SizeF getSubItemSize(int index)
		{
			SizeF subSize = new SizeF(0, 0);
			if (index >= 0 & index < _subItemHosts.Count) {
				if (index == 0) {
					subSize = _subItemHosts[0].OriginalSize;
					subSize.Width += 12 + (_node.Level * _owner._indent);
				} else {
					subSize = _subItemHosts[index].OriginalSize;
				}
				if (_node.IsExpanded) {
					SizeF childSize = default(SizeF);
					foreach (TreeNodeHost tnHost in _childHosts) {
						if (tnHost.Visible) {
							childSize = tnHost.getSubItemSize(index);
							if (subSize.Width < childSize.Width)
								subSize.Width = childSize.Width;
						}
					}
				}
			}
			return subSize;
		}
		/// <summary>
		/// Gets the size of the node's text, with maximum width and height allowed.
		/// </summary>
		/// <param name="maxWidth">Maximum width allowed of the node's text size.</param>
		/// <param name="maxHeight">Maximum height allowed of the node's text size.</param>
		[Description("Gets the size of the node's text, with maximum width and height allowed.")]
		public SizeF getTextSize(int maxWidth, int maxHeight)
		{
			SizeF result = default(SizeF);
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Alignment = StringAlignment.Center;
			if (!_owner._allowMultiline)
				strFormat.FormatFlags = strFormat.FormatFlags | StringFormatFlags.LineLimit;
			result = _owner._gObj.MeasureString(_node.Text, _node.NodeFont, maxWidth, strFormat);
			if (_owner._showImages)
				result.Width += 20;
			if (result.Height > maxHeight)
				result.Height = maxHeight;
			return result;
		}
		/// <summary>
		/// Gets the bounding rectangle of the TreeNode text.
		/// </summary>
		public Rectangle getTextRectangle()
		{
			Rectangle txtRect = new Rectangle(0, 0, 0, 0);
			txtRect.Size = _subItemHosts[0].OriginalSize.ToSize();
			if (txtRect.Height == 0) {
				txtRect.Height = _node.NodeFont.Height;
				txtRect.Width = 10;
			}
			txtRect.Y = _location.Y;
			txtRect.X = _subItemHosts[0].Bounds.X + (_owner._showImages ? 20 : 0);
			return txtRect;
		}
		// Properties
		/// <summary>
		/// Determine x location of the TreeNodeHost.
		/// </summary>
		public int X {
			get { return _location.X; }
			set {
				_location.X = value;
				relocateSubItems();
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.X = _location.X;
				}
			}
		}
		/// <summary>
		/// Determine y location of the TreeNodeHost.
		/// </summary>
		public int Y {
			get { return _location.Y; }
			set {
				int dy = value - Location.Y;
				_location.Y = value;
				_dropDownRect.Y = _location.Y + ((_size.Height - 10) / 2);
				foreach (TreeNodeSubItemHost siHost in _subItemHosts) {
					siHost.Y = _location.Y;
				}
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.Y += dy;
				}
			}
		}
		/// <summary>
		/// Determine the location of the TreeNodeHost.
		/// </summary>
		[Description("Determine the locatoin of the host in ListView." + "This location can be determined by ListView itself, or by the GroupHost where this item is attached.")]
		public Point Location {
			get { return _location; }
			set {
				int dy = _location.Y - value.Y;
				_location = value;
				_dropDownRect.Y = _location.Y + ((_size.Height - 10) / 2);
				foreach (TreeNodeSubItemHost siHost in _subItemHosts) {
					siHost.Y = _location.Y;
				}
				foreach (TreeNodeHost tnHost in _childHosts) {
					tnHost.Y += dy;
				}
			}
		}
		/// <summary>
		/// Determine the visibility of the TreeNodeHost.  The visibility of a TreeNodeHost is determined by filtering operation.
		/// </summary>
		[Description("Determine the visibility of the host.")]
		public bool Visible {
			get { return _visible; }
			set {
				_visible = value;
				if (!_visible) {
					_onMouseDown = false;
					_onHover = false;
					_onHoverDropDown = false;
				}
			}
		}
		/// <summary>
		/// Determine whether the TreeNodeSubItemHost is selected.
		/// </summary>
		public bool Selected {
			get { return _selected; }
			set { _selected = value; }
		}
		/// <summary>
		/// Gets a value indicating the TreeNodeHost is visible in the client area of ListView control.
		/// </summary>
		public bool IsVisible {
			get {
				if (!_visible)
					return false;
				if (_parentHost != null) {
					if (!_parentHost.IsVisible)
						return false;
					if (!_parentHost.Node.IsExpanded)
						return false;
				}
				if (_location.X > _owner._clientArea.Right | _location.Y > _owner._clientArea.Bottom)
					return false;
				Rectangle rect = Bounds;
				if (rect.Right < _owner._clientArea.X | rect.Bottom < _owner._clientArea.Y)
					return false;
				return true;
			}
		}
		/// <summary>
		/// Gets the rightmost location of the TreeNodeHost.
		/// </summary>
		public int Right {
			get { return _location.X + _size.Width; }
		}
		/// <summary>
		/// Gets the bottommost location of the TreeNodeHost.
		/// </summary>
		public int Bottom {
			get { return _location.Y + _size.Height + _childSize.Height; }
		}
		/// <summary>
		/// Gets the bounding rectangle of the TreeNodeHost.
		/// </summary>
		[Description("Gets the bounding rectangle of the TreeNodeHost.  It can be different when the item is selected.")]
		public Rectangle Bounds {
			get {
				Rectangle rectArea = default(Rectangle);
				rectArea = new Rectangle(_location, _size);
				rectArea.Height += _childSize.Height;
				return rectArea;
			}
		}
		/// <summary>
		/// Gets the TreeNode object contained within TreeNodeHost.
		/// </summary>
		[Description("Gets TreeNode object contained in the TreeNodeHost.")]
		public TreeNode Node {
			get { return _node; }
		}
		/// <summary>
		/// Gets a value indicating the mouse is pressed on the TreeNodeHost.
		/// </summary>
		[Description("Gets a value indicating the host is pressed.")]
		public bool OnMouseDown {
			get { return _onMouseDown; }
		}
		/// <summary>
		/// Gets a value indicating the mouse pointer is moved over the TreeNodeHost.
		/// </summary>
		[Description("Gets a value indicating the mouse pointer is moved over the host.")]
		public bool OnHover {
			get { return _onHover; }
		}
		/// <summary>
		/// Gets the parent of the host.
		/// </summary>
		public TreeNodeHost ParentHost {
			get { return _parentHost; }
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
			if (Bottom < _owner._clientArea.Y | Right < _owner._clientArea.X)
				return false;
			bool stateChanged = false;
			TreeNodeSubItemHost subitemHover = null;
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
						if (subitemHover != null)
							subitemHover.OnHover = false;
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
			if (_owner._fullRowSelect) {
				Rectangle itemRect = new Rectangle(_location, _size);
				if (itemRect.Contains(e.Location) & e.X > _owner._clientArea.X & e.Y > _owner._clientArea.Y) {
					if (getVisibleChildCount() > 0) {
						if (_dropDownRect.Contains(e.Location)) {
							if (!_onHoverDropDown) {
								_onHoverDropDown = true;
								stateChanged = true;
							}
						} else {
							if (_onHoverDropDown) {
								_onHoverDropDown = false;
								stateChanged = true;
							}
						}
					}
					if (!_onHover) {
						_owner.invokeNodeMouseHover(_node);
						_onHover = true;
						if (_owner._nodeToolTip) {
							if (Renderer.ToolTip.containsToolTip(_node.ToolTipTitle, _node.ToolTip, _node.ToolTipImage)) {
								_owner._needToolTip = true;
								_owner._currentToolTip = _node.ToolTip;
								_owner._currentToolTipTitle = _node.ToolTipTitle;
								_owner._currentToolTipImage = _node.ToolTipImage;
								_owner._currentToolTipRect = itemRect;
								_owner._tooltipCaller = this;
							}
						} else {
							// Check if mouse hover on node text.
							if (subitemHover != null) {
								if (subitemHover.needToolTip() & hoverIndex == 0) {
									_owner._needToolTip = true;
									_owner._currentToolTip = subitemHover.getSubItemString();
									_owner._currentToolTipRect = subitemHover.toolTipRect();
									_owner._tooltipCaller = this;
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
			} else {
				if (subitemHover != null & hoverIndex == 0) {
					if (!_onHover) {
						_owner.invokeNodeMouseHover(_node);
						if (_owner._nodeToolTip) {
							if (Renderer.ToolTip.containsToolTip(_node.ToolTipTitle, _node.ToolTip, _node.ToolTipImage)) {
								_owner._needToolTip = true;
								_owner._currentToolTip = _node.ToolTip;
								_owner._currentToolTipTitle = _node.ToolTipTitle;
								_owner._currentToolTipImage = _node.ToolTipImage;
								_owner._currentToolTipRect = subitemHover.toolTipRect();
								_owner._tooltipCaller = this;
							}
						} else {
							if (subitemHover.needToolTip()) {
								_owner._needToolTip = true;
								_owner._currentToolTip = subitemHover.getSubItemString();
								_owner._currentToolTipRect = subitemHover.toolTipRect();
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
				if (!_onHover) {
					if (getVisibleChildCount() > 0) {
						if (_dropDownRect.Contains(e.Location)) {
							if (!_onHoverDropDown) {
								_onHoverDropDown = true;
								stateChanged = true;
							}
						} else {
							if (_onHoverDropDown) {
								_onHoverDropDown = false;
								stateChanged = true;
							}
						}
					}
				}
			}
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					if (tnHost.Visible)
						stateChanged = stateChanged | tnHost.mouseMove(e);
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the mouse is pressed over the host.
		/// </summary>
		[Description("Test whether the mouse is pressed over the host.")]
		public bool mouseDown(MouseEventArgs e)
		{
			bool stateChanged = false;
			if (_onHover) {
				if (_owner._fullRowSelect) {
					if (_onHoverDropDown & e.Button == System.Windows.Forms.MouseButtons.Left) {
						if (_node.IsExpanded) {
							_node.Collapse();
						} else {
							_node.Expand();
							if (_node.IsExpanded & !_filterPerformed) {
								foreach (ColumnHeader column in _owner._columns) {
									if (column.EnableFiltering) {
										int colIndex = _owner._columns.IndexOf[column];
										ColumnFilterHandle filterHandler = _owner._columnControl.FilterHandler[column];
										_owner.collectFilter(_node, colIndex, filterHandler);
									}
								}
								_filterPerformed = true;
							}
						}
						stateChanged = true;
					} else {
						if (_subItemHosts[0].OnHoverCheck) {
							_node.Checked = !_node.Checked;
						} else {
							if (!object.ReferenceEquals(_owner._selectedHost, this)) {
								TreeNodeEventArgs bsEvent = new TreeNodeEventArgs(_node, TreeNodeAction.Unknown);
								_owner.invokeNodeBeforeSelect(_node, bsEvent);
								if (!bsEvent.Cancel) {
									_owner.setSelectedHost(this);
									_node.Expand();
									if (_node.IsExpanded & !_filterPerformed) {
										foreach (ColumnHeader column in _owner._columns) {
											if (column.EnableFiltering) {
												int colIndex = _owner._columns.IndexOf[column];
												ColumnFilterHandle filterHandler = _owner._columnControl.FilterHandler[column];
												_owner.collectFilter(_node, colIndex, filterHandler);
											}
										}
										_filterPerformed = true;
									}
								}
								_owner.invokeNodeMouseDown(_node, e);
								if (e.Button == System.Windows.Forms.MouseButtons.Left)
									_node.Expand();
							}
							if (!_onMouseDown) {
								_onMouseDown = true;
								stateChanged = true;
							}
						}
					}
				} else {
					if (_subItemHosts[0].OnHoverCheck) {
						_node.Checked = !_node.Checked;
					} else {
						if (!object.ReferenceEquals(_owner._selectedHost, this)) {
							TreeNodeEventArgs bsEvent = new TreeNodeEventArgs(_node, TreeNodeAction.Unknown);
							_owner.invokeNodeBeforeSelect(_node, bsEvent);
							if (!bsEvent.Cancel) {
								_owner.setSelectedHost(this);
								_node.Expand();
								if (_node.IsExpanded & !_filterPerformed) {
									foreach (ColumnHeader column in _owner._columns) {
										if (column.EnableFiltering) {
											int colIndex = _owner._columns.IndexOf[column];
											ColumnFilterHandle filterHandler = _owner._columnControl.FilterHandler[column];
											_owner.collectFilter(_node, colIndex, filterHandler);
										}
									}
									_filterPerformed = true;
								}
							}
							_owner.invokeNodeMouseDown(_node, e);
							if (e.Button == System.Windows.Forms.MouseButtons.Left)
								_node.Expand();
						}
						if (!_onMouseDown) {
							_onMouseDown = true;
							stateChanged = true;
						}
					}
				}
			} else {
				if (_onMouseDown) {
					_onMouseDown = false;
					stateChanged = true;
				}
			}
			if (_onHoverDropDown & e.Button == System.Windows.Forms.MouseButtons.Left) {
				if (_node.IsExpanded) {
					_node.Collapse();
				} else {
					_node.Expand();
					if (_node.IsExpanded & !_filterPerformed) {
						foreach (ColumnHeader column in _owner._columns) {
							if (column.EnableFiltering) {
								int colIndex = _owner._columns.IndexOf[column];
								ColumnFilterHandle filterHandler = _owner._columnControl.FilterHandler[column];
								_owner.collectFilter(_node, colIndex, filterHandler);
							}
						}
						_filterPerformed = true;
					}
				}
				stateChanged = true;
			}
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					if (tnHost.Visible)
						stateChanged = stateChanged | tnHost.mouseDown(e);
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the mouse is released over the host.
		/// </summary>
		[Description("Test whether the mouse is released over the host.")]
		public bool mouseUp(MouseEventArgs e)
		{
			bool stateChanged = false;
			if (_onMouseDown) {
				_onMouseDown = false;
				_owner.invokeNodeMouseUp(_node, e);
				stateChanged = true;
			}
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					if (tnHost.Visible)
						stateChanged = stateChanged | tnHost.mouseUp(e);
				}
			}
			return stateChanged;
		}
		/// <summary>
		/// Test whether the mouse pointer is leaving the host.
		/// </summary>
		public bool mouseLeave()
		{
			bool stateChanged = false;
			foreach (TreeNodeSubItemHost tnsiHost in _subItemHosts) {
				stateChanged = stateChanged | tnsiHost.mouseLeave();
			}
			if (_onHover | _onHoverDropDown) {
				_onHover = false;
				_onHoverDropDown = false;
				stateChanged = true;
				_owner.invokeNodeMouseLeave(_node);
			}
			if (getVisibleChildCount() > 0 & _node.IsExpanded) {
				foreach (TreeNodeHost tnHost in _childHosts) {
					if (tnHost.Visible)
						stateChanged = stateChanged | tnHost.mouseLeave();
				}
			}
			return stateChanged;
		}
		// Node event handler
		/// <summary>
		/// Performs additional action when a TreeNode is added to the node.
		/// </summary>
		private void node_NodeAdded(object sender, CollectionEventArgs e)
		{
			TreeNode newNode = (TreeNode)e.Item;
			TreeNodeHost newHost = new TreeNodeHost(newNode, _owner, this);
			newHost.Visible = _owner.filterNode(newNode);
			_childHosts.Add(newHost);
			sortChildren(false);
			if (_node.IsExpanded) {
				_owner.measureAll();
				_owner.relocateAll();
				_owner.Invalidate();
			}
		}
		/// <summary>
		/// Performs additional action when a TreeNode is removed from the node.
		/// </summary>
		private void node_NodeRemoved(object sender, CollectionEventArgs e)
		{
			TreeNode remNode = (TreeNode)e.Item;
			TreeNodeHost remHost = null;
			foreach (TreeNodeHost tnHost in _childHosts) {
				if (object.ReferenceEquals(tnHost.Node, remNode)) {
					remHost = tnHost;
					break; // TODO: might not be correct. Was : Exit For
				}
			}
			if (remHost == null)
				return;
			remHost.removeHandlers();
			_childHosts.Remove(remHost);
			sortChildren(false);
			if (_node.IsExpanded) {
				_owner.measureAll();
				_owner.relocateAll();
				_owner.Invalidate();
			}
		}
		/// <summary>
		/// Performs additional action the children of the node has been cleared.
		/// </summary>
		private void node_NodesOnClear(object sender, CollectionEventArgs e)
		{
			foreach (TreeNodeHost tnHost in _childHosts) {
				tnHost.removeHandlers();
			}
			_childHosts.Clear();
			if (_node.IsExpanded) {
				_owner.measureAll();
				_owner.relocateAll();
				_owner.Invalidate();
			}
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
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible) {
				iSize = tnHost.getSubItemSize(index);
				if (maxWidth < iSize.Width)
					maxWidth = Math.Ceiling(iSize.Width);
			}
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
	/// Compares two TreeNodeHosts for equivalence.  Depending on the column sort order.
	/// </summary>
	[Description("Compares two TreeNodeHosts for equivalence.  Depending on the column sort order.")]
	private int nodeHostComparer(TreeNodeHost host1, TreeNodeHost host2)
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
					vNode1 = host1.Node.SubItems(currentColumn).Value;
					vNode2 = host2.Node.SubItems(currentColumn).Value;
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
	/// Filter a TreeNode based on existing filter parameters on each columns.
	/// </summary>
	[Description("Filter a TreeNode based on existing filter parameters on each columns.")]
	private bool filterNode(TreeNode node)
	{
		bool result = true;
		if (_performValidation) {
			List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
			int i = 0;
			while (i <= node.SubItems.Count & i < handlers.Count) {
				result = result & handlers[i].filterValue(node.SubItems(i).Value);
				i = i + 1;
			}
		}
		return result;
	}
	/// <summary>
	/// Filter a TreeNode based on specified filter parameters.
	/// </summary>
	[Description("Filter a TreeNode based on specified filter parameters.")]
	private bool filterNode(TreeNode node, List<ColumnFilterHandle> handlers)
	{
		bool result = true;
		if (_performValidation) {
			int i = 0;
			while (i <= node.SubItems.Count & i < handlers.Count) {
				result = result & handlers[i].filterValue(node.SubItems(i).Value);
				i = i + 1;
			}
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
		int indeterminateCount = 0;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Node.CheckState == CheckState.Checked) {
				checkedCount += 1;
			} else if (tnHost.Node.CheckState == CheckState.Indeterminate) {
				indeterminateCount += 1;
			}
		}
		if (checkedCount == _nodeHosts.Count) {
			_columnControl.CheckState = CheckState.Checked;
		} else {
			if (checkedCount == 0) {
				if (indeterminateCount == 0) {
					_columnControl.CheckState = CheckState.Unchecked;
				} else {
					_columnControl.CheckState = CheckState.Indeterminate;
				}
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
		_nodeHosts.Sort(nodeHostComparer);
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.sortChildren(false);
		}
	}
	/// <summary>
	/// Measure all existing hosts, and the client area used to display all hosts.
	/// </summary>
	private void measureAll()
	{
		if (!_performValidation)
			return;
		if (_nodeHosts.Count == 0) {
			_clientArea = new Rectangle(1, _columnControl.Bottom + 1, this.Width - (_vScroll.Width + 2), this.Height - (_columnControl.Bottom + 2));
			_hScroll.Left = 1;
			_hScroll.Top = this.Height - (_hScroll.Height + 1);
			_hScroll.Width = this.Width - 2;
			int columnsWidth = _columnControl.ColumnsWidth + (_checkBoxes ? 22 : 0);
			if (columnsWidth > this.Width - (_vScroll.Width + _clientArea.X)) {
				_hScroll.Visible = true;
				_hScroll.SmallChange = _clientArea.Width / 20;
				_hScroll.LargeChange = _clientArea.Width / 10;
				_hScroll.Maximum = (columnsWidth - _clientArea.Width) + _hScroll.LargeChange;
			} else {
				_hScroll.Visible = false;
			}
			return;
		}
		int itemsHeight = 0;
		int itemsWidth = _columnControl.ColumnsWidth;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.measureSize();
			tnHost.measureChildren();
		}
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible) {
				itemsHeight += tnHost.Bounds.Height + 1;
			}
		}
		_clientArea.X = 1;
		_clientArea.Y = _columnControl.Bottom + 1;
		int heightUsed = _clientArea.Y;
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
		_vScroll.Top = _clientArea.Y;
		_vScroll.Left = this.Width - (_vScroll.Width + 1);
		_vScroll.Height = _clientArea.Height;
		_hScroll.Left = 1;
		_hScroll.Top = this.Height - (_hScroll.Height + 1);
		_hScroll.Width = (_vScroll.Visible ? this.Width - (_vScroll.Width + 2) : this.Width - 2);
	}
	/// <summary>
	/// Relocate all existing hosts and scrollbars.
	/// </summary>
	private void relocateAll()
	{
		if (_nodeHosts.Count == 0)
			return;
		int x = _clientArea.X;
		int y = _clientArea.Y;
		if (_hScroll.Visible)
			x -= _hScroll.Value;
		if (_vScroll.Visible)
			y -= _vScroll.Value;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.Y = y;
			tnHost.relocateSubItems();
			tnHost.relocateChildrenHosts();
			if (tnHost.Visible)
				y = tnHost.Bounds.Bottom + 1;
		}
	}
	/// <summary>
	/// Sets the selected TreeNodeHost to a specified TreeNodeHost.
	/// </summary>
	private void setSelectedHost(TreeNodeHost host)
	{
		bool selectedHostChanged = false;
		if (_selectedHost == null)
			selectedHostChanged = true;
		if (!object.ReferenceEquals(_selectedHost, host))
			selectedHostChanged = true;
		if (_selectedHost != null)
			_selectedHost.Selected = false;
		_selectedHost = host;
		if (_selectedHost != null)
			_selectedHost.Selected = true;
		if (selectedHostChanged) {
			if (_selectedHost != null)
				if (AfterSelect != null) {
					AfterSelect(this, new TreeNodeEventArgs(_selectedHost.Node, TreeNodeAction.Unknown));
				}
			if (SelectedNodeChanged != null) {
				SelectedNodeChanged(this, new EventArgs());
			}
		}
	}
	/// <summary>
	/// Raise the NodeMouseHover event.
	/// </summary>
	private void invokeNodeMouseHover(TreeNode node)
	{
		if (NodeMouseHover != null) {
			NodeMouseHover(this, new TreeNodeEventArgs(node, TreeNodeAction.MouseHover));
		}
	}
	/// <summary>
	/// Raise the NodeMouseLeave event.
	/// </summary>
	private void invokeNodeMouseLeave(TreeNode node)
	{
		if (NodeMouseLeave != null) {
			NodeMouseLeave(this, new TreeNodeEventArgs(node, TreeNodeAction.MouseLeave));
		}
	}
	/// <summary>
	/// Raise the NodeMouseDown event.
	/// </summary>
	private void invokeNodeMouseDown(TreeNode node, MouseEventArgs e)
	{
		if (NodeMouseDown != null) {
			NodeMouseDown(this, new TreeNodeMouseEventArgs(node, TreeNodeAction.MouseDown, e));
		}
	}
	/// <summary>
	/// Raise the NodeMouseUp event.
	/// </summary>
	private void invokeNodeMouseUp(TreeNode node, MouseEventArgs e)
	{
		if (NodeMouseUp != null) {
			NodeMouseUp(this, new TreeNodeMouseEventArgs(node, TreeNodeAction.MouseDown, e));
		}
	}
	/// <summary>
	/// Raise the BeforeSelect event.
	/// </summary>
	private void invokeNodeBeforeSelect(TreeNode node, TreeNodeEventArgs e)
	{
		if (BeforeSelect != null) {
			BeforeSelect(this, e);
		}
	}
	/// <summary>
	/// Gets a string represent the value of a TreeNodeSubItem.
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
	/// Show a TextBox for label editing on a specified TreeNodeHost.
	/// </summary>
	private void showTextBoxEditor(TreeNodeHost aHost)
	{
		if (aHost == null)
			return;
		if (_columns.Count == 0)
			return;
		if (!_columns[0].Visible)
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
		_txtEditor.Text = aHost.Node.Text;
		_txtEditor.Visible = true;
		_txtEditor.SelectAll();
		_txtEditor.Focus();
	}
	/// <summary>
	/// Collapse all existing children in a node.
	/// </summary>
	private void collapseAllChild(TreeNode node)
	{
		bool changeInternalThread = false;
		// This variable is used to avoid repeat call to method measureAll, relocateAll, and Invalidate.
		if (!_internalThread) {
			_internalThread = true;
			changeInternalThread = true;
		}
		foreach (TreeNode tn in node.Nodes) {
			tn._collapse();
			if (tn.Nodes.Count > 0)
				collapseAllChild(tn);
		}
		if (changeInternalThread)
			_internalThread = false;
	}
	/// <summary>
	/// Expand all existing children in a node.
	/// </summary>
	private void expandAllChild(TreeNode node)
	{
		bool changeInternalThread = false;
		// This variable is used to avoid repeat call to method measureAll, relocateAll, and Invalidate.
		if (!_internalThread) {
			_internalThread = true;
			changeInternalThread = true;
		}
		foreach (TreeNode tn in node.Nodes) {
			tn._expand();
			if (tn.Nodes.Count > 0)
				expandAllChild(tn);
		}
		if (changeInternalThread)
			_internalThread = false;
	}
	/// <summary>
	/// Check the CheckState of a TreeNode.
	/// </summary>
	private void checkNodeCheckedState(TreeNode node)
	{
		int checkedCount = 0;
		int indeterminateCount = 0;
		foreach (TreeNode tn in node.Nodes) {
			if (tn.CheckState == CheckState.Checked)
				checkedCount += 1;
			if (tn.CheckState == CheckState.Indeterminate)
				indeterminateCount += 1;
		}
		if (checkedCount == 0 & indeterminateCount == 0) {
			node.setCheckState(CheckState.Unchecked);
		} else if (checkedCount == node.Nodes.Count) {
			node.setCheckState(CheckState.Checked);
		} else {
			node.setCheckState(CheckState.Indeterminate);
		}
		if (node.Parent != null)
			checkNodeCheckedState(node.Parent);
	}
	/// <summary>
	/// Change the checked property on children node of a TreeNode.
	/// </summary>
	private void changeChildrenChecked(TreeNode node)
	{
		foreach (TreeNode tn in node.Nodes) {
			tn.setChecked(node.Checked);
			if (tn.Nodes.Count > 0)
				changeChildrenChecked(tn);
		}
	}
	/// <summary>
	/// Gets previous visible node host from current selected node host.
	/// </summary>
	private TreeNodeHost selectPrevNodeHost()
	{
		if (_nodeHosts.Count == 0)
			return null;
		if (_selectedHost == null)
			return null;
		if (_selectedHost.ParentHost != null) {
			return _selectedHost.ParentHost.getPrevHost(_selectedHost);
		} else {
			int hostIndex = _nodeHosts.IndexOf(_selectedHost) - 1;
			while (hostIndex >= 0) {
				if (_nodeHosts[hostIndex].Visible)
					return _nodeHosts[hostIndex].getLastVisibleNode();
				hostIndex -= 1;
			}
		}
		return null;
	}
	/// <summary>
	/// Gets next visible node host from current selected node host.
	/// </summary>
	private TreeNodeHost selectNextNodeHost()
	{
		if (_nodeHosts.Count == 0)
			return null;
		if (_selectedHost == null)
			return null;
		if (_selectedHost.getVisibleChildCount() > 0 & _selectedHost.Node.IsExpanded)
			return _selectedHost.getNextHost(_selectedHost);
		TreeNodeHost fromHost = _selectedHost;
		if (_selectedHost.ParentHost != null) {
			TreeNodeHost result = null;
			TreeNodeHost pHost = _selectedHost.ParentHost;
			while (result == null & pHost != null) {
				result = pHost.getNextHost(fromHost);
				if (result != null)
					return result;
				fromHost = pHost;
				pHost = pHost.ParentHost;
			}
		}
		int i = _nodeHosts.IndexOf(fromHost) + 1;
		while (i < _nodeHosts.Count) {
			if (_nodeHosts[i].Visible)
				return _nodeHosts[i];
			i += 1;
		}
		return null;
	}
	/// <summary>
	/// Gets next visible node host from current selected node host, specified by starting charachter of the node's text.
	/// </summary>
	private TreeNodeHost selectNextNodeHost(string startsWith)
	{
		if (_nodeHosts.Count == 0)
			return null;
		TreeNodeHost result = null;
		int i = 0;
		if (_selectedHost != null) {
			if (_selectedHost.getVisibleChildCount() > 0 & _selectedHost.Node.IsExpanded) {
				result = _selectedHost.getNextHost(_selectedHost, startsWith);
				if (result != null)
					return result;
			}
			TreeNodeHost fromHost = _selectedHost;
			if (_selectedHost.ParentHost != null) {
				TreeNodeHost pHost = _selectedHost.ParentHost;
				while (result == null & pHost != null) {
					result = pHost.getNextHost(fromHost, startsWith);
					if (result != null)
						return result;
					fromHost = pHost;
					pHost = pHost.ParentHost;
				}
			}
			i = _nodeHosts.IndexOf(fromHost) + 1;
			while (i < _nodeHosts.Count) {
				if (_nodeHosts[i].Visible) {
					result = _nodeHosts[i].getNextHost(_nodeHosts[i], startsWith, false);
					if (result != null)
						return result;
				}
				i += 1;
			}
		}
		// No node host found, starting from the first node.
		i = 0;
		while (i < _nodeHosts.Count) {
			if (_nodeHosts[i].Visible) {
				result = _nodeHosts[i].getNextHost(_nodeHosts[i], startsWith, false);
				if (result != null)
					return result;
			}
			i += 1;
		}
		return null;
	}
	/// <summary>
	/// Gets the first visible node host.
	/// </summary>
	private TreeNodeHost selectFirstNodeHost()
	{
		int i = 0;
		while (i < _nodeHosts.Count) {
			if (_nodeHosts[i].Visible)
				return _nodeHosts[i];
			i += 1;
		}
		return null;
	}
	/// <summary>
	/// Gets the last visible node host.
	/// </summary>
	private TreeNodeHost selectLastNodeHost()
	{
		int i = _nodeHosts.Count - 1;
		while (i >= 0) {
			if (_nodeHosts[i].Visible)
				return _nodeHosts[i].getLastVisibleNode();
			i -= 1;
		}
		return null;
	}
	/// <summary>
	/// Ensures that the specified node is visible within the control, scrolling the contents of the control if necessary.
	/// </summary>
	private void ensureVisible(TreeNodeHost nodeHost)
	{
		if (nodeHost == null)
			return;
		if (!nodeHost.Visible)
			return;
		int dx = 0;
		int dy = 0;
		if (nodeHost.X < _clientArea.X | nodeHost.Right > _clientArea.Right) {
			if (nodeHost.X < _clientArea.X) {
				dx = nodeHost.X - _clientArea.X;
			} else {
				dx = nodeHost.Right - _clientArea.Right;
			}
		}
		if (nodeHost.Y < _clientArea.Y | nodeHost.Bottom > _clientArea.Bottom) {
			if (nodeHost.Y < _clientArea.Y) {
				dy = nodeHost.Y - _clientArea.Y;
			} else {
				dy = nodeHost.Bottom - _clientArea.Bottom;
			}
		}
		if (_vScroll.Visible | _hScroll.Visible) {
			_vScroll.Value += dy;
			_hScroll.Value += dx;
		}
	}
	/// <summary>
	/// Gets a TreeNodeHost for specified TreeNode object.
	/// </summary>
	private TreeNodeHost getNodeHost(TreeNode node)
	{
		if (node == null)
			return null;
		if (_nodeHosts.Count == 0)
			return null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible) {
				TreeNodeHost result = tnHost.getHost(node);
				if (result != null) {
					if (result.Visible) {
						return result;
					} else {
						return null;
					}
				}
			}
		}
		return null;
	}
	/// <summary>
	/// Collecting filter information on a TreeNode based on specified columnIndex.
	/// </summary>
	/// <remarks></remarks>
	private void collectFilter(TreeNode aNode, int columnIndex, ColumnFilterHandle filterHandler)
	{
		if (columnIndex >= 0 & columnIndex < _columns.Count) {
			if (columnIndex <= aNode.SubItems.Count) {
				filterHandler.addFilter(aNode.SubItems(columnIndex).Value);
			}
			if (aNode.Nodes.Count > 0 & aNode.IsExpanded) {
				foreach (TreeNode node in aNode.Nodes) {
					collectFilter(node, columnIndex, filterHandler);
				}
			}
		}
	}
	/// <summary>
	/// Collecting filter information based on specified columnIndex.
	/// </summary>
	/// <remarks></remarks>
	private void collectFilter(int columnIndex, ColumnFilterHandle filterHandler)
	{
		if (columnIndex >= 0 & columnIndex < _columns.Count) {
			_columnControl.clearFilters(columnIndex);
			if (_nodes.Count > 0) {
				foreach (TreeNode node in _nodes) {
					collectFilter(node, columnIndex, filterHandler);
				}
			}
		}
	}
	#endregion
	#region "Friend Routines."
	// Friend property, to provide design mode to the other classes.
	internal bool IsDesignMode {
		get { return DesignMode; }
	}
	// TreeNode related.
	internal void _nodeTextChanged(TreeNode node)
	{
		if (_columns.Count == 0)
			return;
		if (_columns[0].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		// Rebuilding filter
		TreeNodeHost nodeHost = null;
		TreeNodeHost foundHost = null;
		List<object> objs = new List<object>();
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null)
				nodeHost = foundHost;
			tnHost.collectSubItemValue(0, objs);
		}
		if (nodeHost == null)
			return;
		_columnControl.reloadFilter(0, objs);
		nodeHost.Visible = filterNode(node);
		sortAll();
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _nodeFontChanged(TreeNode node)
	{
		if (_columns.Count == 0)
			return;
		if (_columns[0].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null)
				break; // TODO: might not be correct. Was : Exit For
		}
		if (foundHost == null)
			return;
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _nodeBackColorChanged(TreeNode node)
	{
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null) {
				if (foundHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _nodeColorChanged(TreeNode node)
	{
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null) {
				if (foundHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _nodeImageChanged(TreeNode node)
	{
		if (!_showImages)
			return;
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null) {
				if (foundHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _nodeExpandedImageChanged(TreeNode node)
	{
		if (!_showImages)
			return;
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null) {
				if (foundHost.IsVisible)
					this.Invalidate();
				return;
			}
		}
	}
	internal void _nodeUseNodeStyleForSubItemsChanged(TreeNode node)
	{
		_nodeFontChanged(node);
	}
	internal void _nodeSubItemsChanged(TreeNode node)
	{
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null)
				break; // TODO: might not be correct. Was : Exit For
		}
		if (foundHost == null)
			return;
		foundHost.refreshSubItem();
		sortAll();
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
	}
	// TreeNode events
	internal void _nodeBeforeCheck(TreeNode node, TreeNodeEventArgs e)
	{
		if (BeforeCheck != null) {
			BeforeCheck(this, e);
		}
	}
	[Description("Fired when checked property of TreeNode has been changed.")]
	internal void _nodeChecked(TreeNode node)
	{
		if (node.Checked) {
			_checkedNodes.Add(node);
		} else {
			_checkedNodes.Remove(node);
		}
		if (AfterCheck != null) {
			AfterCheck(this, new TreeNodeEventArgs(node, TreeNodeAction.Checked));
		}
		if (node.Parent != null)
			checkNodeCheckedState(node.Parent);
		if (node.Nodes.Count > 0)
			changeChildrenChecked(node);
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(node);
			if (foundHost != null)
				break; // TODO: might not be correct. Was : Exit For
		}
		if (foundHost == null)
			return;
		if (!_internalThread) {
			changeColumnCheckState();
			this.Invalidate(true);
		}
	}
	internal void _invokeNodeChecked(TreeNode node)
	{
		if (node.Checked) {
			_checkedNodes.Add(node);
		} else {
			_checkedNodes.Remove(node);
		}
		if (AfterCheck != null) {
			AfterCheck(this, new TreeNodeEventArgs(node, TreeNodeAction.Checked));
		}
	}
	internal void _nodeBeforeCollapse(TreeNode node, TreeNodeEventArgs e)
	{
		if (BeforeCollapse != null) {
			BeforeCollapse(this, e);
		}
	}
	internal void _nodeCollapsed(TreeNode node, bool ignoreChildren = true)
	{
		if (AfterCollapse != null) {
			AfterCollapse(this, new TreeNodeEventArgs(node, TreeNodeAction.Collapse));
		}
		if (!ignoreChildren)
			collapseAllChild(node);
		if (_internalThread)
			return;
		measureAll();
		relocateAll();
		if (_selectedHost != null) {
			if (_selectedHost.isDescendantFrom(node)) {
				setSelectedHost(_selectedHost.getParentHost(node));
				ensureVisible(_selectedHost);
			}
		}
		this.Invalidate();
	}
	internal void _invokeNodeCollapsed(TreeNode node)
	{
		if (AfterCollapse != null) {
			AfterCollapse(this, new TreeNodeEventArgs(node, TreeNodeAction.Collapse));
		}
	}
	internal void _nodeBeforeExpand(TreeNode node, TreeNodeEventArgs e)
	{
		if (BeforeExpand != null) {
			BeforeExpand(this, e);
		}
	}
	internal void _nodeExpanded(TreeNode node, bool ignoreChildren = true)
	{
		if (AfterExpand != null) {
			AfterExpand(this, new TreeNodeEventArgs(node, TreeNodeAction.Expand));
		}
		if (!ignoreChildren)
			expandAllChild(node);
		if (_internalThread)
			return;
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _invokeNodeExpanded(TreeNode node)
	{
		if (AfterExpand != null) {
			AfterExpand(this, new TreeNodeEventArgs(node, TreeNodeAction.Expand));
		}
	}
	// TreeNodeSubItem related.
	internal void _subitemValueChanged(TreeNode.TreeNodeSubItem subitem)
	{
		int subItemIndex = subitem.TreeNode.SubItems.IndexOf(subitem) + 1;
		if (_columns.Count == 0 | subItemIndex >= _columns.Count)
			return;
		if (_columns[subItemIndex].SizeType == ColumnSizeType.Auto)
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		// Rebuilding filter
		TreeNodeHost nodeHost = null;
		TreeNodeHost foundHost = null;
		List<object> objs = new List<object>();
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(subitem.TreeNode);
			if (foundHost != null)
				nodeHost = foundHost;
			tnHost.collectSubItemValue(subItemIndex, objs);
		}
		if (nodeHost == null)
			return;
		_columnControl.reloadFilter(subItemIndex, objs);
		nodeHost.Visible = filterNode(subitem.TreeNode);
		sortAll();
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _subitemFontChanged(TreeNode.TreeNodeSubItem subitem)
	{
		if (subitem.TreeNode.UseNodeStyleForSubItems)
			return;
		bool reloadAll = false;
		int subItemIndex = subitem.TreeNode.SubItems.IndexOf(subitem) + 1;
		if (_columns.Count == 0 | subItemIndex >= _columns.Count)
			return;
		if (_columns[subItemIndex].SizeType == ColumnSizeType.Auto) {
			_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
			reloadAll = true;
		}
		TreeNodeHost foundHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			foundHost = tnHost.getHost(subitem.TreeNode);
			if (foundHost != null)
				break; // TODO: might not be correct. Was : Exit For
		}
		if (foundHost == null)
			return;
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	internal void _subitemBackColorChanged(TreeNode.TreeNodeSubItem subitem)
	{
		if (subitem.TreeNode.UseNodeStyleForSubItems)
			return;
		_nodeBackColorChanged(subitem.TreeNode);
	}
	internal void _subitemColorChanged(TreeNode.TreeNodeSubItem subitem)
	{
		if (subitem.TreeNode.UseNodeStyleForSubItems)
			return;
		_nodeColorChanged(subitem.TreeNode);
	}
	internal void _subitemPrintValueOnBarChanged(TreeNode.TreeNodeSubItem subitem)
	{
		_nodeColorChanged(subitem.TreeNode);
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
				this.Invalidate();
			}
		}
	}
	// Behavior
	[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets the collection of ColumnHeader that are assigned to the MultiColumnTree control")]
	public ColumnHeaderCollection Columns {
		get { return _columns; }
	}
	[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets the collection of TreeNode that are assigned to the MultiColumnTree control")]
	public TreeNodeCollection Nodes {
		get { return _nodes; }
	}
	[Category("Behavior"), Browsable(false), Description("Gets the collection of checked TreeNode that are assigned to the MultiColumnTree control")]
	public CheckedTreeNodeCollection CheckedNodes {
		get { return _checkedNodes; }
	}
	[Category("Behavior"), DefaultValue(false), Description("Determine whether the user can edit the labels of nodes in the control.")]
	public bool LabelEdit {
		get { return _labelEdit; }
		set { _labelEdit = value; }
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
	public bool NodeToolTip {
		get { return _nodeToolTip; }
		set { _nodeToolTip = value; }
	}
	// Appearance
	[Category("Appearance"), DefaultValue(false), Description("Determine a check box appears next to each node in the control.")]
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
	[Category("Appearance"), DefaultValue(false), Description("Determine whether multiline node's text and subitem's value is allowed.")]
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
	[Category("Appearance"), DefaultValue("\\"), Description("Determine the delimiter string that the tree node path uses.")]
	public string PathSeparator {
		get { return _pathSeparator; }
		set { _pathSeparator = value; }
	}
	[Category("Appearance"), DefaultValue(true), Description("Determine whether Images are shown on each TreeNode.")]
	public bool ShowImages {
		get { return _showImages; }
		set {
			if (_showImages != value) {
				_showImages = value;
				measureAll();
				relocateAll();
				this.Invalidate();
			}
		}
	}
	[Category("Appearance"), DefaultValue(20), Description("Determine the distance to indent each of the child tree node levels.")]
	public int Indent {
		get { return _showImages; }
		set {
			if (_indent != value & value > _mininumIndent) {
				_indent = value;
				measureAll();
				relocateAll();
				this.Invalidate();
			}
		}
	}
	[Browsable(false), Description("Gets or sets the tree node that is currently selected in the tree view control.")]
	public TreeNode SelectedNode {
		get {
			if (_selectedHost == null) {
				return null;
			} else {
				return _selectedHost.Node;
			}
		}
		set {
			if (_selectedHost != null) {
				if (object.ReferenceEquals(_selectedHost.Node, value))
					return;
			}
			TreeNodeHost foundHost = getNodeHost(value);
			if (foundHost != null) {
				TreeNodeEventArgs beforeSelect = new TreeNodeEventArgs(foundHost.Node, TreeNodeAction.Unknown);
				if (BeforeSelect != null) {
					BeforeSelect(this, beforeSelect);
				}
				if (beforeSelect.Cancel)
					return;
				foundHost.expandAllParent();
				setSelectedHost(foundHost);
				ensureVisible(_selectedHost);
				this.Invalidate();
			} else {
				if (value == null) {
					setSelectedHost(null);
					this.Invalidate();
				}
			}
		}
	}
	#endregion
	#region "Constructor."
	public MultiColumnTree()
	{
		Resize += MultiColumnTree_Resize;
		Paint += MultiColumnTree_Paint;
		MouseWheel += MultiColumnTree_MouseWheel;
		MouseUp += MultiColumnTree_MouseUp;
		MouseMove += MultiColumnTree_MouseMove;
		MouseLeave += MultiColumnTree_MouseLeave;
		MouseDown += MultiColumnTree_MouseDown;
		LostFocus += MultiColumnTree_LostFocus;
		KeyPress += MultiColumnTree_KeyPress;
		KeyDown += MultiColumnTree_KeyDown;
		GotFocus += MultiColumnTree_GotFocus;
		FontChanged += MultiColumnTree_FontChanged;
		EnabledChanged += MultiColumnTree_EnabledChanged;
		Disposed += MultiColumnTree_Disposed;
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
		_columns = new ColumnHeaderCollection(this);
		_columnControl = new ColumnHeaderControl(this, Renderer.ToolTip.TextFont);
		_nodes = new TreeNodeCollection(null, this);
		_checkedNodes = new CheckedTreeNodeCollection(this);
		_txtEditor = new TextBoxLabelEditor();
		_txtEditor.Visible = false;
		// Setting up graphics object for text measurement.
		_gObj.SmoothingMode = SmoothingMode.AntiAlias;
		_gObj.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
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
		switch (keyData) {
			case Keys.Up:
			case Keys.Down:
			case Keys.Space:
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.Home:
			case Keys.End:
			case Keys.F2:
			case Keys.Left:
			case Keys.Right:
				return true;
			default:
				return base.IsInputKey(keyData);
		}
	}
	#endregion
	#region "Public Routines."
	/// <summary>
	/// Suspend collecting information for filtering.
	/// </summary>
	/// <remarks></remarks>
	[Description("Suspend validation process for all process.")]
	public void suspendValidation()
	{
		_performValidation = false;
	}
	/// <summary>
	/// Resume collecting information for filtering.
	/// </summary>
	/// <remarks></remarks>
	[Description("Resume suspended validation process.")]
	public void resumeValidation()
	{
		_performValidation = true;
		foreach (ColumnHeader column in _columns) {
			if (column.EnableFiltering) {
				int colIndex = _columns.IndexOf[column];
				ColumnFilterHandle filterHandler = _columnControl.FilterHandler[column];
				collectFilter(colIndex, filterHandler);
			}
		}
		_columnControl.relocateHosts();
		measureAll();
		_columnControl.moveColumns((_hScroll.Visible ? -_hScroll.Value : 0));
		relocateAll();
		this.Invalidate(true);
	}
	/// <summary>
	/// Collapses all the tree nodes.
	/// </summary>
	public void collapseAll()
	{
		_internalThread = true;
		foreach (TreeNode tn in _nodes) {
			tn.Collapse(false);
		}
		_internalThread = false;
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	/// <summary>
	/// Expands all the tree nodes.
	/// </summary>
	public void expandAll()
	{
		_internalThread = true;
		foreach (TreeNode tn in _nodes) {
			tn.ExpandAll();
		}
		_internalThread = false;
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	/// <summary>
	/// Retrieves the number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.
	/// </summary>
	public int getNodesCount(bool includeSubTrees)
	{
		int result = _nodes.Count;
		if (includeSubTrees) {
			foreach (TreeNode tn in _nodes) {
				result += tn.GetNodeCount(includeSubTrees);
			}
		}
		return result;
	}
	/// <summary>
	/// Retrieves the bounding rectangle of a TreeNode in MultiColumnTree control.
	/// </summary>
	public Rectangle getNodeRectangle(TreeNode node)
	{
		Rectangle result = new Rectangle(0, 0, 0, 0);
		if (node != null) {
			TreeNodeHost tnHost = getNodeHost(node);
			if (tnHost != null)
				result = tnHost.Bounds;
		}
		return result;
	}
	#endregion
	#region "Event Handlers."
	private void _nodes_AfterClear(object sender, CollectionEventArgs e)
	{
		setSelectedHost(null);
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.removeHandlers();
		}
		_nodeHosts.Clear();
		_columnControl.clearFilters();
		_vScroll.Visible = false;
		_clientArea = new Rectangle(1, _columnControl.Bottom + 1, this.Width - 2, this.Height - (_columnControl.Bottom + 1 + (_hScroll.Visible ? _hScroll.Height + 1 : 0)));
		this.Invalidate(true);
	}
	private void _nodes_AfterInsert(object sender, CollectionEventArgs e)
	{
		TreeNode newNode = (TreeNode)e.Item;
		TreeNodeHost newHost = new TreeNodeHost(newNode, this, null);
		_nodeHosts.Add(newHost);
		newHost.Visible = filterNode(newNode);
		sortAll();
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
		this.Invalidate(true);
	}
	private void _nodes_AfterRemove(object sender, CollectionEventArgs e)
	{
		TreeNode remNode = (TreeNode)e.Item;
		TreeNodeHost remHost = null;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (object.ReferenceEquals(tnHost.Node, remNode)) {
				remHost = tnHost;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		if (remHost == null)
			return;
		remHost.removeHandlers();
		if (object.ReferenceEquals(remHost, _selectedHost))
			setSelectedHost(null);
		_columnControl.relocateHosts((_hScroll.Visible ? -_hScroll.Value : 0));
		measureAll();
		relocateAll();
		this.Invalidate(true);
	}
	private void _hScroll_ValueChanged(object sender, System.EventArgs e)
	{
		if (!_hScroll.Visible)
			return;
		_columnControl.moveColumns(-_hScroll.Value);
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.X = _clientArea.X - _hScroll.Value;
		}
		this.Invalidate(true);
	}
	private void _vScroll_ValueChanged(object sender, System.EventArgs e)
	{
		if (!_vScroll.Visible)
			return;
		if (_nodeHosts.Count == 0)
			return;
		int lastY = _nodeHosts[0].Y;
		int dy = (_clientArea.Y - _vScroll.Value) - lastY;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.Y += dy;
		}
		if (_txtEditor.Visible)
			_txtEditor.Top += dy;
		this.Invalidate();
	}
	private void _columnControl_AfterColumnCustomFilter(object sender, ColumnEventArgs e)
	{
		ColumnCustomFilterEventArgs cEvent = new ColumnCustomFilterEventArgs(e.Column);
		if (ColumnCustomFilter != null) {
			ColumnCustomFilter(this, cEvent);
		}
		if (!cEvent.CancelFilter) {
			List<ColumnFilterHandle> handlers = _columnControl.FilterHandlers;
			foreach (TreeNodeHost tnHost in _nodeHosts) {
				tnHost.Visible = filterNode(tnHost.Node, handlers);
				if (tnHost.Visible)
					tnHost.filterChildren(handlers, false);
			}
			if (_selectedHost != null) {
				if (!_selectedHost.Visible)
					setSelectedHost(null);
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
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.Visible = filterNode(tnHost.Node, handlers);
			if (tnHost.Visible)
				tnHost.filterChildren(handlers, false);
		}
		if (_selectedHost != null) {
			if (!_selectedHost.Visible)
				setSelectedHost(null);
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
		foreach (TreeNode tn in _nodes) {
			tn.Checked = _columnControl.CheckState == CheckState.Checked;
			//changeChildrenChecked(tn)
		}
		this.Invalidate();
		_internalThread = false;
	}
	private void _columnControl_ColumnOrderChanged(object sender, ColumnEventArgs e)
	{
		measureAll();
		relocateAll();
		this.Invalidate();
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
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void _columns_AfterRemove(object sender, CollectionEventArgs e)
	{
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void _columns_ColumnFilterChanged(object sender, System.EventArgs e)
	{
		try {
			ColumnHeader column = (ColumnHeader)sender;
			if (column.EnableFiltering & _performValidation) {
				int colIndex = _columns.IndexOf[column];
				ColumnFilterHandle filterHandler = _columnControl.FilterHandler[column];
				collectFilter(colIndex, filterHandler);
			}
		} catch (Exception ex) {
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
						if (_currentEditedHost.Node.Text != _txtEditor.Text) {
							_currentEditedHost.Node.Text = _txtEditor.Text;
							if (AfterLabelEdit != null) {
								AfterLabelEdit(this, new TreeNodeEventArgs(_currentEditedHost.Node, TreeNodeAction.LabelEdit));
							}
						}
					}
					this.Focus();
				}
				break;
			case Keys.Control | Keys.Return:
				if (_txtEditor.Multiline) {
					if (_currentEditedHost != null) {
						if (_currentEditedHost.Node.Text != _txtEditor.Text) {
							_currentEditedHost.Node.Text = _txtEditor.Text;
							if (AfterLabelEdit != null) {
								AfterLabelEdit(this, new TreeNodeEventArgs(_currentEditedHost.Node, TreeNodeAction.LabelEdit));
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
	private void MultiColumnTree_Disposed(object sender, System.EventArgs e)
	{
		if (_gObj != null)
			_gObj.Dispose();
		if (_gBmp != null)
			_gBmp.Dispose();
	}
	private void MultiColumnTree_EnabledChanged(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void MultiColumnTree_FontChanged(object sender, System.EventArgs e)
	{
		measureAll();
		relocateAll();
		this.Invalidate();
	}
	private void MultiColumnTree_GotFocus(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void MultiColumnTree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		switch (e.KeyCode) {
			case Keys.Up:
				TreeNodeHost upperHost = null;
				if (_selectedHost != null) {
					upperHost = selectPrevNodeHost();
				} else {
					upperHost = selectFirstNodeHost();
				}
				if (upperHost != null) {
					TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(upperHost.Node, TreeNodeAction.Unknown);
					if (BeforeSelect != null) {
						BeforeSelect(this, bfrSelect);
					}
					if (bfrSelect.Cancel)
						return;
					setSelectedHost(upperHost);
					ensureVisible(_selectedHost);
					this.Invalidate();
				}
				break;
			case Keys.Down:
				TreeNodeHost lowerHost = null;
				if (_selectedHost != null) {
					lowerHost = selectNextNodeHost();
				} else {
					lowerHost = selectFirstNodeHost();
				}
				if (lowerHost != null) {
					TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(lowerHost.Node, TreeNodeAction.Unknown);
					if (BeforeSelect != null) {
						BeforeSelect(this, bfrSelect);
					}
					if (bfrSelect.Cancel)
						return;
					setSelectedHost(lowerHost);
					ensureVisible(_selectedHost);
					this.Invalidate();
				}
				break;
			case Keys.Left:
				if (_selectedHost != null) {
					if (_selectedHost.getVisibleChildCount() > 0 & _selectedHost.Node.IsExpanded) {
						_selectedHost.Node.Collapse();
					} else {
						if (_selectedHost.ParentHost != null) {
							TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(_selectedHost.ParentHost.Node, TreeNodeAction.Unknown);
							if (BeforeSelect != null) {
								BeforeSelect(this, bfrSelect);
							}
							if (bfrSelect.Cancel)
								return;
							setSelectedHost(_selectedHost.ParentHost);
							ensureVisible(_selectedHost);
							this.Invalidate();
						}
					}
				} else {
					TreeNodeHost firstHost = selectFirstNodeHost();
					if (firstHost != null) {
						TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(firstHost.Node, TreeNodeAction.Unknown);
						if (BeforeSelect != null) {
							BeforeSelect(this, bfrSelect);
						}
						if (bfrSelect.Cancel)
							return;
						setSelectedHost(firstHost);
						if (_selectedHost != null) {
							ensureVisible(_selectedHost);
							this.Invalidate();
						}
					}
				}
				break;
			case Keys.Right:
				if (_selectedHost != null) {
					if (_selectedHost.getVisibleChildCount() > 0 & !_selectedHost.Node.IsExpanded) {
						_selectedHost.Node.Expand();
					} else {
						if (_selectedHost.getVisibleChildCount() > 0) {
							TreeNodeHost nextHost = _selectedHost.getNextHost(_selectedHost);
							TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(nextHost.Node, TreeNodeAction.Unknown);
							if (BeforeSelect != null) {
								BeforeSelect(this, bfrSelect);
							}
							if (bfrSelect.Cancel)
								return;
							setSelectedHost(nextHost);
							ensureVisible(_selectedHost);
							this.Invalidate();
						}
					}
				} else {
					TreeNodeHost firstHost = selectFirstNodeHost();
					if (firstHost != null) {
						TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(firstHost.Node, TreeNodeAction.Unknown);
						if (BeforeSelect != null) {
							BeforeSelect(this, bfrSelect);
						}
						if (bfrSelect.Cancel)
							return;
						setSelectedHost(firstHost);
						if (_selectedHost != null) {
							ensureVisible(_selectedHost);
							this.Invalidate();
						}
					}
				}
				break;
			case Keys.Home:
				TreeNodeHost firstHost = selectFirstNodeHost();
				if (firstHost != null) {
					TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(firstHost.Node, TreeNodeAction.Unknown);
					if (BeforeSelect != null) {
						BeforeSelect(this, bfrSelect);
					}
					if (bfrSelect.Cancel)
						return;
					setSelectedHost(firstHost);
					if (_selectedHost != null) {
						ensureVisible(_selectedHost);
						this.Invalidate();
					}
				}
				break;
			case Keys.End:
				TreeNodeHost lastHost = selectFirstNodeHost();
				if (lastHost != null) {
					TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(lastHost.Node, TreeNodeAction.Unknown);
					if (BeforeSelect != null) {
						BeforeSelect(this, bfrSelect);
					}
					if (bfrSelect.Cancel)
						return;
					setSelectedHost(lastHost);
					if (_selectedHost != null) {
						ensureVisible(_selectedHost);
						this.Invalidate();
					}
				}
				break;
			case Keys.F2:
				if (!_labelEdit)
					return;
				if (_selectedHost == null)
					return;
				TreeNodeEventArgs lblEditEvent = new TreeNodeEventArgs(_selectedHost.Node, TreeNodeAction.LabelEdit);
				if (BeforeLabelEdit != null) {
					BeforeLabelEdit(this, lblEditEvent);
				}

				if (lblEditEvent.Cancel)
					return;
				ensureVisible(_selectedHost);
				this.Invalidate();
				showTextBoxEditor(_selectedHost);
				break;
		}
	}
	private void MultiColumnTree_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
	{
		TreeNodeHost nextHost = selectNextNodeHost(e.KeyChar.ToString());
		if (nextHost != null & !object.ReferenceEquals(nextHost, _selectedHost)) {
			TreeNodeEventArgs bfrSelect = new TreeNodeEventArgs(nextHost.Node, TreeNodeAction.Unknown);
			if (BeforeSelect != null) {
				BeforeSelect(this, bfrSelect);
			}
			if (bfrSelect.Cancel)
				return;
			setSelectedHost(nextHost);
			ensureVisible(_selectedHost);
			this.Invalidate();
		}
	}
	private void MultiColumnTree_LostFocus(object sender, System.EventArgs e)
	{
		this.Invalidate();
	}
	private void MultiColumnTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		_tooltip.Hide();
		this.Focus();
		bool stateChanged = false;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible)
				stateChanged = stateChanged | tnHost.mouseDown(e);
		}
		if (stateChanged)
			this.Invalidate();
	}
	private void MultiColumnTree_MouseLeave(object sender, System.EventArgs e)
	{
		_tooltip.Hide();
		bool stateChanged = false;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible)
				stateChanged = stateChanged | tnHost.mouseLeave();
		}
		if (stateChanged)
			this.Invalidate();
	}
	private void MultiColumnTree_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		bool stateChanged = false;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible)
				stateChanged = stateChanged | tnHost.mouseMove(e);
		}
		if (stateChanged)
			this.Invalidate();
		if (_needToolTip) {
			_tooltip.Show(this, _currentToolTipRect);
			_needToolTip = false;
		}
	}
	private void MultiColumnTree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		bool stateChanged = false;
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			if (tnHost.Visible)
				stateChanged = stateChanged | tnHost.mouseUp(e);
		}
		if (stateChanged)
			this.Invalidate();
	}
	private void MultiColumnTree_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
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
			}
		}
	}
	private void MultiColumnTree_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		e.Graphics.Clear(Color.White);
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
		foreach (TreeNodeHost tnHost in _nodeHosts) {
			tnHost.drawUnFrozen(e.Graphics, frozenCols.Count);
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
			foreach (TreeNodeHost tnHost in _nodeHosts) {
				tnHost.drawFrozen(e.Graphics, unfrozenCols.Count);
			}
		}
		linePen.Dispose();
		lineBrush.Dispose();
		if (_hScroll.Visible & _vScroll.Visible) {
			Rectangle aRect = new Rectangle(_vScroll.Left - 1, _hScroll.Top - 1, _vScroll.Width + 1, _hScroll.Height + 1);
			e.Graphics.FillRectangle(Brushes.Gainsboro, aRect);
		}
		e.Graphics.DrawRectangle(Pens.LightGray, 0, 0, this.Width - 1, this.Height - 1);
		if (_columnControl.OnColumnResize)
			e.Graphics.DrawLine(Pens.Black, _columnControl.ResizeCurrentX, 0, _columnControl.ResizeCurrentX, this.Height);
	}
	private void MultiColumnTree_Resize(object sender, System.EventArgs e)
	{
		_columnControl.relocateHosts();
		measureAll();
		_columnControl.moveColumns((_hScroll.Visible ? -_hScroll.Value : 0));
		relocateAll();
		this.Invalidate(true);
	}
	#endregion
}