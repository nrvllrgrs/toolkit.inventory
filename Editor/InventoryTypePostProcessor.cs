using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
	/// <summary>
	/// Ensures every ItemType/CurrencyType ScriptableObject in the project has a unique m_id.
	/// Runs automatically whenever assets are imported, and can also be triggered
	/// manually via  Tools > Toolkit > Validate Item IDs.
	/// </summary>
	public class InventoryTypePostProcessor : AssetPostprocessor
	{
		[MenuItem("Tools/Toolkit/Inventory/Validate Item IDs")]
		public static void ValidateAllItemIds() => ValidateAllIds<ItemType>();

		[MenuItem("Tools/Toolkit/Inventory/Validate Currency IDs")]
		public static void ValidateAllCurrencyIds() => ValidateAllIds<CurrencyType>();

		public static void ValidateAllIds<T>()
			where T : ScriptableObject
		{
			int fixedCount = EnforceUniqueIds<T>(true);
			if (fixedCount == 0)
			{
				Debug.Log($"[InventoryTypePostProcessor] All {typeof(T).Name} IDs are unique. No changes needed.");
			}
			else
			{
				Debug.Log($"[InventoryTypePostProcessor] Fixed {fixedCount} duplicate ID(s). Assets saved.");
			}
		}

		// Called by Unity after any asset import / re-import / move / delete.
		static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			// Only bother scanning when an asset that could be an ItemType was touched.
			bool relevant = false;
			foreach (var path in importedAssets)
			{
				if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
				{
					relevant = true;
					break;
				}
			}

			if (!relevant)
				return;

			EnforceUniqueIds<ItemType>(false);
			EnforceUniqueIds<CurrencyType>(false);
		}

		/// <summary>
		/// Scans every ItemType asset in the project. Any asset whose m_id is
		/// empty or collides with a previously-seen ID is assigned a fresh GUID.
		/// </summary>
		/// <param name="logAll">When true, logs every asset checked (useful for the manual run).</param>
		/// <returns>Number of assets whose ID was regenerated.</returns>
		private static int EnforceUniqueIds<T>(bool logAll)
			where T : ScriptableObject
		{
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			if (guids.Length == 0)
				return 0;

			// seen maps  id → first asset path that claimed it
			var idToAssetMap = new Dictionary<string, string>(guids.Length, StringComparer.Ordinal);
			int fixedCount = 0;

			AssetDatabase.StartAssetEditing();
			try
			{
				foreach (string guid in guids)
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					var asset = AssetDatabase.LoadAssetAtPath<T>(path);

					if (asset == null)
						continue;

					// Use SerializedObject so we can write the private backing field.
					var serializedAsset = new SerializedObject(asset);
					var idProp = serializedAsset.FindProperty("m_id");

					if (idProp == null)
						continue;

					string id = idProp.stringValue;
					bool needsNewId = false;

					if (string.IsNullOrEmpty(id))
					{
						Debug.LogWarning($"[InventoryTypePostProcessor] '{path}' has an empty ID — assigning a new one.", asset);
						needsNewId = true;
					}
					else if (idToAssetMap.TryGetValue(id, out string firstOwner))
					{
						Debug.LogWarning(
							$"[InventoryTypePostProcessor] Duplicate ID '{id}' found on '{path}'. " +
							$"First seen on '{firstOwner}'. Assigning a new ID to the duplicate.", asset);
						needsNewId = true;
					}

					if (needsNewId)
					{
						idProp.stringValue = id = Guid.NewGuid().ToString();
						serializedAsset.ApplyModifiedPropertiesWithoutUndo();
						EditorUtility.SetDirty(asset);

						++fixedCount;
					}
					else if (logAll)
					{
						Debug.Log($"[InventoryTypePostProcessor] '{path}' — ID OK ({guid})", asset);
					}

					idToAssetMap.Add(id, path);
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
			}

			if (fixedCount > 0)
			{
				AssetDatabase.SaveAssets();
			}

			return fixedCount;
		}
	}
}