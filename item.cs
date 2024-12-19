using System;

public class Equipment
{
	public Weapon weapon;
	public Shield shield;
	public Armor armor;
}

public class Weapon
{
	WeaponData data;
    public Weapon(WeaponData data)
	{
		this.data = data;
	}
}

public class Shield
{
	ShieldData data;
}

public class Armor
{
	ArmorData data;
}

public class UsableItem
{
    ConsumableItemData data;
    public UsableItem(ConsumableItemData data)
    {
        this.data = data;
    }

	public void Use(Character character, int param)
	{
		if (data.itemTarget == CosumableItemTargetType.Player)
		{
			switch(data.useType)
			{
				case ItemUseType.RecoverHP:
					character.RecoverHP(param);
                    break;
				case ItemUseType.RecoverMP:
					character.RecoverMP(param);
					break;
				case ItemUseType.Damage:
                case ItemUseType.AreaDamage:
					character.GetDamage(param);
					break;
            }
		}
    }
}
