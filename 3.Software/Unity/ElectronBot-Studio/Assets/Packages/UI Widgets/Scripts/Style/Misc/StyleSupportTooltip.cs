namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the tooltip.
	/// </summary>
	public class StyleSupportTooltip : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public GameObject Text;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Tooltip.Background.ApplyTo(Background);
			style.Tooltip.Text.ApplyTo(Text);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Tooltip.Background.GetFrom(Background);
			style.Tooltip.Text.GetFrom(Text);

			return true;
		}
		#endregion
	}
}