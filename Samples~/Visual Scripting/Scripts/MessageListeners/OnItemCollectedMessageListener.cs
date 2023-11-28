using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[AddComponentMenu("")]
	public class OnItemCollectedMessageListener : MessageListener
	{
		private void Start() => GetComponent<Item>()?.onCollected.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnItemCollected, gameObject, value);
		});
	}
}
