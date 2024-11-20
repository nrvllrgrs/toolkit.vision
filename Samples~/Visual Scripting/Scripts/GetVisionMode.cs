using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Vision")]
	[UnitTitle("Get Vision Mode")]
	public class GetVisionMode : Unit
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput visionMode;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Enter);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			visionMode = ValueOutput(nameof(visionMode), (flow) => VisionModeManager.CastInstance.activeMode);
		}

		private ControlOutput Enter(Flow flow)
		{
			return exit;
		}

		#endregion
	}
}