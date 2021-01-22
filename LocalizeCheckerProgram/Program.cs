using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using CommandLine;
using LocalizeCheckerProgram;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace LocalizeChecker
{
    class Program
    {
        static List<string> filePaths = new List<string>();

        public static LogTableInfo[] Stretch(string[] args, BackgroundWorkerParameter backgroundWorkerParameter)
        {
            string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));
            LogStore logStore = new LogStore();
            LogTableInfo[] logTableInfos = new LogTableInfo[0];

            if (!FileSave.SaveBackupFiles(filePaths))
            {
                Console.WriteLine("이미 변환된 파일입니다.");
                return logTableInfos;
            }

            FileStretcher fileStretcher = new FileStretcher();
            fileStretcher.StretchFiles(filePaths, backgroundWorkerParameter);

            if (fileStretcher.isCanceled)
            {
                return logTableInfos;
            }

            logTableInfos = logStore.StoreLogOfStretcher(filePaths, FileStretcher.stretchFailedFilesInfos);
            return logTableInfos;
        }

        public static LogTableInfo[] Revert(string[] args, BackgroundWorkerParameter backgroundWorkerParameter)
        {
            string solutionFilePath = args[0];
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(solutionFilePath));
            LogTableInfo[] logTableInfos = new LogTableInfo[0];
            FileRevert fileRevert = new FileRevert();

            if (!fileRevert.RevertFiles(filePaths, backgroundWorkerParameter, false))
            {
                Console.WriteLine("취소됐습니다.");
                return logTableInfos;
            }

            LogStore logStore = new LogStore();
            logTableInfos = logStore.StoreLogOfReverter(filePaths, fileRevert.reversionFailedFilesInfos);
            return logTableInfos;
        }

        public static void WriteToConsole(string message)
        {
            AttachConsole(-1);
            Console.WriteLine(message);
        }
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);


        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            else
            {
                CommandLine.Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(opt =>
               {
                   //WriteToConsole("");
                   string[] targetFilePath = { opt.FilePath };
                   BackgroundWorkerParameter backgroundWorkerParameter = new BackgroundWorkerParameter();
                   if (opt.Stretch)
                   {
                       Stretch(targetFilePath, backgroundWorkerParameter);
                   }
                   else if (opt.Revert)
                   {
                       Revert(targetFilePath, backgroundWorkerParameter);
                   }

               })
               .WithNotParsed<Options>(e =>
               {
               });
                //Console.ReadKey();
            }
        }
    }
}
