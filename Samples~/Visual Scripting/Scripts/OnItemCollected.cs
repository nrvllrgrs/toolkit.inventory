using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[UnitTitle("On Item Collected"), UnitSurtitle("Item")]
	public class OnItemCollected : BaseItemEventUnit
	{
		public override Type MessageListenerType => typeof(OnItemCollectedMessageListener);
	}
}