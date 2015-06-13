using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandGesture
{
    public enum FPSModeTransitionType
    {
        Idle,
        Move,
        SwifeEnter,
        LSwife,
        RSwife,

        Shoot,
    }

    //idle == move?
    public enum FPSModeStateType
    {
        Idle,
        Move,
        SwifeEnter,
        LSwife,
        RSwife,

        Shoot,
    }


    //손이 2개일 때 어느 것을 먼저 처리할 것인가..
    //오른손 왼손을 따로 돌려야하나?
    public class FPSStateManager : IStateManger
    {
        public StateManger2<FPSStateManager> manager;
        public List<Finger> hands;

        public FPSStateManager()
        {
            manager = new StateManger2<FPSStateManager>(this);

            manager.AddState((int)FPSModeStateType.Idle, new FPS_IdleState<FPSStateManager>());
            manager.AddState((int)FPSModeStateType.Move, new FPS_MoveState<FPSStateManager>());
            manager.AddState((int)FPSModeStateType.SwifeEnter, new FPS_SwifeEnterState<FPSStateManager>());
            manager.AddState((int)FPSModeStateType.LSwife, new FPS_LSwifeState<FPSStateManager>());
            manager.AddState((int)FPSModeStateType.RSwife, new FPS_RSwifeState<FPSStateManager>());
            manager.AddState((int)FPSModeStateType.RSwife, new FPS_ShootState<FPSStateManager>());

            //manager.AddState((int)StateType.RBUp, new RBUpState<BasicStateManager>());

            manager.AddTransition(-1, (int)BasicModeTransitionType.Idle, (int)FPSModeStateType.Idle);

            manager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.Move, (int)FPSModeStateType.Move);
            manager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);
            manager.AddTransition((int)FPSModeStateType.Idle, (int)FPSModeTransitionType.Shoot, (int)FPSModeStateType.Shoot);

            manager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
            manager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);
            manager.AddTransition((int)FPSModeStateType.Move, (int)FPSModeTransitionType.Shoot, (int)FPSModeStateType.Shoot);


            manager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
            manager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.Move, (int)FPSModeStateType.Move);
            //움직이다가 swifeEnter로 가는 경우가 있을까..
            manager.AddTransition((int)FPSModeStateType.LSwife, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);

            manager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.Idle, (int)FPSModeStateType.Idle);
            manager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.Move, (int)FPSModeStateType.Move);
            //움직이다가 swifeEnter로 가는 경우가 있을까..
            manager.AddTransition((int)FPSModeStateType.RSwife, (int)FPSModeTransitionType.SwifeEnter, (int)FPSModeStateType.SwifeEnter);

            manager.AddTransition((int)FPSModeStateType.SwifeEnter, (int)FPSModeTransitionType.LSwife, (int)FPSModeStateType.LSwife);
            manager.AddTransition((int)FPSModeStateType.SwifeEnter, (int)FPSModeTransitionType.RSwife, (int)FPSModeStateType.RSwife);

            //shoot이 연결이 안됨

            manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public void Update(List<Finger> hands)
        {
            this.hands = hands;
            manager.Update();
        }

        public string GetCurState()
        {
            return ((FPSModeTransitionType)manager.GetCurStateType()).ToString();
        }
    }

    public class FPS_IdleState<T> : IState<T> where T : FPSStateManager
    {
        public override void OnEnter(T t)
        { base.OnEnter(t); }

        public override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = 0; i < context.hands.Count; i++)
            {
                if (context.hands[i].m_tipPoint.Count == 5)
                {
                    context.manager.Transition((int)FPSModeTransitionType.SwifeEnter);
                    //return;
                }

                if (context.hands[i].m_tipPoint.Count == 1
                    && context.hands[i].GetFingerRatio(context.hands[i].m_tipPoint[0]) <= 1.4)
                {
                    context.manager.Transition((int)FPSModeTransitionType.Move);
                }

                //슛이 basic모드과 같은 손가락인 경우에도 체크가 됨..
                //y축의 손가락 포인트를 이용해 체크해야 할 것 같기도 하당...
                if ( context.hands[i].GetFingerRatio(context.hands[i].m_tipPoint[0]) >= 1.6)
                {
                    context.manager.Transition((int)FPSModeTransitionType.Shoot);
                }
            }
        }

        public override void OnExit()
        { base.OnExit(); }
    }

    public class FPS_MoveState<T> : IState<T> where T : FPSStateManager
    {
        public override void OnEnter(T t)
        { base.OnEnter(t); }

        public override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = 0; i < context.hands.Count; i++)
            {
                if (context.hands[i].m_tipPoint.Count == 0)
                {
                    ApiController.MouseEvent(ApiController.eMouseEventType.SETPOS, context.hands[i].m_centerPoint.X, context.hands[i].m_centerPoint.Y);
                    return;
                }

                if (context.hands[i].m_tipPoint.Count == 5)
                {
                    context.manager.Transition((int)FPSModeTransitionType.SwifeEnter);
                    return;
                }
            }
            context.manager.Transition((int)FPSModeTransitionType.Idle);
        }

        public override void OnExit()
        { base.OnExit(); }
    }

    public class FPS_SwifeEnterState<T> : IState<T> where T : FPSStateManager
    {
        OpenCvSharp.CvPoint enterPoint;

        public override void OnEnter(T t)
        { 
            base.OnEnter(t);
            for (int i = 0; i < context.hands.Count; i++)
            {
                if (context.hands[i].m_tipPoint.Count == 5)
                {
                    enterPoint = context.hands[i].m_centerPoint;
                    return;
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < context.hands.Count; i++)
            {
                if (context.hands[i].m_tipPoint.Count == 5)
                {
                    if (enterPoint.X - context.hands[i].m_centerPoint.X > 1f
                        || enterPoint.Y - context.hands[i].m_centerPoint.Y > 1f)
                        context.manager.Transition((int)FPSModeTransitionType.LSwife);

                    if (enterPoint.X - context.hands[i].m_centerPoint.X > -1f
                        || enterPoint.Y - context.hands[i].m_centerPoint.Y > -1f)
                        context.manager.Transition((int)FPSModeTransitionType.RSwife);
                }
            }
        }

        public override void OnExit()
        { base.OnExit(); }
    }

    public class FPS_LSwifeState<T> : IState<T> where T : FPSStateManager
    {
        public override void OnEnter(T t)
        { base.OnEnter(t); }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //무슨 키인지 모름..흑.. 그래서 그냥 Idle로 넘김...ㅋㅋ
            context.manager.Transition((int)FPSModeTransitionType.Idle);

            //for (int i = 0; i < context.hands.Count; i++)
            //{
            //    if (context.hands[i].m_tipPoint.Count == 5)
            //    {
            //    }
            //}
        }

        public override void OnExit()
        { base.OnExit(); }
    }

    public class FPS_RSwifeState<T> : IState<T> where T : FPSStateManager
    {
        public override void OnEnter(T t)
        { base.OnEnter(t); }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //무슨 키인지 모름..흑.. 그래서 그냥 Idle로 넘김...ㅋㅋ
            context.manager.Transition((int)FPSModeTransitionType.Idle);
        }

        public override void OnExit()
        { base.OnExit(); }
    }

    public class FPS_ShootState<T> : IState<T> where T : FPSStateManager
    {
        public override void OnEnter(T t)
        { base.OnEnter(t); }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //무슨 키인지 모름....

            //슛 끝나면 무조건 Idle로 가서 다시 체크하도록 하면 될거같다.
            context.manager.Transition((int)FPSModeTransitionType.Idle);
        }

        public override void OnExit()
        { base.OnExit(); }
    }
}
