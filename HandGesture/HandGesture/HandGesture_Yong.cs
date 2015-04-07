using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace HandGesture
{
    class HandGesture_Yong : IRecognition
    {
        public IplImage m_img, m_skinBinaryImg;
        private IplImage m_retImg;

        public HandGesture_Yong()
        {
            m_img = WebcamController.m_img;
            m_skinBinaryImg = new IplImage(m_img.Width, m_img.Height, BitDepth.U8, 1);
        }

        #region IRecognition은 인터페이스다! 그래서 구현한다!
        public bool Detect()
        {
            return false;
        }
        public IplImage ExtractRecognitionImageIpl()
        {
            this.preProcessing();
            return m_retImg;
        }
        public Bitmap ExtractRecognitionImageBitmap()
        {
            return ExtractRecognitionImageIpl().ToBitmap();
        }
        public bool RecognitionProcessing()
        {
            return false;
        }
        #endregion

        void preProcessing()
        {
            m_img = WebcamController.m_img;

            //CrCb컬러맵을 통해서 피부색부분 바이너리 뭐...그렇다고
            m_img.CvtColor(m_img, ColorConversion.BgrToCrCb);
            m_img.InRangeS(new CvScalar(0, 140, 40), new CvScalar(255, 170, 150), m_skinBinaryImg);

            m_retImg = m_skinBinaryImg;
        }
    }
}
