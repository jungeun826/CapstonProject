using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    //static class StateManager
    //{
    //    delegate void fingersDel();

    //    enum fstate
    //    {
    //        IDLE,
    //        MOVE,
    //        LBDOWN,
    //        LBUP,
    //        RBDOWN,
    //        RBUP
    //    }

    //    static List<Finger> m_fingers;

    //    static fingersDel func;
        
    //    static public void update(List<Finger> curFingers)
    //    {
    //        m_fingers = curFingers;
    //        if (func != null) func();
    //        else
    //        {
    //            func = idleFunc;
    //            func();
    //        }
    //    }

    //    static private void idleFunc()
    //    {
    //        int i;
    //        for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++);
    //        if (i == m_fingers.Count)
    //        {
    //            //idle
    //            return;
    //        }

    //        if (m_fingers[i].m_tipPoint.Count == 2)
    //        {
    //            Console.WriteLine("changed move mode");
    //            func -= idleFunc;
    //            func += moveFunc;
    //        }
    //    }

    //    private static void moveFunc()
    //    {
    //        int i;
    //        for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
    //        if (i == m_fingers.Count)
    //        {
    //            Console.WriteLine("changed idle mode");
    //            func -= moveFunc;
    //            func += idleFunc;
    //            return;
    //        }

    //        if (m_fingers[i].m_tipPoint.Count == 1 || m_fingers[i].GetFingerAngle() < 0.7)
    //        {
    //            Console.WriteLine("changed left down mode");
    //            func -= moveFunc;
    //            func += lbdFunc;

    //            ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
    //            return;
    //        }

    //        ApiController.SetCursorPos(m_fingers[i].m_centerPoint.X, m_fingers[i].m_centerPoint.Y);

    //        return;
    //    }

    //    private static void lbdFunc()
    //    {
    //        int i;
    //        for (i = 0; i < m_fingers.Count && m_fingers[i].m_tipPoint.Count == 0; i++) ;
    //        if (i == m_fingers.Count)
    //        {
    //            Console.WriteLine("left up and changed idle mode");
    //            ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
    //            func -= lbdFunc;
    //            func += idleFunc;
    //            return;
    //        }

    //        if (m_fingers[i].m_tipPoint.Count > 1 && m_fingers[i].GetFingerAngle() > 0.7)
    //        {
    //            ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTUP);
    //            Console.WriteLine("left up and changed move mode");
    //            func -= lbdFunc;
    //            func += moveFunc;
    //            return;
    //        }

    //    }
    //}

    public enum TransitionType
    {
        Idle,
        Move,
        LBDown,
        LBUp,
        RBDown,
        RBUp,
    }

    public enum StateType
    {
        Idle,
        Move,
        LBDown,
        LBUp,
        RBDown,
        RBUp,
    }

    public class MoveState
        <T> : IState<T> where T : BasicStateManager
    {
        public override void OnEnter(T t)
        {
            base.OnEnter(t);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = 0;  i < context.hands.Count; i++)
            {

                if( context.hands[i].m_tipPoint.Count == 2 )
                {
                    ApiController.SetCursorPos(context.hands[i].m_centerPoint.X, context.hands[i].m_centerPoint.Y);
                    
                }
                
                if (context.hands[i].m_tipPoint.Count == 1 || context.hands[i].GetFingerAngle() < 0.5)
                {
                    Finger.FingerType fingerType = context.hands[i].GetFingerType();
                    if (fingerType.HasFlag(Finger.FingerType.ForeFinger) && fingerType.HasFlag(Finger.FingerType.IndexFinger))
                        context.manager.Transition((int)TransitionType.LBDown);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

        }
    }
    public class LBDownState<T> : IState<T> where T : BasicStateManager
    {
        private bool isDown = false;
        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            isDown = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(isDown == false)
            {
                Debug.Log("LB Mouse Down");
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                isDown = true;
            }

            for (int i = 0; i < context.hands.Count; i++)
            {
                if (context.hands[i].m_tipPoint.Count > 1 && context.hands[i].GetFingerAngle() > 0.7)
                {
                    context.manager.Transition((int)TransitionType.LBUp);
                    return;
                }
                ////move
                //if (context.hands[i].m_tipPoint.Count == 2)
                //{
                //    context.manager.Transition((int)TransitionType.Move);
                //}

            }

            context.manager.Transition((int)TransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            isDown = false;
        }
    }
    public class LBUpState<T> : IState<T> where T : BasicStateManager
    {
        private bool isUp = false;

        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            isUp = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (isUp == false)
            {
                Debug.Log("LB Mouse Down");
                ApiController.mouse_event(ApiController.MOUSEEVENTF_LEFTDOWN);
                isUp = true;
            }

            for (int i = 0; i < context.hands.Count; i++)
            {
                //move
                if (context.hands[i].m_tipPoint.Count == 2)
                {
                    context.manager.Transition((int)TransitionType.Move);
                }
            }

            context.manager.Transition((int)TransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            isUp = false;
        }
    }
    public class RBDownState<T> : IState<T>
    {
    }
    public class RBUpState<T> : IState<T>
    {
    }
    public class IdleState<T> : IState<T> where T : BasicStateManager
    {
        public override void OnUpdate()
        {
 	        base.OnUpdate();
            for (int i = 0;  i < context.hands.Count; i++)
            {
                //move
                if( context.hands[i].m_tipPoint.Count == 2 )
                {
                    context.manager.Transition((int)TransitionType.Move);
                }
                
                //lb down
                if (context.hands[i].m_tipPoint.Count == 1 || context.hands[i].GetFingerAngle() < 0.5)
                {
                    context.manager.Transition((int)TransitionType.LBDown);
                }

                context.hands[i].GetFingerType();
                //rb down
            }
        }
    }


    public class BasicStateManager
    {
        public StateManger2<BasicStateManager> manager;
        public List<Finger> hands;

        public BasicStateManager()
        {
            manager = new StateManger2<BasicStateManager>(this);

            manager.AddState((int)StateType.Idle, new IdleState<BasicStateManager>());
            manager.AddState((int)StateType.Move, new MoveState<BasicStateManager>());
            manager.AddState((int)StateType.LBDown, new LBDownState<BasicStateManager>());
            manager.AddState((int)StateType.LBUp, new LBUpState<BasicStateManager>());
            manager.AddState((int)StateType.RBDown, new RBDownState<BasicStateManager>());
            manager.AddState((int)StateType.RBUp, new RBUpState<BasicStateManager>());
            //manager.AddState((int)StateType.RBUp, new RBUpState<BasicStateManager>());

            manager.AddTransition(-1, (int)TransitionType.Idle, (int)StateType.Idle);

            manager.AddTransition((int)StateType.Idle, (int)TransitionType.Move, (int)StateType.Move);
            manager.AddTransition((int)StateType.Idle, (int)TransitionType.LBDown, (int)StateType.LBDown);
            manager.AddTransition((int)StateType.Idle, (int)TransitionType.RBDown, (int)StateType.RBDown);

            manager.AddTransition((int)StateType.Move, (int)TransitionType.LBDown, (int)StateType.LBDown);
            manager.AddTransition((int)StateType.LBDown, (int)TransitionType.LBUp, (int)StateType.LBUp);
            manager.AddTransition((int)StateType.LBUp, (int)TransitionType.Move, (int)StateType.Move);
            manager.AddTransition((int)StateType.Move, (int)TransitionType.RBDown, (int)StateType.RBDown);
            manager.AddTransition((int)StateType.RBDown, (int)TransitionType.RBUp, (int)StateType.RBUp);
            manager.AddTransition((int)StateType.RBUp, (int)TransitionType.Move, (int)StateType.Move);


            manager.AddTransition((int)StateType.Move, (int)TransitionType.Idle, (int)StateType.Idle);
            manager.AddTransition((int)StateType.LBDown, (int)TransitionType.Idle, (int)StateType.Idle);
            manager.AddTransition((int)StateType.Move, (int)TransitionType.Idle, (int)StateType.Idle);
            manager.AddTransition((int)StateType.RBDown, (int)TransitionType.Idle, (int)StateType.Idle);

            manager.Transition((int)TransitionType.Idle);
        }

        public void Update(List<Finger> hands)
        {
            this.hands = hands;
            manager.Update();
        }

        //public void ChangeTransition(TransitionType type)
        //{
        //    manager.Transition((int)type);
        //}
    }

    public struct TransitionInfo
    {
        public int curStateType;
        public int nextStateType;

        public TransitionInfo(int curStateType, int nextStateType)
        {
            this.curStateType = curStateType;
            this.nextStateType = nextStateType;
        }
    }

    public class StateManger2<T>
    {
        public T context;

        IState<T> prevState = null;
        IState<T> curState = null;
        int curStateType = -1;

        Dictionary<int, IState<T>> stateDic = new Dictionary<int, IState<T>>();
        Dictionary<int, List<TransitionInfo>> transitionDic = new Dictionary<int, List<TransitionInfo>>();
        
        public StateManger2(T context)
        {
            this.context = context;
        }

        public void Update()
        {
            if(curState != null)
            curState.OnUpdate();
        }

        public void AddState(int stateType, IState<T> state)
        {
            if (stateDic.ContainsKey(stateType))
            {
                Debug.Log("Already Exist State ");
            }
            else
            {
                stateDic.Add(stateType, state);
            }
        }

        public void AddTransition(int curStateType, int transitionType, int nextStateType)
        {
            TransitionInfo info = new TransitionInfo(curStateType, nextStateType);
            List<TransitionInfo> infoList = null;
            if (transitionDic.ContainsKey(transitionType))
            {
                //Debug.Log("already exist type");
                infoList = transitionDic[transitionType];
                infoList.Add(info);
                transitionDic[transitionType] = infoList;
            }
            else
            {
                infoList = new List<TransitionInfo>();
                infoList.Add(info);
                transitionDic.Add(transitionType, infoList);
            }
        }

        public void Transition(int transitionType)
        {
            int nextStateType = GetTransitionStateType(transitionType);
            if (nextStateType == -1)
                return;

            IState<T> nextState = stateDic[nextStateType];

            if (prevState != null)
                prevState.OnExit();

            prevState = curState;
            curState = nextState;
            curState.OnEnter(context);

            curStateType = nextStateType;
        }

        private int GetTransitionStateType(int transitionType)
        {
            List<TransitionInfo> infoList = null;
            if (transitionDic.ContainsKey(transitionType))
                infoList = transitionDic[transitionType];
            if (infoList == null)
            {
                Debug.Log("Don't Add Transition Type");
                return -1;
            }

            int nextType = -1;
            for (int i = 0; i < infoList.Count; i++)
            {
                if (infoList[i].curStateType == curStateType)
                    nextType = infoList[i].nextStateType;
            }

            return nextType;
        }

    }


    public class IState<T> 
    {
        public T context;
        public virtual void OnEnter(T t)
        {
            context = t;
        }

        public virtual void OnUpdate(){}
        public virtual void OnExit(){}
    }
}
