// Ai Software Control Library.
using System;

namespace Ai.Control {
	public enum TreeNodeAction {
		Collapse,
		Expand,
		MouseHover,
		MouseLeave,
		LabelEdit,
		MouseDown,
		MouseUp,
		Checked,
		Unknown
	}
	public class TreeNodeEventArgs : EventArgs {
		TreeNode _node;
		TreeNodeAction _action = TreeNodeAction.Unknown;
		bool _cancel = false;
		public TreeNodeEventArgs(TreeNode node, TreeNodeAction action) : base() {
			_node = node;
			_action = action;
		}
		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}
		public TreeNode Node { get { return _node; } }
		public TreeNodeAction Action { get { return _action; } }
	}
}