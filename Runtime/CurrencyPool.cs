using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Inventory
{
    public class CurrencyPool : MonoBehaviour, ICurrency
	{
		#region Fields

		[SerializeField]
		private CurrencyType m_currencyType;

		[SerializeField]
		private bool m_autoManaged = false;

		[SerializeField, Min(0)]
		private int m_amount;

		private int m_prevAmount;

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent<int> m_onAmountChanged;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onCapacityEntered;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onCapacityExited;

		#endregion

		#region Properties

		public CurrencyType currencyType => m_currencyType;

		public int amount
		{
			get => InventoryManager.GetAmount(m_currencyType);
			set
			{
				// No change, skip
				if (amount == value)
					return;

				InventoryManager.SetAmount(m_currencyType, value);
			}
		}

		public UnityEvent<int> onAmountChanged => m_onAmountChanged;
		public UnityEvent onCapacityEntered => m_onCapacityEntered;
		public UnityEvent onCapacityExited => m_onCapacityExited;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_autoManaged)
			{
				InventoryManager.Register(m_currencyType, m_amount);
				CurrencyPoolValueChanged(currencyType);
				m_prevAmount = amount;
			}
			else if (InventoryManager.TryGetAmount(m_currencyType, out int amount))
			{
				m_prevAmount = amount;
			}
		}

		private void OnDestroy()
		{
			if (m_autoManaged)
			{
				InventoryManager.Unregister(m_currencyType);
			}
		}

		private void OnEnable()
		{
			InventoryManager.CurrencyPoolAmountChanged += CurrencyPoolValueChanged;
		}

		private void OnDisable()
		{
			InventoryManager.CurrencyPoolAmountChanged -= CurrencyPoolValueChanged;
		}

		public void Add(int amount = 1)
		{
			InventoryManager.ModifyAmount(m_currencyType, amount);
		}

		public void Remove(int amount = 1)
		{
			InventoryManager.ModifyAmount(m_currencyType, -amount);
		}

		private void CurrencyPoolValueChanged(CurrencyType currencyType)
		{
			if (currencyType != m_currencyType)
				return;

			int amount = this.amount;
			m_onAmountChanged?.Invoke(amount);

			int effectiveMaxStack = m_currencyType.effectiveMaxStack;
			if (effectiveMaxStack > 0)
			{
				if (amount == effectiveMaxStack)
				{
					m_onCapacityEntered?.Invoke();
				}
				else if (m_prevAmount == effectiveMaxStack)
				{
					m_onCapacityExited?.Invoke();
				}
			}
			m_prevAmount = amount;
		}

		#endregion
	}
}