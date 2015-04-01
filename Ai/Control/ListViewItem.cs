// Ai Software Control Library.
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;

namespace Ai.Control {
	/// <summary>
	/// Class to represent an Item in the ListView.
	/// </summary>
	[DefaultProperty("Text")]
	public class ListViewItem {
		#region Public Classes
		/// <summary>
		/// Class to represent a SubItem of an Item in the ListView.
		/// </summary>
		[DefaultProperty("Value")]
		public class ListViewSubItem {
			#region Declarations
			internal ListViewItem _owner = null;
			internal Font _font = null;
			object _value = null;
			object _tag = null;
			System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
            System.Drawing.Color _color = System.Drawing.Color.Black;
			string _name = "ListViewSubItem";
			bool _printValueOnBar = false;
			#endregion
			#region Constructor
			/// <summary>
			/// Create an instance of ListViewSubItem.
			/// </summary>
			public ListViewSubItem() {
				_owner = new ListViewItem();
				_font = _owner.Font;
			}
			/// <summary>
			/// Create an instance of the ListViewSubItem with specified ListViewItem as its owner.
			/// </summary>
			public ListViewSubItem(ListViewItem owner) {
				_owner = owner;
				_font = _owner.Font;
			}
			#endregion
			#region Public Properties
			/// <summary>
			/// Determine the name of the ListViewSubItem Object.
			/// </summary>
			[DefaultValue("ListViewSubItem"), 
				Description("Determine the name of the ListViewSubItem Object.")]
			public string Name {
				get { return _name; }
				set { _name = value; }
			}
			/// <summary>
			/// Determine an Object data associated with the ListViewSubItem.
			/// </summary>
			[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
				Description("Determine an Object data associated with the ListViewSubItem.")]
			public object Tag {
				get { return _tag; }
				set { _tag = value; }
			}
			/// <summary>
			/// Determine the value of the ListViewSubItem.
			/// </summary>
			[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
				Description("Determine the value of the ListViewSubItem.")]
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
			/// Determine a Font object to draw the value of the ListViewSubItem.
			/// </summary>
			[Description("Determine a Font object to draw the value of the ListViewSubItem.")]
			public System.Drawing.Font Font {
				get { return _font; }
				set {
					if (_font != value) {
						_font = value;
						_owner._owner._subitemFontChanged(this);
					}
				}
			}
			/// <summary>
			/// Determine a color used for ListViewSubItem background.
			/// </summary>
			[DefaultValue(typeof(System.Drawing.Color), "Transparent"), 
				Description("Determine a color used for ListViewSubItem background.")]
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
			/// Determine the color used to draw the value of the ListViewSubItem.
			/// </summary>
			[DefaultValue(typeof(System.Drawing.Color), "Black"), 
				Description("Determine the color used to draw the value of the ListViewSubItem.")]
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
			public ListViewItem ListViewItem { get { return _owner; } }
			[Browsable(false)]
			public ListView ListView { get { return _owner._owner; } }
			/// <summary>
			/// Draw subitem value when column format is Bar
			/// </summary>
			[DefaultValue(false), Description("Draw subitem value when column format is Bar")]
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
		/// Class to represent a Collection of ListViewSubItem object.
		/// </summary>
		public class ListViewSubItemCollection : CollectionBase {
			ListViewItem _owner;
			/// <summary>
			/// Create an instance of ListViewSubItemCollection with a ListViewItem as its owner.
			/// </summary>
			[Description("Create an instance of ListViewSubItemCollection with a ListViewItem as its owner.")]
			public ListViewSubItemCollection(ListViewItem owner) : base() { _owner = owner; }
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
			/// Gets a ListViewSubItem object in the collection specified by its index.
			/// </summary>
			[Description("Gets a ListViewSubItem object in the collection specified by its index.")]
			public ListViewSubItem this[int index] {
				get {
					if (index == 0) {
						ListViewSubItem aSubItem = new ListViewSubItem();
                        aSubItem.Value = _owner._text;
                        aSubItem.Font = _owner.Font;
                        aSubItem.Color = _owner.Color;
                        aSubItem.BackColor = _owner.BackColor;
                        return aSubItem;
					} else if (index >=1 && index <= List.Count) {
						return (ListViewSubItem)List[index - 1];
					}
					return null;
				}
			}
			/// <summary>
			/// Gets the index of a ListViewSubItem object in the collection.
			/// </summary>
			[Description("Gets the index of a ListViewSubItem object in the collection.")]
			public int IndexOf(ListViewSubItem item) { return List.IndexOf(item); }
			/// <summary>
			/// Add a ListViewSubItem object to the collection.
			/// </summary>
			[Description("Add a ListViewSubItem object to the collection.")]
			public ListViewSubItem Add(ListViewSubItem item) {
				item._owner = _owner;
				int index = List.Add(item);
				return (ListViewSubItem)List[index];
			}
			/// <summary>
			/// Add a ListViewSubItem object to the collection by providing its value.
			/// </summary>
			[Description("Add a ListViewSubItem object to the collection by providing its value.")]
			public ListViewSubItem Add(object value) {
				ListViewSubItem anItem = new ListViewSubItem(_owner);
				anItem.Value = value;
				return this.Add(anItem);
			}
			/// <summary>
			/// Add a ListViewSubItem collection to the collection.
			/// </summary>
			[Description("Add a ListViewSubItem collection to the collection."), 
				System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
			public void AddRange(ListViewSubItemCollection items) {
				foreach (ListViewSubItem item in items) this.Add(item);
			}
			/// <summary>
			/// Insert a ListViewSubItem object to the collection at specified index.
			/// </summary>
			[Description("Insert a ListViewSubItem object to the collection at specified index.")]
			public void Insert(int index, ListViewSubItem item) {
				item._owner = _owner;
				List.Insert(index, item);
			}
			/// <summary>
			/// Remove a ListViewSubItem object from the collection.
			/// </summary>
			[Description("Remove a ListViewSubItem object from the collection.")]
			public void Remove(ListViewSubItem item) {
				if (List.Contains(item)) List.Remove(item);
			}
			/// <summary>
			/// Determine whether a ListViewSubItem object exist in the collection.
			/// </summary>
			[Description("Determine whether a ListViewSubItem object exist in the collection.")]
			public bool Contains(ListViewSubItem item) { return List.Contains(item); }
			protected override void OnValidate(object value) {
				if (!typeof(ListViewSubItem).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Control.ListViewSubItem", "value");
			}
            protected override void OnClear() { if (Clearing != null) Clearing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClear)); }
            protected override void OnClearComplete() { if (AfterClear != null) AfterClear(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnClearComplete)); }
			protected override void OnInsert(int index, object value) {
				if (_owner._owner.IsDesignMode) {
					ListViewSubItem item = (ListViewSubItem)value;
					item.Font = _owner._owner.Font;
				}
                if (Inserting != null) Inserting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsert, index, value));
			}
            protected override void OnInsertComplete(int index, object value) { if (AfterInsert != null) AfterInsert(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnInsertComplete, index, value)); }
            protected override void OnRemove(int index, object value) { if (Removing != null) Removing(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemove, index, value)); }
            protected override void OnRemoveComplete(int index, object value) { if (AfterRemove != null) AfterRemove(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnRemoveComplete, index, value)); }
            protected override void OnSet(int index, object oldValue, object newValue) { if (Setting != null) Setting(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
            protected override void OnSetComplete(int index, object oldValue, object newValue) { if (AfterSet != null) AfterSet(this, new CollectionEventArgs(CollectionEventArgs.EventType.OnSet, index, oldValue, newValue)); }
		}
		#endregion
		#region Declarations
		internal ListView _owner = null;
		internal ListViewGroup _group = null;
		internal System.Drawing.Font _font = null;
		string _text = "ListViewItem";
		string _name = "ListViewItem";
		System.Drawing.Color _color = System.Drawing.Color.Black;
		System.Drawing.Color _backColor = System.Drawing.Color.Transparent;
		bool _checked = false;
		System.Drawing.Image _smallImage = null;
		System.Drawing.Image _iconImage = null;
		System.Drawing.Image _tileImage = null;
		System.Drawing.Image _thumbnailImage = null;
		System.Drawing.Image _previewImage = null;
		object _tag = null;
		string _tooltip = "";
		string _tooltipTitle = "";
		System.Drawing.Image _tooltipImage = null;
		bool _useItemStyleForSubItems = true;
		ListViewSubItemCollection _subItems;
		#endregion
		#region Constructor
		/// <summary>
		/// Create an instance of ListViewItem.
		/// </summary>
		[Description("Create an instance of ListViewItem.")]
		public ListViewItem() {
			_owner = new ListView();
			_subItems = new ListViewSubItemCollection(this);
			_font = _owner.Font;
			_subItems.AfterClear += _subItems_AfterClear;
			_subItems.AfterInsert += _subItems_AfterInsert;
			_subItems.AfterRemove += _subItems_AfterRemove;
			_subItems.AfterSet += _subItems_AfterSet;
		}
		/// <summary>
		/// Create an Instance of ListViewItem with specified ListView as its owner.
		/// </summary>
		[Description("Create an Instance of ListViewItem with specified ListView as its owner.")]
		public ListViewItem(ListView owner) {
			_owner = owner;
			_subItems = new ListViewSubItemCollection(this);
			_font = _owner.Font;
			_subItems.AfterClear += _subItems_AfterClear;
			_subItems.AfterInsert += _subItems_AfterInsert;
			_subItems.AfterRemove += _subItems_AfterRemove;
			_subItems.AfterSet += _subItems_AfterSet;
		}
		#endregion
		#region Public Properties
		/// <summary>
		/// Determine the text displayed in a ListViewItem.
		/// </summary>
		[DefaultValue("ListViewItem"), 
			Description("Determine the text displayed in a ListViewItem.")]
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;
					_owner._itemTextChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine the font used to draw the Text of a ListViewItem.
		/// </summary>
		[Description("Determine the font used to draw the Text of a ListViewItem.")]
		public System.Drawing.Font Font {
			get {
				if (_font == null) return _owner.Font;
				else return _font;
			}
			set {
				if (_font != value) {
					_font = value;
					_owner._itemFontChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine a color used for ListViewItem background.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Color), "Transparent"), 
			Description("Determine a color used for ListViewItem background.")]
		public System.Drawing.Color BackColor {
			get { return _backColor; }
			set {
				if (_backColor != value) {
					_backColor = value;
					_owner._itemBackColorChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine a color used to draw the Text of a ListViewItem.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Color), "Black"), 
			Description("Determine a color used to draw the Text of a ListViewItem.")]
		public System.Drawing.Color Color {
			get { return _color; }
			set {
				if (_color != value) {
					_color = value;
					_owner._itemColorChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when view state of the ListView is Details or List.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when view state of the ListView is Details or List.")]
		public System.Drawing.Image SmallImage {
			get { return _smallImage; }
			set {
				if (_smallImage != value) {
					_smallImage = value;
					_owner._itemSmallImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when view state of the ListView is Tile.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when view state of the ListView is Tile.")]
		public System.Drawing.Image TileImage {
			get { return _tileImage; }
			set {
				if (_tileImage != value) {
					_tileImage = value;
					_owner._itemTileImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when view state of the ListView is Thumbnail.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when view state of the ListView is Thumbnail.")]
		public System.Drawing.Image ThumbnailImage {
			get { return _thumbnailImage; }
			set {
				if (_thumbnailImage != value) {
					_thumbnailImage = value;
					_owner._itemThumbnailImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine an image to be displayed when view state of the ListView is Preview.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine an image to be displayed when view state of the ListView is Preview.")]
		public System.Drawing.Image PreviewImage {
			get { return _previewImage; }
			set {
				if (_previewImage != value) {
					_previewImage = value;
					_owner._itemPreviewImageChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine whether a ListViewItem object is checked.
		/// </summary>
		[DefaultValue(false), 
			Description("Determine whether a ListViewItem object is checked.")]
		public bool Checked {
			get { return _checked; }
			set {
				if (_checked != value) {
					ItemEventArgs beforeCheck = new ItemEventArgs(this);
					_owner.invokeItemBeforeCheck(beforeCheck);
					if (beforeCheck.Cancel) return;
					_checked = value;
					_owner._itemCheckedChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine whether item style is used for all subitems.
		/// </summary>
		[DefaultValue(true), 
			Description("Determine whether item style is used for all subitems.")]
		public bool UseItemStyleForSubItems {
			get { return _useItemStyleForSubItems; }
			set {
				if (_useItemStyleForSubItems != value) {
					_useItemStyleForSubItems = value;
					_owner._itemUseItemStyleForSubItemsChanged(this);
				}
			}
		}
		/// <summary>
		/// Determine the name of a ListViewItem object.
		/// </summary>
		[DefaultValue("ListViewItem"), 
			Description("Determine the name of a ListViewItem object.")]
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		/// <summary>
		/// Determine the contents of the ToolTip should be displayed when mouse hover the ListViewItem.
		/// </summary>
		[DefaultValue(""), EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
			typeof(System.Drawing.Design.UITypeEditor)), 
			Description("Determine the contents of the ToolTip should be displayed when mouse hover the ListViewItem.")]
		public string ToolTip {
			get { return _tooltip; }
			set { _tooltip = value; }
		}
		/// <summary>
		/// Determine the title of the ToolTip should be displayed when mouse hover the ListViewItem.
		/// </summary>
		[Description("Determine the title of the ToolTip should be displayed when mouse hover the ListViewItem.")]
		public string ToolTipTitle {
			get { return _tooltipTitle; }
			set { _tooltipTitle = value; }
		}
		/// <summary>
		/// Determine the image of the ToolTip should be displayed when mouse hover the ListViewItem.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Determine the image of the ToolTip should be displayed when mouse hover the ListViewItem.")]
		public System.Drawing.Image ToolTipImage {
			get { return _tooltipImage; }
			set { _tooltipImage = value; }
		}
		/// <summary>
		/// Determine an object data associated with ListViewItem object.
		/// </summary>
		[DefaultValue(""), TypeConverter(typeof(StringConverter)), 
			Description("Determine an object data associated with ListViewItem object.")]
		public object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
		/// <summary>
		/// Determine where the ListViewItem placed in a ListViewGroup.
		/// </summary>
		[DefaultValue(typeof(ListViewGroup), "null"), 
			Description("Determine where the ListViewItem placed in a ListViewGroup.")]
		public ListViewGroup Group {
			get { return _group; }
			set {
				if (_group != value) {
					if (_group != null) _group.Items.Remove(this);
					_group = value;
					if (_group != null) {
						if (!_group.Items.Contains(this)) _group.Items.Add(this);
					}
					_owner._itemGroupChanged(this);
				}
			}
		}
		/// <summary>
		/// Gets the collection of ListViewSubItem objects assigned to the current ListViewItem.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
			Description("Gets the collection of ListViewSubItem objects assigned to the current ListViewItem.")]
		public ListViewSubItemCollection SubItems { get { return _subItems; } }
		[Browsable(false)]
		public ListView ListView { get { return _owner; } }
		#endregion
		#region Event Handlers
		private void _subItems_AfterClear(object sender, CollectionEventArgs e) { _owner._itemSubItemsChanged(this); }
		private void _subItems_AfterInsert(object sender, CollectionEventArgs e) { _owner._itemSubItemsChanged(this); }
		private void _subItems_AfterRemove(object sender, CollectionEventArgs e) { _owner._itemSubItemsChanged(this); }
		private void _subItems_AfterSet(object sender, CollectionEventArgs e) { _owner._itemSubItemsChanged(this); }
		#endregion
		#region Internal Methods
		internal void setChecked(bool value) {
			if (_checked != value) {
				_checked = value;
				_owner._itemCheckedChanged(this);
			}
		}
		#endregion
	}
}