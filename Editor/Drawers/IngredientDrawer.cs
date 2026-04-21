using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	[CustomPropertyDrawer(typeof(ItemType.Ingredient))]
	public class IngredientDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var itemProp = property.FindPropertyRelative("item");
			var amountProp = property.FindPropertyRelative("amount");

			// Get values for the label
			string itemName = itemProp.objectReferenceValue != null
				? itemProp.objectReferenceValue.name
				: "[Empty]";
			int amount = amountProp != null ? amountProp.intValue : 0;

			if (EditorGUIRectLayout.Foldout(ref position, property, $"{itemName} x{amount}"))
			{
				++EditorGUI.indentLevel;
				{
					EditorGUIRectLayout.PropertyField(ref position, itemProp);
					EditorGUIRectLayout.PropertyField(ref position, amountProp);
				}
				--EditorGUI.indentLevel;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight;
			if (property.isExpanded)
			{
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("item"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("amount"))
					+ (EditorGUIUtility.standardVerticalSpacing * 2f);
			}

			return height;
		}
	}
}