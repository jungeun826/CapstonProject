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
        public IplImage ConvertToBinaryIpl(IplImage target)
        {
            IplImage retImg = new IplImage(target.Width, target.Height, BitDepth.U8, 1);
            target.CvtColor(target, ColorConversion.BgrToCrCb);
            target.InRangeS(new CvScalar(0, 135, 30), new CvScalar(255, 170, 160), retImg);

            return retImg;
        }

        public Bitmap ConvertToBinaryBMP(IplImage target)
        {
            return ConvertToBinaryIpl(target).ToBitmap();
        }

        public IplImage ExtractSkinAsIplBinary(IplImage target)
        {
            IplImage origin = target.Clone();
            IplImage maskImg = ConvertToBinaryIpl(origin);
            maskImg.Not(maskImg);
            target.AndS(0, target, maskImg);
            return target;
        }

        public Bitmap ExtractSkinAsBMPBinary(IplImage target)
        {
            return ExtractSkinAsIplBinary(target).ToBitmap();
        }

        //노이즈 제거 (동적테이블)

        //확장된 harr특징

        //차연산

        //합연산
        
        //모폴로지

        //등등
    }
}
