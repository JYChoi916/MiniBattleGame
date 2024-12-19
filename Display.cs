using System;
using System.Numerics;
using System.Xml.Linq;
using ConsoleTables;

public static class Display
{
    public static int SelectInput(string titleString, string[] menuStrings, int maxNumber, bool hasZero = false, bool isLineInput = false, string inputString = "선택하세요")
    {
        Console.ResetColor();
        List<string> selectStringLines = new List<string>();
        for (int i = 0; i < menuStrings.Length; ++i)
        {
            string line = String.Format($"┃   {menuStrings[i]}");
            line = String.Format($"{line}   ┃");
            selectStringLines.Add(line);
        }
        int fullCharCount = Utility.GetFullCharCount(selectStringLines[0]);
        Console.WriteLine(titleString);
        string lineString = "┏";
        int lineLength = selectStringLines[0].Length + fullCharCount - 4;
        for (int i = 0; i < lineLength; ++i)
            lineString += "━";
        lineString += "┓";

        Console.WriteLine(lineString);
        for (int i = 0; i < selectStringLines.Count; ++i)
        {
            Console.WriteLine(selectStringLines[i]);
        }

        lineString = "┗";
        for (int i = 0; i < selectStringLines[0].Length + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "┛";

        Console.WriteLine(lineString);

        int selectNumber = -1;
        ConsoleKeyInfo consoleKeyInfo;
        string inputLine = "";
        while (true)
        {
            Console.Write($"{inputString} : ");

            if (isLineInput == false)
            { 
                consoleKeyInfo = Console.ReadKey();

                if (consoleKeyInfo.KeyChar < 49 + (hasZero ? -1 : 0) || consoleKeyInfo.KeyChar > 57)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("!!!!잘못 입력했습니다!!!!");
                    Console.ResetColor();
                    continue;
                }
                else
                {
                    selectNumber = consoleKeyInfo.KeyChar - '0';
                    if (selectNumber > maxNumber)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine();
                        Console.WriteLine("!!선택 할수 없는 번호를 입력하셨습니다!!");
                        Console.ResetColor();
                        continue;
                    }
                }
            }
            else
            {
                inputLine = Console.ReadLine();
                if(int.TryParse(inputLine, out selectNumber))
                {
                    if (selectNumber > maxNumber)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine();
                        Console.WriteLine("!!선택 할수 없는 번호를 입력하셨습니다!!");
                        Console.ResetColor();
                        continue;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("!!!!잘못 입력했습니다!!!!");
                    Console.ResetColor();
                    continue;
                }
            }

            Console.WriteLine();
            break;
        }

