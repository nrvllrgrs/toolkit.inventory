using UnityEditor;
using ToolkitEngine.Inventory;
using UnityEngine;

namespace ToolkitEditor.Inventory
{
	[CustomEditor(typeof(CurrencyPool))]
    public class CurrencyPoolEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_currencyType;
		protected SerializedProperty m_autoManaged;
		protected SerializedProperty m_amount;
		protected SerializedProperty m_onAmountChanged;
		protected SerializedProperty m_onCapacityEntered;
		protected SerializedProperty m_onCapacityExited;

		#endregion

		#region Methods

		protected void OnEnable()
		{
			m_currencyType = serializedObject.FindProperty(nameof(m_currencyType));
			m_autoManaged = serializedObject.FindProperty(nameof(m_autoManaged));
			m_amount = serializedObject.FindProperty(nameof(m_amount));
			m_onAmountChanged = serializedObject.FindProperty(nameof(m_onAmountChanged));
			m_onCapacityEntered = serializedObject.FindProperty(nameof(m_onCapacityEntered));
			m_onCapacityExited = serializedObject.FindProperty(nameof(m_onCapacityExited));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_currencyType);

			EditorGUILayout.PropertyField(m_autoManaged);
			if (m_autoManaged.boolValue)
			{
				++EditorGUI.indentLevel;
				{
					EditorGUILayout.PropertyField(m_amount);
				}
				--EditorGUI.indentLevel;
			}

			if (Application.isPlaying && InventoryManager.Exists)
			{
				EditorGUILayout.Space();

				EditorGUI.BeginDisabledGroup(true);
				{
					var currencyType = m_currencyType.objectReferenceValue as CurrencyType;
					if (currencyType != null
						&& InventoryManager.TryGetAmount(currencyType, out int amount))
					{
						EditorGUILayout.LabelField("Runtime Amount", amount.ToString());
					}
					EditorGUI.EndDisabledGroup();
				}
			}
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onAmountChanged, "Events"))
			{
				EditorGUILayout.PropertyField(m_onAmountChanged);
				EditorGUILayout.PropertyField(m_onCapacityEntered);
				EditorGUILayout.PropertyField(m_onCapacityExited);
			}
		}

		#endregion
	}
}