using System.Reflection;

namespace MiniBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataTables.LoadTables();

            MonsterData monsterData = DataTables.GetMonsterData("mon_01_08");
        }
    }
}
