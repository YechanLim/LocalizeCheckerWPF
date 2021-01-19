using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LocalizeChecker
{
    class FileSave
    {
        public static bool SaveBackupFiles(List<string> filePaths)
        {
            try
            {
                foreach (string filePath in filePaths)
                {
                    string destinationFile = filePath + ".#localizer#";
                    if (File.Exists(destinationFile))
                    {
                        return false;
                    }

                    File.Copy(filePath, destinationFile, true);
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Backup파일을 저장하는 중 오류가 발생했습니다. 원인: " + e.Message);
                return false;
            }
            }

    }
}
