using System.ComponentModel.DataAnnotations;
using System.Numerics;
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

    virtual public void RecoverHP(int recoverPoint)
    {
        Console.Write($"{GetName()}, ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{recoverPoint} ");
        Console.ResetColor();
        Console.WriteLine($" HP 회복 !!!!!");
        currentHP += recoverPoint;
        currentHP = currentHP > maxHP ? maxHP : currentHP;
    }
    virtual public void RecoverMP(int recoverPoint)
    {
        Console.Write($"{GetName()}, ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{recoverPoint} ");
        Console.ResetColor();
        Console.WriteLine($" HP 회복 !!!!!");
        currentMP += recoverPoint;
        currentMP = currentMP > maxMP ? maxMP : currentMP;
    }

    virtual public void Die() {
        isDead = true;
    }
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
    int gold;
    public int Gold { get { return gold; } }
    Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    public Player() : base()
    {
        gold = 0;
        inventory = new Inventory(20);
    }

    public void GetGold(int gold)
    {
        this.gold += gold;
    }

    public string[] GetPlayerInfoString()
    {
        string[] playersInfo =
        {
            $"이  름 : {name}",
            $"직  업 : {classType.ToString()}",
            $"레  벨 : {level}",
            $"경험치 : {exp}/{nextExp}",
            $"Ｈ  Ｐ : {currentHP}/{maxHP}",
            $"Ｍ  Ｐ : {currentMP}/{maxMP}",
            $"ＳＴＲ : {status.strength}",
            $"ＩＮＴ : {status.intelligence}",
            $"ＶＩＴ : {status.vitality}",
            $"ＤＥＸ : {status.dexterity}",
            $"ＡＧＩ : {status.agility}",
            $"ＬＵＫ : {status.luck}",
            $"소지금 : {gold}",
        };
        return playersInfo;
    }

    public void SetName(string name)
    {
        this.name = name;
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
            Console.WriteLine();
            Console.WriteLine($"{name}의 공격!!!");
            dice = Utility.GetRandom(0, 100);
            if (dice <= atkRatio)
            {
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
        int dmg = damage - status.vitality;
        Console.WriteLine();
        Console.Write($"{name}의 ");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($"{dmg}");
        Console.ResetColor();
        Console.WriteLine("데미지 !!!!!");
        currentHP -= dmg;
        if(currentHP < 0)
        {
            isDead = true;
        }
    }

    public override void Die()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"!!!  {name}, 전투불능  !!!");
        Console.ResetColor();
        Console.WriteLine($"다른 모험가에 의해 마을로 옮겨집니다.");
        Console.ReadKey();
    }

    public void ActiveTurn(List<Character> monsters)
    {
        Console.WriteLine();
        Console.WriteLine($"{name}의 턴입니다!!");
        // 일반 공격 / 스킬 사용 / 아이템 사용 선택 
        ConsoleKeyInfo cki = Console.ReadKey();
        if (cki.Key == ConsoleKey.C)
        {
            for(int i = 0; i < monsters.Count; ++i)
            {
                Monster m = monsters[i] as Monster;
                m.Die();
            } 
        }

        bool usedTurn = false;

        while (!usedTurn)
        {
            string[] actionStrings =
            {
                "1. 일 반 공 격",
                "2. 아이템 사용",
                //"3. 스 킬 사 용",
                "0. 도 망 치 기"
            };

            int selectedAction = Display.SelectInput("행동을 선택해 주세요", actionStrings, actionStrings.Length, true);
            
            switch (selectedAction)
            {
                case 1:
                    int targetMonsterIndex = SeletectTargetIndex(monsters);
                    if (targetMonsterIndex >= 0)
                    {
                        Attack(monsters[targetMonsterIndex]);
                        usedTurn = true;
                    }
                    break;
                case 2:
                    usedTurn = SelectAndUseItem(monsters);
                    break;
                //case 3:
                //    break;
                case 0:
                    usedTurn = true;
                    break;
            }
        }

        // 일반 공격


        // 아이템 사용


        // 스킬 사용

        Console.WriteLine();
        Console.WriteLine("키를 눌러 턴을 종료하세요");
        Console.ReadKey();
    }

    public bool SelectAndUseItem(List<Character> monsters)
    {
        bool itemUsed;
        List<Character> targets = new List<Character>();
        while (true)
        {
            ItemSlot slot = inventory.ShowAndSelectInventory(true);
            if (slot != null)
            {
                ConsumableItemData cData = DataTables.GetConsumeItemData(slot.ItemID);
                if (cData.itemTarget == CosumableItemTargetType.Player)
                {
                    targets.Add(this);
                }
                else
                {
                    int selectTargetIndex = SeletectTargetIndex(monsters);
                    targets.Add(monsters[selectTargetIndex]);
                    if (cData.useType == ItemUseType.AreaDamage)
                    {
                        if (selectTargetIndex > 0)
                            targets.Add(monsters[selectTargetIndex - 1]);

                        if (selectTargetIndex < monsters.Count - 1)
                            targets.Add(monsters[selectTargetIndex + 1]);
                    }
                }

                Console.WriteLine($"{slot.Name}, 사용하시겠습니까?");
                int select = Display.SelectYesOrNo();
                if (select == 1)
                {
                    if (slot.UseItem(this, targets))
                        return true;
                }
            }
            else
            {
                break;
            }
        }
        return false;
    }

    public int SeletectTargetIndex(List<Character> monsters)
    {
        List<Monster> targets = new List<Monster>();
        List<string> targetMonsterNames = new List<string>();
        for (int i = 0; i < monsters.Count; i++)
        {
            Monster m = monsters[i] as Monster;
            targetMonsterNames.Add($"{i+1}. {m.monsterUniqueName}");
        }
        targetMonsterNames.Add("0. 취소");
        Utility.MakeSameLengthStrings(targetMonsterNames);

        Console.WriteLine();
        return Display.SelectInput("공격할 대상을 선택하세요", targetMonsterNames.ToArray(), targetMonsterNames.Count, true) - 1;
    }

    public void GetReward(Reward reward)
    {
        if (reward.exp > 0)
        {
            exp += reward.exp;
            Console.Write("경험치를 "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(reward.exp); Console.ResetColor(); Console.WriteLine(" 얻었습니다.");
            ClassData classData = DataTables.GetClassData(classType);
            ExpType expType = (ExpType)classData.levelUpID;
            ExpData expData = DataTables.GetExpData(expType.ToString());
            if (exp >= nextExp && level < expData.maxLevel)
            {
                Console.ForegroundColor = ConsoleColor.Green; Console.Write(level + 1); Console.ResetColor(); Console.WriteLine(" 레벨이 되었습니다 !!!!");
                Console.Write("힘이     "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upStr); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                Console.Write("지능이   "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upInt); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                Console.Write("체력이   "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upVit); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                Console.Write("솜씨가   "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upDex); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                Console.Write("민첩이   "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upAgi); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                Console.Write("운이     "); Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write(classData.upLuk); Console.ResetColor(); Console.WriteLine("증가하였습니다.");
                LevelUp();
            }
        }

        if (reward.gold > 0)
        {
            gold += reward.gold;
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write($"{reward.gold} "); Console.ResetColor(); Console.WriteLine("골드를 얻었습니다.");
        }

        if (reward.items.Count >0)
        {
            foreach (var itemId in reward.items) {
                ItemData data = DataTables.GetItemData(itemId);
                if (data != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; Console.Write($"{data.name} ");
                    Console.WriteLine("발견!!");
                    while (inventory.AddItem(data, 1) == false)
                    {
                        Console.WriteLine("가방에 빈 공간이 없습니다. 소지품을 비우시겠습니까?");
                        int select = Display.SelectYesOrNo();
                        if (select == 1)
                        {
                            while (true)
                            {
                                var itemSlot = inventory.ShowAndSelectInventory();
                                if (itemSlot != null)
                                {
                                    string itemCountString = itemSlot.currentStack > 1 ? $" x {itemSlot.currentStack}" : "";
                                    Console.WriteLine();
                                    Console.WriteLine($"{itemSlot.Name}{itemCountString} - 이 자리를 비우시겠습니까?");
                                    select = Display.SelectYesOrNo();
                                    if (select == 1)
                                    {
                                        itemSlot.RemoveItem();
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else break;
                            }
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("아이템을 포기합니다.");
                            break;
                        }
                    }
                }
            }
        }
    }
}

public class Reward
{
    public List<int> items;
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

    public Reward GenerateReward()
    {
        Console.WriteLine();
        Console.WriteLine($"=>{monsterUniqueName}의 죽음!!!");
        Reward reward = new Reward();
        reward.exp = data.deathExp;
        reward.gold = Utility.GetRandom(data.minGold, data.maxGold);
        reward.items = new List<int>();
        for (int i = 0; i < data?.itemDropRates?.Count; ++i)
        {
            int dice = Utility.GetRandom(0, 100);
            if (dice <= data.itemDropRates[i])
            {
                reward.items.Add(data.itemDropList[i]);
            }
        }
        return reward;
    }

    public void ActiveTurn(Player player)
    {
        Console.WriteLine();
        Console.WriteLine($"{monsterUniqueName}의 턴입니다!!");

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
        int damage = GameManager.CalculateDamage(null, status, 1);
        Console.WriteLine();
        int atkRatio = GameManager.CalculateAttackHit(status, 0, targetCharacter.Status);
        int dice = Utility.GetRandom(0, 100);
        if (dice <= atkRatio)
        {
            Console.WriteLine($"{monsterUniqueName}의 공격 !!!");
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
        int dmg = damage - status.vitality;
        Console.WriteLine();
        Console.Write($" => {monsterUniqueName}의 ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($" {dmg} ");
        Console.ResetColor();
        Console.WriteLine("데미지 !!!!!");
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            currentHP = 0;
        }
    }
}
