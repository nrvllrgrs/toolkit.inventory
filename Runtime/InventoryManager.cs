using System.Collections.Generic;
using System.Linq;

namespace ToolkitEngine.Inventory
{
	public class InventoryManager : InstantiableSubsystem<InventoryManager, InventoryManagerConfig>
    {
		#region Fields

		private Dictionary<string, InventoryList> m_inventories = new();

		#endregion

		#region Properties

		public string[] keys => m_inventories.Keys.ToArray();

		#endregion

		#region Methods

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
			return m_inventories.TryGetValue(key, out inventory);
		}

		#endregion
	}
}