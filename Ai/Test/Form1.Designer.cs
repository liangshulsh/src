namespace Test
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
            Ai.Control.ColumnHeader columnHeader1 = new Ai.Control.ColumnHeader();
            Ai.Control.ColumnHeader columnHeader2 = new Ai.Control.ColumnHeader();
            this.mct = new Ai.Control.MultiColumnTree();
            this.SuspendLayout();
            // 
            // mct
            // 
            this.mct.AllowDrop = true;
            this.mct.CheckBoxes = true;
            columnHeader1.CustomFilter = null;
            columnHeader1.Tag = null;
            columnHeader1.Width = 150;
            columnHeader2.CustomFilter = null;
            columnHeader2.SizeType = Ai.Control.ColumnSizeType.Fill;
            columnHeader2.Tag = null;
            this.mct.Columns.Add(columnHeader1);
            this.mct.Columns.Add(columnHeader2);
            this.mct.Culture = new System.Globalization.CultureInfo("en-US");
            this.mct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mct.FullRowSelect = true;
            this.mct.Location = new System.Drawing.Point(0, 0);
            this.mct.Name = "mct";
            this.mct.Padding = new System.Windows.Forms.Padding(1);
            this.mct.SelectedNode = null;
            this.mct.Size = new System.Drawing.Size(458, 297);
            this.mct.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 297);
            this.Controls.Add(this.mct);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Ai.Control.MultiColumnTree mct;








    }
}

