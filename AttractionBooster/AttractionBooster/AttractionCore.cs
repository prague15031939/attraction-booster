using System;
using System.Collections.Generic;

namespace AttractionBooster
{
    public class AttractionCore
    {
        private const double _start = -1.8;

        private const double _end = 1.8;

        public (double, double) GetInitial(double t)
        {
            return (0, GetValue(0, t));
        }

        public IEnumerable<(double, double)> GetRigthRange(double t)
        {
            for (var x = 0.0d; x <= _end; x += ScaleStep(t))
            {
                yield return (x, GetValue(x, t));
            }
        }

        public IEnumerable<(double, double)> GetLeftRange(double t)
        {
            for (var x = 0.0d; x >= _start; x -= ScaleStep(t))
            {
                yield return (x, GetValue(x, t));
            }
        }

        private double GetValue(double x, double t)
        {
            return Math.Cbrt(x * x) + 0.9 * Math.Sqrt(3.3 - x * x) * Math.Sin(t * Math.PI * x);
        }

        private double ScaleStep(double t)
        {
            if (t <= 7) return 0.03;

            if (t <= 11) return 0.02;

            return 0.015;
        }
    }
}
