namespace AeroNonClientButtons
{
    partial class MainForm
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
            this.tsNonClientToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.tsSep01 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.lblMe = new System.Windows.Forms.Label();
            this.lblLink = new System.Windows.Forms.LinkLabel();
            this.tsNonClientToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsNonClientToolStrip
            // 
            this.tsNonClientToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.tsNonClientToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsNonClientToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.tsSep01,
            this.btnUndo,
            this.btnRedo});
            this.tsNonClientToolStrip.Location = new System.Drawing.Point(9, 5);
            this.tsNonClientToolStrip.Name = "tsNonClientToolStrip";
            this.tsNonClientToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.tsNonClientToolStrip.Size = new System.Drawing.Size(123, 25);
            this.tsNonClientToolStrip.TabIndex = 0;
            this.tsNonClientToolStrip.Text = "Tools";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = global::AeroOrbTest.Properties.Resources.new16;
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Margin = new System.Windows.Forms.Padding(0);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(23, 25);
            this.btnNew.Text = "New Document";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::AeroOrbTest.Properties.Resources.open16;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 25);
            this.btnOpen.Text = "Open document";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::AeroOrbTest.Properties.Resources.save16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 25);
            this.btnSave.Text = "Save document";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tsSep01
            // 
            this.tsSep01.Name = "tsSep01";
            this.tsSep01.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Image = global::AeroOrbTest.Properties.Resources.undo16;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Margin = new System.Windows.Forms.Padding(0);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 25);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Image = global::AeroOrbTest.Properties.Resources.redo16;
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Margin = new System.Windows.Forms.Padding(0);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 25);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // lblMe
            // 
            this.lblMe.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMe.AutoSize = true;
            this.lblMe.BackColor = System.Drawing.Color.Transparent;
            this.lblMe.Location = new System.Drawing.Point(77, 117);
            this.lblMe.Name = "lblMe";
            this.lblMe.Size = new System.Drawing.Size(169, 13);
            this.lblMe.TabIndex = 1;
            this.lblMe.Text = "2009 Jose Manuel Menendez Poo";
            // 
            // lblLink
            // 
            this.lblLink.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblLink.AutoSize = true;
            this.lblLink.BackColor = System.Drawing.Color.Transparent;
            this.lblLink.Location = new System.Drawing.Point(95, 135);
            this.lblLink.Name = "lblLink";
            this.lblLink.Size = new System.Drawing.Size(124, 13);
            this.lblLink.TabIndex = 2;
            this.lblLink.TabStop = true;
            this.lblLink.Text = "www.menendezpoo.com";
            this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 264);
            this.Controls.Add(this.lblLink);
            this.Controls.Add(this.lblMe);
            this.Controls.Add(this.tsNonClientToolStrip);
            this.Name = "MainForm";
            this.Text = "Aero Non Client Area Buttons";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tsNonClientToolStrip.ResumeLayout(false);
            this.tsNonClientToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsNonClientToolStrip;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripSeparator tsSep01;
        private System.Windows.Forms.Label lblMe;
        private System.Windows.Forms.LinkLabel lblLink;
    }
}

