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

    class DetectorManager : Base.Singletone<DetectorManager>, Base.ISingleTon
    {

        public DetectorMode DetectMode { get; private set; }
        public HandGestureDetector handDetector = new HandGestureDetector();
        private CvSize? _monitorSize = null;
        public CvSize? MonitorSize
        {
            get
            {
                if (_monitorSize == null)
                {
                    _monitorSize = new CvSize(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                                              System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                }

                return _monitorSize;
            }
        }
        //지금 생각해보니 list일 필요는 없다.
        //예전에는 Fps인지, 뭔지 하나씩 처리한다는 말이 없어서 리스트로 해놓았음..
        //하지만 변경 귀차나서 그냥 씀..
        //private Dictionary<DetectorMode, List<IRecognition>> detectorDic = new Dictionary<DetectorMode, List<IRecognition>>();
        //private Dictionary<DetectorMode, IStateManger> fsmDic = new Dictionary<DetectorMode, IStateManger>();
        

        public void Init()
        {
            
        }

        public void Init(DetectorMode mode)
        {
            switch (mode)
            {
                case DetectorMode.Basic:
                    //basicStateManager = new StateManager();
                    //fsmDic.Add(mode, basicStateManager);
                    break;
                case DetectorMode.FPS:
                    //FPSStateManager FPSStateManager = new FPSStateManager();
                    //fsmDic.Add(mode, FPSStateManager);
                    break;
                case DetectorMode.Racing:
                    //RacingStateManager racingStateManager = new RacingStateManager();
                    //fsmDic.Add(mode, racingStateManager);
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
#if PCVer
            switch (this.DetectMode)
            {
                case DetectorMode.Basic:
                    WebcamController.Instance.PlayFileName = "hand5.avi";
                    break;
                case DetectorMode.FPS:
                    WebcamController.Instance.PlayFileName = "hand6.avi";
                    break;
                case DetectorMode.Racing:
                    //ApiController.keybd_event((uint)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Tab), 0, 0x00, 0);
                    
                    WebcamController.Instance.PlayFileName = "hand4.avi";
                    break;
                case DetectorMode.Custom:
                    //BasicStateManager stateManager = new BasicStateManager();
                    //fsmDic.Add(mode, stateManager);
                    break;
                default:
                    break;
            }
#endif
        }



        public void UpdateManager()
        {
            List<Finger> hands = handDetector.Detect();

            if (hands == null) return;
            ProcessStateManager(hands);
            //if (!fsmDic.ContainsKey(DetectMode))
            //    return;

            //fsmDic[DetectMode].Update(fingers);
        }

        public void ProcessStateManager(List<Finger> hands)
        {
            switch (DetectMode)
            {
                case DetectorMode.Basic:
                    BasicStateManager.Update(hands);
                    break;
                case DetectorMode.FPS:
                    FPSStateManager.Update(hands);
                    break;
                case DetectorMode.Racing:
                    RacingStateManager.Update(hands);
                    break;
                case DetectorMode.Custom:
                    break;
                default:
                    break;
            }
        }

        //public string GetCurState()
        //{
        //    if (!fsmDic.ContainsKey(DetectMode))
        //        return "Not contains fsm / Mode : " + DetectMode.ToString();

        //    return fsmDic[DetectMode].GetCurState();
        //}
    }
}
