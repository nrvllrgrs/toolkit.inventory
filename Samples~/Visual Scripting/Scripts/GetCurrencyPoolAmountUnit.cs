using Unity.VisualScripting;
using ToolkitEngine.Inventory;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Inventory")]
	[UnitTitle("Get Currency Amount")]
	public class GetCurrencyPoolAmountUnit : Unit
	{
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabel("Currency")]
		public ValueInput currencyType { get; set; }

		[DoNotSerialize]
		public ValueOutput amount { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), (flow) => exit);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			currencyType = ValueInput<CurrencyType>(nameof(currencyType), default);
			amount = ValueOutput(nameof(amount), (flow) =>
			{
				return InventoryManager.GetAmount(
					flow.GetValue<CurrencyType>(currencyType));
			});
		}

		#endregion
	}
}