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
        #region member
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
        //private IplImage filterImg;
        private int maxDist = 0;

        //public IplImage FilterImg { get { return filterImg; } }
        public IplImage BlobImg { get; set; }
        public IplImage ConvexHullImg { get; set; }
        public IplImage ResultImg { get { return resultImg; } }
        public int MaxDist { get { return maxDist; } }
        private  List<Finger> fingers = new List<Finger>();

        #endregion

        #region implement IRecognition
        public List<Finger> Detect()
        {
            fingers.Clear();
            //컬러맵 변환
            IplImage webcamImg = WebcamController.Instance.WebcamImage;
            if (webcamImg == null) return null;
            resultImg = new IplImage(webcamImg.Size, BitDepth.U8, 3);
            Cv.Copy(webcamImg, resultImg);

            using (var imgSrc = webcamImg.Clone())
            using (var imgHSV = new IplImage(imgSrc.Size, BitDepth.U8, 3))
            using (var imgH = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgS = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgV = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgBackProjection = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgFlesh = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgHull = new IplImage(imgSrc.Size, BitDepth.U8, 1))
            using (var imgDefect = new IplImage(imgSrc.Size, BitDepth.U8, 3))
            using (var imgContour = new IplImage(imgSrc.Size, BitDepth.U8, 3))
            using (var storage = new CvMemStorage())
            {
                // RGB -> HSV
                Cv.CvtColor(imgSrc, imgHSV, ColorConversion.BgrToHsv);
                Cv.CvtPixToPlane(imgHSV, imgH, imgS, imgV, null);
                IplImage[] hsvPlanes = { imgH, imgS, imgV };

                // skin region
                RetrieveFleshRegion(imgSrc, hsvPlanes, imgBackProjection);
                List<IplImage> listOfBlobImg = FilterBlobImgList(imgBackProjection);
                int blobCnt = listOfBlobImg.Count;

                for (int k = 0; k < blobCnt ; k++)
                {
                    Cv.Copy(listOfBlobImg[k], imgFlesh);
                    CvSeq<CvPoint> contours = FindContours(listOfBlobImg[k], storage);
                    if (contours == null)
                    {return null;}

                    int cntFinger = 0;
                    CvPoint conCenter = new CvPoint(0, 0);
                    double conDist = -1, maxConDist = -1;
                    int x, y, height, width;
                    CvRect roi = contours.BoundingRect();
                    x = roi.X; y = roi.Y; height = roi.Height; width = roi.Width;
                    if (height > width * 1.5) height = (int)(width * 1.5);
                    for (int i = x; i < x + width; i++)
                    {
                        for (int j = y; j < y + height; j++)
                        {
                            conDist = Cv.PointPolygonTest(contours, new CvPoint(i, j), true);
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
                    {
                        Finger finger = new Finger(conCenter, (int)maxConDist);
                        fingers.Add(finger);
                        //resultImg.DrawCircle(conCenter, (int)maxConDist, CvColor.Violet);
                    }
                    else continue;

                    Cv.DrawContours(imgContour, contours, CvColor.Red, CvColor.Green, 0, 3, LineType.AntiAlias);

                    // finds convex hull
                    int[] hull;
                    Cv.ConvexHull2(contours, out hull, ConvexHullOrientation.Clockwise);
                    Cv.Copy(imgFlesh, imgHull);
                    DrawConvexHull(contours, hull, resultImg);

                    // gets convexity defexts
                    Cv.Copy(imgContour, imgDefect);
                    CvSeq<CvConvexityDefect> defect = Cv.ConvexityDefects(contours, hull);

                    var tempLoop = defect.ToArray();
                    foreach (CvConvexityDefect ccd in tempLoop)
                    {
                        if (ccd.End.Y < conCenter.Y + maxConDist / 2)
                        {
                            int dis = (int)ccd.End.DistanceTo(conCenter);
                            if (dis < maxConDist * 1.6) continue;
                            fingers[k].addTip(ccd.End);
                            fingers[k].addDepth(ccd.DepthPoint);

                            resultImg.DrawCircle(ccd.End, 2, CvColor.Red, -1);
                            //resultImg.DrawLine(ccd.End, conCenter, CvColor.Aqua);
                            resultImg.DrawLine(conCenter, ccd.DepthPoint, CvColor.Aqua);
                            cntFinger++;
                            resultImg.PutText(cntFinger.ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Black);
                            //resultImg.PutText(((int)fingers[k].GetFingerAngle2(ccd.DepthPoint, conCenter)).ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
                        }
                    }
                }
            }

            return fingers;
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

        /// <summary>
        /// Gets flesh regions by histogram back projection
        /// </summary>
        /// <param name="imgSrc"></param>
        /// <param name="hsvPlanes"></param>
        /// <param name="imgRender"></param>
        private void RetrieveFleshRegion(IplImage imgSrc, IplImage[] hsvPlanes, IplImage imgDst)
        {
            int[] histSize = new int[] { 30, 32 };
            float[] hRanges = { 0.0f, 20f };
            float[] sRanges = { 50f, 255f };
            float[][] ranges = { hRanges, sRanges };

            imgDst.Zero();
            using (CvHistogram hist = new CvHistogram(histSize, HistogramFormat.Array, ranges, true))
            {
                hist.Calc(hsvPlanes, false, null);
                float minValue, maxValue;
                hist.GetMinMaxValue(out minValue, out maxValue);
                hist.Normalize(imgSrc.Width * imgSrc.Height * 255 / maxValue);
                hist.CalcBackProject(hsvPlanes, imgDst);
            }
        }

        /// <summary>
        /// Gets the largest blob
        /// </summary>
        /// <param name="imgSrc"></param>
        /// <param name="imgRender"></param>
        private void FilterByMaximumBlob(IplImage imgSrc, IplImage imgDst)
        {
            CvBlobs blobs = new CvBlobs();

            imgDst.Zero();
            blobs.Label(imgSrc);
            CvBlob max = blobs.GreaterBlob();
            if (max == null)
                return;
            blobs.FilterByArea(max.Area, max.Area);
            blobs.FilterLabels(imgDst);
        }

        /// <summary>
        /// Opening
        /// </summary>
        /// <param name="img"></param>
        private void Interpolate(IplImage img)
        {
            Cv.Dilate(img, img, null, 2);
            Cv.Erode(img, img, null, 2);
        }

        /// <summary>
        /// Find contours
        /// </summary>
        /// <param name="img"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        private CvSeq<CvPoint> FindContours(IplImage img, CvMemStorage storage)
        {
            // 輪郭抽出
            CvSeq<CvPoint> contours;
            using (IplImage imgClone = img.Clone())
            {
                Cv.FindContours(imgClone, storage, out contours);
                if (contours == null)
                {
                    return null;
                }
                contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 3, true);
            }
            // 一番長そうな輪郭のみを得る
            CvSeq<CvPoint> max = contours;
            for (CvSeq<CvPoint> c = contours; c != null; c = c.HNext)
            {
                if (max.Total < c.Total)
                {
                    max = c;
                }
            }
            return max;
        }

        /// <summary>
        /// ConvexHullの描画
        /// </summary>
        /// <param name="contours"></param>
        /// <param name="hull"></param>
        /// <param name="img"></param>
        private void DrawConvexHull(CvSeq<CvPoint> contours, int[] hull, IplImage img)
        {
            CvPoint pt0 = contours[hull.Last()].Value;
            foreach (int idx in hull)
            {
                CvPoint pt = contours[idx].Value;
                Cv.Line(img, pt0, pt, new CvColor(255, 255, 255));
                pt0 = pt;
            }
        }

        /// <summary>
        /// ConvexityDefectsの描画
        /// </summary>
        /// <param name="img"></param>
        /// <param name="defect"></param>
        private void DrawDefects(IplImage img, CvSeq<CvConvexityDefect> defect)
        {
            int count = 0;
            foreach (CvConvexityDefect item in defect)
            {
                CvPoint p1 = item.Start, p2 = item.End;
                double dist = GetDistance(p1, p2);
                CvPoint2D64f mid = GetMidpoint(p1, p2);
                img.DrawLine(p1, p2, CvColor.White, 3);
                img.DrawCircle(item.DepthPoint, 10, CvColor.Green, -1);
                img.DrawLine(mid, item.DepthPoint, CvColor.White, 1);
                Console.WriteLine("No:{0} Depth:{1} Dist:{2}", count, item.Depth, dist);
                count++;
            }
        }

        /// <summary>
        /// 2点間の距離を得る
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double GetDistance(CvPoint p1, CvPoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        /// <summary>
        /// 2点の中点を得る
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private CvPoint2D64f GetMidpoint(CvPoint p1, CvPoint p2)
        {
            return new CvPoint2D64f
            {
                X = (p1.X + p2.X) / 2.0,
                Y = (p1.Y + p2.Y) / 2.0
            };
        }
    }
}
