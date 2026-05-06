using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
	[CustomEditor(typeof(CurrencyType), true)]
	public class CurrencyTypeEditor : BaseToolkitEditor
	{
		#region Fields

		protected CurrencyType m_currencyType;

		protected SerializedProperty m_id;

#if USE_UNITY_LOCALIZATION
		protected SerializedProperty m_localizedName;
		protected SerializedProperty m_localizedDescription;
#else
        protected SerializedProperty m_name;
        protected SerializedProperty m_description;
#endif

		protected SerializedProperty m_icon;
		protected SerializedProperty m_properties;

		protected SerializedProperty m_maxStack;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_currencyType = target as CurrencyType;

			m_id = serializedObject.FindProperty(nameof(m_id));

#if USE_UNITY_LOCALIZATION
			m_localizedName = serializedObject.FindProperty(nameof(m_localizedName));
			m_localizedDescription = serializedObject.FindProperty(nameof(m_localizedDescription));
#else
            m_name = serializedObject.FindProperty(nameof(m_name));
            m_description = serializedObject.FindProperty(nameof(m_description));
#endif
			m_icon = serializedObject.FindProperty(nameof(m_icon));
			m_properties = serializedObject.FindProperty(nameof(m_properties));

			m_maxStack = serializedObject.FindProperty(nameof(m_maxStack));
		}

		protected override void DrawProperties()
		{
			DrawIDProperty(m_id);

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
#if USE_UNITY_LOCALIZATION
			EditorGUILayout.PropertyField(m_localizedName, new GUIContent("Name"));
			EditorGUILayout.PropertyField(m_localizedDescription, new GUIContent("Description"));
#else
            EditorGUILayout.PropertyField(m_name);
			EditorGUILayout.PropertyField(m_description);
#endif
			DrawIconProperty(m_icon);
			EditorGUILayout.PropertyField(m_properties);

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_maxStack);
		}

		#endregion
	}
}
