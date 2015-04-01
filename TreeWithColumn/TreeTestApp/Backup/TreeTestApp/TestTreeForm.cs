using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TreeTestApp
{
	public partial class TestTreeForm : UserControl
	{
		NodeCollectionUnitTest m_validator;
		public TestTreeForm()
		{
			InitializeComponent();
			m_validator = new NodeCollectionUnitTest(m_tree.Nodes);

			Bitmap bmp = new Bitmap(typeof(Form1), "bitmapstrip.bmp");
			bmp.MakeTransparent(Color.Magenta);

			m_tree.Images = new ImageList();
			m_tree.Images.Images.AddStrip(bmp);
		}
		private void OnFill(object sender, EventArgs e)
		{
			m_tree.BeginUpdate();
			m_validator.FillRandom(2, 100000);
			m_tree.EndUpdate();
		}
		private void OnValidate(object sender, EventArgs e)
		{
			m_tree.BeginUpdate();
			int cnt = 0;
			m_validator.RenumberAllNodes();
			foreach (CommonTools.Node node in m_tree.Nodes)
			{
				m_validator.ValidateNodeCount(node);
				cnt += node.VisibleNodeCount;
			}
			Random random = new Random();
			foreach (CommonTools.Node node in CommonTools.NodeCollection.ForwardNodeIterator(m_tree.Nodes.FirstNode, false))
				node.ImageId = random.Next(0, 5);

			m_tree.EndUpdate();

			if (m_tree.NodesSelection.Count < 20)
			{
				List<CommonTools.Node> nodes = new List<CommonTools.Node>(m_tree.NodesSelection.GetSortedNodes());
				foreach (CommonTools.Node node in nodes)
				{
					Console.WriteLine("node id '{0}',{1},{2},{3}", node.GetId(), node[0], node[1], node[2]);
				}
			}
		}
	}
	public class NodeCollectionUnitTest
	{
		CommonTools.NodeCollection	m_collection;
		List<CommonTools.Node> m_allNodes = new List<CommonTools.Node>();
		List<CommonTools.Node> m_leafNodes = new List<CommonTools.Node>();
		int m_maxDepth = 8;

		public NodeCollectionUnitTest()
		{
			m_collection = new CommonTools.NodeCollection(null);
		}
		public NodeCollectionUnitTest(CommonTools.NodeCollection collection)
		{
			m_collection = collection;
		}
		public void FillRandom(int rootNodeCount, int nodeCount)
		{
			int nextId = m_allNodes.Count + m_leafNodes.Count;
			while (rootNodeCount-- > 0)
			{
				CommonTools.Node node = new CommonTools.Node(new object[2] {nextId.ToString(), null});
				nextId++;
				m_collection.Add(node);
				m_allNodes.Add(node);
			}
			Random random = new Random();
			while (nodeCount-- > 0)
			{
				CommonTools.Node node = new CommonTools.Node(new object[2] {nextId.ToString(), null});
				nextId++;

				// now find random node
				int index = (int)System.Math.Round(random.NextDouble() * (double)m_allNodes.Count-1);
				if (index < 0)
					index = 0;
				if (index >= m_allNodes.Count)
					index = m_allNodes.Count-1;

				m_allNodes[index].Nodes.Add(node);
				m_allNodes[index].Expand();

				int parentCount = 0;
				CommonTools.Node parent = node.Parent;
				while (parent != null)
				{
					parentCount++;
					parent = parent.Parent;
				}

				if (parentCount < m_maxDepth)
					m_allNodes.Add(node);
				else
					m_leafNodes.Add(node);
			}
		}
		public void RemoveRandom(int nodeCount)
		{
			Random random = new Random();
			while (nodeCount-- > 0)
			{
				// now find random node
				int index = (int)System.Math.Round(random.NextDouble() * (double)m_allNodes.Count-1);
				if (index < 0)
					index = 0;
				if (index >= m_allNodes.Count)
					index = m_allNodes.Count-1;
				if (index < 0)
					return;
				CommonTools.Node node = m_allNodes[index];
				node.Owner.Remove(node);
				m_allNodes.Remove(node);
				if (node.HasChildren)
				{
					foreach(CommonTools.Node childnode in node.Nodes)
						m_allNodes.Remove(childnode);
				}
			}
		}
		public void RenumberAllNodes()
		{
			int nameIndex		= m_collection.FieldIndex("name");
			int absoluteIndex	= m_collection.FieldIndex("absoluteIndex");
			int visibleIndex	= m_collection.FieldIndex("visibleIndex");
			int childCountIndex	= m_collection.FieldIndex("childCount");
			int totalChildCountIndex	= m_collection.FieldIndex("totalChildCount");
			int visibleCountIndex		= m_collection.FieldIndex("visibleCount");
			
			int index = 0;
			// total index
			foreach (CommonTools.Node node in CommonTools.NodeCollection.ForwardNodeIterator(m_collection.FirstNode, false))
			{
				node[absoluteIndex] = index++;
			}
			// update total child count - slow slow
			foreach (CommonTools.Node node in CommonTools.NodeCollection.ForwardNodeIterator(m_collection.FirstNode, false))
			{
				CommonTools.Node leafnode = CommonTools.NodeCollection.FindNodesBottomLeaf(node, false);
				int index1 = (int)node[absoluteIndex];
				int index2 = (int)leafnode[absoluteIndex];
				node[totalChildCountIndex] = index2 - index1;
			}
			// visible index
			index = 0;
			foreach (CommonTools.Node node in CommonTools.NodeCollection.ForwardNodeIterator(m_collection.FirstNode, true))
			{
				node[visibleIndex] = index++;
			}
		}
		public bool ValidateNodeCount(CommonTools.Node node)
		{
			int totalcount = 1;
			if (node.HasChildren && node.Expanded)
				totalcount += node.Nodes.slowTotalRowCount(true);
			
			if (totalcount != node.VisibleNodeCount)
			{
			CommonTools.Tracing.WriteLine(0, "Node {0}, VisibleCount = cnt {1}, slow {2}, Validation = {3}",
				node[0].ToString(),
				node.VisibleNodeCount,
				totalcount,
				totalcount == node.VisibleNodeCount);
			}
			return totalcount == node.VisibleNodeCount;
		}

	}
}
