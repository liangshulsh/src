// Ai Software Control Library.
using System;

namespace Ai.Control {
	/// <summary>
	/// Provides data for the events that correspond with ListViewItem class.
	/// </summary>
	public class ItemEventArgs : EventArgs {
		ListViewItem _item;
		bool _cancel = false;
		public ItemEventArgs(ListViewItem item) : base() { _item = item; }
		public ListViewItem Item { get { return _item; } }
		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
}