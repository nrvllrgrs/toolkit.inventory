using Unity.VisualScripting;
using ToolkitEngine.Inventory;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Inventory")]
	[UnitTitle("Set Max Stack Override")]
	public class SetCurencyMaxStackOverrideUnit : Unit
    {
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabel("Currency")]
		public ValueInput currencyType { get; set; }

		[DoNotSerialize]
		public ValueInput maxStack { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			currencyType = ValueInput<CurrencyType>(nameof(currencyType), default);
			maxStack = ValueInput(nameof(maxStack), 0);
		}

		private ControlOutput Trigger(Flow flow)
		{
			InventoryManager.SetMaxStackOverride(
				flow.GetValue<CurrencyType>(currencyType),
				flow.GetValue<int>(maxStack));

			return exit;
		}

		#endregion
	}
}