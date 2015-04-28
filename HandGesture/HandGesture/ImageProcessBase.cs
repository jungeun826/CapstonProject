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
            if(target != null)
                return target.ToBitmap();

            return null;
        }

        //yong's codes
        //yong's codes
        protected IplImage ConvertToBinaryIpl(IplImage target)
        {
            IplImage origin = target.Clone();
            IplImage retImg = new IplImage(target.Width, target.Height, BitDepth.U8, 1);
            target.CvtColor(target, ColorConversion.BgrToCrCb);
            target.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), retImg);

            return retImg;
        }

        public IplImage extractor(IplImage target)
        {
            IplImage origin = target.Clone();

            //컬러맵 변환
            target.CvtColor(target, ColorConversion.BgrToCrCb);
            //피부색 검출
            IplImage retImg = new IplImage(target.Width, target.Height, BitDepth.U8, 1);
            target.InRangeS(new CvScalar(0, 135, 40), new CvScalar(255, 170, 150), retImg);


            //////침식 팽창을 이용한 노이즈 제거
            retImg.Erode(retImg, null, 2);
            //retImg.Erode(retImg, null, 2);
            retImg.Dilate(retImg, null, 3);

            //윤곽선 검출
            //IplImage contours = origin.Clone();
            //contours = testContours(contours);
            
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
                return retImg;//.ToBitmap();
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

                //Cv.DrawRect(retImg, interestArea, CvColor.Red, 10);

                CvPoint center = distTF(retImg, interestArea);

                //Cv.DrawCircle(retImg, center, 2, CvColor.Red, 10);
            }

            
           // Cv.DrawCircle(retImg, center, 30, new CvScalar(0, 135, 40), 10);
        
            return retImg;//.ToBitmap();//testContoursBMP(retImg); //retImg.ToBitmap();
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


        private unsafe CvPoint distTF(IplImage img, CvRect interestArea)
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

        private unsafe CvPoint distTF(IplImage img)
        {
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
 

        protected Bitmap ConvertToBinaryBMP(IplImage target)
        {
            return ConvertToBinaryIpl(target).ToBitmap();
        }

        public IplImage extractSkinAsIpl(IplImage target)
        {
            IplImage origin = target.Clone();
            IplImage processImg = origin.Clone();
            IplImage maskImg = ConvertToBinaryIpl(processImg);
            maskImg.Not(maskImg);
            processImg.AndS(0, processImg, maskImg);
            processImg.CvtColor(processImg, ColorConversion.CrCbToBgr);
            //processImg.AndS(0, processImg, maskImg);
            processImg.Smooth(processImg, SmoothType.Median);

            //temp test code
            //IplImage temp = new IplImage(origin.Size, BitDepth.U8, 1);
            //origin.CvtColor(temp, ColorConversion.BgrToGray);
            //target = temp;
            //
            return processImg;
        }
        public Bitmap extractSkinAsBMP(IplImage target)
        {
            return extractSkinAsIpl(target).ToBitmap();
        }

        public IplImage test(IplImage target)
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
        public Bitmap testBMP(IplImage target)
        {
            return test(target).ToBitmap();
        }

        protected IplImage testCanny(IplImage target)
        {
            IplImage cloneImg = new IplImage(target.Size, BitDepth.U8, 1);
            //Cv.Canny(target, cloneImg, 50, 255);
            target.Canny(cloneImg, 50, 255);

            return cloneImg;
        }
        ////////////////
        IplImage g_gray;
        IplImage g_binary;
        int g_thresh = 50;
        CvMemStorage g_storage;

        public IplImage testContours(IplImage target)
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

            Cv.CvtColor(target, g_gray, ColorConversion.BgrToGray);

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
        public Bitmap testContoursBMP(IplImage target)
        {
            return testContours(target).ToBitmap();
        }

        ///////////////////////
        public IplImage FaceDetect(IplImage src)
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

        public Bitmap handDetect(IplImage target)
        {
            return FaceDetect(target).ToBitmap();
        }

        public Bitmap ReturnTracking(IplImage target, out IplImage frame1_1c, out IplImage frame2_1c)
        {
            frame1_1c = null; frame2_1c = null;
            IplImage frame = null, eigImg = null, tempImg = null, pyramid1 = null, pyramid2 = null;
            CvSize frameSize = WebcamController.Instance.FrameSize;
            frame = target;
            IplImage retImg = frame.Clone();

            //초기화 , 두 개의 영상을 불러온다. / grayScale
            AllocateOnDeman(ref frame1_1c, frameSize, BitDepth.U8, 1);
            Cv.ConvertImage(frame, frame1_1c, ConvertImageFlag.Flip);

            frame = WebcamController.Instance.getImg();
            if(frame == null)
            {
                Console.WriteLine("Error!! cam Img is Null");
                return null;
            }

            AllocateOnDeman(ref frame2_1c, frameSize, BitDepth.U8, 1);
            Cv.ConvertImage(frame, frame2_1c, ConvertImageFlag.Flip);

            //추적할 특징을 검출하기 시작함.
            AllocateOnDeman(ref eigImg, frameSize, BitDepth.F32, 1);
            AllocateOnDeman(ref tempImg, frameSize, BitDepth.F32, 1);
            
            int corner_cnt = 400;

            //이미지에서 코너를 추출함
            CvPoint2D32f[] frame1Features = new CvPoint2D32f[corner_cnt];
            Cv.GoodFeaturesToTrack(frame1_1c, eigImg, tempImg, out frame1Features, ref corner_cnt, 0.01f, 0.01f, null);

            CvSize opticalFlowWindow = new CvSize(3, 3);
            CvTermCriteria opticalFlowTerminationCriteria = Cv.TermCriteria(CriteriaType.Iteration | CriteriaType.Epsilon, 20, 0.3f);
            
            //서브 픽셀을 검출하여 정확한 서브 픽셀 위치를 산출함.
            Cv.FindCornerSubPix(frame1_1c, frame1Features, corner_cnt, opticalFlowWindow, new CvSize(-1, -1), opticalFlowTerminationCriteria);
            
            //루카스 카나데 알고리즘
            char[] opticalFlowFoundFeature = new char[corner_cnt];
            float[] opticalFlowFeatureError = new float[corner_cnt];
          
            AllocateOnDeman(ref pyramid1, frameSize, BitDepth.U8, 1);
            AllocateOnDeman(ref pyramid2, frameSize, BitDepth.U8, 1);

            CvPoint2D32f[] frame2Features = new CvPoint2D32f[corner_cnt];

            sbyte[] status = new sbyte[corner_cnt];
            Cv.CalcOpticalFlowPyrLK(frame1_1c, frame2_1c, pyramid1, pyramid2, frame1Features, out frame2Features, opticalFlowWindow, 5, out status , out opticalFlowFeatureError, opticalFlowTerminationCriteria, LKFlowFlag.InitialGuesses);

            int lineThickness = 1;
            CvScalar lineColor = Cv.RGB(255, 0, 0);
            CvPoint p, q;

            double angle;
            double pypotenuse;
            for (int i = 0; i < corner_cnt; i++)
            {
                //feature_found[i]값이 0이 리턴이 되면 대응점을 발견하지 못함
                //feature_errors[i] 현재 프레임과 이전프레임 사이의 거리가 550이 넘으면 예외로 처리

                if (opticalFlowFoundFeature[i] == 0) continue;
                if (opticalFlowFoundFeature[i] > 550) continue;

                p = new CvPoint(Cv.Round(frame1Features[i].X), Cv.Round(frame1Features[i].Y));
                q = new CvPoint(Cv.Round(frame2Features[i].X), Cv.Round(frame2Features[i].Y));
                
                Cv.Line(retImg, p, q, lineColor);
                //angle = Math.Atan2((double)p.Y - q.Y, (double)p.X - q.X);
                //pypotenuse = Math.Sqrt(square(p.Y - q.Y) + square(p.X - q.X));

                //q.X = (int)(p.X - 3 * pypotenuse * Math.Cos(angle));
                //q.Y = (int)(p.Y - 3 * pypotenuse * Math.Sin(angle));
                //Cv.Line(retImg, p, q, lineColor, lineThickness, LineType.AntiAlias, 0);

                //p.X = (int)(q.X + 9 * Math.Cos(angle + PI / 4));
                //p.Y = (int)(q.Y + 9 * Math.Cos(angle + PI / 4));
                //Cv.Line(retImg, p, q, lineColor, lineThickness, LineType.AntiAlias, 0);

                //p.X = (int)(q.X + 9 * Math.Cos(angle - PI / 4));
                //p.Y = (int)(q.Y + 9 * Math.Cos(angle - PI / 4));
                //Cv.Line(retImg, p, q, lineColor, lineThickness, LineType.AntiAlias, 0);
            }
            //Cv.ShowImage("OpticalFlow", frame1);

            return retImg.ToBitmap();
        }

        

        protected CvPoint2D32f[] GetHandFeature(IplImage src)
        {
            //http://cafe.naver.com/opencvsharp 보고 수정하기
            //http://lueseypid.tistory.com/98
            //http://codens.info/594
            //http://stackoverflow.com/questions/10161351/opencv-how-to-plot-velocity-vectors-as-arrows-in-using-single-static-image
            //http://paeton.tistory.com/18
            //http://nikq.nothing.sh/pkn/ms.cgi?ShowDiary_file=/program/1140018030&blogid=&t=sketch
            //http://javava79.tistory.com/124

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
            CvPoint2D32f[] features;
            IplImage smallImg = new IplImage(new CvSize(Cv.Round(img.Width / scale), Cv.Round(img.Height / scale)), BitDepth.U8,1);
            {
                // 얼굴 검출용의 화상의 생성
                using (IplImage gray = new IplImage(img.Size, BitDepth.U8, 1))
                {
                    //Cv.CvtColor(img, gray, ColorConversion.BgrToGray);
                    Cv.Resize(gray, smallImg, Interpolation.Linear);
                    Cv.EqualizeHist(smallImg, smallImg);
                }
                using (CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile("C:\\aGest.xml"))
                using (CvMemStorage storage = new CvMemStorage())
                {
                    storage.Clear();
                    // 얼굴의 검출
                    CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(smallImg, cascade, storage, scaleFactor, minNeighbors, 0, new CvSize(24, 24));
                    features = new CvPoint2D32f[400];
                    // 검출한 얼굴에 원을 그린다
                    for (int i = 0; i < faces.Total; i++)
                    {
                        CvRect r = faces[i].Value.Rect;
                        CvPoint center = new CvPoint
                        {
                            X = Cv.Round((r.X + r.Width * 0.5) * scale),
                            Y = Cv.Round((r.Y + r.Height * 0.5) * scale)
                        };
                        features[i].X = faces[i].Value.Rect.X;
                        features[i].Y = faces[i].Value.Rect.Y;

                        int radius = Cv.Round((r.Width + r.Height) * 0.25 * scale);
                        img.Circle(center, radius, colors[i % 8], 3, LineType.AntiAlias, 0);
                    }
                }

                FindFace = img.Clone();
                return features;
            }
        }

        readonly double PI = 3.14159265358979323846;

        double square(int a)
        {
            return a * a;
        }

        private void AllocateOnDeman(ref IplImage img, CvSize size, BitDepth depth, int channels)
        {
            if(img != null) return;
            img = Cv.CreateImage(size, depth, channels);
            if(img == null) return;
        }
    }
}
