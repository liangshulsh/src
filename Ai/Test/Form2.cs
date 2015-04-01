using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Ai.Renderer;
using Ai.Control;
using System.Runtime.InteropServices;

namespace Test
{
    public partial class Form2 : System.Windows.Forms.Form
    {
        //FloatingToolBox _ft;
        //const int CP_NOCLOSE_BUTTON = 0x200;
        Ai.Control.SkinForm sf;
        Ai.Control.AiSkin skb;
        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;
            sf = new SkinForm();
            skb = new AiSkin();
            //skb.Theme = AiSkin.ColorTheme.BlackBlue;
            // Add a custom button here.
            CustomButton cb = new CustomButton();
            cb.Image = Image.FromFile("D:\\rainbow.jpg"); // Change the image address here.
            cb.Tooltip = "Custom Button : Refresh";
            skb.CustomButtons.Add(cb);
            skb.IsTabbedForm = true;
            // Add tabs here
            int i = 0;
            while (i < 3) {
                Ai.Control.TabFormButton tab = new TabFormButton("Tab " + (i + 1));
                skb.Tabs.Add(tab);
                i++;
            }
            sf.Skin = skb;
            sf.Form = this;
            //button1.Click += _Click;
            /*_ft = new FloatingToolBox(this);
            ToolBoxButton aButton = new ToolBoxButton();
            aButton.Text = "Test";
            aButton.Shortcut = Keys.Alt | Keys.T;
            _ft.Items.Add(aButton);
            aButton.Click += test_Click;*/
            button1.Click += test_Click;
        }
        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }*/
        private void Form2_Load(object sender, EventArgs e) {
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e) {
            /*if (_ft != null) {
                _ft.Dispose();
                _ft = null;
            }*/
        }
        private void _Click(object sender, EventArgs e) {
            /*_ft.X = this.Left;
            _ft.Y = this.Top;
            _ft.show();*/
        }
        private void _Paint(object sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
        }
        private void test_Click(object sender, EventArgs e) {
            Graphics g = this.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(this.BackColor);
            RectangleF rectTxt = new RectangleF(100f, 100f, 200f, 100f);
            g.FillRectangle(Brushes.Blue, rectTxt);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            Ai.Renderer.Drawing.drawTextGlow(g, "ABCDE", this.Font, rectTxt, sf, Color.FromArgb(255, 255, 255), 20, 10);
            g.DrawString("ABCDE", this.Font, Brushes.Black, rectTxt, sf);
            sf.Dispose();
            g.Dispose();
        }
    }
}
