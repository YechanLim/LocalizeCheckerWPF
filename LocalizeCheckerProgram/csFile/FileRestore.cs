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
            restorationFailedFilesInfos = new List<RestorationFailedFilesInfo>();
            int index = 0;
            try
            {
                foreach (string filePath in filePaths)
                {
                    string sourceFile = filePath + ".#localizer#";

                    if (!File.Exists(sourceFile))
                    {
                        return false;
                    }

                    File.Copy(sourceFile, filePath, true);
                    File.Delete(sourceFile);
                    index++;
                }
            }
            catch (Exception e)
            {
                restorationFailedFilesInfos.Add(new RestorationFailedFilesInfo(index, "backup파일을 만드는데 실패했습니다.", e.Message));
            }

            return true;
        }
    }
}
