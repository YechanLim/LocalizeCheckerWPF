using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LocalizeChecker
{
    struct RestorationFailedFilesInfo
    {
        public int Index;
        public string ResultMessage;
        public string RemarkMessage;

        public RestorationFailedFilesInfo(int index, string resultMessage, string remarkMessage)
        {
            Index = index;
            ResultMessage = resultMessage;
            RemarkMessage = remarkMessage;
        }
    };

    class FileRestore
    {
        public static List<RestorationFailedFilesInfo> restorationFailedFilesInfos = new List<RestorationFailedFilesInfo>();

        public static bool RestoreFiles(List<string> filePaths, string solutionName)
        {
        
            string targetPath = $"{Application.StartupPath}\\Backup\\{solutionName}";

            if (!Directory.Exists(targetPath))
            {
                return false;
            }

            int index = 0;
            try
            {
                foreach (string filePath in filePaths)
                {
                    string fileName = $"backup{++index}.resx";
                    string sourceFile = Path.Combine(targetPath, fileName);
                    File.Copy(sourceFile, filePath, true);
                }
            }
            catch (Exception e)
            {
                restorationFailedFilesInfos.Add(new RestorationFailedFilesInfo(index, "backup파일을 만드는데 실패했습니다.", e.Message));
            }

            Directory.Delete(targetPath, true);
            return true;
        }
    }
}
