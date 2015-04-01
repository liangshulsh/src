namespace TreeTestApp
{
	partial class TestTreeForm
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
			CommonTools.TreeListColumn treeListColumn1 = new CommonTools.TreeListColumn("name", "Name");
			CommonTools.TreeListColumn treeListColumn2 = new CommonTools.TreeListColumn("absoluteIndex", "Absolute Index");
			CommonTools.TreeListColumn treeListColumn3 = new CommonTools.TreeListColumn("visibleIndex", "Visible Index");
			CommonTools.TreeListColumn treeListColumn4 = new CommonTools.TreeListColumn("childCount", "Child Count");
			CommonTools.TreeListColumn treeListColumn5 = new CommonTools.TreeListColumn("totalChildCount", "Total Child Count");
			CommonTools.TreeListColumn treeListColumn6 = new CommonTools.TreeListColumn("visibleCount", "Visible Count");
			this.m_fill = new System.Windows.Forms.Button();
			this.m_validate = new System.Windows.Forms.Button();
			this.m_tree = new TreeTestApp.TestTree();
			((System.ComponentModel.ISupportInitialize)(this.m_tree)).BeginInit();
			this.SuspendLayout();
			// 
			// m_fill
			// 
			this.m_fill.Location = new System.Drawing.Point(3, 4);
			this.m_fill.Name = "m_fill";
			this.m_fill.Size = new System.Drawing.Size(75, 23);
			this.m_fill.TabIndex = 1;
			this.m_fill.Text = "Fill";
			this.m_fill.UseVisualStyleBackColor = true;
			this.m_fill.Click += new System.EventHandler(this.OnFill);
			// 
			// m_validate
			// 
			this.m_validate.Location = new System.Drawing.Point(84, 4);
			this.m_validate.Name = "m_validate";
			this.m_validate.Size = new System.Drawing.Size(75, 23);
			this.m_validate.TabIndex = 1;
			this.m_validate.Text = "Validate";
			this.m_validate.UseVisualStyleBackColor = true;
			this.m_validate.Click += new System.EventHandler(this.OnValidate);
			// 
			// m_tree
			// 
			this.m_tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			treeListColumn1.AutoSizeMinSize = 100;
			treeListColumn1.Width = 164;
			treeListColumn2.AutoSizeMinSize = 100;
			treeListColumn2.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			treeListColumn2.HeaderFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			treeListColumn2.Width = 100;
			treeListColumn3.AutoSizeMinSize = 0;
			treeListColumn3.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn3.HeaderFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn3.Width = 100;
			treeListColumn4.AutoSizeMinSize = 0;
			treeListColumn4.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn4.HeaderFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			treeListColumn4.Width = 100;
			treeListColumn5.AutoSizeMinSize = 0;
			treeListColumn5.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn5.HeaderFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn5.Width = 100;
			treeListColumn6.AutoSizeMinSize = 0;
			treeListColumn6.CellFormat.ForeColor = System.Drawing.Color.DarkGreen;
			treeListColumn6.CellFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn6.HeaderFormat.ForeColor = System.Drawing.Color.Red;
			treeListColumn6.HeaderFormat.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			treeListColumn6.Width = 100;
			this.m_tree.Columns.AddRange(new CommonTools.TreeListColumn[] {
            treeListColumn1,
            treeListColumn2,
            treeListColumn3,
            treeListColumn4,
            treeListColumn5,
            treeListColumn6});
			this.m_tree.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.m_tree.Images = null;
			this.m_tree.Location = new System.Drawing.Point(3, 33);
			this.m_tree.Name = "m_tree";
			this.m_tree.Size = new System.Drawing.Size(610, 239);
			this.m_tree.TabIndex = 0;
			this.m_tree.ViewOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			// 
			// TestTreeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_validate);
			this.Controls.Add(this.m_fill);
			this.Controls.Add(this.m_tree);
			this.Name = "TestTreeForm";
			this.Size = new System.Drawing.Size(616, 275);
			((System.ComponentModel.ISupportInitialize)(this.m_tree)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private TestTree m_tree;
		private System.Windows.Forms.Button m_fill;
		private System.Windows.Forms.Button m_validate;
	}
}
