#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// TabIcons drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(TabIcons))]
	public class TabIconsDrawer : PropertyDrawer
	{
		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			// position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Calculate rects
			var width = position.width / 4;
			var tabRect = new Rect(position.x + (width * 0), position.y, width, position.height);
			var nameRect = new Rect(position.x + (width * 1), position.y, width, position.height);
			var iconDefaultRect = new Rect(position.x + (width * 2), position.y, width, position.height);
			var iconActiveGORect = new Rect(position.x + (width * 3), position.y, width, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(tabRect, property.FindPropertyRelative("TabObject"), GUIContent.none);
			EditorGUI.LabelField(tabRect, new GUIContent(string.Empty, "Tab Object"));

			EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);
			EditorGUI.LabelField(nameRect, new GUIContent(string.Empty, "Name"));

			EditorGUI.PropertyField(iconDefaultRect, property.FindPropertyRelative("IconDefault"), GUIContent.none);
			EditorGUI.LabelField(iconDefaultRect, new GUIContent(string.Empty, "Default Icon"));

			EditorGUI.PropertyField(iconActiveGORect, property.FindPropertyRelative("IconActive"), GUIContent.none);
			EditorGUI.LabelField(iconActiveGORect, new GUIContent(string.Empty, "Active Icon"));

			EditorGUI.EndProperty();
		}
	}
}
#endif