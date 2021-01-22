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
        const string titleText_RevertResult = "결과 복원중";
        BackgroundWorker worker;

        public Progress(BackgroundWorker worker)
        {
            this.worker = worker;
            InitializeComponent();
        }

        public void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarStatus.Value = e.ProgressPercentage;
            log.Text = (string)e.UserState;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
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
                return;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show("확인: 결과 복원 및 종료", "취소하시겠습니까?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                worker.CancelAsync();
                title.Text = titleText_RevertResult;
            }
        }
    }
}
