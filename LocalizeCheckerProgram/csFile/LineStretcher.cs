using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LocalizeChecker
{
    class LineStretcher
    {
        LineChecker lineChecker = new LineChecker();

        public string InsertCharactersToStretchLine(string line, int lineNum, int index, List<StretchingFailedFilesInfo> stretchingFailedFilesInfos)
        {
            try
            {
                int firstIndexOfValueNodeInnerText = line.IndexOf(LineChecker.StartTagOfValueNode) + LineChecker.StartTagOfValueNode.Length;
                int lastIndexOfValueNodeInnerText = line.LastIndexOf(LineChecker.EndTagOfValueNode);
                int predefinedEntityLength = 0;
                int minimumNumOfCharactersToInsert = 2;
                bool isContainingPredefinedEntity = false;

                if (lineChecker.IsContainingPredefinedEntity(line))
                {
                    isContainingPredefinedEntity = true;
                    predefinedEntityLength = GetPredefinedEntityLength(line, firstIndexOfValueNodeInnerText, lastIndexOfValueNodeInnerText);
                }

                int characterInsertionIndex = firstIndexOfValueNodeInnerText;
                int insertedCharacterNumber = 0;
                double stretchRatio = 0.5;

                int numberOfCharacterToInsert = (int)((lastIndexOfValueNodeInnerText - firstIndexOfValueNodeInnerText - predefinedEntityLength) * stretchRatio) - lineNum;
                if (numberOfCharacterToInsert < minimumNumOfCharactersToInsert)
                {
                    numberOfCharacterToInsert = minimumNumOfCharactersToInsert;
                }

                while (numberOfCharacterToInsert > insertedCharacterNumber)
                {
                    SkipEndOfLineCharacter(line, ref characterInsertionIndex);
                    SkipPredefinedEntity(ref line, ref characterInsertionIndex, ref insertedCharacterNumber);

                    if (insertedCharacterNumber == 0)
                    {
                        line = InsertCharacter(line, CharacterCollection.PrefixOfStretchingLine, ref characterInsertionIndex, ref insertedCharacterNumber);
                    }
                    else if (insertedCharacterNumber == numberOfCharacterToInsert - 1)
                    {
                        lastIndexOfValueNodeInnerText = line.LastIndexOf(LineChecker.EndTagOfValueNode);
                        line = InsertCharacter(line, CharacterCollection.PostfixOfStretchingLine, ref lastIndexOfValueNodeInnerText, ref insertedCharacterNumber);
                    }
                    else
                    {
                        line = InsertCharacter(line, CharacterCollection.InfixOfStretchingLine, ref characterInsertionIndex, ref insertedCharacterNumber);
                    }
                }
                return line;
            }
            catch (Exception e)
            {
                stretchingFailedFilesInfos.Add(new StretchingFailedFilesInfo(index, "Line에 character를 추가하는데 오류가 발생했습니다.", $"{e.Message}"));
                Console.WriteLine($"Line에 character를 추가하는데 오류가 발생했습니다. 원인: {e.Message}");
                return null;
            }
        }

        string InsertCharacter(string line, char character, ref int index, ref int insertedNum)
        {
            string stretchedLine = line.Insert(index, character.ToString());
            index += 2;
            insertedNum++;
            return stretchedLine;
        }

        void SkipEndOfLineCharacter(string line, ref int index)
        {
            if (IsEndOfLineCharacter(line[index]))
            {
                index++;

                while (IsEndOfLineCharacter(line[index]))
                {
                    index++;
                }
            }
        }

        void SkipPredefinedEntity(ref string line, ref int index, ref int insertedCharacterNum)
        {
            if (IsPrefixOfPredefinedEntity(line[index]))
            {
                line = line.Insert(index, CharacterCollection.InfixOfStretchingLine.ToString());
                insertedCharacterNum++;

                while (!IsPostfixOfPredefinedEntity(line[index]))
                {
                    index++;
                }
                index++;
            }
        }

        bool IsEndOfLineCharacter(char character)
        {
            return character == CharacterCollection.EndOfLineCharacter;
        }

        bool IsPrefixOfPredefinedEntity(char character)
        {
            return character == CharacterCollection.PrefixOfPredefinedEntity;
        }

        bool IsPostfixOfPredefinedEntity(char character)
        {
            return character == CharacterCollection.PostfixOfPredefinedEntity;
        }

        int GetPredefinedEntityLength(string line, int firstIndex, int lastIndex)
        {
            int index = firstIndex;
            int predefinedEntityLength = 0;

            while (index != lastIndex)
            {
                if (IsPrefixOfPredefinedEntity(line[index]))
                {
                    while (line[index] != CharacterCollection.PostfixOfPredefinedEntity)
                    {
                        index++;
                        predefinedEntityLength++;
                    }
                }
                index++;
            }
            return predefinedEntityLength;
        }
    }
}
