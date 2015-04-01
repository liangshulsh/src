// Ai Software Control Library.
using System;
using System.Drawing;

namespace Ai.Control {
	/// <summary>
	/// Provides data for the ColumnBackgroundPaint event.
	/// </summary>
	public class ColumnBackgroundPaintEventArgs : EventArgs {
		ColumnHeader _column;
		int _columnIndex;
		Graphics _graphics;
		Rectangle _rectangle;
		public ColumnBackgroundPaintEventArgs(ColumnHeader column, int index, 
			Graphics graphics, Rectangle rectangle) : base() {
			_column = column;
			_columnIndex = index;
			_graphics = graphics;
			_rectangle = rectangle;
		}
		public ColumnHeader Column { get { return _column; } }
		public int ColumnIndex { get { return _columnIndex; } }
		public Graphics Graphics { get { return _graphics; } }
		public Rectangle Rectangle { get { return _rectangle; } }
	}
}