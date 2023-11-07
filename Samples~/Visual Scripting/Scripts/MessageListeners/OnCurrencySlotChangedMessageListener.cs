using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnCurrencySlotChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<Inventory>()?.onCurrencySlotChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnCurrencySlotChanged, gameObject, value);
		});
	}
}
