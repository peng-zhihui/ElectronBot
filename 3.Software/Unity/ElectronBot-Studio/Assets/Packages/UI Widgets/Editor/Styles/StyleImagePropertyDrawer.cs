namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Property drawer for StyleImage.
	/// </summary>
	[CustomPropertyDrawer(typeof(StyleImage))]
	public class StyleImagePropertyDrawer : PropertyDrawer
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
		/// The height.
		/// </summary>
		protected const float EmptySpace = 2;

		/// <summary>
		/// The indent level.
		/// </summary>
		protected int IndentLevel = 0;

		/// <summary>
		/// The labels.
		/// </summary>
		protected static Dictionary<string, string> Labels = new Dictionary<string, string>()
		{
			{ "Sprite", "Sprite" },
			{ "Color", "Color" },
			{ "ImageType", "Image Type" },
			{ "PreserveAspect", "Preserve Aspect" },
			{ "FillCenter", "Fill Center" },
			{ "FillMethod", "Fill Method" },
			{ "FillOrigin", "Fill Origin" },
			{ "FillAmount", "Fill Amount" },
			{ "FillClockwise", "Clockwise" },
			{ "Material", "Material" },
			{ "PixelsPerUnitMultiplier", "Pixels Per Unit Multiplier" },
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

			IndentLevel = 0;

			if (IsOpened)
			{
				IndentLevel++;

				var sprite = property.FindPropertyRelative("Sprite");
				var image_type = property.FindPropertyRelative("ImageType");

				EditorGUI.BeginChangeCheck();
				position = DrawProperty(position, sprite);
				if (EditorGUI.EndChangeCheck())
				{
					var new_sprite = sprite.objectReferenceValue as Sprite;
					if (new_sprite != null)
					{
						var old_type = (Image.Type)image_type.enumValueIndex;
						if (new_sprite.border.SqrMagnitude() > 0)
						{
							image_type.enumValueIndex = (int)Image.Type.Sliced;
						}
						else if (old_type == Image.Type.Sliced)
						{
							image_type.enumValueIndex = (int)Image.Type.Simple;
						}
					}
				}

				position = DrawProperty(position, property.FindPropertyRelative("Color"));
				position = DrawProperty(position, property.FindPropertyRelative("Material"));

				if (sprite.objectReferenceValue is Sprite)
				{
					position = DrawProperty(position, image_type);

					DrawImageProperties(position, (Image.Type)image_type.enumValueIndex, property);
				}

				IndentLevel--;
			}

			EditorGUI.EndProperty();
		}

		/// <summary>
		/// Draw image properties.
		/// </summary>
		/// <param name="position">Start position.</param>
		/// <param name="imageType">Image type.</param>
		/// <param name="property">Property.</param>
		/// <returns>New position.</returns>
		protected Rect DrawImageProperties(Rect position, Image.Type imageType, SerializedProperty property)
		{
			var fill_method_property = property.FindPropertyRelative("FillMethod");
			var fill_origin = property.FindPropertyRelative("FillOrigin");

			IndentLevel++;

			switch (imageType)
			{
				case Image.Type.Simple:
					position = DrawProperty(position, property.FindPropertyRelative("PreserveAspect"));
					break;
				case Image.Type.Sliced:
					position = DrawProperty(position, property.FindPropertyRelative("FillCenter"));
					position = DrawProperty(position, property.FindPropertyRelative("PixelsPerUnitMultiplier"));
					break;
				case Image.Type.Tiled:
					position = DrawProperty(position, property.FindPropertyRelative("FillCenter"));
					position = DrawProperty(position, property.FindPropertyRelative("PixelsPerUnitMultiplier"));
					break;
				case Image.Type.Filled:
					EditorGUI.BeginChangeCheck();
					position = DrawProperty(position, fill_method_property);
					var fill_method = (Image.FillMethod)fill_method_property.enumValueIndex;

					if (EditorGUI.EndChangeCheck())
					{
						fill_origin.intValue = 0;
					}

					position = DrawFillOrigin(position, fill_method, fill_origin);

					position = DrawProperty(position, property.FindPropertyRelative("FillAmount"));

					if (fill_method == Image.FillMethod.Radial90 || fill_method == Image.FillMethod.Radial180 || fill_method == Image.FillMethod.Radial360)
					{
						position = DrawProperty(position, property.FindPropertyRelative("FillClockwise"));
					}

					position = DrawProperty(position, property.FindPropertyRelative("PreserveAspect"));
					break;
			}

			IndentLevel--;

			return position;
		}

		/// <summary>
		/// Get image properties height.
		/// </summary>
		/// <param name="imageType">Image type.</param>
		/// <param name="property">Property.</param>
		/// <returns>Height.</returns>
		protected static float ImagePropertiesHeight(Image.Type imageType, SerializedProperty property)
		{
			var result = 0f;
			var fill_method_property = property.FindPropertyRelative("FillMethod");
			var fill_origin = property.FindPropertyRelative("FillOrigin");

			switch (imageType)
			{
				case Image.Type.Simple:
					result += GetPropertyHeight(property.FindPropertyRelative("PreserveAspect"));
					break;
				case Image.Type.Sliced:
					result += GetPropertyHeight(property.FindPropertyRelative("FillCenter"));
					result += GetPropertyHeight(property.FindPropertyRelative("PixelsPerUnitMultiplier"));
					break;
				case Image.Type.Tiled:
					result += GetPropertyHeight(property.FindPropertyRelative("FillCenter"));
					result += GetPropertyHeight(property.FindPropertyRelative("PixelsPerUnitMultiplier"));
					break;
				case Image.Type.Filled:
					result += GetPropertyHeight(fill_method_property);
					result += GetPropertyHeight(fill_origin);
					result += GetPropertyHeight(property.FindPropertyRelative("FillAmount"));

					var fill_method = (Image.FillMethod)fill_method_property.enumValueIndex;
					if (fill_method == Image.FillMethod.Radial90 || fill_method == Image.FillMethod.Radial180 || fill_method == Image.FillMethod.Radial360)
					{
						result += GetPropertyHeight(property.FindPropertyRelative("FillClockwise"));
					}

					result += GetPropertyHeight(property.FindPropertyRelative("PreserveAspect"));
					break;
			}

			return result;
		}

		/// <summary>
		/// Draws the Fill Origin property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="fillMethod">Fill method.</param>
		/// <param name="fillOrigin">Fill origin.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		protected Rect DrawFillOrigin(Rect position, Image.FillMethod fillMethod, SerializedProperty fillOrigin)
		{
			var height = EditorGUI.GetPropertyHeight(fillOrigin, new GUIContent(Labels[fillOrigin.name]));
			var indent = Indent * IndentLevel;
			var label_width = EditorGUIUtility.labelWidth - indent;
			var label_rect = new Rect(position.x + indent, position.y, label_width, height);
			EditorGUI.SelectableLabel(label_rect, Labels[fillOrigin.name]);

			var rect = new Rect(position.x + indent + label_width, position.y, position.width - indent - label_width, height);

			switch (fillMethod)
			{
				case Image.FillMethod.Horizontal:
					fillOrigin.intValue = (int)(Image.OriginHorizontal)EditorGUI.EnumPopup(rect, GUIContent.none, (Image.OriginHorizontal)fillOrigin.intValue);
					break;
				case Image.FillMethod.Vertical:
					fillOrigin.intValue = (int)(Image.OriginVertical)EditorGUI.EnumPopup(rect, GUIContent.none, (Image.OriginVertical)fillOrigin.intValue);
					break;
				case Image.FillMethod.Radial90:
					fillOrigin.intValue = (int)(Image.Origin90)EditorGUI.EnumPopup(rect, GUIContent.none, (Image.Origin90)fillOrigin.intValue);
					break;
				case Image.FillMethod.Radial180:
					fillOrigin.intValue = (int)(Image.Origin180)EditorGUI.EnumPopup(rect, GUIContent.none, (Image.Origin180)fillOrigin.intValue);
					break;
				case Image.FillMethod.Radial360:
					fillOrigin.intValue = (int)(Image.Origin360)EditorGUI.EnumPopup(rect, GUIContent.none, (Image.Origin360)fillOrigin.intValue);
					break;
			}

			position.y += rect.height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Draws the property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="field">Field.</param>
		protected Rect DrawProperty(Rect position, SerializedProperty field)
		{
			var height = EditorGUI.GetPropertyHeight(field, new GUIContent(Labels[field.name]));
			var indent = Indent * IndentLevel;
			var label_width = EditorGUIUtility.labelWidth - indent;
			var label_rect = new Rect(position.x + indent, position.y, label_width, height);
			EditorGUI.SelectableLabel(label_rect, Labels[field.name]);

			var rect = new Rect(position.x + indent + label_width, position.y, position.width - indent - label_width, EditorGUI.GetPropertyHeight(field));
			EditorGUI.PropertyField(rect, field, GUIContent.none);

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

			position.y += Height;

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
			var result = Height;

			if (!IsOpened)
			{
				return result;
			}

			var sprite = property.FindPropertyRelative("Sprite");
			var image_type = property.FindPropertyRelative("ImageType");

			result += GetPropertyHeight(sprite);
			result += GetPropertyHeight(property.FindPropertyRelative("Color"));
			result += GetPropertyHeight(property.FindPropertyRelative("Material"));

			if (sprite.objectReferenceValue is Sprite)
			{
				result += GetPropertyHeight(image_type);

				result += ImagePropertiesHeight((Image.Type)image_type.enumValueIndex, property);
			}

			return result;
		}

		/// <summary>
		/// Get GUI height for the specified property data.
		/// </summary>
		/// <param name="property">Property data.</param>
		/// <returns>GUI height.</returns>
		protected static float GetPropertyHeight(SerializedProperty property)
		{
			return EditorGUI.GetPropertyHeight(property, new GUIContent(Labels[property.name])) + EmptySpace;
		}
	}
}