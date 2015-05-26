using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace HandGesture
{
    delegate void updateDelegate();

    /// <summary>
    /// Webcam이 보고있는 것이 무엇인지 알고싶어요?? 나를 써봐요
    /// </summary>
    public class WebcamController : Base.Singletone<WebcamController>, Base.ISingleTon
    {
        private IplImage m_cvImg;
        private CvCapture m_cvCap;
        private updateDelegate m_updateDel;
        private CvSize frameSize;

        public CvSize FrameSize
        {
            get
            {
                return frameSize;
            }
        }

        public IplImage WebcamImage
        {
            get { return m_cvImg!=null?m_cvImg.Clone():null; }
        }

        private int totalFrame = 0;

        public void Init()
        {
#if PCVer
            m_cvCap = CvCapture.FromFile("hand3.avi");
            totalFrame = (int)Cv.GetCaptureProperty(m_cvCap, CaptureProperty.FrameCount);
#elif !PCVer
            m_cvCap = CvCapture.FromCamera(CaptureDevice.DShow, 0);
#endif
            
            m_cvCap.SetCaptureProperty(CaptureProperty.FrameWidth, 320);
            m_cvCap.SetCaptureProperty(CaptureProperty.FrameHeight, 240);

            frameSize.Height = (int)Cv.GetCaptureProperty(m_cvCap, CaptureProperty.FrameHeight);
            frameSize.Width = (int)Cv.GetCaptureProperty(m_cvCap, CaptureProperty.FrameWidth);
            m_updateDel = null;
        }

        public WebcamController()
        {
            //Init();
        }

        /// <summary>
        /// 현제 프레임을 IplImage형식으로 반환한다.
        /// </summary>
        /// <returns>IplImage</returns>
        public IplImage getImg()
        {
            return m_cvImg;
        }

        /// <summary>
        /// 현제 프레임을 Bitmap형식으로 반환한다.
        /// </summary>
        /// <returns>Bitmap</returns>
        public Bitmap getFrameAsBMP()
        {
            return m_cvImg.ToBitmap();
        }

        /// <summary>
        /// 수동으로 현재 프레임을 업데이트 한다.
        /// </summary>
        public void updateFrame()
        {
#if PCVer
            m_cvImg = Cv.QueryFrame(m_cvCap);
            int curFrame = m_cvCap.PosFrames;
            if (curFrame == totalFrame)
            {
                m_cvCap = Cv.CreateFileCapture("hand3.avi");
                curFrame = 0;
            }
#elif !PCVer
            lock (m_cvCap)
            {
                m_cvImg = m_cvCap.QueryFrame();
                if (m_updateDel != null) m_updateDel();
            }
#endif
            if (m_updateDel != null) m_updateDel();
        }

        public IplImage GetCurQueryFrameImg()
        {
            return m_cvCap.QueryFrame();
        }

#if DEBUG
        CvFont font;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public void addDisplayFPS()
        {
            font = new CvFont(FontFace.HersheyComplexSmall, 1.0, 1.0);
            sw.Reset();
            sw.Start();
            m_updateDel += displayFPS;
        }
        public void subDisplayFPS()
        {
            m_updateDel -= displayFPS;
        }

        private void displayFPS()
        {
            if (sw.ElapsedMilliseconds <= 0) return;
            sw.Stop();
            displayString( (1000 / sw.ElapsedMilliseconds).ToString() , 5, 10);
            sw.Reset();
            sw.Start();
        }

        private void displayString(String str, int xPos, int yPos)
        {
            if(m_cvImg != null)
                m_cvImg.PutText(str, new CvPoint(10, 20), font, new CvScalar(255, 255, 255));
        }
#endif
    }
}
