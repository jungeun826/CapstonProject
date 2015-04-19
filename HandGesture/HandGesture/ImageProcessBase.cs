﻿using System;
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
            
            //피부색 범위를 새로지정해야한다.
            target.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), retImg);

            //////침식 팽창을 이용한 노이즈 제거
            retImg.Erode(retImg, null, 2);
            //retImg.Erode(retImg, null, 2);
            retImg.Dilate(retImg, null, 3);
         
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

            //윤곽선 검출
            IplImage contours = origin.Clone();
            contours = testContours(contours);
            
            ////엣지검출
            //IplImage cannyImg = testCanny(origin.Clone());
            //retImg.Sub(cannyImg, retImg);

            //Blobs blobs = new Blobs();
            //    blobs.Labeling(retImg);
				
            //    CvBlob[] b = new CvBlob[2];
            //    List<int> a = new List<int>(b.Length);
            //    for (int i = 0; i < 2; i++)
            //        b[i] = blobs.GetSortedBlob(i);
            //    Blob left, right;
            //    if (b[0].Center.X > b[1].Center.X)
            //    {
            //        left = b[1];
            //        right = b[0];
            //    }
            //    else
            //    {
            //        left = b[0];
            //        right = b[1];
            //    }

            //    CvBlob blobLeft = left;
            //    CvBlob blobRight = right;







            //blobs 레이블링을 통해서 손 검출
            CvBlobs blobs = new CvBlobs();
            IplImage lableImg = new IplImage(retImg.Size, CvBlobLib.DepthLabel, 1);
            blobs.Label(retImg);


            //큰덩어리를 나오게 해주세요
            blobs.Label(retImg);
            CvBlob max = blobs.GreaterBlob();

            if (max == null)
            {
                return retImg.ToBitmap();
            }

            blobs.FilterByArea(max.Area * 3 /4, max.Area);

            blobs.FilterLabels(retImg);

            foreach (CvBlob ttt in blobs.Values)
            {
                //관심영역설정
                int x = ttt.MinX;
                int y = ttt.MinY;
                int width = ttt.MaxX - x;
                int height = ttt.MaxY - y;

                //CvRect interestArea =  Cv.GetImageROI(retImg);
                CvRect interestArea = new CvRect(x, y, width, height);

                Cv.DrawRect(retImg, interestArea, CvColor.Wheat, 10);

                CvPoint center = distTF(retImg, interestArea);

                Cv.DrawCircle(retImg, center, 2, CvColor.Wheat, 10);
            }

            
           // Cv.DrawCircle(retImg, center, 30, new CvScalar(0, 135, 40), 10);
        
            return retImg.ToBitmap();//testContoursBMP(retImg); //retImg.ToBitmap();
            //return testContoursBMP(target);
            //return cannyImg.ToBitmap();
        }//end of extractor





        /*
         그냥 쓰니까 되는데요? ㅋ 좋은자료 감사합니다~ 
         * 마침 딱 필요하던 시기에 발견하게 됐네요.
그리고 cvSetImageROI() 함수로 img,dist,dist8u,dist32s에 
         * 이미지내의 탐색할 부분을 영역 지정하면
         * 끊김없이 연산 속도가 엄청 빨라지네요.
         * 탐색 윈도우를 지정했다면 
         * 이중 for문내의 i,j의 초기값과 제어문의 비교범위도
         * 탐색 윈도우에 맞게 조정해 주는 것도 잊지마세요. 
         * 제가 사용한 방법을 쓰실 때, 함수 인수에 CvRect 형 변수로
         * 탐색 영역을 넘겨 받으면 됩니다. 
         * 그리고 cvRelease로 dist,dist8u,dist32s
         * 모두 메모리 해제해주는 것도 넣어야 되네요.. 
         * 첨에 안넣고 했다가 메모리 누수가 엄청 생기네요..; 
         * 최대 1.1G까지 모아 봤습니다 ㅋ
         */


        public static unsafe CvPoint distTF(IplImage img, CvRect interestArea)
        {
            // 초기화 :
            float[] mask = new float[3];
            IplImage dist = null;
            IplImage dist8u = null;
            IplImage dist32s = null;

            int max;
            CvPoint p = new CvPoint();
            byte* ptr;

            dist = Cv.CreateImage(Cv.GetSize(img), BitDepth.F32, 1);
            dist8u = Cv.CloneImage(img);
            dist32s = Cv.CreateImage(Cv.GetSize(img), BitDepth.S32, 1);

            int x = interestArea.X;
            int y = interestArea.Y;
            int width = interestArea.Width;
            int height = interestArea.Height;
            

            //거리변환 행렬 생성 :
            mask[0] = 1.0f;
            mask[1] = 1.5f;

            //거리변환 함수 사용 : 

            Cv.DistTransform(img, dist, DistanceType.User, 3, mask, null);

            // 눈에 보이게 변환 :
            Cv.ConvertScale(dist, dist, 1000, 0);
            Cv.Pow(dist, dist, 0.5);
            Cv.ConvertScale(dist, dist32s, 1.0, 0.5);
            Cv.AndS(dist32s, Cv.ScalarAll(255), dist32s, null);
            Cv.ConvertScale(dist32s, dist8u, 1, 0);



            //가장 큰 좌표값을 찾는다 :
            for (int i = max = y; i < y+height; i++)
            {
                int index = i * dist8u.WidthStep;
                for (int j = x; j < x + width; j++)
                {

                    ptr = (byte*)dist8u.ImageData;
                    if ((char)ptr[index + j] > max)
                    {
                        max = (char)dist8u[index + j];
                        p.X = j;
                        p.Y = i;

                    }
                }

            }


            ////가장 큰 좌표값을 찾는다 :
            //for (int i = max = 0; i < dist8u.Height; i++)
            //{
            //    int index = i * dist8u.WidthStep;
            //    for (int j = 0; j < dist8u.Width; j++)
            //    {

            //        ptr = (byte*)dist8u.ImageData;
            //        if ((char)ptr[index + j] > max)
            //        {
            //            max = (char)dist8u[index + j];
            //            p.X = j;
            //            p.Y = i;

            //        }
            //    }

            //}
            return p;
        }//end of distTF
 


        public static unsafe CvPoint distTF(IplImage img){
               // 초기화 :
             float[] mask = new float[3];
             IplImage dist = null;
             IplImage dist8u = null;
             IplImage dist32s = null;
             
             int max;
             CvPoint p = new CvPoint() ;
             byte* ptr;
 
            dist = Cv.CreateImage(Cv.GetSize(img), BitDepth.F32, 1);
            dist8u = Cv.CloneImage(img);
            dist32s = Cv.CreateImage(Cv.GetSize(img), BitDepth.S32, 1);

        
             //거리변환 행렬 생성 :
             mask[0] = 1.0f;
             mask[1] = 1.5f;
 
             //거리변환 함수 사용 : 
             
              Cv.DistTransform(img, dist, DistanceType.User, 3, mask, null);

             // 눈에 보이게 변환 :
            Cv.ConvertScale(dist, dist, 1000, 0);
            Cv.Pow(dist, dist, 0.5);
            Cv.ConvertScale(dist, dist32s, 1.0, 0.5);
            Cv.AndS(dist32s, Cv.ScalarAll(255), dist32s, null);
            Cv.ConvertScale(dist32s, dist8u, 1, 0);
            
            //가장 큰 좌표값을 찾는다 :
            for (int i = max = 0; i < dist8u.Height; i++)
            {
                int index = i * dist8u.WidthStep;
                for (int j = 0; j < dist8u.Width; j++)
                {

                    ptr = (byte*)dist8u.ImageData;
                    if ((char)ptr[index + j] > max)
                    {
                        max = (char)dist8u[index + j];
                        p.X = j;
                        p.Y = i;

                    }
                }

            }
            return p;
}//end of distTF
 

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

               //두께가 -1이면 내부를 채워준다고 한다
               // g_gray.DrawContours(contours, new CvScalar(255), new CvScalar(255), 150,-1);

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