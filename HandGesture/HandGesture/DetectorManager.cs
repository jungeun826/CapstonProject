using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace HandGesture
{
    public enum GestureType
    {
        RightClick,
        LeftClick,
        Point,
    }

    class DetectorManager : Base.Singletone<DetectorManager>, Base.ISingleTon
    {
        private HandGestureDetector handGestureDetector = null;
        
        public void Init()
        {
            handGestureDetector = new HandGestureDetector();
        }

        public Bitmap GetBitmapImage(GestureType type)
        {
            switch (type)
            {
                case GestureType.Point:
                    return handGestureDetector.ExtractRecognitionImageBitmap();
            }
            return null;
        }

    }
}
