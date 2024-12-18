using System;

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

	// 호출 되었다는건 이미 소비할 수 있는 아이템이라는 판단 후
	public void UseItem(List<Character> targets)
	{
        ConsumableItemData cData = DataTables.GetConsumeItemData(itemData.itemID);
        UsableItem item = new UsableItem(cData);

		foreach (var target in targets)
		{
			item.Use(target, cData.itemParams);
        }

		currentStack--;
		if (currentStack <= 0)
		{
			RemoveItem();
        }
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
				var itemData = DataTables.GetItemData(4);
				itemSlots[i].AddItem(itemData, 1);
			}
			else
			{
				var itemData = DataTables.GetItemData(0);
				itemSlots[i].AddItem(itemData, 19);
			}
		}
	}

    public bool AddItem(ItemData data, int count)
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
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write($"{data.name} "); Console.ResetColor(); Console.WriteLine("획득!!!");
            // 아이템 추가
            slot.AddItem(data, count);
			return true;
		}
		// 없다면
		else
		{
			// 아이템 추가 실패
			return false;
		}
    }

	public void UseItem(int slotIndex, List<Character> targets)
	{
		if(itemSlots[slotIndex].IsUsable())
		{
			itemSlots[slotIndex].UseItem(targets);
        }
	}

	public ItemSlot ShowAndSelectInventory()
	{
		List<string> itemSlotStrings = new List<string>();
		for(int i = 0; i < itemSlots.Count; ++i)
		{
			string slotString = $"{i+1}. ";
			slotString += itemSlots[i].IsEmpty ? "---- 비어있음 ----" : itemSlots[i].Name;
			itemSlotStrings.Add(slotString);
        }

		itemSlotStrings.Add("0. 돌아가기");

		Console.WriteLine();
		Console.WriteLine("버릴 아이템을 선택해주세요");
        int selectedSlotIndex = Display.SelectInput("---------- 인벤토리 ---------", itemSlotStrings.ToArray(), true);
		if (selectedSlotIndex <= 0)
		{
			return null;
		}
		else
		{
			return itemSlots[selectedSlotIndex - 1];
        }
	}
}
