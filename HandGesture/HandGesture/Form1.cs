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
//using OpenCvSharp.Extensions;

namespace HandGesture
{
    public partial class Form1 : Form
    {
        IRecognition iRecgnition;

        public Form1()
        {
            InitializeComponent();

            iRecgnition = new HandGesture_Yong();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            WebcamController.addDisplayFPS();
#endif
            //타이머 설정
            timer1.Interval = 20;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            WebcamController.updateFrame();
            
            pictureBox1.Image = WebcamController.getFrameAsBMP();

            ResultBox.Image = ImageProcessBase.extractor(WebcamController.m_img); //.RGBToYCbCr(WebcamController.getImg());
            //ResultBox.Image = ImageProcessBase.ConvertToBinaryBMP( WebcamController.m_img );
            //ResultBox.Image = ImageProcessBase.testContoursBMP(WebcamController.m_img);
            //ResultBox.Image = DetectorManager.Instance.GetBitmapImage(GestureType.Point);
            //ResultBox.Image = ImageProcessBase.handDetect(WebcamController.getImg());
            //ResultBox.Image = ImageProcessBase.extractSkinAsBMP(WebcamController.getImg());
            //ResultBox.Image = iRecgnition.ExtractRecognitionImageBitmap();
        }
    }

}
