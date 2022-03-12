namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for close button.
	/// </summary>
	[Serializable]
	public class StyleButtonClose : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText Text;

		/// <summary>
		/// Apply style for the specified button.
		/// </summary>
		/// <param name="button">Button.</param>
		public virtual void ApplyTo(Button button)
		{
			if (button == null)
			{
				return;
			}

			Background.ApplyTo(button.transform);
			Text.ApplyTo(button.transform.Find("Text"));
		}

		/// <summary>
		/// Set style options from the specified button.
		/// </summary>
		/// <param name="button">Button.</param>
		public virtual void GetFrom(Button button)
		{
			if (button == null)
			{
				return;
			}

			Background.GetFrom(button.transform);
			Text.GetFrom(button.transform.Find("Text"));
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Text.SetDefaultValues();
		}
#endif
	}
}