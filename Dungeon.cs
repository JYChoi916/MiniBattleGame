using System;
using System.Net;
using System.Numerics;
using System.Xml.Linq;


// 입장한 던전 클래스
public class Dungeon
{
    Player player;
	DungeonData dungeonData;
	List<DungeonRoom> rooms;
	string name;
	int currentRoomNumber;
    DungeonRoom currentRoom;

    // 턴 계산을 하기 위한 카운터
    float playerTimeCounter;
    List<float> monstersTimeCounter;

    public class DungeonRoom
    {
        MonsterPartyData? partyData;
        List<Monster> monsters;
        public List<Monster> Monsters { get { return monsters; } }

        public DungeonRoom(MonsterPartyData partyData)
        {
            this.partyData = partyData;
            monsters = new List<Monster>();
        }

        public bool CheckClear()
        {
            return false;
        }

        public void Enter()
        {
            int monsterCount = Utility.GetRandom(1, partyData.maxCount+1);
            for (int i = 0; i < monsterCount; ++i)
            {
                int monsterIndex = Utility.GetRandom(0, partyData.monsterIDList.Count);
                var monsterID = partyData.monsterIDList[monsterIndex];
                MonsterData monsterData = DataTables.GetMonsterData(monsterID);
                Monster monster = new Monster(monsterData);
                monsters.Add(monster);
            }
        }
    }

    public Dungeon(Player player, DungeonData dungeonData)
	{
        // 전투를 위한 Player Instance
        this.player = player;

        // 테이블에서 받은 던전 데이터 
        this.dungeonData = dungeonData;
        this.name = dungeonData.name;
		rooms = new List<DungeonRoom>();
        monstersTimeCounter = new List<float>();
    }

    public void Enter()
	{
		Console.Clear();
		// 주사위를 굴려 던전 데이터의 최소/최대 사이 방의 수를 결정
		int roomCount = Utility.GetRandom(dungeonData.minRoomCount, dungeonData.maxRoomCount+1);

		// 모든 방에 대하여 생성할 MonsnterParty 결정
		for(int i = 0; i < roomCount; ++i)
		{
			var partyIDList = i < roomCount - 1 ? dungeonData.normalPartyIDList : dungeonData.bossPartyIDList;
            int partyIDIndex = Utility.GetRandom(0, partyIDList.Count);
			string partyID = partyIDList[partyIDIndex];

			MonsterPartyData partyData = DataTables.GetMonsterPartyData(partyID);
            DungeonRoom room = new DungeonRoom(partyData);
            rooms.Add(room);
        }

		currentRoomNumber = -1;
		EnterNextRoom();
    }

	public void EnterNextRoom()
	{
		currentRoomNumber++;
		if (currentRoomNumber < rooms.Count)
		{
            rooms[currentRoomNumber].Enter();
            currentRoom = rooms[currentRoomNumber];
            playerTimeCounter = 0;
            monstersTimeCounter.Clear();
            currentRoom.Monsters.ForEach(x => monstersTimeCounter.Add(0f));
}
		else
		{
			DungeonClear();
		}
    }

    public void Battle()
    {
        // 화면 클리어
        Console.Clear();

        // 플레이어 정보 표시
        Display.PlayerInfo(player, false);

        // 몬스터 파티 정보 표시
        Display.MonstersInfo(rooms[currentRoomNumber].Monsters);
        Console.WriteLine();

        // 턴 카운팅
        TurnCount();
    }

    void TurnCount ()
    {
        Console.WriteLine();
        int timeGageCount = 0;

        while (true)
        {
            // Player Time Gage
            playerTimeCounter += 1.0f + player.Status.agility * 0.1f;           // 민첩성 수치의 10%를 게이지 추가에 반영
            timeGageCount = (int)(playerTimeCounter / 20.0f);
            Display.PrintTimeGage(timeGageCount, 20, player.Name, ConsoleColor.Green);

            // Monsters Time Gage
            for(int i = 0; i < currentRoom.Monsters.Count; ++i)
            {
                // 몬스터가 죽지 않았을때
                Monster monster = currentRoom.Monsters[i];
                if (currentRoom.Monsters[i].IsDead == false)
                {
                    monstersTimeCounter[i] += 1.0f + monster.Status.agility * 0.1f;     // 민첩성 수치의 10%를 게이지 추가에 반영
                    int tg = (int)(monstersTimeCounter[i] / 20.0f);
                    Display.PrintTimeGage(tg, 20, monster.Data.name, ConsoleColor.Red);
                }
                // 이미 죽었을때에는 턴 카운팅 할 필요X
                else
                {
                    Console.Write($"{monster.Data.name} : ☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠");
                    monstersTimeCounter[i] = 0;
                }
            }

            Thread.Sleep(10);

            // 플레이어 턴
            if (timeGageCount >= 20)                        // 타임 게이지가 20이 되면 턴을 받음
            {
                player.ActiveTurn(currentRoom.Monsters);
                playerTimeCounter = 0;
                break;
            }

            // 몬스터의 턴
            bool monsterHasTurn = false;
            for (int i = 0; i < monstersTimeCounter.Count; ++i)
            {
                int tg = (int)(monstersTimeCounter[i] / 20.0f); 
                if (tg >= 20)
                {
                    currentRoom.Monsters[i].ActiveTurn(player);
                    monstersTimeCounter[i] = 0;
                    monsterHasTurn = true;
                }
            }
            
            if (monsterHasTurn)
                break;

            Console.SetCursorPosition(0, Console.CursorTop - (1 + currentRoom.Monsters.Count));
        }
    }

    void DungeonClear()
	{

	}

	//public Reward GetReward()
	//{

	//}
}
