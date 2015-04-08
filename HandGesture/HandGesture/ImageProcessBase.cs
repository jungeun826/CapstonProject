using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp.Extensions;
using OpenCvSharp.Blob;


namespace HandGesture
{
    abstract class ImageProcessBase
    {
        public Bitmap ConvertIplToBitmap(IplImage target)
        {
            return target.ToBitmap();
        }

        //yong's codes
        //yong's codes
        public static IplImage ConvertToBinaryIpl(IplImage target)
        {
            IplImage retImg = new IplImage(target.Width, target.Height, BitDepth.U8, 1);
            target.CvtColor(target, ColorConversion.BgrToCrCb);
            target.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), retImg);

            return retImg;
        }

        public static Bitmap extractor(IplImage target)
        {
            IplImage origin = target.Clone();

            //컬러맵 변환
            target.CvtColor(target, ColorConversion.BgrToCrCb);
            //피부색 검출
            IplImage retImg = new IplImage(target.Width, target.Height, BitDepth.U8, 1);
            target.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), retImg);

            ////침식 팽창을 이용한 노이즈 제거
            ////retImg.Erode(retImg, null, 2);
            //retImg.Dilate(retImg, null, 3);

            ////스무스 사용
            //retImg.Smooth(retImg, SmoothType.Median);

            ////이진화데이터를 마스크로 추출
            //target = origin.Clone();
            //IplImage maskImg = retImg;
            //maskImg.Not(maskImg);
            //target.AndS(0, target, maskImg);

            ////temp test code
            //IplImage temp = new IplImage(target.Size, BitDepth.U8, 1);
            //target.CvtColor(temp, ColorConversion.BgrToGray);
            //target = temp;

            ////윤곽선 검출
            //IplImage contours = origin.Clone();
            //contours = testContours(contours);
            
            IplImage cannyImg = testCanny(origin.Clone());
            retImg.Sub(cannyImg, retImg);

            //blobs 레이블링을 통해서 손 검출
            CvBlobs blobs = new CvBlobs();
            IplImage lableImg = new IplImage(target.Size, CvBlobLib.DepthLabel, 1);

            blobs.Label(retImg);
            CvBlob max = blobs.GreaterBlob();
            if (max == null)
            {
                return retImg.ToBitmap();
            }
            blobs.FilterByArea(max.Area, max.Area);
            blobs.FilterLabels(retImg);

            return retImg.ToBitmap(); //testContoursBMP(retImg); //retImg.ToBitmap();
            //return testContoursBMP(target);
            //return cannyImg.ToBitmap();
        }
        public static Bitmap ConvertToBinaryBMP(IplImage target)
        {
            return ConvertToBinaryIpl(target).ToBitmap();
        }

        public static IplImage extractSkinAsIpl(IplImage target)
        {
            IplImage origin = target.Clone();
            IplImage maskImg = ConvertToBinaryIpl(origin);
            maskImg.Not(maskImg);
            target.AndS(0, target, maskImg);
            target.Smooth(target, SmoothType.Median);

            //temp test code
            IplImage temp = new IplImage(target.Size, BitDepth.U8, 1);
            target.CvtColor(temp, ColorConversion.BgrToGray);
            target = temp;
            //
            return target;
        }
        public static Bitmap extractSkinAsBMP(IplImage target)
        {
            return extractSkinAsIpl(target).ToBitmap();
        }

        public static IplImage test(IplImage target)
        {
            CvBlobs blobs = new CvBlobs();
            IplImage lableImg = new IplImage(target.Size, CvBlobLib.DepthLabel, 1);
            IplImage retImg = new IplImage(target.Size, BitDepth.U8, 1);

            blobs.Label(target);
            CvBlob max = blobs.GreaterBlob();
            if (max == null)
            {
                return target;
            }
            blobs.FilterByArea(max.Area, max.Area);
            blobs.FilterLabels(retImg);
            return retImg;
        }
        public static Bitmap testBMP(IplImage target)
        {
            return test(target).ToBitmap();
        }

        public static IplImage testCanny(IplImage target)
        {
            IplImage cloneImg = new IplImage(target.Size, BitDepth.U8, 1);
            //Cv.Canny(target, cloneImg, 50, 255);
            target.Canny(cloneImg, 50, 255);

            return cloneImg;
        }
        ////////////////
        static IplImage g_gray;
        static IplImage g_binary;
        static int g_thresh = 50;
        static CvMemStorage g_storage;
        public static IplImage testContours(IplImage target)
        {
            if (g_storage == null)
            {
                g_gray = new IplImage(target.Size, BitDepth.U8, 1);
                g_binary = new IplImage(target.Size, BitDepth.U8, 1);
                g_storage = new CvMemStorage(0);
            }
            else
            {
                g_storage.Clear();
            }

            CvSeq<CvPoint> contours;

            target.CvtColor(g_gray, ColorConversion.BgrToGray);

            g_gray.Threshold(g_gray, g_thresh, 255, ThresholdType.Binary);
            g_gray.Copy(g_binary);

            g_gray.FindContours(g_storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxNone);

            g_gray.Zero();

            if (contours != null)
            {
                contours.ApproxPoly(CvContour.SizeOf, g_storage, ApproxPolyMethod.DP, 3, true);
                g_gray.DrawContours(contours, new CvScalar(255), new CvScalar(255), 150);

            }

            //g_gray.Dilate(g_gray, null, 2);
            //g_gray.Erode(g_gray, null, 2);

            return g_gray;
        }
        public static Bitmap testContoursBMP(IplImage target)
        {
            return testContours(target).ToBitmap();
        }

        ///////////////////////
        public static IplImage FaceDetect(IplImage src)
        {
            IplImage FindFace;
            // CvHaarClassifierCascade, cvHaarDetectObjects
            // 얼굴을 검출하기 위해서 Haar 분류기의 캐스케이드를 이용한다
            CvColor[] colors = new CvColor[]{
                new CvColor(0,0,255),
                new CvColor(0,128,255),
                new CvColor(0,255,255),
                new CvColor(0,255,0),
                new CvColor(255,128,0),
                new CvColor(255,255,0),
                new CvColor(255,0,0),
                new CvColor(255,0,255),
            };
            const double scale = 1;
            const double scaleFactor = 1.139;
            const int minNeighbors = 2;
            IplImage img = src.Clone();
            IplImage smallImg = new IplImage(new CvSize(Cv.Round(img.Width / scale), Cv.Round(img.Height / scale)), BitDepth.U8, 3);
            {
                // 얼굴 검출용의 화상의 생성
                using (IplImage gray = new IplImage(img.Size, BitDepth.U8, 1))
                {
                    Cv.CvtColor(img, gray, ColorConversion.BgrToGray);
                    Cv.Resize(gray, smallImg, Interpolation.Linear);
                    Cv.EqualizeHist(smallImg, smallImg);
                }
                using (CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile("C:\\aGest.xml"))
                using (CvMemStorage storage = new CvMemStorage())
                {
                    storage.Clear();
                    // 얼굴의 검출
                    CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(smallImg, cascade, storage, scaleFactor, minNeighbors, 0, new CvSize(24, 24));
                    // 검출한 얼굴에 원을 그린다
                    for (int i = 0; i < faces.Total; i++)
                    {
                        CvRect r = faces[i].Value.Rect;
                        CvPoint center = new CvPoint
                        {
                            X = Cv.Round((r.X + r.Width * 0.5) * scale),
                            Y = Cv.Round((r.Y + r.Height * 0.5) * scale)
                        };
                        int radius = Cv.Round((r.Width + r.Height) * 0.25 * scale);
                        img.Circle(center, radius, colors[i % 8], 3, LineType.AntiAlias, 0);
                    }
                }
                FindFace = img.Clone();
                return FindFace;
            }
        }

        public static Bitmap handDetect(IplImage target)
        {
            return FaceDetect(target).ToBitmap();
        }
    }
}
