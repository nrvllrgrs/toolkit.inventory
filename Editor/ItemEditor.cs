using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
	[CustomEditor(typeof(Item))]
	public class ItemEditor : BaseToolkitEditor
	{
		#region Fields

		protected SerializedProperty m_itemType;
		protected SerializedProperty m_amount;

		protected SerializedProperty m_onCollected;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_itemType = serializedObject.FindProperty(nameof(m_itemType));
			m_amount = serializedObject.FindProperty(nameof(m_amount));

			m_onCollected = serializedObject.FindProperty(nameof(m_onCollected));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_itemType);
			EditorGUILayout.PropertyField(m_amount);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onCollected, "Events"))
			{
				EditorGUILayout.PropertyField(m_onCollected);

				DrawNestedEvents();
			}
		}

		#endregion
	}
}