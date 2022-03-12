#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ColorHSV drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHSV))]
	public class ColorHSVPropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// The fields.
		/// </summary>
		protected List<string> Fields = new List<string>() { "H", "S", "V", "A" };

		/// <summary>
		/// The margin.
		/// </summary>
		protected float Margin = 3f;

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var width = (position.width - (Margin * (Fields.Count - 1))) / Fields.Count;
			foreach (var field in Fields)
			{
				var rect = new Rect(position.x, position.y, width, position.height);

				EditorGUI.PropertyField(rect, property.FindPropertyRelative(field), GUIContent.none);
				EditorGUI.LabelField(rect, new GUIContent(string.Empty, field));

				position.x += width + Margin;
			}

			EditorGUI.EndProperty();
		}
	}
}
#endif