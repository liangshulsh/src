namespace TreeTestApp
{
	partial class FolderView
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			CommonTools.TreeListColumn treeListColumn1 = new CommonTools.TreeListColumn("folder", "Folder");
			CommonTools.TreeListColumn treeListColumn2 = new CommonTools.TreeListColumn("createdTime", "Date Created");
			CommonTools.TreeListColumn treeListColumn3 = new CommonTools.TreeListColumn("size", "Size");
			this.m_folderTree = new TreeTestApp.FolderViewTree();
			((System.ComponentModel.ISupportInitialize)(this.m_folderTree)).BeginInit();
			this.SuspendLayout();
			// 
			// m_folderTree
			// 
			this.m_folderTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			treeListColumn1.Width = 250;
			treeListColumn2.CellFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			treeListColumn2.CellFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			treeListColumn2.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			treeListColumn2.Width = 130;
			treeListColumn3.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn3.HeaderFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			treeListColumn3.Width = 100;
			this.m_folderTree.Columns.AddRange(new CommonTools.TreeListColumn[] {
            treeListColumn1,
            treeListColumn2,
            treeListColumn3});
			this.m_folderTree.Images = null;
			this.m_folderTree.Location = new System.Drawing.Point(3, 3);
			this.m_folderTree.Name = "m_folderTree";
			this.m_folderTree.Size = new System.Drawing.Size(471, 326);
			this.m_folderTree.TabIndex = 0;
			this.m_folderTree.Text = "treeListView1";
			this.m_folderTree.ViewOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			// 
			// FolderView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_folderTree);
			this.Name = "FolderView";
			this.Size = new System.Drawing.Size(477, 332);
			((System.ComponentModel.ISupportInitialize)(this.m_folderTree)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private FolderViewTree m_folderTree;
	}
}