using System;
using System.Text;

public class ItemSlot
{
	public int currentStack;
	ItemData itemData;
	public ItemData GetItemData()
	{
		return itemData; ;
	}

    public ItemSlot()
	{
		currentStack = 0;
		itemData = null;
	}

	public bool IsEmpty
	{
		get { return itemData == null;  }
	}

	public string Name
	{
		get { return itemData.name; }
	}

	public string ItemID { get { return itemData != null ? itemData.itemID : ""; } }

	public bool IsWeapon()
	{
        if (itemData == null) return false;

		if (itemData.type == ItemType.Weapon)
		{
			return true;
		}

		return false;
    }

	public bool IsArmor()
	{
        if (itemData == null) return false;

        if (itemData.type == ItemType.Armor)
        {
            return true;
        }

        return false;
    }

    public bool IsShield()
    {
        if (itemData == null) return false;

        if (itemData.type == ItemType.Shield)
        {
            return true;
        }

        return false;
    }

    public bool IsUsable()
	{
		if (itemData == null) return false;

		if (itemData.type == ItemType.Consumable)
		{
			ConsumableItemData cData = DataTables.GetConsumeItemData(itemData.itemID);
			return cData.usable;
		}

		return false; ;
	}

	public bool UseItem(Player player, List<Character> targets)
	{
		bool itemUsed = false;
        if (IsUsable())
        {
            ConsumableItemData cData = DataTables.GetConsumeItemData(itemData.itemID);
            UsableItem item = new UsableItem(cData);

            if (targets == null)
			{
				targets = new List<Character>();
				targets.Add(player);
            }

            for (int i = 0; i < targets.Count; ++i)
            {
                string str = $"{player.GetName()}, ";
                str += player == targets[i] ? "자신" : $"{targets[i].GetName()}";

                Console.WriteLine($"{str}에게 {itemData.name} 사용!!");
                int paramIndex = i == 0 ? 0 : Math.Min(i, cData.itemParams.Count - 1);
                item.Use(targets[i], cData.itemParams[paramIndex]);
				itemUsed = true;
            }

            currentStack--;
            if (currentStack <= 0)
            {
                RemoveItem();
            }
        }
        else if (IsEquipable())
        {
            Console.WriteLine("장비 아이템은 아직 구현되지 않았습니다.");
        }
        else
        {
            Console.WriteLine($"{Name} : 이 아이템은 사용 할 수 없는 아이템 입니다.");
        }

        Console.WriteLine();
        Console.WriteLine("키를 눌러 진행하세요...");
        Console.ReadKey();

		return itemUsed;
    }

	public bool IsEquipable ()
	{
		if (itemData == null) return false;

		if (itemData.type == ItemType.Weapon || itemData.type == ItemType.Armor || itemData.type == ItemType.Shield)
			return true;
		else
			return false;
	}

	public void AddItem (ItemData data, int count)
	{
		if (itemData == null)
		{
			this.itemData = data;
			currentStack = count;
			return;
		}
	}

	public void RemoveItem()
	{
        currentStack = 0;
        itemData = null;
    }
}

public class Inventory
{
	List<ItemSlot> itemSlots;
	public Inventory(int slotSize)
	{
		itemSlots = new List<ItemSlot>();
        for(int i = 0; i < slotSize; ++i)
		{
			itemSlots.Add(new ItemSlot());
		}
    }

	public void SetTestInventory()
	{
        for (int i = 0; i < itemSlots.Count; i++) {
			if (i < itemSlots.Count - 1) {
				var itemData = DataTables.GetItemData(Utility.GetRandom(4, 17));
				itemSlots[i].AddItem(itemData, 1);
			}
			else
			{
				var itemData = DataTables.GetItemData(0);
				itemSlots[i].AddItem(itemData, 19);
			}
		}
	}

