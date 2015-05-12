using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Blob;

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
            CvPoint moustPos = new CvPoint(-1, -1);

            //컬러맵 변환
            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            if (webcamImg == null) return false;

            IplImage origin = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 3);
            webcamImg.CvtColor(origin, ColorConversion.BgrToCrCb);
            //피부색 검출
            filterImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
            origin.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), filterImg);
            Interpolate(filterImg);

            List<IplImage> listOfBlobImg = FilterBlobImgList(filterImg, BlobImg);
            //BlobImg = resultImg;
            int cnt = listOfBlobImg.Count;
            if (cnt == 0) return false;

            resultImg = webcamImg.Clone();
            for (int k = 0; k < cnt; k++)
            {
                if (!FindContours(listOfBlobImg[k], storage, out contours))
                {
                    return false;
                }
                contours.ConvexHull2(out hull, ConvexHullOrientation.Counterclockwise);
                defect = contours.ConvexityDefects(hull);

                int cntFinger = 0;
                CvSeq<CvPoint> c = contours;
                CvPoint conCenter = new CvPoint(0, 0);
                double conDist = -1, maxConDist = -1;
                int x, y, height, width;
                CvRect roi = c.BoundingRect();
                x = roi.X; y = roi.Y; height = roi.Height; width = roi.Width;
                if (height > width * 1.5) height = (int)(width * 1.5);
                for (int i = x; i < x + width; i++)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        conDist = Cv.PointPolygonTest(c, new CvPoint(i, j), true);
                        if (conDist > maxConDist)
                        {
                            maxConDist = conDist;
                            conCenter.X = i;
                            conCenter.Y = j;
                        }
                    }
                }
                resultImg.DrawCircle(conCenter, 2, CvColor.White, -1);
                if (maxConDist > 0)
                    resultImg.DrawCircle(conCenter, (int)maxConDist, CvColor.Violet);

                foreach (CvConvexityDefect ccd in defect)
                {
                    if (ccd.End.Y < conCenter.Y)
                    {
                        int dis = (int)ccd.End.DistanceTo(conCenter);
                        if (dis < maxConDist * 1.6) continue;
                        resultImg.DrawCircle(ccd.End, 2, CvColor.Red, -1);
                        resultImg.DrawLine(ccd.End, conCenter, CvColor.Aqua);
                        resultImg.DrawLine(ccd.End, ccd.DepthPoint, CvColor.Red);
                        //resultImg.PutText(((int)(maxDist/dis)).ToString(), ccd.End, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
                        cntFinger++;
                        resultImg.PutText(cntFinger.ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
                    }
                }
                if (cntFinger == 1)
                {
                    moustPos = conCenter;
                }
                resultImg.PutText(cntFinger.ToString(), conCenter, new CvFont(FontFace.HersheyComplex, 1, 1), new CvScalar(255, 255, 255));
            }

            // 1.
            // 해상도 구하기
            // 해상도 전체 좌표에서 포인팅 된 곳 맵핑
            float ratioX = MonitorSize.Value.Width / resultImg.Width;
            float ratioY = MonitorSize.Value.Height / resultImg.Height;
            // 해당 위치로 마우스 이동
            if (moustPos.X != -1)//맘대로움직이지마 ㅡㅡ 마우스야 by.Yong
                ApiController.SetCursorPos((int)(ratioX * moustPos.X), (int)(ratioY * moustPos.Y));


            return true;
        }

        //public bool Detect()
        //{
        //    //컬러맵 변환
        //    IplImage webcamImg = WebcamController.Instance.WebcamImage;
        //    if (webcamImg == null) return false;

        //    IplImage origin = webcamImg.Clone();
        //    IplImage origin2 = webcamImg.Clone();
        //    origin.CvtColor(origin, ColorConversion.BgrToCrCb);
        //    //피부색 검출
        //    filterImg = new IplImage(origin.Width, origin.Height, BitDepth.U8, 1);
        //    origin.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), filterImg);

        //    Interpolate(filterImg);

        //    //filterImg = extractSkinAsIpl(WebcamController.Instance.getImg());
        //    CvPoint? tempCenter = null;
        //    //if (!FilterByMaximalBlob(filterImg, out resultImg, out maxDist, out tempCenter))
        //    //    return false;
        //    //centerPoint = tempCenter;
        //    //BlobImg = resultImg;
        //    if (!FilterByMaximalBlob(filterImg, out resultImg))
        //        return false;
        //    BlobImg = resultImg.Clone();
        //    BlobImg.PutText("blob result img", new CvPoint(10, 10), new CvFont(FontFace.HersheyComplex, 0.4, 0.4), CvColor.White);

        //    if (!FindContours(resultImg, storage, out contours))
        //        return false;
        //    contours.ConvexHull2(out hull, ConvexHullOrientation.Counterclockwise);
        //    defect = contours.ConvexityDefects(hull);
        //    //DrawDefects(ref origin2, defect); //이거는 느려질까봐 빼둘께요 by.Yong
        //    ConvexHullImg = origin2;
        //    ////pixel to radius 필요
        //    //resultImg.DrawCircle(centerPoint.Value, (int)(maxDist / 4.5), CvColor.Navy);
        //    //resultImg.DrawCircle(centerPoint.Value, (int)(maxDist / 3), CvColor.Navy);
        //    //resultImg.DrawCircle(centerPoint.Value, (int)(maxDist / 1.7), CvColor.Navy);

        //    #region 손가락 개수 세기...
        //    ////손가락개수를 세자 ... by.Yong
        //    ///* 두꺼운 원으로 마스킹해서 손가락을 가져오는곳 */
        //    //IplImage temp = resultImg.Clone();
        //    //temp.Zero();
        //    //temp.DrawCircle(centerPoint.Value, (int)(maxDist / 2.5), CvColor.Aqua, 15);
        //    //resultImg.And(temp, resultImg);
        //    //resultImg.Erode(resultImg, null, 1);
        //    //////////////////////////////////////////////////

        //    ///*위의 마스킹된 손가락의 겹쳐지는 부분을 라벨링 이용해 카운트 */
        //    //CvBlobs blobs = new CvBlobs();
        //    //blobs.Label(resultImg); //blobs.Count를 이용한다. 손목도 있으니까 1이 모든 손가락이 접힌거
        //    //String msg;
        //    //msg = blobs.Count == 1 ? "fist" : "hand";
        //    //resultImg.PutText(msg, new CvPoint(10, 20), new CvFont( FontFace.HersheyComplex, 1, 1), new CvScalar(255, 255, 255));
        //    ////////////////////////////////////////////////////////////////


        //    //이건 다른거...
        //    resultImg = webcamImg.Clone();
        //    //resultImg.DrawCircle(conCenter, 2, CvColor.Blue, -1);

        //    CvSeq<CvPoint> c = contours;
        //    CvPoint conCenter = new CvPoint(0, 0);
        //    double conDist = -1, maxConDist = -1;
        //    int x, y, height, width;
        //    CvRect roi = c.BoundingRect();

        //    x = roi.X; y = roi.Y; height = roi.Height; width = roi.Width;
        //    if (height > width) height = width;
        //    for (int i = x; i < resultImg.Width; i++)
        //    {
        //        for (int j = y; j < resultImg.Height; j++)
        //        {
        //            conDist = Cv.PointPolygonTest(c, new CvPoint(i, j), true);
        //            if (conDist > maxConDist)
        //            {
        //                maxConDist = conDist;
        //                conCenter.X = i;
        //                conCenter.Y = j;
        //            }
        //        }
        //    }
        //    resultImg.DrawCircle(conCenter, 2, CvColor.White, -1);
        //    resultImg.DrawCircle(conCenter, (int)maxConDist, CvColor.Violet);

        //    int cntFinger = 0;
        //    foreach (CvConvexityDefect ccd in contours.ConvexityDefects(hull))
        //    {
        //        //이건 center가 countour를 그리는 중심이랑 다른 포인트라서 떄려침
        //        if (ccd.End.Y < conCenter.Y)
        //        {
        //            int dis = (int)ccd.End.DistanceTo(conCenter);
        //            if (dis < maxDist / 4) continue;
        //            resultImg.DrawCircle(ccd.End, 2, CvColor.Red, -1);
        //            resultImg.DrawLine(ccd.End, conCenter, CvColor.Aqua);
        //            resultImg.DrawLine(ccd.End, ccd.DepthPoint, CvColor.Red);
        //            //resultImg.PutText(((int)(maxDist/dis)).ToString(), ccd.End, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
        //            cntFinger++;
        //            resultImg.PutText(cntFinger.ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);

        //            //if (ccd.DepthPoint.Y > conCenter.Y)//엄지인식은 무슨...
        //            //{
        //            //    resultImg.PutText("tumb", ccd.End, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.SteelBlue);
        //            //}
        //        }
        //    }
        //    resultImg.PutText(cntFinger.ToString(), new CvPoint(10, 50), new CvFont(FontFace.HersheyComplex, 1, 1), new CvScalar(255, 255, 255));
        //    #endregion

        //    centerPoint = conCenter;
        //    // 1.
        //    // 해상도 구하기
        //    // 해상도 전체 좌표에서 포인팅 된 곳 맵핑
        //    float ratioX = MonitorSize.Value.Width / resultImg.Width;
        //    float ratioY = MonitorSize.Value.Height / resultImg.Height;
        //    // 해당 위치로 마우스 이동
        //    if (false)//맘대로움직이지마 ㅡㅡ 마우스야 by.Yong
        //        ApiController.SetCursorPos((int)(ratioX * centerPoint.Value.X), (int)(ratioY * centerPoint.Value.Y));

        //    // 2.
        //    // 해상도 구하기
        //    // 해상도 전체 좌표에서 포인팅 되어 있던 곳을 맵핑
        //    // 움직인 벡터 만큼ㄷ의 값을 맵핑
        //    // 현재 좌표에서 움직인 벡터만큼 이동시키기

        //    return true;
        //}
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
