using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;
using System.Linq;

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

			// Capture the layout cursor Y before drawing the property field so we
			// can reconstruct the header rect for drag-and-drop hit-testing.
			Rect cursorRect = EditorGUILayout.GetControlRect(false, 0f);
			Rect itemsHeaderRect = new Rect(cursorRect.x, cursorRect.y, cursorRect.width, EditorGUIUtility.singleLineHeight);

			HandleItemsDragAndDrop(itemsHeaderRect);
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

		private void HandleItemsDragAndDrop(Rect headerRect)
		{
			Event evt = Event.current;
			if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
				return;

			if (!headerRect.Contains(evt.mousePosition))
				return;

			bool hasValidType = DragAndDrop.objectReferences.Any(x => x is ItemType
				|| x is CurrencyType
				|| x is LootTable);

			if (!hasValidType)
				return;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (evt.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				foreach (Object obj in DragAndDrop.objectReferences)
				{
					if (obj is ItemType itemType)
					{
						var entry = new LootEntry(itemType, 1);
						entry.weight = 1f;
						m_lootTable.Add(entry);
					}
					else if (obj is CurrencyType currencyType)
					{
						var entry = new LootEntry(currencyType, 1);
						entry.weight = 1f;
						m_lootTable.Add(entry);
					}
					else if (obj is LootTable lootTable)
					{
						var entry = new LootEntry(lootTable, 1);
						entry.weight = 1f;
						m_lootTable.Add(entry);
					}
				}

				// Sync the SerializedObject so the new entries appear immediately.
				serializedObject.Update();
			}

			evt.Use();
		}

		#endregion
	}
}