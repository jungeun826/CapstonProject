
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandGesture
{
    class Finger
    {
        //[Flags]
        //public enum FingerType
        //{
        //    ForeFinger,
        //    IndexFinger,
        //    MiddleFinger,
        //    RingFinger,
        //    LittleFinger,
        //}

        //private int _fingerCnt = 0;
        //public int FingerCnt { get { return _fingerCnt; } }

        //private FingerType _fingerPos;
        //public FingerType FingerPos { get { return _fingerPos; } }

        //public OpenCvSharp.CvPoint FingerDepthPoint;
        //public OpenCvSharp.CvPoint FingerStart;
        //public OpenCvSharp.CvPoint FingerEnd;

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

        public void addTip(OpenCvSharp.CvPoint tipPoint){
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

            double angle = Math.Acos( Math.Abs( p1.DotProduct(p2) / Math.Sqrt( (p1.DotProduct(p1) * p2.DotProduct(p2)))));
            return angle;
        }
        
    }
}
