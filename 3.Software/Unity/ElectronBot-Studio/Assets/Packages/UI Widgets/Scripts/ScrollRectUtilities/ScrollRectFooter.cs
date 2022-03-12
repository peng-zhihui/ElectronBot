namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRectFooter.
	/// </summary>
	public class ScrollRectFooter : ScrollRectBlock
	{
		/// <inheritdoc/>
		protected override Vector2 GetPosition()
		{
			var result = ScrollRect.content.rect.size
				- (ScrollRect.content.anchoredPosition + ScrollRectTransform.rect.size);
			return result;
		}

		/// <inheritdoc/>
		protected override float RevealPosition(float rate)
		{
			if (IsHorizontal)
			{
				return Mathf.Lerp(0, MaxSize.x, rate);
			}
			else
			{
				return Mathf.Lerp(0, MaxSize.y, rate);
			}
		}

		/// <inheritdoc/>
		protected override void InitReveal()
		{
			if (DisplayType != ScrollRectHeaderType.Reveal)
			{
				return;
			}

			if (IsHorizontal)
			{
				Block.anchorMin = new Vector2(0, 0);
				Block.anchorMax = new Vector2(0, 1);
				Block.pivot = new Vector2(1, 0);
				Block.anchoredPosition = Vector2.zero;
			}
			else
			{
				Block.anchorMin = new Vector2(0, 0);
				Block.anchorMax = new Vector2(1, 0);
				Block.pivot = new Vector2(0, 1);
				Block.anchoredPosition = Vector2.zero;
			}
		}

		/// <inheritdoc/>
		protected override void InitLayout()
		{
			if (Layout == null)
			{
				return;
			}

			MarginDelta = IsHorizontal
				? Layout.MarginRight - MaxSize.x
				: Layout.MarginBottom - MaxSize.y;
		}

		/// <inheritdoc/>
		protected override void UpdateLayout(float size)
		{
		}
	}
}