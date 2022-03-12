namespace UIWidgets
{
	using System;
	using UnityEditor;
	using UnityEditor.UI;

	/// <summary>
	/// Switch editor.
	/// </summary>
	[CustomEditor(typeof(Switch), true)]
	public class SwitchEditor : SelectableEditor
	{
		/// <summary>
		/// Excluded properties.
		/// </summary>
		protected string[] ExcludeProperties;

		/// <summary>
		/// Enable this instance.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			ExcludeProperties = new string[]
			{
				serializedObject.FindProperty("m_Script").propertyPath,
				serializedObject.FindProperty("m_Navigation").propertyPath,
				serializedObject.FindProperty("m_Transition").propertyPath,
				serializedObject.FindProperty("m_Colors").propertyPath,
				serializedObject.FindProperty("m_SpriteState").propertyPath,
				serializedObject.FindProperty("m_AnimationTriggers").propertyPath,
				serializedObject.FindProperty("m_Interactable").propertyPath,
				serializedObject.FindProperty("m_TargetGraphic").propertyPath,
			};
		}

		/// <summary>
		/// Validate targets.
		/// </summary>
		protected virtual void ValidateTargets()
		{
			foreach (var t in targets)
			{
				var v = t as IValidateable;
				if (v != null)
				{
					v.Validate();
				}
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			base.OnInspectorGUI();

			DrawPropertiesExcluding(serializedObject, ExcludeProperties);

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}