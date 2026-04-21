using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	[CustomPropertyDrawer(typeof(ItemType.Price))]
	public class PriceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var currencyProp = property.FindPropertyRelative("currency");
			var amountProp = property.FindPropertyRelative("amount");

			// Get values for the label
			string currencyName = currencyProp.objectReferenceValue != null
				? currencyProp.objectReferenceValue.name
				: "[Empty]";
			int amount = amountProp != null ? amountProp.intValue : 0;

			if (EditorGUIRectLayout.Foldout(ref position, property, $"{currencyName} x{amount}"))
			{
				++EditorGUI.indentLevel;
				{
					EditorGUIRectLayout.PropertyField(ref position, currencyProp);
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
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("currency"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("amount"))
					+ (EditorGUIUtility.standardVerticalSpacing * 2f);
			}

			return height;
		}
	}
}