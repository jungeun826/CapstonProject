using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;

namespace HandGesture
{
    interface IRecognition
    {
        //내부적으로 WebcamController를 얻어와서 처리함.
        List<Finger> Detect();
        IplImage ExtractRecognitionImageIpl();
        Bitmap ExtractRecognitionImageBitmap();
        bool RecognitionProcessing();
    }
}
