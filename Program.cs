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
            int selectedMenu = Display.SelectInput("", menuStrings);

            if (selectedMenu == 3)
                return;

            GameManager gameManager = new GameManager((GameStartType)selectedMenu);

            while(gameManager.Play())
            {
            }

            Console.WriteLine("게임을 종료 합니다.");
        }
    }
}
