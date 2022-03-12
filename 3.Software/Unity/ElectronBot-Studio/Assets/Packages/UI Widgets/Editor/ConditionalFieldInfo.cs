#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Field info for conditional display.
	/// </summary>
	public class ConditionalFieldInfo
	{
		/// <summary>
		/// Indent level.
		/// </summary>
		public int Indent;

		/// <summary>
		/// Field name.
		/// </summary>
		public string Name;

		/// <summary>
		/// Conditions to display field.
		/// </summary>
		public Dictionary<string, Func<SerializedProperty, bool>> Conditions = new Dictionary<string, Func<SerializedProperty, bool>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalFieldInfo"/> class.
		/// </summary>
		/// <param name="fieldName">Field name.</param>
		/// <param name="indent">Indent level.</param>
		/// <param name="conditions">Conditions to display field.</param>
		public ConditionalFieldInfo(string fieldName, int indent = 0, Dictionary<string, Func<SerializedProperty, bool>> conditions = null)
		{
			Name = fieldName;
			Indent = indent;

			if (conditions != null)
			{
				Conditions = conditions;
			}
		}
	}
}
#endif