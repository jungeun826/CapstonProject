using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    public static class FPSStateManager
    {
        delegate string fingersDel();

        static List<Finger> m_fingers;
        static fingersDel func;
        static int x, y;
        static int foreY = -1;

        static public string Update(List<Finger> curFingers)
        {
            m_fingers = curFingers;
            if (func != null)return func();
            else
            {
                func = idleFunc;
                return func();
            }
        }

        static private bool GetHandIdx(out int idx)
        {
            int i = 0;
            for (; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++)
            { }
            idx = i;
            if(m_fingers.Count == 0) return false;
           
            //마지막 hand도 tipcount가 0임..
            if(m_fingers.Count == i ) i = m_fingers.Count -1;
            idx = i;
            
            return true;
        }

        static private string idleFunc()
        {
            int i = 0;
            if (!GetHandIdx(out i)) return "Idle";

            if (m_fingers[i].m_tipPoint.Count <= 2)
            {
                Console.WriteLine("change mouseMove");
                
                x = m_fingers[i].m_centerPoint.X;
                y = m_fingers[i].m_centerPoint.Y;

                func = mouseMoveFunc;
                return "MouseMove";
            }
            return "Idle";
        }

        static private string mouseMoveFunc()
        {
            int i = 0;
            if (!GetHandIdx(out i)) return "???1";

            if (m_fingers[i].m_tipPoint.Count > 2)
            {
                Console.WriteLine("change idle");
                foreY = -1;
                func = idleFunc;
                return "Idle";
            }

            if (foreY == -1)
            {
                if (m_fingers[i].m_tipPoint.Count == 1)
                {
                    foreY = m_fingers[i].GetPixelCntYFingerTip(0);
                }
            }

            if (m_fingers[i].m_tipPoint.Count == 1)
            {
                if (foreY < m_fingers[i].GetPixelCntYFingerTip(0))
                {
                    Console.WriteLine("change shoot");
                    func = shootFunc;
                    return "Shoot";
                }
            }

            //ApiController.SetCursorPos(m_fingers[i].m_centerPoint.X, m_fingers[i].m_centerPoint.Y);
            int dx = m_fingers[i].m_centerPoint.X - x;
            int dy = m_fingers[i].m_centerPoint.Y - y;
            //Debug.Log((dx).ToString() + "," + (dy).ToString());
            ApiController.MoveCursorPos(dx, dy);

            //상대 좌표 이동을 위해 추가
            x = m_fingers[i].m_centerPoint.X;
            y = m_fingers[i].m_centerPoint.Y;

            return "MouseMove";
        }

        static private string shootFunc()
        {
            int i = 0;
            if (!GetHandIdx(out i)) return "???2";

            if (m_fingers[i].m_tipPoint.Count == 1)
            {
                if (foreY * 1.2f > m_fingers[i].GetPixelCntYFingerTip(0))
                {
                    Console.WriteLine("change mouseMove");
                    ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                    ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                    func = mouseMoveFunc;
                    return "MouseMove";
                }
            }

            if (m_fingers[i].m_tipPoint.Count > 2)
            {
                Console.WriteLine("change idle");
                foreY = -1;
                func = idleFunc;
                return "Idle";
            }

            return "Shoot";
        }

        static private void reloadFunc()
        {
            int i = 0;
            if (!GetHandIdx(out i)) return;

            if (m_fingers[i].m_tipPoint.Count == 1)
            {
                if (foreY * 1.2f > m_fingers[i].GetPixelCntYFingerTip(0))
                {
                    Console.WriteLine("change reload");
                    ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                    ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
                    func = mouseMoveFunc;
                    return;
                }
            }

            if (m_fingers[i].m_tipPoint.Count > 2)
            {
                Console.WriteLine("change idle");
                foreY = -1;
                func = idleFunc;
                return;
            }

        }


    }
}

//namespace HandGesture
//{
//    public enum FPSModeTransitionType
//    {
//        Idle,
//        MouseMove,
//        FrontMove,
//        SwifeEnter,
//        LSwife,
//        RSwife,

