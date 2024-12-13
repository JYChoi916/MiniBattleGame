﻿using System.Text.Json.Serialization;
using System.Text.Json;

public enum ClassType
{
    All = -1,
    Novice,
    SwordMan,
    Magician,
    Acolyte,
    Thief,
    Archer
}

public enum Status
{
    Strength,
    Intelligence,
    Vitality,
    Dexterity,
    Agility,
    Luck
}

public enum SkillType
{
    Passive,
    Active
}

public enum SkillTargetType
{
    Single,
    All
}

public enum CosumableItemTargetType
{
    NonUsable = -1,
    Player = 0,
    Enemy
}

public enum DamageAffectionType
{
    Physical,
    Magical,
    Recover
}

public enum ItemType
{
    Consumable,
    KeyItem,
    Weapon,
    Armor
}

public enum ItemAttackType
{
    Melee,
    Range,
    Magic
}

// 직업 데이터
public class ClassData
{
    public ClassType classID { get; set; }     // 직업 ID
    public string name { get; set; }        // 직업 이름
    public int levelUpID { get; set; }      // 레벨업 경험치 테이블
    public int upStr { get; set; }          // 레벨업 당 증가 힘
    public int upInt { get; set; }          // 레벨업 당 증가 지능
    public int upVit { get; set; }          // 레벨업 당 증가 체력
    public int upDex { get; set; }          // 레벨업 당 증가 손재주
    public int upAgi { get; set; }          // 레벨업 당 증가 민첩
    public int upLuk { get; set; }          // 레벨업 당 증가 운
}

public class ExpData
{
    public string expID { get; set; }              // 레벨업 필요 경험치 데이터 ID
    public int maxLevel { get; set; }           // 최대 레벨
    public int firstLevelUpExt { get; set; }    // 1레벨 기준 필요 경험치
    public int coefficient { get; set; }        // 레벨이 오를때마다 계산하는 증가 계수
}

public class SkillData
{
    public string skillID { get; set; }             // 스킬 ID 
    public string name { get; set; }                // 스킬 이름
    public string desc_1 { get; set; }              // 스킬 발동(사용)시 출력되는 메시지 

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> canGetClass { get; set; }   // 해당 스킬을 얻을 수 있는 클래스의 ID목록
    public Status affectiveStatus { get; set; }     // 스킬에 영향을 미치는 스테이터스 
    public int mpCost { get; set; }                 // 스킬 사용에 필요한 MP
    public SkillType skillType { get; set; }        // 스킬 발동 타입
    public SkillTargetType targetType { get; set; }      // 스킬 대상 타입
    public DamageAffectionType attackType { get; set; }      // 스킬 효과 타입

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> skillParams { get; set; }      // 스킬에 사용되는 각종 수치 데이터
    public int getClassLevel { get; set; }          // 스킬을 얻는 레벨
}

public class MonsterData
{
    public string monsterID { get; set; }               // 몬스터 ID
    public string name { get; set; }                    // 몬스터 이름
    public int hp { get; set; }                         // 몬스터 HP
    public int mp { get; set; }                         // 몬스터 MP
    public int strength { get; set; }                   // 힘
    public int intelligence { get; set; }               // 지능
    public int vitality { get; set; }                   // 체력
    public int dexterity { get; set; }                  // 손재주
    public int agility { get; set; }                    // 민첩성
    public int luck { get; set; }                       // 운
    public int deathExp { get; set; }                   // 처치시 획득 경험치
    public int minGold { get; set; }                    // 획득 최소 골드
    public int maxGold { get; set; }                    // 획득 최대 골드

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> usingSkillIDList { get; set; }  // 사용 스킬 목록

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> skillRatioList { get; set; }       // 스킬 사용하는 확률 목록

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> itemDropList { get; set; }         // 처치시 드랍하는 아이템 목록

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> itemDropRates { get; set; }        // 드랍 아이템 확률 목록
}

