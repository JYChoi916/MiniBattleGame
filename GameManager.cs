using System;
public enum GameStartType
{
    New = 1,
    Load
}

public enum GameState
{
    Ready = -1,
    Town,
    Dungeon,
    Quit
}
public class GameManager
{
    GameState currentGameState = GameState.Ready;
    GameState nextGameState = GameState.Town;
    Player player;
    List<string> clearDungeonList;
    Dungeon currentDungeon;

    public GameManager(GameStartType startType)
	{
        clearDungeonList = new List<string>();
        currentDungeon = null;
        switch (startType)
        {
            case GameStartType.New:
                NewGame();
                break;
            case GameStartType.Load:
                LoadGame();
                break;
        }
    }

    private void NewGame()
    {
        #region Input Player Infomation
        Console.Clear();

        // 이름 입력
        while (true) {
            Console.Write("플레이어의 이름을 입력하세요 (최대 6글자) : ");
            string playerName = Console.ReadLine();
            if (playerName.Length > 6)
            {
                Console.WriteLine("길이가 너무 깁니다.");
                continue;
            }
            Console.WriteLine($"입력하신 이름은 \'{playerName}\'입니다. 이 이름으로 정하시겠습니까?");
            string[] selectString =
            {
            "1. 예",
            "2. 아니오"
            };
            int select = Display.SelectInput("", selectString);
            if(select == 1)
            {
                player = new Player(playerName, ClassType.Novice);
                break;
            }
            Console.WriteLine();
        }

        // 던전 클리어 리스트 초기화
        clearDungeonList.Clear();
        #endregion
    }

    private void LoadGame()
    {
    }

    bool SetState(GameState state)
    {
        switch (state)
        {
            case GameState.Town:
                {
                    currentGameState = state;
                    EnterTown();
                    break;
                }
            case GameState.Dungeon:
                {
                    if (currentGameState != state)
                    {
                        currentGameState = state;
                        EnterDungeon();
                    }
                    else
                    {
                        Battle();
                        Console.ReadLine();
                    }
                    break;
                }
            case GameState.Quit:
                {
                    currentGameState = state;
                    return false;
                }
        }

        return true;
    }

    void EnterTown()
    {
        Console.Clear();
        string townIntroString = "마을에 입장했습니다.";
        // 1. 던전 입장     2. 상점   3.여관    4.게임 종료
        string[] townMenuString =
        {
            "1. 던전 입장",
            "2. 상     점",
            "3. 여     관",
            "4. 게임 종료"
        };
        int selectedMenu = Display.SelectInput(townIntroString, townMenuString);
        switch(selectedMenu)
        {
            case 1:
                SelectDungeon();
                break;
            case 2:
                EnterShop();
                break;
            case 3:
                EnterInn();
                break;
            case 4:
                EnterQuitMenu();
                break;
        }
    }

    void SelectDungeon()
    {
        Console.Clear();
        List<DungeonData> enabledDungeonData = new List<DungeonData>();
        for(int i = 0; i < DataTables.DungeonTable.Count; ++i)
        {
            string clearDungeon = DataTables.DungeonTable[i].clearCondition;
            if (clearDungeon != null && clearDungeon.Length > 0)
            {
                if (clearDungeonList.Contains(DataTables.DungeonTable[i].clearCondition))
                    enabledDungeonData.Add(DataTables.DungeonTable[i]);
            }
            else
            {
                enabledDungeonData.Add(DataTables.DungeonTable[i]);
            }
        }

        string dungeonSelectTitle = "입장할 던전을 선택하세요.";
        List<string> dungeonSelectMenuString = new List<string>();
        for(int i = 0; i < enabledDungeonData.Count; ++i)
        {
            dungeonSelectMenuString.Add($"{i+1}. {enabledDungeonData[i].name}");
        }
        int selectedDungeonIndex = Display.SelectInput(dungeonSelectTitle, dungeonSelectMenuString.ToArray());
        DungeonData dungeonData = enabledDungeonData[selectedDungeonIndex-1];

        PrepareDungeon(dungeonData);
    }

    void EnterShop()
    {

    }

    void EnterInn()
    {

    }

    void EnterQuitMenu()
    {

    }

    void PrepareDungeon(DungeonData dungeonData)
    {
        currentDungeon = new Dungeon(player, dungeonData);
        nextGameState = GameState.Dungeon;
        Console.WriteLine($"{dungeonData.name}에 입장합니다.");
        Console.ReadKey();
    }

    void EnterDungeon()
    {
        currentDungeon.Enter();
    }

    void Battle()
    {
        currentDungeon.Battle();
    }

    public bool Play()
    {
        return SetState(nextGameState);
    }
}
