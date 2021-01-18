using System.Windows.Controls;
using System.Windows.Navigation;
using System;
using System.Threading;
using System.Windows;
using System.ComponentModel;

namespace LocalizeCheckerProgram
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : Window
    {
        public Progress()
        {
            InitializeComponent();
        }

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;

			worker.RunWorkerAsync();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < 100; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);
				Thread.Sleep(100);
			}
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBarStatus.Value = e.ProgressPercentage;
		}
	}
}