//        Shoot,
//    }

//    //idle == move?
//    public enum FPSModeStateType
//    {
//        Idle,
//        Move,
//        SwifeEnter,
//        LSwife,
//        RSwife,

//        Shoot,
//    }


//    //손이 2개일 때 어느 것을 먼저 처리할 것인가..
//    //오른손 왼손을 따로 돌려야하나?
//    public class FPSStateManager : IStateManger
//    {
//        public StateManger2<FPSStateManager> RightHandManager;
//        public List<Finger> hands;
//        public int ForeFingerYLength;

//        public FPSStateManager()
//        {
//            RightHandManager = new StateManger2<FPSStateManager>(this);

//            RightHandManager.AddState((int)FPSModeStateType.Idle, new FPS_IdleState<FPSStateManager>());
//            RightHandManager.AddState((int)FPSModeStateType.Move, new FPS_MoveState<FPSStateManager>());
//            RightHandManager.AddState((int)FPSModeStateType.SwifeEnter, new FPS_SwifeEnterState<FPSStateManager>());
//            RightHandManager.AddState((int)FPSModeStateType.LSwife, new FPS_LSwifeState<FPSStateManager>());
//            RightHandManager.AddState((int)FPSModeStateType.RSwife, new FPS_RSwifeState<FPSStateManager>());
//            RightHandManager.AddState((int)FPSModeStateType.RSwife, new FPS_ShootState<FPSStateManager>());

//            //manager.AddState((int)StateType.RBUp, new RBUpState<BasicStateManager>());

//            RightHandManager.AddTransition(-1, (int)BasicModeTransitionType.Idle, (int)FPSModeStateType.Idle);

//            RightHandManager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.MouseMove, (int)FPSModeStateType.Move);
//            RightHandManager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);
//            RightHandManager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.Shoot, (int)FPSModeStateType.Shoot);

//            RightHandManager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
//            RightHandManager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);
//            RightHandManager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.Shoot, (int)FPSModeStateType.Shoot);


//            RightHandManager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
//            RightHandManager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.MouseMove, (int)FPSModeStateType.Move);
//            //움직이다가 swifeEnter로 가는 경우가 있을까..
//            RightHandManager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);

//            RightHandManager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
//            RightHandManager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.MouseMove, (int)FPSModeStateType.Move);
//            //움직이다가 swifeEnter로 가는 경우가 있을까..
//            RightHandManager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);

//            RightHandManager.AddTransition((int)FPSModeStateType.SwifeEnter, (int)FPSModeTransitionType.LSwife, (int)FPSModeStateType.LSwife);
//            RightHandManager.AddTransition((int)FPSModeStateType.SwifeEnter, (int)FPSModeTransitionType.RSwife, (int)FPSModeStateType.RSwife);

//            //shoot이 연결이 안됨

//            RightHandManager.Transition((int)BasicModeTransitionType.Idle);
//        }

//        public void Update(List<Finger> hands)
//        {
//            this.hands = hands;
//            RightHandManager.Update();
//        }

//        public string GetCurState()
//        {
//            return ((FPSModeTransitionType)RightHandManager.GetCurStateType()).ToString();
//        }

//        public bool IsMoveGesture(int handIdx)
//        {
//            if (hands[handIdx].m_centerPoint.X > (WebcamController.Instance.FrameSize.Width / 2))
//                return true;
//            return false ;
//        }

//        public bool IsShootGesture()
//        {

//            return true;
//        }

//        public bool IsSwifeEnterGesture()
//        {
//            return true;
//        }

//        public bool IsLeftSwifeGesture()
//        {
//            return true;
//        }

//        //public bool IsFrontMove(int handIdx)
//        //{
//        //    if (hands[handIdx].m_tipPoint.Count < 2)
//        //    {

//        //    }
//        //}

//        public bool IsRightSwifeGesture()
//        {
//            return true;
//        }
//    }

