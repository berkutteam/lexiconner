using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Extensions
{
    public static class RandomExtensions
    {
        public static long NextPositiveAndGreaterThenZero(this Random random, long minValue, long maxValue)
        {
            throw new NotImplementedException();

            int intRangesInLong = Convert.ToInt32(Math.Floor((double)(long.MaxValue - 0) / (double)(int.MaxValue - 0)));
            int rangeN = random.Next(0, intRangesInLong);
            long rangeStart = rangeN * int.MaxValue;
            int intRangeRandom = random.Next(0, int.MaxValue);
            long result = rangeStart + intRangeRandom;
            return result;
        }
    }
}
