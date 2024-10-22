using System;
using UnityEngine;

namespace ToolkitEngine.Vision
{
	[Serializable]
	public class PropertyData
	{
		#region Enumerators

		public enum PropertyType
		{
			Boolean,
			Color,
			Float,
			Integer,
			Keyword,
			Preset,
		}

		#endregion

		#region Fields

		[SerializeField]
		private PropertyType m_type;

		[SerializeField]
		private string m_name;

		[SerializeField]
		private bool m_boolValue;

		[SerializeField]
		private Color m_colorValue;

		[SerializeField]
		private float m_floatValue;

		[SerializeField]
		private int m_intValue;

		[SerializeField]
		private PropertyPreset m_preset;

		#endregion

		#region Properties

		public string name => GetValue(m_name, m_preset?.data?.name ?? default);
		public PropertyType type => GetValue(m_type, m_preset?.data?.type ?? default);
		public bool boolValue => GetValue(m_boolValue, m_preset?.data?.boolValue ?? default);
		public Color colorValue => GetValue(m_colorValue, m_preset?.data?.colorValue ?? default);
		public float floatValue => GetValue(m_floatValue, m_preset?.data?.floatValue ?? default);
		public int intValue => GetValue(m_intValue, m_preset?.data?.intValue ?? default);
		public PropertyPreset preset => m_preset;

		#endregion

		#region Constructors

		public PropertyData()
		{ }

		public PropertyData(string name, PropertyType type)
		{
			m_name = name;
			m_type = type;
		}

		public PropertyData(string name, PropertyType type, object value)
			: this(name, type)
		{
			SetValue(value);
		}

		#endregion

		#region Methods

		private T GetValue<T>(T value, T presetValue)
		{
			if (m_type != PropertyType.Preset)
				return value;

			return presetValue;
		}

		public object GetValue()
		{
			switch (m_type)
			{
				case PropertyType.Boolean:
				case PropertyType.Keyword:
					return m_boolValue;

				case PropertyType.Color:
					return m_colorValue;

				case PropertyType.Float:
					return m_floatValue;

				case PropertyType.Integer:
					return m_intValue;
			}
			return null;
		}

		public void SetValue(object value)
		{
			switch (m_type)
			{
				case PropertyType.Boolean:
				case PropertyType.Keyword:
					m_boolValue = (bool)value;
					break;

				case PropertyType.Color:
					m_colorValue = (Color)value;
					break;

				case PropertyType.Float:
					m_floatValue = (float)value;
					break;

				case PropertyType.Integer:
					m_intValue = (int)value;
					break;

				case PropertyType.Preset:
					m_preset = (PropertyPreset)value;
					break;
			}
		}

		public object GetMaterialValue(Material material)
		{
			return GetMaterialValue(material, this);
		}

		private object GetMaterialValue(Material material, PropertyData propertyData)
		{
			switch (m_type)
			{
				case PropertyType.Boolean:
					return (bool)Convert.ChangeType(material.GetFloat(m_name), typeof(bool));

				case PropertyType.Color:
					return material.GetColor(m_name);

				case PropertyType.Float:
					return material.GetFloat(m_name);

				case PropertyType.Integer:
					return material.GetInteger(m_name);

				case PropertyType.Keyword:
					return material.IsKeywordEnabled(m_name);

				case PropertyType.Preset:
					return GetMaterialValue(material, propertyData.preset.data);
			}
			return null;
		}

		#endregion
	}
}