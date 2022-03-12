#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base TreeList editor window.
	/// </summary>
	public abstract class TreeListEditorWindow : EditorWindow
	{
		/// <summary>
		/// Display info.
		/// </summary>
		public struct DisplayInfo : IEquatable<DisplayInfo>
		{
			/// <summary>
			/// Depth.
			/// </summary>
			public int Depth;

			/// <summary>
			/// Height.
			/// </summary>
			public float Height;

			/// <summary>
			/// Count.
			/// </summary>
			public int Count;

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is DisplayInfo)
				{
					return Equals((DisplayInfo)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(DisplayInfo other)
			{
				return (Depth == other.Depth) && (Height == other.Height) && (Count == other.Count);
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Depth ^ Height.GetHashCode() ^ Count;
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(DisplayInfo a, DisplayInfo b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(DisplayInfo a, DisplayInfo b)
			{
				return !a.Equals(b);
			}
		}

		/// <summary>
		/// Buttons.
		/// </summary>
		public class Buttons
		{
			/// <summary>
			/// Count.
			/// </summary>
			public static readonly int Count = 6;

			/// <summary>
			/// Move left button.
			/// </summary>
			public Button MoveLeft;

			/// <summary>
			/// Move right button.
			/// </summary>
			public Button MoveRight;

			/// <summary>
			/// Move up button.
			/// </summary>
			public Button MoveUp;

			/// <summary>
			/// Move down button.
			/// </summary>
			public Button MoveDown;

			/// <summary>
			/// Add button.
			/// </summary>
			public Button Add;

			/// <summary>
			/// Delete button.
			/// </summary>
			public Button Delete;

			ContextMenuWindow menu;

			/// <summary>
			/// Get width.
			/// </summary>
			/// <param name="spacing">Spacing.</param>
			/// <returns>Width.</returns>
			public float Width(float spacing)
			{
				return Count * (Button.Width + spacing);
			}

			/// <summary>
			/// Update position.
			/// </summary>
			/// <param name="x">Start X value.</param>
			/// <param name="position">Position.</param>
			/// <param name="spacing">Spacing.</param>
			/// <returns>Max X value.</returns>
			public float UpdatePosition(float x, Rect position, float spacing)
			{
				x = MoveLeft.SetPosition(x, position) + spacing;
				x = MoveRight.SetPosition(x, position) + spacing;
				x = MoveUp.SetPosition(x, position) + spacing;
				x = MoveDown.SetPosition(x, position) + spacing;
				x = Add.SetPosition(x, position) + spacing;
				x = Delete.SetPosition(x, position) + spacing;

				return x;
			}

			/// <summary>
			/// Update Enabled property.
			/// </summary>
			/// <param name="depth">Depth.</param>
			/// <param name="index">Item index.</param>
			/// <param name="previousDepth">Depth of the previous item.</param>
			/// <param name="count">Items count.</param>
			public void UpdateEnabled(int depth, int index, int previousDepth, int count)
			{
				MoveLeft.Enabled = depth > 0;
				MoveRight.Enabled = depth <= previousDepth;
				MoveUp.Enabled = index > 0;
				MoveDown.Enabled = (index + 1) < count;
			}

			/// <summary>
			/// Show.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="info">Info.</param>
			/// <returns>Display info.</returns>
			public DisplayInfo Show(int index, DisplayInfo info)
			{
				info = MoveLeft.Show(index, info);
				info = MoveRight.Show(index, info);
				info = MoveUp.Show(index, info);
				info = MoveDown.Show(index, info);
				info = Add.Show(index, info);
				info = Delete.Show(index, info);

				return info;
			}
		}

		/// <summary>
		/// Button.
		/// </summary>
		public class Button
		{
			/// <summary>
			/// Width.
			/// </summary>
			public static readonly float Width = 20;

			/// <summary>
			/// Name.
			/// </summary>
			public string Name
			{
				get;
				protected set;
			}

			/// <summary>
			/// Label.
			/// </summary>
			public string Label
			{
				get;
				protected set;
			}

			/// <summary>
			/// Position.
			/// </summary>
			public Rect Position
			{
				get;
				private set;
			}

			/// <summary>
			/// Enabled.
			/// </summary>
			public bool Enabled = true;

			/// <summary>
			/// On click action.
			/// </summary>
			public Func<int, DisplayInfo, DisplayInfo> OnClick
			{
				get;
				protected set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Button"/> class.
			/// </summary>
			/// <param name="name">Name.</param>
			/// <param name="label">Label.</param>
			/// <param name="onClick">On click action.</param>
			public Button(string name, string label, Func<int, DisplayInfo, DisplayInfo> onClick)
			{
				Name = name;
				Label = label;
				OnClick = onClick;
			}

			/// <summary>
			/// Set position.
			/// </summary>
			/// <param name="x">Start X value.</param>
			/// <param name="position">Position.</param>
			/// <returns>Max X value/</returns>
			public float SetPosition(float x, Rect position)
			{
				Position = new Rect(x, position.y, Width, position.height);

				return Position.xMax;
			}

			/// <summary>
			/// Show button.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="info">Info.</param>
			/// <returns>Display info.</returns>
			public DisplayInfo Show(int index, DisplayInfo info)
			{
				GUI.enabled = Enabled;
				var options = new GUILayoutOption[] { GUILayout.Width(Width), GUILayout.Height(info.Height) };
				if (GUILayout.Button(Name, options))
				{
					info = OnClick(index, info);
				}

				GUI.enabled = true;
				EditorGUI.LabelField(Position, new GUIContent(string.Empty, Label));

				return info;
			}
		}

		/// <summary>
		/// Field.
		/// </summary>
		protected class Field
		{
			/// <summary>
			/// Property.
			/// </summary>
			public SerializedProperty Property;

			/// <summary>
			/// Name.
			/// </summary>
			public string Name;

			/// <summary>
			/// Header.
			/// </summary>
			public string Header;

			/// <summary>
			/// Label.
			/// </summary>
			public string Label;

			/// <summary>
			/// Position.
			/// </summary>
			public Rect Position;

			/// <summary>
			/// Width.
			/// </summary>
			public float Width
			{
				get
				{
					return Position.width;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Field"/> class.
			/// </summary>
			/// <param name="item">Item.</param>
			/// <param name="name">Name.</param>
			/// <param name="header">Header.</param>
			/// <param name="label">Label.</param>
			public Field(SerializedProperty item, string name, string header = null, string label = null)
			{
				Property = item != null ? item.FindPropertyRelative(name) : null;
				Name = name;
				Header = header ?? name;
				Label = label ?? name;
			}

			/// <summary>
			/// Update position.
			/// </summary>
			/// <param name="x">Start X value.</param>
			/// <param name="width">Width.</param>
			/// <param name="position">Position.</param>
			/// <param name="spacing">Spacing.</param>
			/// <returns>Max X value.</returns>
			public float UpdatePosition(float x, float width, Rect position, float spacing)
			{
				Position = new Rect(x, position.y, width, position.height);

				return Position.xMax + spacing;
			}

			/// <summary>
			/// Show.
			/// </summary>
			public void Show()
			{
				EditorGUI.PropertyField(Position, Property, GUIContent.none);
				EditorGUI.LabelField(Position, new GUIContent(string.Empty, Label));
			}

			/// <summary>
			/// Show label.
			/// </summary>
			/// <param name="style">Style.</param>
			public void ShowLabel(GUIStyle style)
			{
				EditorGUI.LabelField(Position, new GUIContent(Header, Label), style);
			}
		}

		/// <summary>
		/// Fields list.
		/// </summary>
		protected abstract class FieldsList
		{
			/// <summary>
			/// Update position.
			/// </summary>
			/// <param name="x">Start X value.</param>
			/// <param name="depth">Depth.</param>
			/// <param name="position">Position.</param>
			/// <param name="menu">Menu.</param>
			/// <returns>Max X value.</returns>
			public abstract float UpdatePosition(float x, int depth, Rect position, TreeListEditorWindow menu);

			/// <summary>
			/// Show.
			/// </summary>
			public abstract void Show();

			/// <summary>
			/// Show labels.
			/// </summary>
			/// <param name="style">Style.</param>
			public abstract void ShowLabels(GUIStyle style);

			/// <summary>
			/// Get width.
			/// </summary>
			/// <param name="spacing">Spacing.</param>
			/// <returns>Width.</returns>
			public abstract float Width(float spacing);
		}

		/// <summary>
		/// Scrollbar width.
		/// </summary>
		public float ScrollbarWidth = 20;

		/// <summary>
		/// Base height.
		/// </summary>
		public float BaseHeight = 18;

		/// <summary>
		/// Padding.
		/// </summary>
		public float Padding = 4;

		/// <summary>
		/// Spacing.
		/// </summary>
		public float Spacing = 4;

		/// <summary>
		/// Indent per depth level.
		/// </summary>
		public float DepthIndent = 20;

		/// <summary>
		/// Icon width.
		/// </summary>
		public float IconWidth = 140;

		/// <summary>
		/// Template width.
		/// </summary>
		public float TemplateWidth = 80;

		/// <summary>
		/// Checkbox width.
		/// </summary>
		public float CheckboxWidth = 30;

		/// <summary>
		/// Action width.
		/// </summary>
		public float ActionWidth = 250;

		/// <summary>
		/// HotKey width.
		/// </summary>
		public float HotKeyWidth = 100;

		/// <summary>
		/// Add label.
		/// </summary>
		protected string AddLabel = "Add";

		/// <summary>
		/// Init error.
		/// </summary>
		protected string InitError = string.Empty;

		/// <summary>
		/// Label style.
		/// </summary>
		protected GUIStyle LabelStyle;

		/// <summary>
		/// Scroll position.
		/// </summary>
		protected Vector2 ScrollPosition;

		/// <summary>
		/// Target.
		/// </summary>
		protected SerializedObject Target;

		/// <summary>
		/// List.
		/// </summary>
		protected SerializedProperty List;

		/// <summary>
		/// Buttons.
		/// </summary>
		public Buttons ListButtons;

		/// <summary>
		/// AddButton layout options.
		/// </summary>
		protected static readonly GUILayoutOption[] AddButtonOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

		/// <summary>
		/// Set title.
		/// </summary>
		protected abstract void OnEnable();

		/// <summary>
		/// Set title.
		/// </summary>
		/// <param name="newTitle">Title.</param>
		protected void SetTitle(string newTitle)
		{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			titleContent = new GUIContent(newTitle);
#else
			title = name;
#endif
		}

		/// <summary>
		/// Init GUI.
		/// </summary>
		/// <returns>true if GUI successfully inited; otherwise false.</returns>
		protected abstract bool InitGUI();

		/// <summary>
		/// Draw GUI.
		/// </summary>
		protected virtual void OnGUI()
		{
			if (LabelStyle == null)
			{
				LabelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
			}

			if (!InitGUI())
			{
				GUILayout.Label(InitError, EditorStyles.boldLabel);
				return;
			}

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

			var info = new DisplayInfo()
			{
				Depth = -1,
				Count = List.arraySize,
				Height = BaseHeight,
			};

			var item_position = new Rect(position)
			{
				x = Padding,
				y = Padding,
				height = BaseHeight,
			};
			item_position.width -= ScrollbarWidth;

			Header(item_position);
			item_position.y += item_position.height + 3;
			EditorGUILayout.BeginVertical();
			GUILayout.Space(info.Height + 5);
			EditorGUILayout.EndVertical();

			for (int index = 0; index < info.Count; index++)
			{
				if (index == info.Count)
				{
					continue;
				}

				EditorGUILayout.BeginHorizontal();

				info.Height = BaseHeight;
				info = DisplayItem(index, item_position, info);

				EditorGUILayout.EndHorizontal();

				item_position.y += info.Height + 4;
#if UNITY_2020_1_OR_NEWER
#else
				item_position.y += 2;
#endif

				EditorGUILayout.BeginVertical();
				GUILayout.Space(info.Height - BaseHeight);
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndScrollView();

			FixTreeList(info.Count);

			if (GUILayout.Button(AddLabel, AddButtonOptions))
			{
				List.InsertArrayElementAtIndex(List.arraySize);
			}

			Target.ApplyModifiedProperties();
		}

		/// <summary>
		/// Draw header.
		/// </summary>
		/// <param name="position">Position.</param>
		protected abstract void Header(Rect position);

		/// <summary>
		/// Display item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <param name="position">Item position.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected abstract DisplayInfo DisplayItem(int index, Rect position, DisplayInfo info);

		/// <summary>
		/// Fix items.
		/// </summary>
		/// <param name="count">Count.</param>
		protected virtual void FixTreeList(int count)
		{
			int depth = 0;
			for (int i = 0; i < count; i++)
			{
				var sDepth = List.GetArrayElementAtIndex(i).FindPropertyRelative("Depth");
				if (sDepth.intValue < 0)
				{
					sDepth.intValue = 0;
				}

				if ((sDepth.intValue - depth) > 1)
				{
					sDepth.intValue = depth + 1;
				}

				depth = sDepth.intValue;
			}
		}

		/// <summary>
		/// Add item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo Add(int index, DisplayInfo info)
		{
			List.InsertArrayElementAtIndex(index + 1);

			info.Count += 1;

			return info;
		}

		/// <summary>
		/// Delete item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo Delete(int index, DisplayInfo info)
		{
			int depth = List.GetArrayElementAtIndex(index).FindPropertyRelative("Depth").intValue;
			for (int j = index + 1; j < info.Count; j++)
			{
				var child = List.GetArrayElementAtIndex(j);
				if (child.FindPropertyRelative("Depth").intValue <= depth)
				{
					break;
				}

				child.FindPropertyRelative("Depth").intValue -= 1;
			}

			List.DeleteArrayElementAtIndex(index);

			if (index > 0)
			{
				info.Depth = List.GetArrayElementAtIndex(index - 1).FindPropertyRelative("Depth").intValue;
			}
			else
			{
				info.Depth = -1;
			}

			info.Count -= 1;

			return info;
		}

		/// <summary>
		/// Move item to left.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo MoveLeft(int index, DisplayInfo info)
		{
			return ChangeDepth(index, info, -1);
		}

		/// <summary>
		/// Move item to right.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo MoveRight(int index, DisplayInfo info)
		{
			return ChangeDepth(index, info, 1);
		}

		/// <summary>
		/// Change item depth.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <param name="deltaDepth">Depth delta.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo ChangeDepth(int index, DisplayInfo info, int deltaDepth)
		{
			if (deltaDepth == 0)
			{
				return info;
			}

			var sDepth = List.GetArrayElementAtIndex(index).FindPropertyRelative("Depth");
			var depth = sDepth.intValue;
			for (int j = index + 1; j < info.Count; j++)
			{
				var child = List.GetArrayElementAtIndex(j);
				if (child.FindPropertyRelative("Depth").intValue <= depth)
				{
					break;
				}

				child.FindPropertyRelative("Depth").intValue += deltaDepth;
			}

			sDepth.intValue += deltaDepth;

			return info;
		}

		/// <summary>
		/// Move item up.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo MoveUp(int index, DisplayInfo info)
		{
			var sDepth = List.GetArrayElementAtIndex(index).FindPropertyRelative("Depth");
			var depth = sDepth.intValue;
			var new_depth = (index == 1) ? 0 : List.GetArrayElementAtIndex(index - 2).FindPropertyRelative("Depth").intValue;

			List.MoveArrayElement(index, index - 1);

			for (int j = index + 1; j < info.Count; j++)
			{
				var child = List.GetArrayElementAtIndex(j);
				if (child.FindPropertyRelative("Depth").intValue <= depth)
				{
					break;
				}

				List.MoveArrayElement(j, j - 1);
			}

			return ChangeDepth(index - 1, info, new_depth - depth);
		}

		/// <summary>
		/// Move item down.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="info">Info.</param>
		/// <returns>Display info.</returns>
		protected virtual DisplayInfo MoveDown(int index, DisplayInfo info)
		{
			var sDepth = List.GetArrayElementAtIndex(index).FindPropertyRelative("Depth");
			var depth = sDepth.intValue;

			int n = index;
			for (int j = index + 1; j < info.Count; j++)
			{
				var child = List.GetArrayElementAtIndex(j);
				if (child.FindPropertyRelative("Depth").intValue <= depth)
				{
					break;
				}

				n += 1;
			}

			for (int j = n; j >= index; j--)
			{
				List.MoveArrayElement(j, j + 1);
			}

			var new_depth = ((n + 1) == info.Count) ? 0 : List.GetArrayElementAtIndex(index).FindPropertyRelative("Depth").intValue;
			return ChangeDepth(n, info, new_depth - depth);
		}
	}
}
#endif