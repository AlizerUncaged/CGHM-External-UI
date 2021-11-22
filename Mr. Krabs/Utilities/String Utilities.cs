using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class String_Utilities {
        // null terminates a string
        public static string NullTerminate(this string str) {
            return str + '\0';
        }
    }
}
