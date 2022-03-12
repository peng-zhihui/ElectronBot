namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// TabIcons drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(StyleText))]
	public class StyleTextPropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// The indent.
		/// </summary>
		protected const float Indent = 16;

		/// <summary>
		/// The height.
		/// </summary>
		protected const float Height = 18;

		/// <summary>
		/// The empty space between properties.
		/// </summary>
		protected const float EmptySpace = 1;

		/// <summary>
		/// The height of the header.
		/// </summary>
		protected const float HeaderHeight = 24;

		/// <summary>
		/// The width of the checkbox.
		/// </summary>
		protected const float CheckboxWidth = 16;

		/// <summary>
		/// The properties.
		/// </summary>
		protected static List<string> Properties = new List<string>()
		{
			"Font",
			"FontStyle",
			"Size",
			"LineSpacing",
			"RichText",
			"Alignment",
			"HorizontalOverflow",
			"VerticalOverflow",
			"BestFit",
			"Color",
			"Material",
		};

		/// <summary>
		/// The properties with header.
		/// </summary>
		protected static List<string> WithHeader = new List<string>()
		{
			"Font",
			"Alignment",
			"Color",
		};

		/// <summary>
		/// Is opened?
		/// </summary>
		protected bool IsOpened = false;

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = DrawFoldout(position, label);

			if (IsOpened)
			{
				foreach (var field_name in Properties)
				{
					position = DrawToggableProperty(position, field_name, property);
				}
			}

			EditorGUI.EndProperty();
		}

		/// <summary>
		/// Is property with name displayed with header?
		/// </summary>
		/// <returns><c>true</c> if property have header; otherwise, <c>false</c>.</returns>
		/// <param name="name">Name.</param>
		protected static bool IsHeader(string name)
		{
			return WithHeader.Contains(name);
		}

		/// <summary>
		/// Draws the toggable property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="name">Name.</param>
		/// <param name="property">Property.</param>
		protected static Rect DrawToggableProperty(Rect position, string name, SerializedProperty property)
		{
			var change = property.FindPropertyRelative("Change" + name);
			var change_height = IsHeader(name) ? HeaderHeight + Height : Height;
			var total_width = position.width;
			var change_width = EditorGUIUtility.labelWidth + CheckboxWidth;
			var change_rect = new Rect(position.x, position.y, change_width, change_height);

			EditorGUI.PropertyField(change_rect, change, new GUIContent(name));
			EditorGUI.LabelField(change_rect, new GUIContent(string.Empty, "Change " + name));

			position.y += change_height;

			if (change.boolValue)
			{
				position.y -= Height;
				position.x += change_width;
				position.width = total_width - change_width - CheckboxWidth;

				if (name == "Font")
				{
					position = DrawProperty(position, property.FindPropertyRelative(name));
					position = DrawProperty(position, property.FindPropertyRelative(name + "TMPro"));
				}
				else if (name == "BestFit")
				{
					var bestfit = property.FindPropertyRelative(name);
					position = DrawProperty(position, bestfit);

					if (bestfit.boolValue)
					{
						position = DrawPropertyIndent(position, property.FindPropertyRelative("MinSize"), -change_width + CheckboxWidth);
						position = DrawPropertyIndent(position, property.FindPropertyRelative("MaxSize"), -change_width + CheckboxWidth);
					}
				}
				else
				{
					position = DrawProperty(position, property.FindPropertyRelative(name));
				}

				position.x -= change_width;
				position.width = total_width;
			}
			else
			{
				position.y += EmptySpace;
			}

			return position;
		}

		/// <summary>
		/// Draws the property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="field">Field.</param>
		protected static Rect DrawProperty(Rect position, SerializedProperty field)
		{
			var rect = new Rect(position.x, position.y, position.width, Height);
			EditorGUI.PropertyField(rect, field, GUIContent.none);

			position.y += rect.height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Draws the property with indent.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="field">Field.</param>
		/// <param name="indent">Property indent.</param>
		protected static Rect DrawPropertyIndent(Rect position, SerializedProperty field, float indent = 0)
		{
			var rect = new Rect(position.x + indent, position.y, position.width - indent, Height);
			EditorGUI.PropertyField(rect, field);

			position.y += rect.height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Draws the foldout.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="label">Label.</param>
		protected Rect DrawFoldout(Rect position, GUIContent label)
		{
			position.height = Height;
			IsOpened = EditorGUI.Foldout(position, IsOpened, label, true);

			position.x += Indent;
			position.y += Height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <returns>The height of the property.</returns>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var result = 16f;

			if (!IsOpened)
			{
				return result + (EmptySpace * 2);
			}

			foreach (var field_name in Properties)
			{
				result += GetPropertyHeight(field_name, property);
			}

			return result + (EmptySpace * 2);
		}

		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <returns>The height of the property.</returns>
		/// <param name="name">Name.</param>
		/// <param name="property">Property.</param>
		protected static float GetPropertyHeight(string name, SerializedProperty property)
		{
			var result = WithHeader.Contains(name) ? HeaderHeight + Height : Height;
			var change = property.FindPropertyRelative("Change" + name);

			if (change.boolValue)
			{
				result -= Height;

				if (name == "Font")
				{
					result += (Height + EmptySpace) * 2;
				}
				else if (name == "BestFit")
				{
					result += Height + EmptySpace;

					var bestfit = property.FindPropertyRelative(name);
					if (bestfit.boolValue)
					{
						result += (Height + EmptySpace) * 2;
					}
				}
				else
				{
					result += Height + EmptySpace;
				}
			}
			else
			{
				result += EmptySpace;
			}

			return result;
		}
	}
}