using System;
using System.Collections.Generic;
using System.IO;

namespace LocalizeChecker
{
    struct StretchingFailedFilesInfo
    {
        public int Index;
        public string ResultMessage;
        public string RemarkMessage;

        public StretchingFailedFilesInfo(int index, string resultMessage, string remarkMessage)
        {
            Index = index;
            ResultMessage = resultMessage;
            RemarkMessage = remarkMessage;
        }
    };

    class FileStretcher
    {
        LineChecker lineChecker = new LineChecker();
        LineStretcher lineStretcher = new LineStretcher();
        public static List<StretchingFailedFilesInfo> stretchFailedFilesInfos = new List<StretchingFailedFilesInfo>();
        public static int filePathsIndex = -1;
        bool isAlreadyStretchedFile = false;
        bool isFailedToInsertCharacters = false;

        public void StretchFiles(List<string> filePaths)
        {
            stretchFailedFilesInfos = new List<StretchingFailedFilesInfo>();ㄴ
            const string tempFile = "temp.txt";
            string line;

            foreach (string filePath in filePaths)
            {
                lineChecker.IsIncludedInDataNode = false;
                lineChecker.IsMultiLineValueNode = false;
                filePathsIndex++;
                EraseDuplicatedFile(tempFile);

                if (!File.Exists(filePath))
                {
                    stretchFailedFilesInfos.Add(new StretchingFailedFilesInfo(filePathsIndex, "파일이 존재하지 않습니다.", $"{filePath} 파일이 존재하지 않습니다."));
                    continue;
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(tempFile))
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (lineChecker.IsCommentLine(line))
                            {
                                WriteLineToFile(writer, line);
                            }
                            else if (lineChecker.IsToBeStretchedLine(line))
                            {
                                string stretchedLine = StretchLine(line);

                                if (stretchedLine == null)
                                {
                                    isFailedToInsertCharacters = true;
                                    break;
                                }

                                if (stretchedLine == "")
                                {
                                    continue;
                                }
                                WriteLineToFile(writer, stretchedLine);
                            }
                            else
                            {
                                if (lineChecker.IsEndOfFile(line))
                                {
                                    writer.Write(line);
                                    break;
                                }
                                WriteLineToFile(writer, line);
                            }

                            if (isAlreadyStretchedFile)
                            {
                                break;
                            }
                        }
                    }
                    //break;
                    if (!isAlreadyStretchedFile && !isFailedToInsertCharacters)
                    {
                        File.Copy(tempFile, filePath, true);
                    }
                    isAlreadyStretchedFile = false;
                    isFailedToInsertCharacters = false;
                }
                catch (Exception e)
                {
                    stretchFailedFilesInfos.Add(new StretchingFailedFilesInfo(filePathsIndex, "resx 파일을 읽는데 오류가 발생했습니다.", $"{e.Message}"));
                    Console.WriteLine($"resx 파일을 읽는데 오류가 발생했습니다. 원인: {e.Message}");
                }
            }
        }

        string lineToBeStretched = "";
        int ValueNodelineNum = 0;

        string StretchLine(string line)
        {
            if (lineChecker.IsMultiLineValueNode)
            {
                lineToBeStretched += line + '\n';
                ValueNodelineNum++;
                return "";
            }

            if (lineChecker.IsAlreadyStretchedFile(line))
            {
                isAlreadyStretchedFile = true;
                return "";
            }

            lineToBeStretched += line;
            string stretchedLine = lineStretcher.InsertCharactersToStretchLine(lineToBeStretched, ValueNodelineNum);
            lineToBeStretched = "";
            ValueNodelineNum = 0;
            return stretchedLine;
        }

        void EraseDuplicatedFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        void WriteLineToFile(StreamWriter writer, string line)
        {
            writer.WriteLine(line);
        }
    }
}

