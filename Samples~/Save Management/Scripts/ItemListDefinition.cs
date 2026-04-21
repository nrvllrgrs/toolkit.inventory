using System.Collections.Generic;
using ToolkitEngine.SaveManagement;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine.Inventory.SaveManagement
{
	[GenericMenuCategory("Inventory/Lists")]
	public class ItemListDefinition : SaveListDefinition<ItemSlot>
	{
#if UNITY_EDITOR
		protected override void DrawElement(Rect rect, List<ItemSlot> list, int index)
		{
			var item = list[index];
			if (item == null)
				return;

			ItemType slotType = item.slotType;
			int amount = item.amount;

			EditorGUI.BeginChangeCheck();
			{
				slotType = EditorGUIRectLayout.ObjectField(ref rect, slotType, false, new GUIContent("Item"));
				amount = EditorGUIRectLayout.IntField(ref rect, "Amount", amount);
			}
			if (EditorGUI.EndChangeCheck())
			{
				item.Set(slotType, amount);
			}
		}

		protected override float ElementHeightCallback(List<ItemSlot> list, int index)
		{
			var item = list[index];
			if (item == null)
				return 0f;

			return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f;
		}
#endif
	}
}