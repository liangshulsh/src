using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace TreeTestApp
{
	class TestTree : CommonTools.TreeListView
	{
		ContextMenu m_contextMenu = null;

		public TestTree()
		{
			m_contextMenu = new ContextMenu();
			m_contextMenu.MenuItems.Add(new MenuItem("Collapse All Children", new EventHandler(OnCollapseAllChildren)));
			m_contextMenu.MenuItems.Add(new MenuItem("Expand All Children", new EventHandler(OnExpandAllChildren)));
			m_contextMenu.MenuItems.Add(new MenuItem("Delete Selected Node", new EventHandler(OnDeleteSelectedNode)));
		}

		void OnCollapseAllChildren(object sender, EventArgs e)
		{
			BeginUpdate();
			if (MultiSelect && NodesSelection.Count > 0)
			{
				foreach (CommonTools.Node selnode in NodesSelection)
				{
					foreach (CommonTools.Node node in selnode.Nodes)
						node.Collapse();
				}
				NodesSelection.Clear();
			}
			if (FocusedNode != null && FocusedNode.HasChildren)
			{
				foreach (CommonTools.Node node in FocusedNode.Nodes)
					node.Collapse();
			}
			EndUpdate();
		}
		void OnExpandAllChildren(object sender, EventArgs e)
		{
			BeginUpdate();
			if (MultiSelect && NodesSelection.Count > 0)
			{
				foreach (CommonTools.Node selnode in NodesSelection)
					selnode.ExpandAll();
				NodesSelection.Clear();
			}
			if (FocusedNode != null)
				FocusedNode.ExpandAll();
			EndUpdate();
		}
		void OnDeleteSelectedNode(object sender, EventArgs e)
		{
			BeginUpdate();
			CommonTools.Node node = FocusedNode;
			if (node != null && node.Owner != null)
			{
				node.Collapse();
				CommonTools.Node nextnode = CommonTools.NodeCollection.GetNextNode(node, 1);
				if (nextnode == null)
					nextnode = CommonTools.NodeCollection.GetNextNode(node, -1);
				node.Owner.Remove(node);
				FocusedNode = nextnode;
			}
			EndUpdate();
		}
		protected override void BeforeShowContextMenu()
		{
			if (GetHitNode() == null)
				ContextMenu = null;
			else
				ContextMenu = m_contextMenu;
		}
		protected override object GetData(CommonTools.Node node, CommonTools.TreeListColumn column)
		{
			object data = base.GetData(node, column);
			if (data != null)
				return data;

			if (column.Fieldname == "childCount")
			{
				if (node.HasChildren)
					return node.Nodes.Count;
				return "<none>";
			}
			if (column.Fieldname == "visibleCount")
			{
				if (node.HasChildren)
					return node.VisibleNodeCount;
				return "<none>";
			}
			return string.Empty;
		}

	}
}
