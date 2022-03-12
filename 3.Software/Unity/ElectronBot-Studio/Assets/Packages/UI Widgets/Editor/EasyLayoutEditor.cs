#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// EasyLayout editor.
	/// </summary>
	[CustomEditor(typeof(EasyLayout), true)]
	public class EasyLayoutEditor : ConditionalEditor
	{
		void Upgrade()
		{
			foreach (var t in targets)
			{
				var l = t as EasyLayout;
				if (l != null)
				{
					l.Upgrade();
				}
			}
		}

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsCompactOrGrid = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Compact || (LayoutTypes)x.enumValueIndex == LayoutTypes.Grid },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsCompact = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Compact },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsCompactNotFlexible = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Compact },
			{ "compactConstraint", x => (CompactConstraints)x.enumValueIndex != CompactConstraints.Flexible },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsGrid = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Grid },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsGridNotFlexible = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Grid },
			{ "gridConstraint", x => (GridConstraints)x.enumValueIndex != GridConstraints.Flexible },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsFlex = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Flex },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsStaggered = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Staggered },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsEllipse = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex == LayoutTypes.Ellipse },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsNotEllipse = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "layoutType", x => (LayoutTypes)x.enumValueIndex != LayoutTypes.Ellipse },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsSymmetric = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "symmetric", x => x.boolValue },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> IsNotSymmetric = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "symmetric", x => !x.boolValue },
		};

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void Init()
		{
			Upgrade();

			Fields = new List<ConditionalFieldInfo>()
			{
				new ConditionalFieldInfo("mainAxis"),
				new ConditionalFieldInfo("layoutType"),

				new ConditionalFieldInfo("groupPosition", 1, IsCompactOrGrid),
				new ConditionalFieldInfo("rowAlign", 1, IsCompact),
				new ConditionalFieldInfo("innerAlign", 1, IsCompact),
				new ConditionalFieldInfo("compactConstraint", 1, IsCompact),
				new ConditionalFieldInfo("compactConstraintCount", 2, IsCompactNotFlexible),

				new ConditionalFieldInfo("cellAlign", 1, IsGrid),
				new ConditionalFieldInfo("gridConstraint", 1, IsGrid),
				new ConditionalFieldInfo("gridConstraintCount", 2, IsGridNotFlexible),

				new ConditionalFieldInfo("flexSettings", 1, IsFlex),
				new ConditionalFieldInfo("staggeredSettings", 1, IsStaggered),
				new ConditionalFieldInfo("ellipseSettings", 1, IsEllipse),

				new ConditionalFieldInfo("spacing", 0, IsNotEllipse),
				new ConditionalFieldInfo("symmetric"),

				new ConditionalFieldInfo("margin", 1, IsSymmetric),
				new ConditionalFieldInfo("marginTop", 1, IsNotSymmetric),
				new ConditionalFieldInfo("marginBottom", 1, IsNotSymmetric),
				new ConditionalFieldInfo("marginLeft", 1, IsNotSymmetric),
				new ConditionalFieldInfo("marginRight", 1, IsNotSymmetric),

				new ConditionalFieldInfo("topToBottom", 0, IsNotEllipse),

				new ConditionalFieldInfo("rightToLeft"),
				new ConditionalFieldInfo("skipInactive"),
				new ConditionalFieldInfo("resetRotation", 0, IsNotEllipse),
				new ConditionalFieldInfo("childrenWidth"),
				new ConditionalFieldInfo("childrenHeight"),
			};

			IgnoreFields = new List<string>()
			{
				"m_Padding",
				"m_ChildAlignment",
			};
		}

		/// <summary>
		/// Display additional GUI.
		/// </summary>
		protected override void AdditionalGUI()
		{
			if (targets.Length == 1)
			{
				var script = (EasyLayout)target;

				EditorGUILayout.LabelField("Block size", string.Format("{0}x{1}", script.BlockSize.x.ToString(), script.BlockSize.y.ToString()));
				EditorGUILayout.LabelField("UI size", string.Format("{0}x{1}", script.UISize.x.ToString(), script.UISize.y.ToString()));
			}
		}
	}
}
#endif