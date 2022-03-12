namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Style editor.
	/// </summary>
	[CustomEditor(typeof(Style), true)]
	public class StyleEditor : Editor
	{
		/// <summary>
		/// Serialized events.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Style fast properties.
		/// </summary>
		protected List<string> Fast = new List<string>()
		{
			"Fast",
		};

		/// <summary>
		/// Style detailed properties.
		/// </summary>
		protected List<string> Detailed = new List<string>();

		/// <summary>
		/// Add property.
		/// </summary>
		/// <param name="property">Property.</param>
		protected void AddProperty(SerializedProperty property)
		{
			if (Fast.Contains(property.name))
			{
				return;
			}

			Detailed.Add(property.name);
		}

		/// <summary>
		/// Add serialized property.
		/// </summary>
		/// <param name="name">Property name.</param>
		protected void AddSerializedPropery(string name)
		{
			var property = serializedObject.FindProperty(name);
			if (property != null)
			{
				SerializedProperties.Add(name, property);
			}
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Detailed.Clear();
			SerializedProperties.Clear();

			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}

			foreach (var f in Fast)
			{
				AddSerializedPropery(f);
			}

			foreach (var d in Detailed)
			{
				AddSerializedPropery(d);
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			var header_style = new GUIStyle
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold,
			};

			var menu = PrefabsMenu.Instance;
			var style = (Style)target;
			if (style == menu.DefaultStyle)
			{
				EditorGUILayout.LabelField("Default Style", header_style);
				if (GUILayout.Button("Undo default"))
				{
					menu.DefaultStyle = null;
					EditorUtility.SetDirty(menu);
				}
			}
			else
			{
				EditorGUILayout.LabelField("Not Default Style");
				if (GUILayout.Button("Set as default"))
				{
					menu.DefaultStyle = style;
					EditorUtility.SetDirty(menu);
				}
			}

			serializedObject.Update();

			EditorGUILayout.LabelField("Style: Fast Settings", header_style);

			for (int i = 0; i < Fast.Count; i++)
			{
				EditorGUILayout.PropertyField(SerializedProperties[Fast[i]], true);
			}

			if (GUILayout.Button("Apply Fast Settings"))
			{
				style.Fast.ChangeStyle(style);
			}

			if (GUILayout.Button("Apply Fast Font Settings"))
			{
				style.Fast.SetFonts(style);
			}

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField("Style: Detailed Settings", header_style);

			for (int i = 0; i < Detailed.Count; i++)
			{
				EditorGUILayout.PropertyField(SerializedProperties[Detailed[i]], true);
			}

			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}