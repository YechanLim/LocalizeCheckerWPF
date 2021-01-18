using System;
using System.Collections.Generic;
using System.IO;

namespace LocalizeChecker
{   
    class LineChecker
    {
        bool isMultiLineComment = false;
        public bool IsIncludedInDataNode = false;
        public bool IsMultiLineValueNode = false;
        public const string StartTagOfValueNode = "<value>";
        public const string EndTagOfValueNode = "</value>";

        public bool IsCommentLine(string line)
        {
            const string SingleLineComment = "//";
            const string beginningOfMultiLineComment = "<!--";
            const string endOfMultiLineComment = "-->";
            const char space = ' ';
                            
            if (line.Contains(beginningOfMultiLineComment))
            {
                isMultiLineComment = true;
                return true;
            }

            if (isMultiLineComment)
            {
                if (line.Contains(endOfMultiLineComment))
                {
                    isMultiLineComment = false;
                }
                return true;
            }

            if (line.Trim(space).StartsWith(SingleLineComment))
            {
                return true;
            }
            return false;
        }

        public bool IsToBeStretchedLine(string line)
        {
            if (IsContainingStartTagOfDataNode(line) && !IsContainingTypeAttribute(line))
            {
                IsIncludedInDataNode = true;
                return false;
            }
            else if (IsContainingEndTagOfDataNode(line))
            {
                IsIncludedInDataNode = false;
                return false;
            }

            if (!IsIncludedInDataNode)
            {
                return false;
            }

            if (IsMultiLineValueNode)
            {
                if (IsContainingEndTagOfValueNode(line))
                {
                    IsMultiLineValueNode = false;
                }
                return true;
            }

            if (IsContainingStartTagOfValueNode(line) && !IsContainingEndTagOfValueNode(line))
            {
                IsMultiLineValueNode = true;
                return true;
            }
            else if (IsContainingStartTagOfValueNode(line) && IsContainingEndTagOfValueNode(line))
            {
                return true;
            }

            return false;
        }

        bool IsContainingTypeAttribute(string line)
        {
            string typeAttribute = "type=";
            return line.Contains(typeAttribute);
        }

        bool IsContainingStartTagOfDataNode(string line)
        {
            const string startTagOfDataNode = "<data";
            return line.Contains(startTagOfDataNode);
        }

        bool IsContainingEndTagOfDataNode(string line)
        {
            const string endTagOfDataNode = "</data>";
            return line.Contains(endTagOfDataNode);
        }

        bool IsContainingStartTagOfValueNode(string line)
        {
            return line.Contains(StartTagOfValueNode);
        }

        bool IsContainingEndTagOfValueNode(string line)
        {
            return line.Contains(EndTagOfValueNode);
        }

        public bool IsAlreadyStretchedFile(string line)
        {
            return (line[line.IndexOf(StartTagOfValueNode) + StartTagOfValueNode.Length] == CharacterCollection.PrefixOfStretchingLine) && (line[line.LastIndexOf(EndTagOfValueNode) - 1]) == CharacterCollection.PostfixOfStretchingLine;
        }

        public bool IsContainingPredefinedEntity(string line)
        {
            return line.Contains(CharacterCollection.PrefixOfPredefinedEntity.ToString()) && line.Contains(CharacterCollection.PostfixOfPredefinedEntity.ToString());
        }

        public bool IsEndOfFile(string line)
        {
            const string endTagOfRootElement = "</root>";

            return line.Contains(endTagOfRootElement);
        }
    }
}
