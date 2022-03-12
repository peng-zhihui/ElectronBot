namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the slider.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class StyleSupportSlider : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Style for the main background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public Image Fill;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public Image Handle;

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var slider = GetComponent<Slider>();
			if (slider == null)
			{
				return false;
			}

			SetStyle(style.Slider);

			return true;
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="style">Style.</param>
		public virtual void SetStyle(StyleSlider style)
		{
			style.Background.ApplyTo(Background);
			style.Fill.ApplyTo(Fill);
			style.Handle.ApplyTo(Handle);
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var slider = GetComponent<Slider>();
			if (slider == null)
			{
				return false;
			}

			GetStyle(style.Slider);

			return true;
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style.</param>
		public virtual void GetStyle(StyleSlider style)
		{
			style.Background.GetFrom(Background);
			style.Fill.GetFrom(Fill);
			style.Handle.GetFrom(Handle);
		}
	}
}