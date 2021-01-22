using System;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using LocalizeChecker;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LocalizeCheckerProgram
{
    public partial class Home : Page
    {
        string fileSelectionMissingMessage = "선택된 파일이 없습니다.";
        string StretchingFailedMessage = "이미 변환된 파일입니다.";
        string reversionFailedMessage = "이미 복원된 파일입니다.";
        public static string[] filePath;

        public Home()
        {
            InitializeComponent();
            filePathTextBlock.Text = fileSelectionMissingMessage;
        }

        private void SelectFileDialog_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Solution Files (*.sln)|*.sln";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePathTextBlock.Text = openFileDialog.FileName;
                filePathTextBlock.ToolTip = openFileDialog.FileName;
                InitializeAlertText();
                SetLogNumberText(new LogTableInfo[0]);
                MakeDataGrid(new LogTableInfo[0]);
            }
        }

        private void StretchFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionMissingMessage)
            {
                stretchingAlertText.Text = fileSelectionMissingMessage;
                return;
            }

            string[] filePath_temp = { filePathTextBlock.Text };
            filePath = filePath_temp;
            Progress progressPage = new Progress();
            progressPage.isAsycStretch = true;
            OpenProgressPage(progressPage);

            if (progressPage.isAlreadyCompleted)
            {
                SetLogNumberText(progressPage.logTableInfos);
                stretchingAlertText.Text = StretchingFailedMessage;
                MakeDataGrid(new LogTableInfo[0]);
                return;
            }

            SetLogNumberText(progressPage.logTableInfos);
            InitializeAlertText();
            MakeDataGrid(progressPage.logTableInfos);
        }

        private void RevertFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionMissingMessage)
            {
                reversionAlertText.Text = fileSelectionMissingMessage;
                return;
            }

            string[] filePath1 = { filePathTextBlock.Text };
            filePath = filePath1;
            Progress progressPage = new Progress();
            progressPage.isAsycStretch = false;
            OpenProgressPage(progressPage);

            if (progressPage.isAlreadyCompleted)
            {
                SetLogNumberText(progressPage.logTableInfos);
                reversionAlertText.Text = reversionFailedMessage;
                MakeDataGrid(new LogTableInfo[0]);
                return;
            }

            SetLogNumberText(progressPage.logTableInfos);
            InitializeAlertText();
            MakeDataGrid(progressPage.logTableInfos);
        }

        private void OpenProgressPage(Progress progressPage)
        {
            progressPage.Owner = Window.GetWindow(this);
            progressPage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            progressPage.ShowDialog();
        }

        private void MakeDataGrid(LogTableInfo[] resultTableLogInfos)
        {
            try
            {
                dataGrid.ItemsSource = resultTableLogInfos;
                dataGrid.Columns[dataGrid.Columns.Count - 1].Header = "";
                dataGrid.Columns[dataGrid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            catch (Exception e)
            {
                Console.WriteLine("DataGrid를 만드는데 오류가 발생했습니다. 원인: " + e.Message);
            }
        }

        private void SetLogNumberText(LogTableInfo[] logTableInfos)
        {
            fileNum.Text = logTableInfos.Length.ToString();

            if (logTableInfos.Length == 0)
            {
                successfulFileNum.Text = "0";
                failedFileNum.Text = "0";
                return;
            }

            int failedNum = 0;
            foreach (LogTableInfo logTableInfo in logTableInfos)
            {
                if (logTableInfo.Result == "성공")
                {
                    continue;
                }
                failedNum++;
            }
            successfulFileNum.Text = (logTableInfos.Length - failedNum).ToString();
            failedFileNum.Text = failedNum.ToString();
        }

        private void InitializeAlertText()
        {
            stretchingAlertText.Text = "";
            reversionAlertText.Text = "";
        }
    }
}
