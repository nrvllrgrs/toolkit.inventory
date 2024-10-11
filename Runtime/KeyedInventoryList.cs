namespace ToolkitEngine.Inventory
{
	public sealed class KeyedInventoryList : KeyedComponent<InventoryList>
    {
		#region Methods

		private void OnEnable()
		{
			InventoryManager.CastInstance.Register(this);
		}

		private void OnDisable()
		{
			InventoryManager.CastInstance.Unregister(this);
		}

		#endregion
	}
}