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

		#region Properties

		public System.Type subsystemType => typeof(InventoryManager);
		public GameObject template => m_template;

		#endregion
	}
}