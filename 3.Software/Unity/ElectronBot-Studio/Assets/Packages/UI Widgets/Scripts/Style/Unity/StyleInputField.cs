namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the InputField.
	/// </summary>
	[Serializable]
	public class StyleInputField : IStyleDefaultValues
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
		/// Style for the placeholder.
		/// </summary>
		[SerializeField]
		public StyleText Placeholder;

		/// <summary>
		/// Apply style to the specified InputField.
		/// </summary>
		/// <param name="component">InputField.</param>
		public virtual void ApplyTo(InputField component)
		{
			if (component == null)
			{
				return;
			}

			Background.ApplyTo(component);
			Text.ApplyTo(component.textComponent);

			if (component.placeholder != null)
			{
				Placeholder.ApplyTo(component.placeholder.gameObject);
			}
		}

		/// <summary>
		/// Apply style to the specified InputField.
		/// </summary>
		/// <param name="component">InputField.</param>
		public virtual void ApplyTo(InputFieldAdapter component)
		{
			if (component == null)
			{
				return;
			}

			Background.ApplyTo(component);

			if (component.textComponent != null)
			{
				Text.ApplyTo(component.textComponent.gameObject);
			}

			if (component.placeholder != null)
			{
				Placeholder.ApplyTo(component.placeholder.gameObject);
			}
		}

#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
		/// <summary>
		/// Apply style to the specified InputField.
		/// </summary>
		/// <param name="component">Slider.</param>
		public virtual void ApplyTo(TMPro.TMP_InputField component)
		{
			if (component == null)
			{
				return;
			}

			Background.ApplyTo(component);
			Text.ApplyTo(component.textComponent.gameObject);

			if (component.placeholder != null)
			{
				Placeholder.ApplyTo(component.placeholder.gameObject);
			}
		}
#endif

		/// <summary>
		/// Set style options from the specified InputField.
		/// </summary>
		/// <param name="component">InputField.</param>
		public virtual void GetFrom(InputField component)
		{
			if (component == null)
			{
				return;
			}

			Background.GetFrom(component);
			Text.GetFrom(component.textComponent);

			if (component.placeholder != null)
			{
				Placeholder.GetFrom(component.placeholder.gameObject);
			}
		}

		/// <summary>
		/// Set style options from the specified InputFieldAdapter.
		/// </summary>
		/// <param name="component">InputFieldAdapter.</param>
		public virtual void GetFrom(InputFieldAdapter component)
		{
			if (component == null)
			{
				return;
			}

			Background.GetFrom(component);

			if (component.textComponent != null)
			{
				Text.GetFrom(component.textComponent.gameObject);
			}

			if (component.placeholder != null)
			{
				Placeholder.GetFrom(component.placeholder.gameObject);
			}
		}

#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
		/// <summary>
		/// Set style options from the specified TMP_InputField.
		/// </summary>
		/// <param name="component">TMP_InputField.</param>
		public virtual void GetFrom(TMPro.TMP_InputField component)
		{
			if (component == null)
			{
				return;
			}

			Background.GetFrom(component);
			Text.GetFrom(component.textComponent.gameObject);

			if (component.placeholder != null)
			{
				Placeholder.GetFrom(component.placeholder.gameObject);
			}
		}
#endif

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Text.SetDefaultValues();
			Placeholder.SetDefaultValues();
		}
#endif
	}
}