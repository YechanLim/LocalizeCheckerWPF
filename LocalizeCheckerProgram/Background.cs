using System.Windows.Controls;
using System.Windows.Navigation;
using System;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using LocalizeChecker;


namespace LocalizeCheckerProgram
{
    public struct BackgroundWorkerParameter
    {
        public bool IsAsynStretch;
        public object Sender;
        public DoWorkEventArgs Args;
        public BackgroundWorker Worker;
    };

    class Background
    {
        public BackgroundWorker worker;
        public LogTableInfo[] logTableInfos = new LogTableInfo[0];
        public bool isAlreadyCompleted = false;

        const string titleText_Revert = "다국어 복원중";
        const string titleText_Stretch = "다국어 변환중";
        const string titleText_RevertResult = "결과 복원중";
        bool isAsyncStretch;
        string filePath;
        Progress progress;
        
        public Background(string filePath ,bool isAsyncStretch)
        {
            this.filePath = filePath;
            this.isAsyncStretch = isAsyncStretch;
        }

        public void Start()
        {
            worker = new BackgroundWorker();
            progress = new Progress(worker);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += progress.Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            if (isAsyncStretch)
            {
                worker.DoWork += Worker_DoWork_Stretch;
            }
            else
            {
                progress.title.Text = titleText_Revert;
                worker.DoWork += Worker_DoWork_Revert;
            }

            worker.RunWorkerAsync();
            progress.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            progress.ShowDialog();
        }

        private void Worker_DoWork_Stretch(object sender, DoWorkEventArgs args)
        {
            logTableInfos = Program.Stretch(filePath, InitializeBackgroundWorkerParameter(isAsyncStretch, sender, args));
        }

        private void Worker_DoWork_Revert(object sender, DoWorkEventArgs args)
        {
            logTableInfos = Program.Revert(filePath, InitializeBackgroundWorkerParameter(isAsyncStretch, sender, args));
            Console.WriteLine(logTableInfos.Length);
        }

        private BackgroundWorkerParameter InitializeBackgroundWorkerParameter(bool isAsycStretch, object sender, DoWorkEventArgs args)
        {
            BackgroundWorkerParameter backgroundWorkerParameter = new BackgroundWorkerParameter();
            backgroundWorkerParameter.IsAsynStretch = isAsycStretch;
            backgroundWorkerParameter.Sender = sender;
            backgroundWorkerParameter.Args = args;
            backgroundWorkerParameter.Worker = worker;
            return backgroundWorkerParameter;
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Console.WriteLine("취소가 완료되었습니다.");
            }
            else if (progress.progressBarStatus.Value != 100)
            {
                isAlreadyCompleted = true;
            }
            Console.WriteLine("completed done");
            progress.Close();
        }
    }
}
