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
using System.Runtime.InteropServices;
using HandGesture;

namespace HandGesture
{
    public partial class Form1 : Form
    {
        //핫키 등록이랑 제거
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(int hwnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(int hwnd, int id);

        DebugForm debugForm;

        public bool Pause { get; set; }

        HandGestureDetector detector;
        
        public Form1()
        {
            InitializeComponent();

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
            RegisterHotKey((int)this.Handle, 0, 0x0, (int)Keys.F10);
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
            
            DrawImg(detector.ResultImg);
        }

        private void DrawImg(IplImage img1 = null)
        {
            if (img1 != null)
                pictureBox1.Image = detector.ConvertIplToBitmap(img1);
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
        protected override void WndProc(ref Message m) //윈도우프로시저 콜백함수
        {
            base.WndProc(ref m);

            if (m.Msg == (int)0x312) //핫키가 눌러지면 312 정수 메세지를 받게됨
            {
                if (m.WParam == (IntPtr)0x0) // 그 키의 ID가 0이면
                {
                    if (debugForm != null && debugForm.Created)
                    {
                        debugForm.Close();
                    }
                    else
                    {
                        debugForm = new DebugForm();
                        debugForm.Show();
                    }
                }
            }
        }
    }

}
