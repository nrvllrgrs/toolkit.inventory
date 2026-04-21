using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Inventory
{
	public class Currency : MonoBehaviour, ICurrency
	{
		#region Fields

		[SerializeField]
		private CurrencyType m_currencyType;

		[SerializeField, Min(0)]
		private int m_amount = 1;

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent<int> m_onAmountChanged;

		#endregion

		#region Properties

		public CurrencyType currencyType
		{
			get => m_currencyType;
			set => m_currencyType = value;
		}

		public int amount
		{
			get => m_amount;
			set
			{
				// No change, skip
				if (m_amount == value)
					return;

				m_amount = value;
				m_onAmountChanged?.Invoke(m_amount);
			}
		}

		public UnityEvent<int> onAmountChanged => m_onAmountChanged;

		#endregion
	}

	public interface ICurrency
	{
		public CurrencyType currencyType { get; }
		public int amount { get; set; }
		public UnityEvent<int> onAmountChanged { get; }
	}
}