public class MonsterPartyData
{
    public string partyID { get; set; }                 // 몬스터 파티 ID

    [JsonPropertyName("monsterIDList")]
    public List<string> monsterIDList { get; set; }     // 등장하는 몬스터 ID 목록

    public int maxCount { get; set; }                   // 해당 파티에 등장하는 최대 몬스터 수
}

public class DungeonData
{
    public string dungeonID { get; set; }                   // 던전 ID
    public string name { get; set; }                        // 던전 이름
    public int minRoomCount { get; set; }                   // 최소 배틀 횟수
    public int maxRoomCount { get; set; }                   // 최대 배틀 횟수

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> normalPartyIDList { get; set; }     // 마지막 방을 제외한 몬스터  파티 목록

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> bossPartyIDList { get; set; }       // 마지막 방 몬스터 파티 목록
}

public class ItemData
{
    public int id { get; set; }             // 아이템 드랍 또는 인벤토리에 사용할 ID
    public string name { get; set; }        // 아이템 이름
    public string itemID { get; set; }      // 실제 아이템의 사용 또는 착용을 하기 위한 데이터 테이블의 ID
    public ItemType type { get; set; }      // 아이템의 종류
    public int maxStack { get; set; }       // 인벤토리 1칸에 쌓아둘 수 있는 최대 개수
    public int price { get; set; }          // 상점에 판매할 때 가격 (상점에 등장시에는 이 가격의 두배)
}

public class ConsumableItemData
{
    public string itemID { get; set; }                      // 아이템 ID
    public bool usable { get; set; }                        // 사용가능 여부
    public CosumableItemTargetType itemTarget { get; set; } // 아이템 사용 대상 

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> itemParams { get; set; }                     // 아이템 사용 관련 수치 리스트
}

public class WeaponData
{
    public string itemID { get; set; }                  // 무기 ID
    public ItemAttackType attackType { get; set; }      // 공격 타입 (보정 받는 스탯이 달라짐)
    public int plusATK { get; set; }                    // +로 추가되는 공격력
    public int percentATK { get; set; }                 // x로 증가되는 공격력
    public int percentMATK { get; set; }                // x로 증가되는 마법공격력
    public int criticalRatio { get; set; }              // 치명타 확률 증가량

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int> equipableClasses { get; set; }     // 착용가능한 직업 목록
}



public static class DataTables
{
    private static string[] tableFiles = {
        "classTable",
        "monsterTable",
        "monsterPartyTable",
        "dungeonTable",
        "expTable",
        "skillTable",
        "itemTable",
        "consumableItemTable",
        "weaponTable"
    };

    private static List<ClassData> classTable = new List<ClassData>();
    private static Dictionary<ClassType, ClassData> classDic = new Dictionary<ClassType, ClassData>();

    private static List<MonsterData> monsterTable = new List<MonsterData>();
    private static Dictionary<string, MonsterData> monsterDic = new Dictionary<string, MonsterData>();

    private static List<MonsterPartyData> monsterPartyTable  = new List<MonsterPartyData>();
    private static Dictionary<string, MonsterPartyData> monsterPartyDic = new Dictionary<string, MonsterPartyData>();

    private static List<DungeonData> dungeonTable = new List<DungeonData>();
    private static Dictionary<string, DungeonData> dungeonDic = new Dictionary<string, DungeonData>();

    private static List<ExpData> expTable = new List<ExpData>();
    private static Dictionary<string, ExpData> expDic = new Dictionary<string, ExpData>();

    private static List<SkillData> skillTable = new List<SkillData>();
    private static Dictionary<string, SkillData> skillDic = new Dictionary<string,SkillData>();

    private static List<ItemData> itemTable = new List<ItemData>();

    private static List<ConsumableItemData> consumableTable = new List<ConsumableItemData>();
    private static Dictionary<string, ConsumableItemData> consumableDic = new Dictionary<string, ConsumableItemData>();

    private static List<WeaponData> weaponTable = new List<WeaponData>();
    private static Dictionary<string, WeaponData> weaponDic = new Dictionary<string, WeaponData>();

