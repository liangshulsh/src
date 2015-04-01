using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ai.Reporting.Core {
    /// <summary>
    /// Specifies the positioning method used for the component.
    /// </summary>
    public enum Positioning { 
        /// <summary>
        /// Object is positioned relative to the left top corner of the document.
        /// </summary>
        Absolute,
        /// <summary>
        /// Object is positioned relative to parent element's position.
        /// </summary>
        Relative
    }
    /// <summary>
    /// Specifies the dragging point of the component.
    /// </summary>
    public enum DragPoint { 
        /// <summary>
        /// Component being resized by dragging its top-left corner.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Component being resized by dragging its top-right corner.
        /// </summary>
        TopRight,
        BottomLeft,
        BottomRight,
        Position
    }
    /// <summary>
    /// A Class to represent each single component of a report.
    /// </summary>
    public abstract class Component {
        #region Members
        /// <summary>
        /// Stores the location of the component within its parent.
        /// </summary>
        internal PointF _location;
        /// <summary>
        /// Stores the size of the component.
        /// </summary>
        internal SizeF _size;
        /// <summary>
        /// Stores the method used to locate the component within its parent.
        /// </summary>
        internal Positioning _position;
        /// <summary>
        /// Stores the method how the component will be aligned within its parent.
        /// </summary>
        internal ContentAlignment _alignment;
        /// <summary>
        /// Stores the parent of this component.
        /// </summary>
        internal Component _parent;
        /// <summary>
        /// Stores the name of the component.
        /// </summary>
        internal string _name;
        /// <summary>
        /// Stores the selection status of the component.
        /// </summary>
        internal bool _selected;
        #endregion
        #region Constrcutor
        public Component() {
            _location = new PointF();
            _size = new SizeF();
            _position = Positioning.Relative;
            _alignment = ContentAlignment.TopLeft;
            _name = "";
            _parent = null;
        }
        public Component(Component parent) {
            _location = new PointF();
            _size = new SizeF();
            _position = Positioning.Relative;
            _alignment = ContentAlignment.TopLeft;
            _name = "";
            _parent = parent;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the component relative to the upper-left corner of its parent or document.
        /// </summary>
        public PointF Location {
            get { return _location; }
            set { _location = value; }
        }
        /// <summary>
        /// Gets or sets the height and width of the component.
        /// </summary>
        public SizeF Size {
            get { return _size; }
            set { _size = value; }
        }
        /// <summary>
        /// Gets or sets the relative positioning of the component.
        /// </summary>
        public Positioning Position {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// Gets or sets how component is aligned to the component's parent.
        /// </summary>
        public ContentAlignment Alignment {
            get { return _alignment; }
            set { _alignment = value; }
        }
        /// <summary>
        /// Gets or sets the distance, between the left edge of the component and the left edge of its parent's area.
        /// </summary>
        public float Left {
            get { return _location.X; }
            set { _location.X = value; }
        }
        /// <summary>
        /// Gets or sets the distance, between the top edge of the component and the top edge of its parent's area.
        /// </summary>
        public float Top {
            get { return _location.Y; }
            set { _location.Y = value; }
        }
        /// <summary>
        /// Gets or sets the width of the component.
        /// </summary>
        public float Width {
            get { return _size.Width; }
            set { _size.Width = value; }
        }
        /// <summary>
        /// Gets or sets the height of the component.
        /// </summary>
        public float Height {
            get { return _size.Height; }
            set { _size.Height = value; }
        }
        /// <summary>
        /// Gets or sets selected status of the component, only used in designer.
        /// </summary>
        internal bool Selected {
            get { return _selected; }
            set { _selected = value; }
        }
        #endregion
        #region Abstract Members
        // Drawing
        /// <summary>
        /// Draws the component in design mode.
        /// </summary>
        /// <param name="g">Graphics object where the component will be drawn.</param>
        public abstract void drawDesigner(Graphics g);
        /// <summary>
        /// Draws the component in runtime mode (print preview or print).
        /// </summary>
        /// <param name="g">Graphics object where the component will be drawn.</param>
        public abstract void draw(Graphics g);
        public abstract bool mouseMove(MouseEventArgs e);
        public abstract bool mouseDown(MouseEventArgs e);
        public abstract bool mouseUp(MouseEventArgs e);
        #endregion
    }
}