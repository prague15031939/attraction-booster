using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttractionBooster
{
    public class Renderer
    {
        public Graphics OutputGraphics { get; private set; }

        //private Graphics CachedGraphics;

        private int _width;

        private int _height;

        private AttractionCore _core;

        private static object _lock = new object();

        public Renderer(Graphics graphics, int width, int height)
        {
            OutputGraphics = graphics;

            //CachedGraphics = OutputGraphics;

            _width = width;
            _height = height;
            _core = new AttractionCore();
        }

        public void Run(object obj)
        {
            var cancellationToken = (CancellationToken)obj;

            while (true)
            {
                for (var t = 0.0d; t <= 13; t += 0.2)
                {
                    Render(t);

                    if (cancellationToken.IsCancellationRequested) return;
                }

                for (var t = 13.0d; t >= 0; t -= 0.2)
                {
                    Render(t);

                    if (cancellationToken.IsCancellationRequested) return;
                }
            }
        }

        private void Render(double t)
        {
            var timer = new Stopwatch();
            timer.Start();

            OutputGraphics.Clear(Color.White);

            var rightPart = Task.Run(() => RenderPart(_core.GetRigthRange, t));
            var leftPart = Task.Run(() => RenderPart(_core.GetLeftRange, t));

            Task.WaitAll(new[] { rightPart, leftPart });

            //Thread.Sleep(20);

            Debug.WriteLine(timer.Elapsed);
        }

        private void RenderPart(Func<double, IEnumerable<(double, double)>> rangeHandler, double t)
        {
            var pen = new Pen(Color.FromArgb(255, 184, 198), 2.5f);

            var previousPoint = _core.GetInitial(t);

            foreach (var point in rangeHandler.Invoke(t))
            {
                var curvePoints = new PointF[]
                {
                    new PointF(ScaleX(previousPoint.Item1), ScaleY(previousPoint.Item2)),
                    new PointF(ScaleX(point.Item1), ScaleY(point.Item2)),
                };

                lock (_lock)
                {
                    OutputGraphics.DrawCurve(pen, curvePoints);
                }

                previousPoint = point;
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
