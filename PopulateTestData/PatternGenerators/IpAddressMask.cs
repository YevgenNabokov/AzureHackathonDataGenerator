using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateTestData.PatternGenerators
{
    public class IpAddressMask
    {
        public IpAddressMask(int? part1, int? part2, int? part3, int? part4)
        {
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
            Part4 = part4;
        }

        public int? Part1 { get; }

        public int? Part2 { get; }

        public int? Part3 { get; }

        public int? Part4 { get; }

        public string GetRandomIpAddressString()
        {
            var rnd = new Random();
            return $"{this.Part1 ?? rnd.Next(0, 255)}.{this.Part2 ?? rnd.Next(0, 255)}.{this.Part3 ?? rnd.Next(0, 255)}.{this.Part4 ?? rnd.Next(0, 255)}";
        }
    }
}
