using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Xml.Linq;

public interface IAttackable
{
    public void Attack(Character targetCharacter);
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
    public CharacterStatus Status { get { return status; } }
    protected int currentHP;
    public int CurrentHP { get { return currentHP; } }
    protected int maxHP;
    protected int currentMP;
    protected int maxMP;
    protected int atk;
    protected int matk;
    protected int def;
    protected int mdef;
    protected bool isDead;
    public bool IsDead { 
        get { return isDead; } 
    }

    public Character()
    {
        status = new CharacterStatus();
        isDead = false;
    }

    virtual public string GetName() { return ""; }

    virtual public void GetDamage(int damage) { }
}

public class Player : Character, IAttackable
{
    string name;
    public override string GetName()
    {
        return name;
    }
    ClassType classType;
    public ClassType ClassType { get { return classType; } }

    int level;
    int exp;
    int nextExp;

    public Player(string name, ClassType classType) : base()
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
        if (level < 1)
            nextExp = expData.firstLevelUpEXP;
        else
            nextExp = nextExp + (level * expData.coefficient);
        
        level++;
        // 습득 할 수 있는 스킬 체크 후 습득
    }

    public void RecoverAll()
    {
    }

    public void Attack(Character targetCharacter)
    {
        int damage = GameManager.CalculateDamage(null, status, level);

        // 무기를 들었을 경우 
        int dice = Utility.GetRandom(0, 100);
        if (dice <= status.luck + 20)
        {
            damage *= 2;
            targetCharacter.GetDamage(damage);
        }
        else
        {
            int atkRatio = GameManager.CalculateAttackHit(status, level, targetCharacter.Status);
            dice = Utility.GetRandom(0, 100);
            if (dice <= atkRatio)
            {
                Console.WriteLine($"{name}의 공격!!!");
                targetCharacter.GetDamage(damage);
            }
            else
            {
                string targetName = targetCharacter.GetName();
                Console.WriteLine($" => {targetName}의 공격 회피!\t{name}의 공격이 실패 하였습니다.");
            }
        }
    }

    public override void GetDamage(int damage)
    {
        Console.Write($"{name}의 ");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($"{damage}");
        Console.ResetColor();
        Console.WriteLine("데미지 !!!!!");
        currentHP -= damage - status.vitality;
        if(currentHP < 0)
        {
            isDead = true;
        }
    }

    public void Die()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"!!!  {name}, 전투불능  !!!");
        Console.ResetColor();
        Console.WriteLine($"다른 모험가에 의해 마을로 옮겨집니다.");
        Console.ReadKey();
    }

    public void ActiveTurn(List<Monster> monsters)
    {
        Console.WriteLine($"{name}의 턴입니다!!");
        // 스킬 사용 및 일반 공격 선택 

        // 스킬 사용

        // 일반 공격
        int targetMonsterIndex = SeletectTargetIndex(monsters);
        Attack(monsters[targetMonsterIndex]);
        Console.WriteLine("키를 눌러 턴을 종료하세요");
        Console.ReadKey();
    }

    public int SeletectTargetIndex(List<Monster> monsters)
    {
        List<Monster> targets = new List<Monster>();
        List<string> targetMonsterNames = new List<string>();
        for (int i = 0; i < monsters.Count; i++)
        {
            targetMonsterNames.Add($"{i+1}. {monsters[i].monsterUniqueName}");
        }

        return Display.SelectInput("공격할 대상을 선택하세요", targetMonsterNames.ToArray()) - 1;
    }

    public void GetReward(Reward reward)
    {
        exp += reward.exp;
        if(exp >= nextExp)
        {
            LevelUp();
        }

        // 골드와 아이템은 인벤토리 구현 이후 추가
    }
}

public class Reward
{
    public List<int> itemId;
    public int gold;
    public int exp;
}

public class Monster : Character, IAttackable
{
    [Required]
    MonsterData data;
    public MonsterData Data { get { return data; } }

    public string monsterUniqueName = "";
    public override string GetName()
    {
        return monsterUniqueName;
    }

    public Monster(MonsterData data) : base()
    {
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


    public string[] GetMonsterInfos()
    {
        string hpString = isDead ? "X DEAD X" : $"{currentHP}/{maxHP}";
        string mpString = isDead ? "X DEAD X" : $"{currentMP}/{maxMP}";
        string[] monsterInfos = 
        {
            $"이  름 : {monsterUniqueName}",
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

    public Reward Die()
    {
        isDead = true;
        Reward reward = new Reward();
        reward.exp = data.deathExp;
        reward.gold = Utility.GetRandom(data.minGold, data.maxGold);
        reward.itemId = new List<int>();
        for (int i = 0; i < data?.itemDropRates?.Count; ++i)
        {
            int dice = Utility.GetRandom(0, 100);
            if (dice <= data.itemDropRates[i])
            {
                reward.itemId.Add(data.itemDropList[i]);
            }
        }
        return reward;
    }

    public void ActiveTurn(Player player)
    {
        Console.WriteLine($"{data.name}의 턴입니다!!");

        int maxDice = 100;
        int skillIndex = -1;
        for(int i = 0; i < data.skillRatioList.Count; ++i)
        {
            int ratio = data.skillRatioList[i];
            int dice = Utility.GetRandom(0, maxDice);
            maxDice -= ratio;
            if (dice < ratio)
            {
                skillIndex = i;
                break;
            }
        }

        if (skillIndex > 0)
        {
            SkillData skillData = DataTables.GetSkillData(data.usingSkillIDList[skillIndex]);
            // 스킬 사용
        }
        else
        {
            Attack(player);
        }

        Console.WriteLine("아무 키나 누르시면 진행합니다.");
        Console.ReadKey();
    }

    public void Attack(Character targetCharacter)
    {
        int damage = GameManager.CalculateDamage(null, status, 0);

        int atkRatio = GameManager.CalculateAttackHit(status, 0, targetCharacter.Status);
        int dice = Utility.GetRandom(0, 100);
        if (dice <= atkRatio)
        {
            targetCharacter.GetDamage(damage);
        }
        else
        {
            string targetName = targetCharacter.GetName();
            Console.WriteLine($" => {targetName}의 공격 회피!\t{monsterUniqueName}의 공격이 실패 하였습니다.");
        }
    }

    public override void GetDamage(int damage)
    {
        Console.Write($" => {monsterUniqueName}의 ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($" {damage} ");
        Console.ResetColor();
        Console.WriteLine("데미지 !!!!!");
        currentHP -= damage - status.vitality;
        if (currentHP <= 0)
        {
            currentHP = 0;
        }
    }
}
