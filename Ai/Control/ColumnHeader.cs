// Ai Software Control Library.
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;

namespace Ai.Control {
    /// <summary>
    /// Predefined format to display the value of a list item.
    /// </summary>
    [Description("Predefined format to display the value of a list item.")]
    public enum ColumnFormat { 
        // Date Time data type format.
        // Custom format.
        CustomDateTime,
        // Standard date time format.
        ShortDate,
        LongDate,
        FullDateShortTime,
        FullDateLongTime,
        GeneralDateShortTime,
        GeneralDateLongTime,
        RoundTripDateTime,
        RFC1123,
        SortableDateTime,
        ShortTime,
        LongTime,
        UniversalSortableDateTime,
        UniversalFullDateTime,
        // Numeric data type format.
        // Custom format.
        Bar,
        Custom,
        // Standard numeric format.
        Currency,
        DecimalNumber,
        Exponential,
        FixedPoint,
        General,
        Number,
        Percent,
        RoundTrip,
        HexaDecimal,
        // String data type.
        None,
        Password,
        // Boolean
        Check
    }
    /// <summary>
    /// Define how a column width on a list will be calculated.
    /// </summary>
    [Description("Define how a column width on a list will be calculated.")]
    public enum ColumnSizeType { 
        Auto,
        Fixed,
        Percentage,
        Fill
    }
    /// <summary>
    /// Enumeration to determine how the value must be compared with the filter parameters.
    /// </summary>
    /// <remarks>For the string type, if ContainsAny, ContainsAll, or NotContain is specified, the delimiter used to separate the words is comma and space.</remarks>
    [Description("Enumeration to determine how the value must be compared with the filter parameters.")]
    public enum FilterRangeMode { 
        // Numeric and DateTime data type.
        Between = 0,
        Outside = 1,
        // String data type.
        StartsWith = 0,
        EndsWith = 1,
        ContainsAll = 2,
        ContainsAny = 3,
        NotContain = 4,
        // Image only data type, for future implementation.
        Equal = 3
    }
    internal enum FilterChooserResult { OK, Cancel, Custom }
    /// <summary>
    /// Determine how the comparison method for filtering operation should be performed.
    /// </summary>
    [Description("Determine how the comparison method for filtering operation should be performed.")]
    internal enum FilterMode { ByRange, ByValue }
    public delegate bool CustomFilterFunction(object value);
    /// <summary>
    /// A class to represent column header of a ListView or MultiColumnTree control.
    /// </summary>
    /// <remarks>
    /// Each column can have specific format to display value of the related subitem, but column 0 formating will always be ignored.
    /// Text alignment for column header and text alignment for displayed value of the related subitem can be different, 
    /// for column header determined by TextAlign property of ColumnHeader class, and for displayed value determined by ColumnAlign property of ColumnHeader class.
    /// Each column can have sort and filter capabilities, specified by EnableSorting and SortOrder for sort capability, and EnableFiltering for filter capability.
    /// Each column can be hidden or freezed, specified by EnableHidden and Visible for visibility, and EnableFreezed and Freezed.
    /// Freezed column will be shown first, and top of the unfreezed column.
    /// Freezed column cannot be moved, or as a movement target of the other column, but still can be resized.
    /// An additional option placed at the right most of the column header, to change visibility and freezed for each column on runtime.
    /// </remarks>
    public class ColumnHeader {
        #region Declarations
        SortOrder _sortOrder = SortOrder.None;
        ColumnFormat _format = ColumnFormat.None;
        ColumnSizeType _sizeType = ColumnSizeType.Fixed;
        int _width = 75;
        string _text = "Column";
        HorizontalAlignment _textAlign = HorizontalAlignment.Left;
        HorizontalAlignment _columnAlign = HorizontalAlignment.Left;
        string _customFormat = "#";
        string _name = "ColumnHeader";
        Image _image = null;
        Object _tag = null;
        string _tooltip = "";
        string _tooltiptitle = "";
        Image _tooltipimage = null;
        bool _enableSorting = false;
        bool _enableFiltering = false;
        bool _enableFrozen = false;
        bool _enableHidden = false;
        bool _visible = true;
        bool _frozen = false;
        double _maxValue = 100D;
        double _minValue = 0D;
        bool _enableCustomFilter = false;
        CustomFilterFunction _customFilter = null;
        internal object _owner;
        internal int _displayIndex = 0;
        #endregion
        #region Internal Events
        internal event EventHandler<EventArgs> EnableSortingChanged;
        internal event EventHandler<EventArgs> EnableFilteringChanged;
        internal event EventHandler<EventArgs> EnableFrozenChanged;
        internal event EventHandler<EventArgs> EnableHiddenChanged;
        internal event EventHandler<EventArgs> SortOrderChanged;
        internal event EventHandler<EventArgs> FormatChanged;
        internal event EventHandler<EventArgs> CustomFormatChanged;
        internal event EventHandler<EventArgs> ColumnAlignChanged;
        internal event EventHandler<EventArgs> WidthChanged;
        internal event EventHandler<EventArgs> SizeTypeChanged;
        internal event EventHandler<EventArgs> TextChanged;
        internal event EventHandler<EventArgs> ImageChanged;
        internal event EventHandler<EventArgs> VisibleChanged;
        internal event EventHandler<EventArgs> FrozenChanged;
        internal event EventHandler<EventArgs> TextAlignChanged;
        internal event EventHandler<EventArgs> MaximumValueChanged;
        internal event EventHandler<EventArgs> MinimumValueChanged;
        #endregion
        #region Constructor
        public ColumnHeader() { _owner = null; }
        public ColumnHeader(ListView owner) { _owner = owner; }
        public ColumnHeader(MultiColumnTree owner) { _owner = owner; }
        #endregion
        #region Public Properties
        /// <summary>
        /// Determine whether the Column can perform sort operation.
        /// </summary>
        [DefaultValue(false), Description("Determine whether the Column can perform sort operation.")]
        public bool EnableSorting {
            get { return _enableSorting; }
            set {
                if (_enableSorting != value) {
                    _enableSorting = value;
                    if (EnableSortingChanged != null) EnableSortingChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine whether the Column can perform filter operation.
        /// </summary>
        [DefaultValue(false), Description("Determine whether the Column can perform filter operation.")]
        public bool EnableFiltering {
            get { return _enableFiltering; }
            set {
                if (_enableFiltering != value) {
                    _enableFiltering = value;
                    if (EnableFilteringChanged != null) EnableFilteringChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine whether the Column can be freezed.
        /// </summary>
        [DefaultValue(false), Description("Determine whether the Column can be freezed.")]
        public bool EnableFrozen {
            get { return _enableFrozen; }
            set {
                if (value && (_sizeType == ColumnSizeType.Fill)) return;
                if (_enableFrozen != value) {
                    _enableFrozen = value;
                    if (EnableFrozenChanged != null) EnableFrozenChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine whether the Column can be hidden.
        /// </summary>
        [DefaultValue(false), Description("Determine whether the Column can be hidden.")]
        public bool EnableHidden {
            get { return _enableHidden; }
            set {
                if (_enableHidden != value) {
                    _enableHidden = value;
                    if (EnableHiddenChanged != null) EnableHiddenChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Specifies a how the items in the List will be sorted based on this Column.
        /// </summary>
        [DefaultValue(typeof(SortOrder), "None"), Description("Specifies a how the items in the List will be sorted based on this Column.")]
        public SortOrder SortOrder {
            get { return _sortOrder; }
            set {
                if (_sortOrder != value) {
                    _sortOrder = value;
                    if (SortOrderChanged != null) SortOrderChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Specifies how the value or the SubItem in a ListViewItem will be formatted.
        /// </summary>
        [DefaultValue(typeof(ColumnFormat), "None"), Description("Specifies how the value or the SubItem in a ListViewItem will be formatted.")]
        public ColumnFormat Format {
            get { return _format; }
            set {
                if (_format != value) {
                    _format = value;
                    if (FormatChanged != null) FormatChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Specifies how the value or the SubItem in a ListItem will be formatted using this format.
        /// </summary>
        [DefaultValue("#"), Description("Specifies how the value or the SubItem in a ListItem will be formatted using this format.")]
        public string CustomFormat {
            get { return _customFormat; }
            set {
                if (_customFormat != value) {
                    _customFormat = value;
                    if (CustomFormatChanged != null) CustomFormatChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Specifies how a SubItem on a ListItem will be aligned.
        /// </summary>
        [DefaultValue(typeof(HorizontalAlignment), "Left"), Description("Specifies how a SubItem on a ListItem will be aligned.")]
        public HorizontalAlignment ColumnAlign {
            get { return _columnAlign; }
            set {
                if (_columnAlign != value) {
                    _columnAlign = value;
                    if (ColumnAlignChanged != null) ColumnAlignChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine whether custom filter button is appears in column's filter popup window.
        /// </summary>
        [DefaultValue(false), Description("Determine whether custom filter button is appears in column's filter popup window.")]
        public bool EnableCustomFilter {
            get { return _enableCustomFilter; }
            set { _enableCustomFilter = value; }
        }
        /// <summary>
        /// Determine width of the Column.
        /// </summary>
        [DefaultValue(75), Description("Determine width of the Column.")]
        public int Width {
            get { return _width; }
            set {
                if (_width != value) {
                    if (_sizeType == ColumnSizeType.Fixed && value < 25) return;
                    _width = value;
                    if (WidthChanged != null) WidthChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine how the Column width should be calculated.
        /// </summary>
        [DefaultValue(typeof(ColumnSizeType), "Fixed"), Description("Determine how the Column width should be calculated.")]
        public virtual ColumnSizeType SizeType {
            get { return _sizeType; }
            set {
                if (_sizeType != value) {
                    if (_width < 25 && _sizeType == ColumnSizeType.Fixed) return;
                    _sizeType = value;
                    if (_sizeType == ColumnSizeType.Fill) {
                        _enableFrozen = false;
                        _frozen = false;
                    }
                    if (SizeTypeChanged != null) SizeTypeChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine the text displayed in the Column header.
        /// </summary>
        [DefaultValue("Column"), Description("Determine the text displayed in the Column header.")]
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
        /// Determine the image displayed in the Column header.
        /// </summary>
        [DefaultValue(typeof(Image), "Null"), Description("Determine the image displayed in the Column header.")]
        public Image Image {
            get { return _image; }
            set {
                if (_image != value) {
                    _image = value;
                    if (ImageChanged != null) ImageChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine whether the Column is displayed in the List control.
        /// </summary>
        [DefaultValue(true), Description("Determine whether the Column is displayed in the List control.")]
        public bool Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;
                    if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine wherther the Column should be freeze in List control.
        /// </summary>
        [DefaultValue(false), Description("Determine wherther the Column should be freeze in List control.")]
        public bool Frozen {
            get { return _frozen; }
            set {
                if (_enableFrozen && _sizeType != ColumnSizeType.Fill) {
                    if (_frozen != value) {
                        _frozen = value;
                        if (FrozenChanged != null) FrozenChanged(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Determine how the text of the Column should be aligned.
        /// </summary>
        [DefaultValue(typeof(HorizontalAlignment), "Left"), Description("Determine how the text of the Column should be aligned.")]
        public HorizontalAlignment TextAlign {
            get { return _textAlign; }
            set {
                if (_textAlign != value) {
                    _textAlign = value;
                    if (TextAlignChanged != null) TextAlignChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine the name of the Column.
        /// </summary>
        [DefaultValue("ColumnHeader"), Description("Determine the name of the Column.")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Determine the contents of the ToolTip should be displayed when mouse hover the Column.
        /// </summary>
        [DefaultValue(""), EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), 
            typeof(System.Drawing.Design.UITypeEditor)), 
            Description("Determine the contents of the ToolTip should be displayed when mouse hover the Column.")]
        public string ToolTip {
            get { return _tooltip; }
            set { _tooltip = value; }
        }
        /// <summary>
        /// Determine the title of the ToolTip should be displayed when mouse hover the Column.
        /// </summary>
        [DefaultValue(""), Description("Determine the title of the ToolTip should be displayed when mouse hover the Column.")]
        public string ToolTipTitle {
            get { return _tooltiptitle; }
            set { _tooltiptitle = value; }
        }
        /// <summary>
        /// Determine the image of the ToolTip should be displayed when mouse hover the Column.
        /// </summary>
        [DefaultValue(typeof(Image), "Null"), Description("Determine the image of the ToolTip should be displayed when mouse hover the Column.")]
        public Image ToolTipImage {
            get { return _tooltipimage; }
            set { _tooltipimage = value; }
        }
        [Browsable(false)]
        public object Owner { get { return _owner; } }
        public int DisplayIndex { get { return _displayIndex; } }
        /// <summary>
        /// Determine an Object data associated with the Column.
        /// </summary>
        [DefaultValue(""), 
            Description("Determine an Object data associated with the Column."), 
            TypeConverter(typeof(StringConverter))]
        public object Tag {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Determine the maximum value for the column.  This property is used when the Format property is set to Bar.
        /// </summary>
        [DefaultValue(100D), Description("Determine the maximum value for the column.  This property is used when the Format property is set to Bar.")]
        public double MaximumValue {
            get { return _maxValue; }
            set {
                if (_maxValue != value) {
                    _maxValue = value;
                    if (MaximumValueChanged != null) MaximumValueChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Determine the minimum value for the column.  This property is used when the Format property is set to Bar.
        /// </summary>
        [DefaultValue(0D), Description("Determine the minimum value for the column.  This property is used when the Format property is set to Bar.")]
        public double MinimumValue {
            get { return _minValue; }
            set {
                if (_minValue != value) {
                    _minValue = value;
                    if (MinimumValueChanged != null) MinimumValueChanged(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Provide a function used to filter a value when using custom filtering operation.
        /// </summary>
        [Browsable(false), Description("Provide a function used to filter a value when using custom filtering operation.")]
        public CustomFilterFunction CustomFilter {
            get { return _customFilter; }
            set { _customFilter = value; }
        }
        #endregion
    }
    #region Internal Classes
    /// <summary>
    /// Class to represent a filter item of a ColumnHeader.
    /// </summary>
    internal class ColumnFilterItem : IComparable {
        ColumnFilterHandle _owner;
        object _value;
        bool _selected = true;
        public ColumnFilterItem(ColumnFilterHandle owner) {
            _owner = owner;
            _value = null;
        }
        public ColumnFilterItem(ColumnFilterHandle owner, object value) {
            _owner = owner;
            _value = value;
        }
        public bool Selected {
            get { return _selected; }
            set { _selected = value; }
        }
        public object Value {
            get { return _value; }
            set { _value = value; }
        }
        public ColumnFilterHandle Owner { get { return _owner; } }
        public int CompareTo(object obj) {
            ColumnFilterItem i2 = (ColumnFilterItem)obj;
            switch (_owner.DisplayFormat) { 
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
                    double d1 = (double)_value;
                    double d2 = (double)i2._value;
                    return d1.CompareTo(d2);
                case ColumnFormat.DecimalNumber:
                    int int1 = (int)_value;
                    int int2 = (int)i2._value;
                    return int1.CompareTo(int2);
                case ColumnFormat.None:
                case ColumnFormat.Password:
                    return string.Compare((string)_value, (string)i2._value, true);
                case ColumnFormat.Check:
                    bool bool1 = (bool)_value;
                    bool bool2 = (bool)i2._value;
                    Comparer boolComparer = new Comparer(new CultureInfo("en-US"));
                    return boolComparer.Compare(bool1, bool2);
                default:
                    DateTime dt1 = (DateTime)_value;
                    DateTime dt2 = (DateTime)i2._value;
                    return DateTime.Compare(dt1, dt2);
            }
        }
        public override bool Equals(object obj) {
            if (obj.GetType() == typeof(ColumnFilterItem)) {
                ColumnFilterItem i2 = (ColumnFilterItem)obj;
                switch (_owner.DisplayFormat) {
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
                        double d1 = (double)_value;
                        double d2 = (double)i2._value;
                        return d1.CompareTo(d2) == 0;
                    case ColumnFormat.DecimalNumber:
                        int int1 = (int)_value;
                        int int2 = (int)i2._value;
                        return int1.CompareTo(int2) == 0;
                    case ColumnFormat.None:
                    case ColumnFormat.Password:
                        return string.Compare((string)_value, (string)i2._value, true) == 0;
                    case ColumnFormat.Check:
                        bool bool1 = (bool)_value;
                        bool bool2 = (bool)i2._value;
                        Comparer boolComparer = new Comparer(new CultureInfo("en-US"));
                        return boolComparer.Compare(bool1, bool2) == 0;
                    default:
                        DateTime dt1 = (DateTime)_value;
                        DateTime dt2 = (DateTime)i2._value;
                        return DateTime.Compare(dt1, dt2) == 0;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Class to handle filtering operaton for a ColumnHeader.
    /// </summary>
    internal class ColumnFilterHandle {
        ColumnHeader _column;
        List<ColumnFilterItem> _items = new List<ColumnFilterItem>();
        List<object> _values = new List<object>();
        object _minValue = "";
        object _maxValue = "";
        bool _useCustomFilter = false;
        FilterMode _filterMode = FilterMode.ByValue;
        FilterRangeMode _rangeMode = FilterRangeMode.Between;
        public ColumnFilterHandle(ColumnHeader column) { _column = column; }
        public object MaxValueSelected {
            get { return _maxValue; }
            set { _maxValue = value; }
        }
        public object MinValueSelected {
            get { return _minValue; }
            set { _minValue = value; }
        }
        public FilterMode FilterMode {
            get { return _filterMode; }
            set { _filterMode = value; }
        }
        public FilterRangeMode RangeMode {
            get { return _rangeMode; }
            set { _rangeMode = value; }
        }
        public bool UseCustomFilter {
            get { return _useCustomFilter; }
            set { _useCustomFilter = value; }
        }
        public object MinValue {
            get {
                if (_items.Count > 0) return _items[0].Value;
                return null;
            }
        }
        public object MaxValue {
            get {
                if (_items.Count > 0) return _items[_items.Count - 1].Value;
                return null;
            }
        }
        public ColumnHeader Column { get { return _column; } }
        public List<ColumnFilterItem> Items { get { return _items; } }
        public ColumnFormat DisplayFormat { get { return _column.Format; } }
        public List<object> Values { get { return _values; } }
        public void addFilter(object value) {
            if (value != null) {
                ColumnFilterItem aCFI = new ColumnFilterItem(this);
                bool find = false;
                aCFI.Value = value;
                find = _values.Contains(value);
                if (!find) {
                    _values.Add(value);
                    aCFI.Selected = true;
                    _items.Add(aCFI);
                    _items.Sort();
                }
            }
        }
        public void reloadFilter(List<object> objs) {
            // Create a temporary list, to remove unused filter item, also adding the new ones.
            List<ColumnFilterItem> _tmpItems = new List<ColumnFilterItem>();
            foreach (object obj in objs) {
                ColumnFilterItem anItem = new ColumnFilterItem(this, obj);
                _tmpItems.Add(anItem);
                if (!_values.Contains(obj)) {
                    _values.Add(obj);
                    ColumnFilterItem aCFI = new ColumnFilterItem(this, obj);
                    aCFI.Selected = true;
                    _items.Add(aCFI);
                }
            }
            // Remove filter item(s) that does not exist in the new list.
            int i = 0;
            while (i < _items.Count) {
                if (!_tmpItems.Contains(_items[i])) _items.RemoveAt(i);
                else i++;
            }
            _tmpItems.Clear();
            _items.Sort();
        }
        public bool filterValue(object itemValue) {
            if (!_column.EnableFiltering) return true;
            if (_column.EnableCustomFilter && _useCustomFilter) {
                if (_column.CustomFilter == null) return true;
                return _column.CustomFilter.Invoke(itemValue);
            }
            if (itemValue != null) {
                switch (_column.Format) { 
                    case ColumnFormat.None:
                    case ColumnFormat.Password:
                        string strValue = (string)itemValue;
                        if (_filterMode == FilterMode.ByRange) {
                            switch (_rangeMode) { 
                                case FilterRangeMode.StartsWith:
                                    return strValue.StartsWith((string)_minValue);
                                case FilterRangeMode.EndsWith:
                                    return strValue.EndsWith((string)_minValue);
                                case FilterRangeMode.ContainsAny:
                                case FilterRangeMode.ContainsAll:
                                    string words = (string)_minValue;
                                    string[] strWords = words.Split(new char[] { ',', ' ' });
                                    if (strWords.Length > 0) {
                                        bool result = _rangeMode == FilterRangeMode.ContainsAll;
                                        foreach (string s in strWords) {
                                            if (_rangeMode == FilterRangeMode.ContainsAll) result = result && strValue.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1;
                                            else result = result || strValue.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1;
                                        }
                                        return result;
                                    }
                                    return true;
                                case FilterRangeMode.NotContain:
                                    string nwords = (string)_minValue;
                                    string[] nstrWords = nwords.Split(new char[] { ',', ' ' });
                                    if (nstrWords.Length > 0) {
                                        foreach (string s in nstrWords) {
                                            if (strValue.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1) return false;
                                        }
                                        return true;
                                    }
                                    return true;
                                default:
                                    return true;
                            }
                        } else {
                            foreach (ColumnFilterItem cfi in _items) {
                                if (cfi.Selected) {
                                    if (string.Compare(strValue, (string)cfi.Value, true) == 0) return true;
                                }
                            }
                            return false;
                        }
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
                        double dblValue = (double)itemValue;
                        if (_filterMode == FilterMode.ByRange) {
                            double dblMin = (double)_minValue;
                            double dblMax = (double)_maxValue;
                            if (_rangeMode == FilterRangeMode.Between) return (dblValue >= dblMin && dblValue <= dblMax);
                            else return (dblValue < dblMin || dblValue > dblMax);
                        } else {
                            foreach (ColumnFilterItem cfi in _items) {
                                if (cfi.Selected) {
                                    if ((double)cfi.Value == dblValue) return true;
                                }
                            }
                            return false;
                        }
                    case ColumnFormat.DecimalNumber:
                        int intValue = (int)itemValue;
                        if (_filterMode == FilterMode.ByRange) {
                            int intMin = (int)_minValue;
                            int intMax = (int)_maxValue;
                            if (_rangeMode == FilterRangeMode.Between) return (intValue >= intMin && intValue <= intMax);
                            else return (intValue < intMin || intValue > intMax);
                        } else {
                            foreach (ColumnFilterItem cfi in _items) {
                                if (cfi.Selected) {
                                    if ((int)cfi.Value == intValue) return true;
                                }
                            }
                            return false;
                        }
                    default:
                        DateTime dValue = (DateTime)itemValue;
                        DateTime minDate = (DateTime)_minValue;
                        DateTime maxDate = (DateTime)_maxValue;
                        if (_filterMode == FilterMode.ByRange) {
                            if (_rangeMode == FilterRangeMode.Between) return (dValue >= minDate && dValue <= maxDate);
                            else return (dValue < minDate || dValue > maxDate);
                        } else {
                            foreach (ColumnFilterItem cfi in _items) {
                                if (cfi.Selected) {
                                    if ((DateTime)cfi.Value == dValue) return true;
                                }
                            }
                            return false;
                        }
                }
            }
            return true;
        }
        public ColumnFilterItem getFilterItem(object value) {
            foreach (ColumnFilterItem cfi in _items) {
                if (cfi.Equals(value)) return cfi;
            }
            return null;
        }
    }
    /// <summary>
    /// Control to display filter options on the popup window.
    /// </summary>
    internal class FilterChooser : System.Windows.Forms.Control {
        #region Declarations
        Rectangle _chkRect;
        CheckState _chkState = CheckState.Unchecked;
        bool _hoverChk = false;
        ToolStripDropDown _toolStrip;
        List<ItemHost> _itemHosts = new List<ItemHost>();
        int _startIndex = 0, _endIndex = -1;
        Rectangle _clientBound;
        FilterChooserResult _result = FilterChooserResult.Cancel;
        ItemHost _hoverItem = null;
        ColumnFilterHandle _filterHandle;
        int _top = 0;
        CultureInfo _ci;
        ItemHost _customHost = null;
        List<System.Windows.Forms.Control> _ctrlRanges = new List<System.Windows.Forms.Control>();
        VScrollBar _vscroll = null;
        RadioButton _rdbRange = null;
        RadioButton _rdbValue = null;
        Button _btnOK = null;
        Button _btnCancel = null;
        #endregion
        /// <summary>
        /// Class to control FilterItem operations.
        /// </summary>
        private class ItemHost {
            ColumnFilterItem _item = null;
            FilterChooser _parent;
            bool _checked = false;
            public ItemHost(ColumnFilterItem item, FilterChooser parent) {
                _item = item;
                _parent = parent;
                if (_item != null) _checked = _item.Selected;
            }
            public void drawItem(Graphics g, Rectangle rect, bool hLited, bool enabled) {
                Rectangle txtRect = new Rectangle(rect.X + 20, rect.Y, rect.Width - 20, rect.Height);
                StringFormat txtFormat = new StringFormat();
                if (_item == null) txtRect = rect;
                txtFormat.LineAlignment = StringAlignment.Center;
                txtFormat.Alignment = StringAlignment.Near;
                txtFormat.FormatFlags = StringFormatFlags.NoWrap;
                txtFormat.Trimming = StringTrimming.EllipsisCharacter;
                if (hLited) Ai.Renderer.Button.draw(g, rect, Ai.Renderer.Drawing.ColorTheme.Blue, 2, true, false, false, true);
                if (_checked) {
                    PointF[] chkPoints = new PointF[3];
                    Pen chkPen;
                    chkPoints[0].X = (float)(rect.X + 6);
                    chkPoints[1].X = (float)(rect.X + (20 * 0.4));
                    chkPoints[2].X = (float)(20 - 6);
                    chkPoints[0].Y = (float)(rect.Y + (rect.Height / 2));
                    chkPoints[1].Y = (float)(rect.Y + (rect.Height / 2) + 4);
                    chkPoints[2].Y = (float)(rect.Y + (rect.Height / 2) - 4);
                    if (enabled) chkPen = new Pen(Color.Black, 2);
                    else chkPen = new Pen(Color.Gray, 2);
                    g.DrawLines(chkPen, chkPoints);
                    chkPen.Dispose();
                }
                if (_item != null) g.DrawString(getValueString(_item.Value), _parent.Font, enabled ? Brushes.Black : Brushes.Gray, txtRect, txtFormat);
                else g.DrawString("Custom Filter ...", _parent.Font, enabled ? Brushes.Black : Brushes.Gray, txtRect, txtFormat);
                txtFormat.Dispose();
            }
            public void drawItem(Graphics g, Rectangle rect, bool hLited) { drawItem(g, rect, hLited, true); }
            public void drawItem(Graphics g, Rectangle rect) { drawItem(g, rect, false, true); }
            public bool Checked {
                get { return _checked; }
                set { _checked = value; }
            }
            public ColumnFilterItem Item { get { return _item; } }
            private string getValueString(object value) {
                string result = "";
                switch (_parent._filterHandle.DisplayFormat) { 
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
                        // Convert the value to double
                        try {
                            double dblValue = (double)value;
                            switch (_parent._filterHandle.DisplayFormat) { 
                                case ColumnFormat.Bar:
                                case ColumnFormat.Custom:
                                    result = dblValue.ToString(_parent._filterHandle.Column.CustomFormat, _parent._ci);
                                    break;
                                case ColumnFormat.Currency:
                                    result = dblValue.ToString("C", _parent._ci);
                                    break;
                                case ColumnFormat.Exponential:
                                    result = dblValue.ToString("E", _parent._ci);
                                    break;
                                case ColumnFormat.FixedPoint:
                                    result = dblValue.ToString("F", _parent._ci);
                                    break;
                                case ColumnFormat.General:
                                    result = dblValue.ToString("G", _parent._ci);
                                    break;
                                case ColumnFormat.HexaDecimal:
                                    result = dblValue.ToString("X", _parent._ci);
                                    break;
                                case ColumnFormat.Number:
                                    result = dblValue.ToString("N", _parent._ci);
                                    break;
                                case ColumnFormat.Percent:
                                    result = dblValue.ToString("P", _parent._ci);
                                    break;
                                case ColumnFormat.RoundTrip:
                                    result = dblValue.ToString("R", _parent._ci);
                                    break;
                            }
                        } catch (Exception) { }
                        break;
                    case ColumnFormat.DecimalNumber:
                        // Convert to integer.
                        try {
                            int intValue = (int)value;
                            result = intValue.ToString("D", _parent._ci);
                        } catch (Exception) { }
                        break;
                    default:
                        try {
                            DateTime dtValue = (DateTime)value;
                            switch (_parent._filterHandle.DisplayFormat) { 
                                case ColumnFormat.CustomDateTime:
                                    result = dtValue.ToString(_parent._filterHandle.Column.CustomFormat, _parent._ci);
                                    break;
                                case ColumnFormat.ShortDate:
                                    result = dtValue.ToString("d", _parent._ci);
                                    break;
                                case ColumnFormat.LongDate:
                                    result = dtValue.ToString("D", _parent._ci);
                                    break;
                                case ColumnFormat.FullDateShortTime:
                                    result = dtValue.ToString("f", _parent._ci);
                                    break;
                                case ColumnFormat.FullDateLongTime:
                                    result = dtValue.ToString("F", _parent._ci);
                                    break;
                                case ColumnFormat.GeneralDateShortTime:
                                    result = dtValue.ToString("g", _parent._ci);
                                    break;
                                case ColumnFormat.GeneralDateLongTime:
                                    result = dtValue.ToString("G", _parent._ci);
                                    break;
                                case ColumnFormat.RoundTripDateTime:
                                    result = dtValue.ToString("O", _parent._ci);
                                    break;
                                case ColumnFormat.RFC1123:
                                    result = dtValue.ToString("R", _parent._ci);
                                    break;
                                case ColumnFormat.SortableDateTime:
                                    result = dtValue.ToString("s", _parent._ci);
                                    break;
                                case ColumnFormat.ShortTime:
                                    result = dtValue.ToString("t", _parent._ci);
                                    break;
                                case ColumnFormat.LongTime:
                                    result = dtValue.ToString("T", _parent._ci);
                                    break;
                                case ColumnFormat.UniversalSortableDateTime:
                                    result = dtValue.ToString("u", _parent._ci);
                                    break;
                                case ColumnFormat.UniversalFullDateTime:
                                    result = dtValue.ToString("U", _parent._ci);
                                    break;
                            }
                        } catch (Exception) { }
                        break;
                }
                return result;
            }
        }
        public FilterChooser(ColumnFilterHandle handle, ToolStripDropDown toolStrip, Font font, CultureInfo ci) {
            this.KeyDown += FilterChooser_KeyDown;
            this.MouseDown += FilterChooser_MouseDown;
            this.MouseLeave += FilterChooser_MouseLeave;
            this.MouseMove += FilterChooser_MouseMove;
            this.MouseWheel += FilterChooser_MouseWheel;
            this.Paint += FilterChooser_Paint;
            this.Resize += FilterChooser_Resize;
            _toolStrip = toolStrip;
            _filterHandle = handle;
            _ci = ci;
            _filterHandle.UseCustomFilter = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.BackColor = Ai.Renderer.Popup.BackgroundBrush().Color;
            this.Font = font;
            if (_filterHandle.Items.Count > 2) {
                Label lbFrom, lbTo;
                ComboBox cmb;
                _rdbRange = new RadioButton();
                _rdbRange.Top = 2;
                _rdbRange.Left = 2;
                this.Controls.Add(_rdbRange);
                switch (_filterHandle.DisplayFormat) { 
                    case ColumnFormat.None:
                    case ColumnFormat.Password:
                        _rdbRange.Text = "Words";
                        _top = _rdbRange.Bottom + 2;
                        Label lbMode, lbWord;
                        TextBox txt = new TextBox();
                        cmb = new ComboBox();
                        lbWord = new Label();
                        lbWord.Left = 5;
                        lbWord.Top = _top;
                        this.Controls.Add(lbWord);
                        lbWord.Text = "Words";
                        _ctrlRanges.Add(lbWord);
                        _top = lbWord.Bottom + 2;
                        txt.Text = (string)_filterHandle.MinValueSelected;
                        txt.Top = _top;
                        txt.Left = 5;
                        this.Controls.Add(txt);
                        _ctrlRanges.Add(txt);
                        _top = txt.Bottom + 2;
                        lbMode = new Label();
                        lbMode.Left = 5;
                        lbMode.Top = _top;
                        this.Controls.Add(lbMode);
                        lbMode.Text = "Search by";
                        _ctrlRanges.Add(lbMode);
                        _top = lbMode.Bottom + 2;
                        cmb.Items.Add("Prefix");
                        cmb.Items.Add("Suffix");
                        cmb.Items.Add("All Words (AND)");
                        cmb.Items.Add("Any Words (OR)");
                        cmb.Items.Add("Not Contain");
                        cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmb.Top = _top;
                        cmb.Left = 5;
                        this.Controls.Add(cmb);
                        _ctrlRanges.Add(cmb);
                        try {
                            cmb.SelectedIndex = (int)_filterHandle.RangeMode;
                        } catch (Exception) {
                            cmb.SelectedIndex = 0;
                        }
                        _top = cmb.Bottom + 5;
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
                    case ColumnFormat.DecimalNumber:
                        _rdbRange.Text = "Value range";
                        _top = _rdbRange.Bottom + 2;
                        NumericUpDown nud1, nud2;
                        cmb = new ComboBox();
                        lbFrom = new Label();
                        lbFrom.Left = 5;
                        lbFrom.Top = _top;
                        this.Controls.Add(lbFrom);
                        lbFrom.Text = "From";
                        _ctrlRanges.Add(lbFrom);
                        _top = lbFrom.Bottom + 2;
                        nud1 = new NumericUpDown();
                        nud1.Tag = "Lowest";
                        if (_filterHandle.DisplayFormat == ColumnFormat.DecimalNumber) {
                            nud1.DecimalPlaces = 0;
                            nud1.Minimum = (int)_filterHandle.MinValue;
                            nud1.Maximum = (int)_filterHandle.MaxValue;
                            try {
                                nud1.Value = (int)_filterHandle.MinValueSelected;
                            } catch (Exception) {
                                nud1.Value = nud1.Minimum;
                            }
                        } else {
                            nud1.DecimalPlaces = 2;
                            nud1.Minimum = (decimal)_filterHandle.MinValue;
                            nud1.Maximum = (decimal)_filterHandle.MaxValue;
                            try {
                                nud1.Value = (decimal)_filterHandle.MinValueSelected;
                            } catch (Exception) {
                                nud1.Value = nud1.Minimum;
                            }
                        }
                        nud1.Top = _top;
                        nud1.Left = 5;
                        nud1.TextAlign = HorizontalAlignment.Right;
                        nud1.ValueChanged += nud_ValueChanged;
                        this.Controls.Add(nud1);
                        _top = nud1.Bottom + 2;
                        _ctrlRanges.Add(nud1);
                        lbTo = new Label();
                        lbTo.Top = _top;
                        lbTo.Left = 5;
                        this.Controls.Add(lbTo);
                        lbTo.Text = "To";
                        _ctrlRanges.Add(lbTo);
                        _top = lbTo.Bottom + 2;
                        nud2 = new NumericUpDown();
                        nud2.Tag = "Highest";
                        nud2.DecimalPlaces = _filterHandle.DisplayFormat == ColumnFormat.DecimalNumber ? 0 : 2;
                        nud2.Minimum = nud1.Minimum;
                        nud2.Maximum = nud1.Maximum;
                        if (_filterHandle.DisplayFormat == ColumnFormat.DecimalNumber) {
                            try {
                                nud2.Value = (int)_filterHandle.MaxValueSelected;
                            } catch (Exception) {
                                nud2.Value = nud2.Maximum;
                            }
                        } else {
                            try {
                                nud2.Value = (decimal)_filterHandle.MaxValueSelected;
                            } catch (Exception) {
                                nud2.Value = nud2.Maximum;
                            }
                        }
                        nud2.Top = _top;
                        nud2.Left = 5;
                        nud2.TextAlign = HorizontalAlignment.Right;
                        nud2.ValueChanged += nud_ValueChanged;
                        this.Controls.Add(nud2);
                        _ctrlRanges.Add(nud2);
                        _top = nud2.Bottom + 5;
                        cmb.Items.Add("Inside Range");
                        cmb.Items.Add("Outside Range");
                        cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmb.Top = _top;
                        cmb.Left = 5;
                        this.Controls.Add(cmb);
                        _ctrlRanges.Add(cmb);
                        try {
                            cmb.SelectedIndex = (int)_filterHandle.RangeMode;
                        } catch (Exception) {
                            cmb.SelectedIndex = 0;
                        }
                        _top = cmb.Bottom + 5;
                        break;
                    default:
                        _rdbRange.Text = "Date / Time range";
                        _top = _rdbRange.Bottom + 2;
                        DateTimePicker dp1, dp2;
                        cmb = new ComboBox();
                        DateTimeFormatInfo dtfi = _ci.DateTimeFormat;
                        lbFrom = new Label();
                        lbFrom.Left = 5;
                        lbFrom.Top = _top;
                        this.Controls.Add(lbFrom);
                        lbFrom.Text = "From";
                        _ctrlRanges.Add(lbFrom);
                        _top = lbFrom.Bottom + 2;
                        dp1 = new DateTimePicker();
                        dp1.Tag = "Lowest";
                        dp1.Format = DateTimePickerFormat.Custom;
                        switch (_filterHandle.DisplayFormat) {
                            case ColumnFormat.CustomDateTime:
                                dp1.CustomFormat = _filterHandle.Column.CustomFormat;
                                break;
                            case ColumnFormat.ShortDate:
                                dp1.CustomFormat = dtfi.ShortDatePattern;
                                break;
                            case ColumnFormat.LongDate:
                                dp1.CustomFormat = dtfi.LongDatePattern;
                                break;
                            case ColumnFormat.FullDateShortTime:
                                dp1.CustomFormat = dtfi.LongDatePattern + " " + dtfi.ShortTimePattern;
                                break;
                            case ColumnFormat.FullDateLongTime:
                                dp1.CustomFormat = dtfi.FullDateTimePattern;
                                break;
                            case ColumnFormat.GeneralDateShortTime:
                                dp1.CustomFormat = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
                                break;
                            case ColumnFormat.GeneralDateLongTime:
                                dp1.CustomFormat = dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;
                                break;
                            case ColumnFormat.RoundTripDateTime:
                                dp1.CustomFormat = "yyyy-MM-dd T HH:mm:ss";
                                break;
                            case ColumnFormat.RFC1123:
                                dp1.CustomFormat = dtfi.RFC1123Pattern;
                                break;
                            case ColumnFormat.SortableDateTime:
                                dp1.CustomFormat = dtfi.SortableDateTimePattern;
                                break;
                            case ColumnFormat.ShortTime:
                                dp1.CustomFormat = dtfi.ShortTimePattern;
                                break;
                            case ColumnFormat.LongTime:
                                dp1.CustomFormat = dtfi.LongTimePattern;
                                break;
                            case ColumnFormat.UniversalSortableDateTime:
                                dp1.CustomFormat = dtfi.UniversalSortableDateTimePattern;
                                break;
                            case ColumnFormat.UniversalFullDateTime:
                                dp1.CustomFormat = dtfi.FullDateTimePattern;
                                break;
                        }
                        dp1.MinDate = (DateTime)_filterHandle.MinValue;
                        dp1.MaxDate = (DateTime)_filterHandle.MaxValue;
                        try {
                            dp1.Value = (DateTime)_filterHandle.MinValueSelected;
                        } catch (Exception) {
                            dp1.Value = dp1.MinDate;
                        }
                        dp1.Top = _top;
                        dp1.Left = 5;
                        dp1.ShowUpDown = true;
                        dp1.ValueChanged += dp_ValueChanged;
                        this.Controls.Add(dp1);
                        _top = dp1.Bottom + 2;
                        _ctrlRanges.Add(dp1);
                        lbTo = new Label();
                        lbTo.Top = _top;
                        lbTo.Left = 5;
                        this.Controls.Add(lbTo);
                        lbTo.Text = "To";
                        _ctrlRanges.Add(lbTo);
                        _top = lbTo.Bottom + 2;
                        dp2 = new DateTimePicker();
                        dp2.Tag = "Highest";
                        dp2.Format = DateTimePickerFormat.Custom;
                        dp2.CustomFormat = dp1.CustomFormat;
                        dp2.MinDate = dp1.MinDate;
                        dp2.MaxDate = dp1.MaxDate;
                        try {
                            dp2.Value = (DateTime)_filterHandle.MaxValueSelected;
                        } catch (Exception) {
                            dp2.Value = dp2.MaxDate;
                        }
                        dp2.Top = _top;
                        dp2.Left = 5;
                        dp2.ShowUpDown = true;
                        dp2.ValueChanged += dp_ValueChanged;
                        this.Controls.Add(dp2);
                        _ctrlRanges.Add(dp2);
                        _top = dp2.Bottom + 5;
                        cmb.Items.Add("Inside Range");
                        cmb.Items.Add("Outside Range");
                        cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmb.Top = _top;
                        cmb.Left = 5;
                        this.Controls.Add(cmb);
                        _ctrlRanges.Add(cmb);
                        try {
                            cmb.SelectedIndex = (int)_filterHandle.RangeMode;
                        } catch (Exception) {
                            cmb.SelectedIndex = 0;
                        }
                        _top = cmb.Bottom + 5;
                        break;
                }
                _rdbValue = new RadioButton();
                _rdbValue.Top = _top;
                _rdbValue.Left = 2;
                this.Controls.Add(_rdbValue);
                _rdbValue.Text = "Values";
                _top = _rdbValue.Bottom + 2;
                if (_filterHandle.FilterMode == FilterMode.ByRange) _rdbRange.Checked = true;
                else _rdbValue.Checked = true;
            } else {
                _top = 2;
            }
            _chkRect = new Rectangle(0, _top, 22, 22);
            _top = _top + 20;
            _vscroll = new VScrollBar();
            _vscroll.LargeChange = 1;
            this.Controls.Add(_vscroll);
            _vscroll.ValueChanged += _vscroll_ValueChanged;
            if (_rdbRange != null) _rdbRange.CheckedChanged += _rdbRange_CheckedChanged;
            if (_rdbValue != null) _rdbValue.CheckedChanged += _rdbValue_CheckedChanged;
            createHosts(true);
        }
        public void createHosts(bool measureHeight) {
            int i;
            ItemHost anHost;
            int iHeight = (int)this.Font.Height + 12;
            _itemHosts.Clear();
            i = 0;
            while (i < _filterHandle.Items.Count) { 
                anHost = new ItemHost(_filterHandle.Items[i], this);
                _itemHosts.Add(anHost);
                i++;
            }
            getEndIndex();
            if (measureHeight) {
                if (_filterHandle.Items.Count > 7) this.Height = (int)((7 * iHeight) + (_top + 1));
                else this.Height = (int)((_filterHandle.Items.Count * iHeight) + (_top + 1));
            }
            if (_startIndex > 0 || _endIndex < _itemHosts.Count - 1) { 
                _vscroll.Visible = true;
                _vscroll.Maximum = (int)(_filterHandle.Items.Count - ((_endIndex - _startIndex) + 1));
                _clientBound = new Rectangle(1, _top, (int)(this.Width - (_vscroll.Width + 2)), _vscroll.Height);
            } else { 
                _vscroll.Visible = false;
                _clientBound = new Rectangle(1, _top, this.Width - 2, _vscroll.Height);
            }
            _btnOK = new Button();
            _btnOK.Text = "OK";
            _btnOK.Top = _clientBound.Bottom + 3;
            this.Controls.Add(_btnOK);
            _ctrlRanges.Add(_btnOK);
            _btnCancel = new Button();
            _btnCancel.Text = "Cancel";
            _btnCancel.Top = _clientBound.Bottom + 3;
            this.Controls.Add(_btnCancel);
            _ctrlRanges.Add(_btnCancel);
            if (_btnOK != null) _btnOK.Click += _btnOK_Click;
            if (_btnCancel != null) _btnCancel.Click += _btnCancel_Click;
            if (_filterHandle.Column.EnableCustomFilter) { 
                _customHost = new ItemHost(null, this);
                this.Height += (int)((2 * _btnOK.Height) + 12);
                this.MinimumSize = new Size(200, (int)(_top + (2 * _btnOK.Height) + 42));
            } else { 
                this.Height += _btnOK.Height + 6;
                this.MinimumSize = new Size(200, _top + _btnOK.Height + 36);
            }
            checkCheckedState();
        }
        public void createHosts() { createHosts(false); }
        public ColumnFilterHandle FilterHandle { get { return _filterHandle; } }
        public FilterChooserResult Result { get { return _result; } }
        private void checkCheckedState() {
            int chkCount = 0;
            foreach (ItemHost ih in _itemHosts) {
                if (ih.Checked) chkCount++;
            }
            if (chkCount == 0) {
                _chkState = CheckState.Unchecked;
            } else {
                if (chkCount == _itemHosts.Count) _chkState = CheckState.Checked;
                else _chkState = CheckState.Indeterminate;
            }
        }
        private void getEndIndex() {
            int i;
            bool quit;
            int visArea = 0;
            int iHeight = (int)(this.Font.Height + 12);
            i = _startIndex;
            quit = i >= _itemHosts.Count;
            while (!quit) { 
                visArea = visArea + iHeight;
                if (i < _itemHosts.Count - 1) quit = visArea + iHeight > _clientBound.Height;
                i++;
                if (!quit) quit = i >= _itemHosts.Count;
            }
            _endIndex = i - 1;
            if (_endIndex == _itemHosts.Count - 1) {
                // Try to move starting index
                i = _startIndex - 1;
                quit = i < 0;
                while (!quit) {
                    if (visArea + iHeight < _clientBound.Height) {
                        quit = visArea + iHeight > _clientBound.Height;
                        _startIndex = i;
                        if (i > 0) {
                            i = i - 1;
                            visArea = visArea + iHeight;
                        } else {
                            quit = true;
                        }
                    } else {
                        quit = true;
                    }
                }
            }
        }
        #region Event Handlers
        private void FilterChooser_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            switch (e.KeyData) { 
                case Keys.Escape:
                    _result = FilterChooserResult.Cancel;
                    _toolStrip.Close();
                    break;
                case Keys.Return:
                    if (_btnOK != null) {
                        if (_btnOK.Enabled) {
                            _btnOK_Click(this, new EventArgs());
                            return;
                        }
                    }
                    _toolStrip.Close();
                    break;
                default:
                    if (_rdbRange != null) {
                        if (_rdbRange.Checked) return;
                    }
                    switch (e.KeyData) { 
                        case Keys.Space:
                            if (this._hoverItem != null) {
                                this._hoverItem.Checked = !this._hoverItem.Checked;
                                checkCheckedState();
                                this.Invalidate();
                            }
                            break;
                        case Keys.Up: 
                        case Keys.Down:
                            int currentIndex;
                            if (e.KeyData == Keys.Up) {
                                if (this._hoverItem != null) {
                                    currentIndex = _itemHosts.IndexOf(this._hoverItem) - 1;
                                    if (currentIndex < 0) return;
                                    this._hoverItem = _itemHosts[currentIndex];
                                } else {
                                    this._hoverItem = _itemHosts[_itemHosts.Count - 1];
                                    currentIndex = _itemHosts.Count - 1;
                                }
                            } else {
                                if (this._hoverItem != null) {
                                    currentIndex = _itemHosts.IndexOf(this._hoverItem) + 1;
                                    if (currentIndex >= _itemHosts.Count) return;
                                    this._hoverItem = _itemHosts[currentIndex];
                                } else {
                                    this._hoverItem = _itemHosts[0];
                                    currentIndex = 0;
                                }
                            }
                            if (_vscroll.Visible) {
                                if (currentIndex >= _startIndex && currentIndex <= _endIndex) {
                                    this.Invalidate();
                                } else {
                                    if (currentIndex < _startIndex) _vscroll.Value = currentIndex;
                                    else _vscroll.Value = _startIndex + (currentIndex - _endIndex);
                                }
                            } else {
                                this.Invalidate();
                            }
                            break;
                    }
                    break;
            }
        }
        private void FilterChooser_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (_customHost != null) {
                if (_hoverItem == _customHost) {
                    _result = FilterChooserResult.Custom;
                    _filterHandle.UseCustomFilter = true;
                    _toolStrip.Close();
                    return;
                }
            }
            if (_rdbValue == null) return;
            if (_rdbValue.Checked) {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    if (this._hoverItem != null) {
                        this._hoverItem.Checked = !this._hoverItem.Checked;
                        checkCheckedState();
                        this.Invalidate();
                    }
                    if (_hoverChk) {
                        switch (_chkState) {
                            case CheckState.Checked:
                                _chkState = CheckState.Unchecked;
                                foreach (ColumnFilterItem fi in _filterHandle.Items) fi.Selected = false;
                                break;
                            case CheckState.Unchecked:
                            case CheckState.Indeterminate:
                                _chkState = CheckState.Checked;
                                foreach (ColumnFilterItem fi in _filterHandle.Items) fi.Selected = true;
                                break;
                        }
                        this.Invalidate();
                    }
                }
            }
        }
        private void FilterChooser_MouseLeave(object sender, System.EventArgs e) {
            bool changed = false;
            if (this._hoverItem != null) {
                this._hoverItem = null;
                changed = true;
            }
            if (_hoverChk) {
                _hoverChk = false;
                changed = true;
            }
            if (changed) this.Invalidate();
            this.Cursor = Cursors.Default;
        }
        private void FilterChooser_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            int i,lastY;
            Rectangle aRect;
            bool find = false, changed = false;
            int iHeight = (int)(this.Font.Height + 12);
            lastY = _clientBound.Top;
            i = _startIndex;
            while (i <= _endIndex && !find) {
                aRect = new Rectangle(_clientBound.Left, lastY, _clientBound.Width, iHeight);
                if (aRect.Contains(e.X, e.Y)) {
                    find = true;
                    if (this._hoverItem != _itemHosts[i]) {
                        this._hoverItem = _itemHosts[i];
                        changed = true;
                    }
                }
                lastY = lastY + iHeight;
                i++;
            }
            if (_customHost != null) {
                aRect = new Rectangle(_clientBound.X, _btnOK.Bottom + 3, this.Width - 3, iHeight);
                if (aRect.Contains(e.X, e.Y)) {
                    find = true;
                    if (this._hoverItem != _customHost) {
                        this._hoverItem = _customHost;
                        changed = true;
                    }
                }
            }
            if (!find) {
                if (this._hoverItem != null) {
                    this._hoverItem = null;
                    changed = true;
                }
                if (_chkRect.Contains(e.X, e.Y)) {
                    if (!_hoverChk) {
                        _hoverChk = true;
                        changed = true;
                    }
                } else {
                    if (_hoverChk) {
                        _hoverChk = false;
                        changed = true;
                    }
                }
            } else {
                if (_hoverChk) {
                    _hoverChk = false;
                    changed = true;
                }
            }
            if (changed) this.Invalidate();
        }
        private void FilterChooser_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (_rdbValue == null) return;
            if (_rdbValue.Checked) {
                if (_vscroll.Visible) {
                    if (e.Delta > 0) {
                        if (_startIndex > 0) _vscroll.Value -= 1;
                    } else {
                        if (_endIndex < _filterHandle.Items.Count - 1) _vscroll.Value += 1;
                    }
                }
            }
        }
        private void FilterChooser_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            int i, lastY;
            Rectangle aRect, txtRect;
            StringFormat txtFormat;
            Rectangle _splitRect = new Rectangle(0, this.Height - 10, this.Width, 10);
            int iHeight = (int)(this.Font.Height + 12);
            bool itemsEnabled = true;
            if (_rdbValue != null) itemsEnabled = _rdbValue.Checked;
            txtFormat = new StringFormat();
            txtFormat.FormatFlags = StringFormatFlags.NoWrap;
            txtFormat.LineAlignment = StringAlignment.Center;
            txtFormat.Alignment = StringAlignment.Near;
            txtRect = new Rectangle(20, _top - 20, this.Width - 23, 20);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.FillRectangle(Ai.Renderer.Popup.PlacementBrush(), 
                new Rectangle(0, _top - 20, 20, this.Height));
            e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 20, _top, 20, this.Height);
            lastY = _clientBound.Top;
            i = _startIndex;
            while (i <= _endIndex) {
                aRect = new Rectangle(_clientBound.Left, lastY, _clientBound.Width, iHeight);
                _itemHosts[i].drawItem(e.Graphics, aRect, _itemHosts[i] == _hoverItem, itemsEnabled);
                lastY = lastY + iHeight;
                i++;
            }
            e.Graphics.FillRectangle(Ai.Renderer.Popup.SeparatorBrush(), 
                new Rectangle(0, _top - 20, this.Width, 20));
            e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 0, _top, this.Width, _top);
            if (itemsEnabled) e.Graphics.DrawString(_filterHandle.Column.Text, this.Font, Brushes.Black, txtRect, txtFormat);
            else e.Graphics.DrawString(_filterHandle.Column.Text, this.Font, Brushes.Gray, txtRect, txtFormat);
            aRect = new Rectangle(0, _btnOK.Top - 3, this.Width, this.Height - (_btnOK.Top - 3));
            e.Graphics.FillRectangle(Ai.Renderer.Popup.BackgroundBrush(), aRect);
            e.Graphics.DrawLine(Ai.Renderer.Popup.SeparatorPen(), 0, _btnOK.Top - 3, this.Width, _btnOK.Top - 3);
            if (_customHost != null) {
                aRect = new Rectangle(_clientBound.X, _btnOK.Bottom + 3, this.Width - 3, iHeight);
                _customHost.drawItem(e.Graphics, aRect, _hoverItem == _customHost, true);
            }
            Ai.Renderer.CheckBox.drawCheckBox(e.Graphics, _chkRect, _chkState, 14, itemsEnabled, _hoverChk);
            txtFormat.Dispose();
        }
        private void FilterChooser_Resize(object sender, System.EventArgs e) {
            _vscroll.Top = _top + 1;
            if (_btnOK == null) {
                _vscroll.Height = this.Height - (_top + 1);
            } else {
                if (_customHost == null) {
                    _btnOK.Top = this.Height - (_btnOK.Height + 3);
                    _btnCancel.Top = _btnOK.Top;
                } else {
                    _btnOK.Top = this.Height - ((_btnOK.Height * 2) + 6);
                    _btnCancel.Top = _btnOK.Top;
                }
                _vscroll.Height = (_btnOK.Top - 3) - (_top + 1);
            }
            _vscroll.Left = this.Width - _vscroll.Width;
            _clientBound.Height = _vscroll.Height;
            getEndIndex();
            if (_startIndex > 0 || _endIndex < _itemHosts.Count - 1) {
                _vscroll.Visible = true;
                _vscroll.Maximum = _filterHandle.Items.Count - ((_endIndex - _startIndex) + 1);
                _clientBound = new Rectangle(1, _top, this.Width - (_vscroll.Width + 2), _vscroll.Height);
            } else {
                _vscroll.Visible = false;
                _clientBound = new Rectangle(1, _top, this.Width - 2, _vscroll.Height);
            }
            if (_ctrlRanges.Count > 0) {
                foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                    if (c.GetType() == typeof(Button)) {
                        if (c == _btnCancel) c.Left = this.Width - (c.Width + 5);
                        else c.Left = this.Width - ((c.Width * 2) + 10);
                    } else if (c.GetType() == typeof(TextBox)) {
                        c.Width = this.Width - 10;
                    } else if (c.GetType() == typeof(ComboBox)) {
                        c.Width = this.Width - 10;
                    } else if (c.GetType() == typeof(NumericUpDown)) {
                        c.Width = this.Width - 10;
                    } else if (c.GetType() == typeof(DateTimePicker)) {
                        c.Width = this.Width - 10;
                    }
                }
            }
        }
        private void _vscroll_ValueChanged(object sender, System.EventArgs e) {
            _startIndex = _vscroll.Value;
            getEndIndex();
            this.Invalidate();
        }
        private void _rdbRange_CheckedChanged(object sender, System.EventArgs e) {
            foreach (System.Windows.Forms.Control c in _ctrlRanges) c.Enabled = _rdbRange.Checked;
            if (_vscroll != null) _vscroll.Enabled = _rdbValue.Checked;
            this.Invalidate();
        }
        private void _rdbValue_CheckedChanged(object sender, System.EventArgs e) {
            foreach (System.Windows.Forms.Control c in _ctrlRanges) c.Enabled = _rdbRange.Checked;
            if (_vscroll != null) _vscroll.Enabled = _rdbValue.Checked;
            this.Invalidate();
        }
        private void _btnOK_Click(object sender, System.EventArgs e) {
            _result = FilterChooserResult.OK;
            if (_rdbRange == null) {
                _filterHandle.FilterMode = FilterMode.ByValue;
                foreach (ItemHost iHost in _itemHosts) iHost.Item.Selected = iHost.Checked;
                _toolStrip.Close();
                return;
            }
            if (_rdbRange.Checked) {
                ComboBox cmb;
                _filterHandle.FilterMode = FilterMode.ByRange;
                switch (_filterHandle.DisplayFormat) {
                    case ColumnFormat.None:
                    case ColumnFormat.Password:
                        foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                            if (c.GetType() == typeof(TextBox)) {
                                TextBox txt = (TextBox)c;
                                _filterHandle.MinValueSelected = txt.Text;
                            } else if (c.GetType() == typeof(ComboBox)) {
                                cmb = (ComboBox)c;
                                _filterHandle.RangeMode = (FilterRangeMode)cmb.SelectedIndex;
                            }
                        }
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
                    case ColumnFormat.DecimalNumber:
                        foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                            if (c.GetType() == typeof(NumericUpDown)) {
                                NumericUpDown nud = (NumericUpDown)c;
                                if (nud.Tag == "Lowest") _filterHandle.MinValueSelected = nud.Value;
                                else _filterHandle.MaxValueSelected = nud.Value;
                            } else if (c.GetType() == typeof(ComboBox)) {
                                cmb = (ComboBox)c;
                                _filterHandle.RangeMode = (FilterRangeMode)cmb.SelectedIndex;
                            }
                        }
                        break;
                    default:
                        foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                            if (c.GetType() == typeof(DateTimePicker)) {
                                DateTimePicker dp = (DateTimePicker)c;
                                if (dp.Tag == "Lowest") _filterHandle.MinValueSelected = dp.Value;
                                else _filterHandle.MaxValueSelected = dp.Value;
                            } else if (c.GetType() == typeof(ComboBox)) {
                                cmb = (ComboBox)c;
                                _filterHandle.RangeMode = (FilterRangeMode)cmb.SelectedIndex;
                            }
                        }
                        break;
                }
                _toolStrip.Close();
            } else {
                _filterHandle.FilterMode = FilterMode.ByValue;
                foreach (ItemHost iHost in _itemHosts) iHost.Item.Selected = iHost.Checked;
                _toolStrip.Close();
            }
        }
        private void nud_ValueChanged(object sender, EventArgs e) {
            NumericUpDown nud1 = (NumericUpDown)sender;
            NumericUpDown nud2 = null;
            foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                if (c.GetType() == typeof(NumericUpDown)) {
                    if (nud1.Tag == "Lowest") {
                        if (c.Tag == "Highest") {
                            nud2 = (NumericUpDown)c;
                            break;
                        }
                    } else {
                        if (c.Tag == "Lowest") {
                            nud2 = (NumericUpDown)c;
                            break;
                        }
                    }
                }
            }
            if (nud2 != null) {
                if (nud1.Tag == "Lowest") {
                    if (nud2.Value < nud1.Value) nud2.Value = nud1.Value;
                } else {
                    if (nud2.Value > nud1.Value) nud2.Value = nud1.Value;
                }
            }
        }
        private void dp_ValueChanged(object sender, EventArgs e) {
            DateTimePicker dp1 = (DateTimePicker)sender;
            DateTimePicker dp2 = null;
            foreach (System.Windows.Forms.Control c in _ctrlRanges) {
                if (c.GetType() == typeof(NumericUpDown)) {
                    if (dp1.Tag == "Lowest") {
                        if (c.Tag == "Highest") {
                            dp2 = (DateTimePicker)c;
                            break;
                        }
                    } else {
                        if (c.Tag == "Lowest") {
                            dp2 = (DateTimePicker)c;
                            break;
                        }
                    }
                }
            }
            if (dp2 != null) {
                if (dp1.Tag == "Lowest") {
                    if (dp2.Value < dp1.Value) dp2.Value = dp1.Value;
                } else {
                    if (dp2.Value > dp1.Value) dp2.Value = dp1.Value;
                }
            }
        }
        protected override bool IsInputKey(System.Windows.Forms.Keys keyData) {
            switch (keyData) {
                case Keys.Up:
                case Keys.Down:
                case Keys.Space:
                case Keys.Return:
                case Keys.Escape:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
        private void _btnCancel_Click(object sender, System.EventArgs e) {
            _result = FilterChooserResult.Cancel;
            _toolStrip.Close();
        }
        #endregion
    }
    #endregion
}