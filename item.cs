using System;

public class Equipments
{
    List<EquipmentSlot> equiptedItems = new List<EquipmentSlot>();

    public Equipments()
    {
        for (int i = 0; i < 3; ++i)
            equiptedItems.Add(new EquipmentSlot());
    }

    public EquipmentSlot GetEquipmentSlot(EquipmentType type)
    {
        return equiptedItems[(int)type];
    }

    public string GetEquiptedItemName(EquipmentType type)
    {
        if (equiptedItems[(int)type].equipedItem == null)
            return "-- 미착용 --";

        return equiptedItems[(int)type].equipedItem.data.name;
    }

    public void EquipItem(ItemSlot slot, EquipmentType type)
    {
        var eSlot = equiptedItems[(int)type];
        if (eSlot.equipedItem == null)
        {
            EquipmentItem item = null;
            switch (type)
            {
                case EquipmentType.Weapon:
                    item = new WeaponItem(slot.GetItemData());
                    break;
                case EquipmentType.Armor:
                    item = new ArmorItem(slot.GetItemData());
                    break;
                case EquipmentType.Shield:
                    item = new ShieldItem(slot.GetItemData());
                    break;
            }
            eSlot.equipedItem = item;
        }
        else
        {
            var data = eSlot.equipedItem.data;
            eSlot.equipedItem.data = slot.GetItemData();
            slot.RemoveItem();
            slot.AddItem(data, 1);
        }
    }
}

public class EquipmentSlot
{
    public EquipmentItem equipedItem;
    public EquipmentSlot()
    {
        equipedItem = null;
    }
}

public class EquipmentItem
{
    public ItemData data;
	public EquipmentItem(ItemData data)
	{
		this.data = data;
	}
    public ItemData GetData()
    {
        return data;
    }
}

public class WeaponItem : EquipmentItem
{
    public WeaponItem(ItemData data) : base(data)
    {
    }

    public WeaponData GetWeaponData()
    {
        return DataTables.GetWeaponData(data.itemID);
    }
}

public class ShieldItem : EquipmentItem
{
    public ShieldItem(ItemData data) : base(data)
    {
    }
}

public class ArmorItem : EquipmentItem
{
    public ArmorItem(ItemData data) : base(data)
    {
    }
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
