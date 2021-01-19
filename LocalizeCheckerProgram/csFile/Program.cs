using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using CommandLine;

namespace LocalizeChecker
{
    class Program
    {
        public static List<string> filePaths = new List<string>();
        public static LogTableInfo[] logTableInfos;

        public static bool Stretch(string[] args, object sender)
        {
            string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));

            if (!FileSave.SaveBackupFiles(filePaths))
            {
                return false;
            }

            FileStretcher fileStretcher = new FileStretcher();
            fileStretcher.StretchFiles(filePaths, sender);
            logTableInfos = LogStore.StoreLogOfStretcher(filePaths, FileStretcher.stretchFailedFilesInfos);
            return true;
        }

        public static bool Restore(string[] args, object sender)
        {
            string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));

            if (!FileRestore.RestoreFiles(filePaths, sender))
            {
                return false;
            }

            logTableInfos = LogStore.StoreLogOfRestorer(filePaths, FileRestore.restorationFailedFilesInfos);
            return true;
        }

        public static void TestThread(object sender)
        {
            int k = 0;
            while (k <= 100)
            {
                for (int i = 0; i < 1000000000; i++)
                {

                }
                k++;
                (sender as BackgroundWorker).ReportProgress(k);
            }
        }
    }
}
