namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Double carousel.
	/// Requires Graphic component on slides gameobject, it can be transparent.
	/// </summary>
	/// <typeparam name="T">Component type.</typeparam>
	public class DoubleCarousel<T> : MonoBehaviour
		where T : Component
	{
		/// <summary>
		/// Custom set direct slide state.
		/// </summary>
		public Action<T, float> CustomSetDirectSlideState;

		/// <summary>
		/// Custom set reverse slide state.
		/// </summary>
		public Action<T, float> CustomSetReverseSlideState;

		/// <summary>
		/// Paginator with direct scroll.
		/// </summary>
		[SerializeField]
		protected ScrollRectPaginator DirectPaginator;

		/// <summary>
		/// ScrollRect to reverse scroll.
		/// </summary>
		[SerializeField]
		protected ScrollRect ReverseScrollRect;

		/// <summary>
		/// Images scale.
		/// </summary>
		[SerializeField]
		public float Scale = 1.5f;

		/// <summary>
		/// Resize slides.
		/// </summary>
		[SerializeField]
		protected bool ResizeSlides = true;

		/// <summary>
		/// Current RectTransform.
		/// </summary>
		protected RectTransform CurrentRectTransform;

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <summary>
		/// Direct content.
		/// </summary>
		protected RectTransform DirectContent;

		/// <summary>
		/// DirectContent children.
		/// </summary>
		protected List<RectTransform> DirectChildren = new List<RectTransform>();

		/// <summary>
		/// DirectContent targets.
		/// </summary>
		protected List<T> DirectChildrenTargets = new List<T>();

		/// <summary>
		/// Reverse content.
		/// </summary>
		protected RectTransform ReverseContent;

		/// <summary>
		/// ReverseContent children.
		/// </summary>
		protected List<RectTransform> ReverseChildren = new List<RectTransform>();

		/// <summary>
		/// ReverseContent targets.
		/// </summary>
		protected List<T> ReverseChildrenTargets = new List<T>();

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			ReverseContent = ReverseScrollRect.content;
			DirectContent = DirectPaginator.GetScrollRect().content;

			// duplicate first and last slides
			var first = ReverseContent.GetChild(0);
			var last = ReverseContent.GetChild(ReverseContent.childCount - 1);
			Instantiate(first, ReverseContent, true);
			Instantiate(last, ReverseContent, true).SetAsFirstSibling();

			GetContainerComponents(ReverseContent, ReverseChildren, ReverseChildrenTargets);
			GetContainerComponents(DirectContent, DirectChildren, DirectChildrenTargets);

			CurrentRectTransform = transform as RectTransform;
			if (ResizeSlides)
			{
				EnsureSlidesImage(DirectContent);

				ResizeListener = Utilities.GetOrAddComponent<ResizeListener>(this);
				ResizeListener.OnResizeNextFrame.AddListener(UpdateSlidesSize);
				UpdateSlidesSize();
			}

			// init
			DirectPaginator.OnMovement.AddListener(UpdateSlidesState);
			DirectPaginator.Init();
			UpdateSlidesState(DirectPaginator.CurrentPage, 0f);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected void OnDestroy()
		{
			if (DirectPaginator != null)
			{
				DirectPaginator.OnMovement.RemoveListener(UpdateSlidesState);
			}

			if (ResizeListener != null)
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(UpdateSlidesSize);
			}
		}

		/// <summary>
		/// Get container components.
		/// </summary>
		/// <param name="container">Container.</param>
		/// <param name="children">Result children list.</param>
		/// <param name="targets">Result targets list.</param>
		protected virtual void GetContainerComponents(Transform container, List<RectTransform> children, List<T> targets)
		{
			children.Clear();
			targets.Clear();

			for (int i = 0; i < container.childCount; i++)
			{
				var child = container.GetChild(i);

				children.Add(child as RectTransform);
				targets.Add(child.GetComponent<T>());
			}
		}

		/// <summary>
		/// Ensure all slides have an Image component.
		/// </summary>
		/// <param name="container">Container.</param>
		protected virtual void EnsureSlidesImage(Transform container)
		{
			for (int i = 0; i < container.childCount; i++)
			{
				var child = container.GetChild(i);
				var g = child.GetComponent<Graphic>();
				if (g == null)
				{
					var img = child.gameObject.AddComponent<Image>();
					img.color = Color.clear;
				}
			}
		}

		/// <summary>
		/// Update slides size.
		/// </summary>
		protected virtual void UpdateSlidesSize()
		{
			SetSlidesSize(CurrentRectTransform.rect.size);
		}

		/// <summary>
		/// Set slides size.
		/// </summary>
		/// <param name="size">Size.</param>
		protected virtual void SetSlidesSize(Vector2 size)
		{
			var page = DirectPaginator.CurrentPage;
			SetSlidesSize(size, DirectContent);
			SetSlidesSize(size, ReverseContent);

			DirectPaginator.PageSizeType = PageSizeType.Fixed;
			DirectPaginator.PageSize = DirectPaginator.IsHorizontal() ? size.x : size.y;
			DirectPaginator.SetPage(page);
		}

		/// <summary>
		/// Set slides size.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="container">Container.</param>
		protected virtual void SetSlidesSize(Vector2 size, Transform container)
		{
			for (int i = 0; i < container.childCount; i++)
			{
				var slide = container.GetChild(i) as RectTransform;
				slide.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				slide.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}
		}

		/// <summary>
		/// Update slides state.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void UpdateSlidesState(int slideIndex, float ratio)
		{
			// scroll in the reverse direction
			var position = DirectPaginator.GetContentSize() - DirectPaginator.GetPosition();
			ReverseContent.anchoredPosition = DirectPaginator.IsHorizontal()
				? new Vector2(-position, ReverseContent.anchoredPosition.y)
				: new Vector2(ReverseContent.anchoredPosition.x, position);

			var clamped_index = ClampDirectSlide(slideIndex);
			var reverse_index = (slideIndex == -1)
				? ReverseContent.childCount - 1
				: ClampReverseSlide(ReverseContent.childCount - clamped_index - 2);
			SetReverseSlideState(reverse_index, ratio);
			SetReverseSlideState(ClampReverseSlide(reverse_index - 1), 1f - ratio);

			SetDirectSlideState(clamped_index, ratio);
			SetDirectSlideState(ClampDirectSlide(clamped_index + 1), 1f - ratio);
		}

		/// <summary>
		/// Clamp direct slide index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <returns>Clamped index.</returns>
		protected virtual int ClampDirectSlide(int slideIndex)
		{
			if (slideIndex < 0)
			{
				slideIndex += DirectPaginator.Pages;
			}

			if (slideIndex >= DirectPaginator.Pages)
			{
				slideIndex -= DirectPaginator.Pages;
			}

			return slideIndex;
		}

		/// <summary>
		/// Clamp reverse slide index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <returns>Clamped index.</returns>
		protected virtual int ClampReverseSlide(int slideIndex)
		{
			if (slideIndex < 0)
			{
				slideIndex += ReverseChildren.Count;
			}

			if (slideIndex >= ReverseChildren.Count)
			{
				slideIndex -= ReverseChildren.Count;
			}

			return slideIndex;
		}

		/// <summary>
		/// Set state for the slide with the specified index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void SetDirectSlideState(int slideIndex, float ratio)
		{
			if (CustomSetDirectSlideState != null)
			{
				if (slideIndex >= DirectChildren.Count)
				{
					slideIndex = DirectChildren.Count - 1;
				}

				CustomSetDirectSlideState(DirectChildrenTargets[slideIndex], ratio);
			}
		}

		/// <summary>
		/// Set state for the slide with the specified index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void SetReverseSlideState(int slideIndex, float ratio)
		{
			if (CustomSetReverseSlideState != null)
			{
				CustomSetReverseSlideState(ReverseChildrenTargets[slideIndex], ratio);
			}
			else
			{
				SetReverseSlideState(ReverseChildren[slideIndex], ratio);
			}
		}

		/// <summary>
		/// Set state for the specified slide.
		/// </summary>
		/// <param name="slide">Slide.</param>
		/// <param name="ratio">Ratio.</param>
		protected void SetReverseSlideState(RectTransform slide, float ratio)
		{
			var scale = Mathf.Lerp(1f, Scale, ratio);
			slide.localScale = new Vector3(scale, scale, scale);

			var graphic = slide.GetComponent<Graphic>();
			var color = graphic.color;
			color.a = 1f - ratio;
			graphic.color = color;
		}
	}
}