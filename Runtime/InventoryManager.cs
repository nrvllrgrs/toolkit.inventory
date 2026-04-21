using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	public class InventoryManager : ConfigurableSubsystem<InventoryManager, InventoryManagerConfig>
    {
		#region Fields

		private Dictionary<string, InventoryList> m_inventories = new();
		private Dictionary<CurrencyType, int> m_currencyPools = new();
		private Dictionary<CurrencyType, int> m_maxStackOverride = new();

		#endregion

		#region Events

		public static Action<CurrencyType> CurrencyPoolAmountChanged;
		public static Action<CurrencyType> CurrencyMaxStackOverrideChanged;

		#endregion

		#region Properties

		public string[] keys => m_inventories.Keys.ToArray();

		#endregion

		#region Methods

		public void Instantiate()
		{
			IInstantiableSubsystem.Instantiate(Config?.template);
		}

		public void Register(KeyedInventoryList item)
		{
			if (!m_inventories.ContainsKey(item.key))
			{
				m_inventories.Add(item.key, item.value);
			}
		}

		public void Unregister(KeyedInventoryList item)
		{
			if (m_inventories.ContainsKey(item.key))
			{
				m_inventories.Remove(item.key);
			}
		}

		public bool TryGetInventory(string key, out InventoryList inventory)
		{
			inventory = null;
			return !string.IsNullOrWhiteSpace(key)
				&& m_inventories.TryGetValue(key, out inventory);
		}

		#endregion

		#region CurrencyPool Methods

		public static void Register(CurrencyType currencyType, int amount)
		{
			if (currencyType == null)
				return;

			if (CastInstance.m_currencyPools.ContainsKey(currencyType))
			{
				Debug.Log($"CurrencyPool {currencyType.name} already exists.");
				return;
			}

			CastInstance.m_currencyPools.Add(currencyType, amount);
			CurrencyPoolAmountChanged?.Invoke(currencyType);
		}

		public static void Unregister(CurrencyType currencyType)
		{
			if (!Exists)
				return;

			if (currencyType == null)
				return;

			CastInstance.m_currencyPools.Remove(currencyType);
			CurrencyPoolAmountChanged?.Invoke(currencyType);
		}
		
		public static bool IsRegistered(CurrencyType currencyType) => CastInstance.m_currencyPools.ContainsKey(currencyType);

		public static bool TryGetAmount(CurrencyType currencyType, out int amount)
		{
			amount = 0;
			if (!Exists)
				return false;

			amount = 0;
			return currencyType != null && CastInstance.m_currencyPools.TryGetValue(currencyType, out amount);
		}

		public static int GetAmount(CurrencyType currencyType)
		{
			return TryGetAmount(currencyType, out int amount)
				? amount
				: 0;
		}

		public static void SetAmount(CurrencyType currencyType, int amount)
		{
			if (currencyType == null || !CastInstance.m_currencyPools.TryGetValue(currencyType, out int currAmount))
				return;

			if (!currencyType.SetAmount(ref currAmount, amount))
				return;

			CastInstance.m_currencyPools[currencyType] = currAmount;
			CurrencyPoolAmountChanged?.Invoke(currencyType);
		}

		public static void ModifyAmount(CurrencyType currencyType, int delta)
		{
			if (!TryGetAmount(currencyType, out var amount))
			{
				amount = 0;
			}
			SetAmount(currencyType, amount + delta);
		}

		public static bool HasMaxStackOverride(CurrencyType currencyType) => CastInstance.m_maxStackOverride.ContainsKey(currencyType);

		public static bool TryGetMaxStackOverride(CurrencyType currencyType, out int maxStack)
		{
			return CastInstance.m_maxStackOverride.TryGetValue(currencyType, out maxStack);
		}

		public static int GetMaxStackOverride(CurrencyType currencyType)
		{
			return TryGetMaxStackOverride(currencyType, out int maxStack)
				? maxStack
				: 0;
		}

		public static void SetMaxStackOverride(CurrencyType currencyType, int maxStack)
		{
			if (CastInstance.m_maxStackOverride.TryGetValue(currencyType, out int value))
			{
				if (value != maxStack)
				{
					CastInstance.m_maxStackOverride[currencyType] = maxStack;
					CurrencyMaxStackOverrideChanged?.Invoke(currencyType);
				}
			}
			else
			{
				CastInstance.m_maxStackOverride.Add(currencyType, maxStack);
				CurrencyMaxStackOverrideChanged?.Invoke(currencyType);
			}

			// Re-clamp current pool to new cap
			if (TryGetAmount(currencyType, out int current))
			{
				SetAmount(currencyType, current);
			}
		}

		public static bool ClearMaxStackOverride(CurrencyType currencyType)
		{
			if (CastInstance.m_maxStackOverride.Remove(currencyType))
			{
				// Re-clamp current pool to new cap
				if (TryGetAmount(currencyType, out int current))
				{
					SetAmount(currencyType, current);
				}

				CurrencyMaxStackOverrideChanged?.Invoke(currencyType);
				return true;
			}
			return false;
		}

		public static void ClearMaxStackOverride()
		{
			var keys = CastInstance.m_maxStackOverride.Keys.ToArray();
			CastInstance.m_maxStackOverride.Clear();

			foreach (var key in keys)
			{
				CurrencyMaxStackOverrideChanged?.Invoke(key);
			}
		}

		#endregion
	}
}