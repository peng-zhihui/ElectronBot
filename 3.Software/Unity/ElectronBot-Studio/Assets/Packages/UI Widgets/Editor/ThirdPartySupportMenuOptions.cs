#if UNITY_EDITOR
namespace UIWidgets
{
	using System.IO;
	using UnityEditor;

	/// <summary>
	/// Menu options to toggle third party packages support.
	/// </summary>
	[InitializeOnLoad]
	public static class ThirdPartySupportMenuOptions
	{
		/// <summary>
		/// Upgrade widgets at scene.
		/// </summary>
		// [MenuItem("Window/New UI Widgets/Upgrade Widgets at Scene")]
		public static void Upgrade()
		{
#if UNITY_2017_1_OR_NEWER
			var gameobjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var go in gameobjects)
			{
				var components = go.GetComponentsInChildren<IUpgradeable>();
				foreach (var component in components)
				{
					component.Upgrade();
				}
			}
#endif
		}

		static ThirdPartySupportMenuOptions()
		{
			FixAssemblyDefinitions();
#if UNITY_2017_4_OR_LATER
			EditorApplication.projectChanged += FixAssemblyDefinitions;
#endif
		}

#if UNITY_5_6_OR_NEWER
		#region DataBindSupport
		const string DataBindSupport = "UIWIDGETS_DATABIND_SUPPORT";

		/// <summary>
		/// Is Data Bind for Unity installed?
		/// </summary>
		public static bool DataBindInstalled
		{
			get
			{
				return UtilitiesEditor.GetType("Slash.Unity.DataBind.Core.Presentation.DataProvider") != null;
			}
		}

