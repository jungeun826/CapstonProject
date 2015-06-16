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

        HandDetector detector;

        public Form1()
        {
            InitializeComponent();

            //button_basic.BackgroundImage = Image.FromFile("BASIC.jpg");
            //button_fps.BackgroundImage = Image.FromFile("FPS.jpg");
            //button_racing.BackgroundImage = Image.FromFile("RACING.jpg");


            DetectorManager.Instance.Init(DetectorMode.Basic);
            DetectorManager.Instance.Init(DetectorMode.FPS);
            DetectorManager.Instance.Init(DetectorMode.Racing);
            detector = DetectorManager.Instance.handDetector;

            //opticalFlow = new OpticalFlow();
            WebcamController.Instance.Init();
            WebcamController.Instance.updateFrame();

           

            Pause = false;

            label1.Text = "BASIC";

            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey((int)this.Handle, 0, 0x0, (int)Keys.F10);
            RegisterHotKey((int)this.Handle, 1, 0x0, (int)Keys.F11);
            RegisterHotKey((int)this.Handle, 2, 0x0, (int)Keys.F9);
            RegisterHotKey((int)this.Handle, 3, 0x0, (int)Keys.F8);
            RegisterHotKey((int)this.Handle, 4, 0x0, (int)Keys.F1);
            RegisterHotKey((int)this.Handle, 5, 0x0, (int)Keys.F2);
            RegisterHotKey((int)this.Handle, 6, 0x0, (int)Keys.F3);
            //타이머 설정
            timer1.Interval = 20;
            timer1.Enabled = true;


            //BasicRadioBtn.Checked = true;

            //트레이아이콘과 연결
            WCC.ContextMenuStrip = contextMenuStrip1;

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

            if (debugForm != null && debugForm.Created)
            {
                debugForm.updateFPS();
                debugForm.updateState();
                DrawImg(detector.ResultImg);
            }

        }

        private void DrawImg(IplImage img1 = null)
        {
            if (img1 != null)
                debugForm.updatePicture( detector.ConvertIplToBitmap(img1));
        }


        private void button_basic_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Basic);
            label1.Text = DetectorManager.Instance.DetectMode.ToString();
        
        }

        private void button_fps_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.FPS);
            label1.Text = DetectorManager.Instance.DetectMode.ToString();
            
        }

        private void button_racing_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Racing);
            label1.Text =  DetectorManager.Instance.DetectMode.ToString();
       
        }


        //라디오버튼을 버튼으로 바꾸었다. 되는지 봐야지..
        //private void FPSRadioBtn_CheckedChanged(object sender, EventArgs e)
        //{
        //    DetectorManager.Instance.ChangeDetectMode(DetectorMode.FPS);
        //    label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        //}

        //private void RacingRadioBtn_CheckedChanged(object sender, EventArgs e)
        //{
        //    DetectorManager.Instance.ChangeDetectMode(DetectorMode.Racing);
        //    label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        //}

        //private void CustomRadioBtn_CheckedChanged(object sender, EventArgs e)
        //{
        //    DetectorManager.Instance.ChangeDetectMode(DetectorMode.Custom);
        //    label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        //}


        ////기본모드 : 마우스 포인팅과 클릭만 함.
        //private void BasicRadioBtn_Click(object sender, EventArgs e)
        //{
        //    DetectorManager.Instance.ChangeDetectMode(DetectorMode.Basic);
        //    label1.Text = ((RadioButton)sender).Name + " / " + DetectorManager.Instance.DetectMode.ToString();
        //}

        //private void PauseBtn_Click(object sender, EventArgs e)
        //{
        //    this.Pause = !this.Pause;
        //    if (!this.Pause)
        //    {
        //        PauseBtn.Text = "StartFrame";
        //    }
        //    else
        //    {
        //        PauseBtn.Text = "StopFrame";
        //    }
        //}
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

                else if (m.WParam == (IntPtr)0x1)
                {
                    Application.Exit();
                }
                else if (m.WParam == (IntPtr)0x2)
                {
                    if(debugForm != null)
                    debugForm.isDrawing = !debugForm.isDrawing;
                }
                else if (m.WParam == (IntPtr)0x3)
                {
                    this.Pause = !this.Pause;
                }
                else if (m.WParam == (IntPtr)0x4)
                {
                    DetectorManager.Instance.ChangeDetectMode(DetectorMode.Basic);
                    label1.Text = DetectorManager.Instance.DetectMode.ToString();
                }
                else if (m.WParam == (IntPtr)0x5)
                {
                    DetectorManager.Instance.ChangeDetectMode(DetectorMode.FPS);
                    label1.Text = DetectorManager.Instance.DetectMode.ToString();
                }
                else if (m.WParam == (IntPtr)0x6)
                {
                    DetectorManager.Instance.ChangeDetectMode(DetectorMode.Racing);
                    label1.Text = DetectorManager.Instance.DetectMode.ToString();
                }

            }
        }

    

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey((int)this.Handle, 0);
            UnregisterHotKey((int)this.Handle, 1);
            UnregisterHotKey((int)this.Handle, 2);
            UnregisterHotKey((int)this.Handle, 3);
            UnregisterHotKey((int)this.Handle, 4);
            UnregisterHotKey((int)this.Handle, 5);
            UnregisterHotKey((int)this.Handle, 6);

            //이거 윤희님이 왜 해두신건지 몰라서 쓰지도 않아서 주석처리 했습니다. By.Yong
            //e.Cancel = true;
            ////방법1
            //this.Visible = false;
            //방법2
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

        }

        //메뉴바 연결
        private void 사용법ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //사용법창을 연결해준다
            Form howToUseForm = new howToUse();
            howToUseForm.Show();
        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //정보창을 연결해준다.
            Form infoForm = new info();
            infoForm.Show();
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            WCC.Visible = false;
        }
        //시스템트레이
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.Visible = false;

                this.WindowState = FormWindowState.Minimized;
                //    this.Hide();
                //    return;
                //
            }
            else
            {
                //방법1
                this.Visible = true;
                //방법2
                //this.WindowState = FormWindowState.Normal;
                //this.ShowInTaskbar = true;

                //    this.Show();
                //    notifyIcon1.Visible = false;
                this.WindowState = FormWindowState.Normal;
                //
            }
        }
        //시스템트레이리스트
        private void open_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            //this.Visible = true;
            this.Show();

        }

        private void BASIC_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Custom);
            label1.Text = DetectorManager.Instance.DetectMode.ToString();
        }

        private void FPS_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.FPS);
            label1.Text = DetectorManager.Instance.DetectMode.ToString();   
        }

        private void RACING_Click(object sender, EventArgs e)
        {
            DetectorManager.Instance.ChangeDetectMode(DetectorMode.Racing);
            label1.Text = DetectorManager.Instance.DetectMode.ToString();
        }

        private void INFO_Click(object sender, EventArgs e)
        {
            //정보창을 연결해준다.
            Form infoForm = new info();
            infoForm.Show();
        }

        private void EXIT_Click(object sender, EventArgs e)
        {
            Application.Exit();
            WCC.Visible = false;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



     
    }
}
