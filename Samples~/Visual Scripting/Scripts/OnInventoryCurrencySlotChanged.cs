using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Inventory.VisualScripting
{
    [UnitTitle("On Currency Slot Changed"), UnitSurtitle("Inventory")]
    public class OnInventoryCurrencySlotChanged : BaseCurrencyEventUnit
    {
        public override Type MessageListenerType => typeof(OnInventoryCurrencySlotChangedMessageListener);
    }
}