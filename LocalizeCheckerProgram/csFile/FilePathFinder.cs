using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LocalizeChecker
{
    static class FilePathFinder
    {
        public static List<string> GetCSprojFilePathList(string filePath)
        {
            List<string> filePaths = new List<string>();
            const string csprojFileExtension = ".csproj";
            string basePath = Path.GetDirectoryName(filePath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} 파일을 찾을 수 없습니다.");
                return filePaths;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (IsCSprojFileExtentionIncludedLine(line, csprojFileExtension) && ParseCSprojFileRelativePath(line, csprojFileExtension) != null)
                        {
                                filePaths.Add(Path.Combine(basePath, ParseCSprojFileRelativePath(line, csprojFileExtension)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"sln 파일을 읽는데 오류가 발생했습니다. 원인: {e.Message} ");
            }
            return filePaths;
        }

        static bool IsCSprojFileExtentionIncludedLine(string line, string csprojFileExtension)
        {
            return line.Contains(csprojFileExtension);
        }

        static string ParseCSprojFileRelativePath(string line, string csprojFileExtension)
        {
            int stringIndex = line.IndexOf(csprojFileExtension);
            while (line[--stringIndex] != '\"')
            {
                if(stringIndex <= -1)
                {
                    break; 
                }
            }

            stringIndex++;
            int firIndex = stringIndex;
            int endIndex = line.IndexOf(csprojFileExtension) + csprojFileExtension.Length - 1;
           
            try
            {
                return line.Substring(firIndex, endIndex - firIndex + 1);
            }
            catch(Exception e)
            {
                Console.WriteLine($"csproj 상대 경로를 얻는데 실패했습니다. 원인: {e.Message}");
                return null;
            }
        }

        public static List<string> GetResxFilePathList(List<string> csprojFilePaths)
        {
            List<string> filePaths = new List<string>();
            const string embeddedResourceTagName = "EmbeddedResource";
            const string generatorTagName = "Generator";
            const string generatorNodeInnerText = "ResXFileCodeGenerator";
            
            foreach (string csprojFilePath in csprojFilePaths)
            {
                if (!File.Exists(csprojFilePath))
                {
                    Console.WriteLine($"csproj 파일을 읽을 수 없습니다. filePath: {csprojFilePath}");
                    continue;
                }

                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(csprojFilePath);
                    XmlNodeList embeddedResourceNodeList = doc.GetElementsByTagName(embeddedResourceTagName);

                    for (int i = 0; i < embeddedResourceNodeList.Count; i++)
                    {
                        XmlNodeList childNodesOfEmbeddedResourceNode = embeddedResourceNodeList[i].ChildNodes;
                        for (int j = 0; j < childNodesOfEmbeddedResourceNode.Count; j++)
                        {
                            if (IsValidGeneratorNode(childNodesOfEmbeddedResourceNode[j].Name, childNodesOfEmbeddedResourceNode[j].InnerText, generatorTagName, generatorNodeInnerText))
                            {
                                filePaths.Add(Path.Combine(Path.GetDirectoryName(csprojFilePath), embeddedResourceNodeList[i].Attributes["Include"].Value));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"csproj 파일을 읽는데 오류가 발생했습니다. 원인: {e.Message} 경로: {csprojFilePath}");
                }
            }
            return filePaths;
        }

        static bool IsValidGeneratorNode(string TagName, string innerText, string generatorTagName, string generatorNodeInnerText)
        {
            return TagName == generatorTagName && innerText.Contains(generatorNodeInnerText);
        }

    }
}
