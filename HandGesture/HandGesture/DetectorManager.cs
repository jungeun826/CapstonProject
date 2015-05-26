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

        //지금 생각해보니 list일 필요는 없다.
        //예전에는 Fps인지, 뭔지 하나씩 처리한다는 말이 없어서 리스트로 해놓았음..
        //하지만 변경 귀차나서 그냥 씀..
        private Dictionary<DetectorMode, List<IRecognition>> detectorDic = new Dictionary<DetectorMode, List<IRecognition>>();

        public void Init()
        {

        }

        public void Init(DetectorMode mode, IRecognition detector)
        {
            if (!detectorDic.ContainsKey(mode))
            {
                List<IRecognition> list = new List<IRecognition>();
                list.Add(detector);
                detectorDic.Add(mode, list);
            }
            else
            {
                List<IRecognition> list = detectorDic[mode];
                list.Add(detector);
            }
        }

        public void ChangeDetectMode(DetectorMode changeMode)
        {
            this.DetectMode = changeMode;
        }


        public void UpdateManager()
        {
            //switch (DetectMode)
            //{
            //    case DetectorMode.Basic:
                    
            //        break;
            //    case DetectorMode.FPS:

            //        break;
            //    case DetectorMode.Racing:
            //        break;
            //    case DetectorMode.Custom:
            //        break;
            //    default:
            //        break;
            //}
            foreach (IRecognition recogn in detectorDic[DetectMode])
            {
                if (recogn.Detect())
                {
                    //이거 계속 출력되는거 거슬려서 주석처리함 by.yong
                    //Debug.Log("Detect HandGesture : " + DetectMode.ToString() + " Mode");
                }
            }
        }

        //public Bitmap GetBitmapImage(GestureType type)
        //{
        //    switch (type)
        //    {
        //        case GestureType.Point:
        //            return handGestureDetector.ExtractRecognitionImageBitmap();
        //    }
        //    return null;
        //}

    }
}
