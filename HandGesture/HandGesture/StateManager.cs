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

            //ApiController.GetCursorPos(out x, out y);
            x = context.hands[0].m_centerPoint.X;
            y = context.hands[0].m_centerPoint.Y;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            int i = context.GetHandIdx();


            //if (context.RBFingerLength == -1)
            //{
            //    int rbFingerTipIdx = context.GetRBFingerTipIdx(i);

            //    if (rbFingerTipIdx != -1)
            //    {
            //        int pixelLegth = context.hands[i].GetPixelCntYFingerTip(rbFingerTipIdx);
            //        if (context.hands[i].m_rad <= pixelLegth)
            //            context.RBFingerLength = pixelLegth;
            //    }
            //}

            //if (context.LBFingerLength == -1)
            //{
            //    int lbFingerTipIdx = context.GetLBFingerTipIdx(i);

            //    if (lbFingerTipIdx != -1)
            //    {
            //        int pixelLegth = context.hands[i].GetPixelCntXFingerTip(lbFingerTipIdx);

            //        if (context.hands[i].m_rad <= pixelLegth)
            //            context.LBFingerLength = pixelLegth;
            //    }

            //}

                if (context.IsRightGesture(i, true))
                {
                    context.manager.Transition((int)BasicModeTransitionType.RBDown);
                    return;
                }

                if (context.IsLeftGesture(i, true))
                {
                    context.manager.Transition((int)BasicModeTransitionType.LBDown);
                    return;
                }

                if (!context.IsMoveGesture(i))
                {
                    context.manager.Transition((int)BasicModeTransitionType.Idle);
                    return;
                }

                int dx = context.hands[i].m_centerPoint.X - x;
                int dy = context.hands[i].m_centerPoint.Y - y;
                ApiController.MouseEvent(ApiController.eMouseEventType.MOVEPOS, dx, dy);

                x = context.hands[i].m_centerPoint.X;
                y = context.hands[i].m_centerPoint.Y;
                return;
            
        }

        public override void OnExit()
        {
            base.OnExit();

        }
    }
    public class LBDownState<T> : IState<T> where T : BasicStateManager
    {
        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            //isDown = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (context.isLBDown == false)
            {
                Debug.Log("LB Mouse Down");
                ApiController.MouseEvent(ApiController.eMouseEventType.MOUSEEVENTF_LEFTDOWN);
                context.isLBDown = true;
            }

            int i = context.GetHandIdx();
            if (context.IsLeftGesture(i, false))
            { 
                context.manager.Transition((int)BasicModeTransitionType.LBUp);
                return;
            }

            context.manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            //isDown = false;
        }
    }
    public class LBUpState<T> : IState<T> where T : BasicStateManager
    {
        //private bool isUp = false;

        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            //isUp = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (context.isLBDown == true)
            {
                Debug.Log("LB Mouse Up");
                ApiController.MouseEvent(ApiController.eMouseEventType.MOUSEEVENTF_LEFTUP);
                context.isLBDown = false;
            }

            //int i = context.GetHandIdx();
            //if (context.IsMoveGesture(i))
            //{
            //    context.manager.Transition((int)BasicModeTransitionType.Move);
            //    return;
            //}

            context.manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            //isUp = false;
        }
    }
    public class RBDownState<T> : IState<T> where T : BasicStateManager
    {
        //private bool isDown = false;
        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            //context.IsRBDown = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (context.IsRBDown == false)
            {
                Debug.Log("RB Mouse Down");
                ApiController.MouseEvent(ApiController.eMouseEventType.MOUSEEVENTF_RIGHTDOWN);
                context.IsRBDown = true;
            }

            int i = context.GetHandIdx();
            if (context.IsRightGesture(i, false))
            {
                context.manager.Transition((int)BasicModeTransitionType.RBUp);
                return;
            }

            context.manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            //isDown = false;
        }
    }
    public class RBUpState<T> : IState<T> where T : BasicStateManager
    {
        //private bool isUp = false;

        public override void OnEnter(T t)
        {
            base.OnEnter(t);
            //isUp = false;
            //context.isRBDown = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (context.IsRBDown == true)
            {
                Debug.Log("RB Mouse Up");
                ApiController.MouseEvent(ApiController.eMouseEventType.MOUSEEVENTF_RIGHTUP);
                context.IsRBDown= false;
            }

            int i = context.GetHandIdx();
            //if (context.IsRightGesture(i,false))
            //{
            //    context.manager.Transition((int)BasicModeTransitionType.Move);
            //    return;
            //}

            context.manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public override void OnExit()
        {
            base.OnExit();
            //isUp = false;
        }
    }

    public class IdleState<T> : IState<T> where T : BasicStateManager
    {
        public override void OnUpdate()
        {
 	        base.OnUpdate();
            int i = context.GetHandIdx();
                //move
                if( context.IsMoveGesture(i) )
                {
                    context.IdleLengthOfIf = context.hands[i].GetLengthOfIf();
                    context.manager.Transition((int)BasicModeTransitionType.Move);
                    return;
                }
                
                ////lb down
                //if (context.IsLeftGesture(i, true))
                //{
                //    context.manager.Transition((int)BasicModeTransitionType.LBDown);
                //    return;
                //}

                ////rb down
                //if (context.IsRightGesture(i, true))
                //{
                //    context.manager.Transition((int)BasicModeTransitionType.RBDown);
                //    return;
                //}
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }


    public class BasicStateManager : IStateManger
    {
        public StateManger2<BasicStateManager> manager;
        public List<Finger> hands;
        public double IdleLengthOfIf = 0f;

        public int RBFingerLength = -1;
        public int LBFingerLength = -1;
        public bool isLBDown = false;
        public bool IsRBDown = false;

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
            //manager.AddTransition((int)BasicModeStateType.Idle, (int)BasicModeTransitionType.LBDown, (int)BasicModeStateType.LBDown);
            //manager.AddTransition((int)BasicModeStateType.Idle, (int)BasicModeTransitionType.RBDown, (int)BasicModeStateType.RBDown);

            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.LBDown, (int)BasicModeStateType.LBDown);
            manager.AddTransition((int)BasicModeStateType.LBDown, (int)BasicModeTransitionType.LBUp, (int)BasicModeStateType.LBUp);
            manager.AddTransition((int)BasicModeStateType.LBUp, (int)BasicModeTransitionType.Move, (int)BasicModeStateType.Move);
            manager.AddTransition((int)BasicModeStateType.LBUp, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.RBDown, (int)BasicModeStateType.RBDown);
            manager.AddTransition((int)BasicModeStateType.RBDown, (int)BasicModeTransitionType.RBUp, (int)BasicModeStateType.RBUp);
            manager.AddTransition((int)BasicModeStateType.RBUp, (int)BasicModeTransitionType.Move, (int)BasicModeStateType.Move);
            manager.AddTransition((int)BasicModeStateType.RBUp, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);


            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.LBDown, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.Move, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);
            manager.AddTransition((int)BasicModeStateType.RBDown, (int)BasicModeTransitionType.Idle, (int)BasicModeStateType.Idle);

            manager.Transition((int)BasicModeTransitionType.Idle);
        }

        public void Update(List<Finger> hands)
        {
            this.hands = hands;
            if(hands.Count > 0)
                hands[0].GetFingerType();
            manager.Update();
        }

        public string GetCurState()
        {
            return ((BasicModeStateType)manager.GetCurStateType()).ToString();
        }

        /// <summary>
        /// tipPoiont가 0이 아닌 손의 인덱스를 찾는다. i==0인 경우 주먹인 손이거나, 첫번째 손에 손가락이 펴져 있는 경우다.
        /// </summary>
        /// <returns></returns>
        public int GetHandIdx()
        {
            int i;
            for (i = 0; i < hands.Count && hands[i].m_tipPoint.Count == 0; i++) ;

            if (hands.Count >= i)
                i = hands.Count - 1;

            return i;
        }

        public bool IsMoveGesture(int handIdx)
        {
            return (hands[handIdx].m_tipPoint.Count == 2);

            //bool existType = hands[handIdx].HasFingerType(Finger.FingerType.ForeFinger | Finger.FingerType.IndexFinger);
            //if (existType) 
            //    return true;

            //return false;
        }

        public bool IsRightGesture(int handIdx, bool isDown)
        {

            if (hands[handIdx].m_tipPoint.Count > 1)
            {
                double tempLengthOfIF = hands[handIdx].GetLengthOfIf();
                if (isDown)
                    return (IdleLengthOfIf * 0.6 > tempLengthOfIF);
                if (!isDown)
                    return (IdleLengthOfIf * 0.6 <= tempLengthOfIF);
            }

            return false;

            //int tipIdx = GetRBFingerTipIdx(handIdx);
            //if (tipIdx == -1) return false;
            //double tempRBFingerLength = hands[handIdx].GetPixelCntYFingerTip(tipIdx);
            //if (RBFingerLength == -1) return false;
            //if (hands[handIdx].m_tipPoint.Count > 1)
            //{
            //    if (isDown && RBFingerLength * 0.6f > tempRBFingerLength) 
            //        return true;
            //    if (!isDown && RBFingerLength * 0.8f < tempRBFingerLength) 
            //        return true;
            //}

            //return false;
        }

        public bool IsLeftGesture(int handIdx, bool isDown)
        {
            if (hands[handIdx].m_tipPoint.Count == 1) return true;
            if (isDown && hands[handIdx].GetFingerAngle() < 0.7) return true;
            else if (!isDown && hands[handIdx].GetFingerAngle() > 0.7) return true;

            return false;
            //int tipIdx = GetLBFingerTipIdx(handIdx);
            //if (tipIdx == -1) return false;
            //int tempLBLength = hands[handIdx].GetPixelCntXFingerTip(tipIdx);
            //if (LBFingerLength == -1) return false;
            //if (isDown && hands[handIdx].m_tipPoint.Count == 1)
            //{
            //    if (hands[handIdx].HasFingerType(Finger.FingerType.IndexFinger))
            //        return true;
            //}
            
            //if (isDown && LBFingerLength * 0.5f > tempLBLength) 
            //    return true;
            //if (!isDown && LBFingerLength * 0.5f < tempLBLength) 
            //    return true;

            //return false;
        }

        public int GetRBFingerTipIdx(int handIdx)
        {
            return hands[handIdx].GetFingerTypeTipIdx(Finger.FingerType.IndexFinger);
        }

        public int GetLBFingerTipIdx(int handIdx)
        {
            return hands[handIdx].GetFingerTypeTipIdx(Finger.FingerType.ForeFinger);
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
#if DEBUG
                if(curState != null && nextState != null)
                    Debug.Log(curState.ToString() + " ------>> " + nextState.ToString());
#endif
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
