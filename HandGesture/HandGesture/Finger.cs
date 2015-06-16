
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandGesture
{
    public class Finger
    {
        public List<OpenCvSharp.CvPoint> m_tipPoint;
        public List<OpenCvSharp.CvPoint> m_depthPoint;
        public OpenCvSharp.CvPoint m_centerPoint;
        public int m_rad;

        public Finger()
        {
            m_tipPoint = new List<OpenCvSharp.CvPoint>();
            m_depthPoint = new List<OpenCvSharp.CvPoint>();
            m_centerPoint = new OpenCvSharp.CvPoint(0, 0);
            m_rad = 0;
        }

        public Finger(OpenCvSharp.CvPoint center, int rad = 0)
        {
            m_tipPoint = new List<OpenCvSharp.CvPoint>();
            m_depthPoint = new List<OpenCvSharp.CvPoint>();
            m_centerPoint = center;
            m_rad = rad;
        }

        public void addTip(OpenCvSharp.CvPoint tipPoint)
        {
            m_tipPoint.Add(tipPoint);
        }

        public void addDepth(OpenCvSharp.CvPoint depthPoint)
        {
            m_depthPoint.Add(depthPoint);
        }

        public double GetFingerAngle()
        {
            if (m_tipPoint.Count < 2) return 0;
            OpenCvSharp.CvPoint p1, p2;
            p1 = m_tipPoint[0] - m_centerPoint;
            p2 = m_tipPoint[1] - m_centerPoint;

            double angle = GetFingerAngle(p1, p2);
            return angle;
        }

        

        private double GetFingerAngle(OpenCvSharp.CvPoint p1, OpenCvSharp.CvPoint p2)
        {
            return Math.Acos(p1.DotProduct(p2) / Math.Sqrt((p1.DotProduct(p1) * p2.DotProduct(p2))));
        }

        public int GetPixelCntYFingerTip(int TipIdx)
        {
            int pixelCnt = -1;
            if(TipIdx < m_tipPoint.Count)
                pixelCnt = Math.Abs(m_tipPoint[TipIdx].Y - m_centerPoint.Y) - m_rad;

            if (pixelCnt <= 0)
                return -1;

            return pixelCnt;
        }

    }
}
