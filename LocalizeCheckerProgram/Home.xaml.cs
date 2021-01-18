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
        string fileSelectionAlertMessage = "선택된 파일이 없습니다.";
        string StretchingFailedMessage = "이미 변환된 파일입니다.";
        string RestorationFailedMessage = "이미 복원된 파일입니다.";
        public Home()
        {
            InitializeComponent();
            filePathTextBlock.Text = fileSelectionAlertMessage;
        }

        private void SelectFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Solution Files (*.sln)|*.sln";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePathTextBlock.Text = openFileDialog.FileName;
                filePathTextBlock.ToolTip = openFileDialog.FileName;
                stretchingAlertText.Text = "";
                restorationAlertText.Text = "";
                MakeDataGrid(new LogTableInfo[0]);
            }
        }

        private void MakeDataGrid(LogTableInfo[] resultTableLogInfos)
        {
            dataGrid.ItemsSource = resultTableLogInfos;
            int count = dataGrid.Columns.Count;
            dataGrid.Columns[count -1].Header = "";
            dataGrid.Columns[count -1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void StretchFiles_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionAlertMessage)
            {
                stretchingAlertText.Text = fileSelectionAlertMessage;
                return;
            }

            string[] filePath = { filePathTextBlock.Text };
            if (Program.Stretch(filePath) == false)
            {
                stretchingAlertText.Text = StretchingFailedMessage;
                return;
            }
            //var progressPage = new Progress();
            //progressPage.ShowDialog();

            ////this.NavigationService.Navigate(new Uri("Progress.xaml", UriKind.Relative));
            restorationAlertText.Text = "";
            stretchingAlertText.Text = "";
            MakeDataGrid(Program.logTableInfos);
        }

        private void RestoreFiles_Click(object sender, RoutedEventArgs e)
        {
            if (filePathTextBlock.Text == fileSelectionAlertMessage)
            {
                restorationAlertText.Text = fileSelectionAlertMessage;
                return;
            }

            string[] filePath = { filePathTextBlock.Text };
            if (Program.Restore(filePath) == false)
            {
                restorationAlertText.Text = RestorationFailedMessage;
                return;
            }

            restorationAlertText.Text = "";
            stretchingAlertText.Text = "";
            MakeDataGrid(Program.logTableInfos);
        }
    }
}
