﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using System.Windows.Markup;
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
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameManager();

            return instance;
        }
    }
    GameState currentGameState = GameState.Ready;
    GameState nextGameState = GameState.Town;
    Player player;
    List<string> clearDungeonList;
    Dungeon currentDungeon;

    public GameManager()
	{
        instance = this;

        // 클리어 한 던전의 정보를 기록할 던전 ID 리스트
        clearDungeonList = new List<string>();

        // 플레이어 생성
        player = new Player();

        // 현재 입장한 던전은 없는 상태로 초기화 -> 던전에 입장하면 해당 던전의 클래스로 연결할 예정
        currentDungeon = null;
    }

    public void StartGame(GameStartType startType)
    {
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

            int select = Display.SelectYesOrNo();
            if (select == 1)
            {
                player.SetName(playerName);
                player.SetClass(ClassType.Novice);
                player.GetGold(500);
                var slot = player.Inventory.AddItem(DataTables.GetItemData(4), 1, false);
                player.Equip(slot, false);
                player.Inventory.SetTestInventory(); // Test Inventory
                SelectInitialStat();
                IntroMessage();
                break;
            }

            Console.WriteLine();
        }

        // 던전 클리어 리스트 초기화
        clearDungeonList.Clear();
        #endregion
    }

    private void SelectInitialStat()
    {
        string[] statusType =
        {
            "1. 보통 : ",
            "2. 체력 : ",
            "3. 지능 : ",
            "4. 민첩 : ",
            "5. 명중 : "
        };

        int[,] statusSets = 
        {
            {  5,  5,  5,  5,  5,  5 },
            {  9,  2,  8,  5,  4,  2 },
            {  3, 10,  4,  4,  3,  2 },
            { 10,  1,  3,  5,  9,  8 },
            {  3,  5,  3,  9,  4,  6 },
        };

        List<string> strings = new List<string>();
        strings.Clear();
        for (int i = 0; i < 5; ++i)
        {
            string str = String.Format($"{statusType[i]} 힘:{statusSets[i, 0],2}, 지능:{statusSets[i, 1],2}, 체력:{statusSets[i, 2],2}, "
                                                  + $"솜씨:{statusSets[i, 3],2}, 민첩:{statusSets[i, 4],2}, 운:{statusSets[i, 5],2}");
            strings.Add(str);
        }

        while (true)
        {
            Console.WriteLine();
            string titleString = "캐릭터 기본 능력치를 선택해 주세요";
            int selectInput = Display.SelectInput(titleString, strings.ToArray(), strings.Count) - 1;

            Console.WriteLine();
            Console.WriteLine(
                $"힘   : {statusSets[selectInput, 0],2}\n" +
                $"지능 : {statusSets[selectInput, 1],2}\n" +
                $"체력 : {statusSets[selectInput, 2],2}\n" +
                $"솜씨 : {statusSets[selectInput, 3],2}\n" +
                $"민첩 : {statusSets[selectInput, 4],2}\n" +
                $"운   : {statusSets[selectInput, 5],2}\n");
            Console.WriteLine();
            Console.WriteLine("이 능력치로 결정하시겠습니까?");
            int select = Display.SelectYesOrNo();

            if(select == 1)
            {
                player.Status.strength = statusSets[selectInput, 0];
                player.Status.intelligence = statusSets[selectInput, 1];
                player.Status.vitality = statusSets[selectInput, 2];
                player.Status.dexterity = statusSets[selectInput, 3];
                player.Status.agility = statusSets[selectInput, 4];
                player.Status.luck = statusSets[selectInput, 5];

                Console.WriteLine();
                Console.WriteLine("게임을 시작하기 위해 아무키나 누르세요");
                Console.ReadKey();
                break;
            }
        }
    }

    private void IntroMessage()
    {
        Console.Clear();
        Console.WriteLine("대충 Intro Message");
        Console.WriteLine("아무 키나 누르세요..");
        Console.ReadKey();
    }

    private void LoadGame()
    {
    }

    bool SetState(GameState nextState)
    {
        // 입력된 상태값에 따라 게임의 모드를 바꾼다.
        switch (nextState)
        {
            case GameState.Town:                        // 마을 진입
                {
                    currentGameState = nextState;
                    EnterTown();
                    break;
                }
            case GameState.Dungeon:                     // 던전 진입
                {
                    // 게임 상태가 던전 모드가 아니었을때는 던전 입장 명령 실행
                    if (currentGameState != nextState)  
                    {
                        currentGameState = nextState;
                        EnterDungeon();
                    }
                    // 이미 던전 모드였을 경우는 배틀 로직을 실행
                    else
                    {
                        if (player.IsDead == false && currentDungeon.IsCleared == false)
                        {
                            Battle();
                        }
                        else
                        {
                            if (currentDungeon.IsCleared)
                                clearDungeonList.Add(currentDungeon.DungeonID);

                            nextGameState = GameState.Town;
                        }
                    }
                    break;
                }
            case GameState.Quit:
                {
                    // 종료 상태가 되었을때
                    currentGameState = nextState;

                    // false를 반환한다.
                    return false;
                }
        }

        // 그 외는 모두 true를 반환한다.
        return true;
    }

    void EnterTown()
    {
        Console.Clear();
        string townIntroString = "마을에 입장했습니다.";
        // 1. 던전 입장     2. 상점   3.여관    4.게임 종료
        string[] townMenuString =
        {
            "1. 던 전 입 장",
            "2. 상       점",
            "3. 여       관",
            "4. 인 벤 토 리",
            "5. 장 비 착 용",
            "6. 게 임 종 료"
        };
        int selectedMenu = Display.SelectInput(townIntroString, townMenuString, townMenuString.Length);
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
                OpenInventory();
                break;
            case 5:
                OpenEquipment();
                break;
            case 6:
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
        dungeonSelectMenuString.Add($"0. 마을로 돌아가기");

        Utility.MakeSameLengthStrings(dungeonSelectMenuString);
        
        int selectedDungeonIndex = Display.SelectInput(dungeonSelectTitle, dungeonSelectMenuString.ToArray(), dungeonSelectMenuString.Count, true);
        if (selectedDungeonIndex > 0)
        {
            DungeonData dungeonData = enabledDungeonData[selectedDungeonIndex - 1];
            PrepareDungeon(dungeonData);
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("키를 눌러 마을로 돌아갑니다.");
            Console.ReadKey();
            EnterTown();
        }
    }

    void EnterShop()
    {
        Console.Clear();
        Console.WriteLine("상점은 공사중입니다.");
        Console.WriteLine("아무키나 누르시면 마을로 돌아갑니다.");
        Console.ReadKey();
        nextGameState = GameState.Town;
    }

    void EnterInn()
    {
        Console.Clear();
        Console.WriteLine("여관은 공사중입니다.");
        Console.WriteLine("아무키나 누르시면 마을로 돌아갑니다.");
        Console.ReadKey();
        nextGameState = GameState.Town;
    }

    public void OpenInventory()
    {
        while (true)
        {
            Console.Clear();

            ItemSlot selectSlot = player.Inventory.ShowAndSelectInventory(ItemType.All);
            if (selectSlot == null)
                break;

            if (selectSlot != null)
            {
                Console.WriteLine($"{selectSlot.Name}, 사용하시겠습니까?");
                int select = Display.SelectYesOrNo();
                if (select == 1)
                {
                    if (selectSlot.IsEquipable())
                    {
                        Console.WriteLine("장비 아이템은 아직 미구현 입니다.");
                        Console.ReadKey();
                        continue;
                    }
                    
                    if (selectSlot.IsEmpty)
                    {
                        Console.WriteLine("비어있는 아이템 슬롯입니다.");
                        Console.ReadKey();
                        continue;
                    }

                    if (selectSlot.IsUsable())
                    {
                        ConsumableItemData cData = DataTables.GetConsumeItemData(selectSlot.ItemID);
                        if (cData.itemTarget == CosumableItemTargetType.Player)
                        {
                            selectSlot.UseItem(player, null);
                            continue;
                        }
                        else if (cData.itemTarget == CosumableItemTargetType.NonUsable)
                        {
                            Console.WriteLine("사용할 수 없는 아이템입니다.");
                            Console.ReadKey();
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("마을에서는 사용할 수 없는 아이템입니다.");
                            Console.ReadKey();
                            continue;
                        }
                    }
                }
            }
        }
        Console.WriteLine("마을로 돌아갑니다.");
        Console.ReadKey();
        nextGameState = GameState.Town;
    }

    public void OpenEquipment()
    {
        while (true)
        {
            Console.Clear();
            Display.PlayerInfo(player, true);

            int equipParts = player.Inventory.ShowEquipmens(player.Equips);
            if (equipParts < 0)
            {
                nextGameState = GameState.Town;
                break;
            }

            List<string> menuString = new List<string>();
            menuString.Add("1. 착용하기");
            menuString.Add("2. 해제하기");
            menuString.Add("0. 취    소");
            int selectedMenu = Display.SelectInput("", menuString.ToArray(), menuString.Count, true);
            if (selectedMenu == 0)
            {
                continue;
            }

            if (selectedMenu == 1)
            {
                ItemSlot slot = player.Inventory.ShowAndSelectInventory((ItemType)(equipParts));
                if (slot != null)
                {
                    player.Equip(slot, true);
                    continue;
                }
            }
            else
            {
                player.UnEquipItem((EquipmentType)(equipParts));
            }
        }
    }

    void EnterQuitMenu()
    {
        Console.WriteLine("게임을 종료합니다.");
        nextGameState = GameState.Quit;
    }

    void PrepareDungeon(DungeonData dungeonData)
    {
        currentDungeon = new Dungeon(player, dungeonData);
        nextGameState = GameState.Dungeon;
        Console.WriteLine($"{dungeonData.name}에 입장합니다.");
        Console.WriteLine("아무키나 누르세요...");
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
        // 게임의 상태를 제어하는 SetState의 결과만을 호출한다.
        return SetState(nextGameState);
    }
    
    public static int CalculateDamage(WeaponData weaponData, CharacterStatus status, int level)
    {
        int baseWeaponDamage = CaclulateWeaponDamage(weaponData, status);

        int sign = Utility.GetRandom(-5, 6);
        int randomAtk = (int)(baseWeaponDamage * 0.01f * sign);
        baseWeaponDamage += randomAtk;
        int damage = baseWeaponDamage + CalculateStatusDamage(weaponData, status, level);
        if (weaponData != null)
            damage = (int)(damage * (1 + (weaponData.percentATK * 0.01f))) ;

        return damage;
    }

    public static int CaclulateWeaponDamage(WeaponData weaponData, CharacterStatus status)
    {
        if (weaponData == null) return 0;

        int firstDamageStat = status.strength;
        if (weaponData.attackType == ItemAttackType.Range) firstDamageStat = status.dexterity;
        int baseWeaponDamage = (int)(weaponData.plusATK * ((firstDamageStat + 200.0f) * 0.005f));
        return baseWeaponDamage;
    }

    public static int CalculateStatusDamage(WeaponData weaponData, CharacterStatus status, int level)
    {
        int firstDamageStat = status.strength;
        if (weaponData != null && weaponData.attackType == ItemAttackType.Range) firstDamageStat = status.dexterity;
        int secondDamageStat = status.dexterity;
        if (weaponData != null && weaponData.attackType == ItemAttackType.Range) firstDamageStat = status.strength;

        int damage = firstDamageStat + (int)(secondDamageStat * 0.5f) + (int)(status.luck * 0.333f) + (int)(level * 0.25f) * 2;
        return damage;
    }

    public static int CalculateAttackHit(CharacterStatus attakerStat, int attakerLevel, CharacterStatus targetStat)
    {
        int atkRatio = 80;
        int atkRatioCorrection = (attakerStat.dexterity + attakerLevel) - (targetStat.agility);
        atkRatio += atkRatioCorrection;
        atkRatio = Math.Clamp(atkRatio, atkRatio, 95);

        return atkRatio;
    }
}
