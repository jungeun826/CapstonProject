
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
            None=0,
            ForeFinger=1,
            IndexFinger=2,
            MiddleFinger=4,
            RingFinger=8,
            LittleFinger=16,
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

        // 중심점과 손가락 끝점 사이의 거리와, 손의 반지름을 이용해 비율값을 구한다.
        public double GetFingerRatio(OpenCvSharp.CvPoint fingerPoint)
        {
            double fingerDist = GetDist(fingerPoint, m_centerPoint);
            return fingerDist / m_rad;
        }

        private double GetFingerAngle(OpenCvSharp.CvPoint p1, OpenCvSharp.CvPoint p2)
        {
            return Math.Acos(p1.DotProduct(p2) / Math.Sqrt((p1.DotProduct(p1) * p2.DotProduct(p2))));
        }

        public int GetFingerTypeTipIdx(FingerType checkType)
        {
            double r;
            OpenCvSharp.CvPoint tempPoint = m_centerPoint;
            double angle;
            OpenCvSharp.CvPoint depthPoint ;
            for (int i = 0; i < m_depthPoint.Count ; i++)
            {
                depthPoint = m_depthPoint[i];
                r = GetDist(depthPoint, m_centerPoint);
                tempPoint.X = m_centerPoint.X + (int)r;
                angle = Math.Atan2(depthPoint.Y - tempPoint.Y, depthPoint.X - tempPoint.X);  //GetFingerAngle(depthPoint - m_centerPoint, tempPoint - m_centerPoint);

                if ((checkType == FingerType.ForeFinger && -3.0f <= angle && angle < -2.6f)
                    ||( checkType == FingerType.IndexFinger && -2.05f < angle && angle < 2f)
                    || (checkType == FingerType.MiddleFinger && -2.6f <= angle && angle < -2.3f)
                    || (checkType == FingerType.RingFinger && -2.3 <= angle && angle < -2.15f)
                    || (checkType == FingerType.LittleFinger && -2.15 <= angle && angle < -2.05f))
                    return i;
            }
            return -1;
        }

        public FingerType GetFingerType()
        {
            if (m_depthPoint.Count < 1) return FingerType.None;
            double r;
            OpenCvSharp.CvPoint tempPoint = m_centerPoint;
            double angle;
            FingerType type = FingerType.None;

#if DEBUG
            int i = 0;
            Dictionary<FingerType, double> angles = new Dictionary<FingerType, double>();
            angles.Add(FingerType.None, 0.0f);
#endif
            foreach (OpenCvSharp.CvPoint depthPoint in m_depthPoint)
            {
                r = GetDist(depthPoint, m_centerPoint);
                tempPoint.X = m_centerPoint.X + (int)r;
                angle = 180 * Math.Atan2(depthPoint.Y - tempPoint.Y, depthPoint.X - tempPoint.X) % 180 ;  //GetFingerAngle(depthPoint - m_centerPoint, tempPoint - m_centerPoint);

                FingerType curType = FingerType.None; 
                if (-3.0f <= angle && angle < -2.6f)
                    curType = FingerType.LittleFinger;
                else if (-2.6f <= angle && angle < -2.3f)
                    curType = FingerType.RingFinger;
                else if ( -2.3 <= angle &&angle < -2.15f)
                    curType = FingerType.MiddleFinger;
                else if (-2.15 <= angle && angle < -1.7f)
                    curType = FingerType.IndexFinger;
                else if (-1.7f <= angle && angle < 2f)
                    curType = FingerType.ForeFinger;
                type |= curType;
#if DEBUG
                if (!angles.ContainsKey(curType))
                    angles.Add(curType, angle);
#endif
            }
#if DEBUG
            string tempText = "";
            string angleText = "";
            foreach (FingerType fingerType in System.Enum.GetValues(typeof(FingerType)))
            {
                if (type.HasFlag(fingerType))
                {
                    angleText += angles[fingerType].ToString() + "/";
                    tempText += fingerType.ToString() + " / ";
                }
            }
            //Debug.Log(m_depthPoint.Count.ToString() + " / " + type.ToString());
#endif
            return type;
        }

        public bool HasFingerType(FingerType checkType)
        {
            Finger.FingerType type = GetFingerType();
            return type.HasFlag(checkType);
        }

        public double GetLengthOfIf()
        {
            if (m_tipPoint.Count == 2)
                return m_tipPoint[0].Y > m_tipPoint[1].Y ?
                        m_tipPoint[0].DistanceTo(m_depthPoint[0])
                        : m_tipPoint[1].DistanceTo(m_depthPoint[1]);

            return 0.0;
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

        public int GetPixelCntXFingerTip(int TipIdx)
        {
            int pixelCnt = -1;
            if (TipIdx < m_tipPoint.Count)
                pixelCnt = Math.Abs(m_tipPoint[TipIdx].X - m_centerPoint.X) - m_rad;

            if (pixelCnt <= 0)
                return -1;

            return pixelCnt;
        }
        
        private double GetDist(OpenCvSharp.CvPoint p1, OpenCvSharp.CvPoint p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

    }
}
