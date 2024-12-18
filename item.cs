using System;
public class Item
{
	ItemData data;
	public Item(ItemData data)
    {
        this.data = data;
    }
}

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

public class ConsumableItem
{
	protected ConsumableItemData data;
	public ConsumableItem(ConsumableItemData data)
	{
		this.data = data;
	}
}

public class UsableItem : ConsumableItem
{
    public UsableItem(ConsumableItemData data) : base(data)
    {
    }

	public void Use(Character character, List<int> param)
	{
		if (data.itemTarget == CosumableItemTargetType.Player)
		{
			switch(data.useType)
			{
				case ItemUseType.RecoverHP:
					character.RecoverHP(param[0]);
                    break;
				case ItemUseType.RecoverMP:
					character.RecoverMP(param[0]);
					break;
				case ItemUseType.Damage:
                case ItemUseType.AreaDamage:
					character.GetDamage(param[0]);
					break;
            }
		}
	}
}
