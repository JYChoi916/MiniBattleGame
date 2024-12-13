using System.ComponentModel;
using System.Reflection;

namespace MiniBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataTables.LoadTables();

            // Main Menu 구현
            // 메인 화면 출력

            // 메인 메뉴 구성 : 1. 새로운 게임,  2. 게임 불러오기,  3. 게임 종료
            string[] menuStrings = { "1. 새로운게임", "2. 게임불러오기", "3. 게임종료" };
            SelectPrint(menuStrings);
        }

        static void SelectPrint(string[] strings)
        {
            Console.ResetColor();
            string selectStringLine = String.Format($"┃");
            for(int i = 0; i < strings.Length; ++i)
            {
                selectStringLine = String.Format($"{selectStringLine}   {strings[i]}");
            }
            selectStringLine = String.Format($"{selectStringLine}   ┃");
            int fullCharCount = GetFullCharCount(selectStringLine);
            string lineString = "┏";
            for(int i = 0; i < selectStringLine.Length + fullCharCount - 4; ++i)
                lineString += "━";
            lineString += "┓";

            //lineString = Half2Full(lineString);
            Console.WriteLine(lineString);
            Console.WriteLine(selectStringLine);

            lineString = "┗";
            for (int i = 0; i < selectStringLine.Length + fullCharCount - 4; ++i)
                lineString += "━";
            lineString += "┛";

            //lineString = Half2Full(lineString);
            Console.WriteLine(lineString);
        }

        private static int GetFullCharCount(string str)
        {
            int count = 0;
            for(int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                int byteCount = System.Text.Encoding.UTF8.GetByteCount(new char[] { c });
                if (byteCount > 1)
                    count++;
            }

            return count;
        }
    }
}
