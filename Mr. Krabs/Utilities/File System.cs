using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class FileSystem {
        public static readonly string CurrentFilename = Assembly.GetExecutingAssembly().Location;
        public static readonly string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
