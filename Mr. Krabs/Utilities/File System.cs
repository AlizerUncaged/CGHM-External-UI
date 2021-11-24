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
        public static readonly string CurrentFolder = Path.GetDirectoryName(CurrentFilename);

        public static readonly string ChewyGumballUserDataFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\ChewyGumball";

        private static readonly string installationFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\ChewyGumball\External";
        public static string ChewyGumballInstallationFolder {
            get {
                if (!Directory.Exists(installationFolder)) {
                    Directory.CreateDirectory(installationFolder);
                }
                return installationFolder;
            }
        }



    }
}
