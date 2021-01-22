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
        const string fileSelectionMissingMessage = "선택된 파일이 없습니다.";
        const string StretchingFailedMessage = "이미 변환된 파일입니다.";
        const string reversionFailedMessage = "이미 복원된 파일입니다.";
        const string solutionFileFilter = "Solution Files (*.sln)|*.sln";
        string filePath;

        public Home()
        {
            InitializeComponent();
            filePathText.Text = fileSelectionMissingMessage;
        }

        private void SelectFileDialog_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = solutionFileFilter;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathText.Text = openFileDialog.FileName;
                filePathText.ToolTip = openFileDialog.FileName;
                InitializeAlertText();
                SetResultLogNumber(new LogTableInfo[0]);
                MakeDataGrid(new LogTableInfo[0]);
            }
        }

        private void StretchFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathText.Text == fileSelectionMissingMessage)
            {
                stretchingAlertText.Text = fileSelectionMissingMessage;
                return;
            }

            filePath = filePathText.Text;
            AsyncLocalizeChecker localizeChecker_Background = new AsyncLocalizeChecker(filePath, true);
            localizeChecker_Background.StartLocalizeCheckerAsync();

            SetResultLogNumber(localizeChecker_Background.logTableInfos);
            MakeDataGrid(localizeChecker_Background.logTableInfos);
            if (localizeChecker_Background.isAlreadyCompleted)
            {
                stretchingAlertText.Text = StretchingFailedMessage;
                return;
            }
            InitializeAlertText();
        }

        private void RevertFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathText.Text == fileSelectionMissingMessage)
            {
                reversionAlertText.Text = fileSelectionMissingMessage;
                return;
            }
            filePath = filePathText.Text;
            AsyncLocalizeChecker localizeChecker_Background = new AsyncLocalizeChecker(filePath, false);
            localizeChecker_Background.StartLocalizeCheckerAsync();

            SetResultLogNumber(localizeChecker_Background.logTableInfos);
            MakeDataGrid(localizeChecker_Background.logTableInfos);
            if (localizeChecker_Background.isAlreadyCompleted)
            {
                reversionAlertText.Text = reversionFailedMessage;
                return;
            }
            InitializeAlertText();
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

        private void SetResultLogNumber(LogTableInfo[] logTableInfos)
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