    public ItemSlot AddItem(ItemData data, int count, bool showMessage = true)
	{
		ItemSlot slot = null;

		// 같은 아이템ID에서 스택을 쌓을 수 있는지 확인
		foreach(var s in itemSlots)
		{
			if (s.ItemID == data.itemID && s.currentStack < data.maxStack)
			{
				slot = s;
				break;
			}
		}

        // 없다면 
        if (slot == null)
		{
            // 빈 아이템 슬롯이 있는지 확인
            slot = itemSlots.FirstOrDefault(x => x.IsEmpty);
        }

		// 스택을 쌓을 수 있거나 빈 슬롯이 있다면
		if (slot != null)
		{
			if (showMessage)
			{
				Console.Write("인벤토리에 "); Console.ForegroundColor = ConsoleColor.Yellow; Console.Write($"{data.name} "); Console.ResetColor(); Console.WriteLine("추가.");
			}
            // 아이템 추가
            slot.AddItem(data, count);
			return slot;
		}
		// 없다면
		else
		{
			// 아이템 추가 실패
			return null;
		}
    }

	public ItemSlot ShowAndSelectInventory(ItemType type)
	{
		List<string> itemSlotStrings = new List<string>();
		string slotString = "";
		List<ItemSlot> categorySlots = new List<ItemSlot>();
		int number = 1;
        for (int i = 0; i < itemSlots.Count; ++i)
		{
			if (type == ItemType.Weapon)
			{
				if (itemSlots[i].IsEmpty || itemSlots[i].IsWeapon() == false)
					continue;
            }
			else if (type == ItemType.Armor)
			{
                if (itemSlots[i].IsEmpty || itemSlots[i].IsArmor() == false)
                    continue;
            }
			else if (type == ItemType.Shield)
			{
                if (itemSlots[i].IsEmpty || itemSlots[i].IsShield() == false)
                    continue;
            }
            else if (type == ItemType.Consumable)
			{
                if (itemSlots[i].IsEmpty || itemSlots[i].IsUsable() == false)
                    continue;
            }

            slotString += $"{number,2}. ";

			string itemInfo = $"{itemSlots[i].Name}" + $" x {itemSlots[i].currentStack}";

			slotString += itemSlots[i].IsEmpty ? "---- 비어있음 ----" : $"{itemInfo}";

			itemSlotStrings.Add(slotString);
			slotString = "";

			categorySlots.Add(itemSlots[i]);

            number++;
        }

        itemSlotStrings.Add(" 0. 돌아가기");
		Utility.MakeSameLengthStrings(itemSlotStrings);

		var itemSlotList = type != ItemType.All ? categorySlots : itemSlots;

        Console.WriteLine();
        int selectedSlotIndex = Display.SelectInput("---------- 인벤토리 ---------", itemSlotStrings.ToArray(), itemSlotList.Count, true, true, "아이템을 선택하세요 : ");
		if (selectedSlotIndex <= 0)
		{
			return null;
		}
		else
		{
			return type != ItemType.All ? itemSlotList[selectedSlotIndex - 1] : itemSlots[selectedSlotIndex - 1];
        }
	}

	public int ShowEquipmens(Equipments equipments)
	{
        List<string> equipmentsStrings = new List<string>();
		string itemname = equipments.GetEquiptedItemName(EquipmentType.Weapon);
		equipmentsStrings.Add($"1. 무기 : {itemname}");
        itemname = equipments.GetEquiptedItemName(EquipmentType.Armor);
        equipmentsStrings.Add($"2. 갑옷 : {itemname}");
        itemname = equipments.GetEquiptedItemName(EquipmentType.Shield);
        equipmentsStrings.Add($"3. 방패 : {itemname}");
		equipmentsStrings.Add("0. 돌아가기");
        Utility.MakeSameLengthStrings(equipmentsStrings);

        return Display.SelectInput("", equipmentsStrings.ToArray(), equipmentsStrings.Count, true, false, "장비 종류를 선택하세요 : ") - 1;
    }
}
