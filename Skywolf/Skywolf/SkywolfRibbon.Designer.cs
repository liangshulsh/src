namespace Skywolf
{
    partial class SkywolfRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public SkywolfRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.skywolfAddin = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.button1 = this.Factory.CreateRibbonButton();
            this.skywolfAddin.SuspendLayout();
            this.group1.SuspendLayout();
            // 
            // skywolfAddin
            // 
            this.skywolfAddin.Groups.Add(this.group1);
            this.skywolfAddin.Label = "Skywolf";
            this.skywolfAddin.Name = "skywolfAddin";
            // 
            // group1
            // 
            this.group1.Items.Add(this.button1);
            this.group1.Label = "group1";
            this.group1.Name = "group1";
            // 
            // button1
            // 
            this.button1.Label = "button1";
            this.button1.Name = "button1";
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButtonClick);
            // 
            // SkywolfRibbon
            // 
            this.Name = "SkywolfRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.skywolfAddin);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.SkywolfRibbon_Load);
            this.skywolfAddin.ResumeLayout(false);
            this.skywolfAddin.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab skywolfAddin;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
    }

    partial class ThisRibbonCollection
    {
        internal SkywolfRibbon SkywolfRibbon
        {
            get { return this.GetRibbon<SkywolfRibbon>(); }
        }
    }
}
