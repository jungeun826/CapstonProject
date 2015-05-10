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

        private IplImage resultImg;
        private IplImage filterImg;
        private CvSeq<CvPoint> contours;
        private int[] hull;
        private CvMemStorage storage = new CvMemStorage();
        private CvSeq<CvConvexityDefect> defect;
        private int maxDist = 0;
        public CvPoint? centerPoint;

        public IplImage FilterImg { get { return filterImg; } }
        public IplImage BlobImg { get; set; }
        public IplImage ConvexHullImg { get; set; }
        public IplImage ResultImg { get { return resultImg; } }
        public int MaxDist { get { return maxDist; } }
        public bool Detect() 
        {
            //컬러맵 변환
            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            IplImage origin = webcamImg.Clone();
            IplImage origin2 = webcamImg.Clone();
            origin.CvtColor(origin, ColorConversion.BgrToCrCb);
            //피부색 검출
            filterImg = new IplImage(origin.Width, origin.Height, BitDepth.U8, 1);
            origin.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), filterImg);

            Interpolate(filterImg);

            //filterImg = extractSkinAsIpl(WebcamController.Instance.getImg());
            CvPoint? tempCenter = null;
            if (!FilterByMaximalBlob(filterImg, out resultImg, out maxDist, out tempCenter))
                return false;
            centerPoint = tempCenter;
            BlobImg = resultImg;
            
            // 1.
            // 해상도 구하기
            // 해상도 전체 좌표에서 포인팅 된 곳 맵핑
            float ratioX = MonitorSize.Value.Width / resultImg.Width;
            float ratioY = MonitorSize.Value.Height / resultImg.Height;
            // 해당 위치로 마우스 이동
            ApiController.SetCursorPos((int)(ratioX * centerPoint.Value.X), (int)(ratioY * centerPoint.Value.Y));

            // 2.
            // 해상도 구하기
            // 해상도 전체 좌표에서 포인팅 되어 있던 곳을 맵핑
            // 움직인 벡터 만큼ㄷ의 값을 맵핑
            // 현재 좌표에서 움직인 벡터만큼 이동시키기

            if (!FindContours(resultImg, storage, out contours))
                return false;

            contours.ConvexHull2(out hull, ConvexHullOrientation.Counterclockwise);
            defect = contours.ConvexityDefects(hull);
            DrawDefects(ref origin2, defect);
            ConvexHullImg = origin2;
            //pixel to radius 필요
            resultImg.DrawCircle(centerPoint.Value, (int)(maxDist / 4.5), CvColor.Navy);
            resultImg.DrawCircle(centerPoint.Value, (int)(maxDist / 3), CvColor.Navy);

            resultImg.DrawCircle(centerPoint.Value, (int)(maxDist/1.7), CvColor.Navy);

            return true; 
        }
        public IplImage ExtractRecognitionImageIpl()
        {
            return resultImg;
        }
        public Bitmap ExtractRecognitionImageBitmap()
        {
            return ConvertIplToBitmap(resultImg);
        }
        public bool RecognitionProcessing() { return true; }
        #endregion
    }
}
