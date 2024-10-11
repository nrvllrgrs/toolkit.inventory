using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnInventoryItemSlotChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<InventoryList>()?.onItemSlotChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnInventoryItemSlotChanged, gameObject, value);
		});
	}
}
