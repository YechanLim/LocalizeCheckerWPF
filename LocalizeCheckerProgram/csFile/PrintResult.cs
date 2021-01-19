using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizeChecker
{
    class PrintResult
    {
        public static void PrintResultOfLocalizeChecker(List<string> filePaths, List<StretchingFailedFilesInfo> stretchingFailedFilesInfos)
        {
            int failedFileListIndex = 0;
            int filePathsIndex = 0;

            foreach (string filePath in filePaths)
            {
                if (failedFileListIndex < stretchingFailedFilesInfos.Count && filePathsIndex == stretchingFailedFilesInfos[failedFileListIndex].Index)
                {
                    Console.WriteLine($"[fail]  {filePath}");
                    failedFileListIndex++;
                }
                else
                {
                    Console.WriteLine($"[success]  {filePath}");
                }

                filePathsIndex++;
            }
            Console.WriteLine($"\n  File: {filePaths.Count}  Success: {filePaths.Count - stretchingFailedFilesInfos.Count}  Fail: {stretchingFailedFilesInfos.Count}");
        }
    }
}
