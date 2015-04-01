// Ai Software Control Library.
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
	public class MenuLabel : MenuItem {
		#region Public Events
		public event EventHandler<EventArgs> TextChanged;
		public event EventHandler<EventArgs> EnabledChanged;
		public event EventHandler<EventArgs> VisibleChanged;
		#endregion
		#region Internal Events
		internal event EventHandler<EventArgs> _TextChanged;
		internal event EventHandler<EventArgs> _EnabledChanged;
		internal event EventHandler<EventArgs> _VisibleChanged;
		#endregion
		public MenuLabel() : base() {
			base._name = "Label";
			base._text = "Label";
		}
		#region Public Properties
		/// <summary>
		/// Gets or sets the name of the MenuLabel object.
		/// </summary>
		[DefaultValue("Label"), Description("Gets or sets the name of the MenuLabel object.")]
		public override string Name {
			get { return base._name; }
			set { base._name = value; }
		}
		/// <summary>
		/// Gets or sets the SizeType of the MenuLabel object.
		/// </summary>
		[Description("Gets or sets the SizeType of the MenuLabel object."), 
			DefaultValue(typeof(MenuItem.ItemSizeType), "Small"), 
			EditorBrowsable(EditorBrowsableState.Never), 
			Browsable(false)]
		public override MenuItem.ItemSizeType SizeType {
			get { return base._sizeType; }
			set { base._sizeType = MenuItem.ItemSizeType.Small; }
		}
		/// <summary>
		/// Gets or sets the name of the MenuLabel object.
		/// </summary>
		[DefaultValue("Label"), Description("Gets or sets the text associated with MenuLabel object.")]
		public override string Text {
			get { return base._text; }
			set { 
				if (base._text != value) {
					base._text = value;
					if (_TextChanged != null) _TextChanged(this, new EventArgs());
                    if (TextChanged != null) TextChanged(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the MenuLabel can respond to user interaction.
		/// </summary>
		[DefaultValue(true), Description("Gets or sets a value indicating whether the MenuLabel can respond to user interaction.")]
		public override bool Enabled {
			get { return base._enabled; }
			set	{
				if (base._enabled != value) {
					base._enabled = value;
					if (_EnabledChanged != null) _EnabledChanged(this, new EventArgs());
					if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the MenuLabel are displayed.
		/// </summary>
		[DefaultValue(true), Description("Gets or sets a value indicating whether the MenuLabel are displayed.")]
		public override bool Visible {
			get { return base._visible; }
			set {
				if (base._visible != value) {
					base._visible = value;
					if (_VisibleChanged != null) _VisibleChanged(this, new EventArgs());
					if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// Gets or sets the image that is displayed on a MenuLabel.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Image), "null"), 
			Description("Gets or sets the image that is displayed on a MenuLabel."), 
			EditorBrowsable(EditorBrowsableState.Never), 
			Browsable(false)]
		public override System.Drawing.Image Image {
			get { return base._image; }
			set { base._image = value; }
		}
		#endregion
	}
}