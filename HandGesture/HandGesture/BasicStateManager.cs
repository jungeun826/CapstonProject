using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    static class BasicStateManager 
    {
        delegate void fingersDel();

        static List<Finger> m_fingers;
        static fingersDel func;
        static int  x, y;
        static double lengthOfIF = 0;

        static public void Update(List<Finger> curFingers)
        {
            m_fingers = curFingers;
            if (func != null) func();
            else
            {
                func = idleFunc;
                func();
            }
        }

        static private void idleFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;

            if (i == m_fingers.Count)
                i = m_fingers.Count - 1;

            if (m_fingers[i].m_tipPoint.Count == 2)
            {
                Console.WriteLine("changed move mode");
                lengthOfIF = m_fingers[i].m_tipPoint[0].Y > m_fingers[i].m_tipPoint[1].Y ?
                    m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_centerPoint)
                    : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_centerPoint);

                x = m_fingers[i].m_centerPoint.X;
                y = m_fingers[i].m_centerPoint.Y;

                func = moveFunc;
            }
        }

        private static void moveFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
            if (i == m_fingers.Count || m_fingers[i].m_tipPoint.Count > 2)
            {
                Console.WriteLine("changed idle mode");
                func = idleFunc;
                return;
            }

            if (m_fingers[i].m_tipPoint.Count == 2)
            {
                double tempLengthOfIF = m_fingers[i].m_tipPoint[0].Y < m_fingers[i].m_tipPoint[1].Y ?
                        m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_depthPoint[0])
                        : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_depthPoint[1]);

                if (lengthOfIF * 0.6 > tempLengthOfIF)
                {
                    Console.WriteLine("changed rbd");
                    func = rbdFunc;

                    ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTDOWN);
                    return;
                }
            }

            if (m_fingers[i].m_tipPoint.Count == 1 || m_fingers[i].GetFingerAngle() < 0.7)
            {
                Console.WriteLine("changed left down mode");
                func = lbdFunc;

                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                return;
            }


            //ApiController.SetCursorPos(m_fingers[i].m_centerPoint.X, m_fingers[i].m_centerPoint.Y);
            ApiController.MoveCursorPos(m_fingers[i].m_centerPoint.X -x, m_fingers[i].m_centerPoint.Y -y);

            //상대 좌표 이동을 위해 추가
            x = m_fingers[i].m_centerPoint.X;
            y = m_fingers[i].m_centerPoint.Y;
            return;
        }

        private static void lbdFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
            if (i == m_fingers.Count)
            {
                Console.WriteLine("left up and changed idle mode");
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                func = idleFunc;
                return;
            }

            if (m_fingers[i].m_tipPoint.Count > 1 && m_fingers[i].GetFingerAngle() > 0.7)
            {
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                Console.WriteLine("left up and changed move mode");
                func = idleFunc;
                return;
            }
        }

        static void rbdFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;

            if (i == m_fingers.Count)
            {
                Console.WriteLine("right up and changed idle mode");
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                func = idleFunc;
                return;
            }

            if (m_fingers[i].m_tipPoint.Count > 1)
            {
                double tempLengthOfIF = m_fingers[i].m_tipPoint[0].Y < m_fingers[i].m_tipPoint[1].Y ?
                        m_fingers[i].m_tipPoint[0].DistanceTo(m_fingers[i].m_depthPoint[0])
                        : m_fingers[i].m_tipPoint[1].DistanceTo(m_fingers[i].m_depthPoint[1]);

                if (lengthOfIF * 0.8 < tempLengthOfIF)
                {
                    Console.WriteLine("changed rbd to move");
                    func = moveFunc;

                    ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTUP);
                    return;
                }
            }

        }
    }
}
