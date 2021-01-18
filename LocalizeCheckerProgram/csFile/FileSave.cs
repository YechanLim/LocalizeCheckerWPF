using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LocalizeChecker
{
    class FileSave
    {
        public static bool SaveBackupFiles(List<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                string destinationFile = filePath + ".#localizer#";
                if (File.Exists(destinationFile))
                {
                    return false;
                }

                File.Copy(filePath, destinationFile, true);
            }
            return true;
        }

    }
}
