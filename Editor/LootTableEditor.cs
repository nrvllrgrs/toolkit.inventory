using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
    [CustomEditor(typeof(LootTable))]
    public class LootTableEditor : Editor
    {
        #region Fields

        protected LootTable m_lootTable;
        protected SerializedProperty m_items;
        protected SerializedProperty m_noDropRate;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_lootTable = target as LootTable;
            m_items = serializedObject.FindProperty(nameof(m_items));
            m_noDropRate = serializedObject.FindProperty(nameof(m_noDropRate));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var totalRates = m_lootTable.totalRates;
            for (int i = 0; i < m_items.arraySize; ++i)
            {
                var itemProp = m_items.GetArrayElementAtIndex(i);
                var percentProp = itemProp.FindPropertyRelative("m_percent");
                percentProp.floatValue = totalRates > 0f ? itemProp.FindPropertyRelative("m_rate").floatValue / totalRates : 0f;
            }

            EditorGUILayout.PropertyField(m_items);
            EditorGUILayout.PropertyField(m_noDropRate);

            float percent = totalRates > 0f ? m_noDropRate.floatValue / totalRates : 0f;

			var rect = GUILayoutUtility.GetLastRect();
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUIRectLayout.BeginFieldOnly(ref rect);
			EditorGUIRectLayout.ProgressBar(ref rect, percent, percent.ToString("P2"));
            EditorGUIRectLayout.EndFieldOnly(ref rect);

            EditorGUILayout.Space(rect.y);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}