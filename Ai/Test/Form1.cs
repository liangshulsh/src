using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Test {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            this.Load += Form_Load;
            mct.ItemDragStart += mct_ItemDragStart;
            mct.ItemDragDrop += mct_ItemDragDrop;
        }
        private void Form_Load(object sender, EventArgs e) {
            int i, j;
            Image img = Image.FromFile("D:\\rainbow.jpg");
            i = 0;
            while (i < 5) {
                Ai.Control.TreeNode pn = new Ai.Control.TreeNode();
                pn.Text = "Parent " + i;
                pn.SubItems.Add("Sub Parent " + i);
                pn.Image = img;
                j = 0;
                while (j < 10) {
                    Ai.Control.TreeNode cn = new Ai.Control.TreeNode();
                    cn.Text = "Child " + j + " from " + i;
                    cn.SubItems.Add("Sub Child " + j);
                    cn.Image = img;
                    pn.Nodes.Add(cn);
                    j++;
                }
                mct.Nodes.Add(pn);
                i++;
            }
        }
        private void mct_ItemDragStart(object sender, Ai.Control.ItemDragDropEventArgs e) {
            if (e.ItemsSource is Ai.Control.MultiColumnTree.SelectedTreeNodeCollection) {
                e.Effects = DragDropEffects.Move;
            } else { 
            }
        }
        private void mct_ItemDragDrop(object sender, Ai.Control.ItemDragDropEventArgs e) {
            if (e.ItemTarget != null && e.ItemsSource is Ai.Control.MultiColumnTree.SelectedTreeNodeCollection) {
                Ai.Control.TreeNode tn = (Ai.Control.TreeNode)e.ItemTarget;
                Ai.Control.MultiColumnTree.SelectedTreeNodeCollection src = (Ai.Control.MultiColumnTree.SelectedTreeNodeCollection)e.ItemsSource;
                foreach (Ai.Control.TreeNode srcTn in src) tn.Nodes.Add(srcTn);
                /*string strMsg = tn.Text;
                foreach (Ai.Control.TreeNode trgTn in tn.Nodes) {
                    if (strMsg != "") strMsg = strMsg + "\n";
                    strMsg = strMsg + trgTn.Text;
                }
                MessageBox.Show(strMsg);*/
            }
        }
    }
}