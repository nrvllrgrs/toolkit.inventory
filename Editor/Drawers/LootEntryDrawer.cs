using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
    [CustomPropertyDrawer(typeof(LootEntry))]
    public class LootEntryDrawer : DropEntryDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_rate"));

            float percent = property.FindPropertyRelative("m_percent").floatValue;

            EditorGUIRectLayout.BeginFieldOnly(ref position);
			EditorGUIRectLayout.ProgressBar(ref position, percent, percent.ToString("P2"));
            EditorGUIRectLayout.EndFieldOnly(ref position);

            DrawDropGUI(ref position, property);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label)
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_rate"))
                + EditorGUIUtility.singleLineHeight
                + (EditorGUIUtility.standardVerticalSpacing * 2f);
        }
    }
}