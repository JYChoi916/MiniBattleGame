using System.Xml.Linq;

interface IAttackable
{
    public void Attack(Character targetCharacter);
}

interface IDamagable
{
    public void GetDamage(int damage);
}

public class CharacterStatus
{
    public int strength;
    public int intelligence;
    public int vitality;
    public int dexterity;
    public int agility;
    public int luck;
}

public class Character
{
    protected CharacterStatus status;
    protected int currentHP;
    protected int maxHP;
    protected int currentMP;
    protected int maxMP;
    protected int atk;
    protected int matk;
    protected int def;
    protected int mdef;
    protected bool isDead;

    public Character()
    {
        status = new CharacterStatus();
        isDead = false;
    }
}

public class Player : Character, IAttackable, IDamagable
{
    string name;
    ClassType classType;

    int level;
    int exp;
    int nextExp;

    public Player(string name, ClassType classType)
    {
        this.name = name;
        SetClass(classType);
    }

    public string[] GetPlayerInfoString()
    {
        string[] playersInfo =
        {
            $"이  름 : {name}",
            $"직  업 : {classType.ToString()}",
            $"Ｈ  Ｐ : {currentHP}/{maxHP}",
            $"Ｍ  Ｐ : {currentMP}/{maxMP}",
            $"ＳＴＲ : {status.strength}",
            $"ＩＮＴ : {status.intelligence}",
            $"ＶＩＴ : {status.vitality}",
            $"ＤＥＸ : {status.dexterity}",
            $"ＡＧＩ : {status.agility}",
            $"ＬＵＫ : {status.luck}",
        };
        return playersInfo;
    }

    public void SetClass(ClassType classType)
    {
        this.classType = classType;
        level = 0;
        LevelUp();
    }

    public void LevelUp()
    {
        level++;
        // 레벨업 증가시 스탯 증가 
        ClassData classData = DataTables.GetClassData(classType);
        status.strength += classData.upStr;
        status.intelligence += classData.upInt;
        status.vitality += classData.upVit;
        status.dexterity += classData.upDex;
        status.agility += classData.upAgi;
        status.luck += classData.upLuk;

        // 최대 HP/MP 계산 + 회복
        maxHP = classData.baseHP + status.vitality * 10;
        maxMP = classData.baseMP + status.intelligence * 6;
        currentHP = maxHP;
        currentMP = maxMP;

        // 경험치 0으로
        exp = 0;

        // 클래스 타입에 따른 레벨업 테이블 데이터에 의해 nextExp를 세팅
        ExpData expData = DataTables.GetExpData(ExpType.novice_exp.ToString());
        nextExp = expData.firstLevelUpExt;

        // 습득 할 수 있는 스킬 체크 후 습득
    }

    public void RecoverAll()
    {
    }

    void IAttackable.Attack(Character targetCharacter)
    {
        
    }

    void IDamagable.GetDamage(int damage)
    {
        
    }
}

public struct Reward
{
    public int itemId;
    public int gold;
    public int exp;
}

public class Monster : Character, IAttackable, IDamagable
{
    MonsterData data;
    public MonsterData Data { get { return data; } }

    public Monster(MonsterData data) {
        this.data = data;
        status.strength = data.strength;
        status.intelligence += data.intelligence;
        status.vitality += data.vitality;
        status.dexterity += data.dexterity;
        status.agility += data.agility;
        status.luck += data.luck;

        maxHP = data.hp;
        currentHP = maxHP;
        maxMP = data.mp;
        currentMP = maxMP;
    }

    void IAttackable.Attack(Character targetCharacter)
    {
        
    }

    void IDamagable.GetDamage(int damage)
    {
        
    }

    public string[] GetMonsterInfos()
    {
        string hpString = isDead ? "☠☠☠☠☠☠☠☠" : $"{currentHP}/{maxHP}";
        string mpString = isDead ? "☠☠☠☠☠☠☠☠" : $"{currentMP}/{maxMP}";
        string[] monsterInfos = 
        {
            $"이  름 : {data.name}",
            $"Ｈ  Ｐ : {hpString}",
            $"Ｍ  Ｐ : {mpString}",
            $"ＳＴＲ : {status.strength}",
            $"ＩＮＴ : {status.intelligence}",
            $"ＶＩＴ : {status.vitality}",
            $"ＤＥＸ : {status.dexterity}",
            $"ＡＧＩ : {status.agility}",
            $"ＬＵＫ : {status.luck}",
        };
        return monsterInfos;
    }

    //public Reward Die ()
    //{
    //    Reward reward;
    //    return reward;
    //}
}
