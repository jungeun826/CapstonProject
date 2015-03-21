using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace HandGesture
{
    public partial class Form1 : Form
    {
        IplImage m_cvImg;
        CvCapture m_cvCap;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //카메라 지정
            //제어판 기준인듯?
            m_cvCap = CvCapture.FromCamera(0);
            m_cvCap.FrameWidth = 3200;
            m_cvCap.FrameHeight = 2400;

            //타이머 설정
            timer1.Interval = 20;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //카메라에서 프레임 가져온다.
            m_cvImg = m_cvCap.QueryFrame();
            //IplImage을 비트맵으로 전환
            pictureBox1.Image = m_cvImg.ToBitmap();
        }
    }

}
