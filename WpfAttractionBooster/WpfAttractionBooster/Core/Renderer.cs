using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfAttractionBooster.Core
{
    public class Renderer
    {
        public WriteableBitmap WorkBitmap { get; private set; }

        private BitmapCache _bitmapCache;

        private System.Windows.Controls.Image _outputImage;

        private int _width;

        private int _height;

        private AttractionCore _core;

        private double _timeStep = 0.05;

        private int _imageScaleCoeff = 60;

        public Renderer(WriteableBitmap workBitmap, System.Windows.Controls.Image outputImage, int width, int height)
        {
            WorkBitmap = workBitmap;
            _bitmapCache = new BitmapCache(workBitmap);

            _outputImage = outputImage;

            _width = width;
            _height = height;
            _core = new AttractionCore();
        }

        public void Run(object obj)
        {
            var cancellationToken = (CancellationToken)obj;

            while (true)
            {
                for (var t = 0.0d; t <= _timeStep * 260;)
                {
                    if (IsRenderRequired)
                    {
                        if (!Render(t, cancellationToken))
                            return;

                        t += _timeStep;
                    }
                }

                Task.Delay(200).Wait();

                for (var t = _timeStep * 260; t >= 0;)
                {
                    if (IsRenderRequired)
                    {
                        if (!Render(t, cancellationToken))
                            return;

                        t -= _timeStep;

                        if (t < 0)
                        {
                            Render(0, cancellationToken);
                            break;
                        }
                    }
                }

                Task.Delay(150).Wait();
            }
        }

        private bool IsRenderRequired => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() % 50 == 0;

        private bool Render(double t, CancellationToken token)
        {
            if (token.IsCancellationRequested) return false;

            _outputImage.Dispatcher.Invoke(() =>
            {
                WorkBitmap.Lock();
                ClearBitmap(WorkBitmap);
            });

            var rightPart = Task.Run(() => RenderPart(_core.GetRigthRange, t));
            var leftPart = Task.Run(() => RenderPart(_core.GetLeftRange, t));

            Task.WaitAll(new[] { rightPart, leftPart });

            if (token.IsCancellationRequested) return false;

            _outputImage.Dispatcher.Invoke(() =>
            {
                WorkBitmap.AddDirtyRect(new Int32Rect(0, 0, WorkBitmap.PixelWidth, WorkBitmap.PixelHeight));
                WorkBitmap.Unlock();
            });

            return true;
        }

        private void RenderPart(Func<double, IEnumerable<(double, double)>> rangeHandler, double t)
        {
            var curvePoints = rangeHandler.Invoke(t).Select(point => new PointF(ScaleX(point.Item1), ScaleY(point.Item2))).ToArray();
            var previousPoint = curvePoints.First();

            foreach (var point in curvePoints)
            {
                DdaLine(previousPoint.X, previousPoint.Y, point.X, point.Y);

                previousPoint = point;
            }
        }

        private float ScaleX(double x)
        {
            return (float)(_imageScaleCoeff * x + _width / 2);
        }

        private float ScaleY(double y)
        {
            return (float)(-_imageScaleCoeff * y + _height / 2);
        }

        #region [ Drawing ]

        private void DdaLine(float x1, float y1, float x2, float y2)
        {
            var x = x1;
            var y = y1;
            var length = Math.Abs(x2 - x1) > Math.Abs(y2 - y1) ? Math.Abs(x2 - x1) : Math.Abs(y2 - y1);
            var dx = (x2 - x1) / length;
            var dy = (y2 - y1) / length;

            DrawPixel((int)x, (int)y);

            for (int i = 0; i < length; i++)
            {
                x += dx;
                y += dy;
                DrawPixel((int)x, (int)y);
            }
        }

        private void DrawPixel(int x, int y)
        {
            unsafe
            {
                var color = Color.FromArgb(255, 96, 128);
                byte* pBackBuffer = (byte*)_bitmapCache.pBackBuffer;

                pBackBuffer += x * 4;
                pBackBuffer += y * _bitmapCache.BackBufferStride;

                if (x >= 0 && x < _bitmapCache.PixelWidth && y >= 0 && y < _bitmapCache.PixelHeight)
                {
                    int colorData = color.R << 16;
                    colorData |= color.G << 8;
                    colorData |= color.B << 0;
                    colorData |= 255 << 24;

                    *(int*)pBackBuffer = colorData;
                }
            }
        }

        private void ClearBitmap(WriteableBitmap bitmap)
        {
            RtlZeroMemory(bitmap.BackBuffer, bitmap.PixelWidth * bitmap.PixelHeight * (bitmap.Format.BitsPerPixel / 8));
            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern void RtlZeroMemory(IntPtr dst, int length);

        #endregion
    }
}
