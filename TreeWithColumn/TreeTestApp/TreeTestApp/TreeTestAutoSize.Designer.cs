namespace TreeTestApp
{
	partial class TreeTestAutoSize
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			CommonTools.TreeListColumn treeListColumn1 = new CommonTools.TreeListColumn("A", "AutoSize (100)");
			CommonTools.TreeListColumn treeListColumn2 = new CommonTools.TreeListColumn("B", "AutoSize (50)");
			CommonTools.TreeListColumn treeListColumn3 = new CommonTools.TreeListColumn("C", "Fixed Size");
			this.m_tree = new TreeTestApp.ResizeTree();
			((System.ComponentModel.ISupportInitialize)(this.m_tree)).BeginInit();
			this.SuspendLayout();
			// 
			// m_tree
			// 
			this.m_tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			treeListColumn1.AutoSize = true;
			treeListColumn1.AutoSizeMinSize = 20;
			treeListColumn1.CellFormat.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			treeListColumn1.Width = 75;
			treeListColumn2.AutoSize = true;
			treeListColumn2.AutoSizeMinSize = 20;
			treeListColumn2.AutoSizeRatio = 50F;
			treeListColumn2.CellFormat.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			treeListColumn2.Width = 75;
			treeListColumn3.AutoSizeMinSize = 0;
			treeListColumn3.CellFormat.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			treeListColumn3.Width = 100;
			this.m_tree.Columns.AddRange(new CommonTools.TreeListColumn[] {
            treeListColumn1,
            treeListColumn2,
            treeListColumn3});
			this.m_tree.ColumnsOptions.HeaderHeight = 40;
			this.m_tree.ColumnsOptions.LeftMargin = 0;
			this.m_tree.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.m_tree.Images = null;
			this.m_tree.Location = new System.Drawing.Point(3, 3);
			this.m_tree.Name = "m_tree";
			this.m_tree.RowOptions.ItemHeight = 20;
			this.m_tree.RowOptions.ShowHeader = false;
			this.m_tree.Size = new System.Drawing.Size(440, 297);
			this.m_tree.TabIndex = 0;
			this.m_tree.Text = "treeListView1";
			this.m_tree.ViewOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_tree.ViewOptions.ShowLine = false;
			this.m_tree.ViewOptions.ShowPlusMinus = false;
			// 
			// TreeTestAutoSize
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_tree);
			this.Name = "TreeTestAutoSize";
			this.Size = new System.Drawing.Size(446, 303);
			((System.ComponentModel.ISupportInitialize)(this.m_tree)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ResizeTree m_tree;
	}
}
