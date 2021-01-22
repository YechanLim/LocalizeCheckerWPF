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

        public static LogTableInfo[] Stretch(string filePath, BackgroundWorkerParameter backgroundWorkerParameter)
        {

            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(filePath));
            LogTableInfo[] logTableInfos = new LogTableInfo[0];

            if (!FileSave.SaveBackupFiles(filePaths))
            {
                Console.WriteLine("이미 변환된 파일입니다.");
                return logTableInfos;
            }

            FileStretcher fileStretcher = new FileStretcher();
            fileStretcher.StretchFiles(filePaths, backgroundWorkerParameter);

            if (backgroundWorkerParameter.Worker.CancellationPending)
            {
                return logTableInfos;
            }

            LogStore logStore = new LogStore();
            logTableInfos = logStore.StoreLogOfStretcher(filePaths, fileStretcher.stretchingFailedFilesInfos);
            return logTableInfos;
        }

        public static LogTableInfo[] Revert(string filePath, BackgroundWorkerParameter backgroundWorkerParameter)
        {
            filePaths = FilePathFinder.GetResxFilePathList(FilePathFinder.GetCSprojFilePathList(filePath));
            LogTableInfo[] logTableInfos = new LogTableInfo[0];
            FileReverter fileRevert = new FileReverter();

            if (!fileRevert.RevertFiles(filePaths, backgroundWorkerParameter, false))
            {
                Console.WriteLine("복원이 취소됐습니다.");
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
                   BackgroundWorkerParameter backgroundWorkerParameter = new BackgroundWorkerParameter();
                   if (opt.Stretch)
                   {
                       Stretch(opt.FilePath, backgroundWorkerParameter);
                   }
                   else if (opt.Revert)
                   {
                       Revert(opt.FilePath, backgroundWorkerParameter);
                   }

               })
               .WithNotParsed<Options>(e =>
               {
               });
            }
        }
    }
}
