using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LocalizeChecker
{
    class FileSave
    {
        public static bool SaveBackupFiles(List<string> filePaths, string solutionName)
        {
            string targetPath = $"{Application.StartupPath}\\Backup\\{solutionName}";

            if (Directory.Exists(targetPath))
            {
                return false;
            }

            Directory.CreateDirectory(targetPath);
            int index = 0;
            foreach (string filePath in filePaths)
            {
                string fileName = $"backup{++index}.resx";
                string destinationFile = Path.Combine(targetPath, fileName);
                File.Copy(filePath, destinationFile, true);
            }
            return true;
        }

    }
}
