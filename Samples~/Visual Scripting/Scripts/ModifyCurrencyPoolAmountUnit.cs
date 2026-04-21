using Unity.VisualScripting;
using ToolkitEngine.Inventory;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Inventory")]
	[UnitTitle("Modify Currency Amount")]
	public class ModifyCurrencyPoolAmountUnit : Unit
	{
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabel("Currency")]
		public ValueInput currencyType { get; set; }

		[DoNotSerialize]
		public ValueInput delta { get; set; }

		[DoNotSerialize]
		public ValueOutput amount { get; set; }

		#endregion

		#region Fields

		private int? m_cachedAmount = null;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			currencyType = ValueInput<CurrencyType>(nameof(currencyType), default);
			delta = ValueInput(nameof(delta), 0);

			amount = ValueOutput(nameof(amount), (flow) =>
			{
				if (!m_cachedAmount.HasValue)
				{
					Trigger(flow);
				}
				return m_cachedAmount.Value;
			});
		}

		private ControlOutput Trigger(Flow flow)
		{
			var type = flow.GetValue<CurrencyType>(currencyType);
			InventoryManager.ModifyAmount(type, flow.GetValue<int>(delta));
			m_cachedAmount = InventoryManager.GetAmount(type);

			return exit;
		}

		#endregion
	}
}