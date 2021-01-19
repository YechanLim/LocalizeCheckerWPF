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
        string RestorationFailedMessage = "이미 복원된 파일입니다.";
        public static string[] filePath;
        public static bool isAsycStretch = false;

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
                SetLogNumberText(0, 0);
                MakeDataGrid(new LogTableInfo[0]);
            }
        }

        private async void StretchFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionMissingMessage)
            {
                stretchingAlertText.Text = fileSelectionMissingMessage;
                return;
            }
            string[] filePath_temp = { filePathTextBlock.Text };
            filePath = filePath_temp;
            isAsycStretch = true;
            Progress progressPage = new Progress();
            OpenProgressPage(progressPage);

            if (progressPage.isAlreadyStretched)
            {
                SetLogNumberText(0, 0);
                stretchingAlertText.Text = StretchingFailedMessage;
                MakeDataGrid(new LogTableInfo[0]);
                return;
            }

            SetLogNumberText(Program.filePaths.Count, FileStretcher.stretchFailedFilesInfos.Count);
            InitializeAlertText();
            MakeDataGrid(Program.logTableInfos);
        }

        private void RestoreFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionMissingMessage)
            {
                restorationAlertText.Text = fileSelectionMissingMessage;
                return;
            }

            string[] filePath1 = { filePathTextBlock.Text };
            filePath = filePath1;
            isAsycStretch = false;
            Progress progressPage = new Progress();
            OpenProgressPage(progressPage);

            if (progressPage.isAlreadyRestored)
            {
                SetLogNumberText(0, 0);
                restorationAlertText.Text = RestorationFailedMessage;
                MakeDataGrid(new LogTableInfo[0]);
                return;
            }

            SetLogNumberText(Program.filePaths.Count, FileRestore.restorationFailedFilesInfos.Count);
            InitializeAlertText();
            MakeDataGrid(Program.logTableInfos);
        }

        private void OpenProgressPage(Progress progressPage)
        {
            progressPage.Owner = Window.GetWindow(this);
            progressPage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            progressPage.ShowDialog();
        }

        private void MakeDataGrid(LogTableInfo[] resultTableLogInfos)
        {
            dataGrid.ItemsSource = resultTableLogInfos;
            dataGrid.Columns[dataGrid.Columns.Count - 1].Header = "";
            dataGrid.Columns[dataGrid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void SetLogNumberText(int total, int failure)
        {
            fileNum.Text = total.ToString();
            successfulFileNum.Text = (total - failure).ToString();
            failedFileNum.Text = failure.ToString();
        }

        private void InitializeAlertText()
        {
            stretchingAlertText.Text = "";
            restorationAlertText.Text = "";
        }
    }
}
