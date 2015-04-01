// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;

namespace Ai.Control {
	/// <summary>
	/// Class to represent an Node in the MultiColumnTree.
	/// </summary>
	[DefaultProperty("Text")]
	public class TreeNode {
		#region Internal Events
		/// <summary>
		/// Occurs when a child node is added to a TreeNode object.
		/// </summary>
		internal event EventHandler<CollectionEventArgs> NodeAdded;
		/// <summary>
		/// Occurs when a child node is removed from a TreeNode object.
		/// </summary>
		internal event EventHandler<CollectionEventArgs> NodeRemoved;
		/// <summary>
		/// Occurs when clearing the children of a TreeNode.
		/// </summary>
		internal event EventHandler<CollectionEventArgs> NodesOnClear;
		#endregion
		#region Public Classes
		/// <summary>
		/// Class to represent a SubItem of a Node in the MultiColumnTree.
		/// </summary>
		[DefaultProperty("Value")]
		public class TreeNodeSubItem {
			#region Declarations
			internal TreeNode _owner = null;
			internal Font _font = null;
			object _value = null;
			object _tag = null;
			System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
			System.Drawing.Color _color = System.Drawing.Color.Black;
			string _name = "TreeNodeSubItem";
			bool _printValueOnBar = false;
			#endregion
			#region Constructor
			/// <summary>
			/// Create an instance of TreeNodeSubItem.
			/// </summary>
			public TreeNodeSubItem() {
				_owner = new TreeNode();
	            _font = _owner.NodeFont;
			}
			/// <summary>
			/// Create an instance of the TreeNodeSubItem with specified TreeNode as its owner.
			/// </summary>
			public TreeNodeSubItem(TreeNode owner) {
				_owner = owner;
				_font = _owner.NodeFont;
			}
			#endregion
			#region Public Properties
			/// <summary>
			/// Determine the name of the TreeNodeSubItem Object.
			/// </summary>
			[DefaultValue("TreeNodeSubItem"), Description("Determine the name of the TreeNodeSubItem Object.")]
			public string Name {
				get { return _name; }
				set { _name = value; }
			}
			/// <summary>
			/// Determine an Object data associated with the TreeNodeSubItem.
			/// </summary>
			[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
				Description("Determine an Object data associated with the TreeNodeSubItem.")]
			public object Tag {
				get { return _tag; }
				set { _tag = value; }
			}
			/// <summary>
			/// Determine the value of the TreeNodeSubItem.
			/// </summary>
			[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
				Description("Determine the value of the TreeNodeSubItem.")]
			public object Value {
				get { return _value; }
				set {
					if (_value != value) {
						_value = value;
						_owner._owner._subitemValueChanged(this);
					}
				}
			}
			/// <summary>
			/// Determine a Font object to draw the value of the TreeNodeSubItem.
			/// </summary>
			[Description("Determine a Font object to draw the value of the TreeNodeSubItem.")]
			public System.Drawing.Font Font {
				get {
					if (_font != null) return _font;
					else return _owner.NodeFont;
				}
				set {
					if (_font != value) {
						_font = value;
						_owner._owner._subitemFontChanged(this);
					}
				}
			}
			/// <summary>
			/// Determine a color used for TreeNodeSubItem background.
			/// </summary>
			[DefaultValue(typeof(System.Drawing.Color), "Transparent"), 
				Description("Determine a color used for TreeNodeSubItem background.")]
			public System.Drawing.Color BackColor {
				get { return _backColor; }
				set {
					if (_backColor != value) {
						_backColor = value;
						_owner._owner._subitemBackColorChanged(this);
					}
				}
			}
			/// <summary>
			/// Determine the color used to draw the value of the TreeNodeSubItem.
			/// </summary>
			[DefaultValue(typeof(System.Drawing.Color), "Black"), 
				Description("Determine the color used to draw the value of the TreeNodeSubItem.")]
			public System.Drawing.Color Color {
				get { return _color; }
				set {
					if (_color != value) {
						_color = value;
						_owner._owner._subitemColorChanged(this);
					}
				}
			}
			[Browsable(false)]
			public TreeNode TreeNode { get { return _owner; } }
			[Browsable(false)]
			public MultiColumnTree MultiColumnTree { get { return _owner._owner; } }
			/// <summary>
			/// Draw subitem value when column format is Bar
			/// </summary>
			[DefaultValue(false), 
				Description("Draw subitem value when column format is Bar")]
			public bool PrintValueOnBar {
				get { return _printValueOnBar; }
				set {
					if (_printValueOnBar != value) {
						_printValueOnBar = value;
						_owner._owner._subitemPrintValueOnBarChanged(this);
					}
				}
			}
			#endregion
		}
		/// <summary>
		/// Class to represent a Collection of TreeNodeSubItem object.
		/// </summary>
		public class TreeNodeSubItemCollection : CollectionBase {
			TreeNode _owner;
			/// <summary>
			/// Create an instance of TreeNodeSubItemCollection with a TreeNode as its owner.
			/// </summary>
			public TreeNodeSubItemCollection(TreeNode owner) : base() { _owner = owner; }
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
			/// Gets a TreeNodeSubItem object in the collection specified by its index.
			/// </summary>
			[Description("Gets a TreeNodeSubItem object in the collection specified by its index.")]
			public TreeNodeSubItem this[int index] {
				get {
					if (index == 0) {
						TreeNodeSubItem aSubItem = new TreeNodeSubItem();
						aSubItem.Value = _owner._text;
						aSubItem.Font = _owner.NodeFont;
						aSubItem.Color = _owner.Color;
						aSubItem.BackColor = _owner.BackColor;
						return aSubItem;
					} else if (index >= 1 && index <= List.Count) {
						return (TreeNodeSubItem)List[index - 1];
					}
					return null;
				}
			}
			/// <summary>
			/// Gets the index of a TreeNodeSubItem object in the collection.
			/// </summary>
			[Description("Gets the index of a TreeNodeSubItem object in the collection.")]
			public int IndexOf(TreeNodeSubItem item) { return List.IndexOf(item); }
			/// <summary>
			/// Add a TreeNodeSubItem object to the collection.
			/// </summary>
			[Description("Add a TreeNodeSubItem object to the collection.")]
			public TreeNodeSubItem Add(TreeNodeSubItem item) {
				item._owner = _owner;
				int index = List.Add(item);
				return (TreeNodeSubItem)List[index];
			}
			/// <summary>
			/// Add a TreeNodeSubItem object to the collection by providing its value.
			/// </summary>
			[Description("Add a TreeNodeSubItem object to the collection by providing its value.")]
			public TreeNodeSubItem Add(object value) {
				TreeNodeSubItem anItem = new TreeNodeSubItem(_owner);
				anItem.Value = value;
				return this.Add(anItem);
			}
			/// <summary>
			/// Add a TreeNodeSubItem collection to the collection.
			/// </summary>
			[Description("Add a TreeNodeSubItem collection to the collection."),
				System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
			public void AddRange(TreeNodeSubItemCollection items) {
				foreach (TreeNodeSubItem subItem in items) this.Add(subItem);
			}
			/// <summary>
			/// Insert a TreeNodeSubItem object to the collection at specified index.
			/// </summary>
			[Description("Insert a TreeNodeSubItem object to the collection at specified index.")]
			public void Insert(int index, TreeNodeSubItem item) {
				item._owner = _owner;
				List.Insert(index, item);
			}
			/// <summary>
			/// Remove a TreeNodeSubItem object from the collection.
			/// </summary>
			[Description("Remove a TreeNodeSubItem object from the collection.")]
			public void Remove(TreeNodeSubItem item) {
				if (List.Contains(item)) List.Remove(item);
			}
			/// <summary>
			/// Determine whether a TreeNodeSubItem object exist in the collection.
			/// </summary>
			[Description("Determine whether a TreeNodeSubItem object exist in the collection.")]
			public bool Contains(TreeNodeSubItem item) { return List.Contains(item); }
			private bool conatinsName(string name) {
				foreach (TreeNodeSubItem si in List) {
					if (string.Compare(si.Name, name, true) == 0) return true;
				}
				return false;
			}
			/// <summary>
			/// Performs additional custom processes when validating a value.
			/// </summary>
			[Description("Performs additional custom processes when validating a value.")]
			protected override void OnValidate(object value) {
				if (!typeof(TreeNodeSubItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must TreeNodeSubItem", "value");
			}
            protected override void OnClear() { if (Clearing != null)Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
            protected override void OnClearComplete() { if (AfterClear != null)AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
			protected override void OnInsert(int index, object value) {
				if (_owner._owner.IsDesignMode) {
					TreeNodeSubItem aSubItem = (TreeNodeSubItem)value;
					aSubItem.Font = _owner._owner.Font;
				}
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
		#endregion
		#region Declarations
		internal MultiColumnTree _owner = null;
		internal Font _font = null;
		internal TreeNode _parent = null;
		string _text = "TreeNode";
		string _name = "TreeNode";
		System.Drawing.Color _color = System.Drawing.Color.Black;
		System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
		bool _checked = false;
		System.Drawing.Image _image = null;
		System.Drawing.Image _expandedImage = null;
		object _tag = null;
		string _tooltip = "";
		string _tooltipTitle = "";
		Image _tooltipImage = null;
		bool _useNodeStyleForSubItems = true;
		bool _isExpanded = false;
		System.Windows.Forms.CheckState _checkState = System.Windows.Forms.CheckState.Unchecked;
		TreeNodeSubItemCollection _subItems;
		TreeNodeCollection _nodes;
		#endregion
		#region Constructor
		/// <summary>
		/// Create an instance of TreeNode.
		/// </summary>
		[Description("Create an instance of TreeNode.")]
		public TreeNode() {
			_owner = new MultiColumnTree();
			_subItems = new TreeNodeSubItemCollection(this);
			_font = _owner.Font;
			_nodes = new TreeNodeCollection(this, _owner);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Create an Instance of TreeNode with specified MultiColumnTree as its owner.
		/// </summary>
		[Description("Create an Instance of TreeNode with specified MultiColumnTree as its owner.")]
		public TreeNode(MultiColumnTree owner) {
			_owner = owner;
			_subItems = new TreeNodeSubItemCollection(this);
			_font = _owner.Font;
			_nodes = new TreeNodeCollection(this, _owner);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Initializes a new instance of the TreeNode class with the specified label text.
		/// </summary>
		[Description("Initializes a new instance of the TreeNode class with the specified label text.")]
		public TreeNode(string text) {
			_text = text;
			_owner = new MultiColumnTree();
			_parent = null;
			_nodes = new TreeNodeCollection(this, _owner);
			_subItems = new TreeNodeSubItemCollection(this);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Initializes a new instance of the TreeNode class with the specified label text and child tree nodes.
		/// </summary>
		[Description("Initializes a new instance of the TreeNode class with the specified label text and child tree nodes.")]
		public TreeNode(string text, TreeNodeCollection nodes) {
			_text = text;
			_owner = new MultiColumnTree();
			_parent = null;
			_nodes = nodes;
			_subItems = new TreeNodeSubItemCollection(this);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Initializes a new instance of the TreeNode class with the specified label text and images to display when the tree node is in a expanded and collapsed state.
		/// </summary>
		[Description("Initializes a new instance of the TreeNode class with the specified label text and images to display when the tree node is in a expanded and collapsed state.")]
		public TreeNode(string text, Image img, Image expImg) {
			_text = text;
			_image = img;
			_expandedImage = expImg;
			_owner = new MultiColumnTree();
			_parent = null;
			_nodes = new TreeNodeCollection(this, _owner);
			_subItems = new TreeNodeSubItemCollection(this);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Initializes a new instance of the TreeNode class with the specified label text, child tree nodes, and images to display when the tree node is in a expanded and collapsed state.
		/// </summary>
		[Description("Initializes a new instance of the TreeNode class with the specified label text, child tree nodes, and images to display when the tree node is in a expanded and collapsed state.")]
		public TreeNode(string text, Image img, Image expImg, TreeNodeCollection nodes) {
			_text = text;
			_image = img;
			_expandedImage = expImg;
			_owner = new MultiColumnTree();
			_parent = null;
			_nodes = nodes;
			_subItems = new TreeNodeSubItemCollection(this);
            _nodes.AfterClear += _nodes_AfterClear;
            _nodes.AfterInsert += _nodes_AfterInsert;
            _nodes.AfterRemove += _nodes_AfterRemove;
            _subItems.AfterClear += _subItems_AfterClear;
            _subItems.AfterInsert += _subItems_AfterInsert;
            _subItems.AfterRemove += _subItems_AfterRemove;
            _subItems.AfterSet += _subItems_AfterSet;
		}
		#endregion
		#region Public Properties
		/// <summary>
		/// Determine the text displayed in a TreeNode.
		/// </summary>
		[DefaultValue("TreeNode"), 
			Description("Determine the text displayed in a TreeNode.")]
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;
					_owner._nodeTextChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine the font used to draw the Text of a TreeNode.
		/// </summary>
		[Description("Determine the font used to draw the Text of a TreeNode.")]
		public Font NodeFont {
			get {
				if (_font == null) return _owner.Font;
				else return _font;
			}
			set {
				if (_font != value) {
					_font = value;
					_owner._nodeFontChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine a color used for TreeNode background.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Color), "Transparent"), 
			Description("Determine a color used for TreeNode background.")]
		public System.Drawing.Color BackColor {
			get { return _backColor; }
			set {
				if (_backColor != value) {
					_backColor = value;
					_owner._nodeBackColorChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine a color used to draw the Text of a TreeNode.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Color), "Black"), 
			Description("Determine a color used to draw the Text of a TreeNode.")]
		public System.Drawing.Color Color {
			get { return _color; }
			set {
				if (_color != value) {
					_color = value;
					_owner._nodeColorChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when the TreeNode is Collapsed.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when the TreeNode is Collapsed.")]
		public System.Drawing.Image Image {
			get { return _image; }
			set {
				if (_image != value) {
					_image = value;
					_owner._nodeImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when the TreeNode is Expanded.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when the TreeNode is Expanded.")]
		public System.Drawing.Image ExpandedImage {
			get { return _expandedImage; }
			set {
				if (_expandedImage != value) {
					_expandedImage = value;
					_owner._nodeExpandedImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine whether a TreeNode object is checked.
		/// </summary>
		[DefaultValue(false), 
			Description("Determine whether a TreeNode object is checked.")]
		public bool Checked {
			get { return _checked; }
			set {
				if (_checked != value) {
					TreeNodeEventArgs e = new TreeNodeEventArgs(this, TreeNodeAction.Checked);
					_owner._nodeBeforeCheck(this, e);
					if (!e.Cancel) {
						_checked = value;
						if (_checked) _checkState = System.Windows.Forms.CheckState.Checked;
						else _checkState = System.Windows.Forms.CheckState.Unchecked;
						_owner._nodeChecked(this);
					}
				}
			}
		}
        /// <summary>
        /// Determine whether item style is used for all subitems.
        /// </summary>
		[DefaultValue(true), 
			Description("Determine whether item style is used for all subitems.")]
		public bool UseNodeStyleForSubItems {
			get { return _useNodeStyleForSubItems; }
			set {
				if (_useNodeStyleForSubItems != value) {
					_useNodeStyleForSubItems = value;
					_owner._nodeUseNodeStyleForSubItemsChanged(this);
				}
			}
		}
        /// <summary>
        /// Determine the name of a TreeNode object.
        /// </summary>
		[DefaultValue("TreeNode"), 
			Description("Determine the name of a TreeNode object.")]
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
        /// <summary>
        /// Determine the contents of the ToolTip should be displayed when mouse hover the TreeNode.
        /// </summary>
		[DefaultValue(""), EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
			typeof(System.Drawing.Design.UITypeEditor)), 
			Description("Determine the contents of the ToolTip should be displayed when mouse hover the TreeNode.")]
		public string ToolTip {
			get { return _tooltip; }
			set { _tooltip = value; }
		}
        /// <summary>
        /// Determine the title of the ToolTip should be displayed when mouse hover the TreeNode.
        /// </summary>
		[DefaultValue(""), 
			Description("Determine the title of the ToolTip should be displayed when mouse hover the TreeNode.")]
		public string ToolTipTitle {
			get { return _tooltipTitle; }
			set { _tooltipTitle = value; }
		}
        /// <summary>
        /// Determine the image of the ToolTip should be displayed when mouse hover the TreeNode.
        /// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine the image of the ToolTip should be displayed when mouse hover the TreeNode.")]
		public System.Drawing.Image ToolTipImage {
			get { return _tooltipImage; }
			set { _tooltipImage = value; }
		}
        /// <summary>
        /// Determine an object data associated with TreeNode object.
        /// </summary>
		[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
			Description("Determine an object data associated with TreeNode object.")]
		public object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
        /// <summary>
        /// Gets the collection of TreeNode objects assigned to the current tree node.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
            Description("Gets the collection of TreeNode objects assigned to the current tree node.")]
		public TreeNodeCollection Nodes { get { return _nodes; } }
        /// <summary>
        /// Gets the collection of TreeNodeSubItem objects assigned to the current TreeNode.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
			Description("Gets the collection of TreeNodeSubItem objects assigned to the current TreeNode.")]
		public TreeNodeSubItemCollection SubItems { get { return _subItems; } }
		[Browsable(false)]
		public MultiColumnTree MultiColumnTree { get { return _owner; } }
        /// <summary>
        /// Gets the path from the root tree node to the current tree node.
        /// </summary>
		[Browsable(false), Description("Gets the path from the root tree node to the current tree node.")]
		public string FullPath {
			get {
				if (_parent != null) return _parent.FullPath + _owner.PathSeparator + _text;
				else return _text;
			}
		}
        /// <summary>
        /// Gets the parent tree node of the current tree node.
        /// </summary>
		[Browsable(false), Description("Gets the parent tree node of the current tree node.")]
		public TreeNode Parent { get { return _parent; } }
        /// <summary>
        /// Gets the zero-based depth of the tree node in the TreeView control.
        /// </summary>
		[Browsable(false), Description("Gets the zero-based depth of the tree node in the TreeView control.")]
		public int Level {
			get {
				int _level = 0;
				TreeNode pNode = _parent;
				while (pNode != null) {
					_level = _level + 1;
					pNode = pNode._parent;
				}
				return _level;
			}
		}
        /// <summary>
        /// Gets a value indicating whether the tree node is in the expanded state.
        /// </summary>
		[Browsable(false), Description("Gets a value indicating whether the tree node is in the expanded state.")]
		public bool IsExpanded { get { return _isExpanded; } }
        public System.Windows.Forms.CheckState CheckState { get { return _checkState; } }
		#endregion
        #region Event Handlers
        private void _subItems_AfterClear(object sender, CollectionEventArgs e) { _owner._nodeSubItemsChanged(this); }
        private void _subItems_AfterInsert(object sender, CollectionEventArgs e) { _owner._nodeSubItemsChanged(this); }
        private void _subItems_AfterRemove(object sender, CollectionEventArgs e) { _owner._nodeSubItemsChanged(this); }
        private void _subItems_AfterSet(object sender, CollectionEventArgs e) { _owner._nodeSubItemsChanged(this); }
        private void _nodes_AfterInsert(object sender, CollectionEventArgs e) {
            if (NodeAdded != null) NodeAdded(this, e);
        }
        private void _nodes_AfterRemove(object sender, CollectionEventArgs e) {
            if (NodeRemoved != null) NodeRemoved(this, e);
        }
        private void _nodes_AfterClear(object sender, CollectionEventArgs e) {
            if (NodesOnClear != null) NodesOnClear(this, e);
        }
        #endregion
        #region Public Functions
        /// <summary>
        /// Collapses the tree node.
        /// </summary>
        [Description("Collapses the tree node.")]
        public void collapse() {
            if (_isExpanded) {
                TreeNodeEventArgs e = new TreeNodeEventArgs(this, TreeNodeAction.Collapse);
                _owner._nodeBeforeCollapse(this, e);
                if (!e.Cancel) {
                    _isExpanded = false;
                    _owner._nodeCollapsed(this);
                }
            }
        }
        /// <summary>
        /// Collapses the TreeNode and optionally collapses its children.
        /// </summary>
        [Description("Collapses the TreeNode and optionally collapses its children.")]
        public void collapse(bool ignoreChildren) { 
            if (_isExpanded) {
                TreeNodeEventArgs e = new TreeNodeEventArgs(this, TreeNodeAction.Collapse);
                _owner._nodeBeforeCollapse(this, e);
                if (!e.Cancel) {
                    _isExpanded = false;
                    _owner._nodeCollapsed(this, ignoreChildren);
                } else {
                    if (!ignoreChildren) {
                        foreach (TreeNode tn in _nodes) tn.collapse(ignoreChildren);
                    }
                }
            } else if (!ignoreChildren) {
                foreach (TreeNode tn in _nodes) tn.collapse(ignoreChildren);
            }
        }
        /// <summary>
        /// Expands the tree node.
        /// </summary>
        [Description("Expands the tree node.")]
        public void expand() { 
            if (!_isExpanded) {
                TreeNodeEventArgs e = new TreeNodeEventArgs(this, TreeNodeAction.Expand);
                _owner._nodeBeforeExpand(this, e);
                if (!e.Cancel) {
                    _isExpanded = true;
                    _owner._nodeExpanded(this);
                }
            }
        }
        /// <summary>
        /// Expands all the child tree nodes.
        /// </summary>
        [Description("Expands all the child tree nodes.")]
        public void expandAll() { 
            if (!_isExpanded) {
                TreeNodeEventArgs e = new TreeNodeEventArgs(this, TreeNodeAction.Expand);
                _owner._nodeBeforeExpand(this, e);
                if (!e.Cancel) {
                    _isExpanded = true;
                    _owner._nodeExpanded(this);
                } else {
                    foreach (TreeNode tn in _nodes) tn.expandAll();
                }
            } else {
                foreach (TreeNode tn in _nodes) tn.expandAll();
            }
        }
        /// <summary>
        /// Returns the number of child tree nodes.
        /// </summary>
        [Description("Returns the number of child tree nodes.")]
        public int getNodeCount(bool includeSubTrees) { 
            int result = _nodes.Count;
            if (includeSubTrees) {
                foreach (TreeNode tn in _nodes) result += tn.getNodeCount(includeSubTrees);
            }
            return result;
        }
        /// <summary>
        /// Removes the current tree node from the MultiColumnTree control.
        /// </summary>
        [Description("Removes the current tree node from the MultiColumnTree control.")]
        public void remove() {
            if (_parent != null) _parent._nodes.Remove(this);
            else _owner.Nodes.Remove(this);
        }
        #endregion
        #region Internal Functions
        internal void _expand() { 
            if (!_isExpanded) {
                _isExpanded = true;
                _owner._invokeNodeExpanded(this);
            }
        }
        internal void _collapse() { 
            if (_isExpanded) {
                _isExpanded = false;
                _owner._invokeNodeCollapsed(this);
            }
        }
        internal void setCheckState(System.Windows.Forms.CheckState state) { 
            if (_checkState != state) {
                _checkState = state;
                if (_checkState == System.Windows.Forms.CheckState.Checked) {
                    if (!_checked) {
                        _checked = true;
                        _owner._invokeNodeChecked(this);
                    }
                } else if (_checkState == System.Windows.Forms.CheckState.Unchecked) {
                    if (_checked) {
                        _checked = false;
                        _owner._invokeNodeChecked(this);
                    }
                }
            }
        }
        internal void setChecked(bool value) { 
            if (_checked != value) {
                _checked = value;
                if (_checked) _checkState = System.Windows.Forms.CheckState.Checked;
                else _checkState = System.Windows.Forms.CheckState.Unchecked;
                _owner._invokeNodeChecked(this);
            }
        }
        internal void setParentNOwner() { 
            _nodes._owner = _owner;
            _nodes._parent = this;
            foreach (TreeNode cn in _nodes) {
                cn._owner = _owner;
                cn._parent = this;
                cn.setParentNOwner();
            }
        }
        #endregion
    }
}