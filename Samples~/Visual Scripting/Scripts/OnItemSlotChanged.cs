using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
	[UnitTitle("On Item Slot Changed"), UnitSurtitle("Inventory")]
	public class OnItemSlotChanged : BaseItemEventUnit
	{
		public override Type MessageListenerType => typeof(OnItemSlotChangedMessageListener);
	}
}