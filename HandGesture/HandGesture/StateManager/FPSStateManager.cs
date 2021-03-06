﻿using System;
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
        static int Threshold = -1;
        static float ShootRate = 0.6f;//1.3f;
        //static float ReloadRate = 0.5f;

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

        static private bool GetRightHandIdx(out int idx)
        {
            int i = 0;
            int rightHandIdx = 0;
            if (m_fingers.Count == 1)
            {
                idx = -1;
                return false;
            }

            for (; i < m_fingers.Count; i++)
            {
                if (m_fingers[rightHandIdx].m_centerPoint.X < m_fingers[i].m_centerPoint.X)
                    rightHandIdx = i;
            }

            idx = rightHandIdx;
            if (m_fingers.Count < 2)
            {
                idx = -1;
                return false;
            }

            //마지막 hand도 tipcount가 0임..
            if (m_fingers.Count == i)
            {
                if (i == rightHandIdx)
                    idx = m_fingers.Count - 1;
            }

            return true;
        }

        static private string idleFunc()
        {
            int i = 0;
            if (!GetRightHandIdx(out i) || i == -1) return "Idle";


            if (ProcessReloadLogic(i))
            {
                return "Reload";
            }

            if (m_fingers[i].m_tipPoint.Count <= 2)
            {
                Console.WriteLine("change mouseMove");
                
                x = m_fingers[i].m_centerPoint.X;
                y = m_fingers[i].m_centerPoint.Y;

                func = mouseMoveFunc;
                return "MouseMove";
            }
            return "IdleIng";
        }

        static private bool ProcessReloadLogic(int idx)
        {
            if (idx != -1 && m_fingers[idx].m_tipPoint.Count == 5)
            {
                Console.WriteLine("change Reload");

                ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTDOWN);
                func = reloadFunc;
                return true;
            }

            return false;
        }


        static int idleMiss = 0;
        static private string mouseMoveFunc()
        {
            int i = 0;
            if (!GetRightHandIdx(out i) || i == -1 || (m_fingers[i].m_tipPoint.Count > 2))
            {
                idleMiss++; 
                if (idleMiss > 10)
                {
                    idleMiss = 0;
                    Console.WriteLine("change idle");
                    func = idleFunc;
                    Threshold = -1;
                    return "Idle";
                }

                if (ProcessReloadLogic(i))
                {
                    return "Reload";
                }

                return "IdleMiss";
            }

            if (i == 1 && m_fingers.Count == 1)
            {

                Console.WriteLine("change idle");
                func = idleFunc;
                Threshold = -1;
                return "Idle";
            }

            SetThreshold(i);
            if (Threshold != -1)
            {
                int LagestY = GetLagestYCntFingerTip(i);

                if (LagestY != -1 && LagestY < Threshold * ShootRate)
                {
                        Console.WriteLine("change shoot");

                        ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                        func = shootFunc;
                        return "Shoot";
                }
            }

            int beforeX = x;
            int beforeY = y;

            beforeX = (int)(m_fingers[i].m_centerPoint.X * 0.8 + beforeX * 0.2);
            beforeY = (int)(m_fingers[i].m_centerPoint.Y * 0.8 + beforeY * 0.2);

            int tempDeltaX = beforeX - x;
            int tempDeltaY = beforeY - y;

            float RatioX = 4f * (DetectorManager.Instance.MonitorSize.Value.Width / WebcamController.Instance.FrameSize.Width);
            float RatioY = 2f *( DetectorManager.Instance.MonitorSize.Value.Height / WebcamController.Instance.FrameSize.Height);

            int moveDeltaX = tempDeltaX, moveDeltaY = tempDeltaY;
            if (Math.Abs(tempDeltaX) < 5)
            {
                notMoveXCnt++;
                tempDeltaX = 0;
            }
            if (Math.Abs(tempDeltaY) < 5)
            {
                tempDeltaY = 0;
                notMoveYCnt++;
            }

            int posX = 0;
            int posY = 0;

            if (tempDeltaX == 0 && notMoveXCnt > 5)
            {
                posX = (int)(((float)moveDeltaY) / 5);
                notMoveXCnt = 0;
            }
            if (tempDeltaY == 0 && notMoveYCnt > 5)
            {
                posY = (int)(((float)moveDeltaY ) / 5);
                notMoveYCnt = 0;
            }

            if (tempDeltaX != 0)
                posX = (int)(((float)tempDeltaX ));
            if (tempDeltaY != 0)
                posY = (int)(((float)tempDeltaY ));

            float rad = ((float)m_fingers[i].m_rad * 1.2f);
            if (m_fingers[i].m_centerPoint.X + rad > WebcamController.Instance.FrameSize.Width)
            {
                posX = 10;
            }
            if (m_fingers[i].m_centerPoint.X - rad < WebcamController.Instance.FrameSize.Width * 0.5)
            {
                posX = -10;
            }

            if (m_fingers[i].m_centerPoint.Y + rad > WebcamController.Instance.FrameSize.Height)
            {
                posY = 10;
            }
            if (m_fingers[i].m_centerPoint.Y - rad <0)
            {
                posY = -10;
            }

            ApiController.MoveCursorDelta((uint)((float)posX), (uint)((float)posY));

            //상대 좌표 이동을 위해 추가
            if (tempDeltaX != 0)
            {
                x = beforeX;
            }
            if (tempDeltaY != 0)
            {
                y = beforeY;
            }

            return "MouseMoveIng";
        }
        static int notMoveXCnt = 0;
        static int notMoveYCnt = 0;


        private static int GetLagestYCntFingerTip(int i)
        {
            int LagestY = 0;

            if (i == -1) return -1;

            if (m_fingers.Count == 1 && 1 == i)
                return -1;

            for (int idx = 0; idx < m_fingers[i].m_tipPoint.Count; idx++)
            {
                int pixelCnt = m_fingers[i].GetPixelCntYFingerTip(idx);
                if (LagestY < pixelCnt)
                    LagestY = pixelCnt;
            }
            return LagestY;
        }

        static private string reloadFunc()
        {
            ApiController.mouse_event(ApiController.MOUSEEVENTF_RIGHTUP);

            Console.WriteLine("change idle");
            func = idleFunc;
            Threshold = -1;
            return "reload";
        }

        private static void SetThreshold(int i)
        {
            if (i == -1)
            {
                Threshold = -1;
                return;
            }

            if (Threshold == -1)
            {
                Threshold = GetLagestYCntFingerTip(i);
                if (Threshold < m_fingers[i].m_rad * 2.2f)
                    Threshold = -1;
            }
        }

        static int shootIngCnt = 0;
        static private string shootFunc() 
        {
            ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);

            if (shootIngCnt++ > 10)
            {
                shootIngCnt = 0;
                Console.WriteLine("change idle");
                func = idleFunc;
                Threshold = -1;
                return "Idle";
            }
            int i = 0;
            if (!GetRightHandIdx(out i))
            {
                shootIngCnt = 0;
                Console.WriteLine("change idle");
                func = idleFunc;
                Threshold = -1;
                return "Idle";
            }

            int LagestY = GetLagestYCntFingerTip(i);
            if (LagestY > Threshold * ShootRate )
            {
                shootIngCnt = 0;
                Console.WriteLine("change mouseMove");
                func = mouseMoveFunc;
                return "MouseMove";
            }

            ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);

            return "Shooting";

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
