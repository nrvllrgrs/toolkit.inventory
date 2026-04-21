using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using NPOI.SS.Formula.Functions;


#if USE_UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace ToolkitEngine.Inventory
{
	[CreateAssetMenu(menuName = "Toolkit/Inventory/Currency")]
	public class CurrencyType : ScriptableObject
	{
		#region Fields

		[SerializeField, ReadOnly]
		private string m_id = Guid.NewGuid().ToString();

#if USE_UNITY_LOCALIZATION
		[SerializeField]
		private LocalizedString m_localizedName;

		[SerializeField]
		private LocalizedString m_localizedDescription;
#else
        [SerializeField]
        private string m_name;

        [SerializeField, TextArea]
        private string m_description;
#endif

		[SerializeField]
		private Sprite m_icon;

		[SerializeField]
		private PropertyCollection m_properties = new();

		[SerializeField, Min(0)]
		[Tooltip("Maximum capacity of currency (0 is infinite)")]
		private int m_maxStack = 0;

		#endregion

		#region Properties

		public string id => m_id;

		/// <summary>
		/// Name of asset
		/// </summary>
		public string assetName
		{
			get => base.name;
			set => base.name = value;
		}

		public new string name
		{
			get
			{
#if USE_UNITY_LOCALIZATION
				if (m_localizedName != null && !m_localizedName.IsEmpty)
				{
					try
					{
						return m_localizedName.GetLocalizedString();
					}
					catch (KeyNotFoundException)
					{
						Debug.LogWarning($"Localization key not found for {base.name} name, falling back to asset name");
						return base.name;
					}
				}
				return base.name;
#else
                return m_name;
#endif
			}
#if UNITY_EDITOR && !USE_UNITY_LOCALIZATION
            set => m_name = value;
#endif
		}

		public string description
		{
			get
			{
#if USE_UNITY_LOCALIZATION
				if (m_localizedDescription != null && !m_localizedDescription.IsEmpty)
				{
					try
					{
						return m_localizedDescription.GetLocalizedString();
					}
					catch (KeyNotFoundException)
					{
						Debug.LogWarning($"Localization key not found for {base.name} description");
						return string.Empty;
					}
				}
				return string.Empty;
#else
                return m_description;
#endif
			}
#if UNITY_EDITOR && !USE_UNITY_LOCALIZATION
            set => m_description = value;
#endif
		}

#if USE_UNITY_LOCALIZATION
		public LocalizedString localizedName { get => m_localizedName; set => m_localizedName = value; }
		public LocalizedString localizedDescription { get => m_localizedDescription; set => m_localizedDescription = value; }
#endif
		public Sprite icon => m_icon;
		public PropertyCollection properties => m_properties;

		public int maxStack
		{
			get => m_maxStack;
			set => m_maxStack = value;
		}

		public int effectiveMaxStack
		{
			get
			{
				return InventoryManager.TryGetMaxStackOverride(this, out int overrideMaxStack)
					? overrideMaxStack
					: maxStack;
			}
		}

		#endregion

		#region Methods

		public bool SetAmount(ref int amount, int value)
		{
			int effectiveMaxStack = this.effectiveMaxStack;
			if (effectiveMaxStack > 0)
			{
				// Clamp to maximum
				value = Mathf.Min(value, effectiveMaxStack);
			}
			value = Mathf.Max(value, 0);

			// No change, skip
			if (amount == value)
				return false;

			amount = value;
			return true;
		}

		public bool ModifyAmount(ref int amount, int delta)
		{
			return SetAmount(ref amount, amount + delta);
		}

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
