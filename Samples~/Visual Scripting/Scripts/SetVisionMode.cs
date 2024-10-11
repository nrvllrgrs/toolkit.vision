using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Vision")]
	[UnitTitle("Set Vision Mode")]
	public class SetVisionMode : Unit
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize]
		public ValueInput mode;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Enter);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			mode = ValueInput<VisionMode>(nameof(mode), null);
		}

		private ControlOutput Enter(Flow flow)
		{
			VisionModeManager.CastInstance.activeMode = flow.GetValue<VisionMode>(mode);
			return exit;
		}

		#endregion
	}
}