// Ai Software Control Library.
using System;

namespace Ai.Control {
	public class ColumnEventArgs : EventArgs {
		ColumnHeader _column;
		public ColumnEventArgs(ColumnHeader column) : base() { _column = column; }
		public ColumnHeader Column { get { return _column; } }
	}
}