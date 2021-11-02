using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs {
    public static class Static_Utilities {

        public static Random Random = new Random(DateTime.UtcNow.Second + DateTime.UtcNow.Millisecond);

        public static double RandomDouble(double max, double min = 0) {
            return Random.NextDouble() * (min - max) + max;
        }

    }
}
