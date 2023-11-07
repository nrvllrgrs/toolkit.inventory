using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
    [CustomPropertyDrawer(typeof(DropEntry))]
    public class DropEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            DrawDropGUI(ref position, property);

            EditorGUI.EndProperty();
        }

        protected void DrawDropGUI(ref Rect position, SerializedProperty property)
        {
            var dropTypeProp = property.FindPropertyRelative("m_dropType");
            EditorGUIRectLayout.EnumPopup<DropEntry.DropType>(ref position, dropTypeProp, "Drop");

            EditorGUIRectLayout.BeginFieldOnly(ref position);

            switch ((DropEntry.DropType)dropTypeProp.enumValueIndex)
            {
                case DropEntry.DropType.Item:
                    EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_item"), new GUIContent(string.Empty));
                    break;

                case DropEntry.DropType.Currency:
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_currency"), new GUIContent(string.Empty));
                    break;

                case DropEntry.DropType.LootTable:
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_lootTable"), new GUIContent(string.Empty));
                    break;
            }
			EditorGUIRectLayout.EndFieldOnly(ref position);

			var amountProp = property.FindPropertyRelative("m_amountType");
			EditorGUIRectLayout.EnumPopup<DropEntry.AmountType>(ref position, amountProp, "Amount");

            switch ((DropEntry.AmountType)amountProp.enumValueIndex)
            {
                case DropEntry.AmountType.Constant:
                    EditorGUIRectLayout.BeginFieldOnly(ref position);
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_amount"), new GUIContent(string.Empty));
                    EditorGUIRectLayout.EndFieldOnly(ref position);
					break;

                case DropEntry.AmountType.Range:
                    ++EditorGUI.indentLevel;

                    var minAmountProp = property.FindPropertyRelative("m_minAmount");
                    var maxAmountProp = property.FindPropertyRelative("m_maxAmount");

                    EditorGUI.BeginChangeCheck();
					EditorGUIRectLayout.PropertyField(ref position, minAmountProp, new GUIContent("Min"));

                    // If min amount changed...
                    if (EditorGUI.EndChangeCheck() && minAmountProp.intValue > maxAmountProp.intValue)
                    {
                        // Clamp to enforce min-max relationship
                        minAmountProp.intValue = maxAmountProp.intValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUIRectLayout.PropertyField(ref position, maxAmountProp, new GUIContent("Max"));

                    // If max amount changed...
                    if (EditorGUI.EndChangeCheck() && maxAmountProp.intValue < minAmountProp.intValue)
                    {
                        // Clamp to enforce min-max relationship
                        maxAmountProp.intValue = minAmountProp.intValue;
                    }

                    --EditorGUI.indentLevel;
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
			var dropTypeProp = property.FindPropertyRelative("m_dropType");
			var amountProp = property.FindPropertyRelative("m_amountType");

			float height = EditorGUI.GetPropertyHeight(dropTypeProp)
                + EditorGUI.GetPropertyHeight(amountProp)
				+ (EditorGUIUtility.standardVerticalSpacing * 2f);

			switch ((DropEntry.DropType)dropTypeProp.enumValueIndex)
			{
				case DropEntry.DropType.Item:
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_item"))
                        + EditorGUIUtility.standardVerticalSpacing;
                    break;

				case DropEntry.DropType.Currency:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_currency"))
						+ EditorGUIUtility.standardVerticalSpacing;
					break;

				case DropEntry.DropType.LootTable:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_lootTable"))
						+ EditorGUIUtility.standardVerticalSpacing;
					break;
			}

			switch ((DropEntry.AmountType)amountProp.enumValueIndex)
			{
				case DropEntry.AmountType.Constant:
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_amount"))
                        + EditorGUIUtility.standardVerticalSpacing;
					break;

				case DropEntry.AmountType.Range:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_minAmount"))
                        + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_maxAmount"))
						+ (EditorGUIUtility.standardVerticalSpacing * 2f);
					break;
			}


			return height;
		}
    }
}