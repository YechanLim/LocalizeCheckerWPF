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

    class AsyncLocalizeChecker
    {
        public LogTableInfo[] logTableInfos = new LogTableInfo[0];
        public bool isAlreadyCompleted = false;
        const string RevertTitleText = "다국어 복원중";
        const string StretchTitleText = "다국어 변환중";
        const string RevertResultTitleText = "결과 복원중";
        bool isAsyncStretch;
        string filePath;
        Progress progress;
        BackgroundWorker worker;

        public AsyncLocalizeChecker(string filePath, bool isAsyncStretch)
        {
            this.filePath = filePath;
            this.isAsyncStretch = isAsyncStretch;
        }

        public void StartLocalizeCheckerAsync()
        {
            worker = new BackgroundWorker();
            progress = new Progress(worker);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += progress.Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            if (isAsyncStretch)
            {
                progress.title.Text = StretchTitleText;
                worker.DoWork += Worker_DoWork_Stretch;
            }
            else
            {
                progress.title.Text = RevertTitleText;
                worker.DoWork += Worker_DoWork_Revert;
            }

            worker.RunWorkerAsync();
            progress.ShowDialog();
        }

        private void Worker_DoWork_Stretch(object sender, DoWorkEventArgs args)
        {
            logTableInfos = Program.Stretch(filePath, InitializeBackgroundWorkerParameter(isAsyncStretch, sender, args));
        }

        private void Worker_DoWork_Revert(object sender, DoWorkEventArgs args)
        {
            logTableInfos = Program.Revert(filePath, InitializeBackgroundWorkerParameter(isAsyncStretch, sender, args));
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
            else if (progress.progressBarStatus.Value != 100)
            {
                isAlreadyCompleted = true;
            }
            progress.Close();
        }


        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            return;
        }
    }
}
