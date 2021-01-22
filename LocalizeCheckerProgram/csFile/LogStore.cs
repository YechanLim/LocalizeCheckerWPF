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
            Result = result;
            Remark = remark;
            Blank = "";
        }

        public int Index { get; set; }
        public string Path { get; set; }
        public string Result { get; set; }
        public string Remark { get; set; }

        public string Blank { get; }
    }

    class LogStore
    {
        public LogTableInfo[] StoreLogOfStretcher(List<string> filePaths, List<StretchingFailedFilesInfo> stretchingFailedFilesInfos)
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
                    logTableInfos[index].Result = "실패\n원인: " + stretchingFailedFilesInfos[failedFileIndex].ResultMessage;
                    logTableInfos[index].Remark = stretchingFailedFilesInfos[failedFileIndex].RemarkMessage;
                    failedFileIndex++;
                    Program.WriteToConsole($"[실패]  {filePath}");
                    continue;
                }
                Program.WriteToConsole($"[성공]  {filePath}");
                logTableInfos[index].Result = "성공";
                logTableInfos[index].Remark = "";
            }
            Program.WriteToConsole($"\n  File: {filePaths.Count}  Success: {filePaths.Count - failedFileIndex}  Fail: {failedFileIndex}");
            return logTableInfos;
        }

        public LogTableInfo[] StoreLogOfReverter(List<string> filePaths, List<ReversionFailedFilesInfo> reversionFailedFilesInfos)
        {
            int failedFileIndex = 0;
            int index = -1;
            LogTableInfo[] logTableInfos = new LogTableInfo[filePaths.Count];
            foreach (string filePath in filePaths)
            {
                index++;
                logTableInfos[index].Index = index + 1;
                logTableInfos[index].Path = filePath;

                if (failedFileIndex < reversionFailedFilesInfos.Count && index == reversionFailedFilesInfos[failedFileIndex].Index)
                {
                    logTableInfos[index].Result = "실패\n원인: " + reversionFailedFilesInfos[failedFileIndex].ResultMessage;
                    logTableInfos[index].Remark = reversionFailedFilesInfos[failedFileIndex].RemarkMessage;
                    failedFileIndex++;
                    Program.WriteToConsole($"[실패]  {filePath}");
                    continue;
                }
                Program.WriteToConsole($"[성공]  {filePath}");
                logTableInfos[index].Result = "성공";
                logTableInfos[index].Remark = "";
            }
            Program.WriteToConsole($"\n  File: {filePaths.Count}  Success: {filePaths.Count - failedFileIndex}  Fail: {failedFileIndex}");
            Console.WriteLine($"\n  File: {filePaths.Count}  Success: {filePaths.Count - failedFileIndex}  Fail: {failedFileIndex}");

            return logTableInfos;
        }

    }
}
