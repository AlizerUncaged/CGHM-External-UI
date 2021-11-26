using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class StringUtilities {
        public static string NullTerminate(this string str) {
            return str + '\0';
        }
    }
}
