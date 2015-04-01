// Ai Software Control Library.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ai.Control {
    /// <summary>
    /// Class to represent a Value Axis in the Chart.
    /// </summary>
    public class ValueAxis {
        #region Public Events
        public event EventHandler<EventArgs> TextChanged;
        public event EventHandler<EventArgs> ValueChanged;
        public event EventHandler<EventArgs> ColorChanged;
        #endregion
        System.Drawing.Color _color = System.Drawing.Color.Black;
        double _value = 0D;
        string _text = "";
        object _tag = null;
        internal Chart _owner = null;
        public ValueAxis() { _owner = new Chart(); }
        public ValueAxis(Chart owner) { _owner = owner; }
        /// <summary>
        /// Gets or sets the text that will be displayed on chart axis.
        /// </summary>
        [DefaultValue(""), Description("Gets or sets the text that will be displayed on chart axis.")]
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
        /// Gets or sets color used to draw the text of an Axis.
        /// </summary>
        [DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Gets or sets color used to draw the text of an Axis.")]
        public System.Drawing.Color Color {
            get { return _color; }
            set {
                if (_color != value) {
                    _color = value;
                    if (ColorChanged != null) ColorChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets or sets the value of an Axis.
        /// </summary>
        [DefaultValue(0D), Description("Gets or sets the value of an Axis.")]
        public double Value {
            get { return _value; }
            set {
                if (_value != value) {
                    _value = value;
                    if (ValueChanged != null) ValueChanged(this, new EventArgs());
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