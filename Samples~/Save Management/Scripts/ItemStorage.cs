using ToolkitEngine.Inventory;
using UnityEngine;

namespace ToolkitEngine.SaveManagement.Inventory
{
	[AddComponentMenu("Save/Inventory/Item Storage")]
	public class ItemStorage : BaseVariableStorage<ItemType, SaveItem>
    {
#if UNITY_EDITOR

		[ContextMenu("Load")]
		protected override bool LoadInternal()
		{
			return base.LoadInternal();
		}

		[ContextMenu("Save")]
		protected override bool SaveInternal()
		{
			return base.SaveInternal();
		}

#endif
	}
}