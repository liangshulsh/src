using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TreeTestApp
{
	public partial class TreeTestAutoSize : UserControl
	{
		public TreeTestAutoSize()
		{
			InitializeComponent();
		}
	}

	public class ResizeTree : CommonTools.TreeListView
	{
		class tmp
		{
			int m_a, m_b, m_c;
			public int A
			{
				get { return m_a; }
				set { m_a = value;}
			}
			public int B
			{
				get { return m_b; }
				set { m_b = value;}
			}
			public int C
			{
				get { return m_c; }
				set { m_c = value;}
			}
			public tmp(int a, int b, int c)
			{
				m_a = a;
				m_b = b;
				m_c = c;
			}
		}
		public ResizeTree()
		{
			int cnt = 1;
			for (int x = 0; x < 100; x++)
			{
				CommonTools.Node node = new CommonTools.Node();
				node.Tag = new tmp(cnt++, cnt++, cnt++);
				Nodes.Add(node);
			}
		}
		protected override object GetData(CommonTools.Node node, CommonTools.TreeListColumn column)
		{
			return CommonTools.PropertyUtil.GetPropertyValue(node.Tag, column.Fieldname);
		}
		protected override CommonTools.TreeList.TextFormatting GetFormatting(CommonTools.Node node, CommonTools.TreeListColumn column)
		{
			CommonTools.TreeList.TextFormatting format = new CommonTools.TreeList.TextFormatting(column.CellFormat);
			int testvalue = (node.NodeIndex & 0x01);
			testvalue |= (column.VisibleIndex & 0x01) << 1;
			switch (testvalue)
			{
				case 0:
					format.BackColor = Color.FromArgb(100, Color.Gainsboro);
					break;
				case 1:
					format.BackColor = Color.FromArgb(70, Color.Gainsboro);
					break;
				case 2:
					format.BackColor = Color.FromArgb(100, Color.Gold);
					break;
				case 3:
					format.BackColor = Color.FromArgb(50, Color.Gold);
					break;
			}
			return format;
		}
	}
}
