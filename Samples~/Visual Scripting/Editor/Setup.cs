using System;
using System.Collections.Generic;
using ToolkitEngine.Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

namespace ToolkitEditor.Inventory.VisualScripting
{
    [InitializeOnLoad]
    public static class Setup
    {
        static Setup()
        {
			bool dirty = false;
			var config = BoltCore.Configuration;

			var assemblyName = new LooseAssemblyName("ToolkitEngine.Inventory");
			if (!config.assemblyOptions.Contains(assemblyName))
			{
				config.assemblyOptions.Add(assemblyName);
				dirty = true;

				Debug.LogFormat("Adding {0} to Visual Scripting assembly options.", assemblyName);
			}

			var types = new List<Type>()
			{
				typeof(InventoryList),
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
				typeof(DropEntry),
				typeof(Recipes),
			};

			foreach (var type in types)
			{
				if (!config.typeOptions.Contains(type))
				{
					config.typeOptions.Add(type);
					dirty = true;

					Debug.LogFormat("Adding {0} to Visual Scripting type options.", type.FullName);
				}
			}

			if (dirty)
			{
				var metadata = config.GetMetadata(nameof(config.typeOptions));
				metadata.Save();
				Codebase.UpdateSettings();
				UnitBase.Rebuild();
			}
		}
    }
}