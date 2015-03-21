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

        static WebcamController()
        {
            //카메라 지정
            //0번카메라를 사용한다.
            m_cvCap = CvCapture.FromCamera(0);
            m_cvCap.FrameWidth = 320;
            m_cvCap.FrameHeight = 240;

            m_updateDel = null;

#if DEBUG
            addDisplayFPS();
#endif
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
        CvFont font;

        private static void addDisplayFPS()
        {
            //font = new CvFont( )
        }

        private static void displayFPS()
        {
            //TODO FPS표시하기 구현

        }
#endif
    }
}
