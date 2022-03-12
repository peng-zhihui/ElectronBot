#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// TreeView.DataSource editor window.
	/// </summary>
	public class TreeViewDataSourceWindow : TreeListEditorWindow
	{
		/// <summary>
		/// Fields.
		/// </summary>
		class Fields : FieldsList
		{
			/// <summary>
			/// Is Expanded.
			/// </summary>
			public Field IsExpanded;

			/// <summary>
			/// Icon.
			/// </summary>
			public Field Icon;

			/// <summary>
			/// Name.
			/// </summary>
			public Field Name;

			float height;

			/// <summary>
			/// Initializes a new instance of the <see cref="Fields"/> class.
			/// </summary>
			/// <param name="item">Item.</param>
			public Fields(SerializedProperty item)
			{
				IsExpanded = new Field(item, "IsExpanded", "Exp.", "Is Expanded?");
				Icon = new Field(item, "Icon");
				Name = new Field(item, "Name");
			}

			/// <inheritdoc/>
			public override float UpdatePosition(float x, int depth, Rect position, TreeListEditorWindow menu)
			{
				var name_width = position.width
					- (depth * menu.DepthIndent)
					- (Button.Width + menu.Spacing)
					- (menu.IconWidth + menu.Spacing)
					- menu.ListButtons.Width(menu.Spacing);

				x = IsExpanded.UpdatePosition(x, Button.Width, position, menu.Spacing);
				x = Icon.UpdatePosition(x, menu.IconWidth, position, menu.Spacing);
				x = Name.UpdatePosition(x, name_width, position, menu.Spacing);

				height = menu.BaseHeight;

				return x;
			}

			/// <inheritdoc/>
			public override void Show()
			{
				var options = new GUILayoutOption[] { GUILayout.Width(Button.Width), GUILayout.Height(height) };
				if (GUILayout.Button(IsExpanded.Property.boolValue ? "-" : "+", options))
				{
					IsExpanded.Property.boolValue = !IsExpanded.Property.boolValue;
				}

				Icon.Show();
				Name.Show();
			}

			/// <inheritdoc/>
			public override void ShowLabels(GUIStyle style)
			{
				IsExpanded.ShowLabel(style);
				Icon.ShowLabel(style);
				Name.ShowLabel(style);
			}

			/// <inheritdoc/>
			public override float Width(float spacing)
			{
				// IsExpanded.Width + spacing
				return Icon.Width + spacing
					+ Name.Width + spacing;
			}
		}

		static GameObject currentGameObject;

		TreeViewDataSource Component;

		/// <summary>
		/// Init.
		/// </summary>
		public static void Init()
		{
			var window = EditorWindow.GetWindow<TreeViewDataSourceWindow>();
			window.Show();
		}

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			SetTitle("TreeView Nodes Editor");
		}

		/// <inheritdoc/>
		protected override bool InitGUI()
		{
			if (Selection.activeGameObject != null)
			{
				var component = Selection.activeGameObject.GetComponent<TreeViewDataSource>();
				if (component != null)
				{
					currentGameObject = Selection.activeGameObject;
				}
			}

			var data = currentGameObject != null ? currentGameObject.GetComponent<TreeViewDataSource>() : null;
			if (data == null)
			{
				return false;
			}

			if (Component == data)
			{
				return true;
			}

			AddLabel = "Add Node";
			InitError = "Please select TreeView in Hierarchy window.";

			Component = data;
			Target = new SerializedObject(data);
			List = Target.FindProperty("Data");

			ListButtons = new Buttons()
			{
				MoveLeft = new Button("←", "Move item with nested items on left", MoveLeft),
				MoveRight = new Button("→", "Move item with nested items on right", MoveRight),
				MoveUp = new Button("↑", "Move item with nested items up", MoveUp),
				MoveDown = new Button("↓", "Move item with nested items down", MoveDown),
				Add = new Button("+", "Add item after current", Add),
				Delete = new Button("-", "Delete current item", Delete),
			};

			return true;
		}

		/// <inheritdoc/>
		protected override void Header(Rect position)
		{
			var fields = new Fields(null);
			fields.UpdatePosition(position.x, 0, position, this);
			fields.ShowLabels(LabelStyle);
		}

		/// <inheritdoc/>
		protected override DisplayInfo DisplayItem(int index, Rect position, DisplayInfo info)
		{
			var item = List.GetArrayElementAtIndex(index);

			var result = info;
			var height = BaseHeight;

			EditorGUI.BeginProperty(position, new GUIContent(), item);

			var fields = new Fields(item);

			var depth = item.FindPropertyRelative("Depth");

			var x = position.x + (DepthIndent * depth.intValue);
			x = fields.UpdatePosition(x, depth.intValue, position, this);

			x = ListButtons.UpdatePosition(x, position, Spacing);
			ListButtons.UpdateEnabled(depth.intValue, index, info.Depth, List.arraySize);

			GUILayout.Space((DepthIndent * depth.intValue) + Padding);

			fields.Show();

			GUILayout.Space(fields.Width(Spacing));

			result.Depth = depth.intValue;
			result = ListButtons.Show(index, result);

			EditorGUI.EndProperty();

			result.Height = height;

			return result;
		}
	}
}
#endif