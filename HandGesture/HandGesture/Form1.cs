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

            ResultBox.Image = detector.ConvertIplToBitmap(detector.extractor(webcamImg));

            pictureBox3.Image = opticalFlow.ConvertIplToBitmap(opticalFlow.OpticalFlow_BM(webcamImg));

            //detector.CheckFeature(ref frame1, Cv.RGB(0, 255, 0));
            //detector.CheckFeature(ref frame2, Cv.RGB(0, 0, 255));

            //pictureBox4.Image = detector.ConvertIplToBitmap(frame1);
            //pictureBox5.Image = detector.ConvertIplToBitmap(frame2);
            //ResultBox.Image = ImageProcessBase.ReturnTracking(ImageProcessBase.extractor(WebcamController.Instance.WebcamImage), ImageProcessBase.GetFaceFeature(WebcamController.Instance.WebcamImage));
            //ResultBox.Image = ImageProcessBase.ConvertToBinaryBMP( WebcamController.m_img );
            //ResultBox.Image = ImageProcessBase.testContoursBMP(WebcamController.m_img);
            //ResultBox.Image = DetectorManager.Instance.GetBitmapImage(GestureType.Point);
            //ResultBox.Image = ImageProcessBase.handDetect(WebcamController.getImg());
            //ResultBox.Image = ImageProcessBase.extractSkinAsBMP(WebcamController.getImg());
            //ResultBox.Image = iRecgnition.ExtractRecognitionImageBitmap();
        }
    }

}
