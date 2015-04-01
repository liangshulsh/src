// Ai Software Control Library.
using System;
using System.Windows.Forms;

namespace Ai.Control {
	public class TreeNodeMouseEventArgs : EventArgs {
		TreeNode _node;
		TreeNodeAction _action;
		MouseEventArgs _e;
		public TreeNodeMouseEventArgs(TreeNode node, TreeNodeAction action, MouseEventArgs e) : base() {
			_node = node;
			_action = action;
			_e = e;
		}
		public TreeNode Node { get { return _node; } }
		public TreeNodeAction Action { get { return _action; } }
		public MouseEventArgs MouseEvent { get { return _e; } }
	}
}