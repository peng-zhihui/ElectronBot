#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UnityEditor;

	/// <summary>
	/// Base editor for the UIWidgets components.
	/// </summary>
	[CustomEditor(typeof(UIWidgetsMonoBehaviour), true)]
	public class UIWidgetsMonoEditor : Editor
	{
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

			ValidateTargets();
		}
	}
}
#endif