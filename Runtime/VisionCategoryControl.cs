using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static ToolkitEngine.Vision.VisionCategory;

namespace ToolkitEngine.Vision
{
	public class VisionCategoryControl : MonoBehaviour, IVisionModeEvents
    {
		#region Fields

		[SerializeField]
		private VisionCategory m_category;

		[SerializeField]
		private List<Renderer> m_ignoredRenderers = new();

		[SerializeField]
		private List<Material> m_ignoredMaterials = new();

		private Dictionary<Renderer, List<Material>> m_defaultMaterialMap = new();
		private Dictionary<Renderer, bool> m_defaultEnabledMap = new();

		/// <summary>
		/// Map of default properties given a material
		/// </summary>
		private Dictionary<Material, List<PropertyData>> m_defaultPropertiesMap = new();

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		[SerializeField]
		private UnityEvent m_onNormalized;

		[SerializeField]
		private UnityEvent m_onSpecialized;

		#endregion

		#region Properties

		public VisionCategory category => m_category;
		public UnityEvent<VisionMode> onChanged => m_onChanged;
		public UnityEvent onNormalized => m_onNormalized;
		public UnityEvent onSpecialized => m_onSpecialized;

		#endregion

		#region Methods

		private void Awake()
		{
			// Get list of properties that can change from any vision mode
			HashSet<PropertyData> mutableProperties = new();
			foreach (var visionMode in VisionModeManager.CastInstance.Config.modes)
			{
				if (!m_category.TryGetProperties(visionMode, out var properties))
					continue;

				foreach (var propertyData in properties)
				{
					if (string.IsNullOrWhiteSpace(propertyData.name))
						continue;

					mutableProperties.Add(new PropertyData(
						propertyData.name,
						propertyData.type));
				}
			}

			foreach (var renderer in GetComponentsInChildren<Renderer>(true))
			{
				if (m_ignoredRenderers.Contains(renderer))
					continue;

				List<Material> materials = new();
				List<PropertyData> properties = new();
				foreach (var material in renderer.materials)
				{
					if (m_ignoredMaterials.Contains(material))
						continue;

					materials.Add(material);

					if (!m_defaultPropertiesMap.ContainsKey(material))
					{
						// Get default value for mutable properties of material
						foreach (var propertyData in mutableProperties)
						{
							if (!material.HasProperty(propertyData.name))
								continue;

							properties.Add(new PropertyData(
								propertyData.name,
								propertyData.type,
								propertyData.GetMaterialValue(material)));
						}

						if (properties.Count > 0)
						{
							m_defaultPropertiesMap.Add(material, properties);
						}
					}
				}

				m_defaultMaterialMap.Add(renderer, materials);
				m_defaultEnabledMap.Add(renderer, renderer.enabled);
			}
		}

		private void OnEnable()
		{
			VisionModeManager.CastInstance.Changed += Changed;
			Changed(null, VisionModeManager.CastInstance.activeMode);
		}

		private void OnDisable()
		{
			VisionModeManager.CastInstance.Changed -= Changed;
			Changed(null, null);
		}

		private void Changed(object sender, VisionMode mode)
		{
			if (m_category == null)
				return;

			bool overridden = false;
			if (mode != null)
			{
				overridden |= SetMaterial(mode);
				overridden |= SetMaterialProperty(mode);
				overridden |= EnableRenderer(mode);
			}
			else
			{
				overridden |= EnableRenderer(null);
				overridden |= SetMaterialProperty(mode = null);
				overridden |= SetMaterial(null);
			}

			m_onChanged?.Invoke(mode);
			if (overridden)
			{
				m_onSpecialized?.Invoke();
			}
			else
			{
				m_onNormalized?.Invoke();
			}
		}

		private bool SetMaterial(VisionMode mode)
		{
			if ((m_category.renderType & RenderType.Replace) == 0)
				return false;

			if (mode != null && m_category.TryGetMaterial(mode, out Material material) && material != null)
			{
				foreach (var p in m_defaultMaterialMap)
				{
					p.Key.SetMaterials(Enumerable.Repeat(material, p.Value.Count).ToList());
				}
				return true;
			}
			else
			{
				foreach (var p in m_defaultMaterialMap)
				{
					p.Key.SetMaterials(p.Value);
				}
				return false;
			}
		}

		private bool SetMaterialProperty(VisionMode mode)
		{
			if ((m_category.renderType & RenderType.Property) == 0)
				return false;

			if (mode != null && m_category.TryGetProperties(mode, out var properties))
			{
				foreach (var propertyData in properties)
				{
					foreach (var materials in m_defaultMaterialMap.Values)
					{
						SetMaterialProperty(propertyData, materials);
					}
				}
				return true;
			}
			// Set to default properties
			else
			{
				var materials = m_defaultPropertiesMap.Keys;
				foreach (var propertyDataList in m_defaultPropertiesMap.Values)
				{
					foreach (var propertyData in propertyDataList)
					{
						SetMaterialProperty(propertyData, materials);
					}
				}
			}
			return false;
		}

		private void SetMaterialProperty(PropertyData propertyData, IEnumerable<Material> materials)
		{
			if (propertyData == null)
				return;

			switch (propertyData.type)
			{
				case PropertyData.PropertyType.Color:
					SetMaterialProperty(propertyData, materials, (material, propertyData) =>
					{
						material.SetColor(propertyData.name, propertyData.colorValue);
					});
					break;

				case PropertyData.PropertyType.Boolean:
					SetMaterialProperty(propertyData, materials, (material, propertyData) =>
					{
						material.SetFloat(propertyData.name, Convert.ToSingle(propertyData.boolValue));
					});
					break;

				case PropertyData.PropertyType.Integer:
					SetMaterialProperty(propertyData, materials, (material, propertyData) =>
					{
						material.SetInteger(propertyData.name, propertyData.intValue);
					});
					break;

				case PropertyData.PropertyType.Float:
					SetMaterialProperty(propertyData, materials, (material, propertyData) =>
					{
						material.SetFloat(propertyData.name, propertyData.floatValue);
					});
					break;

				case PropertyData.PropertyType.Keyword:
					SetMaterialProperty(propertyData, materials, (material, propertyData) =>
					{
						if (propertyData.boolValue)
						{
							material.EnableKeyword(propertyData.name);
						}
						else
						{
							material.DisableKeyword(propertyData.name);
						}
					});
					break;

				case PropertyData.PropertyType.Preset:
					SetMaterialProperty(propertyData.preset.data, materials);
					break;
			}
		}

		private void SetMaterialProperty(PropertyData propertyData, IEnumerable<Material> materials, Action<Material, PropertyData> action)
		{
			foreach (var material in materials)
			{
				action.Invoke(material, propertyData);
			}
		}

		private bool EnableRenderer(VisionMode mode)
		{
			if ((m_category.renderType & RenderType.Toggle) == 0)
				return false;

			if (mode != null)
			{
				bool specialized = false;
				bool enabled = m_category.GetEnabled(mode);
				foreach (var p in m_defaultEnabledMap)
				{
					p.Key.enabled = enabled;
					specialized |= enabled != p.Value;
				}
				return specialized;
			}
			else
			{
				foreach (var p in m_defaultEnabledMap)
				{
					p.Key.enabled = p.Value;
				}
				return false;
			}
		}

		#endregion
	}
}