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
    abstract public class ImageProcessBase
    {
        public static IplImage ROIImg { get; set; }
        public Bitmap ConvertIplToBitmap(IplImage target)
        {
            if(target != null)
                return target.ToBitmap();

            return null;
        }
                
        public List<IplImage> FilterBlobImgList(IplImage imgSrc)
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
            while(blobs.Count != 0){
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
    }
}
