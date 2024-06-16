using System;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	public class InventoryManager : Singleton<InventoryManager>
    {
		#region Fields

		[SerializeField]
		private InventoryMap m_map;

		#endregion

		#region Properties

		public static InventoryMap map => Instance.m_map;

		#endregion

		#region Structures

		[Serializable]
		public class InventoryMap : SerializableDictionary<string, Inventory>
		{ }

		#endregion
	}
}