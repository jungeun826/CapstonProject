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
        private string playFileName = "";

        public string PlayFileName
        {
            get { return playFileName; }
            set
            {
                playFileName = value;
                m_cvCap = CvCapture.FromFile(value);
            }
        }
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
        /// 수동으로 현재 프레임을 업데이트 한다.
        /// </summary>
        public void updateFrame()
        {
#if PCVer
            m_cvImg = Cv.QueryFrame(m_cvCap);
            int curFrame = m_cvCap.PosFrames;
            if (curFrame == totalFrame)
            {
                m_cvCap = Cv.CreateFileCapture(PlayFileName);
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
    }
}
