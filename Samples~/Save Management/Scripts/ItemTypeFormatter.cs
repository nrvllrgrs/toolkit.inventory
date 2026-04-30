using OdinSerializer;

// Registers this formatter with Odin at a higher priority than its built-in
// UnityEngine.Object handler, so ItemType is always serialized as its ID string.
[assembly: RegisterFormatter(typeof(ToolkitEngine.Inventory.SaveManagement.ItemTypeFormatter), priority: 10)]

namespace ToolkitEngine.Inventory.SaveManagement
{
	/// <summary>
	/// Custom Odin Serializer formatter for <see cref="ItemType"/>.
	/// </summary>
	public sealed class ItemTypeFormatter : MinimalBaseFormatter<ItemType>
	{
		private static readonly Serializer<string> s_stringSerializer = Serializer.Get<string>();

		protected override void Read(ref ItemType value, IDataReader reader)
		{
			string id = s_stringSerializer.ReadValue(reader);
			if (string.IsNullOrEmpty(id))
			{
				value = null;
				return;
			}

			var itemDatabase = InventoryManager.Config.itemDatabase;
			if (itemDatabase == null)
				return;

			value = itemDatabase.GetItem(id);
		}

		protected override void Write(ref ItemType value, IDataWriter writer)
		{
			s_stringSerializer.WriteValue(value != null ? value.id : string.Empty, writer);
		}
	}
}