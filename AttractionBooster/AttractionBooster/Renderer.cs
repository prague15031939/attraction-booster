using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AttractionBooster
{
    public class Renderer
    {
        public Graphics OutputGraphics { get; private set; }

        private int _width;

        private int _height;

        private AttractionCore _core;

        private static object _lock = new object();

        public Renderer(Graphics graphics, int width, int height)
        {
            OutputGraphics = graphics;

            _width = width;
            _height = height;
            _core = new AttractionCore();
        }

        public void Run(object obj)
        {
            var cancellationToken = (CancellationToken)obj;

            while (true)
            {
                for (var t = 0.0d; t <= 15; )
                {
                    if (IsRenderRequired)
                    {
                        Render(t);
                        t += 0.2;
                    }

                    if (cancellationToken.IsCancellationRequested) return;
                }

                for (var t = 15.0d; t >= 0; )
                {
                    if (IsRenderRequired)
                    {
                        Render(t);
                        t -= 0.2;
                    }

                    if (cancellationToken.IsCancellationRequested) return;
                }
            }
        }

        private bool IsRenderRequired => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() % 50 == 0;

        private void Render(double t)
        {
            OutputGraphics.Clear(Color.White);

            var rightPart = Task.Run(() => RenderPart(_core.GetRigthRange, t));
            var leftPart = Task.Run(() => RenderPart(_core.GetLeftRange, t));

            Task.WaitAll(new[] { rightPart, leftPart });
        }

        private void RenderPart(Func<double, IEnumerable<(double, double)>> rangeHandler, double t)
        {
            var pen = new Pen(Color.FromArgb(255, 184, 198), 2.5f);

            var curvePoints = rangeHandler.Invoke(t).Select(point => new PointF(ScaleX(point.Item1), ScaleY(point.Item2))).ToArray();

            lock (_lock)
            {
                OutputGraphics.DrawCurve(pen, curvePoints);
            }

            pen.Dispose();
        }

        private float ScaleX(double x)
        {
            return (float)(75 * x + _width / 2);
        }

        private float ScaleY(double y)
        {
            return (float)(-75 * y + _height / 2 + 40);
        }
    }
}
