using UnityEngine;
using UnityEditor;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
    [CustomEditor(typeof(Loot), true)]
    public class LootEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_seed;
		protected SerializedProperty m_lootTable;
		protected SerializedProperty m_amountType;
		protected SerializedProperty m_picks;
		protected SerializedProperty m_pickRange;
		protected SerializedProperty m_guaranteedDrops;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_seed = serializedObject.FindProperty(nameof(m_seed));
			m_lootTable = serializedObject.FindProperty(nameof(m_lootTable));
			m_amountType = serializedObject.FindProperty(nameof(m_amountType));
			m_picks = serializedObject.FindProperty(nameof(m_picks));
			m_pickRange = serializedObject.FindProperty(nameof(m_pickRange));
			m_guaranteedDrops = serializedObject.FindProperty(nameof(m_guaranteedDrops));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_seed);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_lootTable);
			EditorGUILayout.PropertyField(m_amountType, new GUIContent("Amount"));

			switch ((DropEntry.AmountType)m_amountType.enumValueIndex)
			{
				case DropEntry.AmountType.Constant:
					EditorGUILayout.PropertyField(m_picks, new GUIContent(" "));
					break;

				case DropEntry.AmountType.Range:
					++EditorGUI.indentLevel;
					{
						int min = m_pickRange.vector2IntValue.x;
						int max = m_pickRange.vector2IntValue.y;

						EditorGUI.BeginChangeCheck();
						{
							min = EditorGUILayout.IntField("Min", min);
							UpdatePickRange(min, max);
						}
						if (EditorGUI.EndChangeCheck() && min > max)
						{
							UpdatePickRange(max, max);
						}

						EditorGUI.BeginChangeCheck();
						{
							max = EditorGUILayout.IntField("Max", max);
							UpdatePickRange(min, max);
						}
						if (EditorGUI.EndChangeCheck() && max < min)
						{
							UpdatePickRange(min, min);
						}
					}
					--EditorGUI.indentLevel;
					break;
			}

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(m_guaranteedDrops);
		}

		private void UpdatePickRange(int min, int max) => m_pickRange.vector2IntValue = new Vector2Int(min, max);

		#endregion
	}
}