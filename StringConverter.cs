using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransform
{
    class StringConverter
    {
        public static string FloatToStringTime(float time)
        {
            double _time = Convert.ToDouble(time);
            return DoubleToStringTime(_time);
        }

        public static string DoubleToStringTime(double time)
        {
            int miliseconds = Convert.ToInt32(time / 0.001);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, miliseconds);
            int hour = ts.Hours;
            int min = ts.Minutes;
            int sec = ts.Seconds;
            int mili = ts.Milliseconds;
            string minStr;
            if (hour > 0)
            {
                if (min >= 10) { minStr = min.ToString() + ":"; }
                else { minStr = "0" + min.ToString() + ":"; }
            }
            else
            {
                if (min > 0) { minStr = min.ToString() + ":"; }
                else { minStr = ""; }
            }
            return (hour == 0 ? "" : hour.ToString() + ":") + minStr +
                (sec < 10 ? "0" + sec.ToString() : sec.ToString()) + "." +
                (mili < 10 ? "00" + mili.ToString() : (mili < 100 ? "0" + mili.ToString() : mili.ToString()));
            //time = Math.Round(time, 3);
            //int hour = Convert.ToInt32(time / 3600);
            //int minute = Convert.ToInt32(time / 60);
            //double second = time % 60;
            //return (hour == 0 ? "" : hour.ToString() + ":") + 
            //    (hour == 0 && minute == 0 ? "" : (minute < 10 && hour != 0 ? "0" + minute.ToString() : minute.ToString()) + ":") + 
            //    second.ToString("F3");
        }
    }
}