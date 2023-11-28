using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnInventoryItemSlotChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<Inventory>()?.onItemSlotChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnInventoryItemSlotChanged, gameObject, value);
		});
	}
}
