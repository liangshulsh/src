// Ai Software Control Library.
using System;
using System.ComponentModel;

namespace Ai.Control {
	/// <summary>
	/// Provides data for the events that correspond with ListViewGroup class.
	/// </summary>
	public class GroupEventArgs : EventArgs {
		ListViewGroup _group;
		bool _cancel = false;
		public GroupEventArgs(ListViewGroup group) : base() { _group = group; }
		public ListViewGroup Group { get { return _group; } }
		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
}