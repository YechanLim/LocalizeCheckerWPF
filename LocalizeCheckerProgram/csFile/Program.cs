using System;
using System.Collections.Generic;
using System.IO;

namespace LocalizeChecker
{
    class Program
    {
        public static List<string> filePaths = new List<string>();
        public static LogTableInfo[] logTableInfos;

        public static bool Stretch(string[] args)
        {
            string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));
            
            if (!FileSave.SaveBackupFiles(filePaths))
            {
                return false;
            }
            
            FileStretcher fileStretcher = new FileStretcher();
            fileStretcher.StretchFiles(filePaths);
            logTableInfos = LogStore.StoreLogOfStretcher(filePaths, FileStretcher.stretchFailedFilesInfos);
            return true;
        }

        public static bool Restore(string[] args)
        {
        string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));

            if (!FileRestore.RestoreFiles(filePaths, Path.GetFileName(solutionFilePath)))
            {
                return false;
            }

            logTableInfos = LogStore.StoreLogOfRestorer(filePaths, FileRestore.restorationFailedFilesInfos);
            return true;
        }
    }
}