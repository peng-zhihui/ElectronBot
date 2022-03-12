namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// LayoutDropIndicator.
	/// Supports only Horizontal and Vertical Layout groups.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Layout/Layout Drop Indicator")]
	public class LayoutDropIndicator : DropIndicatorBase
	{
		/// <summary>
		/// Is horizontal?
		/// </summary>
		[NonSerialized]
		protected bool IsHorizontal;

		/// <summary>
		/// Current layout group.
		/// </summary>
		[NonSerialized]
		protected HorizontalOrVerticalLayoutGroup CurrentLayoutGroup;

		/// <summary>
		/// Calculate indicator position.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="parent">Parent.</param>
		/// <returns>Position.</returns>
		protected virtual float CalculatePosition(int index, RectTransform parent)
		{
			var position = (float)(IsHorizontal ? CurrentLayoutGroup.padding.left : CurrentLayoutGroup.padding.bottom);
			var spacing = CurrentLayoutGroup.spacing;

			for (int i = 0; i < index; i++)
			{
				if (i > 0)
				{
					position += spacing;
				}

				var size = (parent.GetChild(i) as RectTransform).rect.size;
				position += IsHorizontal ? size.x : size.y;
			}

			return position;
		}

		/// <summary>
		/// Show indicator for the specified index in RectTransform.
		/// </summary>
		/// <param name="index">Position.</param>
		/// <param name="parent">Parent.</param>
		public virtual void Show(int index, RectTransform parent)
		{
			if (index == -1)
			{
				Hide();
				return;
			}

			CurrentLayoutGroup = parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
			if (CurrentLayoutGroup == null)
			{
				Debug.LogWarning(string.Format("Horizontal or Vertical Layout Group component not found on {0}", parent), this);
				return;
			}

			IsHorizontal = CurrentLayoutGroup is HorizontalLayoutGroup;

			var size = IsHorizontal ? parent.rect.height : parent.rect.width;
			var base_position = CalculatePosition(index, parent);
			var position = IsHorizontal ? new Vector2(base_position, 0f) : new Vector2(0f, -base_position);

			var rectTransform = transform as RectTransform;
			if (IsHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
			}

			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			if (IsHorizontal)
			{
				rectTransform.anchorMin = new Vector2(0f, 0.5f);
				rectTransform.anchorMax = new Vector2(0f, 0.5f);
			}
			else
			{
				rectTransform.anchorMin = new Vector2(0.5f, 1f);
				rectTransform.anchorMax = new Vector2(0.5f, 1f);
			}

			rectTransform.anchoredPosition = position;
			rectTransform.SetParent(parent, false);
			rectTransform.SetAsLastSibling();

			LayoutElement.ignoreLayout = true;

			gameObject.SetActive(true);
		}

		/// <summary>
		/// Hide indicator.
		/// </summary>
		public override void Hide()
		{
			var canvas = UtilitiesUI.FindTopmostCanvas(gameObject.transform);
			gameObject.transform.SetParent(canvas, false);

			base.Hide();
		}
	}
}