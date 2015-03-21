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
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using OpenCvSharp.Extensions;
//using System.Windows.

namespace GestureRecognitionProject
{
    public partial class Form1 : Form
    {
        private CvCapture capture;
        private DispatcherTimer timer;
        private WriteableBitmap writeBitmap;
        private IplImage src;
        //private List<System.Windows.Media.Color> bitmapColors;
        //private BitmapPalette bitmapPallette;
            //new BitmapPalette(new List<System.Windows.Media.Color>() { new System.Windows.Media.Color() });
 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //bitmapColors = new List<System.Windows.Media.Color>();
            //bitmapColors.Add(System.Windows.Media.Colors.Red);
            //bitmapColors.Add(System.Windows.Media.Colors.Blue);
            //bitmapColors.Add(System.Windows.Media.Colors.Green);

            
            //bitmapPallette = new BitmapPalette(bitmapColors);
            
            
            
            //IplImage img = Cv.LoadImage("C:\\capture.png", LoadMode.GrayScale);
            //Stream inputStream = null;
            //img.ToStream(inputStream, "", new ImageEncodingParam(ImageEncodingID.JpegQuality, 0));
            //pictureBox1.Image = new Bitmap(inputStream);
            ////pictureBox1.Image = 

            ////카메라 지정
            ////제어판 기준인듯?
            //m_cvCap = CvCapture.FromCamera(CaptureDevice.DShow,0);
            //m_cvCap.FrameWidth = 320;
            //m_cvCap.FrameHeight = 240;

            ////타이머 설정
            //timer1.Interval = 20;
            //timer1.Enabled = true;

            // 캠 목록 얻어오기
            foreach (DirectShowLib.DsDevice ds in DirectShowLib.DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice))
            {
                comboBox1.Items.Add(ds.Name);//.Items.Add(ds.Name);
            }

            SetTimer();
        }

        private void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 33);
            timer.Tick += new EventHandler(timer1_Tick);
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            ////카메라에서 프레임 가져온다.
            //m_cvImg = m_cvCap.QueryFrame();
            ////IplImage을 비트맵으로 전환
            ////pictureBox1.Image = m_cvImg.ToBitmap();
            //Stream inputStream = null;
            //m_cvImg.ToStream(inputStream, "", new ImageEncodingParam(ImageEncodingID.JpegQuality, 0));
            //pictureBox1.Image = new Bitmap(inputStream);
            // 카메라의 프레임을 비트맵으로 변환
            using (src = capture.QueryFrame())//RetrieveFrame())//e.QueryFrame())
            {
                WriteableBitmapConverter.ToWriteableBitmap(src, writeBitmap);
                MemoryStream inputStream = new MemoryStream();
                writeBitmap.ToIplImage().ToStream(inputStream, ".jpeg", new ImageEncodingParam(ImageEncodingID.JpegQuality, 1));
                pictureBox1.Image = new Bitmap(inputStream);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (capture != null)
            {
                capture.Dispose();
            }

            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //initCamera(camList.SelectedIndex);
            try
            {
                // 해당 카메라 가져오기
                capture = CvCapture.FromCamera(0);//CaptureDevice.DShow, comboBox1.SelectedIndex);
                // 이미지에 비트맵 연결
                writeBitmap = new WriteableBitmap(capture.FrameWidth, capture.FrameHeight, 96, 96, PixelFormats.Rgb24, null);// BitmapPalettes.WebPaletteTransparent);
                
                //writeBitmap = new WriteableBitmap(capture.FrameWidth, capture.FrameHeight, 96, 96, PixelFormats.Bgra32, null);
                
                //pictureBox1.Image = writeBitmap.ToIplImage();
            }
            catch (Exception exception)
            {
                //if (timer != null)
                //{
                //    timer.Stop();
                //}

                if (capture != null)
                {
                    capture.Dispose();
                    capture = null;
                }
                MessageBox.Show(exception.ToString());
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (capture != null && !timer.IsEnabled)
            {
                timer.IsEnabled = true;
                timer.Start();
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                timer.IsEnabled = false;
                timer.Stop();
            }
        }
    }
}
