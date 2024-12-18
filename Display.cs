using System;
using System.Numerics;
using System.Xml.Linq;

public static class Display
{
    public static int SelectInput(string titleString, string[] menuStrings, bool hasZero = false)
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

        Console.WriteLine($"{lineString, 10}");

        int selectNumber = -1;
        ConsoleKeyInfo consoleKeyInfo;
        while (true)
        {
            Console.Write("선택하세요 : ");
            consoleKeyInfo = Console.ReadKey();

            if (consoleKeyInfo.KeyChar < 49 + (hasZero ? -1 : 0) || consoleKeyInfo.KeyChar > 57)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("!!!!잘못 입력했습니다!!!!");
                Console.ResetColor();
            }
            else
            {
                selectNumber = consoleKeyInfo.KeyChar - '0';
                if (selectNumber > menuStrings.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("!!선택 할수 없는 번호를 입력하셨습니다!!");
                    Console.ResetColor();
                    continue;
                }

                Console.WriteLine();
                break;
            }
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
        int select = SelectInput("", selectString);
        return select;
    }

    public static void PlayerInfo(Player player, bool isDetailInfo)
    {
        string[] playerInfos = player.GetPlayerInfoString();
        List<string> infos = new List<string>();
        string basicInfo = "";
        if (isDetailInfo == false)
        {
            basicInfo = $"  {playerInfos[0]} | {playerInfos[1]} | {playerInfos[2]} | {playerInfos[3]} | {playerInfos[4]} | {playerInfos[5]}  ";
            infos.Add(basicInfo);
        }
        else
        {
            // 여기는 다시 만들자
            infos.Add($"  {playerInfos[0]} | {playerInfos[1]} | {playerInfos[2]} | {playerInfos[3]} | {playerInfos[4]} | {playerInfos[5]}  ");
            infos.Add($"  {playerInfos[6]} | {playerInfos[7]} | {playerInfos[8]} | {playerInfos[9]} | {playerInfos[10]} | {playerInfos[11]}  ");
            infos.Add($"  {player.Gold}  ");
        }

        int longestLength = infos.Max(s => s.Length);
        string longestString = infos.FirstOrDefault(s => s.Length == longestLength);

        int fullCharCount = Utility.GetFullCharCount(longestString);
        for (int i = 0; i < infos.Count; ++i)
        {
            var info = infos[i];
            if (info.Length < longestLength)
            {
                int fc = Utility.GetFullCharCount(info);
                int fcDiff = fullCharCount - fc;
                if (fcDiff == 0 && longestLength - info.Length <= 2)
                    fcDiff += 2;
                else
                    fcDiff += fcDiff % 2 == 1 ? 1 : 0;
                for (int j = 0; j < longestLength - info.Length + fcDiff + 1; ++j)
                {
                    info += " ";
                }
                infos[i] = info;
            }
        }

        string lineString = "┏━━";
        for (int i = 0; i < longestLength + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "━━┓";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(lineString);
        for(int i = 0; i < infos.Count; ++i)
        {
            Console.Write("┃");
            Console.ResetColor();
            Console.Write(infos[i]);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┃");
        }
        lineString = "┗━━";
        for (int i = 0; i < longestLength + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "━━┛";
        Console.WriteLine(lineString);
        Console.ResetColor();
    }
    public static void MonstersInfo(List<Monster> monsters)
    {
        List<string>monstersInfo = new List<string>();

        Console.WriteLine();

        for(int i = 0; i < monsters.Count; ++i)
        {
            string[] infos = monsters[i].GetMonsterInfos();
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
