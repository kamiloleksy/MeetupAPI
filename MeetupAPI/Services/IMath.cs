using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Services
{
    public interface IMath
    {
        public double Average(IList<double> numbers);
    }
}
