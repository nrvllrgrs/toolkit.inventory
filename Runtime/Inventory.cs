using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Inventory
{
    public abstract class BaseInventoryEventArgs<T> : System.EventArgs
	{
		#region Properties

		public Inventory inventory { get; set; }
		public T slot { get; private set; }
		public int delta { get; private set; }

		#endregion

		#region Constructors

		public BaseInventoryEventArgs(Inventory inventory, T slot)
			: this(inventory, slot, 0)
		{ }

		public BaseInventoryEventArgs(Inventory inventory, T slot, int delta)
		{
			this.inventory = inventory;
			this.slot = slot;
			this.delta = delta;
		}

		#endregion
	}

    public class CurrencyEventArgs : BaseInventoryEventArgs<CurrencySlot>
    {
		#region Constructors

		public CurrencyEventArgs(Inventory inventory, CurrencySlot slot)
            : base(inventory, slot)
		{ }

		public CurrencyEventArgs(Inventory inventory, CurrencySlot slot, int delta)
            : base(inventory, slot, delta)
        { }

		#endregion
	}

    public class ItemEventArgs : BaseInventoryEventArgs<ItemSlot>
    {
		#region Properties

		public float weightDelta => delta * slot.slotType.weight;

		#endregion

		#region Constructors

		public ItemEventArgs(Inventory inventory, ItemSlot slot)
            : base(inventory, slot)
        { }

		public ItemEventArgs(Inventory inventory, ItemSlot slot, int delta)
			: base(inventory, slot, delta)
		{ }

		#endregion
	}

	public class Inventory : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private List<CurrencySlot> m_currencies;

        [SerializeField]
        private List<ItemSlot> m_items;

        [SerializeField, Tooltip("Indicates whether new slots can be added when out of room.")]
        private bool m_expandSlots = true;

        [SerializeField]
        private bool m_useEncumbrance;

        [SerializeField, Min(0f)]
        private float m_encumbranceWeight;

        private float m_weight;

        #endregion

        #region Events

        [SerializeField]
        private UnityEvent<CurrencyEventArgs> m_onCurrencySlotChanged;

        [SerializeField]
        private UnityEvent<ItemEventArgs> m_onItemSlotChanged;

        [SerializeField]
        private UnityEvent<Inventory, bool> m_onEncumbranceChanged;

        #endregion

        #region Properties

        public CurrencySlot[] currencies => m_currencies.ToArray();
        public ItemSlot[] items => m_items.ToArray();

        public float weight
        {
            get => m_weight;
            private set
            {
                if (value == m_weight)
                    return;

                bool wasEncumbered = isEncumbered;

                m_weight = value;

                if (wasEncumbered ^ isEncumbered)
                {
                    m_onEncumbranceChanged?.Invoke(this, isEncumbered);
                }
            }
        }

		public bool isEncumbered => m_weight > m_encumbranceWeight;

		public bool allowAdd
        {
            get => m_expandSlots;
            set => m_expandSlots = value;
        }

        public UnityEvent<CurrencyEventArgs> onCurrencySlotChanged => m_onCurrencySlotChanged;
        public UnityEvent<ItemEventArgs> onItemSlotChanged => m_onItemSlotChanged;
        public UnityEvent<Inventory, bool> onEncumbranceChanged => m_onEncumbranceChanged;

		#endregion

		#region Methods

		private void OnEnable()
        {
            m_weight = 0f;

            foreach (ItemSlot slot in m_items)
            {
                slot.SlotChanged += ItemSlot_SlotChanged;
                weight += slot.weight;
            }

            foreach (CurrencySlot slot in m_currencies)
            {
                slot.SlotChanged += CurrencySlot_SlotChanged;
            }
        }

        private void OnDisable()
        {
            foreach (ItemSlot slot in m_items)
            {
                slot.SlotChanged -= ItemSlot_SlotChanged;
            }

			foreach (CurrencySlot slot in m_currencies)
			{
				slot.SlotChanged -= CurrencySlot_SlotChanged;
			}
		}

		#endregion

		#region Currency Slot Methods

		private void CurrencySlot_SlotChanged(object sender, CurrencyEventArgs e)
		{
            e.inventory = this;
            m_onCurrencySlotChanged?.Invoke(e);
		}

		public int GetCurrencyTotal(CurrencyType currencyType)
		{
			return m_currencies.FirstOrDefault(x => Equals(x.slotType, currencyType))?.amount ?? 0;
		}

        public void AddCurrency(Currency source, bool emptySource = false)
        {
            AddCurrency(source.currencyType, source.amount);

            if (emptySource)
            {
                source.amount = 0;
            }
        }

        public void AddCurrency(CurrencyType currencyType, int amount = 1)
        {
            if (amount <= 0)
                return;

            var currencySlot = m_currencies.FirstOrDefault(x => Equals(x.slotType, currencyType));
            if (currencySlot == null)
            {
				currencySlot = new CurrencySlot(currencyType, amount);
				m_currencies.Add(currencySlot);

				currencySlot.SlotChanged += CurrencySlot_SlotChanged;
                CurrencySlot_SlotChanged(this, new CurrencyEventArgs(this, currencySlot, amount));
            }
            else
            {
                currencySlot.AddToStack(amount);
			}
        }

        public bool TryRemoveCurrency(CurrencyType currencyType, int amount = 1)
        {
            if (amount <= 0)
                return false;

			var currencySlot = m_currencies.FirstOrDefault(x => Equals(x.slotType, currencyType));
            if (currencySlot == null)
                return false;

            currencySlot.RemoveFromStack(amount);
			return true;
        }

        public bool CanBuy(ItemType itemType)
        {
            return itemType.CanBuy(this);
        }

        public bool Buy(ItemType itemType)
        {
            return itemType.Buy(this);
        }

        public bool Buy(ItemType itemType, out int overflow)
        {
            return itemType.Buy(this, out overflow);
		}

		public bool Buy(ItemType itemType, SpawnedAction onSpawnedAction, params object[] args)
		{
            return itemType.Buy(this, onSpawnedAction, args);
		}

		public bool Buy(ItemType itemType, Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
		{
            return Buy(itemType, position, rotation, null, onSpawnedAction, args);
		}

		public bool Buy(ItemType itemType, Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            return itemType.Buy(this, position, rotation, parent, onSpawnedAction, args);
		}

		public bool Buy(ItemType itemType, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Buy(itemType, parent, onSpawnedAction, args);
        }

		public bool Buy(ItemType itemType, Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
		{
            return itemType.Buy(this, parent, instantiateInWorldSpace, onSpawnedAction, args);
		}

        public bool Sell(ItemType itemType)
        {
            return itemType.Sell(this);
        }

		#endregion

		#region Item Slot Methods

		private void ItemSlot_SlotChanged(object sender, ItemEventArgs e)
        {
            weight += e.weightDelta;

            e.inventory = this;
            m_onItemSlotChanged?.Invoke(e);
        }

        public int GetItemTotal() => m_items.Sum(x => x.amount);

        public int GetItemTotal(ItemType itemType)
        {
            return m_items.Where(x => x.id == itemType.id)
                .Sum(x => x.amount);
        }

        public bool HasItem(ItemType itemType)
        {
            if (itemType == null)
                return false;

            foreach (var slot in m_items)
            {
                if (Equals(slot.id, itemType.id) && slot.amount > 0)
                    return true;
            }

            return false;
        }

        public int IndexOf(ItemSlot slot) => m_items.IndexOf(slot);

        public int IndexOf(ItemType item, bool hasAmount = false)
        {
            for (int i = 0; i < m_items.Count; ++i)
            {
                if (m_items[i].id == item.id
                    && !hasAmount || m_items[i].amount > 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public ItemSlot FirstEmptyOrDefault() => FirstEmptyOrDefault(null);

		public ItemSlot FirstEmptyOrDefault(ItemType item)
        {
            if (item == null)
            {
                return m_items.FirstOrDefault(x => x.amount == 0);
            }
            return m_items.FirstOrDefault(x => Equals(x.id, item.id) && x.amount == 0);
        }

        public bool AddItem(Item item, out int overflow)
        {
            return AddItem(item.itemType, item.amount, out overflow);
        }
        
        public bool AddItem(ItemType itemType, int count, out int overflow)
        {
            int initialCount = count;
            overflow = 0;

            // Try to fill existing slots before moving to empty slots
            foreach (var slot in m_items.Where(x => x.id == itemType.id))
            {
                // Enough room in the slot
                if (slot.CanAdd(count, out int remaining))
                {
                    slot.AddToStack(count);
                    return true;
                }

                int fillCount = Mathf.Min(remaining, count);

                // Fill slot as best we can
                slot.AddToStack(fillCount);
                count -= fillCount;

                if (count == 0)
                    return true;
            }

            foreach (var slot in m_items.Where(x => x.id == string.Empty))
            {
                int fillCount = Mathf.Min(count, itemType.maxStack);
                slot.Set(itemType, fillCount);
                count -= fillCount;

                if (count == 0)
                    return true;
            }

			if (m_expandSlots)
			{
                int slotCount = Mathf.CeilToInt(count / itemType.maxStack);
				for (int i = 0; i < slotCount; ++i)
                {
                    // Add new slot to inventory
                    var slot = new ItemSlot(itemType, 0);
					slot.SlotChanged += ItemSlot_SlotChanged;
					weight += slot.weight;
					m_items.Add(slot);

                    // Add items to slot
                    slot.AddToStack(Mathf.Min(count, itemType.maxStack));
                    count -= slot.amount;
                }

				overflow = 0;
                return true;
			}

			overflow = count;
            return initialCount != count;
        }

        public bool TryRemoveItem(ItemSlot slot, int amount = 1)
        {
            if (amount <= slot.amount)
            {
                slot.RemoveFromStack(amount);
                return true;
            }
            return false;
        }

		public bool TryRemoveItem(Item item)
        {
			return TryRemoveItem(item?.itemType, item?.amount ?? 1);
		}

		public bool TryRemoveItem(Item item, int amount = 1)
        {
            return TryRemoveItem(item?.itemType, amount);
        }

		public bool TryRemoveItem(ItemType item, int amount = 1)
        {
            if (item == null)
                return false;

            var slots = m_items.Where(i => Equals(i.slotType.id, item.id));

            if (amount > slots.Sum(x => x.amount))
                return false;

            foreach (ItemSlot slot in slots)
            {
                int removeCount = Mathf.Min(slot.amount, amount);

                TryRemoveItem(slot, removeCount);
                amount -= removeCount;

                if (amount == 0)
                    break;
            }

            return true;
        }

        public void ClearItems()
        {
            foreach (ItemSlot slot in m_items)
            {
                TryRemoveItem(slot, slot.amount);
            }
        }

        public void AddSlot(ItemSlot slot)
        {
            if (m_items.Contains(slot))
                return;

			slot.SlotChanged += ItemSlot_SlotChanged;
			weight += slot.weight;

			m_items.Add(slot);
			m_onItemSlotChanged?.Invoke(new ItemEventArgs(this, slot));
		}

        public void RemoveSlot(ItemSlot slot)
        {
            if (!m_items.Contains(slot))
                return;

			slot.SlotChanged -= ItemSlot_SlotChanged;
			weight -= slot.weight;

            m_items.Remove(slot);
            m_onItemSlotChanged?.Invoke(new ItemEventArgs(this, slot));
		}

		#endregion

		#region Drop Methods

		public void AddDrop(DropEntry drop)
		{
			if (drop == null)
				return;

			int amount = drop.GetAmount();

			switch (drop.dropType)
			{
				case DropEntry.DropType.Item:
					AddItem(drop.itemType, amount, out int overflow);
					break;

				case DropEntry.DropType.Currency:
					AddCurrency(drop.currencyType, amount);
					break;

				case DropEntry.DropType.LootTable:
					foreach (var loot in drop.lootTable.Get(amount))
					{
						AddDrop(loot);
					}
					break;
			}
		}

		#endregion
	}
}