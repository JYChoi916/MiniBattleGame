using System;

public class Reward
{
    public List<int> items;
    public int gold;
    public int exp;
}

public class Monster : Character, IAttackable
{
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
        for (int i = 0; i < data.skillRatioList.Count; ++i)
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
