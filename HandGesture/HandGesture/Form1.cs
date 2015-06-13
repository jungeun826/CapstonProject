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

        public bool Pause { get; set; }

        //DetectorManager detectManager;
        HandGestureDetector detector;
        //FPSGestureDetector fpsDetector;

        //OpticalFlow opticalFlow;

        public Form1()
        {
            InitializeComponent();

            //detector = new HandGestureDetector();
            //fpsDetector = new FPSGestureDetector();

            //DetectorManager.Instance.Init(DetectorMode.Basic, detector);
            //DetectorManager.Instance.Init(DetectorMode.FPS, fpsDetector);

            DetectorManager.Instance.Init(DetectorMode.Basic);
            DetectorManager.Instance.Init(DetectorMode.FPS);
            DetectorManager.Instance.Init(DetectorMode.Racing);
            detector = DetectorManager.Instance.handDetector;

            //opticalFlow = new OpticalFlow();
            WebcamController.Instance.Init();
            WebcamController.Instance.updateFrame();

            ApiController.CallBack = UseApiController;


            Pause = false;
        }

        public void UseApiController(string text)
        {
            pointLabel.Text = text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            WebcamController.Instance.addDisplayFPS();
#endif
            //타이머 설정
            timer1.Interval = 20;
            timer1.Enabled = true;


            BasicRadioBtn.Checked = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (Pause) return;

            WebcamController.Instance.updateFrame();
            DetectorManager.Instance.UpdateManager();
            

            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            if (webcamImg == null)
            {
                return;
            }
            //IplImage webcamImgMask;
            //detector.Mask(webcamImg, out webcamImgMask);

            DrawImg(webcamImg, ImageProcessBase.ROIImg, detector.BlobImg, detector.ConvexHullImg, detector.ResultImg);
            //if (detector.centerPoint.HasValue)
            
            
            string stateText = DetectorManager.Instance.GetCurState();
            if(stateText != DebugLabel.Text)
                DebugLogText.Text += DetectorManager.Instance.GetCurState() + "\n";

            DebugLabel.Text = DetectorManager.Instance.GetCurState();
            

            //pictureBox2.Image = detector.ConvertIplToBitmap(webcamImg);
            //pictureBox1.Image = detector.ExtractRecognitionImageBitmap();

            //IplImage handImg = detector.extractor(webcamImg);
            //ResultBox.Image = detector.ConvertIplToBitmap(handImg);

            //pictureBox3.Image = opticalFlow.ConvertIplToBitmap(opticalFlow.OpticalFlow_LK(handImg));

            //IplImage prevImg = opticalFlow.CheckFeature(opticalFlow.PrevImg, Cv.RGB(0, 255, 0));
            //IplImage curImg = opticalFlow.CheckFeature(opticalFlow.CurImg, Cv.RGB(0, 0, 255));

            //pictureBox4.Image = detector.ConvertIplToBitmap(prevImg);
            //pictureBox5.Image = detector.ConvertIplToBitmap(curImg);
            //ResultBox.Image = ImageProcessBase.ReturnTracking(ImageProcessBase.extractor(WebcamController.Instance.WebcamImage), ImageProcessBase.GetFaceFeature(WebcamController.Instance.WebcamImage));
            //ResultBox.Image = ImageProcessBase.ConvertToBinaryBMP( WebcamController.m_img );
            //ResultBox.Image = ImageProcessBase.testContoursBMP(WebcamController.m_img);
            //ResultBox.Image = DetectorManager.Instance.GetBitmapImage(GestureType.Point);
            //ResultBox.Image = ImageProcessBase.handDetect(WebcamController.getImg());
            //ResultBox.Image = ImageProcessBase.extractSkinAsBMP(WebcamController.getImg());
            //ResultBox.Image = iRecgnition.ExtractRecognitionImageBitmap();
        }

        private void DrawImg(IplImage img1 = null, IplImage img2 = null, IplImage img3 = null, IplImage img4 = null, IplImage img5 = null, IplImage img6 = null)
        {
            if (img1 != null)
                pictureBox1.Image = detector.ConvertIplToBitmap(img1);
            if (img2 != null)
                pictureBox2.Image = detector.ConvertIplToBitmap(img2);
            if (img3 != null)
                pictureBox3.Image = detector.ConvertIplToBitmap(img3);
            if (img4 != null)
                pictureBox4.Image = detector.ConvertIplToBitmap(img4);
            if (img5 != null)
                pictureBox5.Image = detector.ConvertIplToBitmap(img5);
            if (img6 != null)
                pictureBox6.Image = detector.ConvertIplToBitmap(img6);
        }

        private void FPSRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.FPS);
            label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        }

        private void RacingRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Racing);
            label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Right:
                case Keys.Left:
                case Keys.Control | Keys.Down:
                case Keys.Control | Keys.Up:
                case Keys.Control | Keys.Right:
                case Keys.Control | Keys.Left:
                    // 방향키, 혹은 컨트롤키 + 방향키가 입력되었을때
                    // 처리, 혹은 다른 메쏘드 호출을 여기에 적어주시면 됩니다.
                    break;

                case Keys.Control | Keys.X:
                    // 잘라내기 (Ctrl + X)
                    // 처리, 혹은 다른 메쏘드 호출을 여기에 적어주시면 됩니다.
                    break;

                case Keys.Control | Keys.C:
                    // 복사하기 (Ctrl + C)
                    // 처리, 혹은 다른 메쏘드 호출을 여기에 적어주시면 됩니다.
                    break;

                case Keys.Control | Keys.V:
                    // 붙여넣기 (Ctrl + V)
                    // 처리, 혹은 다른 메쏘드 호출을 여기에 적어주시면 됩니다.
                    break;
                case Keys.Control | Keys.Tab:

                default:
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CustomRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Custom);
            label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        }


        //기본모드 : 마우스 포인팅과 클릭만 함.
        private void BasicRadioBtn_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Basic);
            label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            this.Pause = !this.Pause;
            if (!this.Pause)
            {
                PauseBtn.Text = "StartFrame";
            }
            else
            {
                PauseBtn.Text = "StopFrame";
            }
        }

    }

}
