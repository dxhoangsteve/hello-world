using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Helper
{
    public static class RandomHelper
    {
        private static readonly Random _rng = new Random();

        public static string RandomString(int size)
        {
            string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static string RandomStringNumber(int size)
        {
            string _chars = "1234567890";
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static int RandomNumber(int min, int max)
        {
            return _rng.Next(min, max + 1);
        }

        public static T RandomValueFromEnum<T>(int? min = null, int? max = null)
        {
            var values = Enum.GetValues(typeof(T));
            var rs = (T)values.GetValue(_rng.Next(values.Length));

            while ((min != null && Convert.ToInt32(rs) < min) || (max != null && Convert.ToInt32(rs) > max))
            {
                rs = (T)values.GetValue(_rng.Next(values.Length));
            }

            return rs;
        }
    }
}
