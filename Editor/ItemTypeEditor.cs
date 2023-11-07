using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
    [CustomEditor(typeof(ItemType))]
    public class ItemTypeEditor : Editor
    {
        #region Fields

        protected ItemType m_itemType;

        protected SerializedProperty m_id;
        protected SerializedProperty m_parent;
        protected SerializedProperty m_spawner;

        protected SerializedProperty m_name;
        protected SerializedProperty m_description;
        protected SerializedProperty m_icon;
        protected SerializedProperty m_color;

        protected SerializedProperty m_weight;
        protected SerializedProperty m_maxStack;

        protected SerializedProperty m_price;
        protected SerializedProperty m_sellable;
        protected SerializedProperty m_sellFactor;

        protected SerializedProperty m_ingredients;
        protected SerializedProperty m_dismantleMode;
        protected SerializedProperty m_scraps;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_itemType = target as ItemType;

            m_id = serializedObject.FindProperty(nameof(m_id));
            m_parent = serializedObject.FindProperty(nameof(m_parent));
            m_spawner = serializedObject.FindProperty(nameof(m_spawner));

            m_name = serializedObject.FindProperty(nameof(m_name));
            m_description = serializedObject.FindProperty(nameof(m_description));
            m_icon = serializedObject.FindProperty(nameof(m_icon));
            m_color = serializedObject.FindProperty(nameof(m_color));

            m_weight = serializedObject.FindProperty(nameof(m_weight));
            m_maxStack = serializedObject.FindProperty(nameof(m_maxStack));

            m_price = serializedObject.FindProperty(nameof(m_price));
            m_sellable = serializedObject.FindProperty(nameof(m_sellable));
            m_sellFactor = serializedObject.FindProperty(nameof(m_sellFactor));

            m_ingredients = serializedObject.FindProperty(nameof(m_ingredients));
            m_dismantleMode = serializedObject.FindProperty(nameof(m_dismantleMode));
            m_scraps = serializedObject.FindProperty(nameof(m_scraps));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_id, new GUIContent("ID"));
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();

            var lastParent = m_parent.objectReferenceValue;
            EditorGUILayout.PropertyField(m_parent);

            if (EditorGUI.EndChangeCheck())
            {
                if (m_parent.objectReferenceValue != null)
                {
                    var parentItemType = m_parent.objectReferenceValue as ItemType;
                    if (!IsValidParent(parentItemType))
                    {
                        EditorUtility.DisplayDialog("Error", string.Format("{0} creates an invalid parental relationship!", parentItemType.name), "OK");
                        m_parent.objectReferenceValue = lastParent;
                    }
                }
            }

            EditorGUILayout.PropertyField(m_spawner);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_name);
            EditorGUILayout.PropertyField(m_description);
            EditorGUILayout.ObjectField(m_icon, typeof(Sprite), GUILayout.Height(64), GUILayout.Width(64 + EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(m_color);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_weight);
            EditorGUILayout.PropertyField(m_maxStack);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Trading", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_price);
            EditorGUILayout.PropertyField(m_sellable);

            if (m_sellable.boolValue)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_sellFactor);
                --EditorGUI.indentLevel;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Crafting", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ingredients);
            EditorGUILayout.PropertyField(m_dismantleMode);

            if ((ItemType.DismantleMode)m_dismantleMode.intValue == ItemType.DismantleMode.Scrap)
            {
                EditorGUILayout.PropertyField(m_scraps);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsValidParent(ItemType parent)
        {
            if (parent == null)
                return true;
                
            HashSet<ItemType> set = new();
            set.Add(m_itemType);

            var test = parent;
            while (test != null)
            {
                if (set.Contains(test))
                    return false;

                set.Add(test);
                test = test.parent;
            }

            return true;
        }

        #endregion
    }
}