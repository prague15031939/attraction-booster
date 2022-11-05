using System;
using System.Collections.Generic;

namespace WpfAttractionBooster.Core
{
    public class AttractionCore
    {
        private const double _start = -1.8;

        private const double _end = 1.8;

        private const double _step = 0.005;

        public IEnumerable<(double, double)> GetRigthRange(double t)
        {
            for (var x = 0.0d; x <= _end; x += _step)
            {
                yield return (x, GetValue(x, t));
            }
        }

        public IEnumerable<(double, double)> GetLeftRange(double t)
        {
            for (var x = 0.0d; x >= _start; x -= _step)
            {
                yield return (x, GetValue(x, t));
            }
        }

        private double GetValue(double x, double t)
        {
            return Math.Cbrt(x * x) + 0.9 * Math.Sqrt(3.3 - x * x) * Math.Sin(t * Math.PI * x);
        }
    }
}
