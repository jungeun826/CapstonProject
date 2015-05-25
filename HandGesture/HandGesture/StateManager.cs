using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    static class StateManager
    {
        delegate void fingersDel();

        enum fstate
        {
            IDLE,
            MOVE,
            LBDOWN,
            LBUP,
            RBDOWN,
            RBUP
        }

        static List<Finger> m_fingers;

        static fingersDel func;
        
        static public void update(List<Finger> curFingers)
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
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++);
            if (i == m_fingers.Count)
            {
                //idle
                return;
            }

            if (m_fingers[i].m_tipPoint.Count == 2)
            {
                Console.WriteLine("changed move mode");
                func -= idleFunc;
                func += moveFunc;
            }
        }

        private static void moveFunc()
        {
            int i;
            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
            if (i == m_fingers.Count)
            {
                Console.WriteLine("changed idle mode");
                func -= moveFunc;
                func += idleFunc;
                return;
            }

            if (m_fingers[i].m_tipPoint.Count == 1 || m_fingers[i].GetFingerAngle() < 0.7)
            {
                Console.WriteLine("changed left down mode");
                func -= moveFunc;
                func += lbdFunc;

                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                return;
            }

            ApiController.SetCursorPos(m_fingers[i].m_centerPoint.X, m_fingers[i].m_centerPoint.Y);

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
                func -= lbdFunc;
                func += idleFunc;
                return;
            }

            if (m_fingers[i].m_tipPoint.Count > 1 && m_fingers[i].GetFingerAngle() > 0.7)
            {
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                Console.WriteLine("left up and changed move mode");
                func -= lbdFunc;
                func += moveFunc;
                return;
            }

        }
    }
}
