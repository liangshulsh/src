// Ai Software Control Library.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ai.Control {
	public sealed class MenuButton : MenuItem {
		#region Public Events
		public event EventHandler<EventArgs> TextChanged;
		public event EventHandler<EventArgs> EnabledChanged;
		public event EventHandler<EventArgs> VisibleChanged;
		public event EventHandler<EventArgs> SizeTypeChanged;
		public event EventHandler<EventArgs> ImageChanged;
		public event EventHandler<EventArgs> SelectableChanged;
		public event EventHandler<EventArgs> SelectedChanged;
		public event EventHandler<EventArgs> Click;
		#endregion
		#region Internal Events
		internal event EventHandler<EventArgs> _TextChanged;
		internal event EventHandler<EventArgs> _EnabledChanged;
		internal event EventHandler<EventArgs> _VisibleChanged;
		internal event EventHandler<EventArgs> _SizeTypeChanged;
		internal event EventHandler<EventArgs> _ImageChanged;
		internal event EventHandler<EventArgs> _SelectableChanged;
		internal event EventHandler<EventArgs> _SelectedChanged;
		#endregion
		#region Members
		bool _selectable = false;
		bool _selected = false;
		#endregion
		public MenuButton() {
			base._name = "Button";
			base._text = "Button";
		}
		/// <summary>
		/// Gets or sets the name of the MenuButton object.
		/// </summary>
		[DefaultValue("Button"), Description("Gets or sets the name of the MenuButton object.")]
		public override string Name {
			get { return base._name; }
			set { base._name = value; }
		}
		/// <summary>
		/// Gets or sets the SizeType of the MenuButton object.
		/// </summary>
		[Description("Gets or sets the SizeType of the MenuButton object."), 
			DefaultValue(typeof(MenuItem.ItemSizeType), "Small")]
		public override MenuItem.ItemSizeType SizeType {
			get { return base._sizeType; }
			set {
				if (base._sizeType != value) {
					base._sizeType = value;
					if (_SizeTypeChanged != null) _SizeTypeChanged(this, new EventArgs());
					if (SizeTypeChanged != null) SizeTypeChanged(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// Gets or sets the text associated with MenuButton.
		/// </summary>
		[DefaultValue("Button"), Description("Gets or sets the text associated with MenuButton.")]
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
		/// Gets or sets a value indicating whether the MenuButton can respond to user interaction.
		/// </summary>
		[DefaultValue(true), Description("Gets or sets a value indicating whether the MenuButton can respond to user interaction.")]
		public override bool Enabled {
			get { return base._enabled; }
			set {
				if (base._enabled != value) {
					base._enabled = value;
					if (_EnabledChanged != null) _EnabledChanged(this, new EventArgs());
					if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
				}
			}
		}
	}
}