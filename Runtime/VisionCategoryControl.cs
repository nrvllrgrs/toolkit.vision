using System;
using System.Collections;
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
		private Transform m_target;

		[SerializeField]
		private RendererMaterialsMap m_includedRenderers = new();

		private Dictionary<Renderer, List<Material>> m_defaultMaterialMap;
		private Dictionary<Renderer, bool> m_defaultEnabledMap;

		/// <summary>
		/// Map of default properties given a material
		/// </summary>
		private Dictionary<Material, List<PropertyData>> m_defaultPropertiesMap;
		private bool m_special = false;

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

		public VisionCategory category
		{
			get => m_category;
			set
			{
				// No change, skip
				if (m_category == value)
					return;

				m_category = value;

				if (m_special)
				{
					// Reset to default properties
					EnableRenderer(null);
					SetMaterialProperty(null);
					SetMaterial(null);
				}

				UpdateMutableProperties();
				Changed(null, VisionModeManager.CastInstance.activeMode);
			}
		}

		public bool special
		{
			get => m_special;
			private set
			{
				// No change, skip
				if (m_special == value)
					return;

				m_special = value;

				if (value)
				{
					m_onSpecialized?.Invoke();
				}
				else
				{
					m_onNormalized?.Invoke();
				}
			}
		}

		/// <summary>
		/// Collection of default materials modified by vision mode
		/// </summary>
		public Material[] materials => m_defaultMaterialMap.SelectMany(x => x.Value).ToArray();

		public UnityEvent<VisionMode> onChanged => m_onChanged;
		public UnityEvent onNormalized => m_onNormalized;
		public UnityEvent onSpecialized => m_onSpecialized;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_target == null)
			{
				m_target = transform;
			}

			if (!m_includedRenderers.Any())
			{
				foreach (var renderer in m_target.GetComponentsInChildren<Renderer>(true))
				{
					m_includedRenderers.Add(renderer, new MaterialCollection(renderer.materials));
				}
			}

			UpdateMutableProperties();
		}

		private void UpdateMutableProperties()
		{
			m_defaultMaterialMap = new();
			m_defaultEnabledMap = new();
			m_defaultPropertiesMap = new();

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

			foreach (var p in m_includedRenderers)
			{
				List<Material> materials = new();
				List<PropertyData> properties = new();
				foreach (var material in (IEnumerable<Material>)p.Value)
				{
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

				m_defaultMaterialMap.Add(p.Key, materials);
				m_defaultEnabledMap.Add(p.Key, p.Key.enabled);
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

			special = overridden;
			m_onChanged?.Invoke(mode);
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

			// Set to default properties before setting new values
			var defaultMaterials = m_defaultPropertiesMap.Keys;
			foreach (var propertyDataList in m_defaultPropertiesMap.Values)
			{
				foreach (var propertyData in propertyDataList)
				{
					SetMaterialProperty(propertyData, defaultMaterials);
				}
			}

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

		#region Editor-Only
#if UNITY_EDITOR

		[ContextMenu("Populate Renderers")]
		private void PopulateRenderers()
		{
			m_includedRenderers.Clear();

			var target = m_target;
			if (target == null)
			{
				target = transform;
			}

			foreach (var renderer in target.GetComponentsInChildren<Renderer>(true))
			{
				List<Material> materials = new();
				renderer.GetSharedMaterials(materials);
				m_includedRenderers.Add(renderer, new MaterialCollection(materials));
			}
		}

#endif
		#endregion
	}

	[Serializable]
	public class RendererMaterialsMap : SerializableDictionary<Renderer, MaterialCollection>
	{ }

	[Serializable]
	public class MaterialCollection : IEnumerable<Material>
	{
		#region Fields

		[SerializeField]
		private List<Material> m_materials;

		#endregion

		#region Constructors

		public MaterialCollection()
		{ }

		public MaterialCollection(IEnumerable<Material> materials)
		{
			m_materials = new List<Material>(materials);
		}

		#endregion

		#region Methods

		public IEnumerator GetEnumerator() => m_materials.GetEnumerator();

		IEnumerator<Material> IEnumerable<Material>.GetEnumerator() => ((IEnumerable<Material>)m_materials).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_materials).GetEnumerator();

		#endregion
	}
}