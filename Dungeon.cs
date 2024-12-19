using System;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


// 입장한 던전 클래스
public class Dungeon
{
    Player player;
	DungeonData dungeonData;
    public string DungeonID { get { return dungeonData.dungeonID; } }
	List<DungeonRoom> rooms;
	string name;
	int currentRoomNumber;
    DungeonRoom currentRoom;
    public List<Character> GetCurrentMonsters()
    {
        return currentRoom.Monsters;
    }
    bool isCleared;
    public bool IsCleared { get { return isCleared; } }



    // 턴 계산을 하기 위한 카운터
    float playerTimeCounter;
    List<float> monstersTimeCounter;

    public class DungeonRoom
    {
        List<MonsterPartyData> partyDataList;
        List<Character> monsters;
        Dictionary<string, Monster> monsterDic;
        public List<Character> Monsters { get { return monsters; } }
        public DungeonRoom(List<MonsterPartyData> partyDataList)
        {
            this.partyDataList = partyDataList;
            monsters = new List<Character>();
            monsterDic = new Dictionary<string, Monster>();
        }

        public bool CheckClear()
        {
            foreach (Monster monster in monsters)
            {
                if (monster.IsDead == false)
                    return false;
            }

            Console.WriteLine();
            Console.WriteLine("방의 모든 몬스터를 처치했습니다.");
            return true;
        }

        public void Enter()
        {
            int count = 0;
            foreach (var partyData in partyDataList)
            {
                int monsterCount = Utility.GetRandom(1, partyData.maxCount + 1);
                for (int i = 0; i < monsterCount; ++i)
                {
                    int monsterIndex = Utility.GetRandom(0, partyData.monsterIDList.Count);
                    var monsterID = partyData.monsterIDList[monsterIndex];
                    MonsterData monsterData = DataTables.GetMonsterData(monsterID);
                    Monster monster = new Monster(monsterData);
                    monsters.Add(monster);
                    monster.monsterUniqueName = (char)(65 + count) + monsterData.name;
                    count++;
                }
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
        isCleared = false;
		Console.Clear();
		// 주사위를 굴려 던전 데이터의 최소/최대 사이 방의 수를 결정
		int roomCount = Utility.GetRandom(dungeonData.minRoomCount, dungeonData.maxRoomCount+1);

		// 모든 방에 대하여 생성할 MonsnterParty 결정
		for(int i = 0; i < roomCount; ++i)
		{
            List<string> partyIDList;
            List<MonsterPartyData> partyDataList = new List<MonsterPartyData>();
            if (i < roomCount - 1)
            {
                partyIDList = dungeonData.normalPartyIDList;
                int partyIDIndex = Utility.GetRandom(0, partyIDList.Count);
                string partyID = partyIDList[partyIDIndex];
                var partyData = DataTables.GetMonsterPartyData(partyID);
                partyDataList.Add(partyData);
            }
            else
            {
                partyIDList = dungeonData.bossPartyIDList;
                dungeonData.bossPartyIDList.ForEach(x => { partyDataList.Add(DataTables.GetMonsterPartyData(x)); });
            }

            DungeonRoom room = new DungeonRoom(partyDataList);
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
            if (currentRoomNumber < rooms.Count - 1)
            {
                if (currentRoomNumber > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine($"키를 눌러{currentRoomNumber + 1}번째 방으로 진입합니다.");

                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("키를 눌러 마지막 방으로 진입합니다.");
                Console.WriteLine("조심하세요 강력한 몬스터가 등장합니다.");
            }
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
        // 현재 룸의 모든 몬스터가 죽었다면 Room Clear!!
        if (currentRoom.CheckClear())
        {
            EnterNextRoom();
            return;
        }

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
            Display.PrintTimeGage(timeGageCount, 20, player.GetName(), ConsoleColor.Green);

            // Monsters Time Gage
            for(int i = 0; i < currentRoom.Monsters.Count; ++i)
            {
                // 몬스터가 죽지 않았을때
                Monster monster = currentRoom.Monsters[i] as Monster;
                if (currentRoom.Monsters[i].IsDead == false)
                {
                    monstersTimeCounter[i] += 1.0f + monster.Status.agility * 0.1f;     // 민첩성 수치의 10%를 게이지 추가에 반영
                    int tg = (int)(monstersTimeCounter[i] / 20.0f);
                    Display.PrintTimeGage(tg, 20, monster.monsterUniqueName, ConsoleColor.Red);
                }
                // 이미 죽었을때에는 턴 카운팅 할 필요X
                else
                {
                    int fullChCount = Utility.GetFullCharCount(monster.monsterUniqueName);
                    string monsterName = monster.monsterUniqueName.PadLeft(10 - fullChCount);
                    Console.WriteLine($"{monsterName} : X DEAD X");
                    monstersTimeCounter[i] = 0;
                }
            }

            bool anyStatusHasChanged = false;
            // 플레이어 턴
            if (timeGageCount >= 20)                        // 타임 게이지가 20이 되면 턴을 받음
            {
                player.ActiveTurn(currentRoom.Monsters);
                playerTimeCounter = 0;
                anyStatusHasChanged = true;
            }

            // 몬스터의 턴

            for (int i = 0; i < monstersTimeCounter.Count; ++i)
            {
                // 해당 몬스터가 죽었다면
                if (currentRoom.Monsters[i].IsDead == false && currentRoom.Monsters[i].CurrentHP <= 0)
                {
                    Monster m = currentRoom.Monsters[i] as Monster;
                    monstersTimeCounter[i] = 0;

                    // 몬스터의 죽음 처리
                    currentRoom.Monsters[i].Die();
                    Reward reward = m.GenerateReward();

                    // 플레이어에게 보상 전달
                    player.GetReward(reward);
                    Console.WriteLine();
                    Console.WriteLine($"키를 눌러 진행합니다....");
                    Console.ReadKey();

                    anyStatusHasChanged = true;
                    continue;
                }

                int tg = (int)(monstersTimeCounter[i] / 20.0f); 
                if (tg >= 20)
                {
                    Monster m = currentRoom.Monsters[i] as Monster;
                    m.ActiveTurn(player);
                    monstersTimeCounter[i] = 0;
                    anyStatusHasChanged = true;

                    if(player.IsDead)
                    {
                        break;
                    }
                }
            }
            
            if (anyStatusHasChanged)
                break;

            Console.SetCursorPosition(0, Console.CursorTop - (1 + currentRoom.Monsters.Count));
        }
    }

    void DungeonClear()
	{
        Console.WriteLine();
        Console.WriteLine($"{name}을 클리어 하였습니다 축하드립니다.");
        Reward reward = GenerateDungeonReward();
        player.GetReward(reward);
        Console.WriteLine();
        Console.WriteLine($"키를 눌러 마을로 돌아갑니다....");
        Console.ReadKey();
        isCleared = true;
    }

    public Reward GenerateDungeonReward()
    {
        Reward reward = new Reward();
        reward.exp = 0;
        reward.gold = 1000;
        reward.items = new List<int>();

        return reward;
    }
}
