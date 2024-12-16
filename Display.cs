using System;
using System.Xml.Linq;

public static class Display
{
    public static int SelectInput(string titleString, string[] menuStrings)
    {
        Console.ResetColor();
        string selectStringLine = String.Format("┃");
        for (int i = 0; i < menuStrings.Length; ++i)
        {
            selectStringLine = String.Format($"{selectStringLine}   {menuStrings[i]}");
        }
        selectStringLine = String.Format($"{selectStringLine}   ┃");
        int fullCharCount = Utility.GetFullCharCount(selectStringLine);
        Console.WriteLine(titleString);
        string lineString = "┏";
        int lineLength = selectStringLine.Length + fullCharCount - 4;
        for (int i = 0; i < lineLength; ++i)
            lineString += "━";
        lineString += "┓";

        Console.WriteLine(lineString);
        Console.WriteLine(selectStringLine);

        lineString = "┗";
        for (int i = 0; i < selectStringLine.Length + fullCharCount - 4; ++i)
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
                Console.WriteLine();
                break;
            }
        }

        return selectNumber;
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
}
