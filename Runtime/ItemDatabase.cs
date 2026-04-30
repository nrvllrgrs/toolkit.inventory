using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	[CreateAssetMenu(menuName = "Toolkit/Inventory/Item Database", order = 10)]
    public class ItemDatabase : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private List<ItemType> m_items = new();

		private Dictionary<string, ItemType> m_map = new();

		#endregion

		#region Methods

		internal void Initialize()
		{
			foreach (var item in m_items)
			{
				if (item == null)
					continue;

				m_map.Add(item.id, item);
			}
		}

		public ItemType GetItem(string id) => m_map[id];
		public bool TryGetItem(string id, out ItemType item) => m_map.TryGetValue(id, out item);

		#endregion
	}
}