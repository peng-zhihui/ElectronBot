namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Align utilities.
	/// </summary>
	public static class UtilitiesAlign
	{
		/// <summary>
		/// Align by the left side.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Left(RectTransform rectTransform)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rectTransform.rect.width);
		}

		/// <summary>
		/// Align by the center.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Center(RectTransform rectTransform)
		{
			var parent_rt = rectTransform.parent as RectTransform;
			var inset = (parent_rt.rect.width - rectTransform.rect.width) / 2f;
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, rectTransform.rect.width);
		}

		/// <summary>
		/// Align by the right side.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Right(RectTransform rectTransform)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, rectTransform.rect.width);
		}

		/// <summary>
		/// Align by the top side.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Top(RectTransform rectTransform)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, rectTransform.rect.height);
		}

		/// <summary>
		/// Align by the middle.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Middle(RectTransform rectTransform)
		{
			var parent_rt = rectTransform.parent as RectTransform;
			var inset = (parent_rt.rect.height - rectTransform.rect.height) / 2f;
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, rectTransform.rect.height);
		}

		/// <summary>
		/// Align by the bottom side.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		public static void Bottom(RectTransform rectTransform)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, rectTransform.rect.height);
		}
	}
}