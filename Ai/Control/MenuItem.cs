// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Provide a base class for all menu item used in TabbedMenu control.
    /// </summary>
	public abstract class MenuItem {
		/// <summary>
		/// Define how a MenuItem must be resized.
		/// </summary>
		public enum ItemSizeType { Small, Large }
		#region Internal Declarations
		internal string _name = "MenuItem";
        internal string _text = "MenuItem";
		internal System.Drawing.Image _image = null;
		internal bool _visible = true;
		internal bool _enabled = true;
		internal string _tooltip = "";
		internal string _tooltipTitle = "";
		internal System.Drawing.Image _tooltipImage = null;
		internal ItemSizeType _sizeType = ItemSizeType.Small;
		internal object _tag = null;
        internal MenuItem _parent = null;
        internal TabbedMenu _owner = null;
		#endregion
		#region Public Properties
		/// <summary>
		/// Gets or sets the SizeType of the MenuItem object.
		/// </summary>
		[Description("Gets or sets the SizeType of the MenuItem object."), DefaultValue(typeof(ItemSizeType), "Small")]
		public virtual ItemSizeType SizeType {
			get { return _sizeType; }
			set { _sizeType = value; }
		}
		/// <summary>
		/// Gets or sets the name of the MenuItem object.
		/// </summary>
		[DefaultValue("MenuItem"), Description("Gets or sets the name of the MenuItem object.")]
		public virtual string Name {
			get { return _name; }
			set { _name = value; }
		}
		/// <summary>
		/// Gets or sets the text associated with MenuItem.
		/// </summary>
		[DefaultValue("MenuItem"), Description("Gets or sets the text associated with MenuItem.")]
		public virtual string Text {
			get { return _text; }
			set { _text = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether the MenuItem can respond to user interaction.
		/// </summary>
		[DefaultValue(true), Description("Gets or sets a value indicating whether the MenuItem can respond to user interaction.")]
		public virtual bool Enabled {
			get { return _enabled; }
			set { _enabled = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether the MenuItem are displayed.
		/// </summary>
		[DefaultValue(true), Description("Gets or sets a value indicating whether the MenuItem are displayed.")]
		public virtual bool Visible {
			get { return _visible; }
			set { _visible = value; }
		}
		/// <summary>
		/// Gets or sets the image that is displayed on a MenuItem.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), Description("Gets or sets the image that is displayed on a MenuItem.")]
		public virtual System.Drawing.Image Image {
			get { return _image; }
			set { _image = value; }
		}
		/// <summary>
		/// Gets or sets a text displayed on the ToolTip window.
		/// </summary>
		[DefaultValue(""), Description("Gets or sets a text displayed on the ToolTip window.")]
		public string ToolTip {
			get { return _tooltip; }
			set { _tooltip = value; }
		}
		/// <summary>
		/// Gets or sets a title displayed on the ToolTip window.
		/// </summary>
		[DefaultValue(""), Description("Gets or sets a title displayed on the ToolTip window.")]
		public string ToolTIpTitle {
			get { return _tooltipTitle; }
			set { _tooltipTitle = value; }
		}
		/// <summary>
		/// Gets or sets an image displayed on the ToolTip window.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), Description("Gets or sets an image displayed on the ToolTip window.")]
		public System.Drawing.Image ToolTipImage {
			get { return _tooltipImage; }
			set { _tooltipImage = value; }
		}
		/// <summary>
		/// Determine an object data associated with MenuItem object.
		/// </summary>
		[DefaultValue(""), TypeConverter(typeof(StringConverter)), Description("Determine an object data associated with MenuItem object.")]
		public object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
		#endregion
        #region Constructor
        public MenuItem() {
            _parent = null;
            _owner = new TabbedMenu();
        }
        #endregion
    }
}