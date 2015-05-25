﻿using System;
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
        DetectorMode mode = DetectorMode.Basic;

        //DetectorManager detectManager;
        HandGestureDetector detector;
        OpticalFlow opticalFlow;

        public Form1()
        {
            InitializeComponent();

            detector = new HandGestureDetector();
            DetectorManager.Instance.Init(detector);
            
            opticalFlow = new OpticalFlow();
            WebcamController.Instance.Init();
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


            BasicRadioBtn.Checked = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            WebcamController.Instance.updateFrame();
            DetectorManager.Instance.UpdateManager();

            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            if (webcamImg == null)
            {
                return;
            }
            IplImage webcamImgMask;
            detector.Mask(webcamImg, out webcamImgMask);

            DrawImg(webcamImg, ImageProcessBase.ROIImg, detector.FilterImg, detector.BlobImg, detector.ConvexHullImg, detector.ResultImg);
            if (detector.centerPoint.HasValue)
                DebugLabel.Text = detector.centerPoint.Value.X.ToString() + " " + detector.centerPoint.Value.Y.ToString();
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


        //기본모드 : 마우스 포인팅과 클릭만 함.
        private void BasicRadioBtn_Click(object sender, EventArgs e)
        {
            label1.Text = ((RadioButton)sender).Name;
            DetectorManager.Instance.DetectMode = DetectorMode.Basic;

        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            bool pause = !WebcamController.Instance.Pause;
            WebcamController.Instance.Pause = pause;
            if (pause)
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
