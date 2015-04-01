// Ai Software Control Library.
using System;

namespace Ai.Control {
	/// <summary>
	/// Provides data for the ColumnCustomFilter event.
	/// </summary>
	public class ColumnCustomFilterEventArgs : EventArgs {
		ColumnHeader _column;
		bool _cancelFilter = false;
		public ColumnCustomFilterEventArgs(ColumnHeader column) : base() {
			_column = column;
		}
		public bool CancelFilter {
			get { return _cancelFilter; }
			set { _cancelFilter = value; }
		}
		public ColumnHeader Column {
			get { return _column; }
		}
	}
}