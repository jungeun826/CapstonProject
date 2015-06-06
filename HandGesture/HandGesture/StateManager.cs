using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    public enum BasicModeTransitionType
    {
        Idle,
        Move,
        LBDown,
        LBUp,
        RBDown,
        RBUp,
    }

    public enum BasicModeStateType
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
        int x, y;
        public override void OnEnter(T t)
        {
            base.OnEnter(t);

            x = context.hands[0].m_centerPoint.X;
            y = context.hands[0].m_centerPoint.Y;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = 0;  i < context.hands.Count; i++)
            {

                if( context.hands[i].m_tipPoint.Count == 2 )
                {
                    //ApiController.SetCursorPos(context.hands[i].m_centerPoint.X, context.hands[i].m_centerPoint.Y);
                    //ApiController.MoveCursorPos(context.hands[i].m_centerPoint.X - x, context.hands[i].m_centerPoint.Y - y);
                    ApiController.MoveCursorPos(context.hands[i].m_centerPoint.X - x, context.hands[i].m_centerPoint.Y - y);
                }
                
                if (context.hands[i].m_tipPoint.Count == 1 || context.hands[i].GetFingerAngle() < 0.5)
                {
                    Finger.FingerType fingerType = context.hands[i].GetFingerType();
                    if (fingerType.HasFlag(Finger.FingerType.ForeFinger) && fingerType.HasFlag(Finger.FingerType.IndexFinger))
                        context.manager.Transition((int)BasicModeTransitionType.LBDown);
                }

                x = context.hands[i].m_centerPoint.X;
                y = context.hands[i].m_centerPoint.Y;

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
                    context.manager.Transition((int)BasicModeTransitionType.LBUp);
                    return;
                }
                ////move
                //if (context.hands[i].m_tipPoint.Count == 2)
                //{
                //    context.manager.Transition((int)TransitionType.Move);
                //}

            }

            context.manager.Transition((int)BasicModeTransitionType.Idle);
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
                    context.manager.Transition((int)BasicModeTransitionType.Move);
                }
            }

            context.manager.Transition((int)BasicModeTransitionType.Idle);
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
                    context.manager.Transition((int)BasicModeTransitionType.Move);
                }
                
                //lb down
                if (context.hands[i].m_tipPoint.Count == 1 || context.hands[i].GetFingerAngle() < 0.5)
                {
                    context.manager.Transition((int)BasicModeTransitionType.LBDown);
                }

                context.hands[i].GetFingerType();
                //rb down
            }
        }
    }


    public class BasicStateManager : IStateManger
    {
        public StateManger2<BasicStateManager> manager;
        public List<Finger> hands;

        public BasicStateManager()
        {
            manager = new StateManger2<BasicStateManager>(this);

            manager.AddState((int)BasicModeStateType.Idle, new IdleState<BasicStateManager>());
            manager.AddState((int)BasicModeStateType.Move, new MoveState<BasicStateManager>());
            manager.AddState((int)BasicModeStateType.LBDown, new LBDownState<BasicStateManager>());
            manager.AddState((int)BasicModeStateType.LBUp, new LBUpState<BasicStateManager>());
            manager.AddState((int)BasicModeStateType.RBDown, new RBDownState<BasicStateManager>());
            manager.AddState((int)BasicModeStateType.RBUp, new RBUpState<BasicStateManager>());
            //manager.AddState((int)StateType.RBUp, new RBUpState<BasicStateManager>());

            manager.AddTransition(-1, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);

            manager.AddTransition((int)BasicModeStateType.Idle, (int)BasicModeTransitionType.Move, (int)BasicModeStateType.Move);
            manager.AddTransition((int)BasicModeStateType.Idle, (int)BasicModeTransitionType.LBDown, (int)BasicModeStateType.LBDown);
            manager.AddTransition((int)BasicModeStateType.Idle, (int)BasicModeTransitionType.RBDown, (int)BasicModeStateType.RBDown);

            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.LBDown, (int)BasicModeStateType.LBDown);
            manager.AddTransition((int)BasicModeStateType.LBDown, (int)BasicModeTransitionType.LBUp, (int)BasicModeStateType.LBUp);
            manager.AddTransition((int)BasicModeStateType.LBUp, (int)BasicModeTransitionType.Move, (int)BasicModeStateType.Move);
            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.RBDown, (int)BasicModeStateType.RBDown);
            manager.AddTransition((int)BasicModeStateType.RBDown, (int)BasicModeTransitionType.RBUp, (int)BasicModeStateType.RBUp);
            manager.AddTransition((int)BasicModeStateType.RBUp, (int)BasicModeTransitionType.Move, (int)BasicModeStateType.Move);


            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.LBDown, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.RBDown, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);

            manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public void Update(List<Finger> hands)
        {
            this.hands = hands;
            manager.Update();
        }

        public string GetCurState()
        {
            return ((BasicModeStateType)manager.GetCurStateType()).ToString();
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

    public interface IStateManger
    {
        void Update(List<Finger> finger);
        string GetCurState();
    }

    public class StateManger2<T>
    {
        public T context;

        IState<T> prevState = null;
        IState<T> curState = null;
        int curStateType = -1;

        //각 stateType에 따른 State정보를 담음
        Dictionary<int, IState<T>> stateDic = new Dictionary<int, IState<T>>();
        //각 transition에 따른 cur/nextState정보를 담음(하나의 transition이 여러 다른 state로 전이하게 하므로 list사용)
        Dictionary<int, List<TransitionInfo>> transitionDic = new Dictionary<int, List<TransitionInfo>>();
        
        //매니저 할 클래스의 현재 정보를 가지고 있어야 모든 state가 동시에 같은 정보에 접근할 수 있음.
        public StateManger2(T context)
        {
            this.context = context;
        }

        //현재 state의 정보만을 update
        public void Update()
        {
            if(curState != null)
            curState.OnUpdate();
        }

        //처음 스테이트 머신 초기화시에 사용할 state를 등록하는 함수
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

        //처음 스테이트 머신에 사용할 transition를 등록하는 함수
        //현재 state의 type, 해당 type에 넣어줄 transitionType, 현재 state에서 transition이 들어올 때 변경할 stateType을 받아서 추가
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

        /// <summary>
        /// transition을 담당해주는 manager함수
        /// transitionType과 현재 state를 체크해서 다음 state가 무엇인지 찾아내고, 해당하는 state로 전이해줌. 
        /// 변경된 state는 OnExit, OnEnter를 사용해 State처리 초기화해줌
        /// </summary>
        /// <param name="transitionType"></param>
        public void Transition(int transitionType)
        {
            int nextStateType = GetTransitionStateType(transitionType);
            if (nextStateType == -1)
                return;

            if (!stateDic.ContainsKey(nextStateType))
            {
                Debug.Log("State Not Added");
                return;
            }

            IState<T> nextState = stateDic[nextStateType];

            //context가 업데이트 되는지 의문이 든당.
            //if (curState == nextState) return; 

            if (curState != nextState)
            {
                Debug.Log("State Change : " + nextState.ToString());
            }

            if (prevState != null)
                prevState.OnExit();

            prevState = curState;
            curState = nextState;
            curState.OnEnter(context);

            curStateType = nextStateType;
        }

        //넘겨온 transitionType과 현재 statetype을 비교하여 
        //Dictionary에 다음 state에 대한 정보가 있으면 해당 type을 리턴,
        //없으면 -1리턴
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

        public int GetCurStateType()
        { return curStateType;  }

    }

    //모든 state의 기본 구조
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
