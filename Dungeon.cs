using System;
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
            int monsterCount = Utility.GetRandom(1, partyData.maxCount);
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
    }

	void DungeonClear()
	{

	}

	//public Reward GetReward()
	//{

	//}
}
