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

        private 


        protected void DrawDropGUI(ref Rect position, SerializedProperty property)
        {
            bool fixedAmount = false;

			var dropTypeProp = property.FindPropertyRelative("m_dropType");
#if USE_COLLECTABLES_TOOLKIT
            fixedAmount = IsCollectableDrop(dropTypeProp);

			bool IsCollectableDrop(SerializedProperty dropTypeProp)
			{
				return (DropEntry.DropType)dropTypeProp.enumValueIndex == DropEntry.DropType.Collectable;
			}
#endif

			var amountTypeProp = property.FindPropertyRelative("m_amountType");
            var amountValueProp = property.FindPropertyRelative("m_amount");
			
            EditorGUI.BeginChangeCheck();
            {
                EditorGUIRectLayout.EnumPopup<DropEntry.DropType>(ref position, dropTypeProp, "Drop");
            }
            if (EditorGUI.EndChangeCheck())
            {
#if USE_COLLECTABLES_TOOLKIT
                if (IsCollectableDrop(dropTypeProp))
                {
                    fixedAmount = true;
                    amountTypeProp.enumValueIndex = (int)DropEntry.AmountType.Constant;
                    amountValueProp.intValue = 1;
				}
#endif
            }

			EditorGUIRectLayout.BeginFieldOnly(ref position);

            switch ((DropEntry.DropType)dropTypeProp.enumValueIndex)
            {
                case DropEntry.DropType.Item:
                    EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_item"), GUIContent.none);
                    break;

                case DropEntry.DropType.Currency:
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_currency"), GUIContent.none);
                    break;

                case DropEntry.DropType.LootTable:
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_lootTable"), GUIContent.none);
                    break;

#if USE_COLLECTABLES_TOOLKIT
				case DropEntry.DropType.Collectable:
                    EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_collectable"), GUIContent.none);
                    break;
#endif
			}
			EditorGUIRectLayout.EndFieldOnly(ref position);

            EditorGUI.BeginDisabledGroup(fixedAmount);
            {
                EditorGUIRectLayout.EnumPopup<DropEntry.AmountType>(ref position, amountTypeProp, "Amount");
                switch ((DropEntry.AmountType)amountTypeProp.enumValueIndex)
                {
                    case DropEntry.AmountType.Constant:
                        EditorGUIRectLayout.BeginFieldOnly(ref position);
                        EditorGUIRectLayout.PropertyField(ref position, amountValueProp, GUIContent.none);
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
            EditorGUI.EndDisabledGroup();
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

#if USE_COLLECTABLES_TOOLKIT
				case DropEntry.DropType.Collectable:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_collectable"))
                        + EditorGUIUtility.standardVerticalSpacing;
					break;
#endif
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