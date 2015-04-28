using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace HandGesture
{
    class OpticalFlow : ImageProcessBase
    {
        IplImage prevImg;
        IplImage curImg;

        private static int blockSize = 10;
        private static int shiftSize = 10;

        CvSize block = new CvSize(OpticalFlow.blockSize, OpticalFlow.blockSize);
        CvSize shift = new CvSize(OpticalFlow.shiftSize, OpticalFlow.shiftSize);
        CvSize maxRange = new CvSize(20, 20);

        CvSize velSize;

        public OpticalFlow()
        {
            CvSize frameSize = WebcamController.Instance.FrameSize;
            prevImg = null;
            curImg = null;

            velSize = new CvSize((frameSize.Width - block.Width) / shift.Width,
                                  (frameSize.Height - block.Height) / shift.Height);
        }

        
        public IplImage OpticalFlow_BM(IplImage frame)
        {
            if(prevImg == null)
            {
                prevImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
                frame.CvtColor(prevImg, ColorConversion.BgrToGray);
                curImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
                frame.CvtColor(curImg, ColorConversion.BgrToGray);
                return null;
            }

            prevImg = curImg;
            frame.CvtColor(curImg, ColorConversion.BgrToGray);
            
            // cvCalcOpticalFlowBM
            // 블록 매칭에 의한 옵티컬 플로우의 계산
            const int blockSize = 10;
            const int shiftSize = 1;
            CvSize block = new CvSize(blockSize, blockSize);
            CvSize shift = new CvSize(shiftSize, shiftSize);
            CvSize maxRange = new CvSize(50, 50);

            IplImage dstImg = frame.Clone();
                // (1) 속도벡터를 보관하는 구조체의 확보
                int rows = (int)Math.Ceiling((double)prevImg.Height / blockSize);
                int cols = (int)Math.Ceiling((double)prevImg.Width / blockSize);
                using (CvMat velx = Cv.CreateMat(rows, cols, MatrixType.F32C1))
                using (CvMat vely = Cv.CreateMat(rows, cols, MatrixType.F32C1))
                {
                    //try
                    //{
                        Cv.SetZero(velx);
                        Cv.SetZero(vely);
                        // (2) 옵티컬 플로우의 계산 
                        Cv.CalcOpticalFlowBM(prevImg, curImg, block, shift, maxRange, false, velx, vely);
                        // (3) 계산된 플로우를 그리기
                        for (int j = 0; j < vely.Height; j++) //첨부된 소스에는 이부분이 잘못되어 있습니다
                        {
                            for (int i = 0; i < velx.Width; i++)
                            {
                                int dx = (int)Cv.GetReal2D(velx, j, i);
                                int dy = (int)Cv.GetReal2D(vely, j, i);
                                Cv.Line(dstImg,
                                    new CvPoint(i * blockSize, j * blockSize),
                                    new CvPoint(i * blockSize + dx, j * blockSize + dy),
                                    new CvColor(255, 0, 0), 1, LineType.AntiAlias, 0);
                            }
                        }
                    //}
                    //catch (OpenCVException e)
                    //{
                    //    Console.WriteLine(e.ErrMsg);
                    //    if (e.InnerException != null)
                    //    {
                    //        Console.WriteLine(e.InnerException.Message);
                    //    }
                    //}

                 }
                return dstImg;
        }

        public IplImage OpticalFlow_LK(IplImage frame)
        {
            if (prevImg == null)
            {
                prevImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
                frame.CvtColor(prevImg, ColorConversion.BgrToGray);
                curImg = new IplImage(WebcamController.Instance.FrameSize, BitDepth.U8, 1);
                frame.CvtColor(curImg, ColorConversion.BgrToGray);
                return null;
            }

            prevImg = curImg;
            frame.CvtColor(curImg, ColorConversion.BgrToGray);

            IplImage frameClone = null, eigImg = null, tempImg = null, pyramid1 = null, pyramid2 = null;
            CvSize frameSize = WebcamController.Instance.FrameSize;
            frameClone = frame;
            IplImage retImg = frameClone.Clone();

            //초기화 , 두 개의 영상을 불러온다. / grayScale
            AllocateOnDeman(ref prevImg, frameSize, BitDepth.U8, 1);
            Cv.ConvertImage(frameClone, prevImg, ConvertImageFlag.Flip);

            frameClone = WebcamController.Instance.getImg();
            if(frameClone == null)
            {
                Console.WriteLine("Error!! cam Img is Null");
                return null;
            }

            AllocateOnDeman(ref curImg, frameSize, BitDepth.U8, 1);
            Cv.ConvertImage(frameClone, curImg, ConvertImageFlag.Flip);

            //추적할 특징을 검출하기 시작함.
            AllocateOnDeman(ref eigImg, frameSize, BitDepth.F32, 1);
            AllocateOnDeman(ref tempImg, frameSize, BitDepth.F32, 1);
            
            int corner_cnt = 400;

            //이미지에서 코너를 추출함
            CvPoint2D32f[] frame1Features = new CvPoint2D32f[corner_cnt];
            Cv.GoodFeaturesToTrack(prevImg, eigImg, tempImg, out frame1Features, ref corner_cnt, 0.01f, 0.01f, null);

            CvSize opticalFlowWindow = new CvSize(3, 3);
            CvTermCriteria opticalFlowTerminationCriteria = Cv.TermCriteria(CriteriaType.Iteration | CriteriaType.Epsilon, 20, 0.3f);
            
            //서브 픽셀을 검출하여 정확한 서브 픽셀 위치를 산출함.
            Cv.FindCornerSubPix(prevImg, frame1Features, corner_cnt, opticalFlowWindow, new CvSize(-1, -1), opticalFlowTerminationCriteria);
            
            //루카스 카나데 알고리즘
            char[] opticalFlowFoundFeature = new char[corner_cnt];
            float[] opticalFlowFeatureError = new float[corner_cnt];
          
            AllocateOnDeman(ref pyramid1, frameSize, BitDepth.U8, 1);
            AllocateOnDeman(ref pyramid2, frameSize, BitDepth.U8, 1);

            CvPoint2D32f[] frame2Features = new CvPoint2D32f[corner_cnt];

            sbyte[] status = new sbyte[corner_cnt];
            Cv.CalcOpticalFlowPyrLK(prevImg, curImg, pyramid1, pyramid2, frame1Features, out frame2Features, opticalFlowWindow, 5, out status , out opticalFlowFeatureError, opticalFlowTerminationCriteria, LKFlowFlag.InitialGuesses);

            int lineThickness = 1;
            CvScalar lineColor = Cv.RGB(255, 0, 0);
            CvPoint p, q;

            double angle;
            double pypotenuse;
            for (int i = 0; i < corner_cnt; i++)
            {
                //feature_found[i]값이 0이 리턴이 되면 대응점을 발견하지 못함
                //feature_errors[i] 현재 프레임과 이전프레임 사이의 거리가 550이 넘으면 예외로 처리

                if (status[i] == 0) continue;
                //if (opticalFlowFoundFeature[i] == 0) continue;
                //if (opticalFlowFeatureError[i] > 550) continue;

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

            return retImg;
        }

        readonly double PI = 3.14159265358979323846;

        double square(int a)
        {
            return a * a;
        }

        private void AllocateOnDeman(ref IplImage img, CvSize size, BitDepth depth, int channels)
        {
            if (img != null) return;
            img = Cv.CreateImage(size, depth, channels);
            if (img == null) return;
        }
    }
}
