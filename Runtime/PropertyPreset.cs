using UnityEngine;
using static ToolkitEngine.Vision.PropertyData;

namespace ToolkitEngine.Vision
{
	[CreateAssetMenu(menuName = "Toolkit/Vision/Property Preset")]
	public class PropertyPreset : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private PropertyData m_data;

		#endregion

		#region Properties

		public PropertyData data => m_data;

		#endregion

		#region Methods

		public object GetValue()
		{
			switch (data.type)
			{
				case PropertyType.Boolean:
				case PropertyType.Keyword:
					return data.boolValue;

				case PropertyType.Color:
					return data.colorValue;

				case PropertyType.Float:
					return data.floatValue;

				case PropertyType.Integer:
					return data.intValue;
			}
			return null;
		}

		#endregion
	}
}