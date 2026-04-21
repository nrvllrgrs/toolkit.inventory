using UnityEngine;
using ToolkitEngine.Inventory;
using System.Collections.Generic;
using System;

namespace ToolkitEngine.SaveManagement.Inventory
{
	/// <summary>
	/// Companion component that persists an InventoryList to the SaveManager.
	/// Attach alongside an InventoryList and supply a save ID that is unique
	/// across the scene. No SaveDefinition entry is required — this component
	/// registers its own key dynamically via SaveManager.Add.
	/// </summary>
	[RequireComponent(typeof(InventoryList))]
	public class InventoryListSaveable : SaveableBehaviour
	{
		#region Fields

		private InventoryList m_inventory;

		#endregion

		#region Methods

		protected override void Awake()
		{
			base.Awake();
			m_inventory = GetComponent<InventoryList>();
		}

		protected override bool SaveInternal()
		{
			if (string.IsNullOrWhiteSpace(saveId))
			{
				Debug.LogWarning("[InventoryListSaveable] Save ID is empty. Data will not be persisted.", this);
				return false;
			}

			SaveManager.SetGameValue(saveId, CaptureSnapshot());
			return true;
		}

		protected override bool LoadInternal()
		{
			if (!SaveManager.TryGetGameValue<InventorySnapshot>(m_saveId, out var snapshot) || snapshot == null)
				return false;

			RestoreSnapshot(snapshot);
			return true;
		}

		#endregion

		#region Snapshot Methods

		private InventorySnapshot CaptureSnapshot()
		{
			var snapshot = new InventorySnapshot();

			foreach (var slot in m_inventory.currencies)
			{
				snapshot.currencies.Add(new CurrencySlot(slot));
			}
			foreach (var slot in m_inventory.items)
			{
				snapshot.items.Add(new ItemSlot(slot));
			}

			return snapshot;
		}

		private void RestoreSnapshot(InventorySnapshot snapshot)
		{
			RestoreCurrencies(snapshot);
			RestoreItems(snapshot);
		}

		private void RestoreCurrencies(InventorySnapshot snapshot)
		{
			// Zero out all existing currency slots first.
			foreach (var slot in m_inventory.currencies)
			{
				if (slot.amount > 0)
				{
					m_inventory.TryRemoveCurrency(slot.slotType, slot.amount);
				}
			}

			// Re-apply saved amounts. AddCurrency creates the slot if it does
			// not already exist, so this handles both pre-configured and
			// dynamically created currency types.
			foreach (var entry in snapshot.currencies)
			{
				if (entry.slotType == null || entry.amount <= 0)
					continue;

				m_inventory.AddCurrency(entry.slotType, entry.amount);
			}
		}

		private void RestoreItems(InventorySnapshot snapshot)
		{
			// Clear amounts from every existing slot.
			m_inventory.ClearItems();

			// Reconstruct slots from the snapshot.
			foreach (var entry in snapshot.items)
			{
				if (entry.slotType == null)
				{
					// Preserve empty slot for fixed-size inventories.
					var emptySlot = new ItemSlot();
					m_inventory.AddSlot(emptySlot);
				}
				else
				{
					// AddItem handles slot creation and stacking.
					m_inventory.AddItem(entry.slotType, entry.amount);
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Pure-data snapshot of an InventoryList. Contains no scene references —
	/// only asset (ScriptableObject) references that OdinSerializer can safely
	/// round-trip through the binary save file.
	/// </summary>
	[Serializable]
	public class InventorySnapshot
	{
		public List<CurrencySlot> currencies = new();
		public List<ItemSlot> items = new();
	}
}