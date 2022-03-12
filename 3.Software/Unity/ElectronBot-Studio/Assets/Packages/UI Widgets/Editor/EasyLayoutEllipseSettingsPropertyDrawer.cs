#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Property drawer for the EasyLayoutEllipseSettings.
	/// </summary>
	[CustomPropertyDrawer(typeof(EasyLayoutEllipseSettings))]
	public class EasyLayoutEllipseSettingsPropertyDrawer : ConditionalPropertyDrawer
	{
		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void Init()
		{
			if (Fields != null)
			{
				return;
			}

			var is_not_width_auto = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "widthAuto", x => !x.boolValue },
			};
			var is_not_height_auto = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "heightAuto", x => !x.boolValue },
			};
			var is_angle_auto = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "angleStepAuto", x => x.boolValue },
			};
			var is_not_angle_auto = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "angleStepAuto", x => !x.boolValue },
			};
			var is_arc = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "angleStepAuto", x => x.boolValue },
				{ "fill", x => (EllipseFill)x.enumValueIndex == EllipseFill.Arc },
			};
			var is_rotate = new Dictionary<string, Func<SerializedProperty, bool>>()
			{
				{ "elementsRotate", x => x.boolValue },
			};

			Fields = new List<ConditionalFieldInfo>()
			{
				new ConditionalFieldInfo("widthAuto"),
				new ConditionalFieldInfo("width", 1, is_not_width_auto),
				new ConditionalFieldInfo("heightAuto"),
				new ConditionalFieldInfo("height", 1, is_not_height_auto),
				new ConditionalFieldInfo("angleStart"),
				new ConditionalFieldInfo("angleStepAuto"),
				new ConditionalFieldInfo("angleStep", 1, is_not_angle_auto),
				new ConditionalFieldInfo("fill", 1, is_angle_auto),
				new ConditionalFieldInfo("arcLength", 2, is_arc),
				new ConditionalFieldInfo("align"),
				new ConditionalFieldInfo("elementsRotate"),
				new ConditionalFieldInfo("elementsRotationStart", 1, is_rotate),
			};
		}
	}
}
#endif