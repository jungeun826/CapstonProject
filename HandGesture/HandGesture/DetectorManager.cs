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

        public DetectorMode DetectMode { get; set; }

        private Dictionary<DetectorMode, List<IRecognition>> detectorDic = new Dictionary<DetectorMode, List<IRecognition>>();

        public void Init()
        {

        }

        public void Init(HandGestureDetector handGestureDetector)
        {
            if (!detectorDic.ContainsKey(DetectorMode.Basic))
            {
                List<IRecognition> list = new List<IRecognition>();
                list.Add(handGestureDetector);
                detectorDic.Add(DetectorMode.Basic, list);
            }
            else
            {
                List<IRecognition> list = detectorDic[DetectorMode.Basic];
                list.Add(handGestureDetector);
            }
        }

        public void UpdateManager()
        {
            switch (DetectMode)
            {
                case DetectorMode.Basic:
                    foreach (IRecognition recogn in detectorDic[DetectMode])
                    {
                        if (recogn.Detect())
                        {
                            //이거 계속 출력되는거 거슬려서 주석처리함 by.yong
                            //Console.WriteLine("dd");
                        }
                    }
                    break;
                case DetectorMode.FPS:
                    break;
                case DetectorMode.Racing:
                    break;
                case DetectorMode.Custom:
                    break;
                default:
                    break;
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
