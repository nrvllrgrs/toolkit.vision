using System;
using ToolkitEngine.VisualScripting;
using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Events/Vision")]
	[UnitTitle("On Vision Mode Changed")]
	public class OnVisionModeChanged : TargetEventUnit<VisionMode>
	{
		#region Fields

		[UnitHeaderInspectable("Filtered")]
		public bool filtered;

		[DoNotSerialize]
		public ValueInput mode;

		#endregion

		#region Properties

		public override Type MessageListenerType => throw new NotImplementedException();

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			if (filtered)
			{
				mode = ValueInput<VisionMode>(nameof(mode));
			}
		}

		protected override void StartListeningToManager()
		{
			VisionModeManager.CastInstance.Changed += InvokeTrigger;
		}

		protected override void StopListeningToManager()
		{
			VisionModeManager.CastInstance.Changed -= InvokeTrigger;
		}

		protected override bool ShouldTrigger(Flow flow, VisionMode args)
		{
			return !filtered || Equals(flow.GetValue<VisionMode>(mode), args);
		}

		#endregion
	}
}