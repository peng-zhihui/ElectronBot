namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the canvas.
	/// </summary>
	[Serializable]
	public class StyleCanvas : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the default background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
		}
#endif

		/// <summary>
		/// Apply style to the specified component.
		/// </summary>
		/// <param name="component">Canvas.</param>
		public virtual void ApplyTo(Canvas component)
		{
			if (component == null)
			{
				return;
			}

			Background.ApplyTo(component);
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="component">Canvas.</param>
		public virtual void GetFrom(Canvas component)
		{
			if (component == null)
			{
				return;
			}

			Background.GetFrom(component);
		}
	}
}