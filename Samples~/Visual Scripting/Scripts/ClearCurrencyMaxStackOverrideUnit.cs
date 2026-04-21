using Unity.VisualScripting;
using ToolkitEngine.Inventory;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Inventory")]
	[UnitTitle("Clear Max Stack Override")]
	public class ClearCurrencyMaxStackOverrideUnit : Unit
	{
		#region Ports

		[UnitHeaderInspectable("All")]
		public bool clearAll = false;

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabel("Currency")]
		public ValueInput currencyType { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			if (!clearAll)
			{
				currencyType = ValueInput<CurrencyType>(nameof(currencyType), default);
				Requirement(currencyType, enter);
			}
		}

		private ControlOutput Trigger(Flow flow)
		{
			if (clearAll)
			{
				InventoryManager.ClearMaxStackOverride();
			}
			else
			{
				InventoryManager.ClearMaxStackOverride(
					flow.GetValue<CurrencyType>(currencyType));
			}
			return exit;
		}

		#endregion
	}
}