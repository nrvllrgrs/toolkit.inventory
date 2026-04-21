using System;
using System.Collections.Generic;

namespace ToolkitEngine.Inventory
{
	[Serializable, GenericMenuCategory("Inventory")]
	public class CurrencyTypeProperty : BaseProperty<CurrencyType>
	{
		public CurrencyTypeProperty()
			: base() { }

		public CurrencyTypeProperty(CurrencyType value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Inventory")]
	public class ItemTypeProperty : BaseProperty<ItemType>
	{
		public ItemTypeProperty()
			: base() { }

		public ItemTypeProperty(ItemType value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Inventory")]
	public class LootTableProperty : BaseProperty<LootTable>
	{
		public LootTableProperty()
			: base() { }

		public LootTableProperty(LootTable value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Inventory")]
	public class LootEntryProperty : BaseProperty<LootEntry>
	{
		public LootEntryProperty()
			: base() { }

		public LootEntryProperty(LootEntry value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Inventory/Lists")]
	public class PriceListProperty : BasePropertyList<ItemType.Price>
	{
		public PriceListProperty()
			: base() { }

		public PriceListProperty(IEnumerable<ItemType.Price> value)
			: base(value) { }
	}
}