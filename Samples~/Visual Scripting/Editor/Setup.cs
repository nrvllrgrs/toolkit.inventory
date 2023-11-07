using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace ToolkitEditor.Inventory.VisualScripting
{
    public static class Setup
    {
        [MenuItem("Window/Toolkit/Visual Scripting/Setup Inventory")]
        public static void Do()
        {
            bool dirty = false;
            var config = BoltCore.Configuration;

			foreach (var type in typeof(ToolkitEngine.Inventory.Inventory).Assembly.GetTypes())
            {
                if (!config.typeOptions.Contains(type))
                {
                    config.typeOptions.Add(type);
                    dirty = true;

                    Debug.LogFormat("Adding {0} to Visual Scripting type options.", type.FullName);
                }
            }

            if (dirty)
            {
                var metadata = config.GetMetadata(nameof(config.typeOptions));
                metadata.Save();
                Codebase.UpdateSettings();
                UnitBase.Rebuild();
            }
		}
	}
}