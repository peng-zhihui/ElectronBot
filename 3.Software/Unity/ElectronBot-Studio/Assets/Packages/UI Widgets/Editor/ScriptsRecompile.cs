#if UNITY_EDITOR
namespace UIWidgets
{
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Forced recompilation if compilation was not done after Scripting Define Symbols was changed.
	/// </summary>
	public static class ScriptsRecompile
	{
		/// <summary>
		/// Text label for initial state.
		/// </summary>
		public const string StatusInitial = "initial";

		/// <summary>
		/// Text label for state after symbols added.
		/// </summary>
		public const string StatusSymbolsAdded = "symbols added";

		/// <summary>
		/// Text label for recompilation started state.
		/// </summary>
		public const string StatusRecompiledAdded = "recompiled label added";

		/// <summary>
		/// Text label for recompilation labels removed state.
		/// </summary>
		public const string StatusRecompileRemoved = "recompiled label removed";

		/// <summary>
		/// Check if forced recompilation required.
		/// </summary>
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void Run()
		{
#if UIWIDGETS_TMPRO_SUPPORT
			Check(ReferenceGUID.TMProStatus, ReferenceGUID.TMProSupport);
#endif

#if UIWIDGETS_DATABIND_SUPPORT
			Check(ReferenceGUID.DataBindStatus, ReferenceGUID.DataBindSupport);
#endif

#if I2_LOCALIZATION_SUPPORT
			Check(ReferenceGUID.I2LocalizationStatus, ReferenceGUID.I2LocalizationSupport);
#endif
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Conditional compilation.")]
		static void Check(string statusGUID, string[] guids)
		{
			var status = GetStatus(statusGUID);
			Log("check " + statusGUID + "; status: " + status);

			switch (status)
			{
				case StatusInitial:
					break;
				case StatusSymbolsAdded:
					Compatibility.ForceRecompileByGUID(guids);

					SetStatus(statusGUID, StatusRecompiledAdded);

					Log("Forced recompilation started.");
					break;
				case StatusRecompiledAdded:
					Compatibility.RemoveForceRecompileByGUID(guids);

					SetStatus(statusGUID, StatusRecompileRemoved);

					Log("Forced recompilation done; labels removing started");
					break;
				case StatusRecompileRemoved:
					SetStatus(statusGUID, StatusInitial);

					Log("Labels removed.");
					break;
				default:
					Debug.LogWarning("Unknown recompile status: " + status);
					break;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Conditional compilation.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message", Justification = "For the debug purposes.")]
		static void Log(string message)
		{
		}

		/// <summary>
		/// Get forced recompilation status from file with label.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <returns>Status.</returns>
		public static string GetStatus(string guid)
		{
			var asset = UtilitiesEditor.LoadAssetWithGUID<TextAsset>(guid);
			if (asset == null)
			{
				return StatusInitial;
			}

			return asset.text;
		}

		/// <summary>
		/// Set forced recompilation status to file with label.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <param name="status">Status.</param>
		public static void SetStatus(string guid, string status)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			File.WriteAllText(path, status);
		}
	}
}
#endif