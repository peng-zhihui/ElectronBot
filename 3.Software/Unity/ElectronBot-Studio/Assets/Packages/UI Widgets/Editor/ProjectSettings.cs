#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Project settings.
	/// </summary>
	[InitializeOnLoad]
	public static class ProjectSettings
	{
		static ProjectSettings()
		{
			ThirdPartySupportMenuOptions.FixAssemblyDefinitions();
		}

		class Styles
		{
			private Styles()
			{
			}

			/// <summary>
			/// TextMeshPro label.
			/// </summary>
			public static GUIContent TMProLabel = new GUIContent("TextMeshPro Support");

			/// <summary>
			/// DataBind label.
			/// </summary>
			public static GUIContent DataBindLabel = new GUIContent("Data Bind for Unity Support");

			/// <summary>
			/// I2Localization label.
			/// </summary>
			public static GUIContent I2LocalizationLabel = new GUIContent("I2 Localization Support");
		}

		static string TMProStatus
		{
			get
			{
				if (!ThirdPartySupportMenuOptions.TMProInstalled)
				{
					return "TextMeshPro not installed.";
				}

				if (ThirdPartySupportMenuOptions.CanDisableTMProSupport())
				{
					return "Enabled";
				}

				return "Disabled";
			}
		}

		static GUILayoutOption[] nameOptions = new GUILayoutOption[] { GUILayout.Width(170) };

		static GUILayoutOption[] statusOptions = new GUILayoutOption[] { GUILayout.Width(200) };

		static void TMProBlock()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Styles.TMProLabel, nameOptions);
			EditorGUILayout.LabelField(TMProStatus, statusOptions);

			if (ThirdPartySupportMenuOptions.TMProInstalled)
			{
				if (ThirdPartySupportMenuOptions.CanDisableTMProSupport())
				{
					if (GUILayout.Button("Disable"))
					{
						ThirdPartySupportMenuOptions.DisableTMProSupport();
					}

					if (GUILayout.Button("Recompile"))
					{
						ThirdPartySupportMenuOptions.RecompileTMProSupport();
					}
				}
				else
				{
					if (GUILayout.Button("Enable"))
					{
						ThirdPartySupportMenuOptions.EnableTMProSupport();
					}
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		static string DataBindStatus
		{
			get
			{
				if (!ThirdPartySupportMenuOptions.DataBindInstalled)
				{
					return "Data Bind for Unity not installed.";
				}

				if (ThirdPartySupportMenuOptions.CanDisableDataBindSupport())
				{
					return "Enabled";
				}

				return "Disabled";
			}
		}

		static void DataBindBlock()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Styles.DataBindLabel, nameOptions);
			EditorGUILayout.LabelField(DataBindStatus, statusOptions);

			if (ThirdPartySupportMenuOptions.DataBindInstalled)
			{
				if (ThirdPartySupportMenuOptions.CanDisableDataBindSupport())
				{
					if (GUILayout.Button("Disable"))
					{
						ThirdPartySupportMenuOptions.DisableDataBindSupport();
					}

					if (GUILayout.Button("Recompile"))
					{
						ThirdPartySupportMenuOptions.RecompileDataBindSupport();
					}
				}
				else
				{
					if (GUILayout.Button("Enable"))
					{
						ThirdPartySupportMenuOptions.EnableDataBindSupport();
					}
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		static string I2LocalizationStatus
		{
			get
			{
				if (!ThirdPartySupportMenuOptions.I2LocalizationInstalled)
				{
					return "I2 Localization not installed.";
				}

				if (ThirdPartySupportMenuOptions.CanDisableI2LocalizationSupport())
				{
					return "Enabled";
				}

				return "Disabled";
			}
		}

		static void I2LocalizationBlock()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Styles.I2LocalizationLabel, nameOptions);
			EditorGUILayout.LabelField(I2LocalizationStatus, statusOptions);

			if (ThirdPartySupportMenuOptions.I2LocalizationInstalled)
			{
				if (ThirdPartySupportMenuOptions.CanDisableI2LocalizationSupport())
				{
					if (GUILayout.Button("Disable"))
					{
						ThirdPartySupportMenuOptions.DisableI2LocalizationSupport();
					}

					if (GUILayout.Button("Recompile"))
					{
						ThirdPartySupportMenuOptions.RecompileI2LocalizationSupport();
					}
				}
				else
				{
					if (GUILayout.Button("Enable"))
					{
						ThirdPartySupportMenuOptions.EnableI2LocalizationSupport();
					}
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Create settings provider.
		/// </summary>
		/// <returns>Settings provider.</returns>
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			var provider = new SettingsProvider("Project/New UI Widgets", SettingsScope.Project)
			{
				guiHandler = (searchContext) =>
				{
					TMProBlock();
					DataBindBlock();
					I2LocalizationBlock();
				},

				keywords = SettingsProvider.GetSearchKeywordsFromGUIContentProperties<Styles>(),
			};

			return provider;
		}
	}
}
#endif