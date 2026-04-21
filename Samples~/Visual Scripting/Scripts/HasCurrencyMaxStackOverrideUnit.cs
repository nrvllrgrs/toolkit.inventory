using Unity.VisualScripting;
using ToolkitEngine.Inventory;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Inventory")]
	[UnitTitle("Has Max Stack Override")]
	public class HasCurrencyMaxStackOverrideUnit : Unit
	{
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabel("Currency")]
		public ValueInput currencyType { get; set; }

		[DoNotSerialize]
		public ValueOutput result { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), (flow) => exit);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			currencyType = ValueInput<CurrencyType>(nameof(currencyType), default);
			result = ValueOutput(nameof(result), (flow) =>
			{
				return InventoryManager.HasMaxStackOverride(
					flow.GetValue<CurrencyType>(currencyType));
			});
		}

		#endregion
	}
}