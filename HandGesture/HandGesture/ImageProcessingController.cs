using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp.Extensions;


namespace HandGesture
{
    static class ImageProcessingController
    {
        public static IplImage RGBToYCbCr(IplImage rgbImage)
        {
            IplImage ycbcrImg = new IplImage(rgbImage.Size, BitDepth.U8, 3);
            Cv.CvtColor(rgbImage, ycbcrImg, ColorConversion.BgrToCrCb);

            return ycbcrImg;
        }


        public static Bitmap IplToBitmap(IplImage IplImg)
        {
            //IplImage ycbcrImg = ImageProcessingController.RGBToYCbCr(rgbImage);

            PixelFormat pixelFormat = PixelFormat.Format8bppIndexed;
            if (IplImg.Depth == BitDepth.U8)
            {
                if (IplImg.NChannels == 3)
                    pixelFormat = PixelFormat.Format24bppRgb;
            }

            Bitmap bitmap = bitmap = new Bitmap(IplImg.Width, IplImg.Height, IplImg.WidthStep, pixelFormat, IplImg.ImageData);
            return bitmap;
        }

        public static unsafe IplImage ImageToBinary(IplImage img)
        {

            IplImage binaryImg = new IplImage(img.Size, BitDepth.U8, 3);
            byte R, G, B;
            for (int x = 0; x < img.Height; x++)
            {
                for (int y = 0; y < img.Width; y++)
                {
                    int nIdx = x * img.WidthStep + y * img.NChannels;

                    R = img.ImageDataPtr[nIdx + 2];
                    G = img.ImageDataPtr[nIdx + 1];
                    B = img.ImageDataPtr[nIdx + 0];

                    //if ((R < 95 && G > 40 && B > 20)
                    //    && (Max(R, G, B) - Min(R, G, B) < 15)
                    //    && (int)Abs((byte)(R - G)) > 15 && R > G && R > B)
                    {

                        img.ImageDataPtr[nIdx + 2] = 0;
                        img.ImageDataPtr[nIdx + 1] = 0;
                        img.ImageDataPtr[nIdx + 0] = 0;
                    }
                    //else
                    //{
                    //    img.ImageDataPtr[nIdx + 2] = 0;
                    //    img.ImageDataPtr[nIdx + 1] = 0;
                    //    img.ImageDataPtr[nIdx + 0] = 0;
                    //}

                }
            }


            return binaryImg;
        }

        private static byte Max(byte a, byte b, byte c)
        {
            byte max = 0;

            if (max < a) max = a;
            if (max < b) max = b;
            if (max < c) max = c;

            return max;
        }

        private static byte Min(byte a, byte b, byte c)
        {
            byte min = a;

            if (min > b) min = b;
            if (min > c) min = c;

            return min;
        }

        private static byte Abs(byte a)
        {
            if (a >= 0) return a;
            else
                return (byte)-a;
        }
    }
}
