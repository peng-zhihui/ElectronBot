#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// TMPro converter window.
	/// </summary>
	public class ConverterTMProWindow : EditorWindow
	{
		/// <summary>
		/// Show window.
		/// </summary>
		[MenuItem("Window/New UI Widgets/Replace Unity Text with TextMeshPro")]
		public static void Open()
		{
			var window = GetWindow<ConverterTMProWindow>("TextMeshPro Converter");
			window.minSize = new Vector2(520, 400);
			window.currentTarget = GameObject.Find("Canvas");
		}

		/// <summary>
		/// Show window.
		/// </summary>
		/// <param name="converter">Converter.</param>
		public static void Open(ConverterTMPro converter)
		{
			var window = GetWindow<ConverterTMProWindow>("TextMeshPro Converter");
			window.minSize = new Vector2(520, 400);
			window.currentTarget = converter.Target;
			window.converter = converter;
		}

		readonly GUIStyle styleLabel = new GUIStyle();

		GameObject previousTarget;

		GameObject currentTarget;

		Vector2 warningsScrollPosition;

		Vector2 errorsScrollPosition;

		ConverterTMPro converter;

		GUIStyle styleMessage;

		GUIStyle styleMessageSelected;

		void SetStyles()
		{
			styleMessage = new GUIStyle(GUI.skin.button);
			styleMessageSelected = new GUIStyle(GUI.skin.button);

			styleLabel.margin = new RectOffset(4, 4, 2, 2);
			styleLabel.richText = true;

			styleMessage.margin = new RectOffset(4, 4, 2, 2);
			styleMessage.padding = new RectOffset(3, 3, 3, 3);
			styleMessage.normal.textColor = Color.gray;
			styleMessage.alignment = TextAnchor.UpperLeft;
			styleMessage.richText = true;

			styleMessageSelected.margin = new RectOffset(4, 4, 2, 2);
			styleMessageSelected.padding = new RectOffset(3, 3, 3, 3);
			styleMessageSelected.normal.textColor = Color.black;
			styleMessageSelected.alignment = TextAnchor.UpperLeft;
			styleMessageSelected.richText = true;
		}

		void OnGUI()
		{
			SetStyles();

			GUILayout.Label("TextMeshPro Converter", EditorStyles.boldLabel);
			currentTarget = EditorGUILayout.ObjectField("Target", currentTarget, typeof(GameObject), true) as GameObject;

			if ((previousTarget != currentTarget) || (converter == null))
			{
				converter = new ConverterTMPro(currentTarget);
				previousTarget = currentTarget;
			}

			warningsScrollPosition = ShowMessages("Warnings", converter.Warnings, warningsScrollPosition);

			errorsScrollPosition = ShowMessages("Errors", converter.Errors, errorsScrollPosition);

			GUILayout.Label("<b>Convertible InputFields:</b> " + converter.InputFieldsCount.ToString(), styleLabel);
			GUILayout.Label("<b>Convertible Texts:</b> " + converter.TextsCount.ToString(), styleLabel);

			GUILayout.Space(15);

			if (converter.Errors.Count > 0)
			{
				GUILayout.Label("<b>The tagret cannot be converted. Please fix errors first.</b>", styleLabel);
			}
			else if (converter.TotalCount == 0)
			{
				GUILayout.Label("<b>Nothing to convert.</b>", styleLabel);
			}
			else
			{
				var button = (converter.Warnings.Count > 0) ? "Convert without components with warnings." : "Convert";
				if (GUILayout.Button(button))
				{
					converter = new ConverterTMPro(currentTarget);
					converter.Convert();
					Close();
				}
			}
		}

		Vector2 ShowMessages(string header, List<ConverterTMPro.Message> messages, Vector2 scroll, float height = 350f)
		{
			if (messages.Count == 0)
			{
				return scroll;
			}

			GUILayout.Label("<b>" + header + ":</b>", styleLabel);
			var options = new GUILayoutOption[] { GUILayout.Height(height), GUILayout.ExpandHeight(true) };
			scroll = EditorGUILayout.BeginScrollView(scroll, options);
			foreach (var message in messages)
			{
				var is_selected = (message.Target != null) && (Selection.activeObject == message.Target);

				if (GUILayout.Button(message.Info, is_selected ? styleMessageSelected : styleMessage))
				{
					Selection.activeObject = message.Target;
				}

				GUILayout.Space(3);
			}

			EditorGUILayout.EndScrollView();

			return scroll;
		}
	}
}
#endif