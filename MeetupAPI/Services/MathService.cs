using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Services
{
    public class MathService : IMath
    {
        public double Average(IList<double> numbers)
        {
            var sum = numbers.Sum();
            var count = numbers.Count;
            var result = sum / count;
            return result;
        }
    }
}
