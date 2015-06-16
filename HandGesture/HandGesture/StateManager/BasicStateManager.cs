using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    static class BasicStateManager 
    {
        delegate string fingersDel();

        static List<Finger> m_fingers;
        static fingersDel func;
        static int  x, y;
        static double lengthOfIF = 0;

        static public string Update(List<Finger> curFingers)
        {
            m_fingers = curFingers;
            if (func != null) return func();
            else
            {
                func = idleFunc;
                return func();
            }
        }

        static private string idleFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;

            if (i == m_fingers.Count)
                i = m_fingers.Count - 1;

            if (m_fingers[i].m_tipPoint.Count == 2)
            {
#if DEBUG
                Console.WriteLine("changed move mode");
#endif
                lengthOfIF = m_fingers[i].m_tipPoint[0].Y > m_fingers[i].m_tipPoint[1].Y ?
                    m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_centerPoint)
                    : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_centerPoint);

                x = m_fingers[i].m_centerPoint.X;
                y = m_fingers[i].m_centerPoint.Y;

                func = moveFunc;
                return "Move";
            }
            return "Idle";
        }

        private static string moveFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
            if (i == m_fingers.Count || m_fingers[i].m_tipPoint.Count > 2)
            {
#if DEBUG
                Console.WriteLine("changed idle mode");
#endif
                func = idleFunc;
                return "idle";
            }

            if (m_fingers[i].m_tipPoint.Count == 2)
            {
                double tempLengthOfIF = m_fingers[i].m_tipPoint[0].Y < m_fingers[i].m_tipPoint[1].Y ?
                        m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_depthPoint[0])
                        : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_depthPoint[1]);

                if (lengthOfIF * 0.6 > tempLengthOfIF)
                {
#if DEBUG
                    Console.WriteLine("changed rbd");
#endif
                    func = rbdFunc;

                    ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTDOWN);
                    return "Right Button Down";
                }
            }

            if (m_fingers[i].m_tipPoint.Count == 1 || m_fingers[i].GetFingerAngle() < 0.7)
            {
#if DEBUG
                Console.WriteLine("changed left down mode");
#endif
                func = lbdFunc;

                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                return "Left Button Down";
            }
            int beforeX = x;
            int beforeY = y;

            beforeX = (int)(m_fingers[i].m_centerPoint.X * 0.8 + beforeX * 0.2);
            beforeY = (int)(m_fingers[i].m_centerPoint.Y * 0.8 + beforeY * 0.2);

            //ApiController.SetCursorPos(m_fingers[i].m_centerPoint.X, m_fingers[i].m_centerPoint.Y);
            //ApiController.MoveCursorPos(m_fingers[i].m_centerPoint.X -x, m_fingers[i].m_centerPoint.Y -y);
            ApiController.MoveCursorPos(beforeX -x, beforeY -y);
            //상대 좌표 이동을 위해 추가
            x = beforeX; y = beforeY;
            //x = m_fingers[i].m_centerPoint.X;
            //y = m_fingers[i].m_centerPoint.Y;
            return "Move";
        }

        private static string lbdFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
            if (i == m_fingers.Count)
            {
#if DEBUG
                Console.WriteLine("left up and changed idle mode");
#endif
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                func = idleFunc;
                return "Idle";
            }

            if (m_fingers[i].m_tipPoint.Count > 1 && m_fingers[i].GetFingerAngle() > 0.7)
            {
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
#if DEBUG          
                Console.WriteLine("left up and changed move mode"); 
#endif
                func = idleFunc;
                return "Move";
            }
            return "Left Button Down";
        }

        static string rbdFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;

            if (i == m_fingers.Count)
            {
#if DEBUG
                Console.WriteLine("right up and changed idle mode");
#endif
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                func = idleFunc;
                return "Idle";
            }

            if (m_fingers[i].m_tipPoint.Count > 1)
            {
                double tempLengthOfIF = m_fingers[i].m_tipPoint[0].Y < m_fingers[i].m_tipPoint[1].Y ?
                        m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_depthPoint[0])
                        : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_depthPoint[1]);

                if (lengthOfIF * 0.8 < tempLengthOfIF)
                {
#if DEBUG
                    Console.WriteLine("changed rbd to move");
#endif
                    func = moveFunc;

                    ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTUP);
                    return "Move";
                }
            }
            return "Right Button Down";
        }
    }
}
