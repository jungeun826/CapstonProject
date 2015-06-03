using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace HandGesture
{
    public enum DetectorMode
    {
        Basic,
        FPS,
        Racing,
        Custom,
    }

    public enum GestureType
    {
        RightClick,
        LeftClick,
        Point,
    }

    class DetectorManager : Base.Singletone<DetectorManager>, Base.ISingleTon
    {

        public DetectorMode DetectMode { get; private set; }
        public HandGestureDetector handDetector = new HandGestureDetector();

        //지금 생각해보니 list일 필요는 없다.
        //예전에는 Fps인지, 뭔지 하나씩 처리한다는 말이 없어서 리스트로 해놓았음..
        //하지만 변경 귀차나서 그냥 씀..
        //private Dictionary<DetectorMode, List<IRecognition>> detectorDic = new Dictionary<DetectorMode, List<IRecognition>>();
        private Dictionary<DetectorMode, IStateManger> fsmDic = new Dictionary<DetectorMode, IStateManger>();
        

        public void Init()
        {
            
        }

        public void Init(DetectorMode mode)
        {
            switch (mode)
            {
                case DetectorMode.Basic:
                    BasicStateManager basicStateManager = new BasicStateManager();
                    fsmDic.Add(mode, basicStateManager);
                    break;
                case DetectorMode.FPS:
                    FPSStateManager FPSStateManager = new FPSStateManager();
                    fsmDic.Add(mode, FPSStateManager);
                    break;
                case DetectorMode.Racing:
                    //BasicStateManager stateManager = new BasicStateManager();
                    //fsmDic.Add(mode, stateManager);
                    break;
                case DetectorMode.Custom:
                    //BasicStateManager stateManager = new BasicStateManager();
                    //fsmDic.Add(mode, stateManager);
                    break;
                default:
                    break;
            }
        }

        public void ChangeDetectMode(DetectorMode changeMode)
        {
            this.DetectMode = changeMode;
        }


        public void UpdateManager()
        {
            List<Finger> fingers = handDetector.Detect();

            if (fingers == null) return;

            if (!fsmDic.ContainsKey(DetectMode))
                return;

            fsmDic[DetectMode].Update(fingers);
        }

        public string GetCurStateString()
        {
            if (!fsmDic.ContainsKey(DetectMode))
                return "Not contains fsm / Mode : " + DetectMode.ToString();

            return fsmDic[DetectMode].GetCurStateString();
        }

        public int GetCurState()
        {
            if (!fsmDic.ContainsKey(DetectMode))
                return -1; //"Not contains fsm / Mode : " + DetectMode.ToString();

            return fsmDic[DetectMode].GetCurState();
        }

    }
}
