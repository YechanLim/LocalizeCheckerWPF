using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizeChecker
{
    public struct LogTableInfo
    {
        public LogTableInfo(int index, string path, string result, string remark)
        {
            Index = index;
            Path = path;
            결과 = result;
            Remark = remark;
            Blank = "";
        }

        public int Index { get; set; }
        public string Path { get; set; }
        public string 결과 { get; set; }
        public string Remark { get; set; }

        public string Blank { get; }
    }

    class LogStore
    {
        public static LogTableInfo[] StoreLogOfStretcher(List<string> filePaths, List<StretchingFailedFilesInfo> stretchingFailedFilesInfos)
        {
            int failedFileIndex = 0;
            int index = -1;
            LogTableInfo[] logTableInfos = new LogTableInfo[filePaths.Count];

            foreach (string filePath in filePaths)
            {
                index++;
                logTableInfos[index].Index = index + 1;
                logTableInfos[index].Path = filePath;

                if (failedFileIndex < stretchingFailedFilesInfos.Count && index == stretchingFailedFilesInfos[failedFileIndex].Index)
                {
                    logTableInfos[index].결과 = "실패\n원인: " + stretchingFailedFilesInfos[failedFileIndex].ResultMessage;
                    logTableInfos[index].Remark = stretchingFailedFilesInfos[failedFileIndex].RemarkMessage;
                    failedFileIndex++;
                    continue;
                }

                logTableInfos[index].결과 = "성공";
                logTableInfos[index].Remark = "";
            }

            return logTableInfos;
        }

        public static LogTableInfo[] StoreLogOfRestorer(List<string> filePaths, List<RestorationFailedFilesInfo> restorationFailedFilesInfos)
        {
            int failedFileIndex = 0;
            int index = -1;
            LogTableInfo[] logTableInfos = new LogTableInfo[filePaths.Count];
            foreach (string filePath in filePaths)
            {
                index++;
                logTableInfos[index].Index = index + 1;
                logTableInfos[index].Path = filePath;

                if (failedFileIndex < restorationFailedFilesInfos.Count && index == restorationFailedFilesInfos[failedFileIndex].Index)
                {
                    logTableInfos[index].결과 = "실패\n원인: " + restorationFailedFilesInfos[failedFileIndex].ResultMessage;
                    logTableInfos[index].Remark = restorationFailedFilesInfos[failedFileIndex].RemarkMessage;
                    failedFileIndex++;
                    continue;
                }

                logTableInfos[index].결과 = "성공";
                logTableInfos[index].Remark = "";
            }

            return logTableInfos;
        }

    }
}
