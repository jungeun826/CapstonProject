using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    public static class RacingStateManager
    {
        delegate void fingersDel();

        static List<Finger> m_fingers;
        static fingersDel func;

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
            int i = 0;
            for (; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++)
            { }

            if (i == 3)
            {
                Console.WriteLine("change handle");
                ApiController.keybd_event((uint)System.Windows.Forms.Keys.Up, 0, 0x00, 0);
                func = handleFunc;
                return;
            }
        }

        static private void handleFunc()
        {
            int i;
            int ri = 0, li = 0;

            for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++)
            {
                int temp = m_fingers[i].m_centerPoint.X;
                if (m_fingers[ri].m_centerPoint.X < temp)
                {
                    ri = i;
                }

                if (m_fingers[li].m_centerPoint.X > temp)
                {
                    li = i;
                }
            }

            if (m_fingers.Count < 3)
            {
                Console.WriteLine("change idle state");
                ApiController.keybd_event((uint)System.Windows.Forms.Keys.Up, 0, 0x02, 0);
                //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x02, 0);
                //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x02, 0);
                func = idleFunc;
                return;
            }

            if (Math.Abs(m_fingers[li].m_centerPoint.Y - m_fingers[ri].m_centerPoint.Y) > m_fingers[li].m_rad)
            {
                if (m_fingers[li].m_centerPoint.Y < m_fingers[ri].m_centerPoint.Y)
                {
                    ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x00, 0);

                    //sleep해서 프레임 드랍이 심해져서 주석처리함.
                    //System.Threading.Thread.Sleep(100);
                    //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x02, 0);
                }
                else
                {
                    ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x00, 0);
                    //System.Threading.Thread.Sleep(100);
                    //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x02, 0);
                }
            }
            return;
        }

    }
}
//    namespace HandGesture
//{
//    public enum RacingModeTransitionType
//    {
//        Idle,
//        Handle,
//        //Left,
//        //Right,
//    }

//    //idle == move?
//    public enum RacingModeStateType
//    {
//        Idle,
//        Handle,
//        //Left,
//        //Right,
//    }


//    public class RacingStateManager : IStateManger
          
//    {
//        public StateManger2<RacingStateManager> manager;
//            public List<Finger> hands;

//            public RacingStateManager()
//            {
//                manager = new StateManger2<RacingStateManager>(this);

//                manager.AddState((int)RacingModeStateType.Idle, new Racing_IdleState<RacingStateManager>());
//                manager.AddState((int)RacingModeStateType.Handle, new Racing_HandleState<RacingStateManager>());
//                //manager.AddState((int)RacingModeStateType.Left, new Racing_LeftState<RacingStateManager>());
//                //manager.AddState((int)RacingModeStateType.Right, new Racing_RightEnterState<RacingStateManager>());

//                manager.AddTransition(-1, (int)RacingModeTransitionType.Idle, (int)RacingModeStateType.Idle);

//                manager.AddTransition((int)RacingModeStateType.Idle, (int)RacingModeTransitionType.Handle, (int)RacingModeStateType.Handle);
//                manager.AddTransition((int)RacingModeStateType.Handle, (int)RacingModeTransitionType.Idle, (int)RacingModeStateType.Idle);


//                manager.Transition((int)RacingModeTransitionType.Idle);
//            }

//            public void Update(List<Finger> hands)
//            {
//                this.hands = hands;
//                manager.Update();
//            }

//            public string GetCurState()
//            {
//                return ((RacingModeStateType)manager.GetCurStateType()).ToString();
//            }
//    }

//    public class Racing_IdleState<T> : IState<T> where T : RacingStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            int i = 0;
//            for ( ; i < context.hands.Count && context.hands[i].m_tipPoint.Count == 0; i++)
//            {}

//            if (i == 3)
//            {
//                Console.WriteLine("change handle");
//                ApiController.keybd_event((uint)System.Windows.Forms.Keys.Up, 0, 0x00, 0);
//                context.manager.Transition((int)RacingModeTransitionType.Handle);
//                return;
//            }
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class Racing_HandleState<T> : IState<T> where T : RacingStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();

//            int i;
//            int ri = 0, li = 0;

//            for (i = 0; i < context.hands.Count && context.hands[i].m_tipPoint.Count == 0; i++)
//            {
//                int temp = context.hands[i].m_centerPoint.X;
//                if (context.hands[ri].m_centerPoint.X < temp)
//                {
//                    ri = i;
//                }

//                if (context.hands[li].m_centerPoint.X > temp)
//                {
//                    li = i;
//                }
//            }

//            if (context.hands.Count < 3)
//            {
//                Console.WriteLine("change idle state");
//                ApiController.keybd_event((uint)System.Windows.Forms.Keys.Up, 0, 0x02, 0);
//                //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x02, 0);
//                //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x02, 0);
//                context.manager.Transition((int)RacingModeTransitionType.Idle);
//                return;
//            }

//            if (Math.Abs(context.hands[li].m_centerPoint.Y - context.hands[ri].m_centerPoint.Y) > context.hands[li].m_rad)
//            {
//                if (context.hands[li].m_centerPoint.Y < context.hands[ri].m_centerPoint.Y)
//                {
//                    ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x00, 0);
//                    //System.Threading.Thread.Sleep(100);
//                    //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Left, 0, 0x02, 0);
//                }
//                else
//                {
//                    ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x00, 0);
//                    //System.Threading.Thread.Sleep(100);
//                    //ApiController.keybd_event((uint)System.Windows.Forms.Keys.Right, 0, 0x02, 0);
//                }
//            }
//            return;
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }
//}
