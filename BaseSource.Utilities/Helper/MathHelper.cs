using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Helper
{
    public class MathHelper
    {
        public static double Floor(double value, int precision) // làm tròn xuống
        {
            double step = Math.Pow(10, precision);
            double saiso = 1.0 / 1000000;
            return Math.Floor(step * (value + saiso)) / step;
        }

        public static double Ceiling(double value, int precision) // làm tròn lên
        {
            double step = Math.Pow(10, precision);
            double saiso = 1.0 / 1000000;
            return Math.Ceiling(step * (value - saiso)) / step;
        }
    }
}
