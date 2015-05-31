using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace HandGesture
{
    //public class FPSGestureDetector : ImageProcessBase, IRecognition
    //{
    //    #region member
    //    private CvSize? _monitorSize = null;
    //    public CvSize? MonitorSize
    //    {
    //        get
    //        {
    //            if (_monitorSize == null)
    //            {
    //                _monitorSize = new CvSize(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
    //                                          System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
    //            }

    //            return _monitorSize;
    //        }
    //    }
    //    FPSStateManager manager = null;
    //    private IplImage resultImg;
    //    private IplImage filterImg;
    //    private CvSeq<CvPoint> contours;
    //    private int[] hull;
    //    private CvMemStorage storage = new CvMemStorage();
    //    private CvSeq<CvConvexityDefect> defect;
    //    private int maxDist = 0;

    //    public IplImage FilterImg { get { return filterImg; } }
    //    public IplImage BlobImg { get; set; }
    //    public IplImage ConvexHullImg { get; set; }
    //    public IplImage ResultImg { get { return resultImg; } }
    //    public int MaxDist { get { return maxDist; } }
    //    #endregion

    //    #region implement IRecognition
    //    public bool Detect()
    //    {
    //        if (manager == null)
    //            manager = new FPSStateManager();

    //        //컬러맵 변환
    //        IplImage webcamImg = WebcamController.Instance.WebcamImage;
    //        if (webcamImg == null) return false;

    //        IplImage origin = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 3);
    //        webcamImg.CvtColor(origin, ColorConversion.BgrToCrCb);

    //        //피부색 검출
    //        filterImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
    //        origin.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), filterImg);
    //        Interpolate(filterImg);

    //        List<IplImage> listOfBlobImg = FilterBlobImgList(filterImg, BlobImg);
    //        List<Finger> fingers = new List<Finger>();

    //        //BlobImg = resultImg;
    //        int cnt = listOfBlobImg.Count;
    //        if (cnt == 0) return false;

    //        resultImg = webcamImg.Clone();
    //        for (int k = 0; k < cnt; k++)
    //        {
    //            if (!FindContours(listOfBlobImg[k], storage, out contours))
    //            {
    //                return false;
    //            }
    //            contours.ConvexHull2(out hull, ConvexHullOrientation.Counterclockwise);
    //            defect = contours.ConvexityDefects(hull);

    //            int cntFinger = 0;
    //            CvSeq<CvPoint> c = contours;
    //            CvPoint conCenter = new CvPoint(0, 0);
    //            double conDist = -1, maxConDist = -1;
    //            int x, y, height, width;
    //            CvRect roi = c.BoundingRect();
    //            x = roi.X; y = roi.Y; height = roi.Height; width = roi.Width;
    //            if (height > width * 1.5) height = (int)(width * 1.5);
    //            for (int i = x; i < x + width; i++)
    //            {
    //                for (int j = y; j < y + height; j++)
    //                {
    //                    conDist = Cv.PointPolygonTest(c, new CvPoint(i, j), true);
    //                    if (conDist > maxConDist)
    //                    {
    //                        maxConDist = conDist;
    //                        conCenter.X = i;
    //                        conCenter.Y = j;
    //                    }
    //                }
    //            }
    //            resultImg.DrawCircle(conCenter, 2, CvColor.White, -1);
    //            if (maxConDist > 0)
    //            {
    //                Finger finger = new Finger(conCenter, (int)maxConDist);
    //                fingers.Add(finger);
    //                //resultImg.DrawCircle(conCenter, (int)maxConDist, CvColor.Violet);
    //            }
    //            else continue;

    //            foreach (CvConvexityDefect ccd in defect)
    //            {
    //                if (ccd.End.Y < conCenter.Y + maxConDist/2)
    //                {
    //                    int dis = (int)ccd.End.DistanceTo(conCenter);
    //                    if (dis < maxConDist * 1.6) continue;
    //                    fingers[k].addTip(ccd.End);
    //                    fingers[k].addDepth(ccd.DepthPoint);

    //                    resultImg.DrawCircle(ccd.End, 2, CvColor.Red, -1);
    //                    //resultImg.DrawLine(ccd.End, conCenter, CvColor.Aqua);
    //                    resultImg.DrawLine(conCenter, ccd.DepthPoint, CvColor.Aqua);
    //                    cntFinger++;
    //                    resultImg.PutText(cntFinger.ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Black);
    //                    //resultImg.PutText(((int)fingers[k].GetFingerAngle2(ccd.DepthPoint, conCenter)).ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
    //                }
    //            }
    //        }

    //        manager.Update(fingers);


    //        return true;
    //    }

    //    public IplImage ExtractRecognitionImageIpl()
    //    {
    //        return resultImg;
    //    }
    //    public Bitmap ExtractRecognitionImageBitmap()
    //    {
    //        return ConvertIplToBitmap(resultImg);
    //    }
    //    public bool RecognitionProcessing() { return true; }
    //    #endregion

    //}
}
