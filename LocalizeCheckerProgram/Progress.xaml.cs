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

    public partial class Progress : Window
    {
        const string titleText_Revert = "다국어 복원중";
        const string titleText_Stretch = "다국어 변환중";
        const string titleText_RevertResult = "결과 복원중";
        public bool isAlreadyCompleted = false;
        public BackgroundWorker worker;
        public bool isAsycStretch;
        public LogTableInfo[] logTableInfos = new LogTableInfo[0];

        public Progress()
        {
            InitializeComponent();
        }

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;



            if (isAsycStretch)
            {
                worker.DoWork += Worker_DoWork_Stretch;
            }
            else
            {
                title.Text = titleText_Revert;
                worker.DoWork += Worker_DoWork_Revert;
            }

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork_Stretch(object sender, DoWorkEventArgs e)
        {
            logTableInfos = Program.Stretch(Home.filePath, InitializeBackgroundWorkerParameter(isAsycStretch, sender, e));
        }

        private void Worker_DoWork_Revert(object sender, DoWorkEventArgs e)
        {
            logTableInfos = Program.Revert(Home.filePath, InitializeBackgroundWorkerParameter(isAsycStretch, sender, e));
        }

        private BackgroundWorkerParameter InitializeBackgroundWorkerParameter(bool isAsycStretch, object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameter backgroundWorkerParameter = new BackgroundWorkerParameter();
            backgroundWorkerParameter.IsAsynStretch = isAsycStretch;
            backgroundWorkerParameter.Sender = sender;
            backgroundWorkerParameter.Args = e;
            backgroundWorkerParameter.Worker = worker;
            return backgroundWorkerParameter;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarStatus.Value = e.ProgressPercentage;
            log.Text = (string)e.UserState;
        }

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            else if (progressBarStatus.Value != 100)
            {
                isAlreadyCompleted = true;
            }
            Console.WriteLine("completed done");

            _forceClose = true;
            this.Close();
        }

        private bool _forceClose = false;

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_forceClose)
            {
                return;
            }

            if (progressBarStatus.Value != 0 && progressBarStatus.Value != 100)
            {
                e.Cancel = true;
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                if (MessageBox.Show("확인: 결과 복원 및 종료", "취소하시겠습니까?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    worker.CancelAsync();
                    title.Text = titleText_RevertResult;
                    e.Cancel = true;
                    return;
                }
            }
        }

    }
}