//    public class FPS_IdleState<T> : IState<T> where T : FPSStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            for (int i = 0; i < context.hands.Count; i++)
//            {
//                if (context.hands[i].m_tipPoint.Count == 5)
//                {
//                    context.RightHandManager.Transition((int)FPSModeTransitionType.SwifeEnter);
//                    //return;
//                }

//                if (context.hands[i].m_tipPoint.Count == 1
//                    && context.hands[i].GetFingerRatio(context.hands[i].m_tipPoint[0]) <= 1.4)
//                {
//                    context.RightHandManager.Transition((int)FPSModeTransitionType.MouseMove);
//                }

//                //슛이 basic모드과 같은 손가락인 경우에도 체크가 됨..
//                //y축의 손가락 포인트를 이용해 체크해야 할 것 같기도 하당...
//                if ( context.hands[i].GetFingerRatio(context.hands[i].m_tipPoint[0]) >= 1.6)
//                {
//                    context.RightHandManager.Transition((int)FPSModeTransitionType.Shoot);
//                }
//            }
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class FPS_MoveState<T> : IState<T> where T : FPSStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            for (int i = 0; i < context.hands.Count; i++)
//            {
//                if (context.hands[i].m_tipPoint.Count == 0)
//                {
//                    ApiController.MouseEvent(ApiController.eMouseEventType.SETPOS, context.hands[i].m_centerPoint.X, context.hands[i].m_centerPoint.Y);
//                    return;
//                }

//                if (context.hands[i].m_tipPoint.Count == 5)
//                {
//                    context.RightHandManager.Transition((int)FPSModeTransitionType.SwifeEnter);
//                    return;
//                }
//            }
//            context.RightHandManager.Transition((int)FPSModeTransitionType.Idle);
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class FPS_SwifeEnterState<T> : IState<T> where T : FPSStateManager
//    {
//        OpenCvSharp.CvPoint enterPoint;

//        public override void OnEnter(T t)
//        { 
//            base.OnEnter(t);
//            for (int i = 0; i < context.hands.Count; i++)
//            {
//                if (context.hands[i].m_tipPoint.Count == 5)
//                {
//                    enterPoint = context.hands[i].m_centerPoint;
//                    return;
//                }
//            }
//        }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();

//            for (int i = 0; i < context.hands.Count; i++)
//            {
//                if (context.hands[i].m_tipPoint.Count == 5)
//                {
//                    if (enterPoint.X - context.hands[i].m_centerPoint.X > 1f
//                        || enterPoint.Y - context.hands[i].m_centerPoint.Y > 1f)
//                        context.RightHandManager.Transition((int)FPSModeTransitionType.LSwife);

//                    if (enterPoint.X - context.hands[i].m_centerPoint.X > -1f
//                        || enterPoint.Y - context.hands[i].m_centerPoint.Y > -1f)
//                        context.RightHandManager.Transition((int)FPSModeTransitionType.RSwife);
//                }
//            }
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class FPS_LSwifeState<T> : IState<T> where T : FPSStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            //무슨 키인지 모름..흑.. 그래서 그냥 Idle로 넘김...ㅋㅋ
//            context.RightHandManager.Transition((int)FPSModeTransitionType.Idle);

//            //for (int i = 0; i < context.hands.Count; i++)
//            //{
//            //    if (context.hands[i].m_tipPoint.Count == 5)
//            //    {
//            //    }
//            //}
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class FPS_RSwifeState<T> : IState<T> where T : FPSStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            //무슨 키인지 모름..흑.. 그래서 그냥 Idle로 넘김...ㅋㅋ
//            context.RightHandManager.Transition((int)FPSModeTransitionType.Idle);
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }

//    public class FPS_ShootState<T> : IState<T> where T : FPSStateManager
//    {
//        public override void OnEnter(T t)
//        { base.OnEnter(t); }

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            //무슨 키인지 모름....

//            //슛 끝나면 무조건 Idle로 가서 다시 체크하도록 하면 될거같다.
//            context.RightHandManager.Transition((int)FPSModeTransitionType.Idle);
//        }

//        public override void OnExit()
//        { base.OnExit(); }
//    }
//}
