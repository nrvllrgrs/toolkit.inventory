using ToolkitEngine.Inventory;

namespace ToolkitEngine.SaveManagement.Inventory
{
	[GenericMenuCategory("Inventory")]
	public class ItemDefinition : ObjectDefinition<ItemType>
    {
		public ItemDefinition()
			: base()
		{ }
	}
}