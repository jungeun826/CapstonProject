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
    public static class WebcamController
    {
        private static IplImage m_cvImg;
        private static CvCapture m_cvCap;
        private static updateDelegate m_updateDel;
        private static CvSize frameSize;
        public static CvSize FrameSize
        {
            get
            {
                return frameSize;
            }
        }

        public static IplImage m_img
        {
            get { return m_cvImg!=null?m_cvImg.Clone():null; }
        }


        static WebcamController()
        {
            //카메라 지정
            //0번카메라를 사용한다.
            m_cvCap = CvCapture.FromCamera(0);
            m_cvCap.FrameWidth = 320;
            m_cvCap.FrameHeight = 240;
            
            frameSize.Height = (int)Cv.GetCaptureProperty(m_cvCap, CaptureProperty.FrameHeight);
            frameSize.Width = (int)Cv.GetCaptureProperty(m_cvCap, CaptureProperty.FrameWidth);

            m_updateDel = null;

            updateFrame();
        }

        /// <summary>
        /// 현제 프레임을 IplImage형식으로 반환한다.
        /// </summary>
        /// <returns>IplImage</returns>
        public static IplImage getImg()
        {
            return m_cvImg;
        }

        /// <summary>
        /// 현제 프레임을 Bitmap형식으로 반환한다.
        /// </summary>
        /// <returns>Bitmap</returns>
        public static Bitmap getFrameAsBMP()
        {
            return m_cvImg.ToBitmap();
        }

        /// <summary>
        /// 수동으로 현재 프레임을 업데이트 한다.
        /// </summary>
        public static void updateFrame()
        {
            m_cvImg = m_cvCap.QueryFrame();
            if (m_updateDel != null) m_updateDel();
        }

#if DEBUG
        static CvFont font;
        static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public static void addDisplayFPS()
        {
            font = new CvFont(FontFace.HersheyComplexSmall, 1.0, 1.0);
            sw.Reset();
            sw.Start();
            m_updateDel += displayFPS;
        }
        public static void subDisplayFPS()
        {
            m_updateDel -= displayFPS;
        }

        private static void displayFPS()
        {
            sw.Stop();
            displayString( (1000 / sw.ElapsedMilliseconds).ToString() , 5, 10);
            sw.Reset();
            sw.Start();
        }

        private static void displayString(String str, int xPos, int yPos)
        {
            m_cvImg.PutText(str, new CvPoint(10, 20), font, new CvScalar(255, 255, 255));
        }
#endif
    }
}
