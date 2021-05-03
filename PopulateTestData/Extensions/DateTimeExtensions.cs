using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfMonth(this DateTime subject)
        {
            return new DateTime(subject.Year, subject.Month, DateTime.DaysInMonth(subject.Year, subject.Month));
        }

        public static DateTime WithTime(this DateTime subject, int hours, int minutes, int seconds)
        {
            return new DateTime(subject.Year, subject.Month, subject.Day, hours, minutes, seconds);
        }

        public static DateTime WithTime(this DateTime subject, DateTime time)
        {
            return new DateTime(subject.Year, subject.Month, subject.Day, time.Hour, time.Minute, time.Second);
        }
    }
}