    public static void LoadTables()
    {
        ClearTables();

        classTable = LoadTable<List<ClassData>>(tableFiles[0]);
        classTable?.ForEach(x => { classDic.Add(x.classID, x); });
        //if (classTable != null)
        //{
        //    foreach (var data in classTable)
        //    {
        //        classDic.Add(data.classID, data);
        //    }
        //}

        monsterTable = LoadTable<List<MonsterData>>(tableFiles[1]);
        monsterTable?.ForEach(x => { monsterDic.Add(x.monsterID, x); });

        monsterPartyTable = LoadTable<List<MonsterPartyData>>(tableFiles[2]);
        monsterPartyTable?.ForEach(x => { monsterPartyDic.Add(x.partyID, x); });

        dungeonTable = LoadTable<List<DungeonData>>(tableFiles[3]);
        dungeonTable?.ForEach(x => { dungeonDic.Add(x.dungeonID, x); });

        expTable = LoadTable<List<ExpData>>(tableFiles[4]);
        expTable?.ForEach(x => { expDic.Add(x.expID, x); });

        skillTable = LoadTable<List<SkillData>>(tableFiles[5]);
        skillTable?.ForEach(x => { skillDic.Add(x.skillID, x); });

        itemTable = LoadTable<List<ItemData>>(tableFiles[6]);

        consumableTable = LoadTable<List<ConsumableItemData>>(tableFiles[7]);
        consumableTable?.ForEach(x => { consumableDic.Add(x.itemID, x); });

        weaponTable = LoadTable<List<WeaponData>>(tableFiles[8]);
        weaponTable?.ForEach(x => { weaponDic.Add(x.itemID, x); });

        Console.WriteLine("테이블 데이터 로드 완료 ............");
    }

    private static void ClearTables()
    {
        classTable.Clear();
        classDic.Clear();
        monsterTable.Clear();
        monsterDic.Clear();
        monsterPartyTable.Clear();
        monsterPartyDic.Clear();
        dungeonTable.Clear();
        dungeonDic.Clear();
        expTable.Clear();
        expDic.Clear();
        skillTable.Clear();
        skillDic.Clear();
        itemTable.Clear();
        consumableTable.Clear();
        consumableDic.Clear();
        weaponTable.Clear();
        weaponDic.Clear();
    }

    private static T LoadTable<T>(string filename)
    {
        string fullPath = "./Tables/" + filename + ".json";
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        StreamReader sr = new StreamReader(fullPath);
        string jsonString = sr.ReadToEnd();

        T table = default(T);
        try
        {
            table = JsonSerializer.Deserialize<T>(jsonString, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return table;
    }

    public static ClassData GetClassData(ClassType type)
    {
        classDic.TryGetValue(type, out ClassData data);
        return data;
    }

    public static MonsterData GetMonsterData(string id)
    {
        monsterDic.TryGetValue(id, out MonsterData data);
        return data;
    }

    public static MonsterPartyData GetMonsterPartyData(string id)
    {
        monsterPartyDic.TryGetValue(id, out MonsterPartyData data);
        return data;
    }

    public static DungeonData GetDungeonData(string id)
    {
        dungeonDic.TryGetValue(id, out DungeonData data);
        return data;
    }

    public static ExpData GetExpData(string id)
    {
        expDic.TryGetValue(id, out ExpData data);
        return data;
    }

    public static ItemData GetItemData(int index)
    {
        return itemTable[index];
    }

    public static ConsumableItemData GetConsumeItemData(string id)
    {
        consumableDic.TryGetValue(id, out ConsumableItemData data);
        return data;
    }

    public static WeaponData GetWeaponData(string id)
    {
        weaponDic.TryGetValue(id, out WeaponData data);
        return data;
    }

    public static SkillData GetSkillData(string skillID)
    {
        skillDic.TryGetValue(skillID, out SkillData data);
        return data;
    }
}