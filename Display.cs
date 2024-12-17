using System;
using System.Numerics;
using System.Xml.Linq;

public static class Display
{
    public static int SelectInput(string titleString, string[] menuStrings)
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
        while (true)
        {
            Console.Write("선택하세요 : ");
            consoleKeyInfo = Console.ReadKey();

            if (consoleKeyInfo.KeyChar < 49 || consoleKeyInfo.KeyChar > 57)
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
        string basicInfo = $"   {playerInfos[0]}   {playerInfos[1]}   {playerInfos[2]}   ";
        infos.Add(basicInfo);
        if (isDetailInfo)
        {
            infos.Add($"   {playerInfos[4]}   {playerInfos[5]}   {playerInfos[6]}   ");
            infos.Add($"   {playerInfos[7]}   {playerInfos[8]}   {playerInfos[8]}   ");
        }

        int fullCharCount = Utility.GetFullCharCount(basicInfo);

        string lineString = "┏━━";
        for (int i = 0; i < basicInfo.Length + fullCharCount - 4; ++i)
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
        for (int i = 0; i < basicInfo.Length + fullCharCount - 4; ++i)
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

        int fullCharCount = Utility.GetFullCharCount(monstersInfo[0]);
        string lineString = "┏━━";
        for (int i = 0; i < monstersInfo[0].Length + fullCharCount - 4; ++i)
            lineString += "━";
        lineString += "━━┓";

        string space = "";
        for (int i = 0; i < monstersInfo[0].Length / 2 + 2; ++i) space += " ";

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
        //for (int i = characterName.Length; i < 10 - (characterName.Length + fullChCount * 2); ++i)
        //    name += ' ';
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
