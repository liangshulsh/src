// Ai Software Control Library.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Ai.Control {
    /// <summary>
    /// Class to represent a Category Axis in the Chart.
    /// </summary>
    public class Axis {
        #region Public Events
        public event EventHandler<EventArgs> TextChanged;
        public event EventHandler<EventArgs> ValueChanged;
        public event EventHandler<EventArgs> ColorChanged;
        #endregion
        private System.Drawing.Color _color = System.Drawing.Color.Black;
        private object _value = null;
        private object _tag = null;
        private string _text = "";
        internal Chart _owner = null;
        public Axis() { _owner = new Chart(); }
        public Axis(Chart owner) { _owner = owner; }
        /// <summary>
        /// Gets or sets the text that will be displayed on chart axis.
        /// </summary>
        [DefaultValue(""), Description("Gets or sets the text that will be displayed on chart axis.")]
        public string Text {
            get { return _text; }
            set {
                if (_text != value) {
                    _text = value;
                    TextChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets color used to draw the text of an Axis.
        /// </summary>
        [DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Gets or sets color used to draw the text of an Axis.")]
        public System.Drawing.Color Color {
            get { return _color; }
            set {
                if (_color != value) {
                    _color = value;
                    ColorChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the value of an Axis.
        /// </summary>
        [DefaultValue(""), TypeConverter(typeof(StringConverter)), Description("Gets or sets the value of an Axis.")]
        public object Value {
            get { return _value; }
            set {
                if (_value != value) {
                    _value = value;
                    ValueChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets an object data associated with an Axis.
        /// </summary>
        [DefaultValue(""), TypeConverter(typeof(StringConverter)), Description("Gets or sets an object data associated with an Axis.")]
        public object Tag {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}