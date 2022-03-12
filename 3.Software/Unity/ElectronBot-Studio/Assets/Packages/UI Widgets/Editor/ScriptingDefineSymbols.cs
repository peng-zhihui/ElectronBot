#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Scripting Define Symbols.
	/// </summary>
	public static class ScriptingDefineSymbols
	{
		static List<BuildTargetGroup> GetTargets()
		{
			var targets = new List<BuildTargetGroup>();
			foreach (var v in Enum.GetValues(typeof(BuildTarget)))
			{
				var target = (BuildTarget)v;
				var group = BuildPipeline.GetBuildTargetGroup(target);
				if (IsBuildTargetSupported(group, target) && !targets.Contains(group))
				{
					targets.Add(group);
				}
			}

			return targets;
		}

		static bool IsBuildTargetSupported(BuildTargetGroup group, BuildTarget target)
		{
#if UNITY_2018_1_OR_NEWER
			return BuildPipeline.IsBuildTargetSupported(group, target);
#else
			var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
			var buildTargetSupported = typeof(BuildPipeline).GetMethod("IsBuildTargetSupported", flags);
			return Convert.ToBoolean(buildTargetSupported.Invoke(null, new object[] { group, target }));
#endif
		}

		/// <summary>
		/// Add scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol to add.</param>
		public static void Add(string symbol)
		{
			foreach (var target in GetTargets())
			{
				var symbols = Symbols(target);

				if (symbols.Contains(symbol))
				{
					continue;
				}

				symbols.Add(symbol);

				Save(symbols, target);
			}
		}

		/// <summary>
		/// Remove scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol to remove.</param>
		public static void Remove(string symbol)
		{
			foreach (var target in GetTargets())
			{
				var symbols = Symbols(target);

				if (!symbols.Contains(symbol))
				{
					continue;
				}

				symbols.Remove(symbol);

				Save(symbols, target);
			}
		}

		static readonly char[] SymbolSeparator = new char[] { ';' };

		/// <summary>
		/// Get scripting define symbols.
		/// </summary>
		/// <returns>Scripting define symbols.</returns>
		[System.Obsolete("Replaced with Symbols(BuildTargetGroup target).")]
		public static HashSet<string> All()
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

			return new HashSet<string>(symbols.Split(SymbolSeparator));
		}

		/// <summary>
		/// Get scripting define symbols.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Scripting define symbols.</returns>
		public static HashSet<string> Symbols(BuildTargetGroup target)
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
			var result = new HashSet<string>();
			foreach (var symbol in symbols.Split(SymbolSeparator))
			{
				if (symbol.Length > 0)
				{
					result.Add(symbol);
				}
			}

			return result;
		}

		/// <summary>
		/// Check if symbol defined in scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol.</param>
		/// <returns>True if symbol defined; otherwise false.</returns>
		public static bool Contains(string symbol)
		{
			return Symbols(EditorUserBuildSettings.selectedBuildTargetGroup).Contains(symbol);
		}

		static void Save(HashSet<string> symbols, BuildTargetGroup target)
		{
			var arr = new string[symbols.Count];
			symbols.CopyTo(arr);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", arr));
			AssetDatabase.Refresh();
		}
	}
}
#endif