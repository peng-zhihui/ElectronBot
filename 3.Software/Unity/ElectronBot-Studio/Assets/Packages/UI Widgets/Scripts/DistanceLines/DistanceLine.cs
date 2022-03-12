namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Distance line.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DistanceLine : MonoBehaviour
	{
		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				return transform as RectTransform;
			}
		}

		/// <summary>
		/// Label to display distance.
		/// </summary>
		[SerializeField]
		public TextAdapter Label;

		/// <summary>
		/// Format.
		/// </summary>
		[SerializeField]
		public string Format = "0.0";

		/// <summary>
		/// Show distance.
		/// </summary>
		/// <param name="distance">Distance.</param>
		public virtual void ShowDistance(float distance)
		{
			if (Label != null)
			{
				Label.text = distance.ToString(Format);
			}
		}
	}
}