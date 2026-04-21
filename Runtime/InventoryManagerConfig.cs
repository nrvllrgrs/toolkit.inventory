using UnityEngine;

namespace ToolkitEngine.Inventory
{
	[CreateAssetMenu(menuName = "Toolkit/Config/InventoryManager Config")]
	public class InventoryManagerConfig : ScriptableObject
	{
		#region Fields

		[SerializeField]
		private GameObject m_template;

		#endregion

		#region Properties

		public GameObject template => m_template;

		#endregion
	}
}