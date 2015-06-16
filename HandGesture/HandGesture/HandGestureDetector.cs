using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.Extensions;

namespace HandGesture
{

    class HandGestureDetector
    {
        #region member
        
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
            using (var imgYCBCR = new IplImage(imgSrc.Size, BitDepth.U8, 3))
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
                resultImg.CvtColor(imgYCBCR, ColorConversion.BgrToCrCb);

                //피부색 검출
                imgYCBCR.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), imgBackProjection);

                imgBackProjection.Dilate(imgBackProjection, null, 2);
                imgBackProjection.Erode(imgBackProjection, null, 2);
                imgBackProjection.Smooth(imgBackProjection, SmoothType.Gaussian);

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

#if DEBUG
                    resultImg.DrawCircle(conCenter, 2, CvColor.White, -1);
                    resultImg.DrawCircle(conCenter, (int)maxConDist, CvColor.Beige);
#endif
                    if (maxConDist > 0)
                    {
                        Finger finger = new Finger(conCenter, (int)maxConDist);
                        fingers.Add(finger);
                    }
                    else continue;
#if DEBUG
                    Cv.DrawContours(imgContour, contours, CvColor.Red, CvColor.Green, 0, 3, LineType.AntiAlias);
#endif
                    // finds convex hull
                    int[] hull;
                    contours.ConvexHull2(out hull, 
                        (conCenter.X > (WebcamController.Instance.FrameSize.Width/2))? ConvexHullOrientation.Clockwise : ConvexHullOrientation.Counterclockwise);
                    Cv.Copy(imgFlesh, imgHull);
#if DEBUG
                    DrawConvexHull(contours, hull, resultImg);
#endif
                    // gets convexity defexts
                    Cv.Copy(imgContour, imgDefect);
                    CvSeq<CvConvexityDefect> defect = Cv.ConvexityDefects(contours, hull);

                    var tempLoop = defect.ToArray();
#if DEBUG
                    resultImg.PutText(conCenter.X + "," + conCenter.Y, conCenter, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Black);
#endif         
                    foreach (CvConvexityDefect ccd in tempLoop)
                    {
                        if (ccd.End.Y < conCenter.Y + maxConDist / 2)
                        {
                            int dis = (int)ccd.End.DistanceTo(conCenter);
                            int fingerDis = (int)ccd.End.DistanceTo(ccd.DepthPoint);
                            if (dis < maxConDist * 1.6 || fingerDis < maxConDist * 0.8) continue;
                            fingers[k].addTip(ccd.End);
                            fingers[k].addDepth(ccd.DepthPoint);
                            cntFinger++;
#if DEBUG
                            resultImg.PutText(ccd.End.X + "," + ccd.End.Y, ccd.End, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Black);
                            resultImg.DrawCircle(ccd.End, 2, CvColor.Red, -1);
                            //resultImg.DrawLine(ccd.End, conCenter, CvColor.Aqua);
                            resultImg.DrawLine(conCenter, ccd.DepthPoint, CvColor.Aqua);
                            resultImg.PutText(cntFinger.ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Black);
                            //resultImg.PutText(((int)fingers[k].GetFingerAngle2(ccd.DepthPoint, conCenter)).ToString(), ccd.DepthPoint, new CvFont(FontFace.HersheyComplex, 0.5, 0.5), CvColor.Tomato);
#endif
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

        private List<IplImage> FilterBlobImgList(IplImage imgSrc)
        {
            List<IplImage> retList = new List<IplImage>();
            CvBlobs blobs = new CvBlobs();
            IplImage lableImg = new IplImage(imgSrc.Size, BitDepth.U8, 1);
            int cntOfBlobs;
            blobs.Label(imgSrc);

            //큰덩어리를 나오게 해주세요
            CvBlob max = blobs.GreaterBlob();

            if (max == null)
            {
                return retList;
            }
            blobs.FilterByArea(max.Area * 1 / 4, max.Area);
            blobs.FilterLabels(lableImg);

            IplImage blobImg = new IplImage(imgSrc.Size, BitDepth.U8, 1);
            IplImage blobImg2 = new IplImage(imgSrc.Size, BitDepth.U8, 1);
            cntOfBlobs = blobs.Count;
            while (blobs.Count != 0)
            {
                blobImg.Zero();
                CvBlobs temp = blobs.Clone();
                int area = blobs.GreaterBlob().Area;
                temp.FilterByArea(area, area);
                temp.FilterLabels(blobImg);
                blobImg.Smooth(blobImg, SmoothType.Blur);
                blobs.FilterByArea(0, area - 1);
                retList.Add(blobImg.Clone());
            }

            return retList;
        }

        public Bitmap ConvertIplToBitmap(IplImage target)
        {
            if (target != null)
                return target.ToBitmap();

            return null;
        }
    }
}
