using UnityEngine;
using Gilzoide.EasyProjectSettings;
using ToolkitEngine.Inventory;

namespace ToolkitEditor.Inventory
{
	[ProjectSettings(
	    "ProjectSettings/InventorySettings",
	    SettingsPath = "Project/Toolkit/Inventory",
	    Label = "Inventory",
	    SettingsType = SettingsType.ProjectSettings)]
	public class InventorySettings : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private ItemDatabase m_itemDatabase;

		#endregion

		#region Properties

		public ItemDatabase itemDatabase => m_itemDatabase;
		
		#endregion
	}
}