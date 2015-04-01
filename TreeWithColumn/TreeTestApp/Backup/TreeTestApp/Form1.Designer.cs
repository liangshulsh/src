namespace TreeTestApp
{
	partial class Form1
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
			System.Windows.Forms.TabPage tabPage1;
			System.Windows.Forms.TabPage tabPage2;
			this.testTreeForm1 = new TreeTestApp.TestTreeForm();
			this.folderView1 = new TreeTestApp.FolderView();
			this.m_tabControl = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.treeTestAutoSize1 = new TreeTestApp.TreeTestAutoSize();
			tabPage1 = new System.Windows.Forms.TabPage();
			tabPage2 = new System.Windows.Forms.TabPage();
			tabPage1.SuspendLayout();
			tabPage2.SuspendLayout();
			this.m_tabControl.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(this.testTreeForm1);
			tabPage1.Location = new System.Drawing.Point(4, 22);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new System.Windows.Forms.Padding(3);
			tabPage1.Size = new System.Drawing.Size(594, 381);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Tree Validation";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// testTreeForm1
			// 
			this.testTreeForm1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.testTreeForm1.Location = new System.Drawing.Point(3, 3);
			this.testTreeForm1.Name = "testTreeForm1";
			this.testTreeForm1.Size = new System.Drawing.Size(588, 375);
			this.testTreeForm1.TabIndex = 3;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(this.folderView1);
			tabPage2.Location = new System.Drawing.Point(4, 22);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new System.Windows.Forms.Padding(3);
			tabPage2.Size = new System.Drawing.Size(594, 381);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Folder View";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// folderView1
			// 
			this.folderView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.folderView1.Location = new System.Drawing.Point(3, 3);
			this.folderView1.Name = "folderView1";
			this.folderView1.Size = new System.Drawing.Size(588, 375);
			this.folderView1.TabIndex = 0;
			// 
			// m_tabControl
			// 
			this.m_tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_tabControl.Controls.Add(tabPage1);
			this.m_tabControl.Controls.Add(tabPage2);
			this.m_tabControl.Controls.Add(this.tabPage3);
			this.m_tabControl.Location = new System.Drawing.Point(4, 4);
			this.m_tabControl.Name = "m_tabControl";
			this.m_tabControl.SelectedIndex = 0;
			this.m_tabControl.Size = new System.Drawing.Size(602, 407);
			this.m_tabControl.TabIndex = 4;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.treeTestAutoSize1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(594, 381);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "AutoSize";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// treeTestAutoSize1
			// 
			this.treeTestAutoSize1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeTestAutoSize1.Location = new System.Drawing.Point(0, 0);
			this.treeTestAutoSize1.Name = "treeTestAutoSize1";
			this.treeTestAutoSize1.Size = new System.Drawing.Size(594, 381);
			this.treeTestAutoSize1.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 416);
			this.Controls.Add(this.m_tabControl);
			this.Name = "Form1";
			this.Text = "Custom Tree (by Jesper Kristiansen)";
			tabPage1.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			this.m_tabControl.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private TestTreeForm testTreeForm1;
		private System.Windows.Forms.TabControl m_tabControl;
		private FolderView folderView1;
		private System.Windows.Forms.TabPage tabPage3;
		private TreeTestAutoSize treeTestAutoSize1;
	}
}

