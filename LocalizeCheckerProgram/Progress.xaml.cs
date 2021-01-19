using System.Windows.Controls;
using System.Windows.Navigation;
using System;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using LocalizeChecker;

namespace LocalizeCheckerProgram
{
    public partial class Progress : Window
    {
        public bool isAlreadyStretched = false;
        public bool isAlreadyRestored = false;

        public Progress()
        {
            InitializeComponent();
        }

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            if (Home.isAsycStretch)
            {
                worker.DoWork += Worker_DoWork_Stretch;
            }
            else
            {
                worker.DoWork += Worker_DoWork_Restore;
            }

            worker.RunWorkerAsync();
        }

        void Worker_DoWork_Stretch(object sender, DoWorkEventArgs e)
        {
            Program.Stretch(Home.filePath, sender);
        }

        void Worker_DoWork_Restore(object sender, DoWorkEventArgs e)
        {
            Program.Restore(Home.filePath, sender);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
            if (progressBarStatus.Value == 0 && Home.isAsycStretch)
            {
                isAlreadyStretched = true;
            }
            else if (progressBarStatus.Value == 0 && !Home.isAsycStretch)
            {
                isAlreadyRestored = true;
            }

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
