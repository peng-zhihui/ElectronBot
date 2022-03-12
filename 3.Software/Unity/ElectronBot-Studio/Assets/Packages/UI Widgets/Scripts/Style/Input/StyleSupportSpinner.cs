namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the spinner.
	/// </summary>
	public class StyleSupportSpinner : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		public Spinner Spinner;

		/// <summary>
		/// SpinnerFloat.
		/// </summary>
		[SerializeField]
		public SpinnerFloat SpinnerFloat;

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (Spinner != null)
			{
				Spinner.SetStyle(style);
			}

			if (SpinnerFloat != null)
			{
				SpinnerFloat.SetStyle(style);
			}

			style.Spinner.Background.ApplyTo(Background);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (Spinner != null)
			{
				Spinner.GetStyle(style);
			}

			if (SpinnerFloat != null)
			{
				SpinnerFloat.GetStyle(style);
			}

			style.Spinner.Background.GetFrom(Background);

			return true;
		}

		#endregion
	}
}