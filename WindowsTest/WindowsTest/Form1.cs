using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsTest
{
    public partial class Form1 : Form
    {
        internal const int WM_PAINT = 0xF;
        internal const int WM_NCCALCSIZE = 0x0083;

        bool _bResizing = false;

        int _titleHight = 0;
        int _borderWidth = 0;
        Rectangle _rectange;
        bool _rectangeUpdated = false;

        struct NCCALCSIZE_PARAMS
        {
            public _RECT rcNewWindow;
            public _RECT rcOldWindow;
            public _RECT rcClient;
            IntPtr lppos;
        }

        struct _RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public Form1()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            //this.Load += new EventHandler(Form1_Load);
            //this.ResizeBegin += new EventHandler(Form1_ResizeBegin);
            //this.ResizeEnd +=new EventHandler(Form1_ResizeEnd);
            //this.MouseMove += new MouseEventHandler(Form1_MouseMove);
        }

        //void Form1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_bResizing)
        //    {
        //        Invalidate();
        //    }
        //}

        //void Form1_ResizeBegin(object sender, EventArgs e)
        //{
        //    _bResizing = true;
        //}

        void Form1_Load(object sender, EventArgs e)
        {
            
            //ResizeRedraw = true;
        }
        //private void Form1_ResizeEnd(object sender, EventArgs e)
        //{
            
        //    _bResizing = false;
        //    Refresh();
        //}



        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_titleHight == 0)
            {
                e.Graphics.DrawImage(Properties.Resources.Welcome_Scan, e.ClipRectangle);
                Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
                _titleHight = screenRectangle.Top - this.Top;
                _borderWidth = screenRectangle.Left - this.Left;
            }
            
            //base.OnPaint(e);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_NCPAINT = 0x85;
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    base.WndProc(ref m);
                    if (_rectangeUpdated)
                    {
                        IntPtr hdc = GetWindowDC(m.HWnd);
                        if ((int)hdc != 0)
                        {
                            Graphics g = Graphics.FromHdc(hdc);
                             g.DrawImage(Properties.Resources.Welcome_Scan,_rectange);
                             g.DrawLine(new Pen(Color.Blue), new Point(1, 1), new Point(50, 50));
                            g.Flush();
                            ReleaseDC(m.HWnd, hdc);
                            _rectangeUpdated = false;
                        }
                    }
                    break;
                case WM_NCCALCSIZE:
                    {
                        base.WndProc(ref m);
                        if (m.WParam != IntPtr.Zero && _titleHight != 0)
                        {
                            if (m.HWnd == this.Handle)
                            {
                                NCCALCSIZE_PARAMS rcsize = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
                                _rectange = new Rectangle(_borderWidth, _titleHight, rcsize.rcNewWindow.right - rcsize.rcNewWindow.left, rcsize.rcNewWindow.bottom - rcsize.rcNewWindow.top);
                                _rectangeUpdated = true;
                            }

                        }

                        m.Result = new IntPtr(1);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
       

        //private void Form1_Paint(object sender, PaintEventArgs e)
        //{
        //   // e.Graphics.DrawImage(Properties.Resources.Welcome_Scan, new Rectangle(new Point(0, 0), new Size(this.Size.Width + 100, this.Size.Height + 100)));
        //}

        
    }
}
