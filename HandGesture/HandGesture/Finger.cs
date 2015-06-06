
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandGesture
{
    public class Finger
    {
        [Flags]
        public enum FingerType
        {
            None,
            ForeFinger,
            IndexFinger,
            MiddleFinger,
            RingFinger,
            LittleFinger,
        }

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

            double angle = GetFingerAngle(p1, p2);
            return angle;
        }

        // 중심점과 손가락 끝점 사이의 거리와, 손의 반지름을 이용해 비율값을 구한다.
        public double GetFingerRatio(OpenCvSharp.CvPoint fingerPoint)
        {
            if (m_rad == 0)
                Debug.Log("Don't Use This Method : m_rad isn't initialize");

            double fingerDist = GetDist(fingerPoint, m_centerPoint);
            return fingerDist / m_rad;
        }

        private double GetFingerAngle(OpenCvSharp.CvPoint p1, OpenCvSharp.CvPoint p2)
        {
            return Math.Acos(p1.DotProduct(p2) / Math.Sqrt((p1.DotProduct(p1) * p2.DotProduct(p2))));
        }

        public FingerType GetFingerType()
        {
            if (m_depthPoint.Count < 1) return FingerType.None;
            double r;
            OpenCvSharp.CvPoint tempPoint = m_centerPoint;
            double angle;
            FingerType type = FingerType.None;
            foreach (OpenCvSharp.CvPoint depthPoint in m_depthPoint)
            {
                r = GetDist(depthPoint, m_centerPoint);
                tempPoint.X = m_centerPoint.X + (int)r;
                angle = GetFingerAngle(depthPoint - m_centerPoint, tempPoint - m_centerPoint);
                if (2.4 < angle && angle < 3.0f)
                    type |= FingerType.ForeFinger;
                else if (2.0 < angle && angle < 1.6f)
                    type |= FingerType.IndexFinger;
                else if ( 1.4 < angle &&angle < 1.2f)
                    type |= FingerType.MiddleFinger;
                else if (1.0 < angle && angle < 0.8f)
                    type |= FingerType.RingFinger;
                else if (0.8 < angle && angle < 0.4f)
                    type |= FingerType.LittleFinger;
            }

            return type;
        }

        public double GetLengthOfIf()
        {
            if (m_tipPoint.Count == 2)
                return m_tipPoint[0].Y > m_tipPoint[1].Y ?
                        m_tipPoint[0].DistanceTo(m_centerPoint)
                        : m_tipPoint[1].DistanceTo(m_centerPoint);

            return 0.0;
        }

        //public double GetFingerAngle2(OpenCvSharp.CvPoint depthPoint, OpenCvSharp.CvPoint centerPoint)
        //{
        //    double r = GetDist(depthPoint, m_centerPoint);
        //    OpenCvSharp.CvPoint tempPoint = m_centerPoint;
        //    tempPoint.X = m_centerPoint.X + (int)r;
        //    return GetFingerAngle(depthPoint - m_centerPoint, tempPoint - m_centerPoint);
        //}

        private double GetDist(OpenCvSharp.CvPoint p1, OpenCvSharp.CvPoint p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

    }
}
