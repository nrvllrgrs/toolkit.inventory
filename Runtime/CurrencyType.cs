using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ToolkitEngine.Inventory
{
	[CreateAssetMenu(menuName = "Toolkit/Inventory/Currency")]
	public class CurrencyType : ScriptableObject
	{
		#region Fields

		[SerializeField, ReadOnly]
		private string m_id = Guid.NewGuid().ToString();

		[SerializeField]
		private string m_name;

		[SerializeField, TextArea]
		private string m_description;

		[SerializeField]
		private Sprite m_icon;

		[SerializeField]
		private Color m_color = Color.white;

		#endregion

		#region Properties

		public string id => m_id;
		public new string name => m_name;
		public string description => m_description;
		public Sprite icon => m_icon;
		public Color color => m_color;

		#endregion

		#region Methods

		public override bool Equals(object other)
		{
			if (other == null)
				return false;

			if (other is CurrencyType otherCurrencyType)
				return m_id == otherCurrencyType.id;

			return false;
		}

		public override int GetHashCode()
		{
			return m_id.GetHashCode();
		}

		#endregion
	}
}
