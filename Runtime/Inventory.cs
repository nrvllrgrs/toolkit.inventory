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

        [SerializeField, Tooltip("Indicates whether inventory can have infinite slots.")]
        private bool m_infiniteSlots = false;

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

        /// <summary>
        /// Indicates whether inventory is currently encumbered.
        /// </summary>
		public bool isEncumbered => m_weight > m_encumbranceWeight;

		/// <summary>
		/// Indicates whether inventory can have infinite slots.
		/// </summary>
		public bool infiniteSlots => m_infiniteSlots;

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

        public int GetItemTotal(ItemType itemType)
        {
            return m_items.Where(x => x.id == itemType.id)
                .Sum(x => x.amount);
        }

        public bool AddItem(Item item)
        {
            return AddItem(item, out int overflow);
        }

        public bool AddItem(Item item, out int overflow)
        {
            return AddItem(item.itemType, item.amount, out overflow);
        }

        public bool AddItem(ItemType itemType, int count)
        {
            return AddItem(itemType, count, out int overflow);
        }

        public bool AddItem(ItemType itemType, int count, out int overflow)
        {
            int initialCount = count;
            overflow = 0;

            // trying to fill existing slots before moving to empty slots
            foreach (var slot in m_items.Where(x => x.id == itemType.id))
            {
                // enough room in the slot
                if (slot.CanAdd(count, out int remaining))
                {
                    slot.AddToStack(count);
                    return true;
                }

                int fillCount = Mathf.Min(remaining, count);

                // fill slot as best we can
                slot.AddToStack(fillCount);
                count -= fillCount;

                if (count == 0)
                    return true;
            }

            if (!m_infiniteSlots)
            {
                foreach (var slot in m_items.Where(x => x.id == string.Empty))
                {
                    int fillCount = Mathf.Min(count, itemType.maxStack);
                    slot.Set(itemType, fillCount);
                    count -= fillCount;

                    if (count == 0)
                        return true;
                }
            }
            else
            {
                while (count > 0)
                {
                    var slot = new ItemSlot(itemType, Mathf.Min(count, itemType.maxStack));
                    slot.SlotChanged += ItemSlot_SlotChanged;
					weight += slot.weight;

					m_items.Add(slot);

                    // Don't forget to decrement to exit loop
                    count -= slot.amount;
                }
            }

            overflow = count;
            return initialCount != count;
        }

        public bool TryRemoveItem(ItemSlot slot, int amount = 1)
        {
            if (amount <= slot.amount)
            {
                slot.RemoveFromStack(amount);

                if (m_infiniteSlots && slot.amount == 0)
                {
                    slot.SlotChanged -= ItemSlot_SlotChanged;
                    m_items.Remove(slot);
                }

                return true;
            }
            return false;
        }

        public bool TryRemoveItem(ItemType item, int amount = 1)
        {
            var slots = m_items.Where(i => i.slotType == item);

            if (slots.Sum(x => x.amount) > amount)
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

		#endregion
	}
}