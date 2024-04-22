using System;
using System.Collections.Generic;
using ToolkitEngine.Inventory;
using UnityEditor;

namespace ToolkitEditor.Inventory.VisualScripting
{
	[InitializeOnLoad]
    public static class Setup
    {
        static Setup()
        {
			var types = new List<Type>()
			{
				typeof(ToolkitEngine.Inventory.Inventory),
				typeof(ItemSlot),
				typeof(ItemEventArgs),
				typeof(ItemType),
				typeof(Item),
				typeof(CurrencySlot),
				typeof(CurrencyEventArgs),
				typeof(Currency),
				typeof(CurrencyType),
				typeof(LootTable),
				typeof(LootEntry),
				typeof(Loot),
				typeof(Recipes),
			};

			ToolkitEditor.VisualScripting.Setup.Initialize("ToolkitEngine.Inventory", types);
		}
    }
}