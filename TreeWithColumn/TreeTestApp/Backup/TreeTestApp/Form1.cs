using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TreeTestApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		/*
		void treeListView1_NotifyBeforeExpand(CommonTools.Node node, bool isExpanding)
		{
			if (node.Tag != null && node.Tag.ToString() == "virtual")
			{
				if (node.Nodes.Count == 0 && isExpanding)
				{
					for (int x = 0; x < 10; x++)
					{
						CommonTools.Node tmpn = new CommonTools.Node(DateTime.Now.ToLongTimeString());
						tmpn.Expanded = false;
						tmpn.HasChildren = true;
						tmpn.Tag = "virtual";
						node.Nodes.Add(tmpn);

					}
				}
			}
		}
		 * */
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}
	}
}