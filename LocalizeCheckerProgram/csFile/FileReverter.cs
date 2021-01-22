using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using LocalizeCheckerProgram;
using System.Threading;

namespace LocalizeChecker
{
    struct ReversionFailedFilesInfo
    {
        public int Index;
        public string ResultMessage;
        public string RemarkMessage;

        public ReversionFailedFilesInfo(int index, string resultMessage, string remarkMessage)
        {
            Index = index;
            ResultMessage = resultMessage;
            RemarkMessage = remarkMessage;
        }
    };

    class FileReverter
    {
        public List<ReversionFailedFilesInfo> reversionFailedFilesInfos = new List<ReversionFailedFilesInfo>();
        public int index;
        public bool RevertFiles(List<string> filePaths, BackgroundWorkerParameter backgroundWorkerParameter, bool isResultRevertingMode)
        {
            reversionFailedFilesInfos = new List<ReversionFailedFilesInfo>();
            index = -1;
            try
            {
                foreach (string filePath in filePaths)
                {
                    index++;
                    string sourceFile = filePath + CharacterCollection.LocalizerBackupFileExtention;

                    if (backgroundWorkerParameter.Sender != null)
                    {
                        SendDataToBackgroundWorker(filePaths, filePath, backgroundWorkerParameter, isResultRevertingMode);

                        if (backgroundWorkerParameter.IsAsynStretch || isResultRevertingMode)
                        {
                        }
                        else if (backgroundWorkerParameter.Worker.CancellationPending)
                        {
                            return false;
                        }
                    }

                    if (!File.Exists(sourceFile))
                    {
                        //Console.WriteLine("[restore - failed]" + filePath);
                        reversionFailedFilesInfos.Add(new ReversionFailedFilesInfo(index, "파일을 복원하는데 실패했습니다.", "원본파일이 존재하지 않아 복원하지 못했습니다."));
                        continue;
                    }
                    SwitchFile(sourceFile, filePath);
                    //Console.WriteLine("[restore - succeed]" + filePath);
                }

                if (isResultRevertingMode)
                {
                    return true;
                }

                foreach (string filePath in filePaths)
                {
                    string sourceFile = filePath + CharacterCollection.LocalizerBackupFileExtention;
                    File.Delete(sourceFile);
                }
            }
            catch (Exception e)
            {
                reversionFailedFilesInfos.Add(new ReversionFailedFilesInfo(index, "파일을 복원하는데 실패했습니다.", e.Message));
            }

            return true;
        }

        private void SwitchFile(string a, string b)
        {
            string temp = "temp.txt";
            File.Copy(a, temp, true);
            File.Copy(b, a, true);
            File.Copy(temp, b, true);
            File.Delete(temp);
        }

        private void SendDataToBackgroundWorker(List<string> filePaths, string filePath, BackgroundWorkerParameter backgroundWorkerParameter, bool isRevertingMode)
        {
            Thread.Sleep(70);
            if (backgroundWorkerParameter.Worker.CancellationPending && !backgroundWorkerParameter.IsAsynStretch && !isRevertingMode)
            {
                RevertFiles(filePaths.GetRange(0, index), backgroundWorkerParameter, true);
                backgroundWorkerParameter.Args.Cancel = true;
                return;
            }
            int percentProgress = (int)((index * 100) / (filePaths.Count - 1));
            (backgroundWorkerParameter.Sender as BackgroundWorker).ReportProgress(percentProgress, filePath);

        }
    }
}