		/// <summary>
		/// Enable DataBind support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable Data Bind Support", false, 1000)]
#endif
		public static void EnableDataBindSupport()
		{
			if (CanEnableDataBindSupport())
			{
				var root = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(ReferenceGUID.ScriptsFolder));

				var current_path = AssetDatabase.GUIDToAssetPath(ReferenceGUID.DataBindFolder);
				var new_path = root + "/" + Path.GetFileName(current_path);
				if (current_path != new_path)
				{
					AssetDatabase.MoveAsset(current_path, new_path);
				}

				ScriptingDefineSymbols.Add(DataBindSupport);

				ScriptsRecompile.SetStatus(ReferenceGUID.DataBindStatus, ScriptsRecompile.StatusSymbolsAdded);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can enable DataBind support?
		/// </summary>
		/// <returns>true if DataBind installed and support not enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable Data Bind Support", true, 1000)]
#endif
		public static bool CanEnableDataBindSupport()
		{
			if (!DataBindInstalled)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(DataBindSupport);
		}

		/// <summary>
		/// Disable DataBind support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable Data Bind Support", false, 1001)]
#endif
		public static void DisableDataBindSupport()
		{
			if (CanDisableDataBindSupport())
			{
				var root = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(ReferenceGUID.ScriptsFolder));

				var current_path = AssetDatabase.GUIDToAssetPath(ReferenceGUID.DataBindFolder);
				var new_path = root + "/Scripts/ThirdPartySupport/" + Path.GetFileName(current_path);
				if (current_path != new_path)
				{
					AssetDatabase.MoveAsset(current_path, new_path);
				}

				ScriptingDefineSymbols.Remove(DataBindSupport);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can disable DataBind support?
		/// </summary>
		/// <returns>true if DataBind support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable Data Bind Support", true, 1001)]
#endif
		public static bool CanDisableDataBindSupport()
		{
			return ScriptingDefineSymbols.Contains(DataBindSupport);
		}

		/// <summary>
		/// Recompile DataBind support package?
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile Data Bind Support", false, 1002)]
#endif
		public static void RecompileDataBindSupport()
		{
			ScriptsRecompile.SetStatus(ReferenceGUID.DataBindStatus, ScriptsRecompile.StatusSymbolsAdded);
		}

		/// <summary>
		/// Can recompile DataBind support package?
		/// </summary>
		/// <returns>true if DataBind support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile Data Bind Support", true, 1002)]
#endif
		public static bool CanRecompileDataBindSupport()
		{
			return ScriptingDefineSymbols.Contains(DataBindSupport);
		}
		#endregion
#endif

		#region TMProSupport

		const string TMProSupport = "UIWIDGETS_TMPRO_SUPPORT";

		const string TMProAssemblies = "l:UiwidgetsTMProRequiredAssemblyDefinition";

		const string TMProPackage = "Unity.TextMeshPro";

		/// <summary>
		/// Is TextMeshPro installed?
		/// </summary>
		public static bool TMProInstalled
		{
			get
			{
				return !string.IsNullOrEmpty(Compatibility.GetTMProVersion());
			}
		}

		/// <summary>
		/// Enable TMPro Support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable TextMeshPro Support", false, 1003)]
#endif
		public static void EnableTMProSupport()
		{
			if (CanEnableTMProSupport())
			{
				ScriptingDefineSymbols.Add(TMProSupport);
				AssemblyDefinitionsEditor.Add(TMProAssemblies, TMProPackage);

				ScriptsRecompile.SetStatus(ReferenceGUID.TMProStatus, ScriptsRecompile.StatusSymbolsAdded);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can enable TMPro support?
		/// </summary>
		/// <returns>true if TMPro installed and support not enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable TextMeshPro Support", true, 1003)]
#endif
		public static bool CanEnableTMProSupport()
		{
			if (!TMProInstalled)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(TMProSupport);
		}

		/// <summary>
		/// Disable TMPro support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable TextMeshPro Support", false, 1004)]
#endif
		public static void DisableTMProSupport()
		{
			if (CanDisableTMProSupport())
			{
				ScriptingDefineSymbols.Remove(TMProSupport);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can disable TMPro support?
		/// </summary>
		/// <returns>true if TMPro support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable TextMeshPro Support", true, 1004)]
#endif
		public static bool CanDisableTMProSupport()
		{
			return ScriptingDefineSymbols.Contains(TMProSupport);
		}

		/// <summary>
		/// Recompile TMPro support package.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile TextMeshPro Support", false, 1005)]
#endif
		public static void RecompileTMProSupport()
		{
			ScriptsRecompile.SetStatus(ReferenceGUID.TMProStatus, ScriptsRecompile.StatusSymbolsAdded);
		}

		/// <summary>
		/// Can recompile TMPro support package?
		/// </summary>
		/// <returns>true if TMPro support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile TextMeshPro Support", true, 1005)]
#endif
		public static bool CanRecompileTMProSupport()
		{
			return ScriptingDefineSymbols.Contains(TMProSupport);
		}
		#endregion

		#region I2 localization
		const string I2LocalizationSupport = "I2_LOCALIZATION_SUPPORT";

		/// <summary>
		/// Is I2 Localization installed?
		/// </summary>
		public static bool I2LocalizationInstalled
		{
			get
			{
				return UtilitiesEditor.GetType("I2.Loc.LocalizationManager") != null;
			}
		}

		/// <summary>
		/// Enable I2 Localization support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable I2 Localization Support", false, 1006)]
#endif
		public static void EnableI2LocalizationSupport()
		{
			if (CanEnableI2LocalizationSupport())
			{
				ScriptingDefineSymbols.Add(I2LocalizationSupport);

				ScriptsRecompile.SetStatus(ReferenceGUID.I2LocalizationStatus, ScriptsRecompile.StatusSymbolsAdded);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can enable I2 Localization support?
		/// </summary>
		/// <returns>true if I2 Localization installed and support not enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable I2 Localization Support", true, 1006)]
#endif
		public static bool CanEnableI2LocalizationSupport()
		{
			if (!I2LocalizationInstalled)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(I2LocalizationSupport);
		}

		/// <summary>
		/// Disable I2 Localization support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable I2 Localization Support", false, 1007)]
#endif
		public static void DisableI2LocalizationSupport()
		{
			if (CanDisableI2LocalizationSupport())
			{
				ScriptingDefineSymbols.Remove(I2LocalizationSupport);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can disable I2 Localization support?
		/// </summary>
		/// <returns>true if I2 Localization support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable I2 Localization Support", true, 1007)]
#endif
		public static bool CanDisableI2LocalizationSupport()
		{
			return ScriptingDefineSymbols.Contains(I2LocalizationSupport);
		}

		/// <summary>
		/// Recompile I2 Localization support package?
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile I2 Localization Support", false, 1008)]
#endif
		public static void RecompileI2LocalizationSupport()
		{
			ScriptsRecompile.SetStatus(ReferenceGUID.I2LocalizationStatus, ScriptsRecompile.StatusSymbolsAdded);
		}

		/// <summary>
		/// Can recompile I2 Localization support package?
		/// </summary>
		/// <returns>true if I2 Localization support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Recompile I2 Localization Support", true, 1008)]
#endif
		public static bool CanRecompileI2LocalizationSupport()
		{
			return ScriptingDefineSymbols.Contains(I2LocalizationSupport);
		}
		#endregion

		/// <summary>
		/// Fix assembly definitions after package was re-imported.
		/// </summary>
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void FixAssemblyDefinitions()
		{
			if (!ScriptingDefineSymbols.Contains(TMProSupport))
			{
				return;
			}

			if (AssemblyDefinitionsEditor.Contains(TMProAssemblies, TMProPackage))
			{
				return;
			}

			AssemblyDefinitionsEditor.Add(TMProAssemblies, TMProPackage);
			ScriptsRecompile.SetStatus(ReferenceGUID.TMProStatus, ScriptsRecompile.StatusSymbolsAdded);
			AssetDatabase.Refresh();
		}
	}
}
#endif