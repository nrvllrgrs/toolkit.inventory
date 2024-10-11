using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnCurrencySlotChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<InventoryList>()?.onCurrencySlotChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnInventoryCurrencySlotChanged, gameObject, value);
		});
	}
}
