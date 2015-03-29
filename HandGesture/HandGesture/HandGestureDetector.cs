using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;

namespace HandGesture
{
    class HandGestureDetector : ImageProcessBase, IRecognition
    {
        #region implement IRecognition
        public bool Detect() { return true; }
        public IplImage ExtractRecognitionImageIpl() 
        {
            return extractSkinAsIpl(WebcamController.getImg());
        }
        public Bitmap ExtractRecognitionImageBitmap()
        {
            return ConvertIplToBitmap(extractSkinAsIpl(WebcamController.getImg()));
        }
        public bool RecognitionProcessing() { return true; }
        #endregion
    }
}
