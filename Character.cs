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