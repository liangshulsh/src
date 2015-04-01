// Ai Software Control Library.
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ai.Control {
	public class ComboBoxItem {
		#region Declarations
		string _name = "ComboBoxItem";
		string _text = "ComboBoxItem";
		bool _checked = false;
		bool _enabled = true;
		#endregion
		#region Internal Events
		internal event EventHandler<EventArgs> TextChanged;
		internal event EventHandler<EventArgs> CheckedChanged;
		#endregion
		public ComboBoxItem() { }
		#region Public Properties
		/// <summary>
		/// Gets or sets the name of the ComboBoxItem object.
		/// </summary>
		[DefaultValue("ComboBoxItem"), Description("Gets or sets the name of the ComboBoxItem object.")]
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		/// <summary>
		/// Gets or sets the text of the ComboBoxItem object.
		/// </summary>
		[DefaultValue("ComboBoxItem"), Description("Gets or sets the text of the ComboBoxItem object.")]
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;
                    if (TextChanged != null) TextChanged(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// Gets or sets the checked state of the ComboBoxItem object.
		/// </summary>
		[DefaultValue(false), Description("Gets or sets the checked state of the ComboBoxItem object.")]
		public bool Checked {
			get { return _checked; }
			set {
				if (_checked != value) {
					_checked = value;
                    if (CheckedChanged != null) CheckedChanged(this, new EventArgs());
				}
			}
		}
		#endregion
	}
}