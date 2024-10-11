using UnityEngine;
using UnityEditor;

namespace ToolkitEditor.Inventory
{
    [CustomEditor(typeof(ToolkitEngine.Inventory.InventoryList))]
    public class InventoryEditor : BaseToolkitEditor
    {
        #region Fields

        protected SerializedProperty m_currencies;
        protected SerializedProperty m_items;
        protected SerializedProperty m_expandSlots;
        protected SerializedProperty m_useEncumbrance;
        protected SerializedProperty m_encumbranceWeight;

        protected SerializedProperty m_onCurrencySlotChanged;
        protected SerializedProperty m_onItemSlotChanged;
        protected SerializedProperty m_onEncumbranceChanged;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_currencies = serializedObject.FindProperty(nameof(m_currencies));
            m_items = serializedObject.FindProperty(nameof(m_items));
            m_expandSlots = serializedObject.FindProperty(nameof(m_expandSlots));
            m_useEncumbrance = serializedObject.FindProperty(nameof(m_useEncumbrance));
            m_encumbranceWeight = serializedObject.FindProperty(nameof(m_encumbranceWeight));

            m_onCurrencySlotChanged = serializedObject.FindProperty(nameof(m_onCurrencySlotChanged));
            m_onItemSlotChanged = serializedObject.FindProperty(nameof(m_onItemSlotChanged));
            m_onEncumbranceChanged = serializedObject.FindProperty(nameof(m_onEncumbranceChanged));
        }

        protected override void DrawProperties()
        {
            EditorGUILayout.PropertyField(m_currencies);
			EditorGUILayout.PropertyField(m_items);
			EditorGUILayout.PropertyField(m_expandSlots);

			EditorGUILayout.PropertyField(m_useEncumbrance);
			if (m_useEncumbrance.boolValue)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_encumbranceWeight, new GUIContent("Weight Threshold"));
				--EditorGUI.indentLevel;
			}
		}

        protected override void DrawEvents()
        {
			if (EditorGUILayoutUtility.Foldout(m_onCurrencySlotChanged, "Events"))
			{
                EditorGUILayout.PropertyField(m_onCurrencySlotChanged);
				EditorGUILayout.PropertyField(m_onItemSlotChanged);

				if (m_useEncumbrance.boolValue)
				{
					EditorGUILayout.PropertyField(m_onEncumbranceChanged);
				}

                DrawNestedEvents();
			}
		}

        #endregion
    }
}