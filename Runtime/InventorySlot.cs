using NaughtyAttributes;
using System;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
    public abstract class BaseSlot<T>
        where T : ScriptableObject
    {
		#region Fields

		[SerializeField, Required]
		protected T m_slotType;

		[SerializeField, Min(0)]
		protected int m_amount = default;

		#endregion

		#region Properties

		public T slotType => m_slotType;

		/// <summary>
		/// Current number of items in stack
		/// </summary>
		public virtual int amount
		{
			get => m_amount;
			protected set
			{
				if (value == m_amount)
					return;

				int delta = value - m_amount;
				m_amount = value;

                InvokeSlotChanged(delta);
			}
		}

        #endregion

        #region Methods

        protected abstract void InvokeSlotChanged(int delta);

		public virtual void Clear()
		{
			m_amount = 0;
		}

		public void Set(T slotType, int amount = 1)
		{
			m_slotType = slotType;
			this.amount = amount;
		}

		public void AddToStack(int amount = 1)
		{
			this.amount += amount;
		}

		public void RemoveFromStack(int amount = 1)
		{
			this.amount -= amount;

			if (this.amount == 0)
			{
				Clear();
			}
		}

		// splits a stack and returns a new stack
		public bool SplitStack(out T splitStack)
		{
			if (m_amount <= 1) // is splitting even possible? if not return false
			{
				splitStack = null;
				return false;
			}

			int halfStack = Mathf.RoundToInt(m_amount / 2); // get half, remove half
			RemoveFromStack(halfStack);

			//splitStack = new T(m_itemType, halfStack); // new stack with half
			splitStack = default;
			return true;
		}

		#endregion
	}

	[Serializable]
    public class ItemSlot : BaseSlot<ItemType>
    {
        #region Events

		public event EventHandler<ItemEventArgs> SlotChanged;

		#endregion

		#region Properties

        public string id => m_slotType?.id ?? null;

		public override int amount
		{
			protected set => base.amount = Mathf.Clamp(value, 0, m_slotType?.maxStack ?? 0);
		}

        public float weight => m_slotType.weight * m_amount;

		#endregion

		#region Constructors

		// Constructor for empty slot
		public ItemSlot()
			: this(null, 0)
		{ }

		// Constructor for occupied slot
		public ItemSlot(ItemType source, int amount)
        {
            Set(source, amount);
        }

        #endregion

        #region Methods

        protected override void InvokeSlotChanged(int delta)
        {
			SlotChanged?.Invoke(this, new ItemEventArgs(null, this, delta));
		}

		public bool CanAdd(int amountToAdd, out int amountRemaining)
		{
			amountRemaining = m_slotType.maxStack - m_amount;
			return CanAdd(amountToAdd);
		}

		public bool CanAdd(int amountToAdd)
		{
			return m_amount + amountToAdd <= m_slotType.maxStack;
		}

		public override void Clear()
		{
			base.Clear();
			m_slotType = null;
		}

		#endregion
	}

    [Serializable]
    public class CurrencySlot : BaseSlot<CurrencyType>
	{
		#region Events

		public event EventHandler<CurrencyEventArgs> SlotChanged;

		#endregion

		#region Constructors

		// Constructor for empty slot
		public CurrencySlot()
			: this(null, 0)
		{ }

		// Constructor for occupied slot
		public CurrencySlot(CurrencyType source, int amount)
		{
			Set(source, amount);
		}

		#endregion

		#region Methods

		protected override void InvokeSlotChanged(int delta)
        {
            SlotChanged?.Invoke(this, new CurrencyEventArgs(null, this, delta));
        }

        #endregion
    }
}