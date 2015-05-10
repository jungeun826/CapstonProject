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
using HandGesture;

namespace HandGesture
{
    public partial class Form1 : Form
    {
        HandGestureDetector detector;
        OpticalFlow opticalFlow;

        public Form1()
        {
            InitializeComponent();

            detector = new HandGestureDetector();
            opticalFlow = new OpticalFlow();
            WebcamController.Instance.updateFrame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            WebcamController.Instance.addDisplayFPS();
#endif
            //타이머 설정
            timer1.Interval = 20;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            WebcamController.Instance.updateFrame();
            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            if (webcamImg == null)
            {
                return;
            }

            pictureBox2.Image = detector.ConvertIplToBitmap(webcamImg);
            pictureBox1.Image = detector.ConvertIplToBitmap(detector.extractSkinAsIpl(webcamImg));

            IplImage handImg = detector.extractor(webcamImg);
            ResultBox.Image = detector.ConvertIplToBitmap(handImg);

            pictureBox3.Image = opticalFlow.ConvertIplToBitmap(opticalFlow.OpticalFlow_LK(handImg));

            IplImage prevImg = opticalFlow.CheckFeature(opticalFlow.PrevImg, Cv.RGB(0, 255, 0));
            IplImage curImg = opticalFlow.CheckFeature(opticalFlow.CurImg, Cv.RGB(0, 0, 255));

            pictureBox4.Image = detector.ConvertIplToBitmap(prevImg);
            pictureBox5.Image = detector.ConvertIplToBitmap(curImg);
            //ResultBox.Image = ImageProcessBase.ReturnTracking(ImageProcessBase.extractor(WebcamController.Instance.WebcamImage), ImageProcessBase.GetFaceFeature(WebcamController.Instance.WebcamImage));
            //ResultBox.Image = ImageProcessBase.ConvertToBinaryBMP( WebcamController.m_img );
            //ResultBox.Image = ImageProcessBase.testContoursBMP(WebcamController.m_img);
            //ResultBox.Image = DetectorManager.Instance.GetBitmapImage(GestureType.Point);
            //ResultBox.Image = ImageProcessBase.handDetect(WebcamController.getImg());
            //ResultBox.Image = ImageProcessBase.extractSkinAsBMP(WebcamController.getImg());
            //ResultBox.Image = iRecgnition.ExtractRecognitionImageBitmap();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            label1.Text = ((RadioButton)sender).Name;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = ((RadioButton)sender).Name;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = ((RadioButton)sender).Name;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = ((RadioButton)sender).Name;
        }
    }

}
