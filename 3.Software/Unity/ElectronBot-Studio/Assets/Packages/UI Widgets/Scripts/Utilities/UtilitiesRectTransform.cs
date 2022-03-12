namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// RectTransform utilities.
	/// </summary>
	public static class UtilitiesRectTransform
	{
		/// <summary>
		/// Get top left corner anchored position of the specified RectTransform.
		/// </summary>
		/// <param name="target">RectTransform.</param>
		/// <returns>Top left corner anchored position.</returns>
		public static Vector2 GetTopLeftCorner(RectTransform target)
		{
			var size = target.rect.size;
			var pos = target.anchoredPosition;
			var pivot = target.pivot;

			pos.x -= size.x * pivot.x;
			pos.y += size.y * (1f - pivot.y);

			return pos;
		}

		/// <summary>
		/// Set top left corner position of the specified RectTransform.
		/// </summary>
		/// <param name="target">RectTransform.</param>
		/// <param name="position">Top left corner position.</param>
		public static void SetTopLeftCorner(RectTransform target, Vector2 position)
		{
			var delta = position - GetTopLeftCorner(target);
			target.anchoredPosition += delta;
		}

		/// <summary>
		/// Get top left corner global position of the specified RectTransform.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Top left corner global position.</returns>
		public static Vector2 GetTopLeftCornerGlobalPosition(RectTransform target)
		{
			var size = target.rect.size;
			var pivot = target.pivot;
			var position = target.position;

			position.x -= size.x * pivot.x;
			position.y += size.y * (1f - pivot.y);

			return position;
		}

		/// <summary>
		/// Get bottom left corner global position of the specified RectTransform.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Bottom left corner global position.</returns>
		public static Vector2 GetBottomLeftCornerGlobalPosition(RectTransform target)
		{
			var size = target.rect.size;
			var pivot = target.pivot;
			var position = target.position;

			position.x -= size.x * pivot.x;
			position.y -= size.y * pivot.y;

			return position;
		}

		/// <summary>
		/// Get top right corner global position of the specified RectTransform.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Top right corner global position.</returns>
		public static Vector3 GetTopRightCornerGlobalPosition(RectTransform target)
		{
			var size = target.rect.size;
			var pivot = target.pivot;
			var position = target.position;

			position.x -= (size.x * pivot.x) - size.x;
			position.y += size.y * (1f - pivot.y);

			return position;
		}

		/// <summary>
		/// Copy RectTransform values.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		public static void CopyValues(RectTransform source, RectTransform target)
		{
			target.anchorMin = source.anchorMin;
			target.anchorMax = source.anchorMax;
			target.pivot = source.pivot;
			target.sizeDelta = source.sizeDelta;
			target.localPosition = source.localPosition;
			target.localRotation = source.localRotation;
			target.localScale = source.localScale;
		}
	}
}