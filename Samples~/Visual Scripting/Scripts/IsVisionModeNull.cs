using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Vision")]
	[UnitTitle("Is Vision Mode Null")]
	public class IsVisionModeNull : Unit
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput isNull;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Enter);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			isNull = ValueOutput(nameof(isNull), (flow) => VisionModeManager.CastInstance.activeMode == null);
		}

		private ControlOutput Enter(Flow flow)
		{
			return exit;
		}

		#endregion
	}
}