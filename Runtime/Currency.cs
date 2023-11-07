using UnityEngine;

namespace ToolkitEngine.Inventory
{
	public class Currency : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private CurrencyType m_currencyType;

		[SerializeField, Min(1)]
		private int m_amount = 1;

		#endregion

		#region Properties

		public CurrencyType currencyType => m_currencyType;

		public int amount
		{
			get => m_amount;
			set => m_amount = value;
		}

		#endregion
	}
}