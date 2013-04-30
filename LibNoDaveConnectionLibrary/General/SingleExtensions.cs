using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class SingleExtensions
    {
        public static string ToEngineering(float value)
        {
            int exp = (int)(Math.Floor(Math.Log10(value) / 3.0) * 3.0);
            double newValue = value * Math.Pow(10.0, -exp);
            if (newValue >= 1000.0)
            {
                newValue = newValue / 1000.0;
                exp = exp + 3;
            }
            return string.Format("{0:##0}e{1}", newValue, exp);
        }

        public static string ToS5(float value)
        {
            double wrt = value * 10000000;
            return wrt.ToString("0000000E+00").Replace("E", "");


            //double wrt = value * 10000000;
            int exp = (int)(Math.Floor(Math.Log10(wrt) / 3.0) * 3.0);
            double newValue = wrt * Math.Pow(10.0, -exp);
            /*if (newValue >= 1000.0)
            {
                newValue = newValue / 1000.0;
                exp = exp + 3;
            }*/

            if (newValue < 1000000.0)
            {
                newValue = newValue * 1000000.0;
                exp = exp - 6;
            }

            while (newValue > 10000000.0)
            {
                newValue = newValue / 10.0;
                exp = exp + 1;
            }

            return string.Format("+{0:##0}+{1}", newValue, exp);
        }
    }
}
