// Ai Software Control Library.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Ai.Control {
    /// <summary>
    /// Determine the control type to edit data in grid.
    /// </summary>
    public enum DataInputType {
        /// <summary>
        /// Show a single line textbox control.
        /// </summary>
        SingleLineTextBox,
        /// <summary>
        /// Show a drop down contain a multiline textbox control.
        /// </summary>
        MultiLineTextBox,
        /// <summary>
        /// Show a drop down contain a list of value.
        /// </summary>
        List,
        /// <summary>
        /// Show a combo box control.
        /// </summary>
        ComboBox,
        /// <summary>
        /// Show a drop down contain a list of checked value.
        /// </summary>
        CheckedList,
        /// <summary>
        /// Show a check box control.
        /// </summary>
        CheckBox,
        /// <summary>
        /// Show a date time picker control.
        /// </summary>
        DateTime,
        /// <summary>
        /// Show a numeric input control.
        /// </summary>
        Number,
        /// <summary>
        /// Use custom input method.
        /// </summary>
        Custom,
        /// <summary>
        /// No control to be shown.
        /// </summary>
        None
    }
    /// <summary>
    /// Specifies the method to provide custom input.
    /// </summary>
    public enum CustomInputMethod { 
        /// <summary>
        /// Displays a button for browse a value.
        /// </summary>
        Browse,
        /// <summary>
        /// Display a drop down button to shows a popup window to edit the value.
        /// </summary>
        PopUp
    }
    /// <summary>
    /// A class to represent column header of a DataGrid control.
    /// </summary>
    /// <remarks>
    /// Each column have specific input method when editing the value of a cell.
    /// MaxValue and MinValue property will used to set the range value allowed when using Number input type.
    /// MaxDate and MinDate property will used to set up the value allowed when using Date input type.
    /// List property will used to show option when using List, ComboBox, CheckedList input types, 
    /// especially for CheckedList, CheckedItems of a cell is represent the checked items in CheckedList.
    /// </remarks>
    public class DataGridHeader : ColumnHeader {
        #region Members
        DataInputType _inputType = DataInputType.None;
        CustomInputMethod _customMethod = CustomInputMethod.Browse;
        ObjectCollection _list = new ObjectCollection();
        string _listSeparator = ",";
        #endregion
        #region Constructor
        public DataGridHeader() : base() {
            _owner = new DataGrid();
        }
        public DataGridHeader(DataGrid owner) : base() { _owner = owner; }
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets or sets the control to be shown to edit a data in grid.
        /// </summary>
        [DefaultValue(typeof(DataInputType), "None"), Description("Gets or sets the control to be shown to edit a data in grid.")]
        public DataInputType InputType {
            get { return _inputType; }
            set { _inputType = value; }
        }
        /// <summary>
        /// Gets or sets how custom input will be shown to modify the value.
        /// </summary>
        [DefaultValue(typeof(CustomInputMethod), "Browse"), Description("Gets or sets how custom input will be shown to modify the value.")]
        public CustomInputMethod CustomMethod {
            get { return _customMethod; }
            set { _customMethod = value; }
        }
        /// <summary>
        /// Gets or sets the separator to show the value of list.
        /// </summary>
        [DefaultValue(","), Description("Gets or sets the separator to show the value of list.")]
        public string ListSeparator {
            get { return _listSeparator; }
            set { _listSeparator = value; }
        }
        /// <summary>
        ///Gets the collection of object to be shown as list in editor control.
        /// </summary>
        [Browsable(false), Description("Gets the collection of object to be shown as list in editor control.")]
        public ObjectCollection List { get { return _list; } }
        public override ColumnSizeType SizeType {
            get { return base.SizeType; }
            set {
                if (value != ColumnSizeType.Fixed) return;
                base.SizeType = value;
            }
        }
        #endregion
    }
}