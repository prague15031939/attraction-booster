using System;
using System.Windows.Media.Imaging;

namespace WpfAttractionBooster.Core
{
    public class BitmapCache
    {
        public IntPtr pBackBuffer { get; private set; }
        public int BackBufferStride { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        public BitmapCache(WriteableBitmap bitmap)
        {
            pBackBuffer = bitmap.BackBuffer;
            BackBufferStride = bitmap.BackBufferStride;
            PixelWidth = bitmap.PixelWidth;
            PixelHeight = bitmap.PixelHeight;
        }
    }
}