using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnInventoryCurrencySlotChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<Inventory>()?.onCurrencySlotChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnInventoryCurrencySlotChanged, gameObject, value);
		});
	}
}
