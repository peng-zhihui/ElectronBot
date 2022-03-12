namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the button.
	/// </summary>
	[Serializable]
	public class StyleButton : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the mask.
		/// </summary>
		[SerializeField]
		public StyleImage Mask;

		/// <summary>
		/// Style for the border.
		/// </summary>
		[SerializeField]
		public StyleImage Border;

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText Text;

		/// <summary>
		/// Style for the image.
		/// </summary>
		[SerializeField]
		public StyleImage Image;

		/// <summary>
		/// Apply style to the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		public virtual void ApplyTo(GameObject go)
		{
			if (go != null)
			{
				var support = go.GetComponent<StyleSupportButton>();

				if (support != null)
				{
					ApplyTo(support);
				}
				else
				{
					var button = go.GetComponent<Button>();

					if (button != null)
					{
						ApplyTo(button);
					}
				}
			}
		}

		/// <summary>
		/// Apply style to the specified transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public virtual void ApplyTo(Transform transform)
		{
			if (transform != null)
			{
				ApplyTo(transform.gameObject);
			}
		}

		/// <summary>
		/// Apply style for the specified button.
		/// </summary>
		/// <param name="button">Button.</param>
		public virtual void ApplyTo(StyleSupportButton button)
		{
			if (button == null)
			{
				return;
			}

			Background.ApplyTo(button.Background);
			Mask.ApplyTo(button.Mask);
			Border.ApplyTo(button.Border);
			Text.ApplyTo(button.Text);
			Image.ApplyTo(button.Image);
		}

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

			if (button.transform.Find("Mask") == null)
			{
				return;
			}

			Background.ApplyTo(button.transform.Find("Mask/Texture"));
			Mask.ApplyTo(button.transform.Find("Mask"));
			Border.ApplyTo(button.transform.Find("Border"));
			Text.ApplyTo(button.transform.Find("Text"));
			Image.ApplyTo(button.transform.Find("Image"));
		}

		/// <summary>
		/// Set style options from the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		public virtual void GetFrom(GameObject go)
		{
			if (go != null)
			{
				var support = go.GetComponent<StyleSupportButton>();

				if (support != null)
				{
					GetFrom(support);
				}
				else
				{
					var button = go.GetComponent<Button>();

					if (button != null)
					{
						GetFrom(button);
					}
				}
			}
		}

		/// <summary>
		/// Set style options from the specified transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public virtual void GetFrom(Transform transform)
		{
			if (transform != null)
			{
				GetFrom(transform.gameObject);
			}
		}

		/// <summary>
		/// Set style options from the specified button.
		/// </summary>
		/// <param name="button">Button.</param>
		public virtual void GetFrom(StyleSupportButton button)
		{
			if (button == null)
			{
				return;
			}

			Background.GetFrom(button.Background);
			Mask.GetFrom(button.Mask);
			Border.GetFrom(button.Border);
			Text.GetFrom(button.Text);
			Image.GetFrom(button.Image);
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

			if (button.transform.Find("Mask") == null)
			{
				return;
			}

			Background.GetFrom(button.transform.Find("Mask/Texture"));
			Mask.GetFrom(button.transform.Find("Mask"));
			Border.GetFrom(button.transform.Find("Border"));
			Text.GetFrom(button.transform.Find("Text"));
			Image.GetFrom(button.transform.Find("Image"));
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Mask.SetDefaultValues();
			Border.SetDefaultValues();
			Text.SetDefaultValues();
			Image.SetDefaultValues();
		}
#endif
	}
}