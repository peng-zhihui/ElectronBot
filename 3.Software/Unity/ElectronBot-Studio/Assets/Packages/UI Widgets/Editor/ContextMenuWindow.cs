#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;

	/// <summary>
	/// TreeView.DataSource editor window.
	/// </summary>
	public class ContextMenuWindow : TreeListEditorWindow
	{
		/// <summary>
		/// Fields.
		/// </summary>
		class Fields : FieldsList
		{
			/// <summary>
			/// Icon.
			/// </summary>
			public Field Icon;

			/// <summary>
			/// Checked.
			/// </summary>
			public Field Checked;

			/// <summary>
			/// Name.
			/// </summary>
			public Field Name;

			/// <summary>
			/// Template.
			/// </summary>
			public Field Template;

			/// <summary>
			/// Visible.
			/// </summary>
			public Field Visible;

			/// <summary>
			/// Interactable.
			/// </summary>
			public Field Interactable;

			/// <summary>
			/// Ctrl.
			/// </summary>
			public Field Ctrl;

			/// <summary>
			/// Alt.
			/// </summary>
			public Field Alt;

			/// <summary>
			/// Shift.
			/// </summary>
			public Field Shift;

			/// <summary>
			/// HotKey.
			/// </summary>
			public Field HotKey;

			/// <summary>
			/// Initializes a new instance of the <see cref="Fields"/> class.
			/// </summary>
			/// <param name="item">Item.</param>
			public Fields(SerializedProperty item)
			{
				Icon = new Field(item, "Icon");
				Checked = new Field(item, "Checked", "Ch.");
				Name = new Field(item, "Name");
				Template = new Field(item, "Template");

				Visible = new Field(item, "Visible", "Vis.");
				Interactable = new Field(item, "Interactable", "Int.");

				Ctrl = new Field(item, "HotKey.ctrl", "Ctrl", "HotKey Ctrl");
				Alt = new Field(item, "HotKey.alt", "Alt", "HotKey Alt");
				Shift = new Field(item, "HotKey.shift", "Shift", "HotKey Shift");
				HotKey = new Field(item, "HotKey.key", "HotKey", "HotKey");
			}

			/// <inheritdoc/>
			public override float UpdatePosition(float x, int depth, Rect position, TreeListEditorWindow menu)
			{
				var name_width = position.width
					- (depth * menu.DepthIndent)
					- (menu.IconWidth + menu.Spacing)
					- (menu.TemplateWidth + menu.Spacing)
					- ((menu.CheckboxWidth + menu.Spacing) * 6)
					- (menu.HotKeyWidth + menu.Spacing)
					- (menu.ActionWidth + menu.Spacing)
					- menu.ListButtons.Width(menu.Spacing);

				x = Icon.UpdatePosition(x, menu.IconWidth, position, menu.Spacing);
				x = Checked.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);
				x = Name.UpdatePosition(x, name_width, position, menu.Spacing);

				x = Template.UpdatePosition(x, menu.TemplateWidth, position, menu.Spacing);
				x = Visible.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);
				x = Interactable.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);

				x = Ctrl.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);
				x = Alt.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);
				x = Shift.UpdatePosition(x, menu.CheckboxWidth, position, menu.Spacing);
				x = HotKey.UpdatePosition(x, menu.HotKeyWidth, position, menu.Spacing);

				return x;
			}

			/// <inheritdoc/>
			public override void Show()
			{
				Icon.Show();
				Checked.Show();
				Name.Show();

				Template.Show();

				Visible.Show();
				Interactable.Show();

				Ctrl.Show();
				Alt.Show();
				Shift.Show();
				HotKey.Show();
			}

			/// <inheritdoc/>
			public override void ShowLabels(GUIStyle style)
			{
				Icon.ShowLabel(style);
				Checked.ShowLabel(style);
				Name.ShowLabel(style);

				Template.ShowLabel(style);

				Visible.ShowLabel(style);
				Interactable.ShowLabel(style);

				Ctrl.ShowLabel(style);
				Alt.ShowLabel(style);
				Shift.ShowLabel(style);
				HotKey.ShowLabel(style);
			}

			/// <inheritdoc/>
			public override float Width(float spacing)
			{
				return Icon.Width + spacing
					+ Checked.Width + spacing
					+ Name.Width + spacing

					+ Template.Width + spacing

					+ Visible.Width + spacing
					+ Interactable.Width + spacing

					+ Ctrl.Width + spacing
					+ Alt.Width + spacing
					+ Shift.Width + spacing
					+ HotKey.Width + spacing;
			}
		}

		/// <summary>
		/// Init.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Init(SerializedObject target)
		{
			var window = EditorWindow.GetWindow<ContextMenuWindow>();
			window.minSize = new Vector2(1200, 500);
			window.Target = target;
			window.Show();
		}

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			SetTitle("ContextMenu Items Editor");
		}

		/// <inheritdoc/>
		protected override bool InitGUI()
		{
			if ((Target == null) || !(Target.targetObject is UIWidgets.Menu.ContextMenu))
			{
				return false;
			}

			if (List == null)
			{
				AddLabel = "Add Menu Item";
				InitError = "ContextMenu is not selected.";

				List = Target.FindProperty("MenuItemsSerialized");

				ListButtons = new Buttons()
				{
					MoveLeft = new Button("←", "Move node with nested nodes on left", MoveLeft),
					MoveRight = new Button("→", "Move node with nested nodes on right", MoveRight),
					MoveUp = new Button("↑", "Move node with nested nodes up", MoveUp),
					MoveDown = new Button("↓", "Move node with nested nodes down", MoveDown),
					Add = new Button("+", "Add node after current", Add),
					Delete = new Button("-", "Delete current node", Delete),
				};
			}

			return true;
		}

		/// <inheritdoc/>
		protected override void Header(Rect position)
		{
			var fields = new Fields(null);

			var start = fields.UpdatePosition(position.x, 0, position, this);

			var action_position = new Rect(start, position.y, ActionWidth, position.height);
			start = action_position.xMax + Spacing;

			fields.ShowLabels(LabelStyle);

			EditorGUI.LabelField(action_position, new GUIContent("OnClick", "OnClick"), LabelStyle);
		}

		/// <inheritdoc/>
		protected override DisplayInfo DisplayItem(int index, Rect position, DisplayInfo info)
		{
			var item = List.GetArrayElementAtIndex(index);

			var result = info;
			var height = BaseHeight;

			EditorGUI.BeginProperty(position, new GUIContent(), item);

			var fields = new Fields(item);

			var show_action = item.FindPropertyRelative("showAction");
			var action = item.FindPropertyRelative("Action");

			var depth = item.FindPropertyRelative("Depth");

			var x = position.x + (DepthIndent * depth.intValue);
			x = fields.UpdatePosition(x, depth.intValue, position, this);

			var action_position = new Rect(x, position.y, ActionWidth, position.height);
			x = action_position.xMax;

			x = ListButtons.UpdatePosition(x, position, Spacing);
			ListButtons.UpdateEnabled(depth.intValue, index, result.Depth, List.arraySize);

			GUILayout.Space((DepthIndent * depth.intValue) + Spacing);

			fields.Show();

			GUILayout.Space(fields.Width(Spacing));

			var options = new GUILayoutOption[] { GUILayout.Width(ActionWidth), GUILayout.Height(info.Height) };
			if (GUILayout.Button(show_action.boolValue ? "Hide Action" : "Show Action", options))
			{
				show_action.boolValue = !show_action.boolValue;
			}
			else if (show_action.boolValue)
			{
				var drawer = new UnityEventDrawer();
				action_position.y += action_position.height;
				action_position.height = drawer.GetPropertyHeight(action, GUIContent.none);
				height += action_position.height;

				EditorGUI.PropertyField(action_position, action, GUIContent.none);
			}

			result.Depth = depth.intValue;
			result = ListButtons.Show(index, result);

			EditorGUI.EndProperty();

			result.Height = height;

			return result;
		}
	}
}
#endif