        return selectNumber;
    }

    public static int SelectYesOrNo()
    {
        string[] selectString =
{
            "1.   예  ",
            "2. 아니오"
            };
        int select = SelectInput("", selectString, 2);
        return select;
    }

    public static void PlayerInfo(Player player, bool isDetailInfo)
    {
        string[] playerInfos = player.GetPlayerInfoString();

        ConsoleTable consoleTable = new ConsoleTable(playerInfos[0], playerInfos[1]);
        consoleTable.Rows.Clear();
        consoleTable.AddRow(playerInfos[2], playerInfos[3]);
        consoleTable.AddRow("", "");
        consoleTable.AddRow(playerInfos[4], playerInfos[5]);
        consoleTable.AddRow("", "");
        consoleTable.AddRow(playerInfos[6], playerInfos[9]);
        consoleTable.AddRow(playerInfos[7], playerInfos[10]);
        consoleTable.AddRow(playerInfos[8], playerInfos[11]);
        consoleTable.AddRow("", "");
        consoleTable.AddRow(playerInfos[12], playerInfos[13]);

        consoleTable.Write(Format.MarkDown);
        Console.WriteLine();

        ////Utility.MakeSameLengthStrings(playerInfos.ToList());
        //List<string> infos = new List<string>();
        //string basicInfo = "";
        //if (isDetailInfo == false)
        //{
        //    basicInfo = $"  {playerInfos[0],15}|{playerInfos[1],15}|{playerInfos[2],15}|{playerInfos[3],15}|{playerInfos[4],15}|{playerInfos[5],15}  ";
        //    infos.Add(basicInfo);
        //}
        //else
        //{
        //    // 여기는 다시 만들자
        //    infos.Add($"  {playerInfos[0],15}|{playerInfos[1],15}|{playerInfos[2],15}|{playerInfos[3],15}|{playerInfos[4],15}|{playerInfos[5],15}  ");
        //    infos.Add($"  {playerInfos[6],15}|{playerInfos[7],15}|{playerInfos[8],15}|{playerInfos[9],15}|{playerInfos[10],15}|{playerInfos[11],15}  ");
        //    infos.Add($"  {playerInfos[12],15}|{playerInfos[13],15}");
        //}

        ////Utility.MakeSameLengthStrings(infos);

        //int longestLength = infos.Max(s => s.Length);
        //string longestString = infos.FirstOrDefault(s => s.Length == longestLength);

        //int fullCharCount = Utility.GetFullCharCount(longestString);
        //for (int i = 0; i < infos.Count; ++i)
        //{
        //    var info = infos[i];
        //    if (info.Length < longestLength)
        //    {
        //        int fc = Utility.GetFullCharCount(info);
        //        int fcDiff = fullCharCount - fc;
        //        if (fcDiff == 0 && longestLength - info.Length <= 2)
        //            fcDiff += 2;
        //        else
        //            fcDiff += fcDiff % 2 == 1 ? 1 : 0;
        //        for (int j = 0; j < longestLength - info.Length + fcDiff + 1; ++j)
        //        {
        //            info += " ";
        //        }
        //        infos[i] = info;
        //    }
        //}

        //string lineString = "┏━━";
        //for (int i = 0; i < longestLength + fullCharCount - 4; ++i)
        //    lineString += "━";
        //lineString += "━━┓";
        //Console.ForegroundColor = ConsoleColor.Green;
        //Console.WriteLine(lineString);
        //for(int i = 0; i < infos.Count; ++i)
        //{
        //    Console.Write("┃");
        //    Console.ResetColor();
        //    Console.Write(infos[i]);
        //    Console.ForegroundColor = ConsoleColor.Green;
        //    Console.WriteLine("┃");
        //}
        //lineString = "┗━━";
        //for (int i = 0; i < longestLength + fullCharCount - 4; ++i)
        //    lineString += "━";
        //lineString += "━━┛";
        //Console.WriteLine(lineString);
        //Console.ResetColor();
    }
    public static void MonstersInfo(List<Character> monsters)
    {
        List<string>monstersInfo = new List<string>();

        Console.WriteLine();

        for(int i = 0; i < monsters.Count; ++i)
        {
            Monster m = monsters[i] as Monster;
            string[] infos = m.GetMonsterInfos();
            monstersInfo.Add($"{Utility.Half2Full($"   몬스터{i + 1}")} : {infos[0]}    {infos[1]}     ");
        }

        Utility.MakeSameLengthStrings(monstersInfo);
        int longestLength = monstersInfo.Max(s => s.Length);
        string longestString = monstersInfo.FirstOrDefault(s => s.Length == longestLength);
        int fullCharCount = Utility.GetFullCharCount(longestString);

        //for(int i = 0; i < monstersInfo.Count; ++i)
        //{
        //    var info = monstersInfo[i];
        //    if (info.Length < longestLength)
        //    {
        //        int fc = Utility.GetFullCharCount(info);
        //        int fcDiff = fullCharCount - fc;
        //        if (fcDiff == 0 && longestLength - info.Length != 1)
        //            fcDiff += 2;
        //        else
        //            fcDiff += fcDiff % 2 == 1 ? 1 : 0;
        //        for (int j = 0; j < longestLength - info.Length + fcDiff + 1; ++j)
        //        {
        //            info += " ";
        //        }
        //        monstersInfo[i] = info;
        //    }
        //}

        string lineString = "┏━━";
        for (int i = 0; i < longestLength + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "━━┓";

        string space = "";
        for (int i = 0; i < longestLength / 2 + 2; ++i) space += " ";

        string titleString = "몬스터 정보";
        titleString = titleString.Insert(0, space);
        titleString = titleString.Insert(titleString.Length, space);

        Console.WriteLine(titleString);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(lineString);
        for (int i = 0; i < monstersInfo.Count; ++i)
        {
            Console.Write("┃");
            Console.ResetColor();
            Console.Write(monstersInfo[i]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("┃");
        }
        lineString = "┗━━";
        for (int i = 0; i < monstersInfo[0].Length + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "━━┛";
        Console.WriteLine(lineString);
        Console.ResetColor();
    }

    public static void RepeatLastLine(string line)
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.WriteLine(line);
    }

    public static void PrintTimeGage(int currentGage, int maxGage, string characterName, ConsoleColor gageColor)
    {
        string name = characterName;
        int fullChCount = Utility.GetFullCharCount(characterName);
        name = name.PadLeft(10 - fullChCount);
        Console.ResetColor();
        Console.Write($"{name} : [");
        for (int i = 0; i < maxGage; ++i)
        {
            Console.BackgroundColor = i < currentGage ? gageColor : ConsoleColor.Gray;
            Console.Write(" ");
        }
        Console.ResetColor();
        Console.WriteLine("]");
    }
}
