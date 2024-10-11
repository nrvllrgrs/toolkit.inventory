using System;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
	[CreateAssetMenu(menuName = "Toolkit/Config/InventoryManager Config")]
	public class InventoryManagerConfig : ScriptableObject, IInstantiableSubsystemConfig
	{
		#region Fields

		[SerializeField]
		private GameObject m_template;

		#endregion

		#region IInstantiableSubsystemConfig Methods

		public GameObject GetTemplate() => m_template?.gameObject;
		public Type GetManagerType() => typeof(InventoryManager);

		#endregion
	}
}