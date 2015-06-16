using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HandGesture
{
    public partial class DebugForm : Form
    {
        public bool isDrawing;
        public DebugForm()
        {
            isDrawing = false;
            sw.Reset();
            sw.Start();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            InitializeComponent();
        }


        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public void updateFPS()
        {
            if (sw.ElapsedMilliseconds <= 0) return;
            sw.Stop();
            label1.Text = (1000 / sw.ElapsedMilliseconds).ToString();
            sw.Reset();
            sw.Start();
        }
        public void updateState()
        {
            label2.Text = DetectorManager.Instance.state; //"state";
        }

        public void updatePicture(Bitmap img)
        {
            pictureBox1.Image = isDrawing?img : null;
        }
    }
}
