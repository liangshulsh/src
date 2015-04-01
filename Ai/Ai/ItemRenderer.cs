using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ai.Renderer {
    /// <summary>
    /// Represent information the rectangle and the object of the subitem.
    /// </summary>
    public class RectangleInfo {
        Rectangle _rect;
        object _info;
        bool _mouseMoveEffect = true;
        bool _mouseButtonEffect = true;
        public RectangleInfo() { }
        /// <summary>
        /// Gets or sets the rectangle used to placed and draw the object.
        /// </summary>
        public Rectangle Rectangle {
            get { return _rect; }
            set { _rect = value; }
        }
        /// <summary>
        /// Gets or sets the object correspond with the rectangle.
        /// </summary>
        public object Info {
            get { return _info; }
            set { _info = value; }
        }
        /// <summary>
        /// Gets or sets a value whether the object is affected by mouse move event.
        /// </summary>
        public bool MouseMoveEffect {
            get { return _mouseMoveEffect; }
            set { _mouseMoveEffect = value; }
        }
        /// <summary>
        /// Gets or sets a value whether the object is affected by mouse down event.
        /// </summary>
        public bool MouseButtonEffect {
            get { return _mouseButtonEffect; }
            set { _mouseButtonEffect = value; }
        }
    }
    /// <summary>
    /// Represent the information of rectangle(s) used for an item and all objects associated with it.
    /// </summary>
    public sealed class ItemRectangle {
        Rectangle _main;
        Rectangle _sign;
        Rectangle _checkBox;
        bool _relativeToMain = true;
        bool _visible = true;
        List<RectangleInfo> _subItems;
        int _zIndex = 0;
        public ItemRectangle() {
            _subItems = new List<RectangleInfo>();
        }
        /// <summary>
        /// Gets or sets the boundary rectangle for the entire itam.
        /// </summary>
        public Rectangle Main {
            get { return _main; }
            set { _main = value; }
        }
        /// <summary>
        /// Gets or sets the boundary rectangle for the sign of a collapsible / expandable item.
        /// </summary>
        public Rectangle SIgn {
            get { return _sign; }
            set { _sign = value; }
        }
        /// <summary>
        /// Gets or sets the boundary rectangle of the check box of the item.
        /// </summary>
        public Rectangle CheckBox {
            get { return _checkBox; }
            set { _checkBox = value; }
        }
        /// <summary>
        /// Gets or sets a value to determine whether the location of other rectangles is positioned relatively to the main rectangle.
        /// </summary>
        public bool RelativeToMain {
            get { return _relativeToMain; }
            set { _relativeToMain = value; }
        }
        /// <summary>
        /// Gets or sets a value to determine whether the item is shown.
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set { _visible = value; }
        }
        /// <summary>
        /// Gets or sets the Z index of the item.
        /// </summary>
        public int ZIndex {
            get { return _zIndex; }
            set { _zIndex = value; }
        }
        /// <summary>
        /// Gets a list of RectangleInfo object represent the rectangle that used for the subitems.
        /// </summary>
        public List<RectangleInfo> SubItems { get { return _subItems; } }
    }
    /// <summary>
    /// Determine how the item(s) should be positioned.
    /// </summary>
    public enum ItemArrangement {
        /// <summary>
        /// The item(s) is arranged from top to bottom of the container.
        /// </summary>
        Stack,
        /// <summary>
        /// The item(s) is arranged from left to right of the container, and continued to the bottom.
        /// </summary>
        CascadeRight,
        /// <summary>
        /// The item(s) is arranged from top to bottom of the container, and continued to the right.
        /// </summary>
        CascadeBottom,
        /// <summary>
        /// The item(s) is arranged freely by the renderer.
        /// </summary>
        Custom
    }
    /// <summary>
    /// Provides data for item measurement event.
    /// </summary>
    public class ItemMeasureEventArgs : EventArgs {
        object _item;
        ItemRectangle _rectangle;
        List<RectangleInfo> _headersInfo;
        public ItemMeasureEventArgs(object item) : base() {
            _item = item;
        }
        public object Item { get { return _item; } }
        public ItemRectangle Rectangle {
            get { return _rectangle; }
            set { _rectangle = value; }
        }
        public List<RectangleInfo> HeadersInfo {
            get { return _headersInfo; }
            set { _headersInfo = value; }
        }
    }
    /// <summary>
    /// Provides data for item drawing event.
    /// </summary>
    public class ItemDrawEventArgs : EventArgs {
        object _item;
        ItemRectangle _rectangle;
        Graphics _g;
        bool _signHover = false;
        System.Windows.Forms.MouseButtons _signButton = System.Windows.Forms.MouseButtons.None;
        bool _checkHover = false;
        System.Windows.Forms.MouseButtons _checkButton = System.Windows.Forms.MouseButtons.None;
        bool _mainHover = false;
        System.Windows.Forms.MouseButtons _mainButton = System.Windows.Forms.MouseButtons.None;
        public ItemDrawEventArgs(object item) : base() {
            _item = item;
        }
        public ItemRectangle Rectangle {
            get { return _rectangle; }
            set { _rectangle = value; }
        }
        public Graphics Graphics {
            get { return _g; }
            set { _g = value; }
        }
        public bool SignHover {
            get { return _signHover; }
            set { _signHover = value; }
        }
        public System.Windows.Forms.MouseButtons SignButton {
            get { return _signButton; }
            set { _signButton = value; }
        }
        public bool CheckHover {
            get { return _checkHover; }
            set { _checkHover = value; }
        }
        public System.Windows.Forms.MouseButtons CheckButton {
            get { return _checkButton; }
            set { _checkButton = value; }
        }
        public bool MainHover {
            get { return _mainHover; }
            set { _mainHover = value; }
        }
        public System.Windows.Forms.MouseButtons MainButton {
            get { return _mainButton; }
            set { _mainButton = value; }
        }
    }
    /// <summary>
    /// Provides the renderer of an item of a list control.
    /// </summary>
    public abstract class ItemRenderer {
        ItemArrangement _arrangement;
        protected Size _itemSize;
        protected int _horizontalSpacing;
        protected int _verticalSpacing;
        public ItemRenderer() {
            _arrangement = ItemArrangement.Stack;
        }
        #region Public Properties
        /// <summary>
        /// Gets or sets a value to determine how the item(s) should be arranged.
        /// </summary>
        public ItemArrangement Arrangement {
            get { return _arrangement; }
            set { _arrangement = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the size used for the whole items.  This property is not used if the arrangement property is sets to Custom.
        /// </summary>
        public Size ItemSize {
            get { return _itemSize; }
            set { _itemSize = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the width used for the whole items.  This property is not used if the arrangement property is sets to Stack and Custom.
        /// </summary>
        public int ItemWidth {
            get { return _itemSize.Width; }
            set { _itemSize.Width = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the height used for the whole items.  This property is not used if the arrangement property is sets to Custom.
        /// </summary>
        public int ItemHeight {
            get { return _itemSize.Height; }
            set { _itemSize.Height = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the space used for the item horizontally.  This property affects only when the arrangement property is sets to CascadeRight or CascadeBottom.
        /// </summary>
        public int HorizontalSpacing {
            get { return _horizontalSpacing; }
            set { _horizontalSpacing = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the space used for the item vertically.  This property is not used if the arrangement property is sets to Custom.
        /// </summary>
        public int VerticalSpacing {
            get { return _verticalSpacing; }
            set { _verticalSpacing = value; }
        }
        #endregion
        #region Public Functions
        public abstract void measureItem(ItemMeasureEventArgs e);
        public abstract Size measureSubItem(object subItem);
        public abstract void drawItem(ItemDrawEventArgs e);
        #endregion
    }
}