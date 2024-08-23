using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Events/Vision")]
	[UnitTitle("On Vision Mode Changed")]
	public class OnVisionModeChanged : EventUnit<VisionMode>
	{
		#region Fields

		[UnitHeaderInspectable("Filtered")]
		public bool filtered;

		[DoNotSerialize]
		public ValueInput mode;

		private GraphReference m_graph;

		#endregion

		#region Properties

		public string hookName => GetType().Name;

		protected override bool register => true;

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

		public override void StartListening(GraphStack stack)
		{
			base.StartListening(stack);

			if (register && m_graph == null)
			{
				m_graph = stack.AsReference();
				VisionModeManager.Instance.onChanged.AddListener(Changed);
			}
		}

		public override void StopListening(GraphStack stack)
		{
			base.StopListening(stack);

			if (register && m_graph != null)
			{
				VisionModeManager.Instance.onChanged.RemoveListener(Changed);
				m_graph = null;
			}
		}

		private void Changed(VisionMode mode)
		{
			Trigger(m_graph, mode);
		}

		protected override bool ShouldTrigger(Flow flow, VisionMode args)
		{
			return !filtered || Equals(flow.GetValue<VisionMode>(mode), args);
		}

		#endregion
	}